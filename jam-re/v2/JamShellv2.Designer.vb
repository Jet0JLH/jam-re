<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class JamShellv2
    Inherits System.Windows.Forms.Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(JamShellv2))
        Me.TitleLabel = New System.Windows.Forms.Label()
        Me.VersionsLabel = New System.Windows.Forms.Label()
        Me.Pinwall = New System.Windows.Forms.RichTextBox()
        Me.SuspendLayout()
        '
        'TitleLabel
        '
        Me.TitleLabel.AutoSize = True
        Me.TitleLabel.BackColor = System.Drawing.Color.Transparent
        Me.TitleLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TitleLabel.Location = New System.Drawing.Point(12, 3)
        Me.TitleLabel.Name = "TitleLabel"
        Me.TitleLabel.Size = New System.Drawing.Size(66, 13)
        Me.TitleLabel.TabIndex = 5
        Me.TitleLabel.Text = "Jam Skript"
        '
        'VersionsLabel
        '
        Me.VersionsLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.VersionsLabel.AutoSize = True
        Me.VersionsLabel.BackColor = System.Drawing.Color.Transparent
        Me.VersionsLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.VersionsLabel.Location = New System.Drawing.Point(444, 3)
        Me.VersionsLabel.Name = "VersionsLabel"
        Me.VersionsLabel.Size = New System.Drawing.Size(42, 13)
        Me.VersionsLabel.TabIndex = 4
        Me.VersionsLabel.Text = "Version"
        '
        'Pinwall
        '
        Me.Pinwall.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Pinwall.BackColor = System.Drawing.Color.LightBlue
        Me.Pinwall.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.Pinwall.Cursor = System.Windows.Forms.Cursors.Default
        Me.Pinwall.Location = New System.Drawing.Point(13, 19)
        Me.Pinwall.Name = "Pinwall"
        Me.Pinwall.ReadOnly = True
        Me.Pinwall.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None
        Me.Pinwall.Size = New System.Drawing.Size(501, 234)
        Me.Pinwall.TabIndex = 3
        Me.Pinwall.Text = ""
        '
        'JamShellv2
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.ControlDark
        Me.ClientSize = New System.Drawing.Size(526, 265)
        Me.Controls.Add(Me.TitleLabel)
        Me.Controls.Add(Me.VersionsLabel)
        Me.Controls.Add(Me.Pinwall)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "JamShellv2"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "JamShellv2"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents TitleLabel As Label
    Friend WithEvents VersionsLabel As Label
    Friend WithEvents Pinwall As RichTextBox
End Class
