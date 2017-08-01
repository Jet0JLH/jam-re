Public Class port
    Protected vNumber As Integer
    Protected vPosition As Point
    Protected vNetwork As Network
    Protected vNotice As String

    Sub New(number As Integer)
        Me.Number = number
        Me.Position = New Point(0, 0)
        With Form1.defaultNetwork
            Me.Network = New Network(.Name, .BgColor)
        End With
        With Me.Network
            .FontColor = Form1.defaultNetwork.FontColor
            .VlanName = Form1.defaultNetwork.VlanName
            .VlanNumber = Form1.defaultNetwork.VlanNumber
        End With
        Me.Notice = ""
    End Sub
    Public Property Number As Integer
        Get
            Return vNumber
        End Get
        Set(value As Integer)
            vNumber = value
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
    Public Property Network As Network
        Get
            Return vNetwork
        End Get
        Set(value As Network)
            vNetwork = value
        End Set
    End Property
    Public Property Notice As String
        Get
            Return vNotice
        End Get
        Set(value As String)
            vNotice = value
        End Set
    End Property
End Class
