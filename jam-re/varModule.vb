Module varModule
    Public varList As New List(Of varObject)
    Public Class varObject
        Protected vName As String
        Protected vValue As String
        Public Sub New(varName As String)
            Name = varName
            Value = ""
        End Sub
        Public Sub New(varName As String, varValue As String)
            Name = varName
            Value = varValue
        End Sub
        Public Property Name As String
            Get
                Return vName
            End Get
            Set(value As String)
                vName = value
            End Set
        End Property
        Public Property Value As String
            Get
                Return vValue
            End Get
            Set(value As String)
                vValue = value
            End Set
        End Property
    End Class
    Public Sub setVar(varName As String, varValue As String)
        For Each item As varObject In varList
            If item.Name = varName Then
                item.Value = varValue
                Exit Sub
            End If
        Next
        varList.Add(New varObject(varName, varValue))
    End Sub
    Public Function replaceVar(inputString) As String
        Dim outputString As String = inputString
        For Each item In varList
            outputString = outputString.Replace("$" & item.Name & "$", item.Value)
        Next
        Return outputString
    End Function
End Module
