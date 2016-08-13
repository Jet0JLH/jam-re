﻿Imports System.Drawing.Drawing2D
Public Class Form1
    Dim ready As Boolean = False
    Const version As String = "1.3"
    Public commandPointer As Integer = 0
    Public commands As New List(Of String)

    Dim openThread As New Threading.Thread(AddressOf openForm)
    Private Sub Form1_Shown(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Shown
        CheckForIllegalCrossThreadCalls = False
        Label1.Text = "Version: " & version

        Dim exe As New Threading.Thread(AddressOf Execute)
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

            Threading.Thread.Sleep(1)
        Loop
        Do While Me.Height < 265
            Me.Height += 4
            Me.Location = New Point(Me.Location.X, Me.Location.Y - 2)

            Threading.Thread.Sleep(1)
        Loop
    End Sub

    Sub closeForm()
        '526; 265
        Do While Me.Height > 2
            Me.Height -= 4
            Me.Location = New Point(Me.Location.X, Me.Location.Y + 2)

            Threading.Thread.Sleep(1)
        Loop
        Do While Me.Width > 2
            Me.Width -= 4
            Me.Location = New Point(Me.Location.X + 2, Me.Location.Y)

            Threading.Thread.Sleep(1)
        Loop
    End Sub

    Sub Execute()
        Dim text As String = ""
        Try
            If Environment.GetCommandLineArgs.Count <= 1 Then
                RichTextBox1.AppendText(vbCrLf & "Es wurde keine Datei übergeben!")
                ende()
            Else
                Using sr As System.IO.StreamReader = New System.IO.StreamReader(Environment.GetCommandLineArgs(1), System.Text.Encoding.Default)
                    text = sr.ReadToEnd.ToString
                    sr.Close()
                End Using
                text = text.Replace(Chr(10), "").Replace(Chr(13), "").Replace(vbTab, "")

                'Prüfen ob weitere Parameter übergeben wurde
                If Environment.GetCommandLineArgs.Count >= 3 Then
                    For i As Integer = 2 To Environment.GetCommandLineArgs.Count - 1
                        text = text.Replace("%" & i - 1 & "%", Environment.GetCommandLineArgs(i))
                    Next
                End If
                'Ersetze weitere Variablen
                text = text.Replace("%_br%", Chr(13) & Chr(10)) '.Replace("%appdata%", My.Computer.FileSystem.SpecialDirectories.CurrentUserApplicationData).Replace("%allappdata%", My.Computer.FileSystem.SpecialDirectories.AllUsersApplicationData).Replace("%programfiles%", My.Computer.FileSystem.SpecialDirectories.ProgramFiles).Replace("%programs%", My.Computer.FileSystem.SpecialDirectories.Programs)


                For Each item As String In text.Split(";")
                    commands.Add(item)
                Next
                For Me.commandPointer = 0 To commands.Count - 1
                    Dim tempString As String = ""
                    For Each tempChar As Char In commands(commandPointer)
                        If tempChar <> " " Then
                            tempString &= tempChar
                        Else
                            Exit For
                        End If
                    Next
                    If commands(commandPointer).Length <= tempString.Length + 1 Then
                        CommandSelect(tempString)
                    Else
                        CommandSelect(tempString, commands(commandPointer).Substring(tempString.Length + 1))
                    End If
                Next
            End If
        Catch ex As Exception
            RichTextBox1.AppendText("Fehler beim Laden der Skriptdatei aufgetreten!" & vbCrLf & vbCrLf & ex.ToString)
        End Try
        ende()
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

    Public Sub CommandSelect(ByVal command As String, Optional ByVal parameter As String = "")
        Try
            Dim tempCommand As String = command.ToLower
            Select Case tempCommand
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
                Case "exit"
                    ende()
                Case "goto"
                    CmdGoto(parameter)
                Case "ifdirexist"
                    CmdDirExist(parameter)
                Case "iffileexist"
                    CmdFileExist(parameter)
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
            End Select
        Catch ex As Exception
            RichTextBox1.AppendText("Fehler beim Ausführen von Command: " & command & " mit dem Parameter: " & parameter & " aufgetreten!" & vbCrLf & vbCrLf & ex.ToString)
        End Try
    End Sub

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
    Public Sub CmdGoto(ByVal parameter As String)
        For i As Integer = 0 To commands.Count - 1
            If commands(i).ToLower.StartsWith(":" & parameter) Then
                commandPointer = i
                Exit For
            End If
        Next
    End Sub
    Public Sub CmdDirExist(ByVal parameter As String)
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 2 Or splitedParameter.Count = 3 Then
            If My.Computer.FileSystem.DirectoryExists(splitedParameter(0)) Then
                CmdGoto(splitedParameter(1))
            Else
                If splitedParameter.Count = 3 Then
                    CmdGoto(splitedParameter(2))
                End If
            End If
        End If
    End Sub
    Public Sub CmdFileExist(ByVal parameter As String)
        parameter = parameter.Replace(" |", "|").Replace("| ", "|")
        Dim splitedParameter As New List(Of String)
        For Each item In parameter.Split("|")
            splitedParameter.Add(item)
        Next
        If splitedParameter.Count = 2 Or splitedParameter.Count = 3 Then
            If My.Computer.FileSystem.FileExists(splitedParameter(0)) Then
                CmdGoto(splitedParameter(1))
            Else
                If splitedParameter.Count = 3 Then
                    CmdGoto(splitedParameter(2))
                End If
            End If
        End If
    End Sub
    Public Sub CmdShell(ByVal parameter As String)
        Shell(parameter, AppWinStyle.NormalFocus)
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
    
End Class


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
'exit;
'goto labelname;
':labelname;
'IfDirExist pfad | truelable | falselable;
'IfFileExist pfad | truelable | falselable;
'shell befehl;
'visible true/false;
'writeFile Text > Pfad
'writeFileAppend Text > Pfad
'mkDir Pfad;