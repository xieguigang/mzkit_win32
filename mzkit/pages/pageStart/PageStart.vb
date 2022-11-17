#Region "Microsoft.VisualBasic::45e8f1bba0f9a16812868bd16de93448, mzkit\src\mzkit\mzkit\pages\pageStart\PageStart.vb"

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

'   Total Lines: 116
'    Code Lines: 91
' Comment Lines: 3
'   Blank Lines: 22
'     File Size: 4.50 KB


' Class PageStart
' 
'     Sub: BackgroundWorker_DoWork, hideNewsFeeds, LinkLabel1_LinkClicked, LinkLabel2_LinkClicked, LinkLabel3_LinkClicked
'          LinkLabel4_LinkClicked, PageStart_DragDrop, PageStart_DragEnter, PageStart_Load, PageStart_Resize
'          showNewsFeeds
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.Web.WebView2.Core
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class PageStart

    ' 所有需要在JavaScript环境中暴露的对象
    ' 都需要标记上下面的两个自定义属性
    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class LinkActions

        Public Sub ViewRawDataFile()
            Call PageStart.ViewRawDataFile()
        End Sub

        Public Shared Sub OpenRterm()
            ' 打开R终端页面
            Call RibbonEvents.CreateNewScript(Nothing, Nothing)
        End Sub

    End Class

    Dim WithEvents BackgroundWorker As New BackgroundWorker

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        AutoScaleMode = AutoScaleMode.Dpi
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", New LinkActions)
        Call WebView21.CoreWebView2.Navigate($"http://127.0.0.1:{Globals.WebPort}/")
        Call DeveloperOptions(enable:=True)
    End Sub

    Public Sub DeveloperOptions(enable As Boolean)
        WebView21.CoreWebView2.Settings.AreDevToolsEnabled = enable
        WebView21.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = enable
        WebView21.CoreWebView2.Settings.AreDefaultContextMenusEnabled = enable
    End Sub

    Public Shared Sub ViewRawDataFile()
        Dim fileExplorer = WindowModules.fileExplorer

        WindowModules.fileExplorer.DockState = DockState.DockLeft
        WindowModules.rawFeaturesList.DockState = DockState.DockLeft

        If fileExplorer.treeView1.SelectedNode Is Nothing AndAlso fileExplorer.treeView1.Nodes.Count > 0 Then
            If fileExplorer.treeView1.Nodes(0).Nodes.Count = 0 Then
                ' imports raw
                Call WindowModules.OpenFile()
            End If
            If fileExplorer.treeView1.Nodes(0).Nodes.Count = 0 Then
                ' user cancel imports raw data files
                Return
            End If
            Dim firstFile = fileExplorer.treeView1.Nodes(0).Nodes(0)

            fileExplorer.treeView1.SelectedNode = firstFile
            fileExplorer.showRawFile(DirectCast(firstFile.Tag, Raw), False, directSnapshot:=True, contour:=False)
        End If

        MyApplication.host.ShowMzkitToolkit()
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

    Private Sub PageStart_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        hideNewsFeeds()
        frmHtmlViewer.Init(WebView21)
        ' BackgroundWorker.RunWorkerAsync()
    End Sub

    Private Sub hideNewsFeeds()
        'LinkLabel2.Visible = False
        'FlowLayoutPanel1.Visible = False
    End Sub

    Private Sub showNewsFeeds()
        'LinkLabel2.Visible = True
        'FlowLayoutPanel1.Visible = True
    End Sub

    Private Sub BackgroundWorker_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker.DoWork
        Dim news As NewsFeed() = NewsFeed.ParseLatest().ToArray

        If news.IsNullOrEmpty Then
            Call MyApplication.LogText(NewsFeed.html)
        End If

        Invoke(Sub()
                   'If news.Length = 0 Then
                   '    hideNewsFeeds()
                   'Else
                   '    showNewsFeeds()
                   'End If

                   'For Each newsItem As NewsFeed In news
                   '    Dim display As New NewsFeedDisplay
                   '    FlowLayoutPanel1.Controls.Add(display)
                   '    display.ShowNews(newsItem)
                   'Next
               End Sub)
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs)
        Process.Start("http://www.bionovogene.com/news/newsFeed.htm")
    End Sub

    Private Sub PageStart_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        'If Width < 955 Then
        '    LinkLabel2.Visible = False
        '    FlowLayoutPanel1.Visible = False
        'Else
        '    LinkLabel2.Visible = True
        '    FlowLayoutPanel1.Visible = True
        'End If
    End Sub

    Private Sub PageStart_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop, WebView21.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        Dim firstFile As String = files.ElementAtOrDefault(Scan0)

        If Not firstFile Is Nothing Then
            Call Application.DoEvents()
            Call MyApplication.host.Show()
            Call Application.DoEvents()

            If firstFile.ExtensionSuffix("raw", "wiff", "msp") Then
                Call MyApplication.host.OpenFile(firstFile, showDocument:=True)
                Call VisualStudio.ShowDocument(Of frmUntargettedViewer)().loadRaw(WindowModules.rawFeaturesList.CurrentRawFile)
            Else
                Dim page As frmSeeMs = VisualStudio.ShowDocument(Of frmSeeMs)

                page.TabText = "SeeMS: " & firstFile.FileName
                page.LoadRaw(firstFile)
            End If
        End If
    End Sub

    Private Sub PageStart_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter, WebView21.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub
End Class
