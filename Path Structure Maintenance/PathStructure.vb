Imports System.Xml, System.IO, System.Text
Imports HTML, HTML.HTMLWriter, HTML.HTMLWriter.HTMLTable

Public Class PathStructure
  Private _type As PathType
  Private _path As String
  Private _infoFile As IO.FileInfo
  Private _infoFolder As IO.DirectoryInfo
  Private myXML As XmlDocument
  Private _defaultPath, _startPath As String
  Private _variables As New SortedList(Of String, String)
  Private _parent As PathStructure
  Private _children As PathStructure()

  ''' <summary>
  ''' Gets the PathStructure representation of the current path's parent directory.
  ''' </summary>
  ''' <value></value>
  ''' <returns>PathStructure</returns>
  ''' <remarks></remarks>
  Public ReadOnly Property Parent As PathStructure
    Get
      If IsNothing(_parent) Then
        Dim strTemp As String = _path
        If strTemp.EndsWith("\") Then strTemp = strTemp.Remove(strTemp.Length - 1) '' Fix last index issue
        If strTemp.Contains("\") Then '' Verify the path is still valid
          strTemp = strTemp.Remove(strTemp.LastIndexOf("\"))
          _parent = New PathStructure(strTemp)
        End If
      End If
      Return _parent
    End Get
  End Property
  Public ReadOnly Property ParentPath As String
    Get
      If _type = PathType.File Then
        Return _infoFile.DirectoryName
      Else
        Return _infoFolder.Parent.FullName
      End If
    End Get
  End Property
  ''' <summary>
  ''' Enumerates the PathStructure objects the represent the current path's child filesystem objects.
  ''' </summary>
  ''' <value></value>
  ''' <returns>PathStructure()</returns>
  ''' <remarks></remarks>
  Public ReadOnly Property Children As PathStructure()
    Get
      If _type = PathType.Folder Then
        If IsNothing(_children) Then
          Dim arr As New List(Of PathStructure)
          For Each obj As String In IO.Directory.EnumerateFileSystemEntries(_path)
            arr.Add(New PathStructure(obj))
          Next
          _children = arr.ToArray
        End If
        Return _children
      Else
        Return Nothing
      End If
    End Get
  End Property
  ''' <summary>
  ''' Gets the IO.FileInfo of the current path if the current path is a file filesystem object.
  ''' </summary>
  ''' <value></value>
  ''' <returns>IO.FileInfo</returns>
  ''' <remarks></remarks>
  Public ReadOnly Property FileInfo As IO.FileInfo
    Get
      If _type = PathType.File Then
        Return _infoFile
      Else
        Return Nothing
      End If
    End Get
  End Property
  ''' <summary>
  ''' Gets the IO.DirectoryInfo of the current path if the current path is a folder filesystem object.
  ''' </summary>
  ''' <value></value>
  ''' <returns>IO.DirectoryInfo</returns>
  ''' <remarks></remarks>
  Public ReadOnly Property FolderInfo As IO.DirectoryInfo
    Get
      If _type = PathType.Folder Then
        Return _infoFolder
      Else
        Return Nothing
      End If
    End Get
  End Property
  ''' <summary>
  ''' Gets a list of variables in the current path and their values.
  ''' </summary>
  ''' <value>Key is variable name. Value is the variable value.</value>
  ''' <returns>SortedList(Of String, String)</returns>
  ''' <remarks></remarks>
  Public ReadOnly Property Variables As SortedList(Of String, String)
    Get
      Return _variables
    End Get
  End Property
  ''' <summary>
  ''' Gets the UNC formatted path of the current path
  ''' </summary>
  ''' <value></value>
  ''' <returns>String</returns>
  ''' <remarks></remarks>
  Public ReadOnly Property UNCPath As String
    Get
      Return _path
    End Get
  End Property
  ''' <summary>
  ''' Gets the current path's path type, an enum of PathType (File or Folder)
  ''' </summary>
  ''' <value></value>
  ''' <returns>PathType</returns>
  ''' <remarks></remarks>
  Public ReadOnly Property Type As PathType
    Get
      Return _type
    End Get
  End Property
  ''' <summary>
  ''' Gets the default (or root) directory for the current Structure.
  ''' </summary>
  ''' <value></value>
  ''' <returns>String</returns>
  ''' <remarks></remarks>
  Public ReadOnly Property DefaultPath As String
    Get
      Return _defaultPath
    End Get
  End Property
  ''' <summary>
  ''' Gets the current path with replaced variables.
  ''' </summary>
  ''' <value></value>
  ''' <returns>String</returns>
  ''' <remarks></remarks>
  Public ReadOnly Property StartPath As String
    Get
      Return _startPath
    End Get
  End Property
  Public ReadOnly Property PathName As String
    Get
      If _type = PathType.Folder Then
        Return _infoFolder.Name
      ElseIf _type = PathType.File Then
        Return _infoFile.Name
      Else
        Return ""
      End If
    End Get
  End Property
  Public ReadOnly Property Extension As String
    Get
      If _type = PathType.File Then
        If Not IsNothing(_infoFile.Extension) Then
          Return _infoFile.Extension
        End If
      End If
      Return ""
    End Get
  End Property

  Public Enum PathType
    File
    Folder
  End Enum

  Public Overrides Function ToString() As String
    If _type = PathType.File Then
      Return _infoFile.Name
    ElseIf _type = PathType.Folder Then
      Return _infoFolder.Name
    Else
      Return _path
    End If
  End Function

  Public Sub New(ByVal Path As String)
    '' Set path
    _path = GetUNCPath(Path)

    '' Determine/Set path type
    If IO.File.Exists(_path) Then
      _type = PathType.File
    ElseIf IO.Directory.Exists(_path) Then
      _type = PathType.Folder
      If Not _path.EndsWith("\") Then _path += "\"
    Else
      Throw New ArgumentException("Path type not determinable from '" & _path & "'", "Invalid Path Type")
    End If

    '' Set path information
    If _type = PathType.File Then
      _infoFile = New IO.FileInfo(_path)
    ElseIf _type = PathType.Folder Then
      _infoFolder = New IO.DirectoryInfo(_path)
    Else
      Throw New ArgumentException("Path type not determinable from '" & _path & "'.", "Invalid Path Type")
    End If

    If IsNothing(Main._xml) Then
      myXML = New XmlDocument
      myXML.Load(My.Settings.SettingsPath)
    Else
      myXML = Main._xml
    End If
    ''myXML.Load(My.Settings.SettingsPath)
    _defaultPath = myXML.SelectSingleNode("//Structure").Attributes("defaultPath").Value.ToLower
    Dim defSeparator As Integer = CountStringOccurance(_defaultPath, IO.Path.DirectorySeparatorChar)
    '' Enumerate variables
    If Not IsNothing(myXML.SelectNodes("//Variable")) Then
      Dim vars As XmlNodeList = myXML.SelectNodes("//Variable")
      For i = 0 To vars.Count - 1 Step 1
        If IsNumeric(vars(i).InnerText) Then
          Dim index As Integer = Convert.ToInt32(vars(i).InnerText)
          If GetNthIndexOf(_path, IO.Path.DirectorySeparatorChar, index + defSeparator) >= 0 Then
            Dim pre, post As Integer
            pre = GetNthIndexOf(_path, IO.Path.DirectorySeparatorChar, index + (defSeparator - 1)) '2)
            post = GetNthIndexOf(_path, IO.Path.DirectorySeparatorChar, index + defSeparator)
            _variables.Add(vars(i).Attributes("name").Value, _path.Remove(post).Remove(0, pre + 1))
          End If
        End If
      Next
    End If

    '' Set Start path
    _startPath = _defaultPath & "\" & ReplaceVariables(myXML.SelectSingleNode("//Structure").Attributes("path").Value)
  End Sub

  ''' <summary>
  ''' Determines if the current instance of a PathStructure is a descendant of the DefaultPath (or root directory)
  ''' </summary>
  ''' <returns>Boolean</returns>
  ''' <remarks></remarks>
  Public Function IsDescendantOfDefaultPath() As Boolean
    If Not String.IsNullOrEmpty(GetUNCPath(_path)) Then
      If GetUNCPath(_path).ToLower.StartsWith(_defaultPath) Then
        Return True
      End If
    End If
    Return False
  End Function

  ''' <summary>
  ''' Determines whether the current path is part of the Path Structure format.
  ''' </summary>
  ''' <param name="MainStructureName">Sets the default search location within the Path Structure.</param>
  ''' <param name="Candidates">A reference to a list that will be filled with the XPath to any valid Path Structure nodes.</param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Function IsNameStructured(Optional ByVal MainStructureName As String = "", Optional ByRef Candidates As List(Of String) = Nothing) As Boolean
    Dim strTemp As String
    Dim searchXPath As String
    Dim blnFound As Boolean = False
    Dim isMatch As Boolean
    If _type = PathType.File Then
      If Not String.IsNullOrEmpty(MainStructureName) Then
        searchXPath = "//File[@name='" & MainStructureName & "']"
      Else
        strTemp = ReplaceVariables(ParentPath)
        searchXPath = "//Folder[@name='" & Uri.EscapeDataString(strTemp) & "']/File"
        If myXML.SelectNodes(searchXPath).Count <= 0 Then searchXPath = "//File"
      End If
    ElseIf _type = PathType.Folder Then
      If Not String.IsNullOrEmpty(MainStructureName) Then
        searchXPath = "//Folder[@name='" & MainStructureName & "']"
      Else
        strTemp = ReplaceVariables(PathName)
        searchXPath = "//Folder[@name='" & Uri.EscapeDataString(strTemp) & "']"
        If myXML.SelectNodes(searchXPath).Count <= 0 Then searchXPath = "//Folder"
      End If
    End If
    If Not String.IsNullOrEmpty(searchXPath) Then
      Dim objs As XmlNodeList = myXML.SelectNodes(searchXPath)
      If objs.Count > 0 Then
        For i = 0 To objs.Count - 1 Step 1
          If _type = PathType.File Then
            Dim opts As XmlNodeList = objs(i).SelectNodes("Option")
            If opts.Count > 0 Then
              For j = 0 To opts.Count - 1 Step 1
                '' Get theoretical full path of option
                strTemp = ReplaceVariables(GetURIfromXPath(FindXPath(opts(j))) & opts(j).InnerText)
                '' Replace path variables with Regex grouping
                strTemp = New RegularExpressions.Regex("{(.*?)}").Replace(strTemp, "(.*?)")
                If Not (strTemp.IndexOf("(.*?)") >= 0) Then
                  isMatch = String.Equals(_infoFile.DirectoryName & "\" & _infoFile.Name, strTemp, StringComparison.OrdinalIgnoreCase)
                Else
                  If Not String.IsNullOrEmpty(_infoFile.Extension) Then
                    isMatch = New RegularExpressions.Regex(strTemp.Replace("\", "\\"), RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFile.FullName.Replace(_infoFile.Extension, ""))
                  Else
                    isMatch = New RegularExpressions.Regex(strTemp.Replace("\", "\\"), RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFile.FullName)
                  End If
                End If
                If isMatch Then
                  '' Add Option XPath to candidates
                  If Not IsNothing(Candidates) Then Candidates.Add(FindXPath(opts(j)))
                  '' Throw flag that at least one option was found
                  blnFound = True
                  isMatch = False
                End If
              Next
            Else
              '' Get theoretical full path of option
              strTemp = ReplaceVariables(GetURIfromXPath(FindXPath(objs(i))) & objs(i).InnerText)
              '' Replace path variables with Regex grouping
              strTemp = New RegularExpressions.Regex("{(.*?)}").Replace(strTemp, "(.*?)")
              If Not (strTemp.IndexOf("(.*?)") >= 0) Then
                If Not String.IsNullOrEmpty(Me.Extension) Then
                  isMatch = String.Equals(_infoFile.FullName.Replace(_infoFile.Extension, String.Empty), strTemp, StringComparison.OrdinalIgnoreCase)
                Else
                  isMatch = String.Equals(_infoFile.FullName, strTemp, StringComparison.OrdinalIgnoreCase)
                End If
              Else
                If Not String.IsNullOrEmpty(Me.Extension) Then
                  isMatch = New RegularExpressions.Regex(strTemp.Replace("\", "\\"), RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFile.FullName.Replace(_infoFile.Extension, ""))
                Else
                  isMatch = New RegularExpressions.Regex(strTemp.Replace("\", "\\"), RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFile.FullName)
                End If
              End If
            End If
          ElseIf _type = PathType.Folder Then
            '' Get theoretical full path of option
            strTemp = ReplaceVariables(GetURIfromXPath(FindXPath(objs(i))))
            '' Replace path variables with Regex grouping
            strTemp = New RegularExpressions.Regex("{(.*?)}").Replace(strTemp, "(.*?)")
            If Not (strTemp.IndexOf("(.*?)") >= 0) Then
              isMatch = String.Equals(_infoFolder.FullName, strTemp, StringComparison.OrdinalIgnoreCase)
            Else
              isMatch = New RegularExpressions.Regex(strTemp.Replace("\", "\\"), RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFolder.FullName)
            End If
          End If
          If isMatch Then
            '' Add Option XPath to candidates
            If Not IsNothing(Candidates) Then Candidates.Add(FindXPath(objs(i)))
            '' Throw flag that at least one option was found
            blnFound = True
          End If
        Next
      End If
    Else
      Throw New ArgumentException("Couldn't determine path type", "Invalid Path Type")
    End If

    If String.Equals(_path, _startPath, StringComparison.OrdinalIgnoreCase) Then '_path = _startPath Then
      Candidates.Add("//Structure")
      blnFound = True
    End If

    '' Check if more than one Candidate, remove wildcards if at least one does not end in wildcard
    If Candidates.Count > 1 Then
      Dim blnNoWild As Boolean = False
      Dim lstWild As New List(Of Integer)
      For i = 0 To Candidates.Count - 1 Step 1
        strTemp = GetURIfromXPath(Candidates(i))
        If strTemp.IndexOf("}") >= strTemp.Length - 2 Then 'EndsWith("}") Or strTemp.EndsWith("}\") Then
          lstWild.Add(i)
        Else
          blnNoWild = True
        End If
      Next
      '' Remove wildcard(s)
      If blnNoWild And (lstWild.Count > 0) Then
        'For Each wld As Integer In lstWild
        '  Candidates.RemoveAt(wld)
        'Next
        For i = 0 To lstWild.Count - 1 Step 1
          Candidates.RemoveAt(lstWild(i))
        Next
      End If
    End If

    Return blnFound
  End Function
  ''' <summary>
  ''' Determines if the current path is valid according to the provided Path Structure node.
  ''' </summary>
  ''' <param name="Folder">The Path Structure node to check against the current path.</param>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public Overloads Function IsLocationValid(ByVal Folder As XmlElement) As Boolean
    Return IsLocationValid(New Uri(GetURIfromXPath(FindXPath(Folder))))
    'Dim pattern As String = GetURIfromXPath(FindXPath(Folder)) & "(.*?)"
    'If pattern.IndexOf("\") >= 0 Then pattern = pattern.Replace("\", "\\") 'Contains("\") Then pattern = pattern.Replace("\", "\\")
    'If pattern.Contains("{") And pattern.Contains("}") Then pattern = New RegularExpressions.Regex("{(.*?)}").Replace(pattern, "(.*?)")
    'If _type = PathType.File Then
    '  Return New RegularExpressions.Regex(pattern, RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFile.DirectoryName & "\")
    'ElseIf _type = PathType.Folder Then
    '  Return New RegularExpressions.Regex(pattern, RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFolder.FullName)
    'Else
    '  Throw New ArgumentException("Couldn't determine path type", "Invalid Path Type")
    'End If
  End Function
  Public Overloads Function IsLocationValid(ByVal FolderXPath As String) As Boolean
    Return IsLocationValid(myXML.SelectSingleNode(FolderXPath))
    'Dim pattern As String = GetURIfromXPath(FindXPath(myXML.SelectSingleNode(FolderXPath))) & "(.*?)"
    'If pattern.Contains("\") Then pattern = pattern.Replace("\", "\\")
    'If pattern.Contains("{") And pattern.Contains("}") Then pattern = New RegularExpressions.Regex("{(.*?)}").Replace(pattern, "(.*?)")
    'If _type = PathType.File Then
    '  Return New RegularExpressions.Regex(pattern, RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFile.DirectoryName & "\")
    'ElseIf _type = PathType.Folder Then
    '  Return New RegularExpressions.Regex(pattern, RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFolder.FullName)
    'Else
    '  Throw New ArgumentException("Couldn't determine path type", "Invalid Path Type")
    'End If
  End Function
  Public Overloads Function IsLocationValid(ByVal FolderPath As Uri) As Boolean
    Dim pattern As String = FolderPath.AbsolutePath & "(.*?)"
    If pattern.Contains("\") Then pattern = pattern.Replace("\", "\\")
    If pattern.Contains("{") And pattern.Contains("}") Then pattern = New RegularExpressions.Regex("{(.*?)}").Replace(pattern, "(.*?)")
    If _type = PathType.File Then
      Return New RegularExpressions.Regex(pattern, RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFile.DirectoryName & "\")
    ElseIf _type = PathType.Folder Then
      Return New RegularExpressions.Regex(pattern, RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFolder.FullName)
    Else
      Throw New ArgumentException("Couldn't determine path type", "Invalid Path Type")
    End If
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
        Dim u As New StringBuilder
        Do Until x.Name = "Structure"
          If String.Equals(x.Name, "Folder", StringComparison.OrdinalIgnoreCase) Then
            'u = x.Attributes("name").Value & "\" & u
            u.Insert(0, x.Attributes("name").Value & "\")
          End If
          x = x.ParentNode
        Loop
        'u = _startPath & u
        u.Insert(0, _startPath)
        Dim a As XmlAttribute = myXML.CreateAttribute("tmpURI")
        a.Value = u.ToString
        x.Attributes.Append(a)
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

  ''' <summary>
  ''' Replaces PathStructure variables with provided values.
  ''' </summary>
  ''' <param name="Input">Full or partial path string</param>
  ''' <returns>String</returns>
  ''' <remarks></remarks>
  Public Function ReplaceVariables(ByVal Input As String) As String
    If Not IsNothing(_variables) Then
      For i = 0 To _variables.Count - 1 Step 1
        If Input.IndexOf(_variables.Keys(i)) >= 0 Then Input = Input.Replace(_variables.Keys(i), _variables.Values(i))
      Next

      'For Each var As KeyValuePair(Of String, String) In _variables
      '  If Input.Contains(var.Key) Then Input = Input.Replace(var.Key, var.Value)
      'Next
    End If
    Return Input
  End Function

  ''' <summary>
  ''' Gets a full XPath from a absolute or relative XPath.
  ''' </summary>
  ''' <param name="XPath">Absolute or relative XPath</param>
  ''' <returns>String</returns>
  ''' <remarks></remarks>
  Public Function GetStructureTypefromXPath(ByVal XPath As String) As String
    Dim x As XmlElement = myXML.SelectSingleNode(XPath)
    Dim out As New StringBuilder
    If Not IsNothing(x) Then
      If x.HasAttribute("tmpStruct") Then
        Return x.Attributes("tmpStruct").Value
      Else
        Do Until x.ParentNode.Name = "Structure"
          If x.Name = "Structure" Then Exit Do
          If x.Name = "Option" Then
            'out = x.ParentNode.Attributes("name").Value & "-" & x.Attributes("name").Value & "/" & out
            out.Insert(0, x.ParentNode.Attributes("name").Value & "-" & x.Attributes("name").Value & "/")
            x = x.ParentNode
          Else
            'out = x.Attributes("name").Value & "/" & out
            out.Insert(0, x.Attributes("name").Value & "/")
          End If
          x = x.ParentNode
        Loop
        Dim a As XmlAttribute = myXML.CreateAttribute("tmpStruct")
        a.Value = "Structure/" & out.ToString
        x.Attributes.Append(a)
        Return a.Value
      End If
    Else
      Return ""
    End If
  End Function

  Private Function XPathListToURIList(ByVal XPathList As List(Of String)) As List(Of String)
    Dim tmp As New List(Of String)
    For i = 0 To XPathList.Count - 1 Step 1
      tmp.Add(GetURIfromXPath(XPathList(i)))
    Next
    Return tmp
  End Function
  Public Function Audit(ByVal Report As AuditReport, Optional ByRef BadList As List(Of String) = Nothing) As Boolean
    If IsNothing(BadList) Then
      BadList = New List(Of String)
    End If
    Dim ns As New List(Of String)
    Dim found As Boolean = False '' Determines whether any valid locations were found, assume false
    Dim successfulN As String = ""
    Dim struct As String = ""
    If Me.IsNameStructured(, ns) Then
      Report.Report("'" & Me.UNCPath & "' is Name-Structured with '" & ns.Count.ToString & "' structure candidates:<br /><ul><li>" & String.Join("</li><li>", XPathListToURIList(ns).ToArray()) & "</li></ul>", AuditReport.StatusCode.Optimalstatus, Me.UNCPath)
    Else
      Report.Report("'" & Me.UNCPath & "' is not Name-Structured.", AuditReport.StatusCode.ErrorStatus, Me.UNCPath)
    End If
    found = False
    '' Valid XPath's
    For Each n As String In ns.ToArray
      struct = GetURIfromXPath(n)
      If n.Contains("Option") Then n = n.Remove(n.LastIndexOf("/"))
      If Me.IsLocationValid(n) Then
        successfulN = struct
        '' Is a valid location
        found = True
      End If
    Next
    If Not IsNothing(Me.Children) Then
      For Each child As PathStructure In Me.Children
        '' Check if user wants Thumbs.Db deleted
        If My.Settings.blnDeleteThumbsDb And child.UNCPath.ToLower.EndsWith("thumbs.db") Then
          IO.File.Delete(child.UNCPath)
          Report.Report("Deleted Thumbs.Db from '" & child.UNCPath & "'.")
        ElseIf child.UNCPath.ToLower.EndsWith("thumbs.db") Then
          Report.Report("Found Thumbs.Db, but was unable to remove due to settings")
        End If
        AuditChildren(Report, child, BadList)
        Report.FileCount += 1
      Next
    End If
    Report.Report("'" & UNCPath & "' has '" & Report.FileCount.ToString & "' descendant objects")
    If Not found And Not Me.UNCPath.ToLower = Me.StartPath.ToLower Then
      Report.Report("'" & Me.UNCPath & "' does not adhere to a valid location relative to the '" & ns.Count.ToString & "' Name-Structure results found.", AuditReport.StatusCode.ErrorStatus)
      BadList.Add(Me.UNCPath)
    Else
      Report.Report("'" & Me.UNCPath & "' is within the valid location for Path Structure '" & struct & "'", AuditReport.StatusCode.Optimalstatus)
    End If
    If BadList.Count > 0 Then
      Return False
    Else
      Return True
    End If
  End Function
  Private Function AuditChildren(ByRef Report As AuditReport, ByVal Child As PathStructure, ByRef Bads As List(Of String))
    Dim ns As New List(Of String)
    If Child.IsNameStructured(, ns) Then
      Report.Report("'" & Child.UNCPath & "' is Name-Structured with '" & ns.Count.ToString & "' structure candidates:<br /><ul><li>" & String.Join("</li><li>", XPathListToURIList(ns).ToArray) & "</li></ul>", AuditReport.StatusCode.Optimalstatus, Child.UNCPath)
    Else
      Report.Report("'" & Child.UNCPath & "' is not Name-Structured.", AuditReport.StatusCode.ErrorStatus, Child.UNCPath)
    End If
    Dim found As Boolean = False '' Determines whether any valid locations were found, assume false
    Dim successfulN As String = ""
    Dim struct As String
    For Each n As String In ns.ToArray
      struct = GetURIfromXPath(n)
      If n.Contains("Option") Then n = n.Remove(n.LastIndexOf("/"))
      If Child.IsLocationValid(n) Then
        successfulN = struct
        '' Is a valid location
        found = True
      End If
    Next
    If Not found Then
      Bads.Add(Child.UNCPath)
      Report.Report("'" & Child.UNCPath & "' does not adhere to a valid location relative to the '" & ns.Count.ToString & "' Name-Structure results found.", AuditReport.StatusCode.ErrorStatus)
    Else
      Report.Report("'" & Child.UNCPath & "' is within the valid location for Path Structure '" & struct & "'", AuditReport.StatusCode.Optimalstatus)
    End If
    '' Check other children
    If Not IsNothing(Child.Children) Then
      For Each chld As PathStructure In Child.Children
        '' Check if user wants Thumbs.Db deleted
        If My.Settings.blnDeleteThumbsDb And chld.UNCPath.ToLower.EndsWith("thumbs.db") Then
          IO.File.Delete(chld.UNCPath)
          Report.Report("Deleted Thumbs.Db from '" & chld.UNCPath & "'.")
          Continue For
        ElseIf chld.UNCPath.ToLower.EndsWith("thumbs.db") Then
          Report.Report("Found Thumbs.Db, but was unable to remove due to settings")
        End If
        AuditChildren(Report, chld, Bads)
        Report.FileCount += 1
      Next
    End If
  End Function

  Public Function AuditVisual(Optional ByVal Report As AuditVisualReport = Nothing) As AuditVisualReport
    If IsNothing(Report) Then Report = New AuditVisualReport
    Dim ns As New List(Of String)
    Dim found As Boolean = False '' Determines whether any valid locations were found, assume false
    Dim successfulN As String = ""
    Dim struct As String = ""
    Me.IsNameStructured(, ns)

    found = False
    '' Valid XPath's
    For i = 0 To ns.Count - 1 Step 1
      struct = GetURIfromXPath(ns(i))
      If ns(i).IndexOf("Option") >= 0 Then ns(i) = ns(i).Remove(ns(i).LastIndexOf("/"))
      If Me.IsLocationValid(ns(i)) Then
        successfulN = GetDescriptionfromXPath(ns(i))
        '' Is a valid location
        found = True
      End If
    Next
    If ns.Count = 1 Then
      found = True
      struct = GetURIfromXPath(ns(0))
      successfulN = GetDescriptionfromXPath(ns(0))
    End If

    Dim li As HTMLList.ListItem
    Dim ul As HTMLList

    '' Check status of the current path
    If Not found And Not String.Equals(Me.UNCPath, Me.StartPath, StringComparison.OrdinalIgnoreCase) Then 'Me.UNCPath.ToLower = Me.StartPath.ToLower Then
      li = Report.Report("'" & Me.UNCPath & "' does not adhere to any of the '" & ns.Count.ToString & "' valid locations found:" & String.Join("\n * ", ns.ToArray),
                    AuditVisualReport.StatusCode.InvalidPath,
                    Me)
    Else
      li = Report.Report("'" & Me.UNCPath & "' adheres to the Path Structure '" & struct & "': " & successfulN,
                    AuditVisualReport.StatusCode.ValidPath,
                    Me)
    End If

    '' Check status of children paths
    If Not IsNothing(Me.Children) Then
      ul = Report.CreateNewList(Me)
      For i = 0 To Me.Children.Count - 1 Step 1
        Dim cli As HTMLList.ListItem
        Report.FileCount += 1
        '' Check if user wants Thumbs.Db deleted
        If My.Settings.blnDeleteThumbsDb Then
          If Me.Children(i).Type = PathType.File And String.Equals(Me.Children(i).PathName, "thumbs", StringComparison.OrdinalIgnoreCase) And String.Equals(Me.Children(i).Extension, ".db", StringComparison.OrdinalIgnoreCase) Then
            IO.File.Delete(Me.Children(i).UNCPath)
            cli = Report.Report("Deleted Thumbs.Db from '" & Me.Children(i).UNCPath & "'.",
                          AuditVisualReport.StatusCode.Other,
                          Me.Children(i))
            Report.AddListItemToList(ul, cli)
            Continue For
          End If
        End If
        AuditVisualChildren(Report, ul, Me.Children(i))
      Next
      Report.AddListToListItem(li, ul)
    End If

    '' Add item to main list
    Report.AddListItemToList(Nothing, li)
    Return Report
  End Function
  Public Function AuditVisualChildren(ByRef Report As AuditVisualReport, ByRef ParentList As HTMLList, ByVal Child As PathStructure)
    Dim ns As New List(Of String)
    Dim found As Boolean = False '' Determines whether any valid locations were found, assume false
    Dim successfulN As String = ""
    Dim struct As String = ""
    Child.IsNameStructured(, ns)

    found = False
    '' Valid XPath's
    For i = 0 To ns.Count - 1 Step 1
      struct = GetURIfromXPath(ns(i))
      If ns(i).Contains("Option") Then ns(i) = ns(i).Remove(ns(i).LastIndexOf("/"))
      If Me.IsLocationValid(ns(i)) Then
        successfulN = GetDescriptionfromXPath(ns(i))
        '' Is a valid location
        found = True
      End If
    Next
    If ns.Count = 1 Then
      found = True
      struct = GetURIfromXPath(ns(0))
      successfulN = GetDescriptionfromXPath(ns(0))
    End If

    Dim li As HTMLList.ListItem
    Dim ul As HTMLList
    If Not found And Not String.Equals(Me.UNCPath, Me.StartPath, StringComparison.OrdinalIgnoreCase) Then
      li = Report.Report("'" & Child.UNCPath & "' does not adhere to any of the '" & ns.Count.ToString & "' valid locations found:" & String.Join(", ", ns.ToArray),
                    AuditVisualReport.StatusCode.InvalidPath,
                    Child)
    Else
      li = Report.Report("'" & Child.UNCPath & "' adheres to the Path Structure '" & struct & "': " & successfulN,
                    AuditVisualReport.StatusCode.ValidPath,
                    Child)
    End If
    If Not IsNothing(Child.Children) Then
      ul = Report.CreateNewList(Child)
      For i = 0 To Child.Children.Length - 1 Step 1
        Dim cli As HTMLList.ListItem
        '' Check if user wants Thumbs.Db deleted
        Report.FileCount += 1
        If My.Settings.blnDeleteThumbsDb Then
          If Child.Type = PathType.File And String.Equals(Child.PathName, "thumbs", StringComparison.OrdinalIgnoreCase) And String.Equals(Child.Extension, ".db", StringComparison.OrdinalIgnoreCase) Then
            IO.File.Delete(Child.Children(i).UNCPath)
            cli = Report.Report("Deleted Thumbs.Db from '" & Child.Children(i).UNCPath & "'.",
                          AuditVisualReport.StatusCode.Other,
                          Child.Children(i))
            Report.AddListItemToList(ul, cli)
            Continue For
          End If
        End If
        AuditVisualChildren(Report, ul, Child.Children(i))
      Next
      Report.AddListToListItem(li, ul)
    End If

    '' Add item to main list
    Report.AddListItemToList(ParentList, li)
    Return Report
  End Function

  Public Sub LogData(ByVal ChangedPath As String, ByVal Method As String)
    Try
      IO.File.AppendAllText(_defaultPath & "\PathStructure Changes.csv",
                            DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") & "," & _path & "," & ChangedPath & "," & Method & "," & My.User.Name & vbCrLf)
    Catch ex As Exception
      Log("Error while appending change log:" & vbCrLf & vbTab & ex.Message)
    End Try
  End Sub

  Public Class AuditReport
    Private _report As HTML.HTMLWriter
    Private _path As PathStructure
    Private _fileCount As Integer
    Private _showErrors, _showOptimal, _showInformation As Boolean
    Private _errPaths, _optPaths As List(Of String)

    ''' <summary>
    ''' Gets the HTML markup for the report.
    ''' </summary>
    ''' <value></value>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ReportMarkup As String
      Get
        Return _report.HTMLMarkup
      End Get
    End Property
    ''' <summary>
    ''' Gets/Sets the number of files audited in this report.
    ''' </summary>
    ''' <value></value>
    ''' <returns>Integer</returns>
    ''' <remarks></remarks>
    Public Property FileCount As Integer
      Get
        Return _fileCount
      End Get
      Set(value As Integer)
        _fileCount += 1
      End Set
    End Property
    ''' <summary>
    ''' Gets/Sets whether Error messages should be reported or not
    ''' </summary>
    ''' <value></value>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Property ReportErrorMessages As Boolean
      Get
        Return _showErrors
      End Get
      Set(value As Boolean)
        _showErrors = value
      End Set
    End Property
    ''' <summary>
    ''' Gets/Sets whether Optimal messages should be reported or not
    ''' </summary>
    ''' <value></value>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Property ReportOptimalMessages As Boolean
      Get
        Return _showOptimal
      End Get
      Set(value As Boolean)
        _showOptimal = value
      End Set
    End Property
    ''' <summary>
    ''' Gets/Sets whether Information messages should be reported or not
    ''' </summary>
    ''' <value></value>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Property ReportInformationMessages As Boolean
      Get
        Return _showInformation
      End Get
      Set(value As Boolean)
        _showInformation = value
      End Set
    End Property

    ''' <summary>
    ''' Gets a list of error paths
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ErrorPaths As List(Of String)
      Get
        Return _errPaths
      End Get
    End Property
    ''' <summary>
    ''' Gets a list of optimal paths
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property OptimalPaths As List(Of String)
      Get
        Return _optPaths
      End Get
    End Property

    Public Sub New(ByVal CurrentPath As PathStructure)
      _report = New HTML.HTMLWriter
      _report.AddBootstrapReference()
      _path = CurrentPath

      _report += New HTMLHeader("Path Structure Audit", HTMLHeader.HeaderSize.H1)
      _report += New HTMLHeader(_path.UNCPath, HTMLHeader.HeaderSize.H3)

      _errPaths = New List(Of String)
      _optPaths = New List(Of String)

      _showErrors = My.Settings.blnReportErrors
      _showOptimal = My.Settings.blnReportOptimal
      _showInformation = My.Settings.blnReportInformation
    End Sub

    Public Enum StatusCode
      ErrorStatus = 0
      DefaultStatus = 1
      Optimalstatus = 2
    End Enum
    ''' <summary>
    ''' Appends to the current instance of an AuditReport
    ''' </summary>
    ''' <param name="Message">Text to be displayed in report.</param>
    ''' <param name="Code">(Optional) Status code for the message.</param>
    ''' <param name="Path">(Optional) Provides the path to be added to the internal list of error or optimal path(s).</param>
    ''' <remarks></remarks>
    Public Sub Report(ByVal Message As String, Optional ByVal Code As StatusCode = StatusCode.DefaultStatus, Optional ByVal Path As String = "")
      Select Case Code
        Case StatusCode.ErrorStatus
          If _showErrors Then
            _report += New HTMLParagraph(Message, New AttributeList({"class"}, {"alert alert-danger"}))
            If Not String.IsNullOrEmpty(Path) Then _errPaths.Add(Path)
          End If
        Case StatusCode.DefaultStatus
          If _showInformation Then
            _report += New HTMLParagraph(Message, New AttributeList({"class"}, {"alert alert-info"}))
          End If
        Case StatusCode.Optimalstatus
          If _showOptimal Then
            _report += New HTMLParagraph(Message, New AttributeList({"class"}, {"alert alert-success"}))
            If Not String.IsNullOrEmpty(Path) Then _optPaths.Add(Path)
          End If
      End Select
    End Sub
    ''' <summary>
    ''' Appends to the current instance of an AuditReport
    ''' </summary>
    ''' <param name="HTMLMarkup">Raw HTML markup string</param>
    ''' <remarks></remarks>
    Public Sub Raw(ByVal HTMLMarkup As String)
      _report += HTMLMarkup
    End Sub

  End Class
  Public Class AuditVisualReport
    Private _report As HTML.HTMLWriter
    Private _fileCount As Integer
    Private _errPaths, _optPaths As List(Of String)
    Private fileSystem As HTML.HTMLWriter.HTMLList

    ''' <summary>
    ''' Gets the HTML markup for the report.
    ''' </summary>
    ''' <value></value>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Public ReadOnly Property ReportMarkup As String
      Get
        _report.AddToHTMLMarkup(My.Resources.FileSystemVisualAuditTemplate.Replace("{PLACEHOLDER:FILESYSTEM}", fileSystem.Markup))
        Return _report.HTMLMarkup
      End Get
    End Property
    ''' <summary>
    ''' Gets/Sets the number of files audited in this report.
    ''' </summary>
    ''' <value></value>
    ''' <returns>Integer</returns>
    ''' <remarks></remarks>
    Public Property FileCount As Integer
      Get
        Return _fileCount
      End Get
      Set(value As Integer)
        _fileCount += 1
      End Set
    End Property

    Public Sub New()
      _report = New HTML.HTMLWriter
      fileSystem = New HTMLList(HTMLList.ListType.Unordered)
    End Sub

    Public Enum StatusCode
      ValidPath = 0
      InvalidPath = 1
      Other = 2
    End Enum

    ''' <summary>
    ''' Appends to the current instance of an AuditReport
    ''' </summary>
    ''' <param name="Message">Text to be displayed in report.</param>
    ''' <param name="Code">(Optional) Status code for the message.</param>
    ''' <param name="Path">(Optional) Provides the path to be added to the internal list of error or optimal path(s).</param>
    ''' <remarks></remarks>
    Public Function Report(ByVal Message As String, ByVal Code As StatusCode, ByVal Path As PathStructure) As HTMLList.ListItem
      Dim spn As HTMLSpan
      If Path.Type = PathType.Folder Then
        spn = New HTMLSpan("", New AttributeList({"class"}, {"folder"}))
      ElseIf Path.Type = PathType.File Then
        spn = New HTMLSpan("", New AttributeList({"class"}, {"file"}))
      Else
        Return Nothing
      End If

      Dim a As New HTMLAnchor(Path.PathName, , , , , New AttributeList({"data-message", "data-uncpath"}, {Message, Path.UNCPath}))

      Dim li As HTMLList.ListItem
      If Code = StatusCode.ValidPath Then
        li = New HTMLList.ListItem(spn.Markup & a.Markup, New AttributeList({"class"}, {"valid"}))
      ElseIf Code = StatusCode.InvalidPath Then
        li = New HTMLList.ListItem(spn.Markup & a.Markup, New AttributeList({"class"}, {"invalid"}))
      Else
        li = New HTMLList.ListItem(spn.Markup & a.Markup)
      End If

      Return li
    End Function

    Public Function CreateNewList(ByVal path As PathStructure) As HTMLList
      Return New HTMLList(HTMLList.ListType.Unordered, New AttributeList({"style"}, {"display: none;"}))
    End Function
    ''' <summary>
    ''' Adds the raw markup of a list to the innerHTML of a list item.
    ''' </summary>
    ''' <param name="LI"></param>
    ''' <param name="UL"></param>
    ''' <remarks></remarks>
    Public Sub AddListToListItem(ByRef LI As HTMLList.ListItem, ByVal UL As HTMLList)
      LI.AddInnerHTML(UL.Markup)
    End Sub
    ''' <summary>
    ''' Adds the list item to the provided list. If not list is provided, the main list will be used.
    ''' </summary>
    ''' <param name="UL"></param>
    ''' <param name="LI"></param>
    ''' <remarks></remarks>
    Public Sub AddListItemToList(ByRef UL As HTMLList, ByVal LI As HTMLList.ListItem)
      If IsNothing(UL) Then
        UL = fileSystem
      End If
      UL.AddListItem(LI)
    End Sub

    ''' <summary>
    ''' Appends to the current instance of an AuditReport
    ''' </summary>
    ''' <param name="HTMLMarkup">Raw HTML markup string</param>
    ''' <remarks></remarks>
    Public Sub Raw(ByVal HTMLMarkup As String)
      _report += HTMLMarkup
    End Sub

  End Class
End Class
