#Region "Microsoft.VisualBasic::a873fc8824d20329f79e710db5a10d19, mzkit\src\mzkit\mzkit\application\RibbonEvents.vb"

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

'   Total Lines: 424
'    Code Lines: 327
' Comment Lines: 10
'   Blank Lines: 87
'     File Size: 21.72 KB


' Module RibbonEvents
' 
'     Properties: ribbonItems
' 
'     Function: GetQuantizationThreshold, getWelcomeScript
' 
'     Sub: _recentItems_ExecuteEvent, _uiCollectionChangedEvent_ChangedEvent, About_Click, AddHandlers, CombineRowScanTask
'          CopyMatrix, CopyPlotImage, CopyProperties, CreateNewScript, ExitToolsStripMenuItem_Click
'          NavBack_Click, openCmd, OpenMSIRaw, OpenWorkspace, resetLayout
'          RunCurrentScript, ShowExplorer, showHelp, showLoggingWindow, showMsImaging
'          ShowProperties, showRTerm, ShowSearchList, ShowSettings, showStartPage
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.BrukerDataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports Mzkit_win32.BasicMDIForm.RibbonLib.Controls
Imports Mzkit_win32.MatrixViewer
Imports RibbonLib
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports Task
Imports Task.Container
Imports TaskStream
Imports WeifenLuo.WinFormsUI.Docking

