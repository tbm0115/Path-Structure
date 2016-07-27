Imports PathStructureClass
Imports System.Xml

Public Class ExplorerPreview
  Private exp As ExplorerWatcher
  Private pstruct As PathStructure

  Public Sub New(ByVal Watcher As ExplorerWatcher, ByVal PathStruct As PathStructure)
    InitializeComponent()

    pstruct = PathStruct

    exp = Watcher
    AddHandler exp.ExplorerWatcherFound, AddressOf FoundPath
    exp.StartWatcher()
  End Sub

  Delegate Sub ExplorerSearchCallback(ByVal URL As String)
  Private Sub FoundPath(ByVal URL As String)
    Try
      If Me IsNot Nothing Then
        If Me.InvokeRequired Then
          Dim d As New ExplorerSearchCallback(AddressOf FoundPath)
          Me.Invoke(d, New Object() {URL})
          Exit Sub
          'Else
          '  Me.CurrentPath = New Path(_pstruct, URL)
          '  statWatchLabel.Text = "Watching..."
          '  GC.Collect()
        End If
      End If
    Catch ex As Exception
      Log("{ExplorerFound} Failed: " & ex.Message)
    End Try

    For i = 0 To Me.Controls.Count - 1 Step 1
      Me.Controls(i).Dispose()
    Next
    Me.Controls.Clear()

    Application.DoEvents()
  End Sub
  Private Function RecursivePreview(ByVal focus As Path) As Boolean
    Debug.WriteLine(focus.UNCPath)
    Dim added As Boolean = True
    If focus.IsNameStructured Then
      Dim lstCandidateXPaths As New List(Of String)
      For Each cand As StructureCandidate In focus.StructureCandidates.Items
        '' Verify the xmlelement has the 'preview' attribute
        If cand.XElement.HasAttribute("preview") Then
          '' Iterate through each 'preview' xpath result
          lstCandidateXPaths.Add(cand.XElement.SelectSingleNode(cand.XElement.Attributes("preview").Value).FindXPath)
        End If
      Next

      If lstCandidateXPaths.Count > 0 Then
        For i = 0 To focus.Children.Length - 1 Step 1
          Dim pt As Path = focus.Children(i)
          '' Begin loading controls
          If pt.IsNameStructured Then
            If pt.Type = Path.PathType.File And lstCandidateXPaths.Contains(pt.StructureCandidates.GetHighestMatch().XPath) Then
              If Not IsNothing(pt.Extension) Then
                Select Case pt.Extension.ToLower
                  Case ".pdf"
                    Dim pdf As New WebBrowser
                    pdf.Dock = DockStyle.Fill
                    Me.Controls.Add(pdf)
                    pdf.Navigate("file:///" & pt.UNCPath & "#page=1&view=Fit")
                  Case ".txt"
                    Dim txt As New RichTextBox
                    txt.Dock = DockStyle.Fill
                    Me.Controls.Add(txt)
                    txt.LoadFile(pt.UNCPath)
                  Case ".gcode"
                    Dim txt As New RichTextBox
                    txt.Dock = DockStyle.Fill
                    Me.Controls.Add(txt)
                    txt.LoadFile(pt.UNCPath)
                  Case ".eia"
                    Dim txt As New RichTextBox
                    txt.Dock = DockStyle.Fill
                    Me.Controls.Add(txt)
                    txt.LoadFile(pt.UNCPath)
                  Case ".rtf"
                    Dim txt As New RichTextBox
                    txt.Dock = DockStyle.Fill
                    Me.Controls.Add(txt)
                    txt.LoadFile(pt.UNCPath)
                  Case ".html"
                    Dim html As New WebBrowser
                    html.Dock = DockStyle.Fill
                    Me.Controls.Add(html)
                    html.Navigate(pt.UNCPath)
                  Case ".htm"
                    Dim html As New WebBrowser
                    html.Dock = DockStyle.Fill
                    Me.Controls.Add(html)
                    html.Navigate(pt.UNCPath)
                End Select
                If Me.Controls.Count > 0 Then
                  added = True
                End If
              End If
            ElseIf pt.Type = Path.PathType.Folder And lstCandidateXPaths.Contains(pt.StructureCandidates.GetHighestMatch().XPath) Then
              If RecursivePreview(pt) Then Exit For
              Debug.WriteLine(vbTab & vbTab & pt.UNCPath & " is either not a file or " & pt.StructureCandidates.GetHighestMatch().XPath & " doesn't meet any candidates")
            End If
          Else
            Debug.WriteLine(vbTab & vbTab & pt.UNCPath & " is not name structured")
          End If
        Next
      Else
        Debug.WriteLine(vbTab & "No candidates with preview")
      End If
    Else
      Debug.WriteLine(vbTab & "Not name structured")
    End If

    If Me.Controls.Count = 0 Then
      Dim lbl As New Label
      lbl.Text = "No Preview Document found in '" & focus.UNCPath & "'"
      lbl.Dock = DockStyle.Fill
      lbl.Font = New System.Drawing.Font("Arial", 24)
      Me.Controls.Add(lbl)
    Else
      Return True
    End If
    Return False
  End Function

  Private Sub PreviewWindow_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
    RemoveHandler exp.ExplorerWatcherFound, AddressOf FoundPath
  End Sub
End Class