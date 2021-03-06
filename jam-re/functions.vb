﻿Module functions
    Public ReadOnly Property getHive(Hive As String) As Microsoft.Win32.RegistryKey
        Get
            Select Case Hive.ToLower
                Case "hkey_current_user", "hkcu"
                    Return My.Computer.Registry.CurrentUser
                Case "hkey_classes_root", "hkcr"
                    Return My.Computer.Registry.ClassesRoot
                Case "hkey_local_maschine", "hklm"
                    Return My.Computer.Registry.LocalMachine
                Case "hkey_users", "hku"
                    Return My.Computer.Registry.Users
                Case "hkey_current_config", "hkcc"
                    Return My.Computer.Registry.CurrentConfig
                Case "hkey_dyndata", "hkd"
                    Return My.Computer.Registry.DynData
                Case "hkey_performance_data", "hkpd"
                    Return My.Computer.Registry.PerformanceData
                Case Else
                    writeErrorLog(Hive & " ist kein gültiger Reg Hive")
                    Return Nothing
            End Select
        End Get
    End Property
    Public ReadOnly Property getRegValueKind(ValueKind As String) As Microsoft.Win32.RegistryValueKind
        Get
            Select Case ValueKind.ToLower
                Case "dword"
                    Return Microsoft.Win32.RegistryValueKind.DWord
                Case "binary"
                    Return Microsoft.Win32.RegistryValueKind.Binary
                Case "expandstring"
                    Return Microsoft.Win32.RegistryValueKind.ExpandString
                Case "multistring"
                    Return Microsoft.Win32.RegistryValueKind.MultiString
                Case "qword"
                    Return Microsoft.Win32.RegistryValueKind.QWord
                Case "string"
                    Return Microsoft.Win32.RegistryValueKind.String
                Case "unknown"
                    Return Microsoft.Win32.RegistryValueKind.Unknown
                Case Else
                    Return Microsoft.Win32.RegistryValueKind.String
            End Select
        End Get
    End Property
    Public Function humanReadable(value As Double) As String
        Dim count As Byte = 0
        While value > 1024
            count += 1
            value = value / 1024
        End While
        Select Case count
            Case 0
                Return Math.Round(value, 2) & " Byte"
            Case 1
                Return Math.Round(value, 2) & " KB"
            Case 2
                Return Math.Round(value, 2) & " MB"
            Case 3
                Return Math.Round(value, 2) & " GB"
            Case 4
                Return Math.Round(value, 2) & " TB"
            Case Else
                Return Math.Round(value, 2) & " PB"
        End Select
    End Function


End Module
