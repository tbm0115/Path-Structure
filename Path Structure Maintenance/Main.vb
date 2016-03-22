Imports System.Xml
Imports System.Security.Principal

Public Class Main

  Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    My.Application.SaveMySettingsOnExit = True

    Dim myXML As New XmlDocument
    myXML.Load(My.Settings.SettingsPath)
    defaultPath = myXML.SelectSingleNode("Structure").Attributes("defaultPath").Value
    Dim custCode As String = ""
    Dim partNo As String = ""

    If Environment.GetCommandLineArgs.Length > 0 Then
      Dim args As String() = Environment.GetCommandLineArgs
      Dim dg As DialogResult
      Dim strTemp As String = ""
      For Each arg As String In args
        Log("Command Line Argument: '" & arg & "'")
      Next
      If args.Length >= 2 Then
        Select Case args(1)
          Case "-add"
            If args.Length >= 3 Then
              If GetUNCPath(args(2)).ToLower.StartsWith(defaultPath.ToLower) Then
                statCurrentPath.Text = args(2)
                Log("Add Command Received")
                Dim fi As New Add_Folder(args(2))
                fi.Dock = DockStyle.Fill

                pnlContainer.Controls.Add(fi)
                'AddFolder(args(2), args(?))
              Else
                Log("Path does not match default path!" & vbCrLf & vbTab & "'" & defaultPath & "' != '" & args(2) & "'")
                MessageBox.Show("You must be within the default path of '" & defaultPath & "'!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
              End If
            End If
          Case "-addall"
            If args.Length >= 3 Then
              If GetUNCPath(args(2)).ToLower.StartsWith(defaultPath.ToLower) Then
                statCurrentPath.Text = args(2)
                dg = MessageBox.Show("Are you sure you wish to create all main folders in the selected directory?", "Verify", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If dg = Windows.Forms.DialogResult.Yes Then
                  Log("Add All Command Received")
                  Dim c As New PathStructure(args(2))
                  For Each fold As XmlElement In myXML.SelectNodes("//Folder")
                    If Not fold.Attributes("name").Value.Contains("{") And Not fold.Attributes("name").Value.Contains("}") Then
                      If Not IO.Directory.Exists(c.GetURIfromXPath(FindXPath(fold))) Then
                        IO.Directory.CreateDirectory(c.GetURIfromXPath(FindXPath(fold)))
                      End If
                      'AddFolder(args(2), fold.Attributes("name").Value)
                    End If
                  Next
                End If
              Else
                Log("Path does not match default path!" & vbCrLf & vbTab & "'" & defaultPath & "' != '" & args(2) & "'")
                MessageBox.Show("You must be within the default path of '" & defaultPath & "'!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
              End If
              Application.Exit()
            End If
          Case "-format"
            If args.Length >= 3 Then
              If GetUNCPath(args(2)).ToLower.StartsWith(defaultPath.ToLower) Then
                statCurrentPath.Text = args(2)
                Log("Format Command Received")
                Dim fi As New Format_Item(args(2))
                fi.Dock = DockStyle.Fill

                pnlContainer.Controls.Add(fi)
                'FormatFile(args(2), args(?))
              Else
                Log("Path does not match default path!" & vbCrLf & vbTab & "'" & defaultPath & "' != '" & args(2) & "'")
                MessageBox.Show("You must be within the default path of '" & defaultPath & "'!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
              End If
            End If
          Case "-audit"
            If args.Length >= 3 Then
              If GetUNCPath(args(2)).ToLower.StartsWith(defaultPath.ToLower) Then
                statCurrentPath.Text = args(2)
                Log("Audit Command Received")

                Dim pt As New PathStructure(args(2))
                Dim bads As New List(Of String)

                '' If audit report exists, then delete it
                If IO.File.Exists(pt.StartPath & "\Audit Report.html") Then IO.File.Delete(pt.StartPath & "\Audit Report.html")
                Dim auditRpt As New PathStructure.AuditReport(pt)
                If Not pt.Audit(auditRpt, bads) Then
                  MessageBox.Show("Invalid path(s) found. Click OK to open temporary report...", "Audit Failed!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                  Try
                    auditRpt.Report("Bad Paths: " & String.Join("<br />", bads.ToArray), PathStructure.AuditReport.StatusCode.ErrorStatus)
                    IO.File.WriteAllText(pt.StartPath & "\Audit Report.html", auditRpt.ReportMarkup)
                    Process.Start(pt.StartPath & "\Audit Report.html")
                  Catch ex As Exception
                    MessageBox.Show("An error occurred while attempting to create the audit report: " & vbLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                  End Try
                Else
                  MessageBox.Show("Audit Successful!" & vbLf & "No errors found in the Path Structure", "Audit Success!", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If

                pt.LogData(pt.StartPath & "\Audit Report.html", "Generic Audit")
                Application.Exit()
              Else
                Log("Path does not match default path!" & vbCrLf & vbTab & "'" & defaultPath & "' != '" & args(2) & "'")
              End If
            End If
          Case "-clipboard"
            If args.Length >= 3 Then
              If GetUNCPath(args(2)).ToLower.StartsWith(defaultPath.ToLower) Then
                statCurrentPath.Text = args(2)
                Log("Clipboard Command Received")
                Dim fi As New FileClipboard(args(2))
                fi.Dock = DockStyle.Fill

                pnlContainer.Controls.Add(fi)
                'FormatFile(args(2), args(?))
              Else
                Log("Path does not match default path!" & vbCrLf & vbTab & "'" & defaultPath & "' != '" & args(2) & "'")
                MessageBox.Show("You must be within the default path of '" & defaultPath & "'!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
              End If
            End If
          Case "-transfer"
            If args.Length >= 3 Then
              If GetUNCPath(args(2)).StartsWith(defaultPath) Then
                statCurrentPath.Text = args(2)
                Log("Transfer_Files_By_Extension Command Received")
                Dim fi As New Transfer_FilesByExtension(args(2))
                fi.Dock = DockStyle.Fill

                pnlContainer.Controls.Add(fi)
                'FormatFile(args(2), args(?))
              Else
                Log("Path does not match default path!" & vbCrLf & vbTab & "'" & defaultPath & "' != '" & args(2) & "'")
                MessageBox.Show("You must be within the default path of '" & defaultPath & "'!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Application.Exit()
              End If
            End If
        End Select
      End If
    End If
  End Sub

  Private Sub mnuSettings_Click(sender As Object, e As EventArgs) Handles mnuSettings.Click
    Dim identity = WindowsIdentity.GetCurrent
    Dim principal = New WindowsPrincipal(identity)
    If principal.IsInRole(WindowsBuiltInRole.Administrator) Then
      If Not Settings.Visible Then
        Settings.Show()
      Else
        Settings.Focus()
      End If
    Else
      MessageBox.Show("You must be an administrator to access the settings")
    End If
  End Sub

  Private Sub mnuToolsAddAll_Click(sender As Object, e As EventArgs) Handles mnuToolsAddAll.Click
    pnlContainer.Controls.Clear()
    Dim opn As New SelectFolderDialog
    opn.Title = "Select a 'parts' folder to format:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.Directory.Exists(opn.CurrentDirectory) And opn.DialogResult = Windows.Forms.DialogResult.OK Then
      If GetUNCPath(opn.CurrentDirectory).StartsWith(defaultPath) Then
        Debug.WriteLine("Adding folders for '" & opn.CurrentDirectory & "'")
        Dim myXML As New XmlDocument
        myXML.Load(My.Settings.SettingsPath)
        Log("Add All Command Received")
        Dim c As New PathStructure(opn.CurrentDirectory)
        For Each fold As XmlElement In myXML.SelectNodes("//Folder")
          If Not fold.Attributes("name").Value.Contains("{") And Not fold.Attributes("name").Value.Contains("}") Then
            If Not IO.Directory.Exists(c.ReplaceVariables(c.GetURIfromXPath(FindXPath(fold)))) Then
              IO.Directory.CreateDirectory(c.ReplaceVariables(c.GetURIfromXPath(FindXPath(fold))))
            End If
            'AddFolder(args(2), fold.Attributes("name").Value)
          End If
        Next
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & defaultPath & "' != '" & opn.CurrentDirectory & "'")
        MessageBox.Show("You must be within the default path of '" & defaultPath & "'!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub
  Private Sub mnuToolAddFolder_Click(sender As Object, e As EventArgs) Handles mnuToolAddFolder.Click
    pnlContainer.Controls.Clear()
    Dim opn As New SelectFolderDialog
    opn.Title = "Select a 'parts' folder to format:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.Directory.Exists(opn.CurrentDirectory) And opn.DialogResult = Windows.Forms.DialogResult.OK Then
      If GetUNCPath(opn.CurrentDirectory).StartsWith(defaultPath) And Not GetUNCPath(opn.CurrentDirectory) = defaultPath Then
        Log("Add Command Received")
        Dim fi As New Add_Folder(opn.CurrentDirectory)
        fi.Dock = DockStyle.Fill

        pnlContainer.Controls.Add(fi)
        'AddFolder(args(2), args(?))
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & defaultPath & "' != '" & opn.CurrentDirectory & "'")
        MessageBox.Show("You must select a folder within the default path of '" & defaultPath & "'!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub
  Private Sub mnuToolsFormat_Click(sender As Object, e As EventArgs) Handles mnuToolsFormat.Click
    pnlContainer.Controls.Clear()
    Dim opn As New OpenFileDialog
    opn.Title = "Select a file under the 'parts' folder to format:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.File.Exists(opn.FileName) Then
      If GetUNCPath(opn.FileName).StartsWith(defaultPath) Then
        Log("Format Command Received")
        Dim fi As New Format_Item(opn.FileName)
        fi.Dock = DockStyle.Fill

        pnlContainer.Controls.Add(fi)
        'FormatFile(args(2), args(?))
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & defaultPath & "' != '" & opn.FileName & "'")
        MessageBox.Show("You must be within the default path of '" & defaultPath & "'!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub

  Private Sub mnuToolsAuditFile_Click(sender As Object, e As EventArgs) Handles mnuToolsAuditFile.Click
    Dim opn As New OpenFileDialog
    opn.Title = "Select a file under the 'parts' folder to format:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.File.Exists(opn.FileName) Then
      If GetUNCPath(opn.FileName).StartsWith(defaultPath) Then
        Dim pt As New PathStructure(opn.FileName)
        Dim bads As New List(Of String)

        '' If audit report exists, then delete it
        If IO.File.Exists(pt.StartPath & "\Audit Report.html") Then IO.File.Delete(pt.StartPath & "\Audit Report.html")
        Dim auditRpt As New PathStructure.AuditReport(pt)
        If Not pt.Audit(auditRpt, bads) Then
          MessageBox.Show("Invalid path(s) found. Click OK to open temporary report...", "Audit Failed!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
          Try
            auditRpt.Report("Bad Paths: " & String.Join("<br />", bads.ToArray), PathStructure.AuditReport.StatusCode.ErrorStatus)
            IO.File.WriteAllText(pt.StartPath & "\Audit Report.html", auditRpt.ReportMarkup)
            Process.Start(pt.StartPath & "\Audit Report.html")
          Catch ex As Exception
            MessageBox.Show("An error occurred while attempting to create the audit report: " & vbLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
          End Try
        Else
          MessageBox.Show("Audit Successful!" & vbLf & "No errors found in the Path Structure", "Audit Success!", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        pt.LogData(pt.StartPath & "\Audit Report.html", "Generic Audit")
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & defaultPath & "' != '" & opn.FileName & "'")
        MessageBox.Show("You must be within the default path of '" & defaultPath & "'!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub
  Private Sub mnuToolsAuditFolder_Click(sender As Object, e As EventArgs) Handles mnuToolsAuditFolder.Click
    Dim opn As New SelectFolderDialog
    opn.Title = "Select a 'parts' folder to format:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.Directory.Exists(opn.CurrentDirectory) And opn.DialogResult = Windows.Forms.DialogResult.OK Then
      If GetUNCPath(opn.CurrentDirectory).StartsWith(defaultPath) And Not GetUNCPath(opn.CurrentDirectory) = defaultPath Then
        Dim pt As New PathStructure(opn.CurrentDirectory)
        Dim bads As New List(Of String)

        '' If audit report exists, then delete it
        If IO.File.Exists(pt.StartPath & "\Audit Report.html") Then IO.File.Delete(pt.StartPath & "\Audit Report.html")
        Dim auditRpt As New PathStructure.AuditReport(pt)
        If Not pt.Audit(auditRpt, bads) Then
          MessageBox.Show("Invalid path(s) found. Click OK to open temporary report...", "Audit Failed!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
          Try
            auditRpt.Report("Bad Paths:" & String.Join("<br />", bads.ToArray), PathStructure.AuditReport.StatusCode.ErrorStatus)
            IO.File.WriteAllText(pt.StartPath & "\Audit Report.html", auditRpt.ReportMarkup)
            Process.Start(pt.StartPath & "\Audit Report.html")
          Catch ex As Exception
            MessageBox.Show("An error occurred while attempting to create the audit report: " & vbLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
          End Try
        Else
          MessageBox.Show("Audit Successful!" & vbLf & "No errors found in the Path Structure", "Audit Success!", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        pt.LogData(pt.StartPath & "\Audit Report.html", "Generic Audit")
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & defaultPath & "' != '" & opn.CurrentDirectory & "'")
        MessageBox.Show("You must be within the default path of '" & defaultPath & "'!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub

  Private cancelAudit As Boolean = False
  Private Sub CancelAudit_Clicked(ByVal sender As System.Object, ByVal e As EventArgs)
    cancelAudit = True
  End Sub
  Private Sub mnuToolsAuditDefaultPath_Click(sender As Object, e As EventArgs) Handles mnuToolsAuditDefaultPath.Click
    pnlContainer.Controls.Clear()
    Dim rtb As New RichTextBox
    rtb.Dock = DockStyle.Fill
    Dim btnCancel As New Button
    btnCancel.Text = "Cancel Audit"
    btnCancel.Dock = DockStyle.Bottom
    AddHandler btnCancel.Click, AddressOf CancelAudit_Clicked
    pnlContainer.Controls.Add(btnCancel)
    pnlContainer.Controls.Add(rtb)


    Dim tot, bad As Integer
    Dim stp As New Stopwatch
    stp.Start()
    Dim auditRpt As New PathStructure.AuditReport(New PathStructure(defaultPath))
    Dim blnSuccess As Boolean = True
    Dim curIndex As Integer = 0
    Dim totCustomers As Integer = IO.Directory.EnumerateDirectories(defaultPath).Count
    Dim lstTime As New List(Of Double)
    Dim lstAverageHistory As New List(Of Double)
    Dim stpWatch As New Stopwatch
    Dim averageTime As Double

    For Each cust As String In IO.Directory.GetDirectories(defaultPath)
      statProgress.Value = (curIndex / totCustomers) * 100
      '' If parsed Customer is invalid in E2, then continue by skipping
      If My.Settings.blnERPCheck Then
        If Not IsValidCustomerCode(cust.Remove(0, cust.LastIndexOf("\") + 1)) Then
          auditRpt.Report("Skipped '" & cust & "' as the Customer Code could not be found in the E2 database.", PathStructure.AuditReport.StatusCode.ErrorStatus)
          rtb.AppendText("Skipping '" & cust & "' because could not be found in E2" & vbCrLf)
          Continue For
        End If
      End If

      For Each part As String In IO.Directory.GetDirectories(cust)
        stpWatch.Restart()
        rtb.AppendText("Auditing '" & part & "'..." & vbLf)
        statCurrentPath.Text = "'" & part & "'"
        'Debug.WriteLine("Auditing '" & part & "'...")
        Application.DoEvents()

        Dim pt As New PathStructure(part)
        'Dim bads As New List(Of String)

        If Not pt.Audit(auditRpt) Then ', bads) Then
          'auditRpt.Report("Bad Paths:" & String.Join("<br />", bads.ToArray), PathStructure.AuditReport.StatusCode.ErrorStatus)
          'rtb.AppendText(vbTab & "Found '" & bads.Count.ToString & "' errors!" & vbLf)
          bad += 1
          blnSuccess = False
        End If

        'pt.LogData(defaultPath & "\Audit Report.html", "Generic Audit")

        rtb.ScrollToCaret()
        tot += 1

        '' Do math to display
        lstTime.Add(stpWatch.ElapsedMilliseconds)
        averageTime += stpWatch.ElapsedMilliseconds
        lstAverageHistory.Add((averageTime / lstTime.Count))
        statStatus.Text = "Average Processing Time (ms): " & (averageTime / lstTime.Count).ToString & vbTab & "Last Processing Time (ms): " & stpWatch.ElapsedMilliseconds.ToString

        If cancelAudit Then Exit For
      Next
      curIndex += 1

      System.GC.Collect()

      If rtb.Lines.Count > 100 Then
        rtb.Clear()
      End If
      If cancelAudit Then Exit For
    Next
    cancelAudit = False


    auditRpt.Raw(New HTML.HTMLWriter.HTMLCanvas("canvGraph", New HTML.AttributeList({New HTML.AttributeList.AttributeItem("width", "400"), New HTML.AttributeList.AttributeItem("height", "400")}, {New HTML.AttributeList.StyleItem("border", "1px solid black")})).Markup)
    auditRpt.Raw("<script>" & My.Resources.DrawGraph)
    Dim dataPoints As New System.Text.StringBuilder
    dataPoints.Append(String.Join(",", lstTime.ToArray))
    If dataPoints.Chars(dataPoints.Length - 1) = "," Then dataPoints.Remove(dataPoints.Length - 1, 1)
    auditRpt.Raw("var dataPoints = [" & dataPoints.ToString & "];")
    dataPoints = New System.Text.StringBuilder
    dataPoints.Append(String.Join(",", lstAverageHistory.ToArray))
    If dataPoints.Chars(dataPoints.Length - 1) = "," Then dataPoints.Remove(dataPoints.Length - 1, 1)
    auditRpt.Raw("var dataAverage = [" & dataPoints.ToString & "];")
    auditRpt.Raw("DrawPoints(dataAverage, 'yellow');")
    auditRpt.Raw("DrawPoints(dataPoints, 'black');</script>")

    stp.Stop()
    Dim ts As New TimeSpan(stp.ElapsedTicks)
    rtb.AppendText(vbLf & "Audit completed in '" & ts.Hours.ToString & ":" & ts.Minutes.ToString & ":" & ts.Seconds.ToString & "." & ts.Milliseconds.ToString & "'" & vbLf)
    rtb.AppendText(bad.ToString & "/" & tot.ToString & " part folders contained Path Structure error(s)." & vbLf)

    If Not blnSuccess Then
      MessageBox.Show("Invalid path(s) found. Click OK to open temporary report...", "Audit Failed!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      IO.File.WriteAllText(defaultPath & "\Audit Report.html", auditRpt.ReportMarkup)
      Try
        Process.Start(defaultPath & "\Audit Report.html")
      Catch ex As Exception
        MessageBox.Show("An error occurred while attempting to create the audit report: " & vbLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
      End Try
    Else
      MessageBox.Show("Audit Successful!" & vbLf & "No errors found in the Path Structure", "Audit Success!", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End If
  End Sub

  Private Sub mnuToolsClipboard_Click(sender As Object, e As EventArgs) Handles mnuToolsClipboard.Click
    Dim opn As New SelectFolderDialog
    opn.Title = "Select a 'parts' folder to format:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.Directory.Exists(opn.CurrentDirectory) And opn.DialogResult = Windows.Forms.DialogResult.OK Then
      If GetUNCPath(opn.CurrentDirectory).StartsWith(defaultPath) Then
        Log("Clipboard Command Received")
        Dim fi As New FileClipboard(opn.CurrentDirectory)
        fi.Dock = DockStyle.Fill

        pnlContainer.Controls.Add(fi)
        'FormatFile(args(2), args(?))
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & defaultPath & "' != '" & opn.CurrentDirectory & "'")
        MessageBox.Show("You must be within the default path of '" & defaultPath & "'!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub

  Private Sub mnuToolsTransferFilesByExtension_Click(sender As Object, e As EventArgs) Handles mnuToolsTransferFilesByExtension.Click
    Dim opn As New SelectFolderDialog
    opn.Title = "Select a 'parts' folder to search for transfer files:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.Directory.Exists(opn.CurrentDirectory) And opn.DialogResult = Windows.Forms.DialogResult.OK Then
      If GetUNCPath(opn.CurrentDirectory).StartsWith(defaultPath) Then
        Log("Transfer_Files_By_Extension Command Received")
        Dim fi As New Transfer_FilesByExtension(opn.CurrentDirectory)
        fi.Dock = DockStyle.Fill

        pnlContainer.Controls.Add(fi)
        'FormatFile(args(2), args(?))
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & defaultPath & "' != '" & opn.CurrentDirectory & "'")
        MessageBox.Show("You must be within the default path of '" & defaultPath & "'!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub
End Class
