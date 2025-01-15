#Region "Microsoft.VisualBasic::9adc228512d7cd8909c8650769cf9d55, mzkit\src\mzkit\mzkit\pages\dockWindow\frmSettings.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 82
'    Code Lines: 65
' Comment Lines: 0
'   Blank Lines: 17
'     File Size: 3.03 KB


' Class frmSettings
' 
'     Sub: Button1_Click, Button2_Click, frmSettings_Closing, LinkLabel1_LinkClicked, PageSettings_Load
'          SaveSettings, showPage, TreeView1_AfterSelect
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.SettingsPage
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmSettings

    Public ReadOnly Property sourceURL As String
        Get
            Return $"http://127.0.0.1:{Workbench.WebPort}/settings.html"
        End Get
    End Property

    Private Sub frmSettings_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.DockState = DockState.Hidden
    End Sub

    Public Sub closePage()
        DockState = DockState.Hidden
        MyApplication.host.ShowPage(MyApplication.host.mzkitTool)
    End Sub

    ''' <summary>
    ''' initialize of the webview
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub PageSettings_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Text = "Application Settings"
        Me.Icon = My.Resources.settings

        CopyFullPathToolStripMenuItem.Enabled = False
        OpenContainingFolderToolStripMenuItem.Enabled = False

        Me.ShowIcon = True

        WebKit.Init(WebView21)
    End Sub

    ''' <summary>
    ''' open settings page ui
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        ' set settings object from here
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", New SettingsProxy With {.host = Me})
        Call WebView21.CoreWebView2.Navigate(sourceURL)
        Call WebKit.DeveloperOptions(WebView21, enable:=True,)
    End Sub

    ''' <summary>
    ''' open external link in default webbrowser
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub WebView21_NavigationStarting(sender As Object, e As CoreWebView2NavigationStartingEventArgs) Handles WebView21.NavigationStarting
        Dim url As New URL(e.Uri)

        If url.hostName <> "127.0.0.1" AndAlso url.hostName <> "localhost" Then
            e.Cancel = True
            Process.Start(e.Uri)
        End If
    End Sub

    ''' <summary>
    ''' save settings
    ''' </summary>
    ''' <returns></returns>
    Public Async Function SaveSettings() As Threading.Tasks.Task
        Dim check = WebView21.ExecuteScriptAsync("apps.systems.settings.invoke_save()")

        Await check
        Call Workbench.SuccessMessage("New settings value applied and saved!")
    End Function
End Class
