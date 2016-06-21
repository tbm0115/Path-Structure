<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Main
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
    Me.mnuSettings = New System.Windows.Forms.ToolStripMenuItem()
    Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolsAdd = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolsAddAll = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolAddFolder = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolsFormat = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolAudit = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolsAuditFile = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolsAuditFolder = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolsAuditDefaultPath = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolsAuditVisualDefaultPath = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolsClipboard = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolsTransfer = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolsTransferFilesByExtension = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolsPreview = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuToolsFolderHeatMap = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuGeneratePathStructure = New System.Windows.Forms.ToolStripMenuItem()
    Me.statProgress = New System.Windows.Forms.ToolStripProgressBar()
    Me.statStatus = New System.Windows.Forms.ToolStripStatusLabel()
    Me.statCurrentPath = New System.Windows.Forms.ToolStripStatusLabel()
    Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
    Me.mnu = New System.Windows.Forms.MenuStrip()
    Me.pnlContainer = New System.Windows.Forms.Panel()
    Me.StatusStrip1.SuspendLayout()
    Me.mnu.SuspendLayout()
    Me.SuspendLayout()
    '
    'mnuSettings
    '
    Me.mnuSettings.Name = "mnuSettings"
    Me.mnuSettings.Size = New System.Drawing.Size(74, 24)
    Me.mnuSettings.Text = "Settings"
    '
    'ToolsToolStripMenuItem
    '
    Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuToolsAdd, Me.mnuToolsFormat, Me.mnuToolAudit, Me.mnuToolsClipboard, Me.mnuToolsTransfer, Me.mnuToolsPreview, Me.mnuToolsFolderHeatMap})
    Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
    Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(57, 24)
    Me.ToolsToolStripMenuItem.Text = "Tools"
    '
    'mnuToolsAdd
    '
    Me.mnuToolsAdd.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuToolsAddAll, Me.mnuToolAddFolder})
    Me.mnuToolsAdd.Name = "mnuToolsAdd"
    Me.mnuToolsAdd.Size = New System.Drawing.Size(208, 26)
    Me.mnuToolsAdd.Text = "Add"
    '
    'mnuToolsAddAll
    '
    Me.mnuToolsAddAll.Enabled = Global.Path_Structure_Maintenance.My.MySettings.Default.blnAddAll
    Me.mnuToolsAddAll.Name = "mnuToolsAddAll"
    Me.mnuToolsAddAll.Size = New System.Drawing.Size(191, 26)
    Me.mnuToolsAddAll.Text = "All Main Folders"
    '
    'mnuToolAddFolder
    '
    Me.mnuToolAddFolder.Enabled = Global.Path_Structure_Maintenance.My.MySettings.Default.blnAddSingle
    Me.mnuToolAddFolder.Name = "mnuToolAddFolder"
    Me.mnuToolAddFolder.Size = New System.Drawing.Size(191, 26)
    Me.mnuToolAddFolder.Text = "Folder"
    '
    'mnuToolsFormat
    '
    Me.mnuToolsFormat.Enabled = Global.Path_Structure_Maintenance.My.MySettings.Default.blnFormat
    Me.mnuToolsFormat.Name = "mnuToolsFormat"
    Me.mnuToolsFormat.Size = New System.Drawing.Size(208, 26)
    Me.mnuToolsFormat.Text = "Format"
    '
    'mnuToolAudit
    '
    Me.mnuToolAudit.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuToolsAuditFile, Me.mnuToolsAuditFolder, Me.mnuToolsAuditDefaultPath, Me.mnuToolsAuditVisualDefaultPath})
    Me.mnuToolAudit.Enabled = Global.Path_Structure_Maintenance.My.MySettings.Default.blnAudit
    Me.mnuToolAudit.Name = "mnuToolAudit"
    Me.mnuToolAudit.Size = New System.Drawing.Size(208, 26)
    Me.mnuToolAudit.Text = "Audit"
    '
    'mnuToolsAuditFile
    '
    Me.mnuToolsAuditFile.Name = "mnuToolsAuditFile"
    Me.mnuToolsAuditFile.Size = New System.Drawing.Size(209, 26)
    Me.mnuToolsAuditFile.Text = "Audit File"
    '
    'mnuToolsAuditFolder
    '
    Me.mnuToolsAuditFolder.Name = "mnuToolsAuditFolder"
    Me.mnuToolsAuditFolder.Size = New System.Drawing.Size(209, 26)
    Me.mnuToolsAuditFolder.Text = "Audit Folder"
    '
    'mnuToolsAuditDefaultPath
    '
    Me.mnuToolsAuditDefaultPath.Name = "mnuToolsAuditDefaultPath"
    Me.mnuToolsAuditDefaultPath.Size = New System.Drawing.Size(209, 26)
    Me.mnuToolsAuditDefaultPath.Text = "Default Path"
    '
    'mnuToolsAuditVisualDefaultPath
    '
    Me.mnuToolsAuditVisualDefaultPath.Name = "mnuToolsAuditVisualDefaultPath"
    Me.mnuToolsAuditVisualDefaultPath.Size = New System.Drawing.Size(209, 26)
    Me.mnuToolsAuditVisualDefaultPath.Text = "Visual Default Path"
    '
    'mnuToolsClipboard
    '
    Me.mnuToolsClipboard.Enabled = Global.Path_Structure_Maintenance.My.MySettings.Default.blnClipboard
    Me.mnuToolsClipboard.Name = "mnuToolsClipboard"
    Me.mnuToolsClipboard.Size = New System.Drawing.Size(208, 26)
    Me.mnuToolsClipboard.Text = "Copy File Structure"
    '
    'mnuToolsTransfer
    '
    Me.mnuToolsTransfer.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuToolsTransferFilesByExtension})
    Me.mnuToolsTransfer.Name = "mnuToolsTransfer"
    Me.mnuToolsTransfer.Size = New System.Drawing.Size(208, 26)
    Me.mnuToolsTransfer.Text = "Transfer"
    '
    'mnuToolsTransferFilesByExtension
    '
    Me.mnuToolsTransferFilesByExtension.Name = "mnuToolsTransferFilesByExtension"
    Me.mnuToolsTransferFilesByExtension.Size = New System.Drawing.Size(200, 26)
    Me.mnuToolsTransferFilesByExtension.Text = "Files by Extension"
    '
    'mnuToolsPreview
    '
    Me.mnuToolsPreview.Enabled = Global.Path_Structure_Maintenance.My.MySettings.Default.blnPreview
    Me.mnuToolsPreview.Name = "mnuToolsPreview"
    Me.mnuToolsPreview.Size = New System.Drawing.Size(208, 26)
    Me.mnuToolsPreview.Text = "Preview"
    '
    'mnuToolsFolderHeatMap
    '
    Me.mnuToolsFolderHeatMap.Name = "mnuToolsFolderHeatMap"
    Me.mnuToolsFolderHeatMap.Size = New System.Drawing.Size(208, 26)
    Me.mnuToolsFolderHeatMap.Text = "Folder Heat Map"
    '
    'mnuGeneratePathStructure
    '
    Me.mnuGeneratePathStructure.Name = "mnuGeneratePathStructure"
    Me.mnuGeneratePathStructure.Size = New System.Drawing.Size(177, 24)
    Me.mnuGeneratePathStructure.Text = "Generate Path Structure"
    '
    'statProgress
    '
    Me.statProgress.Name = "statProgress"
    Me.statProgress.Size = New System.Drawing.Size(100, 19)
    '
    'statStatus
    '
    Me.statStatus.Name = "statStatus"
    Me.statStatus.Size = New System.Drawing.Size(49, 20)
    Me.statStatus.Text = "Status"
    '
    'statCurrentPath
    '
    Me.statCurrentPath.Name = "statCurrentPath"
    Me.statCurrentPath.Size = New System.Drawing.Size(38, 20)
    Me.statCurrentPath.Text = "Path"
    '
    'StatusStrip1
    '
    Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
    Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.statProgress, Me.statStatus, Me.statCurrentPath})
    Me.StatusStrip1.Location = New System.Drawing.Point(0, 373)
    Me.StatusStrip1.Name = "StatusStrip1"
    Me.StatusStrip1.Size = New System.Drawing.Size(423, 25)
    Me.StatusStrip1.TabIndex = 1
    Me.StatusStrip1.Text = "StatusStrip1"
    '
    'mnu
    '
    Me.mnu.ImageScalingSize = New System.Drawing.Size(20, 20)
    Me.mnu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuSettings, Me.ToolsToolStripMenuItem, Me.mnuGeneratePathStructure})
    Me.mnu.Location = New System.Drawing.Point(0, 0)
    Me.mnu.Name = "mnu"
    Me.mnu.Size = New System.Drawing.Size(423, 28)
    Me.mnu.TabIndex = 2
    Me.mnu.Text = "MenuStrip1"
    '
    'pnlContainer
    '
    Me.pnlContainer.Dock = System.Windows.Forms.DockStyle.Fill
    Me.pnlContainer.Location = New System.Drawing.Point(0, 28)
    Me.pnlContainer.Name = "pnlContainer"
    Me.pnlContainer.Size = New System.Drawing.Size(423, 345)
    Me.pnlContainer.TabIndex = 5
    '
    'Main
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(12.0!, 25.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.ClientSize = New System.Drawing.Size(423, 398)
    Me.Controls.Add(Me.pnlContainer)
    Me.Controls.Add(Me.StatusStrip1)
    Me.Controls.Add(Me.mnu)
    Me.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
    Me.Name = "Main"
    Me.Text = "Path Structure Maintenance"
    Me.StatusStrip1.ResumeLayout(False)
    Me.StatusStrip1.PerformLayout()
    Me.mnu.ResumeLayout(False)
    Me.mnu.PerformLayout()
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents mnuSettings As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents ToolsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuToolsAdd As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuToolsFormat As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuToolAudit As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuToolsAddAll As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuToolAddFolder As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuToolsAuditFile As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuToolsAuditDefaultPath As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuToolsAuditFolder As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents statStatus As System.Windows.Forms.ToolStripStatusLabel
  Friend WithEvents statCurrentPath As System.Windows.Forms.ToolStripStatusLabel
  Friend WithEvents mnuToolsClipboard As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuToolsTransfer As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuToolsTransferFilesByExtension As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents statProgress As System.Windows.Forms.ToolStripProgressBar
  Friend WithEvents mnuToolsPreview As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuGeneratePathStructure As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuToolsFolderHeatMap As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuToolsAuditVisualDefaultPath As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
  Friend WithEvents mnu As System.Windows.Forms.MenuStrip
  Friend WithEvents pnlContainer As System.Windows.Forms.Panel

End Class
