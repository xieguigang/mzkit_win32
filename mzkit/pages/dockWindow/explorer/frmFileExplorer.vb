﻿#Region "Microsoft.VisualBasic::c0eb55e1de5a576797840449030ac912, mzkit\src\mzkit\mzkit\pages\dockWindow\explorer\frmFileExplorer.vb"

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

'   Total Lines: 561
'    Code Lines: 416
' Comment Lines: 40
'   Blank Lines: 105
'     File Size: 22.53 KB


' Class frmFileExplorer
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: CurrentRawFile, deleteFileNode, (+2 Overloads) findRawFileNode, getRawCache, GetRawFiles
'               GetSelectedRaws, GetTotalCacheSize
' 
'     Sub: addFileNode, AddScript, BPCOverlapToolStripMenuItem_Click, Button1_Click, ContourPlotToolStripMenuItem_Click
'          DeleteToolStripMenuItem_Click, frmFileExplorer_Activated, frmFileExplorer_Closing, frmFileExplorer_Load, ImportsRaw
'          ImportsToolStripMenuItem_Click, InitializeFileTree, OpenViewerToolStripMenuItem_Click, RawScatterToolStripMenuItem_Click, RunAutomationToolStripMenuItem_Click
'          SaveFileCache, selectRawFile, showRawFile, ShowSummaryToolStripMenuItem_Click, TICOverlapToolStripMenuItem_Click
'          ToolStripButton2_Click, treeView1_AfterCheck, treeView1_AfterSelect, treeView1_Click, treeView1_GotFocus
'          UpdateMainTitle, XICPeaksToolStripMenuItem_Click
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzXML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Text
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports RibbonLib.Interop
Imports Task
Imports TaskStream
Imports WeifenLuo.WinFormsUI.Docking
Imports std = System.Math

