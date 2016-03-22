<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Settings
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
    Me.btnAddContextMenu = New System.Windows.Forms.Button()
    Me.btnRemoveContextMenu = New System.Windows.Forms.Button()
    Me.GroupBox1 = New System.Windows.Forms.GroupBox()
    Me.chkTransfer = New System.Windows.Forms.CheckBox()
    Me.CheckBox5 = New System.Windows.Forms.CheckBox()
    Me.CheckBox4 = New System.Windows.Forms.CheckBox()
    Me.CheckBox3 = New System.Windows.Forms.CheckBox()
    Me.CheckBox2 = New System.Windows.Forms.CheckBox()
    Me.CheckBox1 = New System.Windows.Forms.CheckBox()
    Me.Label1 = New System.Windows.Forms.Label()
    Me.txtPathStructure = New System.Windows.Forms.TextBox()
    Me.btnBrowse = New System.Windows.Forms.Button()
    Me.GroupBox1.SuspendLayout()
    Me.SuspendLayout()
    '
    'btnAddContextMenu
    '
    Me.btnAddContextMenu.Location = New System.Drawing.Point(12, 342)
    Me.btnAddContextMenu.Name = "btnAddContextMenu"
    Me.btnAddContextMenu.Size = New System.Drawing.Size(216, 66)
    Me.btnAddContextMenu.TabIndex = 0
    Me.btnAddContextMenu.Text = "Add/Update Windows Context Menu"
    Me.btnAddContextMenu.UseVisualStyleBackColor = True
    '
    'btnRemoveContextMenu
    '
    Me.btnRemoveContextMenu.Location = New System.Drawing.Point(234, 342)
    Me.btnRemoveContextMenu.Name = "btnRemoveContextMenu"
    Me.btnRemoveContextMenu.Size = New System.Drawing.Size(216, 66)
    Me.btnRemoveContextMenu.TabIndex = 1
    Me.btnRemoveContextMenu.Text = "Remove Windows Context Menu"
    Me.btnRemoveContextMenu.UseVisualStyleBackColor = True
    '
    'GroupBox1
    '
    Me.GroupBox1.Controls.Add(Me.chkTransfer)
    Me.GroupBox1.Controls.Add(Me.CheckBox5)
    Me.GroupBox1.Controls.Add(Me.CheckBox4)
    Me.GroupBox1.Controls.Add(Me.CheckBox3)
    Me.GroupBox1.Controls.Add(Me.CheckBox2)
    Me.GroupBox1.Controls.Add(Me.CheckBox1)
    Me.GroupBox1.Location = New System.Drawing.Point(13, 88)
    Me.GroupBox1.Name = "GroupBox1"
    Me.GroupBox1.Size = New System.Drawing.Size(440, 248)
    Me.GroupBox1.TabIndex = 2
    Me.GroupBox1.TabStop = False
    Me.GroupBox1.Text = "Context Menu Options"
    '
    'chkTransfer
    '
    Me.chkTransfer.AutoSize = True
    Me.chkTransfer.Checked = Global.Path_Structure_Maintenance.My.MySettings.Default.blnTransferByExtension
    Me.chkTransfer.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.Path_Structure_Maintenance.My.MySettings.Default, "blnTransferByExtension", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
    Me.chkTransfer.Location = New System.Drawing.Point(7, 205)
    Me.chkTransfer.Name = "chkTransfer"
    Me.chkTransfer.Size = New System.Drawing.Size(272, 29)
    Me.chkTransfer.TabIndex = 5
    Me.chkTransfer.Text = "Transfer Files By Extension"
    Me.chkTransfer.UseVisualStyleBackColor = True
    '
    'CheckBox5
    '
    Me.CheckBox5.AutoSize = True
    Me.CheckBox5.Checked = Global.Path_Structure_Maintenance.My.MySettings.Default.blnClipboard
    Me.CheckBox5.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.Path_Structure_Maintenance.My.MySettings.Default, "blnClipboard", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
    Me.CheckBox5.Location = New System.Drawing.Point(7, 170)
    Me.CheckBox5.Name = "CheckBox5"
    Me.CheckBox5.Size = New System.Drawing.Size(358, 29)
    Me.CheckBox5.TabIndex = 4
    Me.CheckBox5.Text = "Create/Send File Format to Clipboard"
    Me.CheckBox5.UseVisualStyleBackColor = True
    '
    'CheckBox4
    '
    Me.CheckBox4.AutoSize = True
    Me.CheckBox4.Checked = Global.Path_Structure_Maintenance.My.MySettings.Default.blnAudit
    Me.CheckBox4.CheckState = System.Windows.Forms.CheckState.Checked
    Me.CheckBox4.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.Path_Structure_Maintenance.My.MySettings.Default, "blnAudit", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
    Me.CheckBox4.Location = New System.Drawing.Point(7, 135)
    Me.CheckBox4.Name = "CheckBox4"
    Me.CheckBox4.Size = New System.Drawing.Size(79, 29)
    Me.CheckBox4.TabIndex = 3
    Me.CheckBox4.Text = "Audit"
    Me.CheckBox4.UseVisualStyleBackColor = True
    '
    'CheckBox3
    '
    Me.CheckBox3.AutoSize = True
    Me.CheckBox3.Checked = Global.Path_Structure_Maintenance.My.MySettings.Default.blnFormat
    Me.CheckBox3.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.Path_Structure_Maintenance.My.MySettings.Default, "blnFormat", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
    Me.CheckBox3.Location = New System.Drawing.Point(7, 100)
    Me.CheckBox3.Name = "CheckBox3"
    Me.CheckBox3.Size = New System.Drawing.Size(188, 29)
    Me.CheckBox3.TabIndex = 2
    Me.CheckBox3.Text = "Format File Name"
    Me.CheckBox3.UseVisualStyleBackColor = True
    '
    'CheckBox2
    '
    Me.CheckBox2.AutoSize = True
    Me.CheckBox2.Checked = Global.Path_Structure_Maintenance.My.MySettings.Default.blnAddSingle
    Me.CheckBox2.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.Path_Structure_Maintenance.My.MySettings.Default, "blnAddSingle", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
    Me.CheckBox2.Location = New System.Drawing.Point(7, 65)
    Me.CheckBox2.Name = "CheckBox2"
    Me.CheckBox2.Size = New System.Drawing.Size(190, 29)
    Me.CheckBox2.TabIndex = 1
    Me.CheckBox2.Text = "Add Single Folder"
    Me.CheckBox2.UseVisualStyleBackColor = True
    '
    'CheckBox1
    '
    Me.CheckBox1.AutoSize = True
    Me.CheckBox1.Checked = Global.Path_Structure_Maintenance.My.MySettings.Default.blnAddAll
    Me.CheckBox1.CheckState = System.Windows.Forms.CheckState.Checked
    Me.CheckBox1.DataBindings.Add(New System.Windows.Forms.Binding("Checked", Global.Path_Structure_Maintenance.My.MySettings.Default, "blnAddAll", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
    Me.CheckBox1.Location = New System.Drawing.Point(7, 30)
    Me.CheckBox1.Name = "CheckBox1"
    Me.CheckBox1.Size = New System.Drawing.Size(215, 29)
    Me.CheckBox1.TabIndex = 0
    Me.CheckBox1.Text = "Add All Main Folders"
    Me.CheckBox1.UseVisualStyleBackColor = True
    '
    'Label1
    '
    Me.Label1.AutoSize = True
    Me.Label1.Location = New System.Drawing.Point(13, 13)
    Me.Label1.Name = "Label1"
    Me.Label1.Size = New System.Drawing.Size(142, 25)
    Me.Label1.TabIndex = 3
    Me.Label1.Text = "Path Structure:"
    '
    'txtPathStructure
    '
    Me.txtPathStructure.DataBindings.Add(New System.Windows.Forms.Binding("Text", Global.Path_Structure_Maintenance.My.MySettings.Default, "SettingsPath", True, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged))
    Me.txtPathStructure.Location = New System.Drawing.Point(12, 41)
    Me.txtPathStructure.Name = "txtPathStructure"
    Me.txtPathStructure.ReadOnly = True
    Me.txtPathStructure.Size = New System.Drawing.Size(324, 30)
    Me.txtPathStructure.TabIndex = 4
    Me.txtPathStructure.Text = Global.Path_Structure_Maintenance.My.MySettings.Default.SettingsPath
    '
    'btnBrowse
    '
    Me.btnBrowse.Location = New System.Drawing.Point(342, 41)
    Me.btnBrowse.Name = "btnBrowse"
    Me.btnBrowse.Size = New System.Drawing.Size(108, 30)
    Me.btnBrowse.TabIndex = 5
    Me.btnBrowse.Text = "Browse..."
    Me.btnBrowse.UseVisualStyleBackColor = True
    '
    'Settings
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(465, 414)
    Me.Controls.Add(Me.btnBrowse)
    Me.Controls.Add(Me.txtPathStructure)
    Me.Controls.Add(Me.Label1)
    Me.Controls.Add(Me.GroupBox1)
    Me.Controls.Add(Me.btnRemoveContextMenu)
    Me.Controls.Add(Me.btnAddContextMenu)
    Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
    Me.Name = "Settings"
    Me.Text = "Settings"
    Me.GroupBox1.ResumeLayout(False)
    Me.GroupBox1.PerformLayout()
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents btnAddContextMenu As System.Windows.Forms.Button
  Friend WithEvents btnRemoveContextMenu As System.Windows.Forms.Button
  Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
  Friend WithEvents CheckBox3 As System.Windows.Forms.CheckBox
  Friend WithEvents CheckBox2 As System.Windows.Forms.CheckBox
  Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
  Friend WithEvents CheckBox4 As System.Windows.Forms.CheckBox
  Friend WithEvents CheckBox5 As System.Windows.Forms.CheckBox
  Friend WithEvents chkTransfer As System.Windows.Forms.CheckBox
  Friend WithEvents Label1 As System.Windows.Forms.Label
  Friend WithEvents txtPathStructure As System.Windows.Forms.TextBox
  Friend WithEvents btnBrowse As System.Windows.Forms.Button
End Class
