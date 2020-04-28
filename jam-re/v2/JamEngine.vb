Public Class JamEngine
    Private _status As EngineStatus
    Private _cmdPointer As Integer
    Private cmds As List(Of String)
    Private executeThread As Threading.Thread
    Public Sub New()
        _status = EngineStatus.Halted
        _cmdPointer = 0
        cmds = New List(Of String)
        executeThread = New Threading.Thread(AddressOf executeThreadSub)
        executeThread.Start()
    End Sub
    Public Sub addCommands(cmd As String)
        cmd = cmd.Replace(Chr(10), "").Replace(Chr(13), "")
        For Each item In cmd.Split(";")
            While item.StartsWith(" ")
                item = item.Substring(1)
            End While
            While item.EndsWith(" ")
                item = item.Substring(0, item.Length - 1)
            End While
            If item.StartsWith("#") = False And item <> "" Then cmds.Add(item)
        Next
    End Sub
#Region "Events"
    Public Event StatusChanged(ByRef sender As JamEngine, ByVal oldStatus As EngineStatus, ByVal newStatus As EngineStatus)
    Public Event writeText(ByRef sender As JamEngine, ByVal text As String, ByVal clearLines As Integer)
    Public Event changeVisibility(ByRef sender As JamEngine, ByVal value As Boolean)
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
        While _status <> EngineStatus.Stopping
            Select Case _status
                Case EngineStatus.Resuming
                    setStatus = EngineStatus.Running
            End Select
            While _status = EngineStatus.Running
                If _cmdPointer >= cmds.Count Then setStatus = EngineStatus.Finished : Exit While
                command.pars(cmds(_cmdPointer))
                Select Case command.command
                    Case "#"
                        'Do nothing. It's a comment
                    Case "sleep", "wait"
                        cmdSleep(command.parameters)
                    Case "message", "echo", "write"
                        cmdMessage(command.parameters)
                    Case "visible"
                        cmdVisible(command.parameters)
                    Case "exit"
                        cmdExit(command.parameters)
                End Select

                _cmdPointer += 1
            End While
            Threading.Thread.Sleep(250)
        End While
        setStatus = EngineStatus.Stopped
    End Sub


#Region "Commands"
    Private Function cmdMessage(parameters As List(Of String)) As cmdError
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        RaiseEvent writeText(Me, parameters(0), 0)
        Return New cmdError("", 0, False)
    End Function
    Private Function cmdSleep(parameters As List(Of String)) As cmdError
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        If IsNumeric(parameters(0)) = False Then Return New cmdError("No number as parameter", cmdErrorCode.WrongType, True)
        Threading.Thread.Sleep(parameters(0) * 1000)
        Return New cmdError("", 0, False)
    End Function
    Private Function cmdVisible(parameters As List(Of String)) As cmdError
        If parameters.Count < 1 Then Return New cmdError("Command has no parameters", cmdErrorCode.NotEnoughParameter, True)
        If parameters(0).ToLower = "true" Or parameters(0) = 1 Then
            RaiseEvent changeVisibility(Me, True)
        ElseIf parameters(0).ToLower = "false" Or parameters(0) = 0 Then
            RaiseEvent changeVisibility(Me, False)
        Else
            Return New cmdError("Wrong Parameter", cmdErrorCode.WrongType, True)
        End If
        Return New cmdError("", 0, False)
    End Function
    Private Function cmdExit(parameters As List(Of String))
        _status = EngineStatus.Stopping
        Return New cmdError("", 0, False)
    End Function
#End Region
End Class
