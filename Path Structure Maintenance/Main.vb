Imports System.Xml
Imports System.Security.Principal
Imports HTML, HTML.HTMLWriter

Public Class Main
  'Public _xml As XmlDocument
  Const MemoryThreshold As ULong = 3000

  Private Sub Main_Load(sender As Object, e As EventArgs) Handles MyBase.Load
    My.Application.SaveMySettingsOnExit = True

    myXML = New XmlDocument
    myXML.Load(My.Settings.SettingsPath)
    For Each struct As XmlElement In myXML.SelectNodes("//Structure")
      defaultPaths.Add(struct.Attributes("defaultPath").Value)
    Next
    'defaultPath = myXML.SelectSingleNode("Structure").Attributes("defaultPath").Value.ToLower
    'Dim custCode As String = ""
    'Dim partNo As String = ""

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
              If IsInDefaultPath(GetUNCPath(args(2))) Then ' GetUNCPath(args(2)).ToLower.StartsWith(defaultPath) Then
                statCurrentPath.Text = GetUNCPath(args(2))
                Log("Add Command Received")
                Dim fi As New Add_Folder(GetUNCPath(args(2)))
                fi.Dock = DockStyle.Fill

                pnlContainer.Controls.Add(fi)
                'AddFolder(args(2), args(?))
              Else
                Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(args(2)) & "'")
                MessageBox.Show("You must be within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
              End If
            End If
          Case "-addall"
            If args.Length >= 3 Then
              If IsInDefaultPath(GetUNCPath(args(2))) Then ' GetUNCPath(args(2)).ToLower.StartsWith(defaultPath) Then
                statCurrentPath.Text = GetUNCPath(args(2))
                dg = MessageBox.Show("Are you sure you wish to create all main folders in the selected directory?", "Verify", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                If dg = Windows.Forms.DialogResult.Yes Then
                  Log("Add All Command Received")
                  Dim c As New PathStructure(GetUNCPath(args(2)))
                  For Each fold As XmlElement In myXML.SelectNodes("//Folder")
                    If Not fold.Attributes("name").Value.Contains("{") And Not fold.Attributes("name").Value.Contains("}") Then
                      If Not IO.Directory.Exists(ReplaceVariables(GetURIfromXPath(FindXPath(fold)), c.UNCPath)) Then 'c.ReplaceVariables(c.GetURIfromXPath(FindXPath(fold)))) Then
                        IO.Directory.CreateDirectory(ReplaceVariables(GetURIfromXPath(FindXPath(fold)), c.UNCPath)) 'c.ReplaceVariables(c.GetURIfromXPath(FindXPath(fold))))
                      End If
                      'AddFolder(args(2), fold.Attributes("name").Value)
                    End If
                  Next
                End If
              Else
                Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(args(2)) & "'")
                MessageBox.Show("You must be within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
              End If
              Application.Exit()
            End If
          Case "-format"
            If args.Length >= 3 Then
              If IsInDefaultPath(GetUNCPath(args(2))) Then ' GetUNCPath(args(2)).ToLower.StartsWith(defaultPath) Then
                statCurrentPath.Text = GetUNCPath(args(2))
                Log("Format Command Received")
                Dim fi As New Format_Item(GetUNCPath(args(2)))
                fi.Dock = DockStyle.Fill

                pnlContainer.Controls.Add(fi)
                'FormatFile(args(2), args(?))
              Else
                Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(args(2)) & "'")
                MessageBox.Show("You must be within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
              End If
            End If
          Case "-audit"
            If args.Length >= 3 Then
              If IsInDefaultPath(GetUNCPath(args(2))) Then ' GetUNCPath(args(2)).ToLower.StartsWith(defaultPath) Then
                statCurrentPath.Text = GetUNCPath(args(2))
                Log("Audit Command Received")

                Dim pt As New PathStructure(GetUNCPath(args(2)))
                Dim bads As New List(Of String)

                '' If audit report exists, then delete it
                If IO.File.Exists(pt.UNCPath & "\Audit Report.html") Then IO.File.Delete(pt.StartPath & "\Audit Report.html")
                Dim auditRpt As New PathStructure.AuditVisualReport(pt)
                auditRpt.Audit()
                IO.File.WriteAllText(pt.UNCPath & "\Audit Report.html", auditRpt.ReportMarkup)
                MessageBox.Show("Audit Successful!", "Audit Complete!", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Process.Start(pt.UNCPath & "\Audit Report.html")
                pt.LogData(pt.StartPath & "\Audit Report.html", "Generic Audit")
                Application.Exit()
              Else
                Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(args(2)) & "'")
              End If
            End If
          Case "-clipboard"
            If args.Length >= 3 Then
              If IsInDefaultPath(GetUNCPath(args(2))) Then ' GetUNCPath(args(2)).ToLower.StartsWith(defaultPath) Then
                statCurrentPath.Text = GetUNCPath(args(2))
                Log("Clipboard Command Received")
                Dim fi As New FileClipboard(GetUNCPath(args(2)))
                fi.Dock = DockStyle.Fill

                pnlContainer.Controls.Add(fi)
                'FormatFile(args(2), args(?))
              Else
                Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(args(2)) & "'")
                MessageBox.Show("You must be within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
              End If
            End If
          Case "-transfer"
            If args.Length >= 3 Then
              If IsInDefaultPath(GetUNCPath(args(2))) Then ' GetUNCPath(args(2)).ToLower.StartsWith(defaultPath) Then
                statCurrentPath.Text = args(2)
                Log("Transfer_Files_By_Extension Command Received")
                Dim fi As New Transfer_FilesByExtension(args(2))
                fi.Dock = DockStyle.Fill

                pnlContainer.Controls.Add(fi)
                'FormatFile(args(2), args(?))
              Else
                Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(args(2)) & "'")
                MessageBox.Show("You must be within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Application.Exit()
              End If
            End If
          Case "-preview"
            If args.Length >= 3 Then
              If IsInDefaultPath(GetUNCPath(args(2))) Then ' GetUNCPath(args(2)).ToLower.StartsWith(defaultPath) Then
                statCurrentPath.Text = GetUNCPath(args(2))
                Log("Preview Command Received")

                '' Load preview control
                Me.WindowState = FormWindowState.Maximized
                Application.DoEvents()
                Dim prev As New Preview(GetUNCPath(args(2)))
                prev.Dock = DockStyle.Fill
                pnlContainer.Controls.Add(prev)
                prev.lstFolders.Focus()
              Else
                Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(args(2)) & "'")
                MessageBox.Show("You must be within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Application.Exit()
              End If
            End If
          Case "-heatmap"
            If args.Length >= 3 Then
              statCurrentPath.Text = GetUNCPath(args(2))
              Log("Folder Heat Map Command Received")

              '' Load preview control
              Me.WindowState = FormWindowState.Maximized
              Application.DoEvents()
              Dim fhm As New FolderHeatMap(GetUNCPath(args(2)))
              fhm.Dock = DockStyle.Fill
              pnlContainer.Controls.Add(fhm)
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
    Dim dialogSelect As New Select_Default_Path()
    Dim dg As DialogResult = dialogSelect.ShowDialog
    Dim defaultPath As String
    If dg = Windows.Forms.DialogResult.OK And Not String.IsNullOrEmpty(dialogSelect.DefaultPath) Then
      defaultPath = dialogSelect.DefaultPath
    Else
      Exit Sub
    End If

    pnlContainer.Controls.Clear()
    Dim opn As New SelectFolderDialog
    opn.Title = "Select a 'parts' folder to format:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.Directory.Exists(opn.CurrentDirectory) And opn.DialogResult = Windows.Forms.DialogResult.OK Then
      If IsInDefaultPath(GetUNCPath(opn.CurrentDirectory), defaultPath) Then ' IsInDefaultPath(GetUNCPath(opn.CurrentDirectory)) Then ' GetUNCPath(opn.CurrentDirectory).StartsWith(defaultPath) Then
        Debug.WriteLine("Adding folders for '" & GetUNCPath(opn.CurrentDirectory) & "'")
        If IsNothing(myXML) Then
          myXML = New XmlDocument
          myXML.Load(My.Settings.SettingsPath)
        End If
        Log("Add All Command Received")
        Dim c As New PathStructure(GetUNCPath(opn.CurrentDirectory))
        For Each fold As XmlElement In myXML.SelectNodes("//Folder")
          If Not fold.Attributes("name").Value.Contains("{") And Not fold.Attributes("name").Value.Contains("}") Then
            If Not IO.Directory.Exists(ReplaceVariables(GetURIfromXPath(FindXPath(fold)), c.UNCPath)) Then 'c.ReplaceVariables(c.GetURIfromXPath(FindXPath(fold)))) Then
              IO.Directory.CreateDirectory(ReplaceVariables(GetURIfromXPath(FindXPath(fold)), c.UNCPath)) 'c.ReplaceVariables(c.GetURIfromXPath(FindXPath(fold))))
            End If
            'AddFolder(args(2), fold.Attributes("name").Value)
          End If
        Next
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(opn.CurrentDirectory) & "'")
        MessageBox.Show("You must be within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub
  Private Sub mnuToolAddFolder_Click(sender As Object, e As EventArgs) Handles mnuToolAddFolder.Click
    Dim dialogSelect As New Select_Default_Path()
    Dim dg As DialogResult = dialogSelect.ShowDialog
    Dim defaultPath As String
    If dg = Windows.Forms.DialogResult.OK And Not String.IsNullOrEmpty(dialogSelect.DefaultPath) Then
      defaultPath = dialogSelect.DefaultPath
    Else
      Exit Sub
    End If
    pnlContainer.Controls.Clear()
    Dim opn As New SelectFolderDialog
    opn.Title = "Select a 'parts' folder to format:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.Directory.Exists(opn.CurrentDirectory) And opn.DialogResult = Windows.Forms.DialogResult.OK Then
      If IsInDefaultPath(GetUNCPath(opn.CurrentDirectory)) Then ' GetUNCPath(opn.CurrentDirectory).StartsWith(defaultPath) And Not GetUNCPath(opn.CurrentDirectory) = defaultPath Then
        Log("Add Command Received")
        Dim fi As New Add_Folder(GetUNCPath(opn.CurrentDirectory))
        fi.Dock = DockStyle.Fill

        pnlContainer.Controls.Add(fi)
        'AddFolder(args(2), args(?))
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(opn.CurrentDirectory) & "'")
        MessageBox.Show("You must select a folder within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub
  Private Sub mnuToolsFormat_Click(sender As Object, e As EventArgs) Handles mnuToolsFormat.Click
    Dim dialogSelect As New Select_Default_Path()
    Dim dg As DialogResult = dialogSelect.ShowDialog
    Dim defaultPath As String
    If dg = Windows.Forms.DialogResult.OK And Not String.IsNullOrEmpty(dialogSelect.DefaultPath) Then
      defaultPath = dialogSelect.DefaultPath
    Else
      Exit Sub
    End If
    pnlContainer.Controls.Clear()
    Dim opn As New OpenFileDialog
    opn.Title = "Select a file under the 'parts' folder to format:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.File.Exists(opn.FileName) Then
      If IsInDefaultPath(GetUNCPath(opn.FileName)) Then ' GetUNCPath(opn.FileName).StartsWith(defaultPath) Then
        Log("Format Command Received")
        Dim fi As New Format_Item(GetUNCPath(opn.FileName))
        fi.Dock = DockStyle.Fill

        pnlContainer.Controls.Add(fi)
        'FormatFile(args(2), args(?))
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(opn.FileName) & "'")
        MessageBox.Show("You must be within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub
  Private Sub mnuToolsAuditFile_Click(sender As Object, e As EventArgs) Handles mnuToolsAuditFile.Click
    Dim dialogSelect As New Select_Default_Path()
    Dim dg As DialogResult = dialogSelect.ShowDialog
    Dim defaultPath As String
    If dg = Windows.Forms.DialogResult.OK And Not String.IsNullOrEmpty(dialogSelect.DefaultPath) Then
      defaultPath = dialogSelect.DefaultPath
    Else
      Exit Sub
    End If
    Dim opn As New OpenFileDialog
    opn.Title = "Select a file under the 'parts' folder to format:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.File.Exists(opn.FileName) Then
      If IsInDefaultPath(GetUNCPath(opn.FileName)) Then ' GetUNCPath(opn.FileName).StartsWith(defaultPath) Then
        Dim pt As New PathStructure(GetUNCPath(opn.FileName))
        Dim bads As New List(Of String)

        '' If audit report exists, then delete it
        If IO.File.Exists(pt.StartPath & "\Audit Report.html") Then IO.File.Delete(pt.StartPath & "\Audit Report.html")
        Dim auditRpt As New PathStructure.AuditVisualReport(pt)
        auditRpt.Audit()
        IO.File.WriteAllText(pt.StartPath & "\Audit Report.html", auditRpt.ReportMarkup)

        pt.LogData(pt.StartPath & "\Audit Report.html", "Generic Audit")
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(opn.FileName) & "'")
        MessageBox.Show("You must be within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub
  Private Sub mnuToolsAuditFolder_Click(sender As Object, e As EventArgs) Handles mnuToolsAuditFolder.Click
    Dim dialogSelect As New Select_Default_Path()
    Dim dg As DialogResult = dialogSelect.ShowDialog
    Dim defaultPath As String
    If dg = Windows.Forms.DialogResult.OK And Not String.IsNullOrEmpty(dialogSelect.DefaultPath) Then
      defaultPath = dialogSelect.DefaultPath
    Else
      Exit Sub
    End If
    Dim opn As New SelectFolderDialog
    opn.Title = "Select a 'parts' folder to format:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.Directory.Exists(opn.CurrentDirectory) And opn.DialogResult = Windows.Forms.DialogResult.OK Then
      If IsInDefaultPath(GetUNCPath(opn.CurrentDirectory)) Then ' GetUNCPath(opn.CurrentDirectory).StartsWith(defaultPath) And Not GetUNCPath(opn.CurrentDirectory) = defaultPath Then
        Dim pt As New PathStructure(GetUNCPath(opn.CurrentDirectory))

        '' If audit report exists, then delete it
        If IO.File.Exists(pt.StartPath & "\Audit Report.html") Then IO.File.Delete(pt.StartPath & "\Audit Report.html")
        Dim auditRpt As New PathStructure.AuditVisualReport(pt)
        auditRpt.Audit()
        Try
          IO.File.WriteAllText(pt.StartPath & "\Audit Report.html", auditRpt.ReportMarkup)
          Process.Start(pt.StartPath & "\Audit Report.html")
        Catch ex As Exception
          MessageBox.Show("An error occurred while attempting to create the audit report: " & vbLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        pt.LogData(pt.StartPath & "\Audit Report.html", "Generic Audit")
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(opn.CurrentDirectory) & "'")
        MessageBox.Show("You must be within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub

  Private cancelAudit As Boolean = False
  Private Sub CancelAudit_Clicked(ByVal sender As System.Object, ByVal e As EventArgs)
    cancelAudit = True
    If auditRpt IsNot Nothing Then auditRpt.Quit()
  End Sub
  'Private Sub mnuToolsAuditDefaultPath_Click(sender As Object, e As EventArgs)
  '  pnlContainer.Controls.Clear()
  '  Dim rtb As New RichTextBox
  '  rtb.Dock = DockStyle.Fill
  '  Dim btnCancel As New Button
  '  btnCancel.Text = "Cancel Audit"
  '  btnCancel.Dock = DockStyle.Bottom
  '  AddHandler btnCancel.Click, AddressOf CancelAudit_Clicked
  '  pnlContainer.Controls.Add(btnCancel)
  '  pnlContainer.Controls.Add(rtb)


  '  Dim tot, bad As Integer
  '  Dim stp As New Stopwatch
  '  stp.Start()
  '  Dim auditRpt As New PathStructure.AuditReport(New PathStructure(defaultPath))
  '  Dim blnSuccess As Boolean = True
  '  Dim curIndex As Integer = 0
  '  Dim totCustomers As Integer = IO.Directory.EnumerateDirectories(defaultPath).Count
  '  Dim lstTime As New List(Of Double)
  '  Dim lstAverageHistory As New List(Of Double)
  '  Dim stpWatch As New Stopwatch
  '  Dim averageTime As Double

  '  Dim ERPVariables As New SortedList(Of String, String)

  '  For Each cust As String In IO.Directory.GetDirectories(defaultPath)
  '    statProgress.Value = (curIndex / totCustomers) * 100
  '    curIndex += 1
  '    '' If parsed Customer is invalid in E2, then continue by skipping
  '    If My.Settings.blnERPCheck Then
  '      Dim c As New PathStructure(cust)

  '      Dim ERPCustCodeTable As String = ""
  '      ERPVariables = GetERPVariables("Audit_CustCode", ERPCustCodeTable, c)
  '      If Not IsValidERP(ERPCustCodeTable, c.Variables) Then
  '        auditRpt.Report("Skipped '" & cust & "' as the Customer Code could not be found in the E2 database.", PathStructure.AuditReport.StatusCode.ErrorStatus)
  '        rtb.AppendText("Skipping '" & cust & "' because could not be found in ERP system." & vbCrLf)
  '        Continue For
  '      Else
  '        rtb.AppendText("Found '" & cust & "' in ERP system. ")
  '      End If
  '    End If

  '    For Each part As String In IO.Directory.GetDirectories(cust)
  '      stpWatch.Restart()
  '      rtb.AppendText("Auditing '" & part & "'..." & vbLf)
  '      statCurrentPath.Text = "'" & part & "'"
  '      Application.DoEvents()

  '      Dim pt As New PathStructure(part)

  '      Dim ERPPartNoTable As String = ""
  '      '' Check if user want to check ERP system
  '      ERPVariables = GetERPVariables("Audit_PartNo", ERPPartNoTable, pt)
  '      If IsValidERP(ERPPartNoTable, ERPVariables) Then
  '        If Not pt.Audit(auditRpt) Then
  '          bad += 1
  '          blnSuccess = False
  '        End If
  '        rtb.ScrollToCaret()
  '        tot += 1

  '        '' Do math to display
  '        lstTime.Add(stpWatch.ElapsedMilliseconds)
  '        averageTime += stpWatch.ElapsedMilliseconds
  '        lstAverageHistory.Add((averageTime / lstTime.Count))
  '        statStatus.Text = "Average Processing Time (ms): " & (averageTime / lstTime.Count).ToString & vbTab & "Last Processing Time (ms): " & stpWatch.ElapsedMilliseconds.ToString

  '      End If
  '      If cancelAudit Then Exit For
  '    Next

  '    System.GC.Collect()

  '    '' Keep track of RichTextBox length and clear frequently to avoid bogging down computer
  '    If rtb.Lines.Count > 100 Then
  '      rtb.Clear()
  '    End If
  '    If cancelAudit Then Exit For
  '  Next
  '  cancelAudit = False


  '  'auditRpt.Raw(New HTML.HTMLWriter.HTMLCanvas("canvGraph", New HTML.AttributeList({New HTML.AttributeList.AttributeItem("width", "400"), New HTML.AttributeList.AttributeItem("height", "400")}, {New HTML.AttributeList.StyleItem("border", "1px solid black")})).Markup)
  '  'auditRpt.Raw("<script>" & My.Resources.DrawGraph)
  '  'Dim dataPoints As New System.Text.StringBuilder
  '  'dataPoints.Append(String.Join(",", lstTime.ToArray))
  '  'If dataPoints.Chars(dataPoints.Length - 1) = "," Then dataPoints.Remove(dataPoints.Length - 1, 1)
  '  'auditRpt.Raw("var dataPoints = [" & dataPoints.ToString & "];")
  '  'dataPoints = New System.Text.StringBuilder
  '  'dataPoints.Append(String.Join(",", lstAverageHistory.ToArray))
  '  'If dataPoints.Chars(dataPoints.Length - 1) = "," Then dataPoints.Remove(dataPoints.Length - 1, 1)
  '  'auditRpt.Raw("var dataAverage = [" & dataPoints.ToString & "];")
  '  'auditRpt.Raw("DrawPoints(dataAverage, 'yellow');")
  '  'auditRpt.Raw("DrawPoints(dataPoints, 'black');</script>")

  '  stp.Stop()
  '  Dim ts As New TimeSpan(0, 0, 0, 0, stp.ElapsedMilliseconds)
  '  rtb.AppendText(vbLf & "Audited '" & auditRpt.FileCount.ToString & "' files in '" & ts.Hours.ToString & ":" & ts.Minutes.ToString & ":" & ts.Seconds.ToString & "." & ts.Milliseconds.ToString & "'" & vbLf)
  '  rtb.AppendText(bad.ToString & "/" & tot.ToString & " part folders contained Path Structure error(s)." & vbLf)

  '  If Not blnSuccess Then
  '    MessageBox.Show("Invalid path(s) found. Click OK to open temporary report...", "Audit Failed!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
  '    IO.File.WriteAllText(defaultPath & "\Audit Report.html", auditRpt.ReportMarkup)
  '    Try
  '      Process.Start(defaultPath & "\Audit Report.html")
  '    Catch ex As Exception
  '      MessageBox.Show("An error occurred while attempting to create the audit report: " & vbLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
  '    End Try
  '  Else
  '    MessageBox.Show("Audit Successful!" & vbLf & "No errors found in the Path Structure", "Audit Success!", MessageBoxButtons.OK, MessageBoxIcon.Information)
  '  End If
  'End Sub
  Private Sub mnuToolsClipboard_Click(sender As Object, e As EventArgs) Handles mnuToolsClipboard.Click
    Dim dialogSelect As New Select_Default_Path()
    Dim dg As DialogResult = dialogSelect.ShowDialog
    Dim defaultPath As String
    If dg = Windows.Forms.DialogResult.OK And Not String.IsNullOrEmpty(dialogSelect.DefaultPath) Then
      defaultPath = dialogSelect.DefaultPath
    Else
      Exit Sub
    End If
    Dim opn As New SelectFolderDialog
    opn.Title = "Select a 'parts' folder to format:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.Directory.Exists(opn.CurrentDirectory) And opn.DialogResult = Windows.Forms.DialogResult.OK Then
      statStatus.Text = opn.CurrentDirectory
      If IsInDefaultPath(GetUNCPath(opn.CurrentDirectory), defaultPath) Then ' IsInDefaultPath(GetUNCPath(opn.CurrentDirectory)) Then ' GetUNCPath(opn.CurrentDirectory).StartsWith(defaultPath) Then
        Log("Clipboard Command Received")
        Dim fi As New FileClipboard(GetUNCPath(opn.CurrentDirectory))
        fi.Dock = DockStyle.Fill

        pnlContainer.Controls.Add(fi)
        'FormatFile(args(2), args(?))
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(opn.CurrentDirectory) & "'")
        MessageBox.Show("You must be within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub
  Private Sub mnuToolsTransferFilesByExtension_Click(sender As Object, e As EventArgs) Handles mnuToolsTransferFilesByExtension.Click
    Dim dialogSelect As New Select_Default_Path()
    Dim dg As DialogResult = dialogSelect.ShowDialog()
    Dim defaultPath As String
    If dg = Windows.Forms.DialogResult.OK And Not String.IsNullOrEmpty(dialogSelect.DefaultPath) Then
      defaultPath = dialogSelect.DefaultPath
    Else
      Exit Sub
    End If
    Debug.WriteLine("After dialog: " & defaultPath)
    Dim opn As New SelectFolderDialog
    opn.Title = "Select a 'parts' folder to search for transfer files:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.Directory.Exists(opn.CurrentDirectory) And opn.DialogResult = Windows.Forms.DialogResult.OK Then
      statStatus.Text = opn.CurrentDirectory
      If IsInDefaultPath(GetUNCPath(opn.CurrentDirectory), defaultPath) Then ' IsInDefaultPath(GetUNCPath(opn.CurrentDirectory)) Then ' GetUNCPath(opn.CurrentDirectory).StartsWith(defaultPath) Then
        Log("Transfer_Files_By_Extension Command Received")
        Dim fi As New Transfer_FilesByExtension(GetUNCPath(opn.CurrentDirectory))
        fi.Dock = DockStyle.Fill

        pnlContainer.Controls.Add(fi)
        'FormatFile(args(2), args(?))
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(opn.CurrentDirectory) & "'")
        MessageBox.Show("You must be within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub
  Private Sub mnuToolsPreview_Click(sender As Object, e As EventArgs) Handles mnuToolsPreview.Click
    Dim dialogSelect As New Select_Default_Path()
    Dim dg As DialogResult = dialogSelect.ShowDialog
    Dim defaultPath As String
    If dg = Windows.Forms.DialogResult.OK And Not String.IsNullOrEmpty(dialogSelect.DefaultPath) Then
      defaultPath = dialogSelect.DefaultPath
    Else
      Exit Sub
    End If
    Dim opn As New SelectFolderDialog
    opn.Title = "Select a folder to search for preview:"
    If IO.Directory.Exists(defaultPath) Then
      opn.InitialDirectory = defaultPath
    End If
    opn.ShowDialog()
    If IO.Directory.Exists(opn.CurrentDirectory) And opn.DialogResult = Windows.Forms.DialogResult.OK Then
      statStatus.Text = opn.CurrentDirectory
      If IsInDefaultPath(GetUNCPath(opn.CurrentDirectory), defaultPath) Then ' IsInDefaultPath(GetUNCPath(opn.CurrentDirectory)) Then ' GetUNCPath(opn.CurrentDirectory).StartsWith(defaultPath) Then
        Log("Preview Command Received")
        Dim fi As New Preview(GetUNCPath(opn.CurrentDirectory))
        fi.Dock = DockStyle.Fill

        pnlContainer.Controls.Add(fi)
        fi.lstFolders.Focus()
        'FormatFile(args(2), args(?))
      Else
        Log("Path does not match default path!" & vbCrLf & vbTab & "'" & GetUNCPath(opn.CurrentDirectory) & "'")
        MessageBox.Show("You must be within a default path!", "Wrong Folder", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
      End If
    End If
  End Sub
  Private Sub mnuGeneratePathStructure_Click(sender As Object, e As EventArgs) Handles mnuGeneratePathStructure.Click
    Dim rpt As New HTMLWriter

    rpt.AddBootstrapReference()
    rpt += "<style>ul li{list-style-type: none;cursor: pointer;}.folder:before{content:'\e118';font-family:'Glyphicons Halflings';font-size:12px;float:left;margin-top:4px;margin-left:-17px;color:#222;}.file:before{content:'\e032';font-family:'Glyphicons Halflings';font-size:12px;float:left;margin-top:4px;margin-left:-17px;color:#222;}.contains:before{color: steelblue;}</style>"
    rpt += New HTMLHeader("Path Structure", HTMLHeader.HeaderSize.H1)
    rpt += New HTMLHeader("This document is not controlled. Created " & DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"), HTMLHeader.HeaderSize.H6)

    rpt += New HTMLParagraph("Folders and Files with a name enclosed in <code>{}</code> are wildcard objects, meaning that the object name can be any valid object name.", New AttributeList({"class"}, {"alert alert-info"}))

    Dim lstMain As New HTMLList(HTMLList.ListType.Unordered, New AttributeList({"id"}, {"structure"}))

    If IsNothing(myXML) Then
      myXML = New XmlDocument
      myXML.Load(My.Settings.SettingsPath)
    End If


    Dim struct As New HTMLList.ListItem("Structure: " & myXML.SelectSingleNode("Structure").Attributes("defaultPath").Value.ToString)
    If myXML.SelectSingleNode("Structure").SelectNodes("./*").Count > 0 Then
      Dim innerList As New HTMLList(HTMLList.ListType.Unordered)
      For Each chld As XmlElement In myXML.SelectSingleNode("Structure").SelectNodes("./*")
        If Not chld.Name = "Variables" And Not chld.Name = "Variable" Then
          innerList += RecursivePathStructure(chld)
        End If
      Next
      struct.AddInnerHTML(innerList.Markup)
    End If
    lstMain.AddListItem(struct)
    'lstMain.SetList()

    rpt += lstMain.Markup

    rpt += ("<script type='text/javascript'>" & _
            "$('#structure ul li ul').toggle();" & _
            "$('.folder,.file').click(function() {" & _
            "$(this).children('ul').slideToggle();" & _
            "return false;" & _
            "});</script>")

    Dim opn As New SaveFileDialog
    opn.Title = "Select a location to save the report..."
    opn.Filter = "HTML|*.html"
    opn.FileName = "Path Structure.html"
    opn.CheckPathExists = True
    opn.OverwritePrompt = True

    opn.ShowDialog()

    If Not String.IsNullOrEmpty(opn.FileName) Then
      Try
        IO.File.WriteAllText(opn.FileName, rpt.HTMLMarkup)
        Process.Start(opn.FileName)
      Catch ex As Exception
        MessageBox.Show("An error occurred while attempting to save and open the Path Structure: " & vbLf & ex.Message & vbLf & opn.FileName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
      End Try
    End If
  End Sub
  Private Function RecursivePathStructure(ByVal node As XmlElement) As HTMLList.ListItem
    Dim li As HTMLList.ListItem
    Dim cls As String = ""
    If node.Name = "Folder" Then
      cls = "folder"
    ElseIf node.Name = "File" Or node.Name = "Option" Then
      cls = "file"
    End If
    li = New HTMLList.ListItem("<b>" & node.Attributes("name").Value.ToString & "</b>: " & node.Attributes("description").Value.ToString, New AttributeList({"class"}, {cls}))
    If node.SelectNodes("./*").Count > 0 Then
      Dim lst As New HTMLList(HTMLList.ListType.Unordered)
      For Each chld As XmlElement In node.SelectNodes("./*")
        lst.AddListItem(RecursivePathStructure(chld))
      Next
      lst.SetList()
      li.AddInnerHTML(lst.Markup)
      li.AttributeList = New AttributeList({"class"}, {cls & " contains"})
    End If
    Return li
  End Function

  Private Sub mnuToolsFolderHeatMap_Click(sender As Object, e As EventArgs) Handles mnuToolsFolderHeatMap.Click
    Dim opn As New SelectFolderDialog
    opn.Title = "Select a folder to search for preview:"
    'If IO.Directory.Exists(defaultPath) Then
    '  opn.InitialDirectory = defaultPath
    'End If
    opn.ShowDialog()
    If IO.Directory.Exists(opn.CurrentDirectory) And opn.DialogResult = Windows.Forms.DialogResult.OK Then
      statCurrentPath.Text = GetUNCPath(opn.CurrentDirectory)
      Log("Preview Command Received")

      '' Load preview control
      Me.WindowState = FormWindowState.Maximized
      Application.DoEvents()
      Dim fhm As New FolderHeatMap(GetUNCPath(opn.CurrentDirectory))
      fhm.Dock = DockStyle.Fill
      pnlContainer.Controls.Add(fhm)
    End If
  End Sub

  Dim auditList As ListBox
  Dim auditRpt As PathStructure.AuditVisualReport
  Private Sub mnuToolsAuditVisualDefaultPath_Click(sender As Object, e As EventArgs) Handles mnuToolsAuditVisualDefaultPath.Click
    Dim dialogSelect As New Select_Default_Path()
    Dim dg As DialogResult = dialogSelect.ShowDialog()
    Dim defaultPath As String
    If dg = Windows.Forms.DialogResult.OK And Not String.IsNullOrEmpty(dialogSelect.DefaultPath) Then
      defaultPath = dialogSelect.DefaultPath
    Else
      Exit Sub
    End If

    pnlContainer.Controls.Clear()
    auditList = New ListBox
    auditList.Dock = DockStyle.Fill
    Dim btnCancel As New Button
    btnCancel.Text = "Cancel Audit"
    btnCancel.Dock = DockStyle.Bottom
    AddHandler btnCancel.Click, AddressOf CancelAudit_Clicked
    pnlContainer.Controls.Add(btnCancel)
    pnlContainer.Controls.Add(auditList)


    Dim tot As Integer
    Dim stp As New Stopwatch
    stp.Start()
    'Dim auditRpt As New PathStructure.AuditVisualReport(New PathStructure(defaultPath))
    auditRpt = New PathStructure.AuditVisualReport(New PathStructure(defaultPath))
    AddHandler auditRpt.ChildAudited, AddressOf AuditChildUpdate
    AddHandler auditRpt.GrandChildAudited, AddressOf AuditGrandchildUpdate

    Dim curIndex As Integer = 0
    Dim totCustomers As Integer = IO.Directory.EnumerateDirectories(defaultPath).Count
    Dim lstTime As New List(Of Double)
    Dim lstAverageHistory As New List(Of Double)
    Dim stpWatch As New Stopwatch
    Dim averageTime As Double

    auditRpt.Audit()
    cancelAudit = False

    stp.Stop()
    Dim ts As New TimeSpan(0, 0, 0, 0, stp.ElapsedMilliseconds)
    auditList.Items.Insert(0, "Audited '" & auditRpt.FileCount.ToString & "' files in '" & ts.Hours.ToString & ":" & ts.Minutes.ToString & ":" & ts.Seconds.ToString & "." & ts.Milliseconds.ToString & "'")
    auditList.Items.Insert(0, tot.ToString & " part folders audited.")
    'rtb.AppendText(vbLf & "Audited '" & auditRpt.FileCount.ToString & "' files in '" & ts.Hours.ToString & ":" & ts.Minutes.ToString & ":" & ts.Seconds.ToString & "." & ts.Milliseconds.ToString & "'" & vbLf)
    'rtb.AppendText(tot.ToString & " part folders audited." & vbLf)

    '' Remove parts progress bar
    'StatusStrip1.Items.Remove(progParts)

    MessageBox.Show("Click OK to open temporary report...", "Audit Complete!", MessageBoxButtons.OK, MessageBoxIcon.Information)
    IO.File.WriteAllText(defaultPath & "\Audit Report.html", auditRpt.ReportMarkup)
    Try
      Process.Start(defaultPath & "\Audit Report.html")
    Catch ex As Exception
      MessageBox.Show("An error occurred while attempting to create the audit report: " & vbLf & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    End Try
  End Sub
  'Public Sub ThreadPart(ByVal params As Object)
  '  params(0).AuditVisualChildren(params(1), params(2), params(0))
  '  params(0).Dispose()
  'End Sub
  'Private Sub AuditChildUpdate(ByVal sender As Object, ByVal e As PathStructure.AuditVisualReport.AuditedEventArgs)
  '  'statProgress.Value = (e.Index / e.ParentTotal) * 100
  '  'If auditList IsNot Nothing Then
  '  '  If Not auditList.InvokeRequired Then
  '  '    auditList.Items.Insert(0, "Child audited '" & e.Path & "'")
  '  '  End If
  '  'End If
  '  Me.AuditUpdate(e)
  '  Application.DoEvents()
  'End Sub
  'Private Sub AuditGrandChildUpdate(ByVal sender As Object, ByVal e As PathStructure.AuditVisualReport.AuditedEventArgs)
  '  statCurrentPath.Text = e.Path
  '  If auditGrandchildProgress IsNot Nothing Then
  '    auditGrandchildProgress.Value = (e.Index / e.ParentTotal) * 100
  '  End If
  '  If auditList IsNot Nothing Then
  '    If Not auditList.InvokeRequired Then
  '      auditList.Items.Insert(0, "Grandchild audited '" & e.Path & "'")
  '    End If
  '  End If
  'End Sub
  Delegate Sub AuditChildUpdateCallback(e As PathStructure.AuditVisualReport.AuditedEventArgs)
  Private Sub AuditChildUpdate(ByVal e As PathStructure.AuditVisualReport.AuditedEventArgs)
    If Me.auditList.InvokeRequired Then
      Dim d As New AuditChildUpdateCallback(AddressOf AuditChildUpdate)
      Me.Invoke(d, New Object() {e})
    Else
      If Me.auditList.Items.Count > 100 Then Me.auditList.Items.Clear()
      Me.auditList.Items.Insert(0, "Audited '" & e.Path & "'")
      Me.statProgress.Value = (e.Index / e.ParentTotal) * 100
      Me.statStatus.Text = e.Index.ToString & " / " & e.ParentTotal.ToString & " children"
      auditGrandchildCount = 0
      Application.DoEvents()
    End If
  End Sub
  Delegate Sub AuditGrandchildUpdateCallback(e As PathStructure.AuditVisualReport.AuditedEventArgs)
  Private auditGrandchildCount As Integer
  Private Sub AuditGrandchildUpdate(ByVal e As PathStructure.AuditVisualReport.AuditedEventArgs)
    If Me.auditList.InvokeRequired Then
      Dim d As New AuditGrandchildUpdateCallback(AddressOf AuditGrandchildUpdate)
      Me.Invoke(d, New Object() {e})
    Else
      'If Me.auditList.Items.Count > 100 Then Me.auditList.Items.Clear()
      'Me.auditList.Items.Insert(0, "Child audited '" & e.Path & "'")
      auditGrandchildCount += 1
      Me.statCurrentPath.Text = auditGrandchildCount.ToString & " objects in child"
      Application.DoEvents()
    End If
  End Sub
End Class
