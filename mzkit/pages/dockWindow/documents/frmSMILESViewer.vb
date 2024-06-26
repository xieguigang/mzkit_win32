﻿#Region "Microsoft.VisualBasic::87eb8bc135369d03c1440626b2bf48fd, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmSMILESViewer.vb"

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

Imports System.Text
Imports System.Threading
Imports BioNovoGene.BioDeep.Chemistry.Model
Imports BioNovoGene.BioDeep.Chemistry.Model.Drawing
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.SDF
Imports BioNovoGene.BioDeep.Chemoinformatics.SMILES
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.Web.WebView2.Core
Imports Microsoft.Web.WebView2.WinForms
Imports Mzkit_win32.BasicMDIForm
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
        Return $"http://127.0.0.1:{Workbench.WebPort}/SMILES.html"
    End Function

    Private Function getKetcher() As String
        Return $"http://127.0.0.1:{Workbench.WebPort}/ketcher/index.html"
    End Function

    Dim tableRows As New List(Of Object())

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim smilesStr As String = Strings.Trim(TextBox1.Text)
        Dim js As String = $"rendering('{smilesStr}');"

        Try
            Dim graph As ChemicalFormula = ParseChain.ParseGraph(smilesStr, strict:=False)
            Dim info As New StringBuilder
            Dim formula As Formula = graph.GetFormula(canonical:=True)

            Call info.AppendLine($"Formula: {formula.ToString}")
            Call info.AppendLine($"Element Composition: {formula.CountsByElement.GetJson}")
            Call info.AppendLine($"Exact Mass: {formula.ExactMass}")

            Call TextBox3.Clear()
            Call DataGridView1.Rows.Clear()
            Call tableRows.Clear()

            TextBox3.Text = info.ToString

            For Each atom As ChemicalElement In graph.AllElements
                Dim connects = ChemicalElement _
                    .GetConnection(graph, atom) _
                    .Select(Function(a)
                                Return $"{CInt(a.keys)}({a.Item2.group})"
                            End Function) _
                    .JoinBy("; ")

                Call tableRows.Add({atom.label, atom.elementName, atom.group, atom.charge, atom.Keys, connects})
                Call DataGridView1.Rows.Add(atom.label, atom.elementName, atom.group, atom.charge, atom.Keys, connects)
            Next
        Catch ex As Exception
            Call TextBox3.Clear()
            Call DataGridView1.Rows.Clear()
            Call tableRows.Clear()
        End Try

        WebView21.CoreWebView2.ExecuteScriptAsync(js)
    End Sub

    Private Sub frmSMILESViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call WebKit.Init(WebView21)
        Call WebKit.Init(WebView22)
        Call Wait()

        Text = "Molecule Drawer"
        TabText = Text

        Call ApplyVsTheme(ContextMenuStrip1)
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

        AddHandler WebView22.CoreWebView2.DownloadStarting,
            Sub(s, evt)
                ' Developer can obtain a deferral for the event so that the CoreWebView2
                ' doesn't examine the properties we set on the event args until
                ' after the deferral completes asynchronously.
                Dim deferral As CoreWebView2Deferral = evt.GetDeferral()
                ' We avoid potential reentrancy from running a message loop in the download
                ' starting event handler by showing our download dialog later when we
                ' complete the deferral asynchronously.
                SynchronizationContext.Current.Post(
                    Sub()
                        Using deferral
                            UpdateProgress(evt.ResultFilePath, evt.DownloadOperation)
                        End Using
                    End Sub, Nothing)
            End Sub
    End Sub

    Private Sub UpdateProgress(localfile As String, download As CoreWebView2DownloadOperation)
        AddHandler download.StateChanged,
            Sub(sender, e)
                Select Case download.State
                    Case CoreWebView2DownloadState.InProgress
                    Case CoreWebView2DownloadState.Interrupted
                    Case CoreWebView2DownloadState.Completed
                        If localfile.ExtensionSuffix("smi", "cxsmi", "inchi") AndAlso MessageBox.Show(
                            text:=$"We check that a new molecule SMILES/InChi structre has been generated,{vbCrLf} do mass spectrum prediction of current molecule?",
                            caption:="MZKit CFM-ID toolkit",
                            buttons:=MessageBoxButtons.YesNo,
                            icon:=MessageBoxIcon.Information
                        ) Then

                            Dim type As String = If(localfile.ExtensionSuffix("inchi"), "inchi", "smiles")

                            ' do cfm-id prediction
                            TextBox1.Text = localfile.ReadAllText.Trim(" "c, vbCr, vbLf, vbTab)

                            Call Button1_Click(Nothing, Nothing)
                            Call OpenCFMIDTool(TextBox1.Text)
                        End If
                End Select
            End Sub
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

    ''' <summary>
    ''' cfm-id prediction
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Call OpenCFMIDTool(TextBox1.Text)
    End Sub

    Private Sub ExportTableFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportTableFileToolStripMenuItem.Click
        Call DataGridView1.SaveDataGrid("Export Molecule Composition To File")
    End Sub

    Private Sub SendToTableViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SendToTableViewerToolStripMenuItem.Click
        Dim table = VisualStudio.ShowDocument(Of frmTableViewer)(, title:="SMILES data")

        ' Call tableRows.Add({atom.label, atom.elementName, atom.group, atom.charge, atom.Keys, connects})
        Call table.LoadTable(
            Sub(a)
                Call a.Columns.Add("#ID", GetType(String))
                Call a.Columns.Add("Atom", GetType(String))
                Call a.Columns.Add("Atom Group", GetType(String))
                Call a.Columns.Add("Charge", GetType(Integer))
                Call a.Columns.Add("Links", GetType(Integer))
                Call a.Columns.Add("Atom Group Connections", GetType(String))

                For Each row In tableRows
                    Call a.Rows.Add(row)
                Next
            End Sub)
    End Sub
End Class
