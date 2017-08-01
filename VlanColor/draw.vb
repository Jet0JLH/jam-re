Module draw
    Dim grafic As Bitmap
    Const spaceY As Integer = 40
    Const spaceX As Integer = 20
    Sub drawSwitch(switch As switch)
        grafic = New Bitmap(switch.Size.Width + (2 * spaceX), switch.Size.Height + spaceY + 1)
        Dim graficGrafics As Graphics = Graphics.FromImage(grafic)
        graficGrafics.FillRectangle(New SolidBrush(switch.Color), spaceX, spaceY, switch.Size.Width, switch.Size.Height)
        graficGrafics.DrawRectangle(Pens.Black, spaceX, spaceY, switch.Size.Width, switch.Size.Height)
        graficGrafics.DrawString(switch.Name, New Font("Arial", 24), Brushes.Black, spaceX, 0)
        'graficGrafics.DrawImage(loadimage(switch), spaceX + switch.LogoPosition.X, spaceY + switch.LogoPosition.Y)
        'Dim locationStringSize As SizeF = graficGrafics.MeasureString(switch.Location, New Font("Arial", 20))
        'graficGrafics.DrawString(switch.Location, New Font("Arial", 20), Brushes.Black, spaceX + switch.Size.Width - locationStringSize.Width, spaceY - locationStringSize.Height)
        Form1.PictureBox1.Image = grafic
    End Sub
    Function loadimage(path As String) As Image
        Dim tempImage As Image
        tempImage = New Bitmap(1, 1)
        If path <> "" Then
            Try
                tempImage = New Bitmap(IO.Path.Combine(Form1.templatesPath, path))
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
        End If

        Return tempImage
    End Function
    Sub drawPorts(switch As switch)
        Dim graficGrafics As Graphics = Graphics.FromImage(grafic)
        For Each item As port In switch.Ports
            graficGrafics.FillRectangle(New SolidBrush(item.Network.BgColor), spaceX + item.Position.X, spaceY + item.Position.Y, 40, 40)
            graficGrafics.DrawRectangle(Pens.Black, spaceX + item.Position.X, spaceY + item.Position.Y, 40, 40)
            Dim fontSize As SizeF = graficGrafics.MeasureString(item.Number, New Font("Arial", 16))
            graficGrafics.DrawString(item.Number, New Font("Arial", 16), New SolidBrush(item.Network.FontColor), spaceX + item.Position.X + 20 - (fontSize.Width / 2), spaceY + item.Position.Y + 20 - (fontSize.Height / 2))
        Next
    End Sub
    Sub drawBorder(switch As switch)
        Dim graficGrafics As Graphics = Graphics.FromImage(grafic)
        For Each item As border In switch.Borders
            graficGrafics.DrawRectangle(New Pen(item.Color), spaceX + item.Position.X, spaceY + item.Position.Y, item.Size.Width, item.Size.Height)
            Dim textSize As SizeF = graficGrafics.MeasureString(item.Text, New Font("Arial", 8))
            graficGrafics.FillRectangle(New SolidBrush(switch.Color), spaceX + item.Position.X + 5, spaceY + item.Position.Y - (textSize.Height / 2), textSize.Width, textSize.Height)
            graficGrafics.DrawString(item.Text, New Font("Arial", 8), Brushes.Black, spaceX + item.Position.X + 5, spaceY + item.Position.Y - (textSize.Height / 2))
        Next
    End Sub
    Sub drawLogos(switch As switch)
        Dim graficGrafics As Graphics = Graphics.FromImage(grafic)
        For Each item As Logo In switch.Logos
            graficGrafics.DrawImage(loadimage(item.Path), spaceX + item.Position.X, spaceY + item.Position.Y)
        Next
    End Sub
End Module
