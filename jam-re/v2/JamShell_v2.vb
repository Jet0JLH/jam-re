Module JamShell_v2
    Dim version As String = My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Build
    Dim engine As JamEngine

    Sub Main()
        Console.Title = "Jam Shell"
        Console.WriteLine("Jam Shell " & version)
        engine = New JamEngine()
        addHandlers()
        engine.startEngine()
        While engine.status <> JamEngine.EngineStatus.Stopped
            If engine.status = JamEngine.EngineStatus.Finished Then
                Console.Write("[Jam]" & IO.Directory.GetCurrentDirectory & "> ")
                engine.addCommands(Console.ReadLine)
                engine.startEngine()
            End If
            Threading.Thread.Sleep(250)
        End While
    End Sub
#Region "Handler"
    Private Sub addHandlers()
        AddHandler engine.writeText, AddressOf writeText
        AddHandler engine.titleChanged, AddressOf titleChanged
        AddHandler engine.visibilityChanged, Sub() Console.WriteLine("Visible command is not supported in shell mode")
        AddHandler engine.directoryChanged, AddressOf directoryChanged
    End Sub
    Private Sub titleChanged(ByRef sender As JamEngine, ByVal value As String)
        Console.Title = value
    End Sub
    Private Sub directoryChanged(ByRef sender As JamEngine, ByVal value As String)
        IO.Directory.SetCurrentDirectory(value)
    End Sub
    Private Sub writeText(ByRef sender As JamEngine, ByVal text As String, ByVal clearLines As Integer, ByVal newLine As Boolean, ByVal clear As Boolean, ByVal isError As Boolean)
        If clear Then
            Console.Clear()
        End If

        Dim lastConsoleFontColor As ConsoleColor = Console.ForegroundColor
        If isError Then Console.ForegroundColor = ConsoleColor.Red
        If newLine Then
            Console.WriteLine(text)
        Else
            Console.Write(text)
        End If
        If isError Then Console.ForegroundColor = lastConsoleFontColor
    End Sub
#End Region
End Module
