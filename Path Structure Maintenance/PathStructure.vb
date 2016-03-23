Imports System.Xml, System.IO, System.Text
Imports HTML, HTML.HTMLWriter, HTML.HTMLWriter.HTMLTable

Public Class PathStructure
  Private _type As PathType
  Private _path As String
  Private _infoFile As IO.FileInfo
  Private _infoFolder As IO.DirectoryInfo
  Private myXML As New XmlDocument
  Private _defaultPath, _startPath As String
  Private _variables As New SortedList(Of String, String)

  Public ReadOnly Property Parent As PathStructure
    Get
      Dim strTemp As String = _path
      If strTemp.EndsWith("\") Then strTemp = strTemp.Remove(strTemp.Length - 1) '' Fix last index issue
      If strTemp.Contains("\") Then '' Verify the path is still valid
        strTemp = strTemp.Remove(strTemp.LastIndexOf("\"))
        Return New PathStructure(strTemp)
      Else
        Return Nothing
      End If
    End Get
  End Property
  Public ReadOnly Property Children As PathStructure()
    Get
      If _type = PathType.Folder Then
        Dim arr As New List(Of PathStructure)
        For Each obj As String In IO.Directory.EnumerateFileSystemEntries(_path)
          arr.Add(New PathStructure(obj))
        Next
        Return arr.ToArray
      Else
        Return Nothing
      End If
    End Get
  End Property
  Public ReadOnly Property FileInfo As IO.FileInfo
    Get
      If _type = PathType.File Then
        Return _infoFile
      Else
        Return Nothing
      End If
    End Get
  End Property
  Public ReadOnly Property FolderInfo As IO.DirectoryInfo
    Get
      If _type = PathType.Folder Then
        Return _infoFolder
      Else
        Return Nothing
      End If
    End Get
  End Property
  Public ReadOnly Property Variables As SortedList(Of String, String)
    Get
      Return _variables
    End Get
  End Property
  Public ReadOnly Property UNCPath As String
    Get
      Return _path
    End Get
  End Property
  Public ReadOnly Property Type As PathType
    Get
      Return _type
    End Get
  End Property
  Public ReadOnly Property DefaultPath As String
    Get
      Return _defaultPath
    End Get
  End Property
  Public ReadOnly Property StartPath As String
    Get
      Return _startPath
    End Get
  End Property

  Public Enum PathType
    File
    Folder
  End Enum

  Public Sub New(ByVal Path As String)
    '' Set path
    _path = GetUNCPath(Path).ToLower

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

    myXML.Load(My.Settings.SettingsPath)
    _defaultPath = myXML.SelectSingleNode("//Structure").Attributes("defaultPath").Value.ToLower

    '' Enumerate variables
    If Not IsNothing(myXML.SelectNodes("//Variable")) Then
      For Each var As XmlElement In myXML.SelectNodes("//Variable")
        If IsNumeric(var.InnerText) Then
          Dim index As Integer = Convert.ToInt32(var.InnerText)
          If GetNthIndexOf(_path, IO.Path.DirectorySeparatorChar, index + 3) >= 0 Then
            'Debug.WriteLine(var.Attributes("name").Value & " index: " & index.ToString & vbLf & vbTab & _
            '                "Path trim index: " & GetNthIndexOf(_path, IO.Path.DirectorySeparatorChar, index + 3).ToString)
            Dim pre, post As Integer
            pre = GetNthIndexOf(_path, IO.Path.DirectorySeparatorChar, index + 2)
            post = GetNthIndexOf(_path, IO.Path.DirectorySeparatorChar, index + 3)
            _variables.Add(var.Attributes("name").Value, _path.Remove(post).Remove(0, pre + 1))
          End If
        End If
      Next
    End If

    '' Set Start path
    _startPath = _defaultPath & "\" & ReplaceVariables(myXML.SelectSingleNode("//Structure").Attributes("path").Value)
  End Sub

  Public Function IsDescendantOfDefaultPath() As Boolean
    If Not String.IsNullOrEmpty(GetUNCPath(_path)) Then
      If GetUNCPath(_path).StartsWith(_defaultPath) Then
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
        searchXPath = "//File"
      End If
    ElseIf _type = PathType.Folder Then
      If Not String.IsNullOrEmpty(MainStructureName) Then
        searchXPath = "//Folder[@name='" & MainStructureName & "']"
      Else
        searchXPath = "//Folder"
      End If
    End If
    If Not String.IsNullOrEmpty(searchXPath) Then
      For Each obj As XmlElement In myXML.SelectNodes(searchXPath)
        If _type = PathType.File Then
          If obj.SelectNodes("Option").Count > 0 Then
            For Each opt As XmlElement In obj.SelectNodes("Option")
              '' Get theoretical full path of option
              strTemp = ReplaceVariables(GetURIfromXPath(FindXPath(opt)) & opt.InnerText)
              '' Replace path variables with Regex grouping
              strTemp = New RegularExpressions.Regex("{(.*?)}").Replace(strTemp, "(.*?)")
              If Not strTemp.Contains("(.*?)") Then
                If Not String.IsNullOrEmpty(_infoFile.Extension) Then
                  isMatch = (_infoFile.FullName.Replace(_infoFile.Extension, "").ToLower = strTemp.ToLower)
                Else
                  isMatch = (_infoFile.FullName.ToLower = strTemp.ToLower)
                End If
              Else
                strTemp = strTemp.Replace("\", "\\") '' Escapes '\' for Regex
                If Not String.IsNullOrEmpty(_infoFile.Extension) Then
                  isMatch = New RegularExpressions.Regex(strTemp, RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFile.FullName.Replace(_infoFile.Extension, ""))
                Else
                  isMatch = New RegularExpressions.Regex(strTemp, RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFile.FullName)
                End If
              End If
              If isMatch Then
                '' Add Option XPath to candidates
                If Not IsNothing(Candidates) Then Candidates.Add(FindXPath(opt))
                '' Throw flag that at least one option was found
                blnFound = True
                isMatch = False
              End If
            Next
          Else
            '' Get theoretical full path of option
            strTemp = ReplaceVariables(GetURIfromXPath(FindXPath(obj)) & obj.InnerText)
            '' Replace path variables with Regex grouping
            strTemp = New RegularExpressions.Regex("{(.*?)}").Replace(strTemp, "(.*?)")
            If Not strTemp.Contains("(.*?)") Then
              If Not String.IsNullOrEmpty(_infoFile.Extension) Then
                isMatch = (_infoFile.FullName.Replace(_infoFile.Extension, "").ToLower = strTemp.ToLower)
              Else
                isMatch = (_infoFile.FullName.ToLower = strTemp.ToLower)
              End If
            Else
              strTemp = strTemp.Replace("\", "\\") '' Escapes '\' for Regex
              If Not String.IsNullOrEmpty(_infoFile.Extension) Then
                isMatch = New RegularExpressions.Regex(strTemp, RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFile.FullName.Replace(_infoFile.Extension, ""))
              Else
                isMatch = New RegularExpressions.Regex(strTemp, RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFile.FullName)
              End If
            End If
          End If
        ElseIf _type = PathType.Folder Then
          '' Get theoretical full path of option
          strTemp = ReplaceVariables(GetURIfromXPath(FindXPath(obj)))
          '' Replace path variables with Regex grouping
          strTemp = New RegularExpressions.Regex("{(.*?)}").Replace(strTemp, "(.*?)")
          If Not strTemp.Contains("(.*?)") Then
            isMatch = (_infoFolder.FullName.ToLower = strTemp.ToLower)
          Else
            strTemp = strTemp.Replace("\", "\\") '' Escapes '\' for Regex
            isMatch = New RegularExpressions.Regex(strTemp, RegularExpressions.RegexOptions.IgnoreCase).IsMatch(_infoFolder.FullName)
          End If
        End If
        If isMatch Then
          '' Add Option XPath to candidates
          If Not IsNothing(Candidates) Then Candidates.Add(FindXPath(obj))
          '' Throw flag that at least one option was found
          blnFound = True
        End If
      Next
    Else
      Throw New ArgumentException("Couldn't determine path type", "Invalid Path Type")
    End If

    If _path = _startPath Then
      Candidates.Add("//Structure")
      blnFound = True
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
    Dim pattern As String = GetURIfromXPath(FindXPath(Folder)) & "(.*?)"
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
  Public Overloads Function IsLocationValid(ByVal FolderXPath As String) As Boolean
    Dim pattern As String = GetURIfromXPath(FindXPath(myXML.SelectSingleNode(FolderXPath))) & "(.*?)"
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

  Public Function GetURIfromXPath(ByVal XPath As String) As String
    If Not String.IsNullOrEmpty(XPath) Then
      Dim x As XmlElement = myXML.SelectSingleNode(XPath)
      Dim u As String = ""
      Do Until x.Name = "Structure"
        If x.Name = "Folder" Then
          u = x.Attributes("name").Value & "\" & u
        End If
        x = x.ParentNode
      Loop
      u = _startPath & u
      Return u
    Else
      Return ""
    End If
  End Function

  Public Function ReplaceVariables(ByVal Input As String) As String
    If Not IsNothing(_variables) Then
      For Each var As KeyValuePair(Of String, String) In _variables
        If Input.Contains(var.Key) Then Input = Input.Replace(var.Key, var.Value)
      Next
    End If
    Return Input
  End Function

  Public Function GetStructureTypefromXPath(ByVal XPath As String) As String
    Dim x As XmlElement = myXML.SelectSingleNode(XPath)
    Dim out As String = ""
    If Not IsNothing(x) Then
      Do Until x.ParentNode.Name = "Structure"
        If x.Name = "Structure" Then Exit Do
        If x.Name = "Option" Then
          out = x.ParentNode.Attributes("name").Value & "-" & x.Attributes("name").Value & "/" & out
          x = x.ParentNode
        Else
          out = x.Attributes("name").Value & "/" & out
        End If
        x = x.ParentNode
      Loop
      Return "Structure/" & out
    Else
      Return ""
    End If
  End Function

  Private Function XPathListToURIList(ByVal XPathList As List(Of String)) As List(Of String)
    Dim tmp As New List(Of String)
    For Each xstr As String In XPathList
      tmp.Add(GetURIfromXPath(xstr))
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
      Report.Report("'" & Me.UNCPath & "' is Name-Structured with '" & ns.Count.ToString & "' structure candidates:<br /><ul><li>" & String.Join("</li><li>", XPathListToURIList(ns).ToArray()) & "</li></ul>", AuditReport.StatusCode.Optimalstatus)
    Else
      Report.Report("'" & Me.UNCPath & "' is not Name-Structured.", AuditReport.StatusCode.ErrorStatus)
    End If
    found = False
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
        AuditChildren(Report, child, BadList)
      Next
    End If
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
  Private Sub AuditChildren(ByVal Report As AuditReport, ByVal Child As PathStructure, ByRef Bads As List(Of String))
    Dim ns As New List(Of String)
    If Child.IsNameStructured(, ns) Then
      Report.Report("'" & Child.UNCPath & "' is Name-Structured with '" & ns.Count.ToString & "' structure candidates:<br /><ul><li>" & String.Join("</li><li>", XPathListToURIList(ns).ToArray) & "</li></ul>", AuditReport.StatusCode.Optimalstatus)
    Else
      Report.Report("'" & Child.UNCPath & "' is not Name-Structured.", AuditReport.StatusCode.ErrorStatus)
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
        AuditChildren(Report, chld, Bads)
      Next
    End If
  End Sub

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

    Public ReadOnly Property ReportMarkup
      Get
        Return _report.HTMLMarkup
      End Get
    End Property

    Public Sub New(ByVal CurrentPath As PathStructure)
      _report = New HTML.HTMLWriter
      _report.AddBootstrapReference()
      _path = CurrentPath

      _report += New HTMLHeader("Path Structure Audit", HTMLHeader.HeaderSize.H1)
      _report += New HTMLHeader(_path.UNCPath, HTMLHeader.HeaderSize.H3)
    End Sub

    Public Enum StatusCode
      ErrorStatus = 0
      DefaultStatus = 1
      Optimalstatus = 2
    End Enum
    Public Sub Report(ByVal Message As String, Optional ByVal Code As StatusCode = StatusCode.DefaultStatus)
      Select Case Code
        Case StatusCode.ErrorStatus
          _report += New HTMLParagraph(Message, New AttributeList({"class"}, {"alert alert-danger"}))
        Case StatusCode.DefaultStatus
          _report += New HTMLParagraph(Message, New AttributeList({"class"}, {"alert alert-info"}))
        Case StatusCode.Optimalstatus
          _report += New HTMLParagraph(Message, New AttributeList({"class"}, {"alert alert-success"}))
      End Select
    End Sub
    Public Sub Raw(ByVal HTMLMarkup As String)
      _report += HTMLMarkup
    End Sub

  End Class
End Class
