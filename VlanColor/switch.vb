Public Class switch
    Protected vSize As Size
    Protected vColor As Color
    Protected vName As String
    Public Ports As New List(Of port)
    Public Borders As New List(Of border)
    Public Logos As New List(Of Logo)

    Sub New(name As String)
        Me.Name = name
        Me.Color = Drawing.Color.Aqua
        Me.Size = New Size(1000, 100)
        Me.Ports.Clear()
        Me.Borders.Clear()
        Me.Logos.Clear()
    End Sub
    Public Property Name As String
        Get
            Return vName
        End Get
        Set(value As String)
            vName = value
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
    Public Property Color As Color
        Get
            Return vColor
        End Get
        Set(value As Color)
            vColor = value
        End Set
    End Property
    'Public Property Ports As List(Of port)
    '    Get
    '        Return vPorts
    '    End Get
    '    Set(value As List(Of port))
    '        vPorts = value
    '    End Set
    'End Property
End Class
