<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class NetworkForm
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Wird vom Windows Form-Designer benötigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.ColumnName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColumnVlanNummer = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColumnVlanName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColumnBgColor = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColumnFontColor = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ColorDialog1 = New System.Windows.Forms.ColorDialog()
        Me.Button1 = New System.Windows.Forms.Button()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ColumnName, Me.ColumnVlanNummer, Me.ColumnVlanName, Me.ColumnBgColor, Me.ColumnFontColor})
        Me.DataGridView1.Location = New System.Drawing.Point(0, 0)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(802, 336)
        Me.DataGridView1.TabIndex = 0
        '
        'ColumnName
        '
        Me.ColumnName.HeaderText = "Name"
        Me.ColumnName.Name = "ColumnName"
        '
        'ColumnVlanNummer
        '
        Me.ColumnVlanNummer.HeaderText = "Vlan Nummer"
        Me.ColumnVlanNummer.Name = "ColumnVlanNummer"
        '
        'ColumnVlanName
        '
        Me.ColumnVlanName.HeaderText = "Vlan Name"
        Me.ColumnVlanName.Name = "ColumnVlanName"
        '
        'ColumnBgColor
        '
        Me.ColumnBgColor.HeaderText = "Vlan Farbe"
        Me.ColumnBgColor.Name = "ColumnBgColor"
        Me.ColumnBgColor.ReadOnly = True
        '
        'ColumnFontColor
        '
        Me.ColumnFontColor.HeaderText = "Schriftfarbe"
        Me.ColumnFontColor.Name = "ColumnFontColor"
        Me.ColumnFontColor.ReadOnly = True
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(715, 343)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "Speichern"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'NetworkForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(802, 378)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.DataGridView1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "NetworkForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "NetworkForm"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents DataGridView1 As System.Windows.Forms.DataGridView
    Friend WithEvents ColumnName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColumnVlanNummer As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColumnVlanName As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColumnBgColor As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColumnFontColor As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents ColorDialog1 As System.Windows.Forms.ColorDialog
    Friend WithEvents Button1 As System.Windows.Forms.Button
End Class
