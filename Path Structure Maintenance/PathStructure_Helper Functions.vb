Imports System.IO, System.Text
Imports System.Xml
Imports System.Data, System.Data.OleDb

Module PathStructure_Helper_Functions
  Public defaultPath, custCode, partNo As String

  Declare Function WNetGetConnection Lib "mpr.dll" Alias "WNetGetConnectionA" (ByVal lpszLocalName As String, _
     ByVal lpszRemoteName As String, ByRef cbRemoteName As Integer) As Integer

  Public Function GetUNCPath(ByVal sFilePath As String) As String
    Dim allDrives() As DriveInfo = DriveInfo.GetDrives()
    Dim d As DriveInfo
    Dim DriveType, Ctr As Integer
    Dim DriveLtr, UNCName As String
    Dim StrBldr As New StringBuilder

    If sFilePath.StartsWith("\\") Then Return sFilePath

    UNCName = Space(160)

    DriveLtr = sFilePath.Substring(0, 3)

    For Each d In allDrives
      If d.Name = DriveLtr Then
        DriveType = d.DriveType
        Exit For
      End If
    Next

    If DriveType = 4 Then

      Ctr = WNetGetConnection(sFilePath.Substring(0, 2), UNCName, UNCName.Length)

      If Ctr = 0 Then
        UNCName = UNCName.Trim
        For Ctr = 0 To UNCName.Length - 1
          Dim SingleChar As Char = UNCName(Ctr)
          Dim asciiValue As Integer = Asc(SingleChar)
          If asciiValue > 0 Then
            StrBldr.Append(SingleChar)
          Else
            Exit For
          End If
        Next
        StrBldr.Append(sFilePath.Substring(2))
        Return StrBldr.ToString
      Else
        Return ""
        'MsgBox("Cannot Retrieve UNC path" & vbCrLf & "Must Use Mapped Drive of SQLServer", MsgBoxStyle.Critical)
      End If
    Else
      Return ""
      'MsgBox("Cannot Use Local Drive" & vbCrLf & "Must Use Mapped Drive of SQLServer", MsgBoxStyle.Critical)
    End If
  End Function

  Public Sub Log(ByVal input As String)
    IO.File.AppendAllText(My.Computer.FileSystem.SpecialDirectories.MyDocuments.ToString & "\Path Structure Log.txt", input & vbLf)
    Debug.WriteLine(input)
  End Sub

  Public Function GetInternalString(ByVal Input As String, ByVal Left As String, ByVal Right As String) As String
    If Input.Contains(Left) And Input.Contains(Right) Then
      Return Input.Substring(Input.IndexOf(Left) + Left.Length, Input.IndexOf(Right, Input.IndexOf(Left) + Left.Length) - Input.IndexOf(Left) - Left.Length)
    Else
      Return Input
    End If
  End Function
  Public Function GetListOfInternalStrings(ByVal Input As String, ByVal Left As String, ByVal Right As String) As List(Of String)
    Dim lst As New List(Of String)
    Do Until Not Input.Contains(Left)
      If Input.Contains(Left) And Input.Contains(Right) Then
        lst.Add(GetInternalString(Input, Left, Right))
        Input = Input.Replace(Left & lst(lst.Count - 1) & Right, "|" & lst(lst.Count - 1) & "|")
      Else
        Exit Do
      End If
    Loop
    Return lst
  End Function

  Public Function AddFolder(ByVal CurrentPath As String, ByVal PathName As String)
    Dim strTemp As String
    Dim dg As DialogResult
    strTemp = IO.Path.Combine(CurrentPath, PathName)

    Dim pt As New PathStructure(CurrentPath)

    Dim folderName As String
    folderName = pt.ReplaceVariables(strTemp)
    If folderName.Contains("{Date}") Then folderName = folderName.Replace("{Date}", DateTime.Now.ToString("MM-dd-yyyy"))
    If folderName.Contains("{Time}") Then folderName = folderName.Replace("{Time}", DateTime.Now.ToString("hh-mm-ss tt"))

    Log("Pre-Folder: " & folderName)

    Dim inpt As List(Of String) = GetListOfInternalStrings(folderName, "{", "}")
    If inpt.Count > 0 Then
      Try
        Dim genDialog As New Generic_Dialog(inpt.Distinct.ToArray)
        genDialog.ShowDialog()
        If genDialog.DialogResult = Windows.Forms.DialogResult.OK Then
          For Each Val As KeyValuePair(Of String, String) In genDialog.Values
            folderName = folderName.Replace("{" & Val.Key & "}", Val.Value)
          Next
        Else
          Return False '' User chose not to continue somehow
        End If
      Catch ex As Exception
        Log("Error showing dialog" & vbCrLf & vbTab & ex.Message)
        Return False
      End Try
    End If

    Log("Post-FolderName: " & folderName)

    dg = MessageBox.Show("Are you sure you wish to create the following directory?" & vbLf & "'" & folderName & "'", "Verify", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
    If dg = Windows.Forms.DialogResult.Yes Then
      IO.Directory.CreateDirectory(folderName)
      pt.LogData(folderName, "Create Folder")
    End If
    Return True
  End Function
  'Public Function FormatFile(ByVal CurrentPath As String, ByVal PathName As String)
  '  Dim strTemp, custCode, partNo As String
  '  Dim dg As DialogResult
  '  Dim myXML As New XmlDocument
  '  myXML.Load(My.Settings.SettingsPath)

  '  dg = MessageBox.Show("Are you sure you wish to format to the following file type?" & vbLf & "'" & PathName & "'", "Verify", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
  '  If dg = Windows.Forms.DialogResult.Yes Then
  '    Dim filInfo As IO.FileInfo = New IO.FileInfo(CurrentPath)
  '    custCode = GetUNCPath(filInfo.DirectoryName).Replace(defaultPath & "\", "")
  '    custCode = custCode.Remove(custCode.IndexOf("\"))
  '    partNo = GetUNCPath(filInfo.DirectoryName).Replace(defaultPath & "\", "")
  '    partNo = partNo.Remove(0, partNo.IndexOf("\") + 1)
  '    If partNo.Contains("\") Then partNo = partNo.Remove(partNo.IndexOf("\"))
  '    Log(vbTab & "Directory: " & filInfo.DirectoryName)
  '    Log(vbTab & "Filename: " & filInfo.Name)
  '    Log(vbTab & "Extension: " & filInfo.Extension)
  '    Log(vbTab & "CustCode: " & custCode)
  '    Log(vbTab & "PartNo: " & partNo)

  '    Dim fileName As String
  '    If PathName.Contains("|") Then
  '      fileName = myXML.SelectSingleNode("//File[@name='" & PathName.Remove(PathName.IndexOf("|")) & "']/Option[@name='" & PathName.Remove(0, PathName.IndexOf("|") + 1) & "']").InnerText
  '      If fileName.Contains("{name}") Then fileName = fileName.Replace("{name}", PathName.Remove(0, PathName.IndexOf("|") + 1))
  '    Else
  '      fileName = myXML.SelectSingleNode("//File[@name='" & PathName & "']").InnerText
  '      If fileName.Contains("{name}") Then fileName = fileName.Replace("{name}", PathName.Remove(PathName.IndexOf("|")))
  '    End If
  '    If fileName.Contains("{PartNo}") Then fileName = fileName.Replace("{PartNo}", partNo)
  '    If fileName.Contains("{Date}") Then fileName = fileName.Replace("{Date}", DateTime.Now.ToString("MM-dd-yyyy"))
  '    If fileName.Contains("{Time}") Then fileName = fileName.Replace("{Time}", DateTime.Now.ToString("hh-mm-ss tt"))

  '    Dim input As List(Of String) = GetListOfInternalStrings(fileName, "{", "}")
  '    If input.Count > 0 Then
  '      Try
  '        'Dim genDialog As New Generic_Dialog(input.Distinct.ToArray)
  '        'genDialog.ShowDialog()
  '        'If genDialog.DialogResult = Windows.Forms.DialogResult.OK Then
  '        '  For Each Val As KeyValuePair(Of String, String) In genDialog.Values
  '        '    fileName = fileName.Replace("{" & Val.Key & "}", Val.Value)
  '        '  Next
  '        'Else
  '        '  Return False '' User chose not to continue somehow
  '        'End If
  '      Catch ex As Exception
  '        Log("Error showing dialog" & vbCrLf & vbTab & ex.Message)
  '        Return False
  '      End Try
  '    End If
  '    Log("Changing the filename from '" & CurrentPath & "' to '" & filInfo.DirectoryName & "\" & fileName & filInfo.Extension & "'")
  '    If Not IO.Directory.Exists(defaultPath & "\" & custCode & "\" & partNo & "\Archive\") Then
  '      IO.Directory.CreateDirectory(defaultPath & "\" & custCode & "\" & partNo & "\Archive\")
  '    End If
  '    IO.File.Copy(CurrentPath, defaultPath & "\" & custCode & "\" & partNo & "\Archive\" & DateTime.Now.ToString("yyyy-MM-dd hh-mm tt") & "_" & filInfo.Name)
  '    IO.File.Move(CurrentPath, filInfo.DirectoryName & "\" & fileName & filInfo.Extension)
  '  End If
  '  Return True
  'End Function

  Public Function FindXPath(ByVal node As XmlNode) As String
    Dim builder As New StringBuilder
    While Not IsNothing(node)
      Select Case node.NodeType
        Case XmlNodeType.Attribute
          builder.Insert(0, "/@" & node.Name)
          node = DirectCast(node, XmlAttribute).OwnerElement
          Continue While
        Case XmlNodeType.Element
          Dim index As Integer = FindElementIndex(DirectCast(node, XmlElement))
          builder.Insert(0, "/" & node.Name & "[" & index & "]")
          node = node.ParentNode
        Case XmlNodeType.Document
          Return builder.ToString
        Case Else
          Throw New ArgumentException("Only elements and attributes are supported")
      End Select
    End While
    Throw New ArgumentException("Node was not in a document")
  End Function
  Private Function FindElementIndex(ByVal Element As XmlElement) As Integer
    Dim parentNode As XmlNode = Element.ParentNode
    If parentNode.NodeType = XmlNodeType.Document Then
      Return 1
    End If
    Dim parent As XmlElement = DirectCast(parentNode, XmlElement)
    Dim index As Integer = 1
    For Each candidate As XmlNode In parent.ChildNodes
      If candidate.NodeType = XmlNodeType.Element And candidate.Name = Element.Name Then
        If DirectCast(candidate, XmlElement).Equals(Element) Then
          Return index
        End If
        index += 1
      End If
    Next
    Throw New ArgumentException("Couldn't find element within parent")
  End Function
  Public Function GetNthIndexOf(ByVal Input As String, ByVal Ch As Char, ByVal Index As Integer) As Integer
    Dim count As Integer = 0
    For i = 0 To Input.Length - 1 Step 1
      If Input(i) = Ch Then
        count += 1
        If count = Index Then
          Return i
        End If
      End If
    Next
    Return -1
  End Function

  Public Function IsValidCustomerCode(ByVal CustCode As String) As Boolean
    Dim blnFound As Boolean = False
    Using Cnn As New OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=\\server\SHARE\Systems Database\Access Database\IrongateApplications.mdb;")
      Cnn.Open()
      Using Cmd As New OleDbCommand("SELECT CustCode,User_Text3 FROM dbo_Estim WHERE CustCode=" & Chr(34) & CustCode & Chr(34) & " OR User_Text3=" & Chr(34) & CustCode & Chr(34), Cnn)
        For Each Rcd As IDataRecord In Cmd.ExecuteReader
          If Rcd.Item("CustCode") = CustCode Or Rcd.Item("User_Text3").ToString = CustCode Then
            blnFound = True
            Exit For
          End If
        Next
      End Using
      Cnn.Close()
    End Using
    Return blnFound
  End Function
End Module
