Public Class JamEngine
    Private _status As EngineStatus
    Private _cmdPointer As Integer
    Private cmds As List(Of String)
    Public callStack As Stack(Of Integer)
    Private executeThread As Threading.Thread
    Public Sub New()
        _status = EngineStatus.Halted
        _cmdPointer = 0
        cmds = New List(Of String)
        callStack = New Stack(Of Integer)
        executeThread = New Threading.Thread(AddressOf executeThreadSub)
        executeThread.Start()
    End Sub
    Public Sub addCommands(cmd As String)
        cmd = cmd.Replace(Chr(10), "").Replace(Chr(13), "")
        Dim addedCmdsCounter As Integer = 0
        For Each item In cmd.Split(";")
            While item.StartsWith(" ")
                item = item.Substring(1)
            End While
            While item.EndsWith(" ")
                item = item.Substring(0, item.Length - 1)
            End While
            If item.StartsWith("#") = False And item <> "" Then cmds.Insert(_cmdPointer + addedCmdsCounter, item) : addedCmdsCounter += 1
        Next
    End Sub
#Region "Events"
    Public Event statusChanged(ByRef sender As JamEngine, ByVal oldStatus As EngineStatus, ByVal newStatus As EngineStatus)
    Public Event writeText(ByRef sender As JamEngine, ByVal text As String, ByVal clearLines As Integer, ByVal newLine As Boolean, ByVal clear As Boolean, ByVal isError As Boolean)
    Public Event visibilityChanged(ByRef sender As JamEngine, ByVal value As Boolean)
    Public Event titleChanged(ByRef sender As JamEngine, ByVal value As String)
    Public Event directoryChanged(ByRef sender As JamEngine, ByVal value As String)
#End Region

#Region "Properties"
    Public ReadOnly Property status As EngineStatus
        Get
            Return _status
        End Get
    End Property
    Public Property cmdPointer As Integer
        Get
            Return _cmdPointer
        End Get
        Set(value As Integer)
            _cmdPointer = value
        End Set
    End Property
    Public ReadOnly Property commandCounts As Integer
        Get
            Return cmds.Count
        End Get
    End Property
    Private WriteOnly Property setStatus As EngineStatus
        Set(value As EngineStatus)
            If _status <> value Then
                Dim temp As EngineStatus = _status
                _status = value
                RaiseEvent StatusChanged(Me, temp, value)
            End If
        End Set
    End Property
