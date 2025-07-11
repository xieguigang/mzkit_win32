﻿#Region "Microsoft.VisualBasic::39d746b4c99fdce964e733247d3678cf, mzkit\src\mzkit\mzkit\application\Globals.vb"

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

'   Total Lines: 510
'    Code Lines: 379
' Comment Lines: 40
'   Blank Lines: 91
'     File Size: 18.71 KB


' Module Globals
' 
'     Properties: loadedSettings, Settings, workspace
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: CheckFormOpened, CurrentRawFile, FindRaws, GetColors, GetTotalCacheSize
'               GetXICMaxYAxis, loadBackground, LoadIonLibrary, LoadKEGG, LoadLipidMaps
'               (+2 Overloads) LoadRawFileCache, RawFileNodeTemplate
' 
'     Sub: AddRecentFileHistory, AddScript, InitExplorerUI, loadRawFile, loadRStudioScripts
'          loadWorkspace, SaveRawFileCache
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Net
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.sciexWiffReader
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.mzkit_win32.Configuration
Imports BioNovoGene.mzkit_win32.MSdata
Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.ServiceHub
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Distributions.BinBox
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.My.FrameworkInternal
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports Microsoft.VisualBasic.ValueTypes
Imports Mzkit_win32.BasicMDIForm
Imports MZWorkPack
Imports PipelineHost
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports Task
Imports Task.Container
Imports TaskStream
Imports WeifenLuo.WinFormsUI.Docking

