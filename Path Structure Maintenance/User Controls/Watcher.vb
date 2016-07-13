Imports PathStructureClass

Public Class Watcher
  Private _pstruct As PathStructure
  Private _exploreWatcher As ExplorerWatcher
  Private _curPath As Path

  Public Property CurrentPath As Path
    Get
      Return _curPath
    End Get
    Set(value As Path)
      _curPath = value
      statPath.Text = _curPath.UNCPath
      FillRealTree(_curPath)
      FillPathTree(_curPath)
    End Set
  End Property

  Public Sub New(ByVal PStruct As PathStructure)
    ' This call is required by the designer.
    InitializeComponent()

    ' Add any initialization after the InitializeComponent() call.
    _pstruct = PStruct

    _exploreWatcher = New ExplorerWatcher(_pstruct, 250)
    AddHandler _exploreWatcher.ExplorerWatcherFound, AddressOf ExplorerFound
    AddHandler _exploreWatcher.ExplorerWatcherAborted, AddressOf ExplorerAbort
    _exploreWatcher.StartWatcher()

    statWatchLabel.Text = "Watching..."
  End Sub


  Private Sub trvFileSystem_NodeMouseDoubleClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles trvFileSystem.NodeMouseDoubleClick
    If trvFileSystem.SelectedNode IsNot Nothing Then
      If trvFileSystem.SelectedNode.Tag IsNot Nothing Then
        Me.CurrentPath = trvFileSystem.SelectedNode.Tag
      Else
        Me.CurrentPath = _curPath.Parent
      End If
    End If
  End Sub

  Public Sub FillRealTree(ByVal Path As Path)
    trvFileSystem.Nodes.Clear()

    If Not _pstruct.defaultPaths.Contains(Path.UNCPath) Then
      trvFileSystem.Nodes.Add(".. [" & Path.PathName & "]")
    End If
    If Not IsNothing(Path.Children) Then
      For i = 0 To Path.Children.Count - 1 Step 1
        Dim nd As TreeNode
        If Path.Children(i).Type = PathStructureClass.Path.PathType.Folder Then
          nd = New TreeNode(Path.Children(i).PathName, 0, 0)
        ElseIf Path.Children(i).Type = PathStructureClass.Path.PathType.File Then
          nd = New TreeNode(Path.Children(i).PathName, 1, 1)
        Else
          Continue For
        End If
        If Path.Children(i).IsNameStructured() Then
          nd.BackColor = Color.LightGreen
        Else
          nd.BackColor = Color.PaleVioletRed
        End If
        nd.Tag = Path.Children(i)
        trvFileSystem.Nodes.Add(nd)
      Next
    End If
  End Sub
  Public Sub FillPathTree(ByVal Path As Path)
    trvPathStructure.Nodes.Clear()
    Path.IsNameStructured()
    Dim structCand As StructureCandidate = Path.StructureCandidates.GetHighestMatch()
    Dim structEl As Xml.XmlElement

    If structCand IsNot Nothing Then
      structEl = structCand.XElement
    End If

    If Not _pstruct.defaultPaths.Contains(Path.UNCPath) And structEl IsNot Nothing Then
      trvPathStructure.Nodes.Add(".. [" & structEl.Attributes("name").Value & "]")
    End If
    If Path IsNot Nothing Then
      If Path.IsNameStructured() And structEl IsNot Nothing Then
        Dim nds As Xml.XmlNodeList = structEl.ChildNodes
        If nds IsNot Nothing Then
          If nds.Count > 0 Then
            For i = 0 To nds.Count - 1 Step 1
              Dim nd As TreeNode
              '' Check for the right icon
              If nds(i).Name = "Folder" Then
                nd = New TreeNode(nds(i).Attributes("name").Value, 0, 0)
              ElseIf nds(i).Name = "File" Then
                nd = New TreeNode(nds(i).Attributes("name").Value, 1, 1)
              Else
                Continue For
              End If
              '' Try to find an existing child in the current path that matches this node's XPath
              Dim strXPath As String = nds(i).FindXPath()
              If _curPath.Children IsNot Nothing Then
                For j = 0 To _curPath.Children.Length - 1 Step 1
                  If _curPath.Children(j).IsNameStructured() Then
                    If String.Equals(_curPath.Children(j).StructureCandidates.GetHighestMatch().XElement.FindXPath, strXPath, System.StringComparison.OrdinalIgnoreCase) Then
                      nd.BackColor = Color.LightGreen
                      nd.Tag = _curPath.Children(j)
                      Exit For
                    End If
                  End If
                Next
              End If
              '' Add to treeview
              trvPathStructure.Nodes.Add(nd)
            Next
          End If
        End If
      Else
        For i = 0 To Path.StructureCandidates.Count - 1 Step 1
          Debug.WriteLine(vbTab & Path.StructureCandidates(i).MatchPercentage.ToString & "% " & Path.StructureCandidates(i).StructurePath)
        Next
      End If
    End If
  End Sub

  Private Sub trvPathStructure_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles trvPathStructure.AfterSelect
    If trvPathStructure.SelectedNode IsNot Nothing Then
      If trvPathStructure.SelectedNode.Tag IsNot Nothing Then
        For i = 0 To trvFileSystem.Nodes.Count - 1 Step 1
          If trvFileSystem.Nodes(i).Tag IsNot Nothing Then
            If trvFileSystem.Nodes(i).Tag.Equals(trvPathStructure.SelectedNode.Tag) Then
              trvFileSystem.SelectedNode = trvFileSystem.Nodes(i)
              Exit For
            End If
          End If
        Next
      End If
    End If
  End Sub

  Private Sub trvFileSystem_MouseDown(sender As Object, e As MouseEventArgs) Handles trvFileSystem.MouseDown
    Dim trvPoint As Point = trvFileSystem.PointToClient(New Point(e.X, e.Y))
    If trvFileSystem.Bounds.Contains(e.X, e.Y) Then
      Dim nd As TreeNode = trvFileSystem.GetNodeAt(e.X, e.Y)
      If nd IsNot Nothing Then
        trvFileSystem.SelectedNode = nd
      End If
    ElseIf trvFileSystem.Bounds.Contains(trvPoint) Then
      Dim nd As TreeNode = trvFileSystem.GetNodeAt(trvPoint)
      If nd IsNot Nothing Then
        trvFileSystem.SelectedNode = nd
      End If

    End If
  End Sub
  Private Sub trvFileSystem_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles trvFileSystem.AfterSelect
    If trvFileSystem.SelectedNode IsNot Nothing Then
      If trvFileSystem.SelectedNode.Tag IsNot Nothing Then
        For i = 0 To trvPathStructure.Nodes.Count - 1 Step 1
          If trvPathStructure.Nodes(i).Tag IsNot Nothing Then
            If trvPathStructure.Nodes(i).Tag.Equals(trvFileSystem.SelectedNode.Tag) Then
              trvPathStructure.SelectedNode = trvPathStructure.Nodes(i)
              Debug.WriteLine("Selected '" & trvPathStructure.Nodes(i).Text & "'")
              Exit For
            End If
          End If
        Next
      Else
        Debug.WriteLine("Selected node's tag is nothing")
      End If
    Else
      Debug.WriteLine("No node selected")
    End If
  End Sub

  Delegate Sub ToolExplorerSearchCallback(ByVal sender As Object, ByVal e As ExplorerWatcher.ExplorerWatcherFoundEventArgs)
  Public Sub ExplorerFound(ByVal sender As Object, ByVal e As ExplorerWatcher.ExplorerWatcherFoundEventArgs)
    If Me IsNot Nothing Then
      If Me.InvokeRequired Then
        Dim d As New ToolExplorerSearchCallback(AddressOf ExplorerFound)
        Me.Invoke(d, New Object() {sender, e})
      Else
        If e.FoundPaths.Count > 0 Then
          Me.CurrentPath = e.FoundPaths(0)
          statWatchLabel.Text = "Watching..."
        End If
      End If
    End If
  End Sub
  Public Sub ExplorerAbort(ByVal sender As Object, ByVal e As System.UnhandledExceptionEventArgs)
    Log("An error occurred while watching Windows Explorer: " & e.ExceptionObject.Message)
    Try
      _exploreWatcher = New ExplorerWatcher(_pstruct, 250)
      _exploreWatcher.StartWatcher()
    Catch ex As Exception
      MessageBox.Show("An error occurred while attempting to fix a crash in the Explorer Watcher:" & vbLf & ex.Message,
                      "Failed to Restart Watcher", MessageBoxButtons.OK, MessageBoxIcon.Error)
      statWatchLabel.Text = "Aborted!"
    End Try
  End Sub

  Private Sub mnuWatchCopy_Click(sender As Object, e As EventArgs) Handles mnuWatchCopy.Click
    Try
      If trvFileSystem.SelectedNode IsNot Nothing Then
        If trvFileSystem.SelectedNode.Tag IsNot Nothing Then
          My.Computer.Clipboard.SetText(trvFileSystem.SelectedNode.Tag.UNCPath)
        Else
          My.Computer.Clipboard.SetText(_curPath.UNCPath)
        End If
      Else
        My.Computer.Clipboard.SetText(_curPath.UNCPath)
      End If
      statWatchLabel.Text = "Copied to Clipboard!"
    Catch ex As Exception
      Log("{Watcher}Copy: " & ex.Message)
      statWatchLabel.Text = "Failed copy!"
    End Try
  End Sub

  Private Sub mnuWatchCreateFolder_Click(sender As Object, e As EventArgs) Handles mnuWatchCreateFolder.Click
    If _curPath IsNot Nothing Then
      If _curPath.IsNameStructured() Then
        Dim nds As Xml.XmlNodeList = _curPath.StructureCandidates.GetHighestMatch().XElement.SelectNodes("Folder")
        If nds IsNot Nothing Then
          If nds.Count > 0 Then
            Dim lst As New List(Of String)
            For i = 0 To nds.Count - 1 Step 1
              Dim strTemp As String = _curPath.Variables.Replace(_pstruct.GetURIfromXPath(nds(i).FindXPath()))
              If (Not strTemp.Contains("{") And Not strTemp.Contains("}")) And Not IO.Directory.Exists(strTemp) Then
                lst.Add(strTemp)
              End If
            Next
            If lst.Count > 0 Then
              For i = 0 To lst.Count - 1 Step 1
                Log("{Watcher}Create Folder: Creating '" & lst(i) & "'")
                IO.Directory.CreateDirectory(lst(i))
              Next
              statWatchLabel.Text = "Created " & lst.Count.ToString & " folders"
            Else
              statWatchLabel.Text = "No valid paths"
            End If
          Else
            statWatchLabel.Text = "No folders"
          End If
        Else
          statWatchLabel.Text = "No Folders"
        End If
      Else
        statWatchLabel.Text = "Invalid path"
      End If
    Else
      statWatchLabel.Text = "Invalid path"
    End If
  End Sub

  Private _dragging As Boolean = False
  Private Sub trvPathStructure_DragDrop(sender As Object, e As DragEventArgs) Handles trvPathStructure.DragDrop
    '' Process drop data
    Dim trvPoint As Point = trvPathStructure.PointToClient(New Point(e.X, e.Y))
    If trvPathStructure.Bounds.Contains(e.X, e.Y) Then
      Dim nd As TreeNode = trvPathStructure.GetNodeAt(e.X, e.Y)
      If nd IsNot Nothing Then
        trvPathStructure.SelectedNode = nd
      End If
    ElseIf trvPathStructure.Bounds.Contains(trvPoint) Then
      Dim nd As TreeNode = trvPathStructure.GetNodeAt(trvPoint)
      If nd IsNot Nothing Then
        trvPathStructure.SelectedNode = nd
      End If

    End If

    If e.Data.GetDataPresent(DataFormats.FileDrop) Then
      If trvPathStructure.SelectedNode IsNot Nothing Then
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        For i = 0 To files.Length - 1 Step 1
          Dim frmt As New FormatDialog(files(i), _pstruct, trvPathStructure.SelectedNode.Text, trvPathStructure.SelectedNode.Tag)
          frmt.ShowDialog()
        Next
        statWatchLabel.Text = "FSO(s) added"
      End If
    ElseIf e.Data.GetDataPresent("System.Windows.Forms.TreeNode") Then
      If trvPathStructure.SelectedNode IsNot Nothing Then
        Dim nd As TreeNode = e.Data.GetData("System.Windows.Forms.TreeNode")
        Dim pt As Path = nd.Tag

        Dim frmt As New FormatDialog(nd.Tag.UNCPath, _pstruct, trvPathStructure.SelectedNode.Text, trvPathStructure.SelectedNode.Tag)
        frmt.ShowDialog()

        statWatchLabel.Text = "File formatted"
      End If
    End If

    _dragging = False
  End Sub
  Private Sub trvPathStructure_DragEnter(sender As Object, e As DragEventArgs) Handles trvPathStructure.DragEnter
    '' Adjust cursor
    _dragging = True
    If e.Data.GetDataPresent(DataFormats.FileDrop) Then
      e.Effect = DragDropEffects.Move
    ElseIf e.Data.GetDataPresent("System.Windows.Forms.TreeNode") Then
      e.Effect = DragDropEffects.Move
    End If
  End Sub
  Private Sub trvPathStructure_DragLeave(sender As Object, e As EventArgs) Handles trvPathStructure.DragLeave
    '' Disable flag to set "SelectedNode"
    _dragging = False
  End Sub
  Private Sub trvPathStructure_MouseMove(sender As Object, e As MouseEventArgs) Handles trvPathStructure.MouseMove
    '' Try to set "SelectedNode" visually
    Static snd As TreeNode
    If _dragging Then
      Dim trvPoint As Point = trvPathStructure.PointToClient(New Point(e.X, e.Y))
      If trvPathStructure.Bounds.Contains(e.Location) Then
        Dim nd As TreeNode = trvPathStructure.GetNodeAt(e.Location)
        If nd IsNot Nothing Then
          If snd IsNot Nothing Then
            If Not snd.Text = nd.Text Then
              trvPathStructure.SelectedNode = trvPathStructure.GetNodeAt(e.Location)
              snd = nd
            End If
          Else
            trvPathStructure.SelectedNode = trvPathStructure.GetNodeAt(e.Location)
            snd = nd
          End If
        End If
      End If
    End If
  End Sub
  Private Sub trvFileSystem_ItemDrag(sender As Object, e As ItemDragEventArgs) Handles trvFileSystem.ItemDrag
    '' Allow the passing of Real objects to the path structure for Formatting/Moving
    If trvFileSystem.SelectedNode IsNot Nothing Then
      If trvFileSystem.SelectedNode.Tag IsNot Nothing Then
        If trvFileSystem.SelectedNode.Tag.Type = Path.PathType.File Then
          statWatchLabel.Text = "Drop into Path Structure to change format"
          trvFileSystem.DoDragDrop(New DataObject("System.Windows.Forms.TreeNode", trvFileSystem.SelectedNode), DragDropEffects.Move)
        Else
          statWatchLabel.Text = "Only files can be changed!"
        End If
      Else
        Debug.WriteLine("Real Node tag doesn't exist")
      End If
    Else
      statWatchLabel.Text = "Select a file!"
      Debug.WriteLine("Real Node not selected")
    End If
  End Sub

End Class
