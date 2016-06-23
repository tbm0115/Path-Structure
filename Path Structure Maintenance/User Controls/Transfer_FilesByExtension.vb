Imports System.Xml

Public Class Transfer_FilesByExtension
  Private _CurrentPath As PathStructure
  'Private myXML As New XmlDocument
  Private fileName As String

  Public Sub New(ByVal CurrentPath As String)
    InitializeComponent()
    _CurrentPath = New PathStructure(CurrentPath)

    'myXML.Load(My.Settings.SettingsPath)

    cmbFolder.Items.Clear()
    For Each fol As XmlElement In _CurrentPath.PathStructure.SelectNodes(".//Folder")
      cmbFolder.Items.Add(fol.Attributes("name").Value)
    Next

    txtFileExtension.AutoCompleteCustomSource.Clear()
    If _CurrentPath.Type = PathStructure.PathType.File Then
      txtFileExtension.AutoCompleteCustomSource.Add(_CurrentPath.FSOInfo.Extension) ' _CurrentPath.FileInfo.Extension)
      txtFileExtension.Text = _CurrentPath.FSOInfo.Extension ' _CurrentPath.FileInfo.Extension
      _CurrentPath = _CurrentPath.Parent
    End If
    If _CurrentPath.Type = PathStructure.PathType.Folder Then
      For Each fil As PathStructure In _CurrentPath.Children
        If fil.Type = PathStructure.PathType.File Then
          If Not txtFileExtension.AutoCompleteCustomSource.Contains(fil.FSOInfo.Extension) Then ' fil.FileInfo.Extension) Then
            txtFileExtension.AutoCompleteCustomSource.Add(fil.FSOInfo.Extension) ' fil.FileInfo.Extension)
          End If
        End If
      Next
    End If
  End Sub

  Private Sub btnTransfer_Click(sender As Object, e As EventArgs) Handles btnTransfer.Click
    If cmbFolder.SelectedIndex > 0 And Not String.IsNullOrEmpty(txtFileExtension.Text) Then
      Dim fold As XmlNodeList = _CurrentPath.PathStructure.SelectNodes(".//Folder[@name='" & cmbFolder.Items(cmbFolder.SelectedIndex).ToString & "']")
      If fold.Count = 1 Then
        Dim cntFiles As Integer = 0
        Dim xpath As String = ""
        Dim fpath As String = ""
        For Each fol As XmlNode In fold
          xpath = FindXPath(fol)
          fpath = _CurrentPath.Variables.Replace(GetURIfromXPath(xpath))

          If Not IO.Directory.Exists(fpath) Then
            IO.Directory.CreateDirectory(fpath)
          End If
          For Each child As PathStructure In _CurrentPath.Children
            If child.Type = PathStructure.PathType.File Then
              If child.FSOInfo.Extension.Contains(txtFileExtension.Text) Then ' child.FileInfo.Extension.Contains(txtFileExtension.Text) Then
                If fpath.IndexOf("Archive", System.StringComparison.OrdinalIgnoreCase) >= 0 Then
                  'child.FileInfo.MoveTo(fpath & "\" & Now.ToString("yyyy-MM-dd hh-mm-ss tt") & "_" & child.FileInfo.Name)
                  'child.LogData(fpath & "\" & Now.ToString("yyyy-MM-dd hh-mm-ss tt") & "_" & child.FileInfo.Name, "TransferByExtension")
                  Dim f As New IO.FileInfo(child.UNCPath)
                  f.MoveTo(fpath & "\" & Now.ToString("yyyy-MM-dd hh-mm-ss tt") & "_" & child.FSOInfo.Name)
                  child.LogData(fpath & "\" & Now.ToString("yyyy-MM-dd hh-mm-ss tt") & "_" & child.FSOInfo.Name, "TransferByExtension")
                Else
                  'child.FileInfo.MoveTo(fpath & "\" & child.FileInfo.Name)
                  'child.LogData(fpath & "\" & child.FileInfo.Name, "TransferByExtension")
                  Dim f As New IO.FileInfo(child.UNCPath)
                  f.MoveTo(fpath & "\" & child.FSOInfo.Name)
                  child.LogData(fpath & "\" & child.FSOInfo.Name, "TransferByExtension")
                End If
                cntFiles += 1
              End If
            End If
          Next
        Next
        MessageBox.Show("Moved '" & cntFiles.ToString & "' files to '" & fpath & "'", "Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Application.Exit()
      ElseIf fold.Count = 0 Then
        MessageBox.Show("You must select a valid folder name", "Invalid Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
        Exit Sub
      Else
        Log("XML Structure cannot have duplicate object names! Found '" & fold.Count.ToString & "' with the name '" & cmbFolder.Items(cmbFolder.SelectedIndex).ToString & "'.")
        MessageBox.Show("An error occurred in the XML Structure!" & vbLf & "Duplicate objects found. See log for more details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Exit Sub
      End If
    Else
      MessageBox.Show("You must provide a valid folder name and valid file extension!", "Invalid Folder/Extension", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
    End If
  End Sub

End Class
