Module logModule
    Public log As String = ""
    Public doLog As Boolean = False
    Public Sub writeCommandInfoLog(command As String, parameter As String)
        If doLog = True Then
            My.Computer.FileSystem.WriteAllText(log, getDate() & " [Info]: Befehl " & command & " mit den Parametern " & parameter & " gestartet." & vbCrLf, True)
        End If
    End Sub
    Public Sub writeInfoLog(text As String)
        If doLog = True Then
            My.Computer.FileSystem.WriteAllText(log, getDate() & " [Info]: " & text & vbCrLf, True)
        End If
    End Sub
    Public Sub writeWarningLog(text As String)
        If doLog = True Then
            My.Computer.FileSystem.WriteAllText(log, getDate() & " [Warnung]: " & text & vbCrLf, True)
        End If
    End Sub
    Public Sub writeCommandErrorLog(command As String, parameter As String, Fehler As String)
        If doLog = True Then
            My.Computer.FileSystem.WriteAllText(log, getDate() & " [Fehler]: Befehl " & command & " mit den Parametern " & parameter & " hat folgenden Fehler zurückgegeben: " & vbCrLf & vbTab & vbTab & vbTab & vbTab & Fehler & vbCrLf, True)
        End If
    End Sub
    Public Sub writeErrorLog(text As String)
        If doLog = True Then
            My.Computer.FileSystem.WriteAllText(log, getDate() & " [Fehler]: " & text & vbCrLf, True)
        End If
    End Sub
    Private Function getDate() As String
        Dim year As String
        Dim month As String
        Dim day As String
        Dim hour As String
        Dim minute As String
        Dim second As String
        Dim millisecond As String
        year = My.Computer.Clock.LocalTime.Year
        month = My.Computer.Clock.LocalTime.Month
        day = My.Computer.Clock.LocalTime.Day
        hour = My.Computer.Clock.LocalTime.Hour
        minute = My.Computer.Clock.LocalTime.Minute
        second = My.Computer.Clock.LocalTime.Second
        millisecond = My.Computer.Clock.LocalTime.Millisecond
        If month.Count = 1 Then
            month = "0" & month
        End If
        If day.Count = 1 Then
            day = "0" & day
        End If
        If hour.Count = 1 Then
            hour = "0" & hour
        End If
        If minute.Count = 1 Then
            minute = "0" & minute
        End If
        If second.Count = 1 Then
            second = "0" & second
        End If
        If millisecond.Count = 1 Then
            millisecond = "00" & millisecond
        ElseIf millisecond.Count = 2 Then
            millisecond = "0" & millisecond
        End If

        Return year & "." & month & "." & day & "-" & hour & ":" & minute & ":" & second & "." & millisecond
    End Function
End Module
