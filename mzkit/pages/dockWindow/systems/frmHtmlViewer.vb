#Region "Microsoft.VisualBasic::e7d46a1e851208677fb39dec47cfa0d2, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmHtmlViewer.vb"

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

'   Total Lines: 77
'    Code Lines: 62
' Comment Lines: 0
'   Blank Lines: 15
'     File Size: 2.54 KB


' Class frmHtmlViewer
' 
'     Sub: CopyFullPath, frmHtmlViewer_Load, LoadHtml, OpenContainingFolder, PDF
'          SaveDocument, WebBrowser1_DocumentCompleted
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports Mzkit_win32.BasicMDIForm.Container
Imports WeifenLuo.WinFormsUI.Docking
Imports WkHtmlToPdf.Arguments

Public Class frmHtmlViewer

    Dim sourceURL As String

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        Me.DockContextMenuStrip1.Items.Add(Me.ToolStripMenuItem1)
        Me.DockContextMenuStrip1.Items.Add(Me.GotoToolStripMenuItem)

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        AutoScaleMode = AutoScaleMode.Dpi
        DockAreas = DockAreas.Document Or DockAreas.Float
        TabText = "Loading WebView2 App..."
    End Sub

    Public Sub PDF(filepath As String)
        Static bin As String = AppEnvironment.getWkhtmltopdf

        If bin.FileExists Then
            Dim env As New PdfConvertEnvironment With {
                .Debug = False,
                .TempFolderPath = TempFileSystem.GetAppSysTempFile,
                .Timeout = 60000,
                .WkHtmlToPdfPath = bin
            }
            Dim content As New PdfDocument With {.Url = {sourceURL}}
            Dim pdfFile As New PdfOutput With {.OutputFilePath = filepath}

            Call WkHtmlToPdf.PdfConvert.ConvertHtmlToPdf(content, pdfFile, env)
        Else
            Call MyApplication.host.showStatusMessage("'wkhtmltopdf' tool is missing for generate PDF file...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        Call WebView21.CoreWebView2.Navigate(sourceURL)
        Call WebKit.DeveloperOptions(WebView21, enable:=True)
    End Sub

    Public Sub LoadHtml(url As String)
        Me.sourceURL = url
    End Sub

    Protected Overrides Sub CopyFullPath()
        Call Clipboard.SetText(sourceURL)
    End Sub

    Protected Overrides Sub OpenContainingFolder()
        Try
            If sourceURL.FileExists Then
                Call Process.Start(sourceURL.ParentPath)
            End If
        Catch ex As Exception
            MessageBox.Show("Document is not a local file...", "Web View2", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
    End Sub

    Protected Overrides Sub SaveDocument() Handles SavePDFToolStripMenuItem.Click
        Using file As New SaveFileDialog With {
            .Title = "Export page as pdf file.",
            .Filter = "PDF file(*.pdf)|*.pdf"
        }
            If file.ShowDialog = DialogResult.OK Then
                Call PDF(file.FileName)
                Call Process.Start(file.FileName)
            End If
        End Using
    End Sub

    Private Sub frmHtmlViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call ApplyVsTheme(ContextMenuStrip1)

        TabText = "Document Viewer"
        ' Icon = My.Resources.IE

        WebKit.Init(WebView21)
    End Sub

    Private Sub WebBrowser1_DocumentCompleted(sender As Object, e As WebBrowserDocumentCompletedEventArgs)
        Me.Text = WebView21.CoreWebView2.DocumentTitle
        Me.TabText = Me.Text
    End Sub


    ''' <summary>
    ''' open external link in default webbrowser
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub WebView21_NavigationStarting(sender As Object, e As CoreWebView2NavigationStartingEventArgs) Handles WebView21.NavigationStarting
        'Dim url As New URL(e.Uri)

        'If url.hostName <> "127.0.0.1" AndAlso url.hostName <> "localhost" Then
        '    e.Cancel = True
        '    Process.Start(e.Uri)
        'End If
    End Sub

    Private Sub frmHtmlViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If MessageBox.Show("Close current app page?", "Workbench WebView", buttons:=MessageBoxButtons.OKCancel, icon:=MessageBoxIcon.Information) = DialogResult.Cancel Then
            e.Cancel = True
        End If
    End Sub

    Private Sub GotoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GotoToolStripMenuItem.Click
        InputDialog.Input(Of InputURL)(
            setConfig:=Sub(input)
                           sourceURL = input.URL
                           WebView21.CoreWebView2.Navigate(sourceURL)
                       End Sub,
            config:=New InputURL With {.URL = sourceURL}
        )
    End Sub
End Class
