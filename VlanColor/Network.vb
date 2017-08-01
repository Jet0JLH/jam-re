Public Class Network
    Protected vName As String
    Protected vVlanNumber As Integer
    Protected vVlanName As String
    Protected vBgColor As Color
    Protected vFontColor As Color

    Sub New(name As String, bgColor As Color, Optional VlanNummer As Integer = -1, Optional VlanName As String = "")
        Me.Name = name
        Me.BgColor = bgColor
        Me.VlanNumber = VlanNumber
        Me.VlanName = VlanName
        Me.FontColor = Color.Black
    End Sub
    Public Property Name As String
        Get
            Return vName
        End Get
        Set(value As String)
            vName = value
        End Set
    End Property
    Public Property BgColor As Color
        Get
            Return vBgColor
        End Get
        Set(value As Color)
            vBgColor = value
        End Set
    End Property
    Public Property VlanName As String
        Get
            Return vVlanName
        End Get
        Set(value As String)
            vVlanName = value
        End Set
    End Property
    Public Property VlanNumber As Integer
        Get
            Return vVlanNumber
        End Get
        Set(value As Integer)
            vVlanNumber = value
        End Set
    End Property
    Public Property FontColor As Color
        Get
            Return vFontColor
        End Get
        Set(value As Color)
            vFontColor = value
        End Set
    End Property
End Class
