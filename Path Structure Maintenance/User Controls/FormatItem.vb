Imports System.Xml
Imports PathStructureClass

Public Class Format_Item
  Private _CurrentPath As Path
  'Private myXML As New XmlDocument
  Private fileName As String
  Private _blnAutoClose As Boolean = True
  Private _struct As XmlElement

  Public Event Accepted(ByVal sender As Object, ByVal e As FormatItemAcceptedEventArgs)

  Public Sub New(ByVal CurrentPath As String, Optional ByVal QuickSelect As Boolean = False, Optional ByVal AutoClose As Boolean = True)
    '' This call is required by the designer.
    InitializeComponent()
    _CurrentPath = New Path(Main.PathStruct, CurrentPath)
    _blnAutoClose = AutoClose

    If _CurrentPath.Type = Path.PathType.File Then
      '' Add any initialization after the InitializeComponent() call.
      'myXML.Load(My.Settings.SettingsPath)
      Log(vbTab & "Directory: " & _CurrentPath.CurrentDirectory)
      Log(vbTab & "Filename: " & _CurrentPath.FileInfo.Name)
      Log(vbTab & "Extension: " & _CurrentPath.FileInfo.Extension)
      For Each var As Path.Variable In _CurrentPath.Variables.Items
        Log(vbTab & var.Name & ": " & var.Value)
      Next
      If QuickSelect Then
        Log(vbTab & "IsNameStructured: " & _CurrentPath.IsNameStructured().ToString)
      End If

      pnlVariables.Controls.Clear()
      cmbFiles.Items.Clear()
      Dim slist As New SortedList(Of String, String)
      If _CurrentPath.StructureCandidates.Count > 0 Then
        Dim searchNode As XmlElement
        For Each struct As Path.StructureCandidate In _CurrentPath.StructureCandidates.Items
          searchNode = Nothing
          If struct.XElement.Name = "File" Then
            If struct.XElement.ParentNode IsNot Nothing Then
              searchNode = struct.XElement.ParentNode
            End If
          ElseIf struct.XElement.Name = "Option" Then
            If struct.XElement.ParentNode.ParentNode IsNot Nothing Then
              searchNode = struct.XElement.ParentNode.ParentNode
            End If
          ElseIf struct.XElement.Name = "Folder" Then
            If struct.XElement.HasChildNodes Then
              searchNode = struct.XElement
            End If
          End If
          If Not IsNothing(searchNode) Then
            For Each nod As XmlElement In searchNode.SelectNodes("File")
              If nod.HasAttribute("name") Then
                If Not slist.ContainsKey(nod.Attributes("name").Value) Then
                  slist.Add(nod.Attributes("name").Value, nod.Attributes("name").Value)
                End If
                'If Not cmbFiles.Items.Contains(nod.Attributes("name").Value) Then
                '  cmbFiles.Items.Add(nod.Attributes("name").Value)
                'End If
              End If
            Next
          End If
        Next
      Else
        For Each fil As XmlElement In Main.PathStruct.Settings.SelectNodes("//File")
          slist.Add(fil.Attributes("name").Value, fil.Attributes("name").Value)
          'cmbFiles.Items.Add(fil.Attributes("name").Value)
        Next
      End If
      If slist.Count > 0 Then
        cmbFiles.Items.AddRange(slist.Values.ToArray())
      End If
    Else
      MessageBox.Show("You must select a file to complete the 'Format' function!", "Invalid FileSystemObject type", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      If _blnAutoClose Then Application.Exit()
    End If
  End Sub

  Private Sub cmbFiles_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbFiles.SelectedIndexChanged
    Dim FileType As String = cmbFiles.Items(cmbFiles.SelectedIndex).ToString
    cmbOptions.Items.Clear()
    pnlVariables.Controls.Clear()
    Dim slist As New SortedList(Of String, String)
    For Each opt As XmlElement In _CurrentPath.GetPathStructure.Settings.SelectNodes("//File[@name='" & FileType & "']/Option")
      slist.Add(opt.Attributes("name").Value, opt.Attributes("name").Value)
      'cmbOptions.Items.Add(opt.Attributes("name").Value)
    Next
    cmbOptions.Items.AddRange(slist.Values.ToArray())
    If cmbOptions.Items.Count > 0 Then
      pnlOptions.Enabled = True
    Else
      pnlOptions.Enabled = False
      LoadFileSyntax(FileType)
    End If

  End Sub

  Private Sub cmbOptions_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbOptions.SelectedIndexChanged
    pnlVariables.Controls.Clear()
    LoadFileSyntax(cmbFiles.Items(cmbFiles.SelectedIndex).ToString, cmbOptions.Items(cmbOptions.SelectedIndex).ToString)
  End Sub

  Private Sub LoadFileSyntax(ByVal nameFile As String, Optional ByVal nameOption As String = "")
    If Not String.IsNullOrEmpty(nameOption) Then
      _struct = _CurrentPath.GetPathStructure.Settings.SelectSingleNode("//File[@name='" & nameFile & "']/Option[@name='" & nameOption & "']")
      fileName = _struct.InnerText
      If fileName.Contains("{name}") Then fileName = fileName.Replace("{name}", nameOption)
    Else
      _struct = _CurrentPath.GetPathStructure.Settings.SelectSingleNode("//File[@name='" & nameFile & "']")
      fileName = _struct.InnerText
      If fileName.Contains("{name}") Then fileName = fileName.Replace("{name}", nameFile)
    End If
    fileName = _CurrentPath.Variables.Replace(fileName) ' _CurrentPath.ReplaceVariables(fileName)
    If fileName.Contains("{Date}") Then fileName = fileName.Replace("{Date}", DateTime.Now.ToString("MM-dd-yyyy"))
    If fileName.Contains("{Time}") Then fileName = fileName.Replace("{Time}", DateTime.Now.ToString("hh-mm-ss tt"))

    fileName += _CurrentPath.Extension ' _CurrentPath.FileInfo.Extension

    Dim input As String() = GetListOfInternalStrings(fileName, "{", "}")
    If input.Length > 0 Then
      For Each str As String In input
        Dim pnl As New Panel
        Dim lbl As New Label
        Dim txt As New TextBox

        pnl.Dock = DockStyle.Top
        pnl.Height = 50

        lbl.Dock = DockStyle.Left
        lbl.AutoSize = False
        lbl.TextAlign = ContentAlignment.MiddleRight
        lbl.Text = str
        lbl.Size = New Size(pnlVariables.Width * 0.3, 30)

        txt.Dock = DockStyle.Right
        txt.Size = New Size(pnlVariables.Width * 0.6, 30)
        txt.Tag = str
        'If _CurrentPath.Variables.ContainsKey("{" & str & "}") Then
        '  txt.Text = _CurrentPath.Variables("{" & str & "}")
        'End If
        If _CurrentPath.Variables.ContainsName("{" & str & "}") Then ' _CurrentPath.Variables.ContainsKey("{" & str & "}") Then
          txt.Text = _CurrentPath.Variables("{" & str & "}").Value ' _CurrentPath.Variables("{" & str & "}")
        End If
        AddHandler txt.TextChanged, AddressOf Variable_Changed

        pnl.Controls.Add(lbl)
        pnl.Controls.Add(txt)

        pnlVariables.Controls.Add(pnl)
        pnlVariables.Controls.SetChildIndex(pnl, 0)
      Next
    End If

    Variable_Changed(Nothing, Nothing)
  End Sub

  Private Sub Variable_Changed(ByVal sender As System.Object, ByVal e As System.EventArgs)
    Dim vals As New SortedList(Of String, String)
    txtPreview.Text = _CurrentPath.Variables.Replace(fileName) ' _CurrentPath.ReplaceVariables(fileName)
    For Each pnl As Control In pnlVariables.Controls
      vals.Add(pnl.Controls(1).Tag, pnl.Controls(1).Text)
      If Not String.IsNullOrEmpty(pnl.Controls(1).Text) Then
        txtPreview.Text = txtPreview.Text.Replace("{" & pnl.Controls(1).Tag & "}", pnl.Controls(1).Text)
      End If
    Next
    If txtPreview.Text.StartsWith("{}") Then txtPreview.Text = _CurrentPath.PathName
  End Sub

  Private Sub btnAccept_Click(sender As Object, e As EventArgs) Handles btnAccept.Click
    If pnlOptions.Enabled And cmbOptions.SelectedIndex < 0 Then
      MessageBox.Show("You must select a file type option!", "Invalid Option", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      Exit Sub
    End If
    Dim strDir As String = _CurrentPath.CurrentDirectory
    If _struct IsNot Nothing Then
      If Not _struct.Name = "Folder" Then
        Do Until _struct.Name = "Folder" Or _struct.ParentNode.Name = "Structure"
          _struct = _struct.ParentNode
        Loop
        If _struct.Name = "Folder" Then
          strDir = _CurrentPath.Variables.Replace(Main.PathStruct.GetURIfromXPath(FindXPath(_struct)))
          If Not IO.Directory.Exists(strDir) Then
            IO.Directory.CreateDirectory(strDir)
          End If
        End If
      End If
    End If
    If IO.File.Exists(strDir & "\" & txtPreview.Text) Then
      Dim archive As String = _CurrentPath.FindNearestArchive()
      If Not String.IsNullOrEmpty(archive) Then
        IO.File.Move(strDir & "\" & txtPreview.Text, archive & "\" & Now.ToString("yyyy-MM-dd hh-mm-ss tt") & "_" & txtPreview.Text)
        Log("Changing the filename from '" & _CurrentPath.UNCPath & "' to '" & strDir & "\" & txtPreview.Text & "'")
        IO.File.Move(_CurrentPath.UNCPath, strDir & "\" & txtPreview.Text) ' & filInfo.Extension)
        _CurrentPath.LogData(strDir & "\" & txtPreview.Text, "Format Filename")
      Else
        MessageBox.Show("The new name seems to exist already and an archive folder could not be found to place the old file.", "Aborting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    Else
      Log("Changing the filename from '" & _CurrentPath.UNCPath & "' to '" & strDir & "\" & txtPreview.Text & "'")
      IO.File.Move(_CurrentPath.UNCPath, strDir & "\" & txtPreview.Text) ' & filInfo.Extension)
      _CurrentPath.LogData(strDir & "\" & txtPreview.Text, "Format Filename")
    End If
    If _blnAutoClose Then Application.Exit()
    RaiseEvent Accepted(Me, New FormatItemAcceptedEventArgs(_CurrentPath.UNCPath))
  End Sub
End Class
Public Class FormatItemAcceptedEventArgs
  Inherits EventArgs

  Public Property Path As String

  Public Sub New(ByVal path As String)
    MyBase.New()

    Me.Path = path
  End Sub
End Class
