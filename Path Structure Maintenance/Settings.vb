﻿Imports Microsoft.Win32
Imports System.Xml
Imports System.Security, System.Security.Principal, System.Security.AccessControl

Public Class Settings
  Private Const regFoldMain As String = "AllFilesystemObjects\\shell\\PathStructure\\"
  Private Const regCommand As String = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\CommandStore\\shell\\"

  Private Sub btnAddContextMenu_Click(sender As Object, e As EventArgs) Handles btnAddContextMenu.Click
    Dim regmenu As RegistryKey
    Dim regcmd As RegistryKey
    Dim myXML As New XmlDocument
    If IO.File.Exists(My.Settings.SettingsPath) Then
      myXML.Load(My.Settings.SettingsPath)

      Try
        Log("Creating registry")
        ''////////////// Add Folders \\\\\\\\\\\\\\\\\\\
        Dim view32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
        '' Add main context menu item 'Path Structure'
        regmenu = Registry.ClassesRoot.CreateSubKey(regFoldMain)
        '' Add main values
        regmenu.SetValue("MUIVerb", "Path Structure")
        regmenu.SetValue("icon", "%windir%\system32\imageres.dll,153")
        regmenu.SetValue("SubCommands", "PathStructure.Open;" & IIf(My.Settings.blnAddAll Or My.Settings.blnAddSingle, "PathStructure.Add;", "") & IIf(My.Settings.blnFormat, "PathStructure.Format;", "") & IIf(My.Settings.blnAudit, "PathStructure.Audit;", "") & IIf(My.Settings.blnClipboard, "PathStructure.Clipboard;", "") & IIf(My.Settings.blnTransferByExtension, "PathStructure.TransferByExtension;", ""))

        ''Create 'Add' submenu
        If My.Settings.blnAddAll Or My.Settings.blnAddSingle Then
          regmenu = view32.CreateSubKey(regCommand & "PathStructure.Add")
          regmenu.SetValue("MUIVerb", "Add")
          regmenu.SetValue("SubCommands", "PathStructure.Add.All;PathStructure.Add.Single;")
        End If

        If My.Settings.blnAddAll Then
          Log(vbTab & "Adding 'Add All'")
          regmenu = view32.CreateSubKey(regCommand & "PathStructure.Add.All")
          regmenu.SetValue("MUIVerb", "Add All main folders")
          regmenu.SetValue("icon", "%windir%\system32\shell32.dll,278")
          regcmd = view32.CreateSubKey(regCommand & "PathStructure.Add.All\\command")
          regcmd.SetValue("", Chr(34) & Application.ExecutablePath & Chr(34) & " -addall " & Chr(34) & "%1" & Chr(34))
        End If

        If My.Settings.blnAddSingle Then
          Log(vbTab & "Adding 'Add Single'")
          regmenu = view32.CreateSubKey(regCommand & "PathStructure.Add.Single")
          regmenu.SetValue("MUIVerb", "Add a Folder...")
          regmenu.SetValue("icon", "%windir%\system32\shell32.dll,278")
          regcmd = view32.CreateSubKey(regCommand & "PathStructure.Add.Single\\command")
          regcmd.SetValue("", Chr(34) & Application.ExecutablePath & Chr(34) & " -add " & Chr(34) & "%1" & Chr(34))
        End If

        If My.Settings.blnFormat Then
          Log(vbTab & "Adding 'Format' commands")
          regmenu = view32.CreateSubKey(regCommand & "PathStructure.Format")
          regmenu.SetValue("MUIVerb", "Format selected file...")
          regmenu.SetValue("icon", "%windir%\system32\comres.dll,6")
          regcmd = view32.CreateSubKey(regCommand & "PathStructure.Format\\command")
          regcmd.SetValue("", Chr(34) & Application.ExecutablePath & Chr(34) & " -format " & Chr(34) & "%1" & Chr(34))
        End If

        If My.Settings.blnAudit Then
          Log(vbTab & "Adding 'Audit' commands")
          regmenu = view32.CreateSubKey(regCommand & "PathStructure.Audit")
          regmenu.SetValue("MUIVerb", "Audit selected object...")
          regmenu.SetValue("icon", "%windir%\system32\comres.dll,6")
          regcmd = view32.CreateSubKey(regCommand & "PathStructure.Audit\\command")
          regcmd.SetValue("", Chr(34) & Application.ExecutablePath & Chr(34) & " -audit " & Chr(34) & "%1" & Chr(34))
        End If

        Log(vbTab & "Adding 'Open' commands")
        regmenu = view32.CreateSubKey(regCommand & "PathStructure.Open")
        regmenu.SetValue("MUIVerb", "Open PathStructure Application")
        regmenu.SetValue("icon", "%windir%\system32\shell32.dll,2")
        regcmd = view32.CreateSubKey(regCommand & "PathStructure.Open\\command")
        regcmd.SetValue("", Chr(34) & Application.ExecutablePath & Chr(34))

        If My.Settings.blnClipboard Then
          Log(vbTab & "Adding 'Clipboard' commands")
          regmenu = view32.CreateSubKey(regCommand & "PathStructure.Clipboard")
          regmenu.SetValue("MUIVerb", "Create Path to Clipboard")
          regmenu.SetValue("icon", "%windir%\system32\ieframe.dll,110")
          regcmd = view32.CreateSubKey(regCommand & "PathStructure.Clipboard\\command")
          regcmd.SetValue("", Chr(34) & Application.ExecutablePath & Chr(34) & " -clipboard " & Chr(34) & "%1" & Chr(34))
        End If

        If My.Settings.blnTransferByExtension Then
          Log(vbTab & "Adding 'TransferByFileExtension' commands")
          regmenu = view32.CreateSubKey(regCommand & "PathStructure.TransferByExtension")
          regmenu.SetValue("MUIVerb", "Transfer Files by Extension")
          regmenu.SetValue("icon", "%windir%\system32\wpdshext.dll,4")
          regcmd = view32.CreateSubKey(regCommand & "PathStructure.TransferByExtension\\command")
          regcmd.SetValue("", Chr(34) & Application.ExecutablePath & Chr(34) & " -transfer " & Chr(34) & "%1" & Chr(34))
        End If

      Catch ex As Exception
        Log("An error occurred..." & vbLf & vbTab & ex.Message)
      End Try
    Else
      Log("Couldn't find Settings File: '" & My.Settings.SettingsPath & "'")
    End If
    MsgBox("Complete!")
  End Sub

  Private Sub btnRemoveContextMenu_Click(sender As Object, e As EventArgs) Handles btnRemoveContextMenu.Click
    Dim myXML As New XmlDocument
    Dim view32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
    If IO.File.Exists(My.Settings.SettingsPath) Then
      myXML.Load(My.Settings.SettingsPath)

      Try
        Log("Deleting registry")
        Dim reg = Registry.ClassesRoot.OpenSubKey(regFoldMain)
        If Not IsNothing(reg) Then
          reg.Close()
          Registry.ClassesRoot.DeleteSubKey(regFoldMain)
          Log("Deleted main 'PathStructure' registry entry")
        Else
          Log("Could not open main 'PathStructure' registry entry")
        End If

        reg = view32.OpenSubKey(regCommand & "PathStructure.Add")
        If Not IsNothing(reg) Then
          reg.Close()
          view32.DeleteSubKey(regCommand & "PathStructure.Add")
          Log("Deleted 'Add' registry entry")
        Else
          Log("Could not open main 'Add' registry entry")
        End If

        reg = view32.OpenSubKey(regCommand & "PathStructure.Add.All")
        If Not IsNothing(reg) Then
          reg.Close()
          view32.DeleteSubKeyTree(regCommand & "PathStructure.Add.All")
          Log("Deleted 'Add All' registry entry")
        Else
          Log("Could not open main 'Add All' registry entry")
        End If

        reg = view32.OpenSubKey(regCommand & "PathStructure.Add.Single")
        If Not IsNothing(reg) Then
          reg.Close()
          view32.DeleteSubKeyTree(regCommand & "PathStructure.Add.Single")
          Log("Deleted 'Add Single' registry entry")
        Else
          Log("Could not open main 'Add Single' registry entry")
        End If

        reg = view32.OpenSubKey(regCommand & "PathStructure.Format")
        If Not IsNothing(reg) Then
          reg.Close()
          view32.DeleteSubKeyTree(regCommand & "PathStructure.Format")
          Log("Deleted 'Format' registry entry")
        Else
          Log("Could not open main 'Format' registry entry")
        End If

        reg = view32.OpenSubKey(regCommand & "PathStructure.Audit")
        If Not IsNothing(reg) Then
          reg.Close()
          view32.DeleteSubKeyTree(regCommand & "PathStructure.Audit")
          Log("Deleted 'Audit' registry entry")
        Else
          Log("Could not open main 'Audit' registry entry")
        End If

        reg = view32.OpenSubKey(regCommand & "PathStructure.Open")
        If Not IsNothing(reg) Then
          reg.Close()
          view32.DeleteSubKeyTree(regCommand & "PathStructure.Open")
          Log("Deleted 'Open' registry entry")
        Else
          Log("Could not open main 'Open' registry entry")
        End If

        reg = view32.OpenSubKey(regCommand & "PathStructure.Clipboard")
        If Not IsNothing(reg) Then
          reg.Close()
          view32.DeleteSubKeyTree(regCommand & "PathStructure.Clipboard")
          Log("Deleted 'Clipboard' registry entry")
        Else
          Log("Could not open main 'Clipboard' registry entry")
        End If

        reg = view32.OpenSubKey(regCommand & "PathStructure.TransferByExtension")
        If Not IsNothing(reg) Then
          reg.Close()
          view32.DeleteSubKeyTree(regCommand & "PathStructure.TransferByExtension")
          Log("Deleted 'TransferByExtension' registry entry")
        Else
          Log("Could not open main 'TransferByExtension' registry entry")
        End If
      Catch ex As Exception
        Log("An error occurred..." & vbLf & vbTab & ex.Message)
      End Try
    Else
      Log("Couldn't find Settings File: '" & My.Settings.SettingsPath & "'")
    End If
    MsgBox("Complete!")
  End Sub


End Class