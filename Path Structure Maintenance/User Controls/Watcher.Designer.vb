<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Watcher
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
    Me.components = New System.ComponentModel.Container()
    Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Watcher))
    Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
    Me.statPath = New System.Windows.Forms.ToolStripStatusLabel()
    Me.statWatchLabel = New System.Windows.Forms.ToolStripStatusLabel()
    Me.mnuWatcher = New System.Windows.Forms.MenuStrip()
    Me.mnuWatchCreate = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuWatchCreateFolder = New System.Windows.Forms.ToolStripMenuItem()
    Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.CopyFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.CopySelectedPathToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.mnuDisableRenameOnMove = New System.Windows.Forms.ToolStripMenuItem()
    Me.ProcessToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.SendToArchiveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.CollapseFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
    Me.lblPath = New System.Windows.Forms.Label()
    Me.trvPathStructure = New System.Windows.Forms.TreeView()
    Me.imgFSO = New System.Windows.Forms.ImageList(Me.components)
    Me.lblReal = New System.Windows.Forms.Label()
    Me.trvFileSystem = New System.Windows.Forms.TreeView()
    Me.spltWorkSpace = New System.Windows.Forms.SplitContainer()
    Me.StatusStrip1.SuspendLayout()
    Me.mnuWatcher.SuspendLayout()
    CType(Me.spltWorkSpace, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.spltWorkSpace.Panel1.SuspendLayout()
    Me.spltWorkSpace.Panel2.SuspendLayout()
    Me.spltWorkSpace.SuspendLayout()
    Me.SuspendLayout()
    '
    'StatusStrip1
    '
    Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
    Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.statPath, Me.statWatchLabel})
    Me.StatusStrip1.Location = New System.Drawing.Point(0, 263)
    Me.StatusStrip1.Name = "StatusStrip1"
    Me.StatusStrip1.Size = New System.Drawing.Size(1074, 29)
    Me.StatusStrip1.TabIndex = 0
    Me.StatusStrip1.Text = "StatusStrip1"
    '
    'statPath
    '
    Me.statPath.BorderSides = CType((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom), System.Windows.Forms.ToolStripStatusLabelBorderSides)
    Me.statPath.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
    Me.statPath.Name = "statPath"
    Me.statPath.Size = New System.Drawing.Size(996, 24)
    Me.statPath.Spring = True
    Me.statPath.Text = "[Path]"
    Me.statPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
    '
    'statWatchLabel
    '
    Me.statWatchLabel.BorderSides = CType((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom), System.Windows.Forms.ToolStripStatusLabelBorderSides)
    Me.statWatchLabel.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter
    Me.statWatchLabel.Name = "statWatchLabel"
    Me.statWatchLabel.Size = New System.Drawing.Size(63, 24)
    Me.statWatchLabel.Text = "[Status]"
    '
    'mnuWatcher
    '
    Me.mnuWatcher.ImageScalingSize = New System.Drawing.Size(20, 20)
    Me.mnuWatcher.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuWatchCreate, Me.CopyToolStripMenuItem, Me.ProcessToolStripMenuItem, Me.SettingsToolStripMenuItem})
    Me.mnuWatcher.Location = New System.Drawing.Point(0, 0)
    Me.mnuWatcher.Name = "mnuWatcher"
    Me.mnuWatcher.Size = New System.Drawing.Size(1074, 28)
    Me.mnuWatcher.TabIndex = 1
    Me.mnuWatcher.Text = "MenuStrip1"
    '
    'mnuWatchCreate
    '
    Me.mnuWatchCreate.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuWatchCreateFolder})
    Me.mnuWatchCreate.Name = "mnuWatchCreate"
    Me.mnuWatchCreate.Size = New System.Drawing.Size(64, 24)
    Me.mnuWatchCreate.Text = "Create"
    '
    'mnuWatchCreateFolder
    '
    Me.mnuWatchCreateFolder.Image = CType(resources.GetObject("mnuWatchCreateFolder.Image"), System.Drawing.Image)
    Me.mnuWatchCreateFolder.Name = "mnuWatchCreateFolder"
    Me.mnuWatchCreateFolder.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.N), System.Windows.Forms.Keys)
    Me.mnuWatchCreateFolder.Size = New System.Drawing.Size(225, 26)
    Me.mnuWatchCreateFolder.Text = "Folders"
    Me.mnuWatchCreateFolder.ToolTipText = "Creates the path structure folders from the current folder"
    '
    'CopyToolStripMenuItem
    '
    Me.CopyToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyFolderToolStripMenuItem, Me.CopySelectedPathToolStripMenuItem})
    Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
    Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(55, 24)
    Me.CopyToolStripMenuItem.Text = "Copy"
    '
    'CopyFolderToolStripMenuItem
    '
    Me.CopyFolderToolStripMenuItem.Name = "CopyFolderToolStripMenuItem"
    Me.CopyFolderToolStripMenuItem.ShortcutKeys = CType(((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.Shift) _
            Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
    Me.CopyFolderToolStripMenuItem.Size = New System.Drawing.Size(263, 26)
    Me.CopyFolderToolStripMenuItem.Text = "Copy Folder"
    '
    'CopySelectedPathToolStripMenuItem
    '
    Me.CopySelectedPathToolStripMenuItem.Name = "CopySelectedPathToolStripMenuItem"
    Me.CopySelectedPathToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
    Me.CopySelectedPathToolStripMenuItem.Size = New System.Drawing.Size(263, 26)
    Me.CopySelectedPathToolStripMenuItem.Text = "Copy Selected Path"
    '
    'SettingsToolStripMenuItem
    '
    Me.SettingsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuDisableRenameOnMove})
    Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
    Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(74, 24)
    Me.SettingsToolStripMenuItem.Text = "Settings"
    '
    'mnuDisableRenameOnMove
    '
    Me.mnuDisableRenameOnMove.CheckOnClick = True
    Me.mnuDisableRenameOnMove.Name = "mnuDisableRenameOnMove"
    Me.mnuDisableRenameOnMove.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.M), System.Windows.Forms.Keys)
    Me.mnuDisableRenameOnMove.Size = New System.Drawing.Size(305, 26)
    Me.mnuDisableRenameOnMove.Text = "Disable Rename on Move"
    '
    'ProcessToolStripMenuItem
    '
    Me.ProcessToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SendToArchiveToolStripMenuItem, Me.CollapseFolderToolStripMenuItem})
    Me.ProcessToolStripMenuItem.Name = "ProcessToolStripMenuItem"
    Me.ProcessToolStripMenuItem.Size = New System.Drawing.Size(70, 24)
    Me.ProcessToolStripMenuItem.Text = "Process"
    '
    'SendToArchiveToolStripMenuItem
    '
    Me.SendToArchiveToolStripMenuItem.Name = "SendToArchiveToolStripMenuItem"
    Me.SendToArchiveToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.A), System.Windows.Forms.Keys)
    Me.SendToArchiveToolStripMenuItem.Size = New System.Drawing.Size(239, 26)
    Me.SendToArchiveToolStripMenuItem.Text = "Send To Archive"
    '
    'CollapseFolderToolStripMenuItem
    '
    Me.CollapseFolderToolStripMenuItem.Name = "CollapseFolderToolStripMenuItem"
    Me.CollapseFolderToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Alt Or System.Windows.Forms.Keys.C), System.Windows.Forms.Keys)
    Me.CollapseFolderToolStripMenuItem.Size = New System.Drawing.Size(239, 26)
    Me.CollapseFolderToolStripMenuItem.Text = "Collapse Folder"
    '
    'lblPath
    '
    Me.lblPath.AutoSize = True
    Me.lblPath.Dock = System.Windows.Forms.DockStyle.Top
    Me.lblPath.Location = New System.Drawing.Point(3, 3)
    Me.lblPath.Name = "lblPath"
    Me.lblPath.Size = New System.Drawing.Size(175, 17)
    Me.lblPath.TabIndex = 2
    Me.lblPath.Text = "Path Structure File System"
    '
    'trvPathStructure
    '
    Me.trvPathStructure.AllowDrop = True
    Me.trvPathStructure.Dock = System.Windows.Forms.DockStyle.Fill
    Me.trvPathStructure.FullRowSelect = True
    Me.trvPathStructure.HideSelection = False
    Me.trvPathStructure.HotTracking = True
    Me.trvPathStructure.ImageIndex = 0
    Me.trvPathStructure.ImageList = Me.imgFSO
    Me.trvPathStructure.Location = New System.Drawing.Point(3, 20)
    Me.trvPathStructure.Name = "trvPathStructure"
    Me.trvPathStructure.SelectedImageIndex = 0
    Me.trvPathStructure.Size = New System.Drawing.Size(545, 212)
    Me.trvPathStructure.TabIndex = 3
    '
    'imgFSO
    '
    Me.imgFSO.ImageStream = CType(resources.GetObject("imgFSO.ImageStream"), System.Windows.Forms.ImageListStreamer)
    Me.imgFSO.TransparentColor = System.Drawing.Color.Transparent
    Me.imgFSO.Images.SetKeyName(0, "Folder_256x256.png")
    Me.imgFSO.Images.SetKeyName(1, "Generic_Document.png")
    '
    'lblReal
    '
    Me.lblReal.AutoSize = True
    Me.lblReal.Dock = System.Windows.Forms.DockStyle.Top
    Me.lblReal.Location = New System.Drawing.Point(3, 3)
    Me.lblReal.Name = "lblReal"
    Me.lblReal.Size = New System.Drawing.Size(113, 17)
    Me.lblReal.TabIndex = 0
    Me.lblReal.Text = "Real File System"
    '
    'trvFileSystem
    '
    Me.trvFileSystem.AllowDrop = True
    Me.trvFileSystem.Dock = System.Windows.Forms.DockStyle.Fill
    Me.trvFileSystem.HideSelection = False
    Me.trvFileSystem.HotTracking = True
    Me.trvFileSystem.ImageIndex = 0
    Me.trvFileSystem.ImageList = Me.imgFSO
    Me.trvFileSystem.Location = New System.Drawing.Point(3, 20)
    Me.trvFileSystem.Name = "trvFileSystem"
    Me.trvFileSystem.SelectedImageIndex = 0
    Me.trvFileSystem.Size = New System.Drawing.Size(513, 212)
    Me.trvFileSystem.TabIndex = 1
    '
    'spltWorkSpace
    '
    Me.spltWorkSpace.Dock = System.Windows.Forms.DockStyle.Fill
    Me.spltWorkSpace.Location = New System.Drawing.Point(0, 28)
    Me.spltWorkSpace.Name = "spltWorkSpace"
    '
    'spltWorkSpace.Panel1
    '
    Me.spltWorkSpace.Panel1.Controls.Add(Me.trvFileSystem)
    Me.spltWorkSpace.Panel1.Controls.Add(Me.lblReal)
    Me.spltWorkSpace.Panel1.Padding = New System.Windows.Forms.Padding(3)
    '
    'spltWorkSpace.Panel2
    '
    Me.spltWorkSpace.Panel2.Controls.Add(Me.trvPathStructure)
    Me.spltWorkSpace.Panel2.Controls.Add(Me.lblPath)
    Me.spltWorkSpace.Panel2.Padding = New System.Windows.Forms.Padding(3)
    Me.spltWorkSpace.Size = New System.Drawing.Size(1074, 235)
    Me.spltWorkSpace.SplitterDistance = 519
    Me.spltWorkSpace.TabIndex = 3
    '
    'Watcher
    '
    Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
    Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
    Me.Controls.Add(Me.spltWorkSpace)
    Me.Controls.Add(Me.StatusStrip1)
    Me.Controls.Add(Me.mnuWatcher)
    Me.Name = "Watcher"
    Me.Size = New System.Drawing.Size(1074, 292)
    Me.StatusStrip1.ResumeLayout(False)
    Me.StatusStrip1.PerformLayout()
    Me.mnuWatcher.ResumeLayout(False)
    Me.mnuWatcher.PerformLayout()
    Me.spltWorkSpace.Panel1.ResumeLayout(False)
    Me.spltWorkSpace.Panel1.PerformLayout()
    Me.spltWorkSpace.Panel2.ResumeLayout(False)
    Me.spltWorkSpace.Panel2.PerformLayout()
    CType(Me.spltWorkSpace, System.ComponentModel.ISupportInitialize).EndInit()
    Me.spltWorkSpace.ResumeLayout(False)
    Me.ResumeLayout(False)
    Me.PerformLayout()

  End Sub
  Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
  Friend WithEvents statWatchLabel As System.Windows.Forms.ToolStripStatusLabel
  Friend WithEvents mnuWatcher As System.Windows.Forms.MenuStrip
  Friend WithEvents mnuWatchCreate As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuWatchCreateFolder As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents statPath As System.Windows.Forms.ToolStripStatusLabel
  Friend WithEvents lblPath As System.Windows.Forms.Label
  Friend WithEvents trvPathStructure As System.Windows.Forms.TreeView
  Friend WithEvents lblReal As System.Windows.Forms.Label
  Friend WithEvents trvFileSystem As System.Windows.Forms.TreeView
  Friend WithEvents spltWorkSpace As System.Windows.Forms.SplitContainer
  Friend WithEvents imgFSO As System.Windows.Forms.ImageList
  Friend WithEvents CopyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents SettingsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents mnuDisableRenameOnMove As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents CopyFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents CopySelectedPathToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents ProcessToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents SendToArchiveToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
  Friend WithEvents CollapseFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
