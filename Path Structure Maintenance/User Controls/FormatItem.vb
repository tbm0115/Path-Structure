Imports System.Xml

Public Class Format_Item
  Private _CurrentPath As PathStructure
  Private myXML As New XmlDocument
  Private fileName As String
  Private _blnAutoClose As Boolean = True

  Public Event Accepted(ByVal sender As Object, ByVal e As FormatItemAcceptedEventArgs)

  Public Sub New(ByVal CurrentPath As String, Optional ByVal QuickSelect As Boolean = False, Optional ByVal AutoClose As Boolean = True)
    '' This call is required by the designer.
    InitializeComponent()
    _CurrentPath = New PathStructure(CurrentPath)
    _blnAutoClose = AutoClose

    If _CurrentPath.Type = PathStructure.PathType.File Then
      '' Add any initialization after the InitializeComponent() call.
      myXML.Load(My.Settings.SettingsPath)
      'If partNo.Contains("\") Then partNo = partNo.Remove(partNo.IndexOf("\"))
      'Log(vbTab & "Directory: " & _CurrentPath.FileInfo.DirectoryName)
      'Log(vbTab & "Filename: " & _CurrentPath.FileInfo.Name)
      'Log(vbTab & "Extension: " & _CurrentPath.FileInfo.Extension)
      Log(vbTab & "Directory: " & _CurrentPath.CurrentDirectory)
      Log(vbTab & "Filename: " & _CurrentPath.FSOInfo.Name)
      Log(vbTab & "Extension: " & _CurrentPath.FSOInfo.Extension)
      'For Each var As KeyValuePair(Of String, String) In _CurrentPath.Variables
      '  Log(vbTab & var.Key & ": " & var.Value)
      'Next
      For Each var As PathStructure.Variable In _CurrentPath.Variables.Items
        Log(vbTab & var.Name & ": " & var.Value)
      Next
      If QuickSelect Then
        Log(vbTab & "IsNameStructured: " & _CurrentPath.IsNameStructured().ToString)
      End If

      pnlVariables.Controls.Clear()
      cmbFiles.Items.Clear()
      If _CurrentPath.StructureCandidates.Count > 0 Then
        For Each fil As String In _CurrentPath.StructureCandidates.ToArray
          If fil.Contains("Option") Then fil = fil.Remove(fil.LastIndexOf("/"))
          cmbFiles.Items.Add(myXML.SelectSingleNode(fil).Attributes("name").Value)
        Next
      Else
        For Each fil As XmlElement In myXML.SelectNodes("//File")
          cmbFiles.Items.Add(fil.Attributes("name").Value)
        Next
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
    For Each opt As XmlElement In myXML.SelectNodes("//File[@name='" & FileType & "']/Option")
      cmbOptions.Items.Add(opt.Attributes("name").Value)
    Next
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
      fileName = myXML.SelectSingleNode("//File[@name='" & nameFile & "']/Option[@name='" & nameOption & "']").InnerText
      If fileName.Contains("{name}") Then fileName = fileName.Replace("{name}", nameOption)
    Else
      fileName = myXML.SelectSingleNode("//File[@name='" & nameFile & "']").InnerText
      If fileName.Contains("{name}") Then fileName = fileName.Replace("{name}", nameFile)
    End If
    fileName = _CurrentPath.Variables.Replace(fileName) ' _CurrentPath.ReplaceVariables(fileName)
    If fileName.Contains("{Date}") Then fileName = fileName.Replace("{Date}", DateTime.Now.ToString("MM-dd-yyyy"))
    If fileName.Contains("{Time}") Then fileName = fileName.Replace("{Time}", DateTime.Now.ToString("hh-mm-ss tt"))

    fileName += _CurrentPath.FSOInfo.Extension ' _CurrentPath.FileInfo.Extension

    Dim input As List(Of String) = GetListOfInternalStrings(fileName, "{", "}")
    If input.Count > 0 Then
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

    txtPreview.Text = _CurrentPath.Variables.Replace(fileName) ' _CurrentPath.ReplaceVariables(fileName)
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
  End Sub

  Private Sub btnAccept_Click(sender As Object, e As EventArgs) Handles btnAccept.Click
    If pnlOptions.Enabled And cmbOptions.SelectedIndex < 0 Then
      MessageBox.Show("You must select a file type option!", "Invalid Option", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      Exit Sub
    End If
    If IO.File.Exists(_CurrentPath.CurrentDirectory & "\" & txtPreview.Text) Then ' _CurrentPath.FileInfo.DirectoryName & "\" & txtPreview.Text) Then
      Dim archive As String = _CurrentPath.FindNearestArchive()
      If Not String.IsNullOrEmpty(archive) Then
        'IO.File.Move(_CurrentPath.FileInfo.DirectoryName & "\" & txtPreview.Text, archive & "\" & Now.ToString("yyyy-MM-dd hh-mm-ss tt") & "_" & txtPreview.Text)
        'Log("Changing the filename from '" & _CurrentPath.UNCPath & "' to '" & _CurrentPath.FileInfo.DirectoryName & "\" & txtPreview.Text & "'")
        'IO.File.Move(_CurrentPath.UNCPath, _CurrentPath.FileInfo.DirectoryName & "\" & txtPreview.Text) ' & filInfo.Extension)
        '_CurrentPath.LogData(_CurrentPath.FileInfo.DirectoryName & "\" & txtPreview.Text, "Format Filename")
        IO.File.Move(_CurrentPath.CurrentDirectory & "\" & txtPreview.Text, archive & "\" & Now.ToString("yyyy-MM-dd hh-mm-ss tt") & "_" & txtPreview.Text)
        Log("Changing the filename from '" & _CurrentPath.UNCPath & "' to '" & _CurrentPath.CurrentDirectory & "\" & txtPreview.Text & "'")
        IO.File.Move(_CurrentPath.UNCPath, _CurrentPath.CurrentDirectory & "\" & txtPreview.Text) ' & filInfo.Extension)
        _CurrentPath.LogData(_CurrentPath.CurrentDirectory & "\" & txtPreview.Text, "Format Filename")
      Else
        MessageBox.Show("The new name seems to exist already and an archive folder could not be found to place the old file.", "Aborting", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    Else
      'Log("Changing the filename from '" & _CurrentPath.UNCPath & "' to '" & _CurrentPath.FileInfo.DirectoryName & "\" & txtPreview.Text & "'")
      'IO.File.Move(_CurrentPath.UNCPath, _CurrentPath.FileInfo.DirectoryName & "\" & txtPreview.Text) ' & filInfo.Extension)
      '_CurrentPath.LogData(_CurrentPath.FileInfo.DirectoryName & "\" & txtPreview.Text, "Format Filename")
      Log("Changing the filename from '" & _CurrentPath.UNCPath & "' to '" & _CurrentPath.CurrentDirectory & "\" & txtPreview.Text & "'")
      IO.File.Move(_CurrentPath.UNCPath, _CurrentPath.CurrentDirectory & "\" & txtPreview.Text) ' & filInfo.Extension)
      _CurrentPath.LogData(_CurrentPath.CurrentDirectory & "\" & txtPreview.Text, "Format Filename")
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
