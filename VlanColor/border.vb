Public Class border
    Protected vColor As Color
    Protected vSize As Size
    Protected vPosition As Point
    Protected vText As String

    Sub New(size As Size, position As Point, Optional text As String = "")
        Me.Size = size
        Me.Position = position
        Me.Text = text
        Me.Color = Drawing.Color.Black
    End Sub
    Public Property Color As Color
        Get
            Return vColor
        End Get
        Set(value As Color)
            vColor = value
        End Set
    End Property
    Public Property Size As Size
        Get
            Return vSize
        End Get
        Set(value As Size)
            vSize = value
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
    Public Property Text As String
        Get
            Return vText
        End Get
        Set(value As String)
            vText = value
        End Set
    End Property
End Class
