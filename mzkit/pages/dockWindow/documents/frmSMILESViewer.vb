#Region "Microsoft.VisualBasic::87eb8bc135369d03c1440626b2bf48fd, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmSMILESViewer.vb"

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

'   Total Lines: 51
'    Code Lines: 39
' Comment Lines: 0
'   Blank Lines: 12
'     File Size: 1.82 KB


' Class frmSMILESViewer
' 
'     Sub: Button1_Click, Button2_Click, Canvas1_Load, frmSMILESViewer_Load, Label1_Click
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemistry.Model.Drawing
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.Web.WebView2.Core
Imports Microsoft.Web.WebView2.WinForms
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmSMILESViewer

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AutoScaleMode = AutoScaleMode.Dpi
        DockAreas = DockAreas.Document Or DockAreas.Float
        TabText = "Loading WebView2 App..."
        Button1.Enabled = False
    End Sub

    Private Function getViewerUrl() As String
        Return AppEnvironment.getWebViewFolder & "/SMILES.html"
    End Function

    Private Function getKetcher() As String
        Return AppEnvironment.getWebViewFolder & "/ketcher/index.html"
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim smilesStr As String = Strings.Trim(TextBox1.Text)
        Dim js As String = $"rendering('{smilesStr}');"

        Try
            Dim graph As ChemicalFormula = ParseChain.ParseGraph(smilesStr)
        Catch ex As Exception

        End Try

        WebView21.CoreWebView2.ExecuteScriptAsync(js)
    End Sub

    Private Sub frmSMILESViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call frmHtmlViewer.Init(WebView21)
        Call frmHtmlViewer.Init(WebView22)
        Call Wait()

        Text = "Molecule Drawer"
        TabText = Text
    End Sub

    Public Shared Sub DeveloperOptions(WebView21 As WebView2, enable As Boolean)
        WebView21.CoreWebView2.Settings.AreDevToolsEnabled = enable
        WebView21.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = enable
        WebView21.CoreWebView2.Settings.AreDefaultContextMenusEnabled = enable
    End Sub

    Private Sub Wait()

    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        WebView21.CoreWebView2.Navigate(getViewerUrl)
        Button1.Enabled = True

        Call DeveloperOptions(WebView21, enable:=True)
    End Sub

    Private Sub WebView22_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView22.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        WebView22.CoreWebView2.Navigate(getKetcher)

        Call DeveloperOptions(WebView22, enable:=True)
    End Sub

    Private Sub WebView21_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs) Handles WebView21.NavigationCompleted
        Me.Text = WebView21.CoreWebView2.DocumentTitle
        Me.TabText = Me.Text
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

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim kcf As String = TextBox2.Text
        Dim model As KCF

        If kcf.LineTokens.Length = 1 Then
            model = ParseChain.ParseGraph(Strings.Trim(TextBox1.Text)).ToKCF
        Else
            model = IO.LoadKCF(kcf)
        End If

        Dim visual As Image = model.Draw().AsGDIImage

        PictureBox1.BackgroundImage = visual
    End Sub
End Class
