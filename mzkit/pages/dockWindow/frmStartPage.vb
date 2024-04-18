#Region "Microsoft.VisualBasic::996f160f0559a76ffbf07119dd772eb1, mzkit\src\mzkit\mzkit\pages\dockWindow\frmStartPage.vb"

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

'   Total Lines: 17
'    Code Lines: 12
' Comment Lines: 1
'   Blank Lines: 4
'     File Size: 517.00 B


' Class frmStartPage
' 
'     Sub: frmStartPage_Load
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmStartPage

    ' 所有需要在JavaScript环境中暴露的对象
    ' 都需要标记上下面的两个自定义属性
    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class LinkActions

        Public host As frmStartPage

        Public Sub ViewRawDataFile()
            Call frmStartPage.ViewRawDataFile()
        End Sub

        Public Sub OpenRterm()
            ' 打开R终端页面
            Call RibbonEvents.CreateNewScript(Nothing, Nothing)
        End Sub

        Public Sub OpenLCMSWorkbench()
            Call RibbonEvents.openLCMSWorkbench()
        End Sub

        Public Async Function GetNewsFeedJSON() As Task(Of String)
            Dim wait_http_get As Task(Of String) = Task(Of String) _
                .Run(Function() As String
                         Return "https://v2.biodeep.cn/api/nmdx-cloud-basic/km-curriculum-info/cloud/list?pageNo=1&pageSize=12&sort=new".GET
                     End Function)

            Return Await wait_http_get
        End Function

    End Class

    Dim WithEvents BackgroundWorker As New BackgroundWorker

    Public ReadOnly Property sourceURL As String
        Get
            Return $"http://127.0.0.1:{Workbench.WebPort}/"
        End Get
    End Property

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AutoScaleMode = AutoScaleMode.Dpi
    End Sub

    Private Sub frmStartPage_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Icon = My.Resources.chemistry
        Me.ShowIcon = True
        '    Me.ShowInTaskbar = True

        SaveDocumentToolStripMenuItem.Enabled = False
        CopyFullPathToolStripMenuItem.Enabled = False
        OpenContainingFolderToolStripMenuItem.Enabled = False

        hideNewsFeeds()
        ' BackgroundWorker.RunWorkerAsync()
        WebKit.Init(WebView21)
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", New LinkActions With {.host = Me})
        Call WebView21.CoreWebView2.Navigate(sourceURL)
        Call WebKit.DeveloperOptions(WebView21, enable:=True,)
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

    Private Sub hideNewsFeeds()
        'LinkLabel2.Visible = False
        'FlowLayoutPanel1.Visible = False
    End Sub

    Private Sub showNewsFeeds()
        'LinkLabel2.Visible = True
        'FlowLayoutPanel1.Visible = True
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

            If firstFile.ExtensionSuffix("raw", "wiff", "msp", "mgf") Then
                Call MyApplication.host.OpenFile(firstFile, showDocument:=True)
                Call VisualStudio.ShowDocument(Of frmUntargettedViewer)().loadRaw(WindowModules.rawFeaturesList.CurrentOpenedFile)
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
