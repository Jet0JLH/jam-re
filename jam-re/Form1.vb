Imports System.Drawing.Drawing2D
Public Class Form1
    Dim ready As Boolean = False
    Dim version As String = My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor
    Public commands As New List(Of String)
    Dim Script As String = ""

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
        Script = ""
        Try
            If Environment.GetCommandLineArgs.Count <= 1 Then
                RichTextBox1.AppendText(vbCrLf & "Es wurde keine Datei übergeben!")
                ende()
                Return False
            Else
                Using sr As System.IO.StreamReader = New System.IO.StreamReader(Environment.GetCommandLineArgs(1), System.Text.Encoding.Default)
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
                    commands.Add(item)
                Next
                Return True
            End If
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
                    returnValue = CommandSelect(tempString)
                Else
                    returnValue = CommandSelect(tempString, commands(commandPointer).Substring(tempString.Length + 1))
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
                    CmdMessage(parameter)
                Case "sleep", "wait"
                    CmdSleep(parameter)
                Case "deldir"
                    CmdDelDir(parameter)
                Case "copydir"
                    CmdCopyDir(parameter)
                Case "delfile"
                    CmdDelFile(parameter)
                Case "copyfile"
                    CmdCopyFile(parameter)
                Case "movedir"
                    CmdMoveDir(parameter)
                Case "movefile"
                    CmdMoveFile(parameter)
                Case "start"
                    CmdStart(parameter)
                Case "startwait"
                    CmdStartWait(parameter)
                Case "title"
                    CmdTitle(parameter)
                Case "exit", "next"
                    Return -2
                Case "goto"
                    Return CmdGoto(parameter)
                Case "ifdirexist"
                    Return CmdDirExist(parameter)
                Case "iffileexist"
                    Return CmdFileExist(parameter)
                Case "shell"
                    CmdShell(parameter)
                Case "visible"
                    CmdVisible(parameter)
                Case "writefile"
                    CmdWriteFile(parameter, False)
                Case "writefileappend"
                    CmdWriteFile(parameter, True)
                Case "mkdir"
                    CmdMkDir(parameter)
                Case "iftaskexist"
                    Return CmdIfTaskExist(parameter)
                Case "taskkill"
                    CmdTaskKill(parameter)
                Case "taskclose"
                    CmdTaskClose(parameter)
                Case "gosub"
                    CmdGoSub(parameter)
                Case "wget"
                    CmdWget(parameter)
            End Select
        Catch ex As Exception
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
            End If
        End If
    End Sub
    Public Sub CmdDelFile(ByVal parameter As String)
        If My.Computer.FileSystem.FileExists(parameter) Then
            My.Computer.FileSystem.DeleteFile(parameter)
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
            End If
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
            End If
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
            End If
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
            If commands(i).ToLower.StartsWith(":" & parameter.ToLower) Then
                Return i
            End If
        Next
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
        End If
        Return -1
    End Function
    Public Sub CmdTaskKill(parameter As String)
        For Each item As System.Diagnostics.Process In Process.GetProcessesByName(parameter)
            item.Kill()
        Next
    End Sub
    Public Sub CmdTaskClose(parameter As String)
        For Each item As System.Diagnostics.Process In Process.GetProcessesByName(parameter)
            item.CloseMainWindow()
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
        End If
        If splitedParameter.Count = 4 Then
            My.Computer.Network.DownloadFile(splitedParameter(0), splitedParameter(1), splitedParameter(2), splitedParameter(3), False, 100000, True)
        End If
    End Sub
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
'startwait | parameter
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
