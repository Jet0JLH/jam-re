Imports System.ComponentModel

Public Class JamRe_v2
    Dim version As String = My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Build
    Dim engine As JamEngine
    Private Sub JamShellv2_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        CheckForIllegalCrossThreadCalls = False
        VersionsLabel.Text = "Version: " & version
        engine = New JamEngine()
        addHandlers()
        statusChanged(engine, engine.status, engine.status)
    End Sub
    Private Sub JamShellv2_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        engine.stopEngine()
    End Sub
#Region "Handler"
    Private Sub addHandlers()
        AddHandler engine.statusChanged, AddressOf statusChanged
        AddHandler engine.visibilityChanged, AddressOf visibilityChanged
        AddHandler engine.writeText, AddressOf writeText
        AddHandler engine.titleChanged, AddressOf titleChanged
        AddHandler engine.directoryChanged, AddressOf directoryChanged
    End Sub
    Private Sub statusChanged(ByRef sender As JamEngine, ByVal oldStatus As JamEngine.EngineStatus, ByVal newStatus As JamEngine.EngineStatus)
        Select Case newStatus
            Case JamEngine.EngineStatus.Running
                Pinwall.BackColor = Color.LightBlue
            Case JamEngine.EngineStatus.Finished
                Pinwall.BackColor = Color.LightGreen
            Case JamEngine.EngineStatus.Resuming
                Pinwall.BackColor = Color.CornflowerBlue
            Case JamEngine.EngineStatus.Halted
                Pinwall.BackColor = Color.LightYellow
            Case JamEngine.EngineStatus.HaltedBecauseError
                Pinwall.BackColor = Color.LightYellow
            Case JamEngine.EngineStatus.HaltedBecauseWarning
                Pinwall.BackColor = Color.LightYellow
            Case JamEngine.EngineStatus.Stopped
                Pinwall.BackColor = Color.Red
                Me.Close()
            Case JamEngine.EngineStatus.Stopping
                Pinwall.BackColor = Color.Red
        End Select
    End Sub
    Private Sub visibilityChanged(ByRef sender As JamEngine, ByVal value As Boolean)
        Me.Visible = value
    End Sub
    Private Sub titleChanged(ByRef sender As JamEngine, ByVal value As String)
        TitleLabel.Text = value
    End Sub
    Private Sub directoryChanged(ByRef sender As JamEngine, ByVal value As String)
        IO.Directory.SetCurrentDirectory(value)
    End Sub
    Private Sub writeText(ByRef sender As JamEngine, ByVal text As String, ByVal clearLines As Integer, ByVal newLine As Boolean, ByVal clear As Boolean)
        If clear Then
            Pinwall.Clear()
        Else
            If clearLines <> 0 Then
                If Pinwall.Lines.Count <= clearLines Then
                    Pinwall.Clear()
                ElseIf clearLines > 0 Then
                    Dim tempList As List(Of String) = Pinwall.Lines.ToList
                    For i As Integer = 0 To clearLines
                        tempList.Remove(tempList.Count - 1)
                    Next
                    Pinwall.Lines = tempList.ToArray
                Else
                    For i As Integer = 0 To clearLines
                        Pinwall.AppendText(vbCrLf)
                    Next
                End If
            End If
        End If
        If newLine Then
            Pinwall.AppendText(text & vbCrLf)
        Else
            Pinwall.AppendText(text)
        End If
    End Sub
#End Region

    Private Sub Pinwall_Click(sender As Object, e As EventArgs) Handles Pinwall.Click 'Test Subroutine
        Dim temp As String = InputBox("Soll ein Befehl hinzugefügt werden?")
        If temp <> "" Then engine.addCommands(temp)
        engine.startEngine()
    End Sub
End Class