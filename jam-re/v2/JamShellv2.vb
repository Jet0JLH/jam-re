Public Class JamShellv2
    Dim version As String = My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Build

    Private Sub JamShellv2_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        CheckForIllegalCrossThreadCalls = False
        VersionsLabel.Text = "Version: " & version
    End Sub
End Class