Public Class NetworkForm

    Private Sub NetworkForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DataGridView1.Rows.Clear()
        For Each item As Network In Form1.networks
            Dim temprow As New DataGridViewRow()
            Dim name As New DataGridViewTextBoxCell()
            Dim number As New DataGridViewTextBoxCell()
            Dim vlanname As New DataGridViewTextBoxCell()
            Dim bgcolor As New DataGridViewTextBoxCell()
            Dim fontcolor As New DataGridViewTextBoxCell()
            name.Value = item.Name
            number.Value = item.VlanNumber
            vlanname.Value = item.VlanName
            bgcolor.Style.BackColor = item.BgColor
            fontcolor.Style.BackColor = item.FontColor
            temprow.Cells.Add(name)
            temprow.Cells.Add(number)
            temprow.Cells.Add(vlanname)
            temprow.Cells.Add(bgcolor)
            temprow.Cells.Add(fontcolor)
            DataGridView1.Rows.Add(temprow)
        Next
    End Sub

    Private Sub DataGridView1_DoubleClick(sender As Object, e As MouseEventArgs) Handles DataGridView1.DoubleClick
        Dim cell As DataGridViewCell = DataGridView1.SelectedCells(0)
        If cell.ColumnIndex = 4 Or cell.ColumnIndex = 3 Then
            ColorDialog1.Color = cell.Style.BackColor
            Select Case ColorDialog1.ShowDialog
                Case Windows.Forms.DialogResult.OK
                    cell.Style.BackColor = ColorDialog1.Color
            End Select
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        With Form1.networks
            .Clear()
            For Each item As DataGridViewRow In DataGridView1.Rows
                If item.IsNewRow = False Then
                    Dim tempNetwork As New Network(item.Cells(0).Value, item.Cells(3).Style.BackColor)
                    tempNetwork.VlanNumber = item.Cells(1).Value
                    tempNetwork.VlanName = item.Cells(2).Value
                    tempNetwork.FontColor = item.Cells(4).Style.BackColor
                    .Add(tempNetwork)
                End If
            Next
        End With
        Form1.exportNetworks()
        Me.Close()
    End Sub
End Class