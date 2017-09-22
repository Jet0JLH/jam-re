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
        With My.Computer.Clock.LocalTime
            outputString = Microsoft.VisualBasic.Strings.Replace(outputString, "%day%", .Day, , , Constants.vbTextCompare)
            outputString = Microsoft.VisualBasic.Strings.Replace(outputString, "%month%", .Month, , , Constants.vbTextCompare)
            outputString = Microsoft.VisualBasic.Strings.Replace(outputString, "%year%", .Year, , , Constants.vbTextCompare)
            outputString = Microsoft.VisualBasic.Strings.Replace(outputString, "%second%", .Second, , , Constants.vbTextCompare)
            outputString = Microsoft.VisualBasic.Strings.Replace(outputString, "%minute%", .Minute, , , Constants.vbTextCompare)
            outputString = Microsoft.VisualBasic.Strings.Replace(outputString, "%hour%", .Hour, , , Constants.vbTextCompare)
            Dim day As String = ""
            Dim month As String = ""
            Dim second As String = ""
            Dim minute As String = ""
            Dim hour As String = ""
            If .Day < 10 Then
                day = "0" & .Day
            Else
                day = .Day
            End If
            If .Month < 10 Then
                month = "0" & .Month
            Else
                month = .Month
            End If
            If .Second < 10 Then
                second = "0" & .Second
            Else
                second = .Second
            End If
            If .Minute < 10 Then
                minute = "0" & .Minute
            Else
                minute = .Minute
            End If
            If .Hour < 10 Then
                hour = "0" & .Hour
            Else
                hour = .Hour
            End If
            outputString = Microsoft.VisualBasic.Strings.Replace(outputString, "%0day%", day, , , Constants.vbTextCompare)
            outputString = Microsoft.VisualBasic.Strings.Replace(outputString, "%0month%", month, , , Constants.vbTextCompare)
            outputString = Microsoft.VisualBasic.Strings.Replace(outputString, "%0second%", second, , , Constants.vbTextCompare)
            outputString = Microsoft.VisualBasic.Strings.Replace(outputString, "%0minute%", minute, , , Constants.vbTextCompare)
            outputString = Microsoft.VisualBasic.Strings.Replace(outputString, "%0hour%", hour, , , Constants.vbTextCompare)
        End With

        Return outputString
    End Function
End Module
