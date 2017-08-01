Public Class Logo
    Protected vPath As String
    Protected vPosition As Point

    Sub New(path As String)
        Me.Path = path
        Me.Position = New Point(0, 0)
    End Sub
    Public Property Path As String
        Get
            Return vPath
        End Get
        Set(value As String)
            vPath = value
        End Set
    End Property
    Public Property Position As Point
        Get
            Return vPosition
        End Get
        Set(value As Point)
            vPosition = value
        End Set
    End Property
End Class