Module RibbonEvents

    Public ReadOnly Property ribbonItems As RibbonItems

    ''' <summary>
    ''' winform application framework could not handling the the ribbon native code error,
    ''' use this function for handling the native to clr interop error
    ''' </summary>
    ''' <param name="btn">interop to ribbon native code</param>
    ''' <param name="handler">clr event code</param>
    Private Sub HookRibbon(ByRef btn As Controls.RibbonButton, handler As EventHandler(Of ExecuteEventArgs))
        Dim native = btn
        Dim clr_exec_hook As EventHandler(Of ExecuteEventArgs) =
            Sub(sender, evt)
                Try
                    Call handler(sender, evt)
                Catch ex As Exception
                    Call App.LogException(New Exception($"ribbon tracer: {native.Name} ({native.Label})", ex))
                    Call MessageBox.Show($"Error while execute ribbon event caller: {native.Label} ({ex.Message})", "Function Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Call Workbench.Warning($"Error while execute ribbon event caller: {native.Label} ({ex.Message})")
                End Try
            End Sub

        ' hook the ribbon native code with
        ' clr event code
        AddHandler btn.ExecuteEvent, clr_exec_hook
    End Sub

    <Extension>
    Public Sub AddHandlers(ribbonItems As RibbonItems)
        _ribbonItems = ribbonItems

        Call HookRibbon(ribbonItems.ButtonExit, AddressOf ExitToolsStripMenuItem_Click)
        Call HookRibbon(ribbonItems.ButtonOpenRaw, AddressOf WindowModules.OpenFile)
        Call HookRibbon(ribbonItems.ButtonImportsRawFiles, AddressOf MyApplication.host.ImportsFiles)
        Call HookRibbon(ribbonItems.ButtonAbout, AddressOf About_Click)
        Call HookRibbon(ribbonItems.ButtonPageNavBack, AddressOf NavBack_Click)
        Call HookRibbon(ribbonItems.ButtonNew, AddressOf CreateNewScript)

        Call HookRibbon(ribbonItems.TweaksImage, AddressOf MyApplication.host.mzkitTool.ShowPlotTweaks)
        Call HookRibbon(ribbonItems.ShowProperty, AddressOf ShowProperties)
        Call HookRibbon(ribbonItems.ButtonCopyProperties, AddressOf CopyProperties)
        Call HookRibbon(ribbonItems.ButtonCopyMatrix, AddressOf CopyMatrix)
        Call HookRibbon(ribbonItems.ButtonCopyPlot, AddressOf CopyPlotImage)

        Call HookRibbon(ribbonItems.ButtonPeakFinding, Sub(sender, e) Call CreatePeakFinding())
        Call HookRibbon(ribbonItems.ButtonMzCalculator, Sub(sender, e) Call MyApplication.host.ShowPage(MyApplication.host.mzkitCalculator))
        Call HookRibbon(ribbonItems.ButtonSettings, AddressOf ShowSettings)
        Call HookRibbon(ribbonItems.ButtonMzSearch, Sub(sender, e) Call MyApplication.host.ShowPage(MyApplication.host.mzkitSearch))
        Call HookRibbon(ribbonItems.ButtonRsharp, AddressOf showRTerm)

        Call HookRibbon(ribbonItems.ButtonDropA, Sub(sender, e) MyApplication.host.ShowPage(MyApplication.host.mzkitTool))
        Call HookRibbon(ribbonItems.ButtonDropB, Sub(sender, e) MyApplication.host.ShowPage(MyApplication.host.mzkitCalculator))
        Call HookRibbon(ribbonItems.ButtonFormulaSearch, Sub(sender, e) MyApplication.host.ShowPage(MyApplication.host.mzkitSearch))
        Call HookRibbon(ribbonItems.ButtonDropD, Sub(sender, e) MyApplication.host.ShowPage(MyApplication.host.mzkitMNtools))
        Call HookRibbon(ribbonItems.ButtonShowSpectrumSearchPage, Sub(sender, e) Call New frmSpectrumSearch().Show(MyApplication.host.DockPanel))

        Call HookRibbon(ribbonItems.ButtonCalculatorExport, Sub(sender, e) Call MyApplication.host.mzkitCalculator.ExportToolStripMenuItem_Click())
        Call HookRibbon(ribbonItems.ButtonExactMassSearchExport, Sub(sender, e) Call MyApplication.host.mzkitTool.ExportExactMassSearchTable())
        Call HookRibbon(ribbonItems.ButtonSave, Sub(sender, e) Call MyApplication.host.saveCurrentFile())
        Call HookRibbon(ribbonItems.ButtonNetworkExport, Sub(sender, e) Call MyApplication.host.mzkitMNtools.saveNetwork())
        Call HookRibbon(ribbonItems.ButtonFormulaSearchExport, Sub(sender, e) Call MyApplication.host.mzkitSearch.SaveSearchResultTable())

        Call HookRibbon(ribbonItems.ButtonBioDeep, Sub(sender, e) Call Process.Start("http://www.biodeep.cn/"))
        Call HookRibbon(ribbonItems.ButtonLicense, Sub(sender, e) Call New frmLicense().ShowDialog())

        Call HookRibbon(ribbonItems.ButtonExportImage, Sub(sender, e) Call MyApplication.host.mzkitTool.SaveImageToolStripMenuItem_Click())
        Call HookRibbon(ribbonItems.ButtonExportMatrix, Sub(sender, e) Call MyApplication.host.mzkitTool.SaveMatrixToolStripMenuItem_Click())

        Call HookRibbon(ribbonItems.ButtonLayout1, Sub(sender, e) Call MyApplication.host.mzkitTool.SaveImageToolStripMenuItem_Click())
        Call HookRibbon(ribbonItems.ButtonLayout2, Sub(sender, e) Call MyApplication.host.mzkitTool.SaveMatrixToolStripMenuItem_Click())

        Call HookRibbon(ribbonItems.ButtonShowStartPage, AddressOf showStartPage)
        Call HookRibbon(ribbonItems.ButtonShowLogWindow, AddressOf showLoggingWindow)

        Call HookRibbon(ribbonItems.ButtonShowExplorer, AddressOf ShowExplorer)
        Call HookRibbon(ribbonItems.ButtonShowSearchList, AddressOf ShowSearchList)
        Call HookRibbon(ribbonItems.ButtonShowProperties, AddressOf ShowProperties)

        Call HookRibbon(ribbonItems.ButtonShowPlotViewer, Sub(sender, e) Call MyApplication.host.mzkitTool.ShowTabPage(MyApplication.host.mzkitTool.TabPage5))
        Call HookRibbon(ribbonItems.ButtonShowMatrixViewer, Sub(sender, e) Call MyApplication.host.mzkitTool.ShowTabPage(MyApplication.host.mzkitTool.TabPage6))
        Call HookRibbon(ribbonItems.ButtonNetworkRender, Sub(sender, e) Call MyApplication.host.mzkitMNtools.RenderNetwork())
        Call HookRibbon(ribbonItems.ButtonRefreshNetwork, Sub(sender, e) Call MyApplication.host.mzkitMNtools.RefreshNetwork())

        Call HookRibbon(ribbonItems.ButtonRunScript, AddressOf RunCurrentScript)
        Call HookRibbon(ribbonItems.ButtonSaveScript, AddressOf MyApplication.host.saveCurrentScript)

        AddHandler ribbonItems.HelpButton.ExecuteEvent, AddressOf showHelp
        AddHandler ribbonItems.RecentItems.ExecuteEvent, AddressOf _recentItems_ExecuteEvent

        Call HookRibbon(ribbonItems.ButtonTIC, Sub(sender, e) Call MyApplication.host.mzkitTool.TIC(isBPC:=False))
        Call HookRibbon(ribbonItems.ButtonBPC, Sub(sender, e) Call MyApplication.host.mzkitTool.TIC(isBPC:=True))
        Call HookRibbon(ribbonItems.ButtonXIC, AddressOf WindowModules.rawFeaturesList.ShowXICToolStripMenuItem_Click)

        Call HookRibbon(ribbonItems.ButtonResetLayout, AddressOf resetLayout)

        Call HookRibbon(ribbonItems.ButtonSingleCellsViewer, AddressOf showSingleCells)
        Call HookRibbon(ribbonItems.ButtonOpenSingleCellsRawDataFile, AddressOf OpenSingleCellsRaw)

        Call HookRibbon(ribbonItems.ButtonMsImaging, AddressOf showMsImaging)
        Call HookRibbon(ribbonItems.ButtonOpenMSIRaw, AddressOf OpenMSIRaw)
        Call HookRibbon(ribbonItems.ButtonMSIRowScans, AddressOf CombineRowScanTask)
        Call HookRibbon(ribbonItems.ButtonImportsSCiLSLab, AddressOf ImportsSCiLSLab)
        Call HookRibbon(ribbonItems.ButtonMsDemo, Sub() WindowModules.msDemo.ShowPage())
        ' Call HookRibbon(ribbonItems.Targeted, Sub() Call ConnectToBioDeep.OpenAdvancedFunction(AddressOf VisualStudio.ShowSingleDocument(Of frmTargetedQuantification))
        Call HookRibbon(ribbonItems.Targeted, Sub() Call VisualStudio.ShowSingleDocument(Of frmTargetedQuantification)())

        Call HookRibbon(ribbonItems.MRMLibrary, Sub() Call VisualStudio.ShowSingleDocument(Of frmMRMLibrary)(Nothing))
        Call HookRibbon(ribbonItems.QuantifyIons, Sub() Call VisualStudio.ShowSingleDocument(Of frmQuantifyIons)(Nothing))
        Call HookRibbon(ribbonItems.GCxGCViewer, Sub() Call VisualStudio.ShowSingleDocument(Of frmGCxGCViewer)(Nothing))

        Call HookRibbon(ribbonItems.LogInBioDeep, Sub() Call New frmLogin().ShowDialog())

        Call HookRibbon(ribbonItems.ButtonInstallMzkitPackage, AddressOf VisualStudio.InstallInternalRPackages)
        Call HookRibbon(ribbonItems.ShowGCMSExplorer, Sub() Call VisualStudio.Dock(WindowModules.GCMSPeaks, DockState.DockLeft))
        Call HookRibbon(ribbonItems.ShowMRMExplorer, Sub() Call VisualStudio.Dock(WindowModules.MRMIons, DockState.DockLeft))

        Call HookRibbon(ribbonItems.Tutorials, Sub() Call openVideoList())
        Call HookRibbon(ribbonItems.ButtonViewSMILES, Sub() Call VisualStudio.ShowSingleDocument(Of frmSMILESViewer)())
        Call HookRibbon(ribbonItems.ButtonPluginManager, Sub() Call VisualStudio.ShowSingleDocument(Of frmPluginMgr)())

        Call HookRibbon(ribbonItems.AdjustParameters, Sub() Call VisualStudio.Dock(WindowModules.parametersTool, DockState.DockRight))
        Call HookRibbon(ribbonItems.ImportsMzwork, Sub() Call OpenWorkspace())

        Call HookRibbon(ribbonItems.ButtonDevTools, Sub() Call openCmd())
        Call HookRibbon(ribbonItems.DOIReference, Sub() Call New frmDOI().ShowDialog())
        Call HookRibbon(ribbonItems.ButtonSystemDiagnosis, Sub() Call CollectSystemInformation())

        Call HookRibbon(ribbonItems.ButtonCFMIDTool, Sub() Call OpenCFMIDTool(Nothing))
        Call HookRibbon(ribbonItems.MsconvertGUI, Sub() Call openMsconvertTool())
        Call HookRibbon(ribbonItems.View3DMALDI, Sub() Call open3dMALDIViewer())

        Call HookRibbon(ribbonItems.ButtonOpenServicesMgr, Sub() Call VisualStudio.ShowSingleDocument(Of frmServicesManager)())

        Call HookRibbon(ribbonItems.ButtonImport10x_genomics, Sub() Call ConvertH5ad())
        Call HookRibbon(ribbonItems.ButtonRenderUMAPScatter, Sub() Call PageMoleculeNetworking.RunUMAP())

        Call HookRibbon(ribbonItems.ButtonSearchPubChem, Sub() Call openShowSearchPubChemLCMS())

        Call HookRibbon(ribbonItems.ButtonOpenVirtualSlideFile, Sub() Call openSlideFile())
        Call HookRibbon(ribbonItems.ButtonOpenTableTool, Sub() Call OpenExcelTableFile2())
        Call HookRibbon(ribbonItems.OpenIonsLibrary, Sub() Call openIonLibrary())
        Call HookRibbon(ribbonItems.ButtonOpenLCMSWorkbench, Sub() Call openLCMSWorkbench())
        Call HookRibbon(ribbonItems.ButtonOpenWorkspace, Sub() Call openLCMSWorkspace())
        Call HookRibbon(ribbonItems.ButtonViewUntargetedScatter, Sub() Call viewUntargettedScatter())

        Call HookRibbon(ribbonItems.ButtonVenn, Sub() Call VisualStudio.ShowDocument(Of frmVennTools)(title:="Venn Plot Tool"))
        Call HookRibbon(ribbonItems.ButtonViewMRI, Sub() Call openMRIRaster())

        Call HookRibbon(ribbonItems.ButtonMSIDebugger, Sub() Call Debugger.MSI.Run())
        Call HookRibbon(ribbonItems.ButtonOpenPeakFeatures, Sub() Call loadPeakFeatures())

        LCMSViewerModule.lcmsViewerhHandle = AddressOf openLcmsScatter
    End Sub

    Sub New()
        ExportApis._openMSImagingFile = AddressOf OpenMSIRaw
        ExportApis._openMSImagingViewer = AddressOf showMsImaging
        ExportApis._openCFMIDTool = AddressOf OpenCFMIDTool
    End Sub

    Private Sub loadPeakFeatures()
        Using file As New OpenFileDialog With {.Filter = "Peak feature data(*.*)|*.*"}
            If file.ShowDialog = DialogResult.OK Then
                Dim peaks As PeakFeature() = SaveSample.ReadSample(file.FileName.Open(FileMode.Open, doClear:=False, [readOnly]:=True)).ToArray
                Dim tableViewer As frmTableViewer = VisualStudio.ShowDocument(Of frmTableViewer)(DockState.Document, $"View Peak Features [{file.FileName}]")

                Call tableViewer.LoadTable(Sub(tb) Call peaks.StreamTo(tb))
            End If
        End Using
    End Sub

    Private Sub openLcmsScatter(data As Object, title As String, click As Action(Of String, Double, Double, Boolean))
        If data Is Nothing Then
            Call Workbench.Warning("no data could be loaded!")
        ElseIf TypeOf data Is Meta() Then
            Call VisualStudio.ShowDocument(Of frmLCMSScatterViewer)(title:=title).loadRaw(DirectCast(data, Meta())).Hook(click)
        ElseIf TypeOf data Is Raw Then
            Call VisualStudio.ShowDocument(Of frmLCMSScatterViewer)(title:=title).loadRaw(DirectCast(data, Raw)).Hook(click)
        Else
            Call Workbench.Warning($"invalid data type({data.GetType.FullName}) for the lcms scatter data viewer!")
        End If
    End Sub

    Public Sub viewUntargettedScatter()
        Dim raw As Raw = WindowModules.rawFeaturesList.CurrentOpenedFile

        If raw Is Nothing Then
            Call Workbench.Warning("Open a raw data file at first!")
            Return
        End If

        Dim page As frmLCMSScatterViewer = VisualStudio.ShowDocument(Of frmLCMSScatterViewer)(title:=raw.source.FileName)
        Call page.loadRaw(raw)
    End Sub

    Public Sub openLCMSWorkspace()
        Using folder As New FolderBrowserDialog
            If folder.ShowDialog = DialogResult.OK Then
                Dim page = VisualStudio.ShowDocument(Of frmMetabonomicsAnalysis)()
                page.workdir = folder.SelectedPath
                page.LoadWorkspace(folder.SelectedPath)
            End If
        End Using
    End Sub

    Public Sub openLCMSWorkbench()
        Dim page = Workbench.AppHost.DockPanel.ActiveDocument

        If TypeOf page Is frmTableViewer Then
            Dim tablePage As frmTableViewer = page
            Dim source As BindingSource = tablePage.AdvancedDataGridView1.DataSource
            Dim dataset As System.Data.DataSet = source.DataSource
            Dim table As DataTable = dataset.Tables.Item(Scan0)
            Dim df As DataFrame = table.DataFrame

            Call frmMetabonomicsAnalysis.LoadData(df, AddressOf New LoadMetabolismData With {.title = tablePage.TabText}.load)
        Else
            ribbonItems.MetaboAnalysis.ContextAvailable = ContextAvailability.Active
        End If
    End Sub

    Private Sub openMRIRaster()
        Using file As New OpenFileDialog With {.Filter = "Nearly raster image(*.nrrd)|*.nrrd"}
            If file.ShowDialog = DialogResult.OK Then
                Dim page = VisualStudio.ShowDocument(Of frmMRIViewer)(title:=$"View [{file.FileName.FileName}]")

                Call TaskProgress.RunAction(
                    run:=Sub(t)
                             Call page.LoadRaster(file.FileName)
                         End Sub,
                    title:=$"Open Nrrd Image",
                    info:=$"Read nrrd image file: {file.FileName}...",
                    host:=page
                )
            End If
        End Using
    End Sub

    Private Sub openIonLibrary()
        Call VisualStudio.ShowSingleDocument(Of frmLCMSLibrary)()
    End Sub

    Private Sub OpenExcelTableFile2()
        Dim filetypes As String() = {
           "Any Excel Table File(*.txt;*.csv;*.tsv;*.xlsx)|*.txt;*.csv;*.xlsx;*.tsv",
           "Tab delimiter file(*.txt;*.tsv)|*.txt;*.tsv",
           "Comma delimiter file(*.csv)|*.csv",
           "Excel table file(*.xlsx)|*.xlsx"
        }

        Using file As New OpenFileDialog With {
            .Filter = filetypes.JoinBy("|")
        }
            If file.ShowDialog = DialogResult.OK Then
                Call SelectSheetName.OpenExcel(
                    fileName:=file.FileName,
                    showFile:=
                        Sub(table, title)
                            Call frmMetabonomicsAnalysis.LoadData(table, AddressOf New LoadMetabolismData With {.title = title}.load)
                        End Sub)
            End If
        End Using
    End Sub



    Private Class LoadMetabolismData

        Public title As String

        Friend Sub load(sampleinfo As SampleInfo(), properties As String(), df As DataFrame, workdir As String)
            VisualStudio.ShowDocument(Of frmMetabonomicsAnalysis)(title:=title).LoadData(sampleinfo, properties, df, workdir, title)
        End Sub

    End Class

    Private Sub openSlideFile()
        Dim filetypes As String() = {
            "Hamamatsu format(*.ndpi)|*.ndpi",
            "Aperio format(*.svs)|*.svs",
            "Philips format, Trestle format, or TIFF Image(*.tif;*.tiff)|*.tif;*.tiff",
            "Ventana format(*.tif;*.bif)|*.tif;*.bif",
            "MIRAX format(*.mrxs)|*.mrxs",
            "Leica format(*.scn)|*.scn",
            "DICOM format(*.dcm)|*.dcm",
            "MZKit Slide Pack File(*.hds;*.mzPack)|*.hds;*.mzPack",
            "General image file(*.bitmap;*.png;*.jpg)|*.bitmap;*.png;*.jpg"
        }

        Using file As New OpenFileDialog With {
            .Filter = filetypes.JoinBy("|")
        }
            If file.ShowDialog = DialogResult.OK Then
                Dim dzifile As String

                If Not file.FileName.ExtensionSuffix("hds", "mzpack") Then
                    dzifile = $"{App.AppSystemTemp}/dzi_store/{file.FileName.MD5}.hds"

                    If dzifile.FileLength < 1024 Then
                        Call DziTools.CreateDziImages(
                            source:=file.FileName,
                            save_dzi:=dzifile
                        )
                    End If
                Else
                    dzifile = file.FileName
                End If

                Call TissueSlideHandler.OpenTifFile(dzifile, file.FileName.FileName)
            End If
        End Using
    End Sub

    Private Sub openShowSearchPubChemLCMS()
        InputDialog.Input(Of InputPubChemProxy)(
            Sub(cfg)
                Dim metadata = cfg.GetAnnotation
                Dim features = WindowModules.fileExplorer

                Call features.SearchFeatures(metadata.formula, mzdiff:=cfg.GetTolerance)
            End Sub)
    End Sub

    Private Sub openVideoList()
        Call VisualStudio.ShowDocument(Of frmHtmlViewer)(title:="View File").LoadHtml("http://education.biodeep.cn/")
    End Sub

    Private Sub ConvertH5ad()
        InputDialog.Input(Of InputConvert10x)(
            Sub(cfg)
                Using file As New SaveFileDialog With {
                    .Filter = "Imaging Pack File(*.mzPack)|*.mzPack"
                }
                    If file.ShowDialog = DialogResult.OK Then
                        With cfg.GetParameters
                            Call ConvertH5ad(file.FileName, .spots, .h5ad, .tag, .targets)
                        End With
                    End If
                End Using
            End Sub)
    End Sub

    Private Sub ConvertH5ad(file_out As String, spots$, h5ad$, tag$, targets As String())
        If spots.StringEmpty OrElse tag.StringEmpty Then
            ' convert h5ad
            Call RscriptProgressTask.Convert10XRawdata(h5ad, save:=file_out)
        Else
            ' convert space ranger result data
            Call RscriptProgressTask.ConvertSTData(spots, h5ad, tag, targets, file_out)
        End If

        If file_out.FileLength > 1024 Then
            If MessageBox.Show("File imports success, load into viewer?", "Task success",
                               MessageBoxButtons.OKCancel,
                               MessageBoxIcon.Information) = DialogResult.OK Then

                Call MyApplication.host.showMzPackMSI(file_out, debug:=False)
            End If
        Else
            Workbench.Warning("10X genomics st-imaging file imports error!")
        End If
    End Sub

    Private Sub open3dMALDIViewer()
        Call VisualStudio.ShowDocument(Of frm3DMALDIViewer)()
    End Sub

    Private Sub openMsconvertTool()
        Call Process.Start($"{App.HOME}/msconvertGUI.exe")
    End Sub

    Friend Sub CreatePeakFinding()
        Dim mzkitTool = MyApplication.host.mzkitTool
        Dim matrix As DataMatrix = mzkitTool._matrix

        If matrix Is Nothing Then
            Call Workbench.Warning("No chromatogram data is loaded into the MZKit data viewer!")
        ElseIf matrix.UnderlyingType Is GetType(ChromatogramTick) Then
            Dim app = VisualStudio.ShowDocument(Of frmPeakFinding)(DockState.Document, $"Peak Finding [{matrix.name}]")
            Dim data = matrix.GetMatrix(Of ChromatogramTick)

            app.LoadMatrix(matrix.name, data)
        ElseIf matrix.UnderlyingType Is GetType(ChromatogramSerial) Then
            Dim app = VisualStudio.ShowDocument(Of frmPeakFinding)(DockState.Document, $"Peak Finding [{matrix.name}]")
            Dim data = matrix.GetMatrix(Of ChromatogramSerial).ToArray

            If data.Length = 1 Then
                Call app.LoadMatrix(matrix.name, data.First)
            Else
                Call SelectSheetName.SelectName(
                    names:=data.Keys,
                    show:=Sub(name)
                              Dim which As ChromatogramSerial = data _
                                  .Where(Function(c) Strings.Trim(c.Name) = name) _
                                  .First

                              Call app.LoadMatrix(which.Name, which)
                          End Sub)
            End If
        Else
            Call Workbench.Warning("Peak finding application only works on the Chromatogram data matrix!")
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="struct">
    ''' molecule structrue string to run prediction
    ''' </param>
    Public Sub OpenCFMIDTool(struct As String)
        Dim tool As New InputCFMIDTool

        If Not struct.StringEmpty Then
            tool.TextBox1.Text = struct
        End If

        Call InputDialog.Input(
            setConfig:=Sub(cfmid)
                           Dim app As New CFM_ID.Prediction($"{cfmid.cfmid_folder}/cfm-predict.exe")
                           Dim result As MspData() = Nothing
                           Dim struct_str As String = cfmid.struct
                           Dim param As String = cfmid.param_config
                           Dim model As String = cfmid.model

                           Call MyApplication.host.showStatusMessage("Run cfm-id prediction for ms2 data!")
                           Call ProgressSpinner.DoLoading(
                              Sub()
                                  result = app.PredictMs2(struct_str, param_filename:=model, config_filename:=param)
                              End Sub)

                           Call MyApplication.host.showStatusMessage(app.ToString)
                           Call VisualStudio _
                              .ShowDocument(Of frmCFMIDOutputViewer) _
                              .SetCFMIDoutput(result)
                       End Sub,
            config:=tool
        )
    End Sub

    Private Sub CollectSystemInformation()
        If MessageBox.Show(
            MyApplication.getCurrentLanguageString("collectSysInfo"),
            MyApplication.getCurrentLanguageString("msgbox_title"),
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Question) = DialogResult.Cancel Then

            Return
        End If
    End Sub

    Friend Sub openCmd()
        Static WorkingDirectory As String = App.HOME & "/RStudio/bin"

        Dim exePath As String = Environment.SystemDirectory & "\cmd.exe"
        Dim StartInfo As New ProcessStartInfo(exePath)
        Dim cmdSession As New Process
        Dim pkg_attach As String = MyApplication.CheckPkgFolder(MyApplication.pkgs).GetDirectoryFullPath

        StartInfo.UseShellExecute = False
        ' start in this directory
        StartInfo.WorkingDirectory = WorkingDirectory
        ' let cmd.exe remain running using /k
        StartInfo.Arguments = $"/k CALL {getWelcomeScript.GetFullPath.CLIPath}"
        StartInfo.EnvironmentVariables("pkg_attach") = pkg_attach
        StartInfo.EnvironmentVariables("R_LIBS_USER") = MyApplication.R_LIBS_USER.GetDirectoryFullPath
        StartInfo.EnvironmentVariables("RSTUDIO_HOME") = $"{App.HOME}/Rstudio/bin"

        cmdSession.StartInfo = StartInfo
        cmdSession.Start()
    End Sub

    Const banner_script As String = "banner_prompt.cmd"

    Private Function getWelcomeScript() As String
        If AppEnvironment.IsDevelopmentMode Then
            Return $"{App.HOME}/../../src\mzkit\rstudio\{banner_script}"
        Else
            Return $"{App.HOME}/Rstudio\bin\{banner_script}"
        End If
    End Function

    Public Sub OpenWorkspace()
        Using file As New OpenFileDialog With {.Filter = "BioNovoGene MZKit Workspace(*.mzWork)|*.mzWork"}
            If file.ShowDialog = DialogResult.OK Then
                Call Globals.loadWorkspace(mzwork:=file.FileName, fromStartup:=False)
            End If
        End Using
    End Sub

    Public Sub CopyPlotImage()
        Dim toolkit = MyApplication.host.mzkitTool
        Dim img As Image = toolkit.PictureBox1.BackgroundImage

        If Not img Is Nothing Then
            Call Clipboard.Clear()
            Call Clipboard.SetImage(img)

            If toolkit._matrix Is Nothing Then
                Call MyApplication.host.showStatusMessage($"Plot image has been copy to clipboard!")
            Else
                Call MyApplication.host.showStatusMessage($"Plot image '{toolkit._matrix.name}' has been copy to clipboard!")
            End If
        End If
    End Sub

    Public Sub CopyMatrix()
        Dim table As DataGridView = MyApplication.host.mzkitTool.DataGridView1
        Dim exportTask As Action =
            Sub()
                Call MyApplication.host.Invoke(
                    Sub()
                        Call doMatrixCopy(table)
                    End Sub)
            End Sub

        If Not table Is Nothing Then
            Call ProgressSpinner.DoLoading(exportTask)
            Call MyApplication.host.showStatusMessage("Matrix data is copy to clipboard!")
        End If
    End Sub

    Private Sub doMatrixCopy(table As DataGridView)
        Dim sb As New StringBuilder
        Dim write As New StringWriter(sb)

        Call table.WriteTableToFile(write)

        Try
            Call Clipboard.Clear()
            Call Clipboard.SetText(sb.ToString)
        Catch ex As Exception
            Call MessageBox.Show("Error while access the clipboard, please retry later.", "Copy Matrix Data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Call Workbench.Warning(ex.ToString)
        End Try
    End Sub

    Public Sub CopyProperties()
        Dim obj As Object = WindowModules.propertyWin.getPropertyObject

        If obj Is Nothing Then
            Return
        ElseIf Not obj.GetType.ImplementInterface(Of ICopyProperties) Then
            Return
        Else
            Call DirectCast(obj, ICopyProperties).Copy()
            Call Workbench.SuccessMessage("Property data is copy to clipboard!")
        End If
    End Sub

    Public Sub CombineRowScanTask()
        Using file As New OpenFileDialog With {
            .Filter = "Thermo Raw(*.raw)|*.raw|sciex Wiff Raw(*.wiff)|*.wiff|BioNovoGene mzPack(*.mzPack)|*.mzPack",
            .Title = "Open MSI row scan raw data files",
            .Multiselect = True
        }
            If file.ShowDialog = DialogResult.OK Then
                Dim baseDir As String = file.FileNames()(Scan0).ParentPath
                Dim fileDirName As String = baseDir.BaseName
                Dim load As New ShowMSIRowScanSummary With {
                    .files = file.FileNames
                }

                Call InputDialog.Input(
                    Sub(creator)
                        Dim cutoff As Double = creator.cutoff
                        Dim basePeak As Double = creator.matrixMz
                        Dim res As Double = creator.resolution

                        Using savefile As New SaveFileDialog With {
                            .Filter = "BioNovoGene mzPack(*.mzPack)|*.mzPack",
                            .Title = "Save MSI raw data file",
                            .FileName = $"{fileDirName}.mzPack",
                            .InitialDirectory = baseDir
                        }
                            If savefile.ShowDialog = DialogResult.OK Then
                                Call MSConvertTask.CreateMSIRawFromRowBinds(
                                    files:=file.FileNames,
                                    savefile:=savefile.FileName,
                                    cutoff:=cutoff,
                                    basePeak:=basePeak,
                                    resoltuion:=res,
                                    norm:=creator.norm,
                                    loadCallback:=Sub(filepath)
                                                      Call RibbonEvents.showMsImaging()
                                                      Call WindowModules.viewer.loadimzML(filepath, debug:=False)
                                                  End Sub
                                )
                            End If
                        End Using
                    End Sub, config:=load)
            End If
        End Using
    End Sub

    Public Sub ImportsSCiLSLab()
        Using file As New OpenFileDialog With {
            .Filter = "SCiLS Lab Matrix Export(*.csv)|*.csv",
            .Multiselect = True
        }
            If file.ShowDialog = DialogResult.OK Then
                Using savefile As New SaveFileDialog With {
                    .Filter = "BioNovoGene mzPack(*.mzPack)|*.mzPack",
                    .Title = "Save MSI raw data file"
                }
                    If savefile.ShowDialog = DialogResult.OK Then
                        Dim msData As String() = file.FileNames
                        Dim tuples = SCiLSLab.CheckSpotFiles(msData).ToArray

                        If tuples.IsNullOrEmpty Then
                            ' missing spot index file
                            MessageBox.Show("invalid selected table data files!", "Imports SCiLS Lab", buttons:=MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Return
                        End If

                        Call MSConvertTask.ImportsSCiLSLab(
                            msData:=tuples,
                            savefile:=savefile.FileName,
                            loadCallback:=Sub(filepath)
                                              Call RibbonEvents.showMsImaging()
                                              Call WindowModules.viewer.loadimzML(filepath, debug:=False)
                                          End Sub)
                    End If
                End Using
            End If
        End Using
    End Sub

    Public Function OpenMSIRawDialog() As OpenFileDialog
        Return New OpenFileDialog() With {
           .Filter = {
               "All Supported Raw(*.raw;*.mzPack;*.imzML;*.cdf;*.mzML)|*.raw;*.mzPack;*.imzML;*.cdf;*.mzML",
               "AP-SMALDI project file(*.udp)|*.udp",
               "Thermo Raw(*.raw)|*.raw",
               "Imaging mzML(*.imzML)|*.imzML",
               "Mzkit Pixel Matrix(*.cdf)|*.cdf",
               "MRM targetted MS-Imaging(*.mzML)|*.mzML",
               "msiPL Dataset(*.h5)|*.h5"
           }.JoinBy("|"),
           .Title = "Open MS-imaging Raw Data File"
        }
    End Function

    ''' <summary>
    ''' open the ms-imaging raw data file
    ''' </summary>
    Public Sub OpenMSIRaw()
        Using file As OpenFileDialog = OpenMSIRawDialog()
            If file.ShowDialog = DialogResult.OK Then
                Call OpenMSIRaw(file.FileName, debug:=False)
            End If
        End Using
    End Sub

    Public Sub OpenMSIRaw(file As String, debug As Boolean)
        Call showMsImaging()

        Select Case file.ExtensionSuffix.ToLower
            Case "mzml", "raw", "udp" : Call WindowModules.viewer.loadRaw(file, Nothing)
            Case "imzml", "mzpack" : Call WindowModules.viewer.loadimzML(file, debug)
            Case "cdf" : Call WindowModules.msImageParameters.loadRenderFromCDF(file)
            Case "h5" : Call WindowModules.viewer.loadimzML(file, debug)
            Case Else
                Call Workbench.AppHost.Warning($"File type(*.{file.ExtensionSuffix}) is not yet supported!")
        End Select
    End Sub

    Public Sub OpenSingleCellsRaw()
        Using file As New OpenFileDialog With {.Filter = "BioNovogene mzPack(*.mzPack)|*.mzPack"}
            If file.ShowDialog = DialogResult.OK Then
                Call showSingleCells()

            End If
        End Using
    End Sub

    Public Sub showSingleCells()
        Dim dockPanel = VisualStudio.DockPanel

        WindowModules.singleCellViewer.Show(dockPanel)
        WindowModules.singleCellsParameters.Show(dockPanel)

        WindowModules.singleCellsParameters.DockState = DockState.DockLeft
        WindowModules.singleCellViewer.DockState = DockState.Document

        Workbench.RibbonItems.MenuSingleCells.ContextAvailable = ContextAvailability.Available
        Workbench.RibbonItems.MenuSingleCells.ContextAvailable = ContextAvailability.Active
    End Sub

    Public Sub showMsImaging()
        Dim dockPanel = VisualStudio.DockPanel

        Call WindowModules.viewer.Show(dockPanel)
        Call WindowModules.msImageParameters.Show(dockPanel)

        WindowModules.msImageParameters.DockState = DockState.DockLeft
    End Sub

    Private Sub resetLayout()
        WindowModules.fileExplorer.DockState = DockState.DockLeft
        WindowModules.rawFeaturesList.DockState = DockState.DockLeftAutoHide
        WindowModules.output.DockState = DockState.DockBottomAutoHide
        WindowModules.propertyWin.DockState = DockState.DockRightAutoHide
    End Sub

    Private Sub _recentItems_ExecuteEvent(sender As Object, e As ExecuteEventArgs)
        If e.Key.PropertyKey = RibbonProperties.RecentItems Then
            ' go over recent items
            Dim objectArray() As Object = CType(e.CurrentValue.PropVariant.Value, Object())
            For i As Integer = 0 To objectArray.Length - 1
                Dim propertySet As IUISimplePropertySet = TryCast(objectArray(i), IUISimplePropertySet)

                If propertySet IsNot Nothing Then
                    Dim propLabel As PropVariant
                    propertySet.GetValue(RibbonProperties.Label, propLabel)
                    Dim label As String = CStr(propLabel.Value)

                    Dim propLabelDescription As PropVariant
                    propertySet.GetValue(RibbonProperties.LabelDescription, propLabelDescription)
                    Dim labelDescription As String = CStr(propLabelDescription.Value)

                    Dim propPinned As PropVariant
                    propertySet.GetValue(RibbonProperties.Pinned, propPinned)
                    Dim pinned As Boolean = CBool(propPinned.Value)

                    ' update pinned value
                    MyApplication.host.recentItems(i).Pinned = pinned
                End If
            Next i
        ElseIf e.Key.PropertyKey = RibbonProperties.SelectedItem Then
            ' get selected item index
            Dim selectedItem As UInteger = CUInt(e.CurrentValue.PropVariant.Value)

            ' get selected item label
            Dim propLabel As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.Label, propLabel)
            Dim label As String = CStr(propLabel.Value)
            Dim sourceFile As String = Nothing

            For Each file As String In Globals.Settings.recentFiles.SafeQuery
                If label = file.FileName Then
                    sourceFile = file
                    Exit For
                End If
            Next

            ' get selected item label description
            Dim propLabelDescription As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.LabelDescription, propLabelDescription)
            Dim labelDescription As String = CStr(propLabelDescription.Value)

            ' get selected item pinned value
            Dim propPinned As PropVariant
            e.CommandExecutionProperties.GetValue(RibbonProperties.Pinned, propPinned)
            Dim pinned As Boolean = CBool(propPinned.Value)

            If label.ExtensionSuffix("R") Then
                If Not sourceFile.FileExists Then
                    MessageBox.Show($"The given R# script file [{label}] is not exists on your file system!", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Else
                    Call MyApplication.host.openRscript(sourceFile)
                End If
            Else
                Dim raw As Raw() = Globals.FindRaws(WindowModules.fileExplorer.treeView1, label).ToArray

                If raw.IsNullOrEmpty Then
                    MessageBox.Show($"The given raw data file [{label}] is not exists on your file system!", "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Else
                    Call MyApplication.host.mzkitTool.TIC(raw)
                End If
            End If
        End If
    End Sub

    Private Sub _uiCollectionChangedEvent_ChangedEvent(sender As Object, e As UICollectionChangedEventArgs)
        MessageBox.Show("Got ChangedEvent. Action = " & e.Action.ToString())
    End Sub

    Friend nav As New Stack(Of Control)

    Private Sub NavBack_Click(sender As Object, e As ExecuteEventArgs)
        If nav.Count > 0 Then
            Call MyApplication.host.ShowPage(nav.Pop, pushStack:=False)
        End If
    End Sub

    Private Sub About_Click(sender As Object, e As ExecuteEventArgs)
        Call New frmSplashScreen() With {.isAboutScreen = True, .TopMost = True}.Show()
    End Sub

    Friend Sub RunCurrentScript(sender As Object, e As ExecuteEventArgs)
        Dim active = MyApplication.host.m_dockPanel.ActiveDocument

        If Not active Is Nothing AndAlso TypeOf CObj(active) Is frmRScriptEdit Then
            Dim editor = DirectCast(CObj(active), frmRScriptEdit)
            Dim script As String = editor.ScriptText

            If Not editor.scriptFile.StringEmpty Then
                script.SaveTo(editor.scriptFile)
                script = editor.scriptFile
            End If

            Call MyApplication.ExecuteRScript(script, isFile:=Not editor.scriptFile.StringEmpty, AddressOf WindowModules.output.AppendRoutput)
            Call VisualStudio.Dock(WindowModules.output, DockState.DockBottom)
        End If
    End Sub

    Friend ReadOnly scriptFiles As New List(Of frmRScriptEdit)

    Public Sub CreateNewScript(sender As Object, e As ExecuteEventArgs)
        Dim newScript As New frmRScriptEdit

        newScript.Show(MyApplication.host.m_dockPanel)
        newScript.DockState = DockState.Document
        newScript.Text = "New R# Script"

        scriptFiles.Add(newScript)

        MyApplication.host.Text = $"BioNovoGene Mzkit [{newScript.Text}]"
    End Sub

    Private Sub showRTerm(sender As Object, e As ExecuteEventArgs)
        WindowModules.RtermPage.Show(MyApplication.host.m_dockPanel)
        WindowModules.RtermPage.DockState = DockState.Document

        MyApplication.host.Text = $"BioNovoGene Mzkit [{  WindowModules.RtermPage.Text}]"
    End Sub

    Private Sub ShowSettings(sender As Object, e As ExecuteEventArgs)
        WindowModules.settingsPage.Show(MyApplication.host.m_dockPanel)
        WindowModules.settingsPage.DockState = DockState.Document

        MyApplication.host.Text = $"BioNovoGene Mzkit [{WindowModules.settingsPage.Text}]"
    End Sub

    Private Sub ShowExplorer(sender As Object, e As ExecuteEventArgs)
        WindowModules.fileExplorer.Show(MyApplication.host.m_dockPanel)
        WindowModules.fileExplorer.DockState = DockState.DockLeft
    End Sub

    Private Sub ShowSearchList(sender As Object, e As ExecuteEventArgs)
        WindowModules.rawFeaturesList.Show(MyApplication.host.m_dockPanel)
        WindowModules.rawFeaturesList.DockState = DockState.DockLeft
    End Sub

    Private Sub ShowProperties(sender As Object, e As ExecuteEventArgs)
        MyApplication.host.ShowPropertyWindow()
    End Sub

    Private Sub showLoggingWindow(sender As Object, e As ExecuteEventArgs)
        WindowModules.output.Show(MyApplication.host.m_dockPanel)
        WindowModules.output.DockState = DockState.DockBottom
    End Sub

    Private Sub showStartPage(sender As Object, e As ExecuteEventArgs)
        If Not Globals.CheckFormOpened(WindowModules.startPage) Then
            WindowModules.startPage = New frmStartPage
        End If

        WindowModules.startPage.Show(MyApplication.host.m_dockPanel)
        WindowModules.startPage.DockState = DockState.Document

        MyApplication.host.Text = $"BioNovoGene Mzkit [{WindowModules.startPage.Text}]"
    End Sub

    Private Sub ExitToolsStripMenuItem_Click(sender As Object, e As ExecuteEventArgs)
        Call MyApplication.host.Close()
    End Sub

    Private Sub showHelp(sender As Object, e As ExecuteEventArgs)
        For Each dir As String In {App.HOME, $"{App.HOME}/docs", $"{App.HOME}/../", $"{App.HOME}/../docs/"}
            If $"{dir}/readme.pdf".FileExists Then
                Call Process.Start($"{dir}/readme.pdf")
                Return
            End If
        Next

        ' try open online page
        Call Process.Start("https://mzkit.org/dist/README.pdf")

        ' Me.showStatusMessage("Manul pdf file is missing...", My.Resources.StatusAnnotations_Warning_32xLG_color)
    End Sub
End Module
