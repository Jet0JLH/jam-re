Public Class Form1
    Public templatesPath As String = ""
    Public locationsPath As String = ""
    Public networksPath As String = ""
    Public networks As New List(Of Network)
    Dim currentSwitch As switch
    Dim currentConfig As config
    Public defaultNetwork As New Network("unknown", Color.LightGray)

    Sub loadGlobalConf(path As String)
        If My.Computer.FileSystem.FileExists(path) Then
            Try
                Dim tempXML As New XDocument
                tempXML = XDocument.Load(path)
                templatesPath = tempXML.Element("conf").Element("templates").Attribute("path").Value
                locationsPath = tempXML.Element("conf").Element("locations").Attribute("path").Value
                networksPath = tempXML.Element("conf").Element("networks").Attribute("path").Value
            Catch ex As Exception
                MsgBox("Fehler beim Laden der Konfigdatei aufgetreten" & vbCrLf & vbCrLf & ex.ToString)
                Me.Close()
            End Try
        Else
            MsgBox("Konfigdatei wurde nicht gefunden", MsgBoxStyle.Critical)
            Me.Close()
        End If
    End Sub
    Sub loadNetworks(path As String)
        If My.Computer.FileSystem.FileExists(path) Then
            Dim tempXML As New XDocument
            tempXML = XDocument.Load(path)
            For Each item As XElement In tempXML.Element("networks").Elements("network")
                Dim name As String = ""
                Dim bgcolor As String = ""
                Dim fontcolor As String = ""
                Dim vlanName As String = ""
                Dim vlanNumber As String = ""
                Try
                    name = item.<name>.Value
                    bgcolor = item.<bgcolor>.Value
                    fontcolor = item.<fontcolor>.Value
                    vlanName = item.<vlan>.<name>.Value
                    vlanNumber = item.<vlan>.<number>.Value
                    If name <> "" And bgcolor <> "" Then
                        Dim tempNetwork As New Network(name, ColorTranslator.FromHtml(bgcolor))
                        If vlanName <> "" Then tempNetwork.VlanName = vlanName
                        If vlanNumber <> "" Then tempNetwork.VlanNumber = vlanNumber
                        If fontcolor <> "" Then
                            tempNetwork.FontColor = ColorTranslator.FromHtml(fontcolor)
                        End If
                        networks.Add(tempNetwork)
                    Else
                        MsgBox("Netzwerk " & name & " konnte nicht geladen werden!")
                    End If
                Catch ex As Exception
                    MsgBox("Fehler beim Einlesen des Netzwerks " & name & vbCrLf & vbCrLf & ex.ToString)
                End Try
            Next
        Else
            MsgBox("Network Konfigdatei wurde nicht gefunden", MsgBoxStyle.Critical)
            Me.Close()
        End If
    End Sub
    Function loadTemplate(path As String) As Boolean
        If My.Computer.FileSystem.FileExists(path) Then
            Try
                Dim tempXML As New XDocument
                tempXML = XDocument.Load(path)
                Dim name As String = ""
                Dim color As String = ""
                Dim height As String = ""
                Dim width As String = ""
                With tempXML.Element("switch")
                    name = .<name>.Value
                    color = .<color>.Value
                    height = .<size>.<height>.Value
                    width = .<size>.<width>.Value
                End With
                If name <> "" Then
                    currentSwitch = New switch(name)
                    If height <> "" And width <> "" Then currentSwitch.Size = New Size(width, height)
                    If color <> "" Then currentSwitch.Color = ColorTranslator.FromHtml(color)
                    With tempXML.Element("switch")
                        For Each item As XElement In .Elements("port")
                            Try
                                Dim number As String = ""
                                Dim positionX As String = ""
                                Dim positionY As String = ""
                                number = item.<number>.Value
                                positionX = item.<position>.<x>.Value
                                positionY = item.<position>.<y>.Value
                                If number <> "" Then
                                    Dim tempPort As New port(number)
                                    If positionX <> "" And positionY <> "" Then tempPort.Position = New Point(positionX, positionY)
                                    currentSwitch.Ports.Add(tempPort)
                                Else
                                    MsgBox("Port hat keine Nummer und konnte nicht geladen werden!", MsgBoxStyle.Critical)
                                End If
                            Catch ex As Exception
                                MsgBox("Port konnte nicht eingelesen werden!" & vbCrLf & vbCrLf & ex.ToString, MsgBoxStyle.Critical)
                            End Try
                        Next
                        For Each item As XElement In .Elements("border")
                            Try
                                Dim borderwidth As String = ""
                                Dim borderheight As String = ""
                                Dim positionX As String = ""
                                Dim positionY As String = ""
                                Dim text As String = ""
                                borderwidth = item.<size>.<width>.Value
                                borderheight = item.<size>.<height>.Value
                                positionX = item.<position>.<x>.Value
                                positionY = item.<position>.<y>.Value
                                text = item.<name>.Value
                                If borderheight <> "" And borderwidth <> "" And positionX <> "" And positionY <> "" Then
                                    Dim tempBorder As New border(New Size(borderwidth, borderheight), New Point(positionX, positionY))
                                    If text <> "" Then tempBorder.Text = text
                                    currentSwitch.Borders.Add(tempBorder)
                                Else
                                    MsgBox("Port konnte nicht geladen werden!", MsgBoxStyle.Critical)
                                End If
                            Catch ex As Exception
                                MsgBox("Border konnte nicht eingelesen werden!" & vbCrLf & vbCrLf & ex.ToString, MsgBoxStyle.Critical)
                            End Try
                        Next
                        For Each item As XElement In .Elements("logo")
                            Try
                                Dim logopath As String = ""
                                Dim logoposx As String = ""
                                Dim logoposy As String = ""
                                logopath = item.<path>.Value
                                logoposx = item.<position>.<x>.Value
                                logoposy = item.<position>.<y>.Value
                                If logopath <> "" Then
                                    Dim tempLogo As New Logo(logopath)
                                    If logoposx <> "" And logoposy <> "" Then tempLogo.Position = New Point(logoposx, logoposy)
                                    currentSwitch.Logos.Add(tempLogo)
                                Else
                                    MsgBox("Logo konnte nicht geladen werden!", MsgBoxStyle.Critical)
                                End If
                            Catch ex As Exception
                                MsgBox("Logo konnte nicht eingelesen werden!" & vbCrLf & vbCrLf & ex.ToString, MsgBoxStyle.Critical)
                            End Try
                        Next
                    End With
                Else
                    MsgBox("Switch konnte nicht geladen werden!")
                    Return False
                End If
            Catch ex As Exception
                MsgBox("Fehler beim Einlesen des Schemas " & vbCrLf & vbCrLf & ex.ToString, MsgBoxStyle.Critical)
                Return False
            End Try
        Else
            MsgBox("Schema konnte nicht geladen werden!", MsgBoxStyle.Critical)
            Return False
        End If
        Return True
    End Function

    Function loadConfig(path As String) As Boolean
        If My.Computer.FileSystem.FileExists(path) Then
            Try
                Dim tempXML As New XDocument
                tempXML = XDocument.Load(path)
                Dim template As String = ""
                template = tempXML.Element("conf").Element("template")
                If template <> "" Then
                    currentConfig = New config(IO.Path.Combine(templatesPath, template))
                    For Each item As XElement In tempXML.Element("conf").Elements("port")
                        Dim number As String = ""
                        Dim vlannumber As String = ""
                        number = item.<number>.Value
                        vlannumber = item.<vlan>.<number>.Value
                        If number <> "" Then
                            Dim tempPort As New port(number)
                            If vlannumber <> "" Then tempPort.Network.VlanNumber = vlannumber
                            currentConfig.ports.Add(tempPort)
                        Else
                            MsgBox("Port konnte nicht geladen werden!", MsgBoxStyle.Critical)
                        End If
                    Next
                    If loadTemplate(currentConfig.Template) Then
                        Return True
                    Else
                        MsgBox("Template konnte nicht geladen werden!", MsgBoxStyle.Critical)
                        Return False
                    End If
                Else
                    MsgBox("Kein Template angegeben!", MsgBoxStyle.Critical)
                    Return False
                End If
            Catch ex As Exception
                MsgBox("Fehler beim Einlesen der Konfiguration " & vbCrLf & vbCrLf & ex.ToString, MsgBoxStyle.Critical)
                Return False
            End Try
        Else
            MsgBox("Konfiguration konnte nicht geladen werden!", MsgBoxStyle.Critical)
            Return False
        End If
        Return True
    End Function
    Function portMatch(config As config, switch As switch) As Boolean
        For Each item As port In config.ports
            For Each item2 As port In switch.Ports
                If item.Number = item2.Number Then
                    For Each item3 As Network In networks
                        If item.Network.VlanNumber = item3.VlanNumber Then
                            item2.Network = item3
                        End If
                    Next
                End If
            Next
        Next
        Return True
    End Function

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        loadGlobalConf("config.txt")
        loadNetworks(networksPath)
    End Sub

    Private Sub SchemaLadenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SchemaLadenToolStripMenuItem.Click
        OpenFileDialog1.InitialDirectory = IO.Path.GetFullPath(templatesPath)
        Select Case OpenFileDialog1.ShowDialog
            Case Windows.Forms.DialogResult.OK
                If loadTemplate(OpenFileDialog1.FileName) Then
                    draw.drawSwitch(currentSwitch)
                    draw.drawLogos(currentSwitch)
                    draw.drawBorder(currentSwitch)
                    draw.drawPorts(currentSwitch)
                End If
        End Select
    End Sub

    Private Sub KonfigLadenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles KonfigLadenToolStripMenuItem.Click
        OpenFileDialog1.InitialDirectory = IO.Path.GetFullPath(locationsPath)
        Select Case OpenFileDialog1.ShowDialog
            Case Windows.Forms.DialogResult.OK
                If loadConfig(OpenFileDialog1.FileName) Then
                    draw.drawSwitch(currentSwitch)
                    draw.drawLogos(currentSwitch)
                    draw.drawBorder(currentSwitch)
                    If portMatch(currentConfig, currentSwitch) Then
                        draw.drawPorts(currentSwitch)
                    Else
                        MsgBox("Portmatch fehlgeschlagen!", MsgBoxStyle.Critical)
                    End If
                End If
        End Select
    End Sub

    Private Sub NetzwerkeBearbeitenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles NetzwerkeBearbeitenToolStripMenuItem.Click
        NetworkForm.ShowDialog()
    End Sub
    Public Sub exportNetworks()
        Dim tempXML As New XDocument(<networks></networks>)
        For Each item As Network In networks
            Dim tempNode As New XElement(<network><name></name><bgcolor></bgcolor><fontcolor></fontcolor><vlan><name></name><number></number></vlan></network>)
            tempNode.Element("name").Value = item.Name
            tempNode.Element("bgcolor").Value = ColorTranslator.ToHtml(item.BgColor)
            tempNode.Element("vlan").Element("name").Value = item.VlanName
            tempNode.Element("vlan").Element("number").Value = item.VlanNumber
            tempNode.Element("fontcolor").Value = ColorTranslator.ToHtml(item.FontColor)
            tempXML.Element("networks").Add(tempNode)
        Next
        Try
            tempXML.Save(networksPath)
        Catch ex As Exception
            MsgBox("Netzwerk Config konnte nicht gespeichert werden!", MsgBoxStyle.Critical)
        End Try
    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        If e.Button = Windows.Forms.MouseButtons.Left Then
            ContextMenuStrip1.Show(New Point(MousePosition.X, MousePosition.Y))
        Else
            ContextMenuStrip1.Close()
        End If
    End Sub
End Class
