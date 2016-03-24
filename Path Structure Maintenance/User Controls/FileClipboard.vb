﻿Imports System.Xml

Public Class FileClipboard
  Private _CurrentPath As PathStructure
  Private myXML As New XmlDocument
  Private fileName As String

  Public Sub New(ByVal CurrentPath As String)
    '' This call is required by the designer.
    InitializeComponent()
    _CurrentPath = New PathStructure(CurrentPath)

    '' Add any initialization after the InitializeComponent() call.
    myXML.Load(My.Settings.SettingsPath)
    For Each var As KeyValuePair(Of String, String) In _CurrentPath.Variables
      Log(vbTab & var.Key & ": " & var.Value)
    Next
    Dim cand As New List(Of String)

    pnlVariables.Controls.Clear()
    cmbFiles.Items.Clear()
    If cand.Count > 0 Then
      For Each fil As String In cand.ToArray
        If fil.Contains("Option") Then fil = fil.Remove(fil.LastIndexOf("/"))
        cmbFiles.Items.Add(myXML.SelectSingleNode(fil).Attributes("name").Value)
      Next
    Else
      For Each fil As XmlElement In myXML.SelectNodes("//File")
        cmbFiles.Items.Add(fil.Attributes("name").Value)
      Next
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
    Dim nod As XmlElement
    If Not String.IsNullOrEmpty(nameOption) Then
      nod = myXML.SelectSingleNode("//File[@name='" & nameFile & "']/Option[@name='" & nameOption & "']")
      fileName = nod.InnerText
      If fileName.Contains("{name}") Then fileName = fileName.Replace("{name}", nameOption)
    Else
      nod = myXML.SelectSingleNode("//File[@name='" & nameFile & "']")
      fileName = nod.InnerText
      If fileName.Contains("{name}") Then fileName = fileName.Replace("{name}", nameFile)
    End If
    fileName = _CurrentPath.ReplaceVariables(fileName)
    If fileName.Contains("{Date}") Then fileName = fileName.Replace("{Date}", DateTime.Now.ToString("MM-dd-yyyy"))
    If fileName.Contains("{Time}") Then fileName = fileName.Replace("{Time}", DateTime.Now.ToString("hh-mm-ss tt"))


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
        AddHandler txt.TextChanged, AddressOf Variable_Changed

        pnl.Controls.Add(lbl)
        pnl.Controls.Add(txt)

        pnlVariables.Controls.Add(pnl)
        pnlVariables.Controls.SetChildIndex(pnl, 0)
      Next
    End If
    fileName = _CurrentPath.GetURIfromXPath(FindXPath(nod)) & fileName
    txtPreview.Text = fileName

  End Sub

  Private Sub Variable_Changed(ByVal sender As System.Object, ByVal e As System.EventArgs)
    Dim vals As New SortedList(Of String, String)
    txtPreview.Text = _CurrentPath.ReplaceVariables(fileName)
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
    Log("Copying '" & txtPreview.Text & "' to clipboard.")
    Clipboard.SetText(txtPreview.Text)
    MessageBox.Show("'" & txtPreview.Text & "' copied to clipboard!")
    _CurrentPath.LogData(txtPreview.Text, "Clipboard Copy")
    Application.Exit()
  End Sub
End Class