Module Globals

    ''' <summary>
    ''' 这个是未进行任何工作区保存所保存的一个默认的临时文件的位置
    ''' </summary>
    Friend ReadOnly defaultWorkspace As String = App.LocalData & "/.defaultWorkspace"
    Friend ReadOnly localfs As Process

    Dim currentWorkspace As ViewerProject

    Public ReadOnly Property Settings As Settings

    Public Property loadedSettings As Boolean = False

    Public ReadOnly Property CurrentVersion As String
        Get
            Return GetType(Globals).Assembly.FromAssembly.AssemblyVersion
        End Get
    End Property

    Public ReadOnly Property BuildTime As Double
        Get
            Return GetType(Globals).Assembly.FromAssembly.BuiltTime.UnixTimeStamp
        End Get
    End Property

    Public ReadOnly Property workspace As ViewerProject
        Get
            Return currentWorkspace
        End Get
    End Property

    Public ReadOnly Property pubchemWebCache As String
        Get
            Return Settings.pubchemWebCache
        End Get
    End Property

    Public ReadOnly Property MSIBootstrapping As SampleBootstrapping
        Get
            If Settings.tissue_map Is Nothing OrElse Settings.tissue_map.bootstrapping Is Nothing Then
                Return SampleBootstrapping.GetDefault
            Else
                Return Settings.tissue_map.bootstrapping
            End If
        End Get
    End Property

    Sub New()
        Call ImageDriver.Register()

        Settings = Settings.GetConfiguration()
        localfs = New Process With {
            .StartInfo = New ProcessStartInfo With {
                .FileName = $"{App.HOME}/Rstudio/bin/Rserve.exe",
                .Arguments = $"--listen /wwwroot ""{AppEnvironment.getWebViewFolder}"" /port {Workbench.WebPort} --parent={App.PID}",
                .CreateNoWindow = True,
                .WindowStyle = ProcessWindowStyle.Hidden,
                .UseShellExecute = True
            }
        }

        Call localfs.Start()
        Call App.AddExitCleanHook(AddressOf shutdownHttpWeb)
        Call ServiceHub.Manager.Hub.Register(New Manager.Service With {
            .Name = "Local FileSystem",
            .Description = "Http services for host MZKit webview pages",
            .isAlive = True,
            .PID = localfs.Id,
            .Port = Workbench.WebPort,
            .Protocol = "HTTP 1.0",
            .StartTime = Now.ToString,
            .HouseKeeping = True，
            .CommandLine = Manager.Service.GetCommandLine(localfs)
        })

        Call FrameworkInternal.ConfigMemory(MemoryLoads.Max)
        Call LicenseFile.ApplyLicense()
        Call WorkStudio.LogCommandLine(localfs)
        Call ShowMSIRowScanSummary.HookIonReader(AddressOf BioNovoGene.Analytical.MassSpectrometry.Assembly.CheckMatrixBaseIon)

        ' initialize for external plugin module
        MSImagingServiceModule.m_StartEngine = Sub() Call MSIDataService.StartMSIService(Nothing)

        Pages.SetDocument(NameOf(SpectrumSearchPage), GetType(frmSpectrumSearch))
        Pages.SetDocument(NameOf(QuantificationLinearPage), GetType(frmTargetedQuantification))
        Pages.SetDocument(NameOf(MRMLibraryPage), GetType(frmMRMLibrary))

        DataTableViewer.HookTableViewer(Function() DirectCast(VisualStudio.ShowDocument(Of frmTableViewer), IDataTableViewer))
        SpectralViewerModule.HookViewer(AddressOf PageMzkitTools.ShowSpectral)
        SpectralViewerModule.HookAnalysis(AddressOf Module2.showMasssdiff)
        SpectralViewerModule.HookClusterLoader(AddressOf MSdata.ShowCluster)

        BaseHook.HookPlotColorSet(AddressOf Globals.GetColors)
        BaseHook.HookShowProperties(AddressOf VisualStudio.ShowProperties)
    End Sub

    Public Sub RegisterActions(println As Action(Of String))
        Call Actions.Register("KEGG Enrichment", New KEGGEnrichmentAction, println)
        Call Actions.Register("Formula Query", New FormulaQueryAction, println)
        Call Actions.Register("Peak Finding", New PeakFindingAction, println)
        Call Actions.Register("Peak List Annotation", New PeakAnnotationAction, println)
        Call Actions.Register("KEGG Stats", New KEGGStatsAction, println)
        Call Actions.Register("View 3D Scatter", New ViewScatter3DAction, println)
        Call Actions.Register("LCMS Scatter", New ViewLCMSScatter, println)
        Call Actions.Register("Metabonomics Analysis", New MetabonomicsAnalysisTool, println)
        Call Actions.Register("Linear Regression", New LinearRegressionAction, println)
        Call Actions.Register("PCA Analysis", New PCAAction, println)
    End Sub

    Private Sub shutdownHttpWeb()
        Try
            Dim url = $"https://127.0.0.1:{Workbench.WebPort}/ctrl/kill"
            Dim httpRequest = CType(WebRequest.Create(url), HttpWebRequest)

            httpRequest.Method = "OPTIONS"
            httpRequest.Headers("Access-Control-Request-Method") = "POST"
            httpRequest.Headers("Access-Control-Request-Headers") = "content-type"

            Dim httpResponse = CType(httpRequest.GetResponse, HttpWebResponse)

            Using streamReader = New StreamReader(httpResponse.GetResponseStream())
                Call Console.WriteLine(streamReader.ReadToEnd)
            End Using

            Call localfs.Kill()
            Console.WriteLine(httpResponse.StatusCode)
        Catch ex As Exception

        End Try
    End Sub

    Public Function LoadChEBI(println As Action(Of String), mode As String(), mzdiff As Tolerance) As MSSearch(Of MetaboliteAnnotation)
        Dim key As String = $"[{mzdiff.ToString}]{mode.JoinBy(",")}"
        Dim adducts = mode.Select(Function(type) Provider.ParseAdductModel(type)).ToArray

        Static dataPack As MetaboliteAnnotation() = KEGGRepo.RequestChebi
        Static cache As New Dictionary(Of String, MSSearch(Of MetaboliteAnnotation))

        Return cache.ComputeIfAbsent(key,
            lazyValue:=Function()
                           Return MSSearch(Of MetaboliteAnnotation).CreateIndex(dataPack, adducts, mzdiff)
                       End Function)
    End Function

    Public Function LoadMetabolights(println As Action(Of String), mode As String(), mzdiff As Tolerance) As MSSearch(Of MetaboliteAnnotation)
        Dim key As String = $"[{mzdiff.ToString}]{mode.JoinBy(",")}"
        Dim adducts = mode.Select(Function(type) Provider.ParseAdductModel(type)).ToArray

        Static dataPack As MetaboliteAnnotation() = KEGGRepo.RequestMetabolights
        Static cache As New Dictionary(Of String, MSSearch(Of MetaboliteAnnotation))

        Return cache.ComputeIfAbsent(key,
            lazyValue:=Function()
                           Return MSSearch(Of MetaboliteAnnotation).CreateIndex(dataPack, adducts, mzdiff)
                       End Function)
    End Function

    Public Function LoadHMDB(println As Action(Of String), mode As String(), mzdiff As Tolerance) As MSSearch(Of MetaboliteAnnotation)
        Dim key As String = $"[{mzdiff.ToString}]{mode.JoinBy(",")}"
        Dim adducts = mode.Select(Function(type) Provider.ParseAdductModel(type)).ToArray

        Static dataPack As MetaboliteAnnotation() = KEGGRepo.RequestHmdb
        Static cache As New Dictionary(Of String, MSSearch(Of MetaboliteAnnotation))

        Return cache.ComputeIfAbsent(key,
            lazyValue:=Function()
                           Return MSSearch(Of MetaboliteAnnotation).CreateIndex(dataPack, adducts, mzdiff)
                       End Function)
    End Function

    Public Function LoadLipidMaps(println As Action(Of String), mode As String(), mzdiff As Tolerance) As MSSearch(Of LipidMaps.MetaData)
        Dim key As String = $"[{mzdiff.ToString}]{mode.JoinBy(",")}"
        Dim adducts = mode.Select(Function(type) Provider.ParseAdductModel(type)).ToArray

        Static dataPack As LipidMaps.MetaData() = KEGGRepo.RequestLipidMaps
        Static cache As New Dictionary(Of String, MSSearch(Of LipidMaps.MetaData))

        Return cache.ComputeIfAbsent(key,
            lazyValue:=Function()
                           Return MSSearch(Of LipidMaps.MetaData).CreateIndex(dataPack, adducts, mzdiff)
                       End Function)
    End Function

    Public Function LoadKEGG(println As Action(Of String), mode As String(), mzdiff As Tolerance) As MSJointConnection
        Static background As Background = loadBackground()
        Static compounds As KEGGCompound() = CompoundSolver.Wraps(KEGGRepo.RequestKEGGCompounds).ToArray
        Static cache As New Dictionary(Of String, CompoundSolver)

        Dim key As String = $"[{mzdiff.ToString}]{mode.JoinBy(",")}"
        Dim adducts = mode.Select(Function(type) Provider.ParseAdductModel(type)).ToArray
        Dim handler As CompoundSolver = cache.ComputeIfAbsent(key,
            lazyValue:=Function()
                           Return CompoundSolver.CreateIndex(compounds, adducts, mzdiff)
                       End Function)

        Return New MSJointConnection(handler, background)
    End Function

    Public Sub AddRecentFileHistory(file As String)
        Settings.recentFiles = {file}.JoinIterates(Settings.recentFiles).Distinct.ToArray
        Settings.Save()
    End Sub

    Public Function GetColors() As String
        Return Globals.Settings.viewer.colorSet.JoinBy(",")
    End Function

    <Extension>
    Public Sub SaveRawFileCache(explorer As TreeView, progress As Action(Of String))
        Dim files As New List(Of Raw)
        Dim scripts As New List(Of String)

        If explorer.Nodes.Count > 0 Then
            Dim rawFileNodes = explorer.Nodes(Scan0)

            For Each node As TreeNode In rawFileNodes.Nodes
                files.Add(node.Tag)
                progress(files.Last.source)
                Application.DoEvents()
            Next

            If explorer.Nodes.Count > 1 Then
                For Each node As TreeNode In explorer.Nodes(1).Nodes
                    scripts.Add(DirectCast(node.Tag, String).GetFullPath)
                Next
            End If
        End If

        Dim opened As New List(Of String)
        Dim unsaved As New List(Of NamedValue)

        For Each doc As IDockContent In MyApplication.host.m_dockPanel.Documents
            If TypeOf doc Is frmRScriptEdit Then
                opened.Add(DirectCast(doc, frmRScriptEdit).scriptFile)

                If DirectCast(doc, frmRScriptEdit).IsUnsaved Then
                    Call unsaved.Add(New NamedValue With {
                        .name = DirectCast(doc, frmRScriptEdit).scriptFile,
                        .text = DirectCast(doc, frmRScriptEdit).ScriptText
                    })
                End If
            End If
        Next

        Call currentWorkspace.SaveAs(files, scripts, opened, unsaved).Save(defaultWorkspace)
    End Sub

    <Extension>
    Public Iterator Function FindRaws(explorer As TreeView, sourceName As String) As IEnumerable(Of Raw)
        For Each node As TreeNode In explorer.Nodes.Item(0).Nodes
            Dim raw As Raw = DirectCast(node.Tag, Raw)

            If raw.source.FileName = sourceName Then
                Yield raw
            End If
        Next
    End Function

    <Extension>
    Public Function GetTotalCacheSize(explorer As TreeNode) As String
        Dim size As Double

        For Each node As TreeNode In explorer.Nodes
            size += DirectCast(node.Tag, Raw).GetCacheFileSize
        Next

        If size = 0.0 Then
            Return "0 KB"
        Else
            Return Lanudry(size)
        End If
    End Function

    Friend sharedProgressUpdater As Action(Of String)

    Public Sub loadWorkspace(mzwork As String, fromStartup As Boolean)
        If Not fromStartup Then
            If MessageBox.Show("Load new workspace will overrides current MZKit workspace, continute to process?",
                               "Load New Workspace",
                               MessageBoxButtons.OKCancel,
                               MessageBoxIcon.Question) = DialogResult.Cancel Then
                Return
            End If
        End If

        Dim work As WorkspaceFile = TaskProgress.LoadData(
            streamLoad:=Function(msg) MZWorkPack.ImportWorkspace(mzwork, msg.Echo),
            info:="Loading MZKit workspace..."
        )
        Dim project As New ViewerProject With {
            .FilePath = Globals.defaultWorkspace,
            .work = work
        }
        Dim explorer = WindowModules.fileExplorer

        sharedProgressUpdater = Sub(text) Workbench.StatusMessage(text)

        Call project.LoadRawFileCache(
            explorer:=explorer.treeView1,
            rawMenu:=explorer.ctxMenuFiles,
            targetRawMenu:=explorer.ctxMenuRawFile,
            scriptMenu:=explorer.ctxMenuScript
        )
        Call MyApplication.host.showStatusMessage("Ready!")
    End Sub

    ''' <summary>
    ''' Make initialization of the tree menu UI, just add the empty folder node
    ''' </summary>
    ''' <param name="explorer"></param>
    ''' <param name="rawMenu"></param>
    ''' <param name="scriptMenu"></param>
    ''' <param name="scripts"></param>
    ''' <param name="rawFiles"></param>
    Public Sub InitExplorerUI(explorer As TreeView, rawMenu As ContextMenuStrip, scriptMenu As ContextMenuStrip,
                              Optional ByRef scripts As TreeNode = Nothing,
                              Optional ByRef rawFiles As TreeNode = Nothing)

        scripts = New TreeNode("R# Automation") With {
            .ImageIndex = 1,
            .SelectedImageIndex = 1,
            .StateImageIndex = 1,
            .ContextMenuStrip = scriptMenu
        }
        rawFiles = New TreeNode("Raw Data Files") With {
            .ImageIndex = 0,
            .StateImageIndex = 0,
            .SelectedImageIndex = 0,
            .ContextMenuStrip = rawMenu
        }

        explorer.Nodes.Clear()
        explorer.Nodes.Add(rawFiles)
        explorer.Nodes.Add(scripts)
    End Sub

    Public Function RawFileNodeTemplate(raw As Raw, targetRawMenu As ContextMenuStrip) As TreeNode
        Dim sourcePath As String = raw.source.GetFullPath(throwEx:=False)
        Dim name As String = raw.source.FileName

        If name.Contains(vbCr) OrElse name.Contains(vbLf) Then
            name = name.LineTokens.Last

            If name.Length > 24 Then
                name = "..." & name.Substring(name.Length - 21)
            End If
        End If

        Dim rawFileNode As New TreeNode(name) With {
            .Checked = True,
            .Tag = raw,
            .ImageIndex = 2,
            .SelectedImageIndex = 2,
            .StateImageIndex = 2,
            .ContextMenuStrip = targetRawMenu,
            .ToolTipText = If(sourcePath.StringEmpty, "source file is missing!", sourcePath)
        }

        Return rawFileNode
    End Function

    ''' <summary>
    ''' two root nodes:
    ''' 
    ''' 1. Raw Data Files
    ''' 2. R# Automation
    ''' </summary>
    ''' <param name="explorer"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LoadRawFileCache(files As ViewerProject,
                                     explorer As TreeView,
                                     rawMenu As ContextMenuStrip,
                                     targetRawMenu As ContextMenuStrip,
                                     scriptMenu As ContextMenuStrip) As Integer
        Dim i As Integer
        Dim scripts, rawFiles As TreeNode