#End Region
    Public Sub startEngine()
        setStatus = EngineStatus.Resuming
    End Sub
    Public Sub stopEngine()
        setStatus = EngineStatus.Stopping
    End Sub
    Public Enum EngineStatus
        Finished = 0 'Progammcode is fully processed
        Running = 1 'Programmcode is still running
        Resuming = 2 'Engine should switch to running
        Halted = 3 'Programcode is not finished, but should halt
        HaltedBecauseError = 4 'Programcode is not finished, but should halt cause of an error
        HaltedBecauseWarning = 5 'Programcode is not finished, but should halt cause of a warning
        Stopping = 6 'Engine should stop thread
        Stopped = 7 ' Engine has stoped
    End Enum
    Public Enum cmdErrorCode
        Success = 0
        Failed = 1
        SyntaxError = 100
        ToMannyParameter = 101
        NotEnoughParameter = 102
        WrongType = 103
        IOError = 200
        DirectoryDosNotExist = 201
        FileDosNotExist = 202
    End Enum
    Public Class cmd
        Public command As String
        Public parameters As List(Of String)
        Public Sub New()
            parameters = New List(Of String)
        End Sub
        Public Sub New(rawCmd As String)
            Me.New
            pars(rawCmd)
        End Sub
        Public Sub pars(rawCmd As String)
            If rawCmd.StartsWith("#") Then
                command = "#"
                parameters.Clear()
            Else
                If rawCmd.Contains(" ") = False Then
                    command = rawCmd.ToLower
                    parameters.Clear()
                Else
                    command = rawCmd.Split(" ")(0).ToLower
                    parameters.Clear()
                    For Each item In rawCmd.Substring(command.Length).Split("|")
                        While item.StartsWith(" ")
                            item = item.Substring(1)
                        End While
                        While item.EndsWith(" ")
                            item = item.Substring(0, item.Length - 1)
                        End While
                        'At this Point we need to expand Vars
                        parameters.Add(item)
                    Next
                End If
            End If
        End Sub
    End Class
    Public Class cmdError
        Private _errorCode As cmdErrorCode
        Private _errorMessage As String
        Private _isFatal As Boolean
        Public Sub New(errorMessage As String, errorCode As cmdErrorCode, isFatal As Boolean)
            _errorCode = errorCode
            _errorMessage = errorMessage
            _isFatal = isFatal
        End Sub
        Public Sub New()
            _errorCode = cmdErrorCode.Success
            _errorMessage = ""
            _isFatal = False
        End Sub
        Public ReadOnly Property errorCode As cmdErrorCode
            Get
                Return _errorCode
            End Get
        End Property
        Public ReadOnly Property errorMessage As String
            Get
                Return _errorMessage
            End Get
        End Property
        Public ReadOnly Property isFatal As Boolean
            Get
                Return _isFatal
            End Get
        End Property
    End Class
    Private Sub executeThreadSub()
        Dim command As New cmd
        Dim returnVal As New cmdError
        While _status <> EngineStatus.Stopping
            Select Case _status
                Case EngineStatus.Resuming
                    setStatus = EngineStatus.Running
            End Select
            While _status = EngineStatus.Running
                If _cmdPointer >= cmds.Count Then setStatus = EngineStatus.Finished : Exit While
                command.pars(cmds(_cmdPointer))
                returnVal = New cmdError("Nothing happens", 0, False)
                If command.command.StartsWith(":") Or command.command.ToLower.StartsWith("sub:") Then
                    returnVal = New cmdError("Allright, Jumppoint", 0, False)
                Else
                    Select Case command.command
                        Case "sleep", "wait"
                            returnVal = cmdSleep(command.parameters)
                        Case "message", "echo", "write"
                            returnVal = cmdMessage(command.parameters)
                        Case "title"
                            returnVal = cmdTitle(command.parameters)
                        Case "visible"
                            returnVal = cmdVisible(command.parameters)
                        Case "clear", "cls"
                            returnVal = cmdClear()
                        Case "exit"
                            returnVal = cmdExit(command.parameters)
                        Case "deldir", "rmdir"
                            returnVal = cmdDelDir(command.parameters)
                        Case "copydir", "cpdir"
                            returnVal = cmdCopyDir(command.parameters)
                        Case "movedir", "mvdir"
                            returnVal = cmdMoveDir(command.parameters)
                        Case "makedir", "mkdir"
                            returnVal = cmdMkDir(command.parameters)
                        Case "delfile", "rmfile"
                            returnVal = cmdDelFile(command.parameters)
                        Case "copyfile", "cpfile"
                            returnVal = cmdCopyFile(command.parameters)
                        Case "movefile", "mvfile"
                            returnVal = cmdMoveFile(command.parameters)
                        Case "makefile", "mkfile"
                            returnVal = cmdMkFile(command.parameters)
                        Case "cd"
                            returnVal = cmdCd(command.parameters)
                        Case "goto"
                            returnVal = cmdGoto(command.parameters)
                        Case "iffileexist"
                            returnVal = cmdIfFileExist(command.parameters)
                        Case "ifdirexist"
                            returnVal = cmdIfDirExist(command.parameters)
                        Case "include"
                            returnVal = cmdInclude(command.parameters)
                            _cmdPointer -= 1 'Set cmdPointer back
                        Case Else
                            returnVal = New cmdError("Command not known", cmdErrorCode.SyntaxError, True)
                    End Select
                End If
                _cmdPointer += 1
                If returnVal.errorCode <> cmdErrorCode.Success Then RaiseEvent writeText(Me, returnVal.errorMessage, 0, True, False, True)
            End While
            Threading.Thread.Sleep(250)
        End While
        setStatus = EngineStatus.Stopped
    End Sub

    Public Function doJump(jumpmark As String) As Boolean
        Dim isSub As Boolean = False
        jumpmark = jumpmark.ToLower
        If jumpmark.StartsWith("sub:") Then jumpmark = jumpmark.Substring(3) : isSub = True
        For i As Integer = 0 To cmds.Count - 1
            If cmds(i) = jumpmark Then
                If isSub Then callStack.Push(_cmdPointer)
                _cmdPointer = i
                Return True
            End If
        Next
        Return False
    End Function


