Imports System.CodeDom.Compiler
Imports System.Reflection
Public Class CCalculator
    Private Shared ass As Assembly
    Private Shared aClass As Type
    Private Shared aMethode As MethodInfo
    Private Shared obj As Object

    Public Shared Function Calc(expr As String) As Double
        If expr.Length = 0 Then Return 0.0
        expr = expr.Replace(",", ".")
        Dim opt As New CompilerParameters(Nothing, String.Empty, False)
        opt.GenerateExecutable = False
        opt.GenerateInMemory = True
        Dim src As String = "Imports System.Math" & vbCrLf & "Public Class Calculate" & vbCrLf & "Public Function Calc() As Double" & vbCrLf & "Return " & expr & vbCrLf & "End Function" & vbCrLf & "End Class" & vbCrLf
        Dim res As CompilerResults = New VBCodeProvider().CompileAssemblyFromSource(opt, src)
        If res.Errors.Count > 0 Then
            Dim errors As String = String.Empty
            For Each cerr As CompilerError In res.Errors
                errors = errors & cerr.ToString() & vbCrLf
            Next
            ass = Nothing
            expr = String.Empty
            Throw New ApplicationException(errors)
        End If
        ass = res.CompiledAssembly
        aClass = ass.GetType("Calculate")
        aMethode = aClass.GetMethod("Calc")
        obj = Activator.CreateInstance(aClass)
        Return Convert.ToDouble(aMethode.Invoke(obj, Nothing))
    End Function

End Class
