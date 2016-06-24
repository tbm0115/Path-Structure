Imports System.IO, System.Text
Imports System.Xml
Imports System.Data, System.Data.OleDb

Module PathStructure_Helper_Functions
  'Public defaultPath, custCode, partNo As String
  Public defaultPaths As New List(Of String)
  Public ERPConnection As New OleDbConnection(My.Settings.ERPConnection)
  Public myXML As XmlDocument

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
        Return sFilePath
        'MsgBox("Cannot Retrieve UNC path" & vbCrLf & "Must Use Mapped Drive of SQLServer", MsgBoxStyle.Critical)
      End If
    Else
      Return sFilePath
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
  Public Function CountStringOccurance(ByVal Input As String, ByVal Identifier As String) As Integer
    Dim i As Integer = 0
    Do Until Not Input.Contains(Identifier)
      If Input.Contains(Identifier) Then
        Input = Input.Remove(0, Input.IndexOf(Identifier) + (Identifier.Length))
        i += 1
      End If
    Loop
    Return i
  End Function
  Public Function AddFolder(ByVal CurrentPath As String, ByVal PathName As String)
    Dim strTemp As String
    Dim dg As DialogResult
    strTemp = IO.Path.Combine(CurrentPath, PathName)

    Dim pt As New PathStructure(CurrentPath)

    Dim folderName As String
    folderName = ReplaceVariables(strTemp, CurrentPath) 'pt.ReplaceVariables(strTemp)
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
  'Public Function GetFileSystemObjectType(ByVal Path As String) As PathStructure.PathType
  '  'If True Then
  '  Dim attr As FileAttributes = File.GetAttributes(Path)
  '  If (attr & FileAttributes.Directory) = FileAttributes.Directory Then
  '    Return PathStructure.PathType.Folder
  '  Else
  '    Return PathStructure.PathType.File
  '  End If
  '  'Else
  '  '  Throw New ArgumentException("Path Structure: The path provided does not appear to exist! '" & Path & "'")
  '  'End If
  'End Function
  Public Function IsInDefaultPath(ByVal Input As String, Optional ByVal PreferredPath As String = "") As Boolean
    For i = 0 To defaultPaths.Count - 1 Step 1
      If (Input.IndexOf(defaultPaths(i), System.StringComparison.OrdinalIgnoreCase) >= 0) Or (defaultPaths(i).IndexOf(Input, System.StringComparison.OrdinalIgnoreCase) >= 0) Then
        If Not String.IsNullOrEmpty(PreferredPath) And Not String.Equals(defaultPaths(i), PreferredPath) Then
          Continue For
        End If
        Return True
      End If
    Next
    Return False
  End Function

  ''' <summary>
  ''' Replaces PathStructure variables with provided values.
  ''' </summary>
  ''' <param name="Input">Full or partial path string</param>
  ''' <returns>String</returns>
  ''' <remarks></remarks>
  Public Function ReplaceVariables(ByVal Input As String, ByVal Path As String) As String
    Dim vars As New PathStructure.VariableArray("//Variables", Path)
    Return vars.Replace(Input)
  End Function

  ''' <summary>
  ''' Converts the XPath for the PathStructure into a valid FileSystem path.
  ''' </summary>
  ''' <param name="XPath">XML XPath</param>
  ''' <returns>String</returns>
  ''' <remarks></remarks>
  Public Function GetURIfromXPath(ByVal XPath As String) As String
    If Not String.IsNullOrEmpty(XPath) Then
      Dim x As XmlElement = myXML.SelectSingleNode(XPath)
      '' Check if the element has the temporary URI for this session. If so, use it
      If x.HasAttribute("tmpURI") Then
        Return x.Attributes("tmpURI").Value
      Else
        Dim a As XmlAttribute = myXML.CreateAttribute("tmpURI")
        x.Attributes.Append(a)
        Dim u As New StringBuilder
        Do Until x.Name = "Structure"
          If String.Equals(x.Name, "Folder", StringComparison.OrdinalIgnoreCase) Then
            u.Insert(0, x.Attributes("name").Value & "\")
          ElseIf String.Equals(x.Name, "File", StringComparison.OrdinalIgnoreCase) Then
            u.Append(x.InnerText)
          ElseIf String.Equals(x.Name, "Option", StringComparison.OrdinalIgnoreCase) Then
            u.Append(x.InnerText)
            x = x.ParentNode '' Set parent node to 'File' so that the next x-set will set x to the folder
          End If
          x = x.ParentNode
        Loop
        'u = _startPath & u
        u.Insert(0, x.Attributes("defaultPath").Value & "\") ' & x.Attributes("path").Value & "\")
        a.Value = u.ToString
        Return u.ToString
      End If
    Else
      Return ""
    End If
  End Function
  Public Function GetDescriptionfromXPath(ByVal XPath As String) As String
    If Not String.IsNullOrEmpty(XPath) Then
      Dim x As XmlElement = myXML.SelectSingleNode(XPath)
      If Not IsNothing(x) Then
        If x.HasAttribute("description") Then
          Return x.Attributes("description").Value
        End If
      End If
    End If
    Return ""
  End Function


  Public Function SurroundJoin(ByVal Arr As String(), ByVal Prefix As String, ByVal Suffix As String, Optional ByVal SkipEmpties As Boolean = False) As String
    Dim out As New StringBuilder
    If Not IsNothing(Arr) Then
      For Each s As String In Arr
        If (SkipEmpties And Not String.IsNullOrEmpty(s)) Or Not SkipEmpties Then
          out.Append(Prefix & s & Suffix)
        End If
      Next
    End If
    Return out.ToString
  End Function

  Public Function GetERPVariables(ByVal Section As String, ByRef TableName As String, Optional ByVal Path As PathStructure = Nothing) As SortedList(Of String, String)
    Dim ERPVariables As New SortedList(Of String, String)
    If IO.File.Exists(My.Settings.ERPSettingsPath) Then
      Using rdr As IO.StreamReader = IO.File.OpenText(My.Settings.ERPSettingsPath)
        Dim strRead As String = ""
        Do Until (strRead = ("[" & Section & "]")) Or rdr.EndOfStream
          '' Do nothing, we're just trying to find the Audit section
          strRead = rdr.ReadLine
          If strRead.StartsWith(Section) And strRead.Contains("=") Then TableName = strRead.Remove(0, strRead.IndexOf("=") + 1)
        Loop
        strRead = ""
        Do Until (strRead.StartsWith("[") And strRead.EndsWith("]")) Or rdr.EndOfStream
          strRead = rdr.ReadLine()
          If strRead.Contains("{") And strRead.Contains("}") Then
            If Not IsNothing(Path) Then
              strRead = ReplaceVariables(strRead, Path.UNCPath) 'Path.ReplaceVariables(strRead)
            End If
          End If
          If strRead.Contains("=") Then
            ERPVariables.Add(strRead.Remove(strRead.IndexOf("=")), strRead.Remove(0, strRead.IndexOf("=") + 1))
          End If
        Loop
      End Using
    End If
    Return ERPVariables
  End Function

  Public Function IsValidERP(ByVal Table As String, ByVal Values As SortedList(Of String, String), Optional ByVal ERPConnection As OleDbConnection = Nothing) As Boolean
    Dim cond As String = ""
    Dim fields As String = ""
    Try
      Dim blnFound As Boolean = False
      Dim Cnn As OleDbConnection
      If IsNothing(ERPConnection) Then
        Cnn = New OleDbConnection(My.Settings.ERPConnection)
        Cnn.Open()
      Else
        Cnn = ERPConnection
      End If

      fields = String.Join(",", Values.Keys)
      If fields.EndsWith(",") Then fields = fields.Remove(fields.Length - 1)
      fields = fields.Replace("{", "").Replace("}", "")
      fields = fields.Replace("||", ",")
      For i = 0 To Values.Count - 1 Step 1 'Each Val As KeyValuePair(Of String, String) In Values
        If Not String.IsNullOrEmpty(Values.Values(i)) Then
          If Values.Keys(i).Contains("||") Then
            Dim orFields As String() = Values.Keys(i).Split("||")
            cond += "(" & SurroundJoin(orFields, "", "=" & Chr(34) & Values.Values(i) & Chr(34) & " OR ", True) & ")"
            If cond.EndsWith(" OR )") Then cond = cond.Remove(cond.LastIndexOf(" OR )")) & ")"
          Else
            cond += Values.Keys(i) & "=" & Chr(34) & Values.Values(0) & Chr(34) & " AND "
          End If
        End If
      Next
      If cond.EndsWith(" AND ") Then cond = cond.Remove(cond.LastIndexOf(" AND "))
      cond = cond.Replace("{", "").Replace("}", "")

      Using Cmd As New OleDbCommand("SELECT " & fields & " FROM " & Table & " WHERE " & cond & ";", Cnn)
        Using Rdr As OleDbDataReader = Cmd.ExecuteReader
          If Rdr.HasRows Then
            blnFound = True '' The fact that the for loop is executing is enough evidence of the existance
          End If
        End Using
      End Using

      If IsNothing(ERPConnection) Then
        Cnn.Close()
      End If

      Return blnFound
    Catch ex As Exception
      Log("Error checking ERP system: " & _
          vbCrLf & vbTab & "Keys: " & String.Join(",", Values.Keys.ToArray) & _
          vbCrLf & vbTab & "Values: " & String.Join(",", Values.Values.ToArray) & _
          vbCrLf & vbTab & "Condition: " & cond & _
          vbCrLf & vbTab & "Error: " & ex.Message)
      Return False
    End Try
  End Function
End Module