''' <summary>
''' 显示一个workspace对象里面所包含有的文件列表
''' </summary>
Public Class frmFileExplorer

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        DoubleBuffered = True
    End Sub

    Private Sub frmFileExplorer_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active
        Call treeView1_AfterSelect(Nothing, Nothing)
    End Sub

    Private Sub frmFileExplorer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.Hide()
    End Sub

    Public Sub selectRawFile(index As Integer)
        Dim raw As TreeNode = treeView1.Nodes.Item(0)

        treeView1.SelectedNode = raw.Nodes.Item(index)
        showRawFile(treeView1.SelectedNode.Tag, XIC:=False, directSnapshot:=True, contour:=False)
    End Sub

    Public Function GetTotalCacheSize() As String
        If treeView1.Nodes.Count > 0 Then
            Return treeView1.Nodes.Item(0).GetTotalCacheSize
        Else
            Return "0 KB"
        End If
    End Function

    Public Function CurrentRawFile() As MZWork.Raw
        If treeView1.SelectedNode Is Nothing Then
            Return Nothing
        ElseIf treeView1.SelectedNode.Tag Is Nothing Then
            Return Nothing
        ElseIf TypeOf treeView1.SelectedNode.Tag Is String Then
            Return Nothing
        Else
            Return treeView1.SelectedNode.Tag
        End If
    End Function

    ''' <summary>
    ''' get all raw data files reference inside current lcms workspace
    ''' </summary>
    ''' <returns></returns>
    Public Iterator Function GetRawFiles() As IEnumerable(Of MZWork.Raw)
        Dim rawList = treeView1.Nodes.Item(Scan0)

        For i As Integer = 0 To rawList.Nodes.Count - 1
            Yield DirectCast(rawList.Nodes(i).Tag, MZWork.Raw)
        Next
    End Function

    Public Iterator Function GetSelectedRaws() As IEnumerable(Of MZWork.Raw)
        If treeView1.Nodes.Count = 0 Then
            Return
        End If

        Dim rawList = treeView1.Nodes.Item(Scan0)

        For i As Integer = 0 To rawList.Nodes.Count - 1
            If Not rawList.Nodes(i).Checked Then
                If Not rawList.Nodes(i) Is treeView1.SelectedNode Then
                    Continue For
                End If
            End If

            Dim raw As MZWork.Raw = rawList.Nodes(i).Tag

            Yield raw
        Next
    End Function

    Sub InitializeFileTree()
        If treeView1.LoadRawFileCache(ctxMenuFiles, ctxMenuRawFile, ctxMenuScript, Globals.Settings.workspaceFile) = 0 Then
            MyApplication.host.showStatusMessage($"It seems that you don't have any raw file opended. You could open raw file through [File] -> [Open Raw File].", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            ' selectRawFile(Scan0)
            ' setCurrentFile()
        End If

        MyApplication.host.ToolStripStatusLabel2.Text = GetTotalCacheSize()
    End Sub

    Private Sub frmFileExplorer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(treeView1)

        treeView1.HotTracking = True
        treeView1.BringToFront()
        treeView1.CheckBoxes = True
        treeView1.ContextMenuStrip = ctxMenuFiles
        treeView1.ShowLines = True
        treeView1.ShowRootLines = True
        treeView1.BorderStyle = BorderStyle.FixedSingle
        treeView1.Dock = DockStyle.Fill
        treeView1.ImageList = ImageList2

        ToolStrip1.Stretch = True

        '   ExportToolStripMenuItem.Text = "Export XIC Ions"

        Me.TabText = "File Explorer"

        LCMSViewerModule.lcmsWorkspace = New Func(Of IEnumerable)(AddressOf GetRawFiles)
        LCMSViewerModule.setWorkFile = New Action(Of Object)(Sub(o) Call SetActiveWorkfile(DirectCast(o, MZWork.Raw)))
        LCMSViewerModule.addWorkFile = New Action(Of Object, Boolean)(Sub(o, f) Call addFileNode(DirectCast(o, MZWork.Raw), f))
        LCMSViewerModule.clearWorkspace = Sub() Call Me.Invoke(Sub() Call Clear())

        Call InitializeFileTree()
        Call ApplyVsTheme(ctxMenuFiles, ToolStrip1, ctxMenuScript, ctxMenuRawFile)
    End Sub

    ''' <summary>
    ''' this method switch to background task or UI foreground 
    ''' task for imports data automatically based on the file
    ''' extension suffix name.
    ''' </summary>
    ''' <param name="fileName"></param>
    Public Sub ImportsRaw(fileName As String, snapshot As Boolean)
        If treeView1.Nodes.Count = 0 Then
            ' just create folder node on the tree UI
            ' if the tree ui is empty
            Call Globals.InitExplorerUI(
                explorer:=treeView1,
                rawMenu:=ctxMenuFiles,
                scriptMenu:=ctxMenuScript
            )
        End If

        ' handling of the MRM mzml data
        If fileName.ExtensionSuffix("mzml") AndAlso (RawScanParser.IsMRMData(fileName) OrElse RawScanParser.IsSIMData(fileName)) Then
            Call MyApplication.host.OpenFile(fileName, showDocument:=True)
        ElseIf treeView1.Nodes.Count = 0 OrElse treeView1.Nodes.Item(0).Nodes.Count = 0 Then
            Call addFileNode(getRawCache(fileName))
        ElseIf fileName.ExtensionSuffix("mzpack") Then
            ' for mzpack data file, use add to the file tree
            ' no needs for the background task of imports data
            Call addFileNode(getRawCache(fileName))
        Else
            ' work in background
            Dim taskList As TaskListWindow = WindowModules.taskWin
            Dim task As TaskUI = taskList.Add("Imports Raw Data", fileName)

            taskList.Show(MyApplication.host.m_dockPanel)
            taskList.DockState = DockState.DockBottom

            Call MyApplication.host.showStatusMessage($"Imports raw data file [{fileName.FileName}] in background, you can open [Task List] panel for view task progress.")
            Call MyApplication.TaskQueue.AddToQueue(
                Sub()
                    Call task.Running()

                    Dim importsTask As New Task.ImportsRawData(
                        file:=fileName,
                        create_snapshot:=snapshot,
                        progress:=Sub(msg)
                                      ' do nothing
                                      Call task.ProgressMessage(msg)
                                      Call MyApplication.LogText(msg)
                                  End Sub,
                        finished:=Sub()
                                      Call task.Finish()
                                  End Sub)

                    Call importsTask.RunImports()

                    addFileNode(importsTask.raw)
                End Sub)
        End If
    End Sub

    Public Sub addFileNode(newRaw As MZWork.Raw, Optional batchImportsCache As Boolean = False)
        Me.Invoke(Sub()
                      Dim file As TreeNode = Globals.RawFileNodeTemplate(newRaw, targetRawMenu:=ctxMenuRawFile)
                      treeView1.Nodes(0).Nodes.Add(file)
                  End Sub)

        Globals.workspace.Add(newRaw)

        If batchImportsCache Then
            MyApplication.host.showStatusMessage($"Imports cache from: {newRaw.source}")
        Else
            ' make status update
            MyApplication.host.showStatusMessage("Ready!")
            MyApplication.host.UpdateCacheSize(GetTotalCacheSize)
        End If
    End Sub

    ''' <summary>
    ''' do raw data file imports task
    ''' </summary>
    ''' <param name="fileName"></param>
    ''' <returns></returns>
    Public Shared Function getRawCache(fileName As String, Optional titleTemplate$ = "Imports raw data [%s]", Optional cachePath As String = Nothing) As MZWork.Raw
        Call Workbench.StatusMessage("Run Raw Data Imports")

        If fileName.ExtensionSuffix("mzpack") Then
            Return MZWork.Raw.UseMzPack(fileName)
        End If

        Return TaskProgress.LoadData(
            streamLoad:=Function(p)
                            Dim task As New Task.ImportsRawData(
                                file:=fileName,
                                progress:=AddressOf p.SetInfo,
                                finished:=AddressOf p.TaskFinish,
                                cachePath:=cachePath
                            )

                            task.RunImports()
                            Return task.raw
                        End Function,
            title:=sprintf(titleTemplate, fileName)
        )
    End Function


    Public Sub SaveFileCache(progress As Action(Of String))
        Call treeView1.SaveRawFileCache(progress)
    End Sub

    Dim lockFileDelete As Boolean = False

    Public Sub showRawFile(raw As MZWork.Raw, XIC As Boolean, directSnapshot As Boolean, contour As Boolean)
        If lockFileDelete Then
            Return
        ElseIf Not raw.cacheFileExists Then
            raw.cache = getRawCache(raw.source, titleTemplate:="Re-Build file cache [%s]").cache
        End If

        If Not raw.isLoaded Then
            raw = raw.LoadMzpack(Sub(a, b) Workbench.StatusMessage($"[{a}] {b}"), strict:=False)
        End If

        Call MyApplication.host.mzkitTool.showScatter(raw, XIC, directSnapshot, contour)
        Call SetActiveWorkfile(raw)
    End Sub

    Public Sub SetActiveWorkfile(raw As MZWork.Raw)
        Call MyApplication.host.Invoke(
            Sub()
                Call WindowModules.rawFeaturesList.LoadRaw(raw)
                Call VisualStudio.ShowProperties(New RawFileProperty(raw))
                Call VisualStudio.Dock(WindowModules.rawFeaturesList, DockState.DockLeft)

                Call UpdateMainTitle(raw.source)
            End Sub)
    End Sub

    Private Sub RawScatterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RawScatterToolStripMenuItem.Click
        If treeView1.SelectedNode Is Nothing Then
            Return
        End If

        If TypeOf treeView1.SelectedNode.Tag Is MZWork.Raw Then
            Call showRawFile(DirectCast(treeView1.SelectedNode.Tag, MZWork.Raw), XIC:=False, directSnapshot:=False, contour:=False)
        End If
    End Sub

    Private Sub XICPeaksToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles XICPeaksToolStripMenuItem.Click
        If treeView1.SelectedNode Is Nothing Then
            Return
        End If

        If TypeOf treeView1.SelectedNode.Tag Is MZWork.Raw Then
            Call showRawFile(DirectCast(treeView1.SelectedNode.Tag, MZWork.Raw), XIC:=True, directSnapshot:=False, contour:=False)
        End If
    End Sub

    Public Sub UpdateMainTitle(source As String)
        If source.Any(Function(c) ASCII.IsNonPrinting(CByte(AscW(c))) OrElse c = ASCII.CR OrElse c = ASCII.LF) Then
            MyApplication.host.Text = $"BioNovoGene Mzkit [{source.Where(Function(c) AscW(c) >= 32).CharString}]"
        Else
            Dim text As String = source.GetFullPath(throwEx:=False)

            If text.StringEmpty Then
                text = source
            End If

            MyApplication.host.Text = $"BioNovoGene Mzkit [{text}]"
        End If
    End Sub

    Public Sub AddScript(script As String)
        treeView1.Nodes(1).Nodes.Add(New TreeNode(script.FileName) With {.Tag = script})
    End Sub

    Private Sub treeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles treeView1.AfterSelect
        If treeView1.SelectedNode Is Nothing Then
            Return
        End If

        'If treeView1.SelectedNode Is treeView1.Nodes(0) Then
        '    treeView1.ContextMenuStrip = ContextMenuStrip1
        'ElseIf treeView1.SelectedNode Is treeView1.Nodes(1) Then
        '    treeView1.ContextMenuStrip = ContextMenuStrip2
        'End If

        If TypeOf treeView1.SelectedNode.Tag Is MZWork.Raw Then
            Call showRawFile(DirectCast(treeView1.SelectedNode.Tag, MZWork.Raw), XIC:=False, directSnapshot:=True, contour:=False)

            '  treeView1.ContextMenuStrip = ContextMenuStrip1

        ElseIf TypeOf treeView1.SelectedNode.Tag Is String Then
            ' 选择了一个脚本文件
            Dim path As String = DirectCast(treeView1.SelectedNode.Tag, String).GetFullPath
            Dim script = RibbonEvents.scriptFiles _
                .Where(Function(a)
                           Return a.scriptFile.GetFullPath = path
                       End Function) _
                .FirstOrDefault

            '  treeView1.ContextMenuStrip = ContextMenuStrip2

            If Not script Is Nothing Then
                script.Show(MyApplication.host.m_dockPanel)
                MyApplication.host.Text = $"BioNovoGene Mzkit [{path.GetFullPath}]"
            ElseIf path.FileExists Then
                ' 脚本文件还没有被打开
                ' 在这里打开脚本文件
                MyApplication.host.openRscript(path)
                MyApplication.host.Text = $"BioNovoGene Mzkit [{path.GetFullPath}]"
            Else
                Workbench.Warning($"script file '{path.FileName}' is not exists...")

                e.Node.ImageIndex = 4
                e.Node.SelectedImageIndex = 4
                e.Node.StateImageIndex = 4
            End If
        End If
    End Sub

    Private Sub treeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles treeView1.AfterCheck
        If e.Node.Tag Is Nothing Then
            ' 是顶层的节点
            For Each fileNode As TreeNode In e.Node.Nodes
                fileNode.Checked = e.Node.Checked
            Next
        Else
            ' do nothing
        End If
    End Sub

    Private Sub treeView1_GotFocus(sender As Object, e As EventArgs) Handles treeView1.GotFocus
        ' Call treeView1_AfterSelect(Nothing, Nothing)
    End Sub

    Private Sub TICOverlapToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TICOverlapToolStripMenuItem.Click
        MyApplication.host.mzkitTool.TIC(isBPC:=False)
    End Sub

    Private Sub BPCOverlapToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BPCOverlapToolStripMenuItem.Click
        MyApplication.host.mzkitTool.TIC(isBPC:=True)
    End Sub

    ''' <summary>
    ''' 将原始数据文件从当前工作区移除
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        Dim fileList As New List(Of TreeNode)

        If treeView1.Nodes.Count = 0 Then
            Return
        End If

        For Each node As TreeNode In treeView1.Nodes(0).Nodes
            If node.Checked Then
                fileList.Add(node)
            End If
        Next

        For Each node As TreeNode In treeView1.Nodes(1).Nodes
            If node.Checked Then
                fileList.Add(node)
            End If
        Next

        If fileList.Count = 0 AndAlso treeView1.SelectedNode Is Nothing Then
            Return
        ElseIf fileList.Count = 0 Then
            Call deleteFileNode(node:=treeView1.SelectedNode, confirmDialog:=True)
        ElseIf MessageBox.Show($"Confirm to remove {fileList.Count} files from current workspace?", "File Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            lockFileDelete = True

            For Each file In fileList
                Call deleteFileNode(file, confirmDialog:=False)
            Next

            lockFileDelete = False
        End If
    End Sub

    Public Function findRawFileNode(sourceName As String) As TreeNode
        If treeView1.Nodes.Count = 0 Then
            Return Nothing
        End If

        For Each node As TreeNode In treeView1.Nodes(0).Nodes
            If DirectCast(node.Tag, MZWork.Raw).source.FileName = sourceName Then
                Return node
            End If
        Next

        Return Nothing
    End Function

    Public Function findRawFileNode(raw As MZWork.Raw) As TreeNode
        If treeView1.Nodes.Count = 0 Then
            Return Nothing
        End If

        For Each node As TreeNode In treeView1.Nodes(0).Nodes
            If node.Tag Is raw Then
                Return node
            End If
        Next

        Return Nothing
    End Function

    Public Function deleteFileNode(node As TreeNode, confirmDialog As Boolean) As DialogResult
        ' 跳过根节点
        If node Is Nothing OrElse node.Tag Is Nothing Then
            Return DialogResult.No
        End If

        Dim fileName As String

        If TypeOf node.Tag Is MZWork.Raw Then
            fileName = DirectCast(node.Tag, MZWork.Raw).source.FileName
        Else
            fileName = DirectCast(node.Tag, String).FileName
        End If

        Dim opt As DialogResult

        If confirmDialog Then
            opt = MessageBox.Show(
                text:=$"Going to removes {fileName} from your workspace?",
                caption:="Delete workspace file",
                buttons:=MessageBoxButtons.YesNo,
                icon:=MessageBoxIcon.Question
            )
        Else
            opt = DialogResult.Yes
        End If

        If opt = DialogResult.Yes Then
            If TypeOf node.Tag Is MZWork.Raw Then
                treeView1.Nodes(0).Nodes.Remove(node)
            Else
                treeView1.Nodes(1).Nodes.Remove(node)
            End If
        End If

        MyApplication.host.ToolStripStatusLabel2.Text = GetTotalCacheSize()

        Return opt
    End Function

    Private Sub RunAutomationToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RunAutomationToolStripMenuItem1.Click
        If treeView1.SelectedNode Is Nothing OrElse treeView1.SelectedNode.Tag Is Nothing OrElse Not TypeOf treeView1.SelectedNode.Tag Is String Then
            Return
        End If

        Dim scriptFile As String = DirectCast(treeView1.SelectedNode.Tag, String)

        Call WindowModules.RtermPage.ShowPage()
        Call MyApplication.ExecuteRScript(scriptFile, isFile:=True, AddressOf WindowModules.output.AppendRoutput)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Dim str As String = Strings.Trim(ToolStripSpringTextBox1.Text)
        Dim mzdiff As Tolerance

        If str.IsSimpleNumber Then
            mzdiff = Tolerance.PPM(20)
        Else
            mzdiff = Tolerance.DeltaMass(0.05)
        End If

        Call SearchFeatures(str, mzdiff)
    End Sub

    Public Sub SearchFeatures(feature As String, mzdiff As Tolerance)
        Dim raws As New List(Of MZWork.Raw)

        For Each node As TreeNode In treeView1.Nodes(0).Nodes
            Call raws.Add(node.Tag)
        Next

        Call ProgressSpinner.DoLoading(
            Sub()
                Call FeatureSearchHandler.SearchByMz(feature, raws, False, mzdiff)
            End Sub, host:=MyApplication.host)
    End Sub

    ''' <summary>
    ''' add adducts to the exact mass and then search for the generated m/z features in ms1 level
    ''' </summary>
    ''' <param name="mass">
    ''' it is the exact mass value, not ion m/z value
    ''' </param>
    ''' <param name="mzdiff"></param>
    Public Sub SearchExactMassFeatures(mass As Double, mzdiff As Tolerance)
        Dim raws As New List(Of MZWork.Raw)

        For Each node As TreeNode In treeView1.Nodes(0).Nodes
            Call raws.Add(node.Tag)
        Next

        Call FeatureSearchHandler.SearchByExactMass(mass, raws, mzdiff)
    End Sub

    Private Sub ImportsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportsToolStripMenuItem.Click
        Call MyApplication.host.ImportsFiles()
    End Sub

    ''' <summary>
    ''' Clear all rawdata file inside current workspace file tree
    ''' </summary>
    Public Sub Clear()
        treeView1.Nodes(0).Nodes.Clear()
    End Sub

    Private Sub ContourPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ContourPlotToolStripMenuItem.Click
        If treeView1.SelectedNode Is Nothing Then
            Return
        End If

        If TypeOf treeView1.SelectedNode.Tag Is MZWork.Raw Then
            Call showRawFile(DirectCast(treeView1.SelectedNode.Tag, MZWork.Raw), XIC:=False, directSnapshot:=False, contour:=True)
        End If
    End Sub

    ''' <summary>
    ''' export workspace as mzwork workspace file
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Using file As New SaveFileDialog With {.Filter = "MZkit workspace(*.mzWork)|*.mzWork"}
            If file.ShowDialog = DialogResult.OK Then
                Call TaskProgress.LoadData(
                    streamLoad:=Function(msg)
                                    Return MZWorkPack.ExportWorkspace(
                                        workspace:=Globals.workspace.work,
                                        save:=file.FileName,
                                        msg:=AddressOf msg.SetInfo
                                    )
                                End Function,
                    title:="Save Workspace",
                    info:=$"Export workspace to [{file.FileName}]"
                )
                Call MessageBox.Show("job done!", "MZKit", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    Private Sub ShowSummaryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowSummaryToolStripMenuItem.Click
        Dim table As frmTableViewer = VisualStudio.ShowDocument(Of frmTableViewer)

        table.ViewRow =
            Sub(row)
                Dim sourceFile As String = row("source")
                Dim node = findRawFileNode(sourceFile)

                Call showRawFile(DirectCast(node.Tag, MZWork.Raw), XIC:=False, directSnapshot:=True, contour:=False)
            End Sub
        table.LoadTable(
            Sub(grid)
                Call grid.Columns.Add("source", GetType(String))
                Call grid.Columns.Add("ms1_scans", GetType(Integer))
                Call grid.Columns.Add("msn_scans", GetType(Integer))
                Call grid.Columns.Add("rtmin", GetType(Double))
                Call grid.Columns.Add("rtmax", GetType(Double))
                Call grid.Columns.Add("total_ions", GetType(Double))
                Call grid.Columns.Add("base_peak", GetType(Double))
                Call grid.Columns.Add("max_intensity", GetType(Double))

                If treeView1.Nodes.Count = 0 Then
                    Return
                End If

                Call ProgressSpinner.DoLoading(Sub()
                                                   Call Thread.Sleep(500)

                                                   For Each node As TreeNode In treeView1.Nodes(0).Nodes
                                                       Dim raw As MZWork.Raw = node.Tag
                                                       Dim load As mzPack = raw _
                                            .LoadMzpack(Sub(title, msg)
                                                            MyApplication.host.showStatusMessage($"{title}: {msg}")
                                                        End Sub) _
                                            .GetLoadedMzpack
                                                       Dim basePeak As ms2 = load.GetBasePeak

                                                       Call table.Invoke(
                                            Sub()
                                                Call grid.Rows.Add(
                                                    node.Text,
                                                    load.MS.Length,
                                                    load.CountMs2,
                                                    load.rtmin.ToString("F1"),
                                                    load.rtmax.ToString("F1"),
                                                    std.Round(load.totalIons),
                                                    If(basePeak Is Nothing, 0, basePeak.mz.ToString("F4")),
                                                    If(basePeak Is Nothing, 0, std.Round(basePeak.intensity))
                                                )
                                            End Sub)
                                                   Next

                                               End Sub)
            End Sub)
    End Sub

    Private Sub ConvertToMzXMLToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ConvertToMzXMLToolStripMenuItem.Click
        Dim node = treeView1.SelectedNode

        If node Is Nothing OrElse TypeOf node.Tag IsNot MZWork.Raw Then
            Return
        Else
            Using save As New SaveFileDialog With {
                .Filter = "mzXML MsData(*.mzXML)|*.mzXML",
                .FileName = $"{DirectCast(node.Tag, MZWork.Raw).source.BaseName}.mzXML"
            }
                If save.ShowDialog = DialogResult.OK Then
                    Dim raw As MZWork.Raw = DirectCast(node.Tag, MZWork.Raw).LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache))
                    Dim mzPack As mzPack = raw.GetLoadedMzpack

                    Using file As Stream = save.FileName.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False),
                        writer As New mzXMLWriter({}, {}, {}, file)

                        Call TaskProgress _
                            .LoadData(
                                streamLoad:=Function(s As Action(Of String))
                                                Call writer.WriteData(mzPack.MS, print:=s)
                                                Return True
                                            End Function,
                                title:="Save as mzXML file",
                                info:="Save mzPack data as mzXML file..."
                            )
                    End Using

                    Call MessageBox.Show("Convert to mzXML success!", "MZKit", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        End If
    End Sub

    ''' <summary>
    ''' extract ms1 features from current raw data file
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub DeconvolutionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeconvolutionToolStripMenuItem.Click
        Dim node As New List(Of MZWork.Raw)

        For Each folder As TreeNode In treeView1.Nodes
            For Each file As TreeNode In folder.Nodes
                If TypeOf file.Tag Is MZWork.Raw Then
                    Call node.Add(file.Tag)
                End If
            Next
        Next

        If node.IsNullOrEmpty Then
            Call Workbench.Warning("No rawdata files was selected for run the LCMS peaktable deconvolution.")
            Return
        End If

        Dim config As New InputLCMSDeconvolution
        config.SetFiles(From raw As MZWork.Raw In node Select raw.cache)

        Call InputDialog.Input(
            Sub(pars)
                Call RunDeconvBackground(pars)
            End Sub, config:=config)
    End Sub

    Private Sub RunDeconvBackground(config As InputLCMSDeconvolution)
        Dim files As String = config.input_raw
        Dim tempTable As String = config.export_file
        Dim cli As String = $"""{RscriptPipelineTask.GetRScript("MS1deconv.R")}"" --raw ""{files}"" --save ""{tempTable}"" --massdiff {config.massDiff} --rt_win {config.rt_win.JoinBy(",")} --n_threads {config.n_threads} /@set tqdm=false --SetDllDirectory {Task.TaskEngine.hostDll.ParentPath.CLIPath}"

        Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli, RscriptPipelineTask.Root)
        Call Workbench.LogText(cli)

        ' work in background
        Dim taskList As TaskListWindow = WindowModules.taskWin
        Dim title As String = If(config.files.Length = 1, "Run Ms1 Deconvolution", "Batch LC-MS deconvolution")
        Dim taskUI As TaskUI = taskList.Add(title, "Export to: " & tempTable.FileName)

        If config.files.Length = 1 Then
            Call runSingle(cli, $"[{config.files(0).FileName}]Peak Table", tempTable, config.files(0))
        Else
            Call MyApplication.TaskQueue.AddToQueue(
                Sub()
                    Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)
                    Dim n As i32 = 0

                    AddHandler pipeline.SetMessage, AddressOf taskUI.ProgressMessage

                    Call taskUI.Running()
                    Call cli.__DEBUG_ECHO
                    Call pipeline.Run()

                    Call taskUI.ProgressMessage("Background task finished, loading data...")

                    Call Me.Invoke(Sub() runBatch(cli, $"[{tempTable.BaseName}]Peak Table", tempTable, taskUI, n))

                    Call taskUI.Finish()

                    Call MessageBox.Show($"Batch LC-MS deconvolution job done!" & vbCrLf & $"Found {n} LCMS ms1 ROI features inside your sample files.",
                                         "LCMS Deconvolution",
                                         MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Sub)
        End If
    End Sub

    Private Sub runBatch(cli As String, title As String, temptable As String, taskUI As TaskUI, n As i32)
        Dim data As xcms2() = temptable.LoadCsv(Of xcms2)
        Dim table = VisualStudio.ShowDocument(Of frmTableViewer)(title:=title)
        Dim sampleNames As String() = data.PropertyNames

        Call table.LoadTable(
            Sub(grid)
                grid.Columns.Add(NameOf(xcms2.ID), GetType(String))
                grid.Columns.Add(NameOf(xcms2.mz), GetType(Double))
                grid.Columns.Add(NameOf(xcms2.mzmin), GetType(Double))
                grid.Columns.Add(NameOf(xcms2.mzmax), GetType(Double))
                grid.Columns.Add(NameOf(xcms2.rt), GetType(Double))
                grid.Columns.Add(NameOf(xcms2.rtmin), GetType(Double))
                grid.Columns.Add(NameOf(xcms2.rtmax), GetType(Double))
                grid.Columns.Add(NameOf(xcms2.npeaks), GetType(Double))

                For Each name As String In sampleNames
                    grid.Columns.Add(name, GetType(Double))
                Next

                For Each item As xcms2 In data
                    Dim row As Object() = New Object(8 + sampleNames.Length - 1) {}

                    row(0) = item.ID
                    row(1) = item.mz
                    row(2) = item.mzmin
                    row(3) = item.mzmax
                    row(4) = item.rt
                    row(5) = item.rtmin
                    row(6) = item.rtmax
                    row(7) = item.npeaks

                    For i As Integer = 0 To sampleNames.Length - 1
                        row(i + 8) = item(sampleNames(i))
                    Next

                    Call grid.Rows.Add(row)
                Next
            End Sub)
    End Sub

    Private Sub runSingle(cli As String, title As String, tempTable As String, filepath As String)
        Dim rawdata As mzPack = Nothing
        Dim data As PeakFeature() = TaskProgress.LoadData(
            streamLoad:=Function(println)
                            Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

                            AddHandler pipeline.SetMessage, AddressOf println.SetInfo

                            Call cli.__DEBUG_ECHO
                            Call pipeline.Run()

                            Try
                                Using s = filepath.Open(FileMode.Open, [readOnly]:=True)
                                    rawdata = mzPack.ReadAll(s, skipMsn:=True, ignoreThumbnail:=True)
                                End Using
                            Catch ex As Exception

                            End Try

                            Return tempTable.LoadCsv(Of PeakFeature)
                        End Function,
            title:="Run Ms1 Deconvolution",
            info:="deconvolution..")

        Dim table = VisualStudio.ShowDocument(Of frmTableViewer)(title:=title)

        table.ViewRow = Sub(row)
                            Dim mz As Double = row("mz")
                            Call frmRawFeaturesList.ViewSingleTarget(mz, rawdata)
                        End Sub
        table.LoadTable(Sub(grid)
                            grid.Columns.Add(NameOf(PeakFeature.xcms_id), GetType(String))
                            grid.Columns.Add(NameOf(PeakFeature.mz), GetType(Double))
                            grid.Columns.Add(NameOf(PeakFeature.rt), GetType(Double))
                            grid.Columns.Add(NameOf(PeakFeature.rtmin), GetType(Double))
                            grid.Columns.Add(NameOf(PeakFeature.rtmax), GetType(Double))
                            grid.Columns.Add(NameOf(PeakFeature.maxInto), GetType(Double))
                            grid.Columns.Add(NameOf(PeakFeature.nticks), GetType(Double))
                            grid.Columns.Add(NameOf(PeakFeature.baseline), GetType(Double))
                            grid.Columns.Add(NameOf(PeakFeature.noise), GetType(Double))
                            grid.Columns.Add(NameOf(PeakFeature.area), GetType(Double))
                            grid.Columns.Add(NameOf(PeakFeature.integration), GetType(Double))
                            grid.Columns.Add(NameOf(PeakFeature.snRatio), GetType(Double))

                            For Each item As PeakFeature In data
                                Call grid.Rows.Add(
                                        item.xcms_id,
                                        item.mz.ToString("F4"),
                                        item.rt.ToString("F2"),
                                        item.rtmin.ToString("F2"),
                                        item.rtmax.ToString("F2"),
                                        item.maxInto.ToString("G3"),
                                        item.nticks,
                                        item.baseline.ToString("G3"),
                                        item.noise.ToString("G3"),
                                        item.area,
                                        item.integration.ToString("F2"),
                                        item.snRatio.ToString("F4")
                                    )
                            Next
                        End Sub)
    End Sub

    Private Sub MoleculeNetworkingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoleculeNetworkingToolStripMenuItem.Click
        Dim node = treeView1.SelectedNode

        If node Is Nothing OrElse TypeOf node.Tag IsNot MZWork.Raw Then
            Return
        Else
            Dim raw = DirectCast(node.Tag, MZWork.Raw)
            Dim mzpackfile As String = raw.cache
            Dim similarityCutoff As Double = ribbonItems.SpinnerSimilarity.DecimalValue
            Dim page As PageMzkitTools = MyApplication.mzkitRawViewer
            Dim getRaw As Func(Of Action(Of String), IEnumerable(Of PeakMs2)) =
                Iterator Function(println)
                    Dim mzpack As IEnumerable(Of ScanMS2) = raw.LoadMzpack(Sub(title, msg) println(msg)).GetMs2Scans

                    For Each ms2 As ScanMS2 In mzpack
                        Yield PageMzkitTools.GetMs2Peak(ms2, raw)
                    Next
                End Function

            Call TaskProgress.RunAction(
                run:=Sub(p)
                         Call page.MolecularNetworkingTool(p, similarityCutoff, getRaw)
                     End Sub,
                title:="Run molecular networking",
                info:="Initialized..."
            )
            Call MyApplication.host.mzkitMNtools.RefreshNetwork()
        End If
    End Sub

    Private Sub ChromatographyViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ChromatographyViewerToolStripMenuItem.Click
        Dim node = treeView1.SelectedNode

        If node Is Nothing OrElse TypeOf node.Tag IsNot MZWork.Raw Then
            Return
        End If

        Dim raw As MZWork.Raw = DirectCast(node.Tag, MZWork.Raw).LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache))
        Dim viewer = VisualStudio.ShowDocument(Of frmUntargettedViewer)()

        viewer.loadRaw(raw)
    End Sub

    Private Sub ScatterViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ScatterViewerToolStripMenuItem.Click
        Dim node = treeView1.SelectedNode

        If node Is Nothing OrElse TypeOf node.Tag IsNot MZWork.Raw Then
            Return
        End If

        Dim raw As MZWork.Raw = DirectCast(node.Tag, MZWork.Raw).LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache))
        Dim viewer = VisualStudio.ShowDocument(Of frmLCMSScatterViewer)()

        viewer.loadRaw(raw)
    End Sub

    Private Sub ExportAsMzXMLFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportAsMzXMLFilesToolStripMenuItem.Click
        Dim rawList As MZWork.Raw() = WindowModules.fileExplorer.GetSelectedRaws.ToArray

        If Not rawList.Any Then
            Call Workbench.Warning("no raw data file was selected.")
            Return
        End If

        Using dir As New FolderBrowserDialog With {.ShowNewFolderButton = True}
            If dir.ShowDialog = DialogResult.OK Then
                Dim folder = dir.SelectedPath

                Call TaskProgress.LoadData(
                    streamLoad:=Function(s As ITaskProgress)
                                    Call s.SetProgressMode()
                                    Call s.SetProgress(0)

                                    For i As Integer = 0 To rawList.Length - 1
                                        Dim raw As MZWork.Raw = rawList(i).LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache))
                                        Dim prog = i / rawList.Length * 100
                                        Dim mzPack As mzPack = raw.GetLoadedMzpack
                                        Dim filename = raw.source.BaseName
                                        Dim savefile = $"{folder}/{filename}.mzXML"

                                        Using file As Stream = savefile.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False),
                                            writer As New mzXMLWriter({}, {}, {}, file)

                                            Call writer.WriteData(mzPack.MS, print:=s.Echo)
                                            Call s.SetProgress(prog)
                                            Call s.SetInfo($"Save mzPack data as mzXML file [{i + 1}/{rawList.Length}]...")
                                        End Using
                                    Next

                                    Return True
                                End Function,
                    title:="export files...",
                    info:="export workspace files as mzXML outputs...",
                    canbeCancel:=True)
            End If
        End Using
    End Sub
End Class
