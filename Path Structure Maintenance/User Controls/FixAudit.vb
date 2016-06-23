Public Class FixAudit
  Private _errPaths As List(Of String)
  Public Sub New(ByVal AuditResult As Object) ' PathStructure.AuditReport)
    ' This call is required by the designer.
    InitializeComponent()

    ' Add any initialization after the InitializeComponent() call.
    _errPaths = AuditResult.ErrorPaths
    If Not IsNothing(_errPaths) Then
      lstFSO.Items.Clear()
      For Each path As String In _errPaths
        Dim tmp As New PathStructure(path)
        lstFSO.Items.Add(tmp)
      Next
      ''lstFSO.Items.AddRange(_errPaths.ToArray)
    End If
  End Sub

  Private Sub lstFSO_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstFSO.SelectedIndexChanged
    If lstFSO.SelectedIndex = -1 Then Exit Sub

    pnlFormat.Controls.Clear()
    Dim fsoFormat As New Format_Item(lstFSO.SelectedItem.UNCPath, , False)
    AddHandler fsoFormat.Accepted, AddressOf FormatAccepted
    pnlFormat.Controls.Add(fsoFormat)
  End Sub

  Private Sub FormatAccepted(ByVal sender As Object, ByVal e As FormatItemAcceptedEventArgs)
    lstFSO.Items.RemoveAt(lstFSO.SelectedIndex)
    pnlFormat.Controls.Clear()
  End Sub
End Class