#Disable Warning
        Call InitExplorerUI(explorer, rawMenu, scriptMenu, scripts, rawFiles)
#Enable Warning

        For Each raw As Raw In files.GetRawDataFiles
            Dim rawFileNode = RawFileNodeTemplate(raw, targetRawMenu)
            Dim name As String = rawFileNode.Text

            Call sharedProgressUpdater($"[Raw File Viewer] Loading {name}...")

            rawFiles.Nodes.Add(rawFileNode)
            'rawFileNode.addRawFile(raw, True, True)
            rawFileNode.Checked = False

            i += 1
        Next

        currentWorkspace = files

        If files.GetAutomationScripts.SafeQuery.Count > 0 Then
            For Each script As String In files.GetAutomationScripts
                Dim fileNode As New TreeNode(script.FileName) With {
                    .Checked = False,
                    .Tag = script,
                    .ImageIndex = 3,
                    .StateImageIndex = 3,
                    .SelectedImageIndex = 3,
                    .ContextMenuStrip = scriptMenu,
                    .ToolTipText = script.GetFullPath(throwEx:=False)
                }

                scripts.Nodes.Add(fileNode)
            Next
        End If

        Call loadRStudioScripts(explorer, scriptMenu)

        Return i
    End Function

    ''' <summary>
    ''' two root nodes:
    ''' 
    ''' 1. Raw Data Files
    ''' 2. R# Automation
    ''' </summary>
    ''' <param name="explorer"></param>
    ''' <param name="defaultWorkspace"></param>
    ''' <returns></returns>
    <Extension>
    Public Function LoadRawFileCache(explorer As TreeView,
                                     rawMenu As ContextMenuStrip,
                                     targetRawMenu As ContextMenuStrip,
                                     scriptMenu As ContextMenuStrip,
                                     Optional defaultWorkspace As String = Nothing) As Integer

        If defaultWorkspace.StringEmpty Then
            defaultWorkspace = Globals.defaultWorkspace
        End If

        If Not defaultWorkspace.FileExists Then
            currentWorkspace = New ViewerProject With {
                .FilePath = defaultWorkspace
            }

            Return 0
        Else
            Call sharedProgressUpdater("Load raw file list...")
        End If

        Dim files As ViewerProject = ViewerProject.LoadWorkspace(defaultWorkspace, sharedProgressUpdater)
        Dim i As Integer = files.LoadRawFileCache(explorer, rawMenu, targetRawMenu, scriptMenu)

        Return i
    End Function

    Private Sub loadRStudioScripts(explorer As TreeView, scriptMenu As ContextMenuStrip)
        Dim scripts As New TreeNode("R# Studio") With {
            .ImageIndex = 1,
            .SelectedImageIndex = 1,
            .StateImageIndex = 1,
            .ContextMenuStrip = scriptMenu
        }

        Dim folder As String = $"{App.HOME}/Rstudio/R"

        If Not folder.DirectoryExists Then
            ' development test
            folder = $"{App.HOME}/../../src/mzkit/setup/demo_script/"
        End If

        Call explorer.Nodes.Add(scripts)
        Call AddScript(scripts, dir:=folder, scriptMenu:=scriptMenu)
    End Sub

    Private Sub AddScript(folder As TreeNode, dir As String, scriptMenu As ContextMenuStrip)
        For Each subfolder As String In dir.ListDirectory
            Dim fileNode As New TreeNode(subfolder.DirectoryName) With {
                .Checked = False,
                .Tag = Nothing,
                .ImageIndex = 1,
                .StateImageIndex = 1,
                .SelectedImageIndex = 1,
                .ContextMenuStrip = scriptMenu
            }

            folder.Nodes.Add(fileNode)
            AddScript(fileNode, subfolder, scriptMenu)
        Next

        For Each script As String In dir.EnumerateFiles("*.R")
            Dim fileNode As New TreeNode(script.FileName) With {
                .Checked = False,
                .Tag = script,
                .ImageIndex = 3,
                .StateImageIndex = 3,
                .SelectedImageIndex = 3,
                .ContextMenuStrip = scriptMenu
            }

            folder.Nodes.Add(fileNode)
        Next
    End Sub

    ''' <summary>
    ''' 加载原始数据文件之中的ms1和ms2 scan树
    ''' 
    ''' ```
    ''' + ms1
    '''    + ms2
    '''    + ms2
    ''' + ms1
    '''    + ms2
    ''' ```
    ''' </summary>
    ''' <param name="rawFileNode"></param>
    ''' <param name="raw"></param>
    <Extension>
    Public Sub loadRawFile(rawFileNode As TreeView, raw As Raw, ByRef hasUVscans As Boolean, rtmin As Double, rtmax As Double)
        Dim [class] As FileApplicationClass = FileApplicationClass.LCMS
        Dim mzpack_file As String = Nothing

        If raw.isLoaded Then
            [class] = raw.GetLoadedMzpack.Application
            mzpack_file = raw.cache
        ElseIf raw.cacheFileExists Then
            Using file As Stream = raw.cache.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                [class] = file.GetMSApplication
                mzpack_file = raw.cache
            End Using
        ElseIf raw.source.FileExists AndAlso raw.source.ExtensionSuffix("mzPack") Then
            Using file As Stream = raw.source.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                [class] = file.GetMSApplication
                mzpack_file = raw.source
            End Using
        End If

        If [class] = FileApplicationClass.MSImaging AndAlso Not mzpack_file.StringEmpty Then
            If MessageBox.Show("It seems that current raw data file is used for MS-imaging, it is recommended that open in MS-Imaging viewer. Open this file in MS-imaging viewer?",
                               "Load MzPack Raw Data",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Warning) = DialogResult.Yes Then
                ' open in ms-imaging viewer
                Call RibbonEvents.OpenMSIRaw(mzpack_file, debug:=False)
                Return
            Else
                Call MessageBox.Show("This operation may takes a very long time to load", "Load MS-imaging data as LC-MS data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
        End If

        rawFileNode.Nodes.Clear()

        If Not raw.isLoaded Then
            Call RunSlavePipeline.HookProgress(
                Sub(pct, msg)
                    Call Application.DoEvents()
                    Call Workbench.StatusMessage($"{pct}/{msg}...")
                End Sub)
            Call raw.LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache))

            If raw.GetLoadedMzpack.MS.IsNullOrEmpty Then
                If Not raw.source.FileExists Then
                    Call MessageBox.Show($"The MS raw data file '{raw.source}' is not exists on your filesystem.", "Load Raw Data Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If
            End If
        End If

        For Each scan As ScanMS1 In raw.GetMs1Scans.Where(Function(t) t.rt >= rtmin AndAlso t.rt <= rtmax)
            Dim scanNode As New TreeNode(scan.scan_id) With {
                .Tag = scan,
                .ImageIndex = 0
            }

            rawFileNode.Nodes.Add(scanNode)

            For Each ms2 As ScanMS2 In scan.products.SafeQuery
                Dim productNode As New TreeNode(ms2.scan_id) With {
                    .Tag = ms2,
                    .ImageIndex = 1,
                    .SelectedImageIndex = 1
                }

                scanNode.Nodes.Add(productNode)
            Next
        Next

        If Not (rtmin < 0 OrElse rtmax > 10 ^ 100) Then
            Call Workbench.StatusMessage($"Filter {rawFileNode.Nodes.Count} ms scan with RT range [{rtmin} sec, {rtmax} sec]!")
        End If

        Dim UVscans As UVScan() = raw _
            .GetUVscans _
            .Where(Function(t)
                       Return t.scan_time >= rtmin AndAlso t.scan_time <= rtmax
                   End Function) _
            .ToArray

        If Not UVscans.IsNullOrEmpty Then
            WindowModules.UVScansList.DockState = DockState.DockLeftAutoHide
            WindowModules.UVScansList.Win7StyleTreeView1.Nodes.Clear()
            WindowModules.UVScansList.Clear()
            hasUVscans = True
            For Each scan As DataBinBox(Of UVScan) In CutBins _
                .FixedWidthBins(UVscans, 99, Function(x) x.scan_time) _
                .Where(Function(b) b.Count > 0)

                Dim scan_time = scan.Sample
                Dim spanNode As New TreeNode With {
                    .Text = $"scan_time: {CInt(scan_time.min)} ~ {CInt(scan_time.max)} sec"
                }

                For Each spectrum In scan.Raw
                    Call New TreeNode With {
                        .Text = spectrum.ToString,
                        .ImageIndex = 1,
                        .SelectedImageIndex = 1,
                        .Tag = spectrum
                    }.DoCall(AddressOf spanNode.Nodes.Add)
                Next

                Call WindowModules.UVScansList.Win7StyleTreeView1.Nodes.Add(spanNode)
            Next
        Else
            hasUVscans = False
        End If
    End Sub

    ''' <summary>
    ''' 这个函数总是返回当前选中的节点的文件根节点相关的数据
    ''' </summary>
    ''' <param name="explorer"></param>
    ''' <returns></returns>
    <Extension>
    Public Function CurrentRawFile(explorer As TreeView) As (raw As Raw, tree As TreeNode)
        Dim node = explorer.SelectedNode

        If node Is Nothing Then
            Return Nothing
        ElseIf TypeOf node.Tag Is Raw Then
            Return (DirectCast(node.Tag, Raw), node)
        Else
            Return (DirectCast(node.Parent.Tag, Raw), node.Parent)
        End If
    End Function

    Public Function CheckFormOpened(form As Form) As Boolean
        For i As Integer = 0 To Application.OpenForms.Count - 1
            If Application.OpenForms.Item(i) Is form Then
                Return True
            End If
        Next

        Return False
    End Function

    <Extension>
    Public Function GetXICMaxYAxis(raw As Raw) As Double
        Dim XIC As Double() = raw _
            .GetMs2Scans _
            .Select(Function(a) a.intensity) _
            .ToArray

        If XIC.Length = 0 Then
            Return 0
        Else
            Return XIC.Max
        End If
    End Function

    Public Function LoadIonLibrary() As IonLibrary
        If Not Globals.Settings.MRMLibfile.FileExists Then
            Globals.Settings.MRMLibfile = New Configuration.Settings().MRMLibfile
        End If

        Dim ionsLib As New IonLibrary(Globals.Settings.MRMLibfile.LoadCsv(Of IonPair))

        Return ionsLib
    End Function
End Module