#Region "Commands"
    Private Function cmdMessage(parameters As List(Of String)) As cmdError
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        RaiseEvent writeText(Me, parameters(0), 0, True, False, False)
        Return New cmdError()
    End Function
    Private Function cmdSleep(parameters As List(Of String)) As cmdError
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        If IsNumeric(parameters(0)) = False Then Return New cmdError("No number as parameter", cmdErrorCode.WrongType, True)
        Threading.Thread.Sleep(parameters(0) * 1000)
        Return New cmdError()
    End Function
    Private Function cmdDelDir(parameters As List(Of String)) As cmdError
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        Try
            If My.Computer.FileSystem.DirectoryExists(parameters(0)) Then
                My.Computer.FileSystem.DeleteDirectory(parameters(0), FileIO.DeleteDirectoryOption.DeleteAllContents)
            Else
                Return New cmdError("Directory dose not exist", cmdErrorCode.DirectoryDosNotExist, False)
            End If
        Catch ex As Exception
            Return New cmdError("Error while deleting directory", cmdErrorCode.Failed, True)
        End Try
        Return New cmdError()
    End Function
    Private Function cmdCopyDir(parameters As List(Of String)) As cmdError
        If parameters.Count < 2 Then Return New cmdError("Command has not enough parameters", cmdErrorCode.NotEnoughParameter, True)
        Try
            If My.Computer.FileSystem.DirectoryExists(parameters(0)) Then
                My.Computer.FileSystem.CopyDirectory(parameters(0), parameters(1))
            Else
                Return New cmdError("Directory dose not exist", cmdErrorCode.DirectoryDosNotExist, False)
            End If
        Catch ex As Exception
            Return New cmdError("Error while copying directory", cmdErrorCode.Failed, True)
        End Try
        Return New cmdError()
    End Function
    Private Function cmdMoveDir(parameters As List(Of String)) As cmdError
        If parameters.Count < 2 Then Return New cmdError("Command has not enough parameters", cmdErrorCode.NotEnoughParameter, True)
        Try
            If My.Computer.FileSystem.DirectoryExists(parameters(0)) Then
                My.Computer.FileSystem.MoveDirectory(parameters(0), parameters(1))
            Else
                Return New cmdError("Directory dose not exist", cmdErrorCode.DirectoryDosNotExist, False)
            End If
        Catch ex As Exception
            Return New cmdError("Error while moving directory", cmdErrorCode.Failed, True)
        End Try
        Return New cmdError()
    End Function
    Private Function cmdMkDir(parameters As List(Of String)) As cmdError
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        Try
            If My.Computer.FileSystem.DirectoryExists(parameters(0)) Then
                Return New cmdError("Directory already exist", cmdErrorCode.IOError, False)
            Else
                My.Computer.FileSystem.CreateDirectory(parameters(0))
            End If
        Catch ex As Exception
            Return New cmdError("Error while creating directory", cmdErrorCode.Failed, True)
        End Try
        Return New cmdError()
    End Function
    Private Function cmdDelFile(parameters As List(Of String)) As cmdError
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        Try
            If My.Computer.FileSystem.FileExists(parameters(0)) Then
                My.Computer.FileSystem.DeleteFile(parameters(0))
            Else
                Return New cmdError("File dose not exist", cmdErrorCode.FileDosNotExist, False)
            End If
        Catch ex As Exception
            Return New cmdError("Error while deleting file", cmdErrorCode.Failed, True)
        End Try
        Return New cmdError()
    End Function
    Private Function cmdCopyFile(parameters As List(Of String)) As cmdError
        If parameters.Count < 2 Then Return New cmdError("Command has not enough parameters", cmdErrorCode.NotEnoughParameter, True)
        Try
            If My.Computer.FileSystem.FileExists(parameters(0)) Then
                My.Computer.FileSystem.CopyFile(parameters(0), parameters(1))
            Else
                Return New cmdError("File dose not exist", cmdErrorCode.FileDosNotExist, False)
            End If
        Catch ex As Exception
            Return New cmdError("Error while copying file", cmdErrorCode.Failed, True)
        End Try
        Return New cmdError()
    End Function
    Private Function cmdMoveFile(parameters As List(Of String)) As cmdError
        If parameters.Count < 2 Then Return New cmdError("Command has not enough parameters", cmdErrorCode.NotEnoughParameter, True)
        Try
            If My.Computer.FileSystem.FileExists(parameters(0)) Then
                My.Computer.FileSystem.MoveFile(parameters(0), parameters(1))
            Else
                Return New cmdError("File dose not exist", cmdErrorCode.FileDosNotExist, False)
            End If
        Catch ex As Exception
            Return New cmdError("Error while moving file", cmdErrorCode.Failed, True)
        End Try
        Return New cmdError()
    End Function
    Private Function cmdMkFile(parameters As List(Of String)) As cmdError
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        Try
            If My.Computer.FileSystem.FileExists(parameters(0)) Then
                Return New cmdError("File already exist", cmdErrorCode.IOError, False)
            Else
                My.Computer.FileSystem.WriteAllText(parameters(0), "", False)
            End If
        Catch ex As Exception
            Return New cmdError("Error while creating file", cmdErrorCode.Failed, True)
        End Try
        Return New cmdError()
    End Function
    Private Function cmdCd(parameters As List(Of String)) As cmdError
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        Try
            If My.Computer.FileSystem.DirectoryExists(parameters(0)) Then
                RaiseEvent directoryChanged(Me, parameters(0))
            Else
                Return New cmdError("Directory dose not exist", cmdErrorCode.DirectoryDosNotExist, True)
            End If
        Catch ex As Exception
            Return New cmdError("Error while changing directory", cmdErrorCode.Failed, True)
        End Try
        Return New cmdError()
    End Function
    Private Function cmdGoto(parameters As List(Of String)) As cmdError
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        If doJump(parameters(0)) = False Then Return New cmdError("No matching jumpoint found", cmdErrorCode.Failed, True)
        Return New cmdError("Jump successfully", 0, False)
    End Function
    Private Function cmdTitle(parameters As List(Of String)) As cmdError
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        RaiseEvent titleChanged(Me, parameters(0))
        Return New cmdError()
    End Function
    Private Function cmdVisible(parameters As List(Of String)) As cmdError
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        If parameters(0).ToLower = "true" Or parameters(0) = 1 Then
            RaiseEvent visibilityChanged(Me, True)
        ElseIf parameters(0).ToLower = "false" Or parameters(0) = 0 Then
            RaiseEvent visibilityChanged(Me, False)
        Else
            Return New cmdError("Wrong Parameter", cmdErrorCode.WrongType, True)
        End If
        Return New cmdError()
    End Function
    Private Function cmdClear() As cmdError
        RaiseEvent writeText(Me, "", 0, False, True, False)
        Return New cmdError()
    End Function
    Private Function cmdExit(parameters As List(Of String)) As cmdError
        If callStack.Count = 0 Then
            _status = EngineStatus.Stopping
        Else
            _cmdPointer = callStack.Pop
        End If
        Return New cmdError()
    End Function
    Private Function cmdIfFileExist(parameters As List(Of String)) As cmdError
        If parameters.Count < 2 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        Try
            If My.Computer.FileSystem.FileExists(parameters(0)) Then
                If doJump(parameters(1)) = False Then Return New cmdError("No matching jumpoint found", cmdErrorCode.Failed, True) Else Return New cmdError("File Exist", 0, False)
            Else
                If parameters.Count > 2 Then
                    If doJump(parameters(2)) = False Then
                        Return New cmdError("No matching jumpoint found", cmdErrorCode.Failed, True)
                    Else
                        Return New cmdError("File not exist", 0, False)
                    End If
                Else
                    Return New cmdError()
                End If
            End If
        Catch ex As Exception
            Return New cmdError("Something went wrong while checking file existence", cmdErrorCode.IOError, True)
        End Try
    End Function
    Private Function cmdIfDirExist(parameters As List(Of String)) As cmdError
        If parameters.Count < 2 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        Try
            If My.Computer.FileSystem.DirectoryExists(parameters(0)) Then
                If doJump(parameters(1)) = False Then Return New cmdError("No matching jumpoint found", cmdErrorCode.Failed, True) Else Return New cmdError("Directory Exist", 0, False)
            Else
                If parameters.Count > 2 Then
                    If doJump(parameters(2)) = False Then
                        Return New cmdError("No matching jumpoint found", cmdErrorCode.Failed, True)
                    Else
                        Return New cmdError("Directory not exist", 0, False)
                    End If
                Else
                    Return New cmdError()
                End If
            End If
        Catch ex As Exception
            Return New cmdError("Something went wrong while checking directory existence", cmdErrorCode.IOError, True)
        End Try
    End Function

    Private Function cmdInclude(parameters As List(Of String)) As cmdError
        cmds.RemoveAt(_cmdPointer) 'Remove Include out of commands
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        If My.Computer.FileSystem.FileExists(parameters(0)) Then
            Try
                addCommands(My.Computer.FileSystem.ReadAllText(parameters(0)))
                Return New cmdError()
            Catch ex As Exception
                Return New cmdError("Error while loading file", cmdErrorCode.IOError, True)
            End Try
        Else
            Return New cmdError("File to include does not exist", cmdErrorCode.FileDosNotExist, True)
        End If
    End Function
#End Region
End Class
