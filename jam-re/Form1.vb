Imports System.Drawing.Drawing2D
Public Class Form1
    Dim ready As Boolean = False
    Dim version As String = My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Build
    Public commands As New List(Of String)
    Dim Script As String = ""
    Dim includes As Byte = 0

    Dim openThread As New Threading.Thread(AddressOf openForm)
    Private Sub Form1_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        CheckForIllegalCrossThreadCalls = False
        Label1.Text = "Version: " & version

        Dim exe As New Threading.Thread(AddressOf executeThread)
        exe.Start()
        openThread.Start()
    End Sub
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Me.Location = New Point(Me.Location.X + (Me.Width / 2), Me.Location.Y + (Me.Height / 2))
        Me.Width = 2
        Me.Height = 2
    End Sub

    Protected Overrides Sub onpaint(ByVal a As System.Windows.Forms.PaintEventArgs)
        ' Formgröße
        Dim größe As New Rectangle(0, 0, Me.Width, Me.Height)
        Using lgb As New LinearGradientBrush(größe, Color.LightGray, Color.DarkGray, LinearGradientMode.BackwardDiagonal)
            ' anzeigen
            a.Graphics.FillRectangle(lgb, größe)
        End Using
    End Sub

    Sub openForm()
        '526; 265
        Do While Me.Width < 526
            Me.Width += 4
            Me.Location = New Point(Me.Location.X - 2, Me.Location.Y)

            'Threading.Thread.Sleep(1)
        Loop
        Do While Me.Height < 265
            Me.Height += 4
            Me.Location = New Point(Me.Location.X, Me.Location.Y - 2)

            'Threading.Thread.Sleep(1)
        Loop
        RichTextBox1.ScrollToCaret()
        Application.DoEvents()
    End Sub

    Sub closeForm()
        '526; 265
        Do While Me.Height > 2
            Me.Height -= 4
            Me.Location = New Point(Me.Location.X, Me.Location.Y + 2)

            'Threading.Thread.Sleep(1)
        Loop
        Do While Me.Width > 2
            Me.Width -= 4
            Me.Location = New Point(Me.Location.X + 2, Me.Location.Y)

            'Threading.Thread.Sleep(1)
        Loop
        Application.DoEvents()
    End Sub

    Sub executeThread()
        If loadScript() = True Then
            Execute()
        End If
        ende()
    End Sub
    Function loadScript() As Boolean
        If Environment.GetCommandLineArgs.Count <= 1 Then
            RichTextBox1.AppendText(vbCrLf & "Es wurde keine Datei übergeben!")
            ende()
            Return False
        Else
            Return loadScripts(Environment.GetCommandLineArgs(1))
        End If
    End Function
    Function loadScripts(file As String) As Boolean
        If includes < 255 Then
            includes += 1
        Else
            RichTextBox1.AppendText("Über 255 Includes. Vermutlich ein Loop. Daher werden keine weiteren Includes gestattet.")
            writeErrorLog("Über 255 Includes. Vermutlich ein Loop. Daher werden keine weiteren Includes gestattet.")
            Return False
        End If
        Script = ""
        Try
            Using sr As System.IO.StreamReader = New System.IO.StreamReader(file, System.Text.Encoding.Default)
                Script = sr.ReadToEnd.ToString
                sr.Close()
            End Using
            Script = Script.Replace(Chr(10), "").Replace(Chr(13), "").Replace(vbTab, "")

            'Prüfen ob weitere Parameter übergeben wurde
            If Environment.GetCommandLineArgs.Count >= 3 Then
                For i As Integer = 2 To Environment.GetCommandLineArgs.Count - 1
                    Script = Script.Replace("%" & i - 1 & "%", Environment.GetCommandLineArgs(i))
                Next
            End If
            'Ersetze weitere Variablen
            Script = Script.Replace("%_br%", Chr(13) & Chr(10))
            Script = Environment.ExpandEnvironmentVariables(Script)


            For Each item As String In Script.Split(";")
                If item.ToLower.StartsWith("include ") Then
                    Try
                        loadScripts(item.Substring(8))
                    Catch ex As Exception
                        RichTextBox1.AppendText("Fehler beim Include von " & item & vbCrLf & ex.ToString)
                    End Try
                Else
                    commands.Add(item)
                End If
            Next
            Return True
        Catch ex As Exception
            RichTextBox1.AppendText("Fehler beim Laden der Skriptdatei aufgetreten!" & vbCrLf & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function
    Sub Execute(Optional commandPointer As Integer = 0)
        Try
            For commandPointer = commandPointer To commands.Count - 1
                Dim tempString As String = ""
                For Each tempChar As Char In commands(commandPointer)
                    If tempChar <> " " Then
                        tempString &= tempChar
                    Else
                        Exit For
                    End If
                Next
                Dim returnValue As Integer = -1
                If commands(commandPointer).Length <= tempString.Length + 1 Then
                    returnValue = CommandSelect(replaceVar(tempString))
                Else
                    returnValue = CommandSelect(replaceVar(tempString), replaceVar(commands(commandPointer).Substring(tempString.Length + 1)))
                End If
                If returnValue > -1 Then
                    commandPointer = returnValue
                ElseIf returnValue = -2 Then
                    Exit Sub
                End If
            Next
        Catch ex As Exception
            RichTextBox1.AppendText("Fehler beim Ausführen des Scripts ist aufgetreten!" & vbCrLf & vbCrLf & ex.ToString)
        End Try
    End Sub

    Sub ende()
        openThread.Abort()
        closeForm()
        ready = True
        Me.Close()
    End Sub

    Private Sub RichTextBox1_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles RichTextBox1.TextChanged
        RichTextBox1.SelectionStart = RichTextBox1.TextLength
        RichTextBox1.ScrollToCaret()
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If ready = False Then
            e.Cancel = True
        End If
    End Sub

    Public Function jumpChecker(lable As String) As Integer
        Dim temp As String = lable.ToLower
        If temp.StartsWith("sub:") Then
            CmdGoSub(lable.Substring(4))
            Return -1
        ElseIf temp.StartsWith("goto:") Then
            Return CmdGoto(lable.Substring(5))
        Else
            Return CmdGoto(lable)
        End If
    End Function

    Public Function CommandSelect(ByVal command As String, Optional ByVal parameter As String = "") 'Deklariert alle Befehle
        Try
            Dim tempCommand As String = command.ToLower
            Select Case tempCommand
                Case "#"
                    'Hierbei handelt es sich um ein Kommentar. Es wird nichts unternommen!
                Case "message"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdMessage(parameter)
                Case "sleep", "wait"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdSleep(parameter)
                Case "deldir"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdDelDir(parameter)
                Case "copydir"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdCopyDir(parameter)
                Case "delfile"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdDelFile(parameter)
                Case "copyfile"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdCopyFile(parameter)
                Case "movedir"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdMoveDir(parameter)
                Case "movefile"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdMoveFile(parameter)
                Case "start"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdStart(parameter)
                Case "startwait"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdStartWait(parameter)
                Case "title"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdTitle(parameter)
                Case "exit", "next"
                    writeCommandInfoLog(tempCommand, "")
                    Return -2
                Case "goto"
                    writeCommandInfoLog(tempCommand, parameter)
                    Return CmdGoto(parameter)
                Case "ifdirexist"
                    writeCommandInfoLog(tempCommand, parameter)
                    Return CmdDirExist(parameter)
                Case "iffileexist"
                    writeCommandInfoLog(tempCommand, parameter)
                    Return CmdFileExist(parameter)
                Case "shell"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdShell(parameter)
                Case "visible"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdVisible(parameter)
                Case "writefile"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdWriteFile(parameter, False)
                Case "writefileappend"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdWriteFile(parameter, True)
                Case "mkdir"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdMkDir(parameter)
                Case "iftaskexist"
                    writeCommandInfoLog(tempCommand, parameter)
                    Return CmdIfTaskExist(parameter)
                Case "taskkill"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdTaskKill(parameter)
                Case "taskclose"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdTaskClose(parameter)
                Case "gosub"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdGoSub(parameter)
                Case "wget"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdWget(parameter)
                Case "log"
                    CmdLog(parameter)
                Case "set"
                    CmdSet(parameter)
                Case "readfile"
                    CmdReadFile(parameter)
                Case "ifstringequal"
                    Return CmdIfStringEqual(parameter)
                Case "ifstringcontain"
                    Return CmdIfStringContain(parameter)
                Case "calculate"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdCalculate(parameter)
                Case "substring"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdSubstring(parameter)
                Case "replacestring"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdReplaceString(parameter)
                Case "setregvalue"
                    writeCommandInfoLog(tempCommand, parameter)
                    CmdSetRegValue(parameter)
                Case "getregvalue"
                    writeCommandInfoLog(tempCommand, parameter)
                    cmdGetRegValue(parameter)
                Case "createregkey"
                    writeCommandInfoLog(tempCommand, parameter)
                    cmdCreateRegKey(parameter)
                Case "delregkey"
                    writeCommandInfoLog(tempCommand, parameter)
                    cmdDelRegKey(parameter)
                Case "delregvalue"
                    writeCommandInfoLog(tempCommand, parameter)
                    cmdDelRegValue(parameter)
                Case "ifpingsuccessfull"
                    writeCommandInfoLog(tempCommand, parameter)
                    Return cmdIfPingSuccessfull(parameter)
                Case "cls", "clear"
                    writeCommandInfoLog(tempCommand, parameter)
                    cmdCls()
                Case "if"
                    writeCommandInfoLog(tempCommand, parameter)
                    Return cmdIf(parameter)
                Case Else
                    If tempCommand.StartsWith(":") = True Then
                        writeInfoLog("Lable " & tempCommand.Substring(1) & " erreicht.")
                    Else
                        writeCommandErrorLog(command, parameter, "Befehl unbekannt!")
                    End If
            End Select
        Catch ex As Exception
            writeCommandErrorLog(command, parameter, ex.ToString)
            RichTextBox1.AppendText("Fehler beim Ausführen von Command: " & command & " mit dem Parameter: " & parameter & " aufgetreten!" & vbCrLf & vbCrLf & ex.ToString)
        End Try
        Return -1
    End Function

    Public Sub CmdSleep(ByVal parameter As String)
        Threading.Thread.Sleep((parameter * 1000))
    End Sub
    Public Sub CmdMessage(ByVal parameter As String)
        RichTextBox1.AppendText(parameter & vbCrLf)
    End Sub
    Public Sub CmdDelDir(ByVal parameter As String)
        If My.Computer.FileSystem.DirectoryExists(parameter) Then
            My.Computer.FileSystem.DeleteDirectory(parameter, FileIO.DeleteDirectoryOption.DeleteAllContents)
        Else
            writeWarningLog("Ordner " & parameter & " existiert nicht.")
        End If
    End Sub
    Public Sub CmdCopyDir(ByVal parameter As String)
        parameter = parameter.Replace(" >", ">").Replace("> ", ">")
        If parameter.Split(">").Count = 2 Then
            Dim tempList As New List(Of String)
            For Each item In parameter.Split(">")
                tempList.Add(item)
            Next
            If My.Computer.FileSystem.DirectoryExists(tempList(0)) Then
                My.Computer.FileSystem.CopyDirectory(tempList(0), tempList(1), True)
            Else
                writeErrorLog("Ordner " & tempList(0) & " ist nicht vorhanden.")
            End If
        Else
            writeErrorLog("Syntaxfehler in Befehl CopyDir mit dem Parameter " & parameter)
        End If
    End Sub
    Public Sub CmdDelFile(ByVal parameter As String)
        If My.Computer.FileSystem.FileExists(parameter) Then
            My.Computer.FileSystem.DeleteFile(parameter)
        Else
            writeWarningLog("Datei " & parameter & " existiert nicht.")
        End If
    End Sub
    Public Sub CmdCopyFile(ByVal parameter As String)
        parameter = parameter.Replace(" >", ">").Replace("> ", ">")
        If parameter.Split(">").Count = 2 Then
            Dim tempList As New List(Of String)
            For Each item In parameter.Split(">")
                tempList.Add(item)
            Next
            If My.Computer.FileSystem.FileExists(tempList(0)) Then
                My.Computer.FileSystem.CopyFile(tempList(0), tempList(1), True)
            Else
                writeErrorLog("Datei " & tempList(0) & " ist nicht vorhanden.")
            End If
        Else
            writeErrorLog("Syntaxfehler in Befehl CopyFile mit dem Parameter " & parameter)
        End If
    End Sub
    Public Sub CmdMoveDir(ByVal parameter As String)
        parameter = parameter.Replace(" >", ">").Replace("> ", ">")
        If parameter.Split(">").Count = 2 Then
            Dim tempList As New List(Of String)
            For Each item In parameter.Split(">")
                tempList.Add(item)
            Next
            If My.Computer.FileSystem.DirectoryExists(tempList(0)) Then
                My.Computer.FileSystem.MoveDirectory(tempList(0), tempList(1), FileIO.UIOption.AllDialogs, FileIO.UICancelOption.DoNothing)
            Else
                writeErrorLog("Ordner " & tempList(0) & " ist nicht vorhanden.")
            End If
        Else
            writeErrorLog("Syntaxfehler in Befehl MoveDir mit dem Parameter " & parameter)
        End If
    End Sub
    Public Sub CmdMoveFile(ByVal parameter As String)
        parameter = parameter.Replace(" >", ">").Replace("> ", ">")
        If parameter.Split(">").Count = 2 Then
            Dim tempList As New List(Of String)
            For Each item In parameter.Split(">")
                tempList.Add(item)
            Next
            If My.Computer.FileSystem.FileExists(tempList(0)) Then
                My.Computer.FileSystem.MoveFile(tempList(0), tempList(1), FileIO.UIOption.AllDialogs, FileIO.UICancelOption.DoNothing)
            Else
                writeErrorLog("Datei " & tempList(0) & " ist nicht vorhanden.")
            End If
        Else
            writeErrorLog("Syntaxfehler in Befehl MoveFile mit dem Parameter " & parameter)
        End If
    End Sub
    Public Sub CmdStart(ByVal parameter As String)
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        If parameter.Split("|").Count = 2 Then
            Dim tempList As New List(Of String)
            For Each item In parameter.Split("|")
                tempList.Add(item)
            Next
            Process.Start(tempList(0), tempList(1))
        Else
            Process.Start(parameter)
        End If
    End Sub
    Public Sub CmdStartWait(ByVal parameter As String)
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        If parameter.Split("|").Count = 2 Then
            Dim tempList As New List(Of String)
            For Each item In parameter.Split("|")
                tempList.Add(item)
            Next
            Process.Start(tempList(0), tempList(1)).WaitForExit()
        Else
            Process.Start(parameter).WaitForExit()
        End If
    End Sub
    Public Sub CmdTitle(ByVal parameter As String)
        Label2.Text = parameter
        Me.Text = parameter
    End Sub
    Public Function CmdGoto(ByVal parameter As String) As Integer
        For i As Integer = 0 To commands.Count - 1
            If commands(i).ToLower = ":" & parameter.ToLower Then
                Return i
            End If
        Next
        writeWarningLog("Lable " & parameter & " wurde nicht gefunden.")
        Return -1
    End Function
    Public Function CmdDirExist(ByVal parameter As String) As Integer
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 2 Or splitedParameter.Count = 3 Then
            If My.Computer.FileSystem.DirectoryExists(splitedParameter(0)) Then
                Return jumpChecker(splitedParameter(1))
            Else
                If splitedParameter.Count = 3 Then
                    Return jumpChecker(splitedParameter(2))
                End If
            End If
        Else
            writeErrorLog("Syntaxfehler in Befehl IfDirExist mit dem Parameter " & parameter)
        End If
        Return -1
    End Function
    Public Function CmdFileExist(ByVal parameter As String) As Integer
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 2 Or splitedParameter.Count = 3 Then
            If My.Computer.FileSystem.FileExists(splitedParameter(0)) Then
                Return jumpChecker(splitedParameter(1))
            Else
                If splitedParameter.Count = 3 Then
                    Return jumpChecker(splitedParameter(2))
                End If
            End If
        Else
            writeErrorLog("Syntaxfehler in Befehl IfFileExist mit dem Parameter " & parameter)
        End If
        Return -1
    End Function
    Public Sub CmdShell(ByVal parameter As String)
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim tempString = parameter.ToLower
        Dim WinStyle As Byte = 0 '0 = Normal 1 = hidden 2 = minimized 3 = maximized
        Dim Focus As Boolean = True
        Dim waitForExit As Boolean = False

        If tempString.Contains("winstyle:normal|") Then
            WinStyle = 0
            tempString = tempString.Replace("winstyle:normal|", "")
        End If
        If tempString.Contains("winstyle:hidden|") Then
            WinStyle = 1
            tempString = tempString.Replace("winstyle:hidden|", "")
        End If
        If tempString.Contains("winstyle:minimized|") Then
            WinStyle = 2
            tempString = tempString.Replace("winstyle:minimized|", "")
        End If
        If tempString.Contains("winstyle:maximized|") Then
            WinStyle = 3
            tempString = tempString.Replace("winstyle:maximized|", "")
        End If
        If tempString.Contains("focus:true|") Then
            Focus = True
            tempString = tempString.Replace("focus:true|", "")
        End If
        If tempString.Contains("focus:false|") Then
            Focus = False
            tempString = tempString.Replace("focus:false|", "")
        End If
        If tempString.Contains("wait:true|") Then
            waitForExit = True
            tempString = tempString.Replace("wait:true|", "")
        End If
        If tempString.Contains("wait:false|") Then
            waitForExit = False
            tempString = tempString.Replace("wait:false|", "")
        End If

        Select Case WinStyle
            Case 0
                Select Case Focus
                    Case True
                        Shell(tempString, AppWinStyle.NormalFocus, waitForExit)
                    Case False
                        Shell(tempString, AppWinStyle.NormalNoFocus, waitForExit)
                End Select
            Case 1
                Shell(tempString, AppWinStyle.Hide, waitForExit)
            Case 2
                Select Case Focus
                    Case True
                        Shell(tempString, AppWinStyle.MinimizedFocus, waitForExit)
                    Case False
                        Shell(tempString, AppWinStyle.MinimizedNoFocus, waitForExit)
                End Select
            Case 3
                Shell(tempString, AppWinStyle.MaximizedFocus, waitForExit)
        End Select
    End Sub
    Public Sub CmdVisible(ByVal parameter As String)
        parameter = parameter.ToLower
        Select Case parameter
            Case "true", "1"
                Me.Visible = True
            Case "false", "0"
                Me.Visible = False
        End Select
    End Sub
    Public Sub CmdWriteFile(ByVal parameter As String, ByVal append As Boolean)
        parameter = parameter.Replace(" >", ">").Replace("> ", ">")
        Dim tempList As New List(Of String)
        For Each item In parameter.Split(">")
            tempList.Add(item)
        Next
        If tempList.Count = 2 Then
            My.Computer.FileSystem.WriteAllText(tempList(1), tempList(0), append)
        Else
            writeErrorLog("Syntaxfehler in Befehl writeFile mit dem Parameter " & parameter)
        End If
    End Sub
    Public Sub CmdMkDir(ByVal parameter As String)
        My.Computer.FileSystem.CreateDirectory(parameter)
    End Sub
    Public Function CmdIfTaskExist(parameter As String) As Integer
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 2 Or splitedParameter.Count = 3 Then
            For Each item As System.Diagnostics.Process In Process.GetProcessesByName(splitedParameter(0))
                Return jumpChecker(splitedParameter(1))
                Exit Function
            Next
            If splitedParameter.Count = 3 Then
                Return jumpChecker(splitedParameter(2))
            End If
        Else
            writeErrorLog("Syntaxfehler in Befehl IfTaskExist mit dem Parameter " & parameter)
        End If
        Return -1
    End Function
    Public Sub CmdTaskKill(parameter As String)
        For Each item As System.Diagnostics.Process In Process.GetProcessesByName(parameter)
            item.Kill()
            writeInfoLog("Prozess " & item.ProcessName & " mit der ID " & item.Id & " wurde terminiert.")
        Next
    End Sub
    Public Sub CmdTaskClose(parameter As String)
        For Each item As System.Diagnostics.Process In Process.GetProcessesByName(parameter)
            item.CloseMainWindow()
            writeInfoLog("Prozess " & item.ProcessName & " mit der ID " & item.Id & " wurde zum Beenden aufgefordert.")
        Next
    End Sub
    Public Sub CmdGoSub(parameter As String)
        For i As Integer = 0 To commands.Count - 1
            If commands(i).ToLower.StartsWith(":" & parameter.ToLower) Then
                Execute(i)
                Exit For
            End If
        Next
    End Sub
    Public Sub CmdWget(parameter As String)
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 2 Then
            My.Computer.Network.DownloadFile(splitedParameter(0), splitedParameter(1), "", "", False, 100000, True)
        ElseIf splitedParameter.Count = 4 Then
            My.Computer.Network.DownloadFile(splitedParameter(0), splitedParameter(1), splitedParameter(2), splitedParameter(3), False, 100000, True)
        Else
            writeErrorLog("Syntaxfehler in Befehl Wget mit dem Parameter " & parameter)
        End If
    End Sub
    Public Sub CmdLog(parameter As String)
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If (splitedParameter.Count = 1 And splitedParameter(0).ToLower = "false") Or splitedParameter.Count = 2 Then
            If splitedParameter(0).ToLower = "false" Then
                doLog = False
                log = ""
            ElseIf splitedParameter(0).ToLower = "true" Then
                doLog = True
                log = splitedParameter(1)
            End If
        Else
            writeErrorLog("Syntaxfehler in Befehl Log mit dem Parameter " & parameter)
        End If
    End Sub
    Public Sub CmdSet(parameter As String)
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If (splitedParameter.Count = 2) Then

            setVar(splitedParameter(0), splitedParameter(1))
        Else
            writeErrorLog("Syntaxfehler in Befehl Set mit dem Parameter " & parameter)
        End If
    End Sub
    Public Sub CmdReadFile(parameter As String)
        parameter = parameter.Replace(" >", ">").Replace("> ", ">")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split(">")
            splitedParameter.Add(item)
        Next
        If (splitedParameter.Count = 2) Then
            If My.Computer.FileSystem.FileExists(splitedParameter(0)) Then
                setVar(splitedParameter(1), My.Computer.FileSystem.ReadAllText(splitedParameter(0)))
            Else
                writeErrorLog("Befehl ReadFile kann die Datei " & splitedParameter(0) & " nicht finden.")
            End If
        Else
            writeErrorLog("Syntaxfehler in Befehl ReadFile mit dem Parameter " & parameter)
        End If
    End Sub
    Public Function CmdIfStringEqual(parameter As String) As Integer
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 3 Or splitedParameter.Count = 4 Then
            If splitedParameter(0).ToLower = splitedParameter(1).ToLower Then
                writeInfoLog("String " & splitedParameter(0) & " stimmt überein. Es wird zu Lable " & splitedParameter(2) & " gesprungen.")
                Return jumpChecker(splitedParameter(2))
                Exit Function
            ElseIf splitedParameter.Count = 4 Then
                writeInfoLog("String " & splitedParameter(0) & " stimmt mit " & splitedParameter(1) & " nicht überein. Es wird zu Lable " & splitedParameter(3) & " gesprungen.")
                Return jumpChecker(splitedParameter(3))
            End If
        End If
        Return -1
    End Function
    Public Function CmdIfStringContain(parameter As String) As Integer
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 3 Or splitedParameter.Count = 4 Then
            If splitedParameter(0).ToLower.Contains(splitedParameter(1).ToLower) Then
                writeInfoLog("String " & splitedParameter(0) & " stimmt überein. Es wird zu Lable " & splitedParameter(2) & " gesprungen.")
                Return jumpChecker(splitedParameter(2))
                Exit Function
            ElseIf splitedParameter.Count = 4 Then
                writeInfoLog("String " & splitedParameter(0) & " stimmt mit " & splitedParameter(1) & " nicht überein. Es wird zu Lable " & splitedParameter(3) & " gesprungen.")
                Return jumpChecker(splitedParameter(3))
            End If
        End If
        Return -1
    End Function

    Public Sub CmdCalculate(parameter As String)
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 2 Then
            Try
                setVar(splitedParameter(0), CCalculator.Calc(splitedParameter(1)))
            Catch ex As Exception
                writeErrorLog("Math Error in Calculate Befehl" & vbCrLf & ex.ToString)
            End Try
        Else
            writeErrorLog("Syntaxfehler in Befehl Calculate mit dem Parameter " & parameter)
        End If
    End Sub
    Public Sub CmdSubstring(parameter As String)
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 3 Or splitedParameter.Count = 4 Then
            Try
                If splitedParameter.Count = 3 Then
                    setVar(splitedParameter(0), splitedParameter(1).Substring(splitedParameter(2)))
                Else
                    setVar(splitedParameter(0), splitedParameter(1).Substring(splitedParameter(2), splitedParameter(3)))
                End If
            Catch ex As Exception
                writeErrorLog("Fehler bei Substring" & vbCrLf & ex.ToString)
            End Try
        Else
            writeErrorLog("Syntaxfehler in Befehl Substring mit dem Parameter " & parameter)
        End If
    End Sub
    Public Sub CmdReplaceString(parameter As String)
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 4 Then
            Try
                setVar(splitedParameter(0), splitedParameter(1).Replace(splitedParameter(2), splitedParameter(3)))
            Catch ex As Exception
                writeErrorLog("Fehler bei ReplaceString" & vbCrLf & ex.ToString)
            End Try
        Else
            writeErrorLog("Syntaxfehler in Befehl ReplaceString mit dem Parameter " & parameter)
        End If
    End Sub
    Public Sub CmdSetRegValue(parameter As String)
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 3 Or splitedParameter.Count = 4 Then
            Try
                If splitedParameter.Count = 3 Then
                    My.Computer.Registry.SetValue(splitedParameter(0), splitedParameter(1), splitedParameter(2))
                Else
                    My.Computer.Registry.SetValue(splitedParameter(0), splitedParameter(1), splitedParameter(2), getRegValueKind(splitedParameter(3)))
                End If
            Catch ex As Exception
                writeErrorLog("Fehler bei SetRegValue" & vbCrLf & ex.ToString)
            End Try
        Else
            writeErrorLog("Syntaxfehler in Befehl SetRegValue mit dem Parameter " & parameter)
        End If
    End Sub
    Public Sub cmdGetRegValue(parameter As String)
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 3 Then
            Try
                setVar(splitedParameter(2), My.Computer.Registry.GetValue(splitedParameter(0), splitedParameter(1), ""))
            Catch ex As Exception
                writeErrorLog("Fehler bei GetRegValue" & vbCrLf & ex.ToString)
            End Try
        Else
            writeErrorLog("Syntaxfehler in Befehl GetRegValue mit dem Parameter " & parameter)
        End If
    End Sub
    Public Sub cmdCreateRegKey(parameter As String)
        Try
            Dim Hive As String = ""
            Dim Key As String = ""
            If parameter.Contains("\") Then
                Hive = parameter.Substring(0, parameter.IndexOf("\"))
                Key = parameter.Substring(parameter.IndexOf("\") + 1)
            ElseIf parameter.Contains("/") Then
                Hive = parameter.Substring(0, parameter.IndexOf("/"))
                Key = parameter.Substring(parameter.IndexOf("/") + 1)
            Else
                writeErrorLog("Syntaxfehler in Befehl CreateRegKey mit dem Parameter " & parameter)
                Exit Sub
            End If
            getHive(Hive).CreateSubKey(Key)
        Catch ex As Exception
            writeErrorLog("Fehler bei CreateRegKey" & vbCrLf & ex.ToString)
        End Try

    End Sub

    Public Sub cmdDelRegKey(parameter As String)
        Try
            Dim Hive As String = ""
            Dim Key As String = ""
            If parameter.Contains("\") Then
                Hive = parameter.Substring(0, parameter.IndexOf("\"))
                Key = parameter.Substring(parameter.IndexOf("\") + 1)
            ElseIf parameter.Contains("/") Then
                Hive = parameter.Substring(0, parameter.IndexOf("/"))
                Key = parameter.Substring(parameter.IndexOf("/") + 1)
            Else
                writeErrorLog("Syntaxfehler in Befehl DelRegKey mit dem Parameter " & parameter)
                Exit Sub
            End If
            getHive(Hive).DeleteSubKeyTree(Key)
        Catch ex As Exception
            writeErrorLog("Fehler bei DelRegKey" & vbCrLf & ex.ToString)
        End Try

    End Sub
    Public Sub cmdDelRegValue(parameter As String)
        Try
            parameter = parameter.Replace(" |", "|").Replace("| ", "|")
            Dim splitedParameter As New List(Of String)
            For Each item In parameter.Split("|")
                splitedParameter.Add(item)
            Next
            If splitedParameter.Count = 2 Then
                Dim Hive As String = ""
                Dim Key As String = ""
                If splitedParameter(0).Contains("\") Then
                    Hive = splitedParameter(0).Substring(0, parameter.IndexOf("\"))
                    Key = splitedParameter(0).Substring(parameter.IndexOf("\") + 1)
                ElseIf splitedParameter(0).Contains("/") Then
                    Hive = splitedParameter(0).Substring(0, parameter.IndexOf("/"))
                    Key = splitedParameter(0).Substring(parameter.IndexOf("/") + 1)
                Else
                    writeErrorLog("Syntaxfehler in Befehl DelRegEntry mit dem Parameter " & parameter)
                    Exit Sub
                End If
                getHive(Hive).OpenSubKey(Key, True).DeleteValue(splitedParameter(1))
            End If
        Catch ex As Exception
            writeErrorLog("Fehler bei DelRegEntry" & vbCrLf & ex.ToString)
        End Try
    End Sub
    Public Function cmdIfPingSuccessfull(parameter As String) As Integer
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 2 Or splitedParameter.Count = 3 Then
            Dim result As Boolean = False
            Dim pingerror As String = ""
            Try
                result = My.Computer.Network.Ping(splitedParameter(0))
            Catch ex As Exception
                result = False
                pingerror = ex.InnerException.Message
            End Try
            If result = True Then
                writeInfoLog(splitedParameter(0) & " ist erreichbar")
                Return jumpChecker(splitedParameter(1))
            Else
                If pingerror <> "" Then
                    writeInfoLog(splitedParameter(0) & " ist nicht erreichbar")
                Else
                    writeInfoLog(splitedParameter(0) & " ist nicht erreichbar: " & pingerror)
                End If
                If splitedParameter.Count = 3 Then
                    Return jumpChecker(splitedParameter(2))
                End If
            End If
        Else
            writeErrorLog("Syntaxerror in Befehl: IfPingSuccessfull")
        End If
        Return -1
    End Function
    Public Sub cmdCls()
        RichTextBox1.Clear()
    End Sub
    Public Function cmdIf(parameter As String) As Integer
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim caseSensitive As Boolean = False
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            If item.ToLower = "casesensitive:true" Then
                caseSensitive = True
            ElseIf item.ToLower = "casesensitive:false" Then
                caseSensitive = False
            Else
                splitedParameter.Add(item)
            End If
        Next
        If splitedParameter.Count = 4 Or splitedParameter.Count = 5 Then
            If caseSensitive = False Then
                splitedParameter(0) = splitedParameter(0).ToLower()
                splitedParameter(2) = splitedParameter(2).ToLower()
            End If
            Dim result As Boolean

            Dim int1
            Dim int2
            If Double.TryParse(splitedParameter(0), New Double) Then
                Dim int11 As Double
                Double.TryParse(splitedParameter(0), int11)
                int1 = int11
            Else
                Dim int11 As String = splitedParameter(0)
                int1 = int11
            End If
            If Double.TryParse(splitedParameter(2), New Double) Then
                Dim int22 As Double
                Double.TryParse(splitedParameter(2), int22)
                int2 = int22
            Else
                Dim int22 As String = splitedParameter(2)
                int2 = int22
            End If

            Select Case splitedParameter(1)
                Case "<"
                    result = int1 < int2
                Case ">"
                    result = int1 > int2
                Case "<="
                    result = int1 <= int2
                Case ">="
                    result = int1 >= int2
                Case "=", "=="
                    result = int1 = int2
                Case "!=", "<>"
                    result = int1 <> int2
                Case "contains"
                    result = splitedParameter(0).Contains(splitedParameter(2))
                Case "startswith"
                    result = splitedParameter(0).StartsWith(splitedParameter(2))
                Case "endswith"
                    result = splitedParameter(0).EndsWith(splitedParameter(2))
                Case Else
                    writeErrorLog(splitedParameter(1) & " ist kein gültiger Operator")
                    Return -1
            End Select
            If result Then
                writeInfoLog("If ist True")
                Return jumpChecker(splitedParameter(3))
            Else
                writeInfoLog("If ist False")
                If splitedParameter.Count = 5 Then
                    Return jumpChecker(splitedParameter(4))
                End If
            End If
        Else
            writeErrorLog("Syntaxerror in Befehl: If")
        End If
        Return -1
    End Function
End Class


'# kommentar;
'message text;
'sleep/wait sekunden;
'delDir pfad
'copyDir quelle > ziel
'delFile pfad
'copyFile quelle > ziel
'moveDir quelle > ziel
'moveFile quelle > ziel
'start pfad | parameter
'startwait pfad | parameter
'title text;
'exit/next;
'goto labelname;
':labelname;
'IfDirExist pfad | truelable | falselable;
'IfFileExist pfad | truelable | falselable;
'shell winstyle:normal | focus:true | wait:false | befehl;
'visible true/false;
'writeFile Text > Pfad;
'writeFileAppend Text > Pfad;
'mkDir Pfad;
'ifTaskExist taskname | truelable | falselable;
'taskKill taskname;
'taskClose taskname;
'goSub lablename;
'wget DownloadDatei | Speicherort;
'log true/false | Pfad_der_Logdatei;
'set varName | varValue;
'readFile File > varName;
'ifStringEqual string1 | string2 | truelable | falselable; (Veraltet! Funktion in Befehl IF integriert)
'ifStringContain zuPrüfendenString | enthälltString | truelable | falselable; (Veraltet! Funktion in Befehl IF integriert)
'calculate ergebnis | rechenstring;
'substring ergebnis | string | startindex | länge;
'replaceString ergebnis | string | oldChar | newChar;
'setRegValue RegPath | EntryName | EntryValue | EntryType;
'getRegValue RegPath | EntryName | varName;
'createRegKey RegPath;
'delRegKey RegPath;
'delRegValue RegPath | EntryName;
'Include Path;
'ifPingSuccessfull Address | truelable | falselable;
'cls;
'if casesensitive:false | wert1 | operator | wert2 | truelable | falselable;
