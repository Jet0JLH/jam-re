Public Class config
    Protected vTemplate As String
    Public ports As New List(Of port)

    Sub New(template As String)
        Me.Template = template
        Me.ports.Clear()
    End Sub
    Public Property Template As String
        Get
            Return vTemplate
        End Get
        Set(value As String)
            vTemplate = value
        End Set
    End Property
End Class
