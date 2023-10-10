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
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.RibbonLib.Controls
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Emit.Delegates
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports RibbonLib
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports Task
Imports Task.Container
Imports TaskStream
Imports WeifenLuo.WinFormsUI.Docking

Module RibbonEvents

    Public ReadOnly Property ribbonItems As RibbonItems

    <Extension>
    Public Sub AddHandlers(ribbonItems As RibbonItems)
        _ribbonItems = ribbonItems

        AddHandler ribbonItems.ButtonExit.ExecuteEvent, AddressOf ExitToolsStripMenuItem_Click
        AddHandler ribbonItems.ButtonOpenRaw.ExecuteEvent, AddressOf WindowModules.OpenFile
        AddHandler ribbonItems.ButtonImportsRawFiles.ExecuteEvent, AddressOf MyApplication.host.ImportsFiles
        AddHandler ribbonItems.ButtonAbout.ExecuteEvent, AddressOf About_Click
        AddHandler ribbonItems.ButtonPageNavBack.ExecuteEvent, AddressOf NavBack_Click
        AddHandler ribbonItems.ButtonNew.ExecuteEvent, AddressOf CreateNewScript

        AddHandler ribbonItems.TweaksImage.ExecuteEvent, AddressOf MyApplication.host.mzkitTool.ShowPlotTweaks
        AddHandler ribbonItems.ShowProperty.ExecuteEvent, AddressOf ShowProperties
        AddHandler ribbonItems.ButtonCopyProperties.ExecuteEvent, AddressOf CopyProperties
        AddHandler ribbonItems.ButtonCopyMatrix.ExecuteEvent, AddressOf CopyMatrix
        AddHandler ribbonItems.ButtonCopyPlot.ExecuteEvent, AddressOf CopyPlotImage

        AddHandler ribbonItems.ButtonPeakFinding.ExecuteEvent, Sub(sender, e) Call CreatePeakFinding()
        AddHandler ribbonItems.ButtonMzCalculator.ExecuteEvent, Sub(sender, e) Call MyApplication.host.ShowPage(MyApplication.host.mzkitCalculator)
        AddHandler ribbonItems.ButtonSettings.ExecuteEvent, AddressOf ShowSettings
        AddHandler ribbonItems.ButtonMzSearch.ExecuteEvent, Sub(sender, e) Call MyApplication.host.ShowPage(MyApplication.host.mzkitSearch)
        AddHandler ribbonItems.ButtonRsharp.ExecuteEvent, AddressOf showRTerm

        AddHandler ribbonItems.ButtonDropA.ExecuteEvent, Sub(sender, e) MyApplication.host.ShowPage(MyApplication.host.mzkitTool)
        AddHandler ribbonItems.ButtonDropB.ExecuteEvent, Sub(sender, e) MyApplication.host.ShowPage(MyApplication.host.mzkitCalculator)
        AddHandler ribbonItems.ButtonFormulaSearch.ExecuteEvent, Sub(sender, e) MyApplication.host.ShowPage(MyApplication.host.mzkitSearch)
        AddHandler ribbonItems.ButtonDropD.ExecuteEvent, Sub(sender, e) MyApplication.host.ShowPage(MyApplication.host.mzkitMNtools)
        AddHandler ribbonItems.ButtonShowSpectrumSearchPage.ExecuteEvent, Sub(sender, e) Call New frmSpectrumSearch().Show(MyApplication.host.DockPanel)

        AddHandler ribbonItems.ButtonCalculatorExport.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitCalculator.ExportToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonExactMassSearchExport.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.ExportExactMassSearchTable()
        AddHandler ribbonItems.ButtonSave.ExecuteEvent, Sub(sender, e) Call MyApplication.host.saveCurrentFile()
        AddHandler ribbonItems.ButtonNetworkExport.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitMNtools.saveNetwork()
        AddHandler ribbonItems.ButtonFormulaSearchExport.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitSearch.SaveSearchResultTable()

        AddHandler ribbonItems.ButtonBioDeep.ExecuteEvent, Sub(sender, e) Call Process.Start("http://www.biodeep.cn/")
        AddHandler ribbonItems.ButtonLicense.ExecuteEvent, Sub(sender, e) Call New frmLicense().ShowDialog()

        AddHandler ribbonItems.ButtonExportImage.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.SaveImageToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonExportMatrix.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.SaveMatrixToolStripMenuItem_Click()

        AddHandler ribbonItems.ButtonLayout1.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.SaveImageToolStripMenuItem_Click()
        AddHandler ribbonItems.ButtonLayout2.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.SaveMatrixToolStripMenuItem_Click()

        AddHandler ribbonItems.ButtonShowStartPage.ExecuteEvent, AddressOf showStartPage
        AddHandler ribbonItems.ButtonShowLogWindow.ExecuteEvent, AddressOf showLoggingWindow

        AddHandler ribbonItems.ButtonShowExplorer.ExecuteEvent, AddressOf ShowExplorer
        AddHandler ribbonItems.ButtonShowSearchList.ExecuteEvent, AddressOf ShowSearchList
        AddHandler ribbonItems.ButtonShowProperties.ExecuteEvent, AddressOf ShowProperties

        AddHandler ribbonItems.ButtonShowPlotViewer.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.ShowTabPage(MyApplication.host.mzkitTool.TabPage5)
        AddHandler ribbonItems.ButtonShowMatrixViewer.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.ShowTabPage(MyApplication.host.mzkitTool.TabPage6)
        AddHandler ribbonItems.ButtonNetworkRender.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitMNtools.RenderNetwork()
        AddHandler ribbonItems.ButtonRefreshNetwork.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitMNtools.RefreshNetwork()

        AddHandler ribbonItems.ButtonRunScript.ExecuteEvent, AddressOf RunCurrentScript
        AddHandler ribbonItems.ButtonSaveScript.ExecuteEvent, AddressOf MyApplication.host.saveCurrentScript

        AddHandler ribbonItems.HelpButton.ExecuteEvent, AddressOf showHelp

        AddHandler ribbonItems.ButtonTIC.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.TIC(isBPC:=False)
        AddHandler ribbonItems.ButtonBPC.ExecuteEvent, Sub(sender, e) Call MyApplication.host.mzkitTool.TIC(isBPC:=True)
        AddHandler ribbonItems.ButtonXIC.ExecuteEvent, AddressOf WindowModules.rawFeaturesList.ShowXICToolStripMenuItem_Click

        AddHandler ribbonItems.ButtonResetLayout.ExecuteEvent, AddressOf resetLayout

        AddHandler ribbonItems.RecentItems.ExecuteEvent, AddressOf _recentItems_ExecuteEvent
        AddHandler ribbonItems.ButtonMsImaging.ExecuteEvent, AddressOf showMsImaging
        AddHandler ribbonItems.ButtonOpenMSIRaw.ExecuteEvent, AddressOf OpenMSIRaw
        AddHandler ribbonItems.ButtonMSIRowScans.ExecuteEvent, AddressOf CombineRowScanTask
        AddHandler ribbonItems.ButtonImportsSCiLSLab.ExecuteEvent, AddressOf ImportsSCiLSLab
        AddHandler ribbonItems.ButtonMsDemo.ExecuteEvent, Sub() WindowModules.msDemo.ShowPage()
        ' AddHandler ribbonItems.Targeted.ExecuteEvent, Sub() Call ConnectToBioDeep.OpenAdvancedFunction(AddressOf VisualStudio.ShowSingleDocument(Of frmTargetedQuantification))
        AddHandler ribbonItems.Targeted.ExecuteEvent, Sub() Call VisualStudio.ShowSingleDocument(Of frmTargetedQuantification)()

        AddHandler ribbonItems.MRMLibrary.ExecuteEvent, Sub() Call VisualStudio.ShowSingleDocument(Of frmMRMLibrary)(Nothing)
        AddHandler ribbonItems.QuantifyIons.ExecuteEvent, Sub() Call VisualStudio.ShowSingleDocument(Of frmQuantifyIons)(Nothing)
        AddHandler ribbonItems.GCxGCViewer.ExecuteEvent, Sub() Call VisualStudio.ShowSingleDocument(Of frmGCxGCViewer)(Nothing)

        AddHandler ribbonItems.LogInBioDeep.ExecuteEvent, Sub() Call New frmLogin().ShowDialog()

        AddHandler ribbonItems.ButtonInstallMzkitPackage.ExecuteEvent, AddressOf VisualStudio.InstallInternalRPackages
        AddHandler ribbonItems.ShowGCMSExplorer.ExecuteEvent, Sub() Call VisualStudio.Dock(WindowModules.GCMSPeaks, DockState.DockLeft)
        AddHandler ribbonItems.ShowMRMExplorer.ExecuteEvent, Sub() Call VisualStudio.Dock(WindowModules.MRMIons, DockState.DockLeft)

        AddHandler ribbonItems.Tutorials.ExecuteEvent, Sub() Call openVideoList()
        AddHandler ribbonItems.ButtonViewSMILES.ExecuteEvent, Sub() Call VisualStudio.ShowSingleDocument(Of frmSMILESViewer)()
        AddHandler ribbonItems.ButtonPluginManager.ExecuteEvent, Sub() Call VisualStudio.ShowSingleDocument(Of frmPluginMgr)()

        AddHandler ribbonItems.AdjustParameters.ExecuteEvent, Sub() Call VisualStudio.Dock(WindowModules.parametersTool, DockState.DockRight)
        AddHandler ribbonItems.ImportsMzwork.ExecuteEvent, Sub() Call OpenWorkspace()

        AddHandler ribbonItems.ButtonDevTools.ExecuteEvent, Sub() Call openCmd()
        AddHandler ribbonItems.DOIReference.ExecuteEvent, Sub() Call New frmDOI().ShowDialog()
        AddHandler ribbonItems.ButtonSystemDiagnosis.ExecuteEvent, Sub() Call CollectSystemInformation()

        AddHandler ribbonItems.ButtonCFMIDTool.ExecuteEvent, Sub() Call OpenCFMIDTool(Nothing)
        AddHandler ribbonItems.MsconvertGUI.ExecuteEvent, Sub() Call openMsconvertTool()
        AddHandler ribbonItems.View3DMALDI.ExecuteEvent, Sub() Call open3dMALDIViewer()

        AddHandler ribbonItems.ButtonOpenServicesMgr.ExecuteEvent, Sub() Call VisualStudio.ShowSingleDocument(Of frmServicesManager)()

        AddHandler ribbonItems.ButtonImport10x_genomics.ExecuteEvent, Sub() Call ConvertH5ad()
        AddHandler ribbonItems.ButtonRenderUMAPScatter.ExecuteEvent, Sub() Call PageMoleculeNetworking.RunUMAP()

        AddHandler ribbonItems.ButtonSearchPubChem.ExecuteEvent, Sub() Call openShowSearchPubChemLCMS()

        AddHandler ribbonItems.ButtonOpenVirtualSlideFile.ExecuteEvent, Sub() Call openSlideFile()
        AddHandler ribbonItems.ButtonOpenTableTool.ExecuteEvent, Sub() Call OpenExcelTableFile2()
        AddHandler ribbonItems.OpenIonsLibrary.ExecuteEvent, Sub() Call openIonLibrary()
    End Sub

    Sub New()
        ExportApis._openMSImagingFile = AddressOf OpenMSIRaw
        ExportApis._openMSImagingViewer = AddressOf showMsImaging
        ExportApis._openCFMIDTool = AddressOf OpenCFMIDTool
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
                Call SelectSheetName.OpenExcel(file.FileName)
            End If
        End Using
    End Sub

    Private Sub openSlideFile()
        Dim filetypes As String() = {
            "Hamamatsu format(*.ndpi)|*.ndpi",
            "Aperio format(*.svs)|*.svs",
            "Philips format, Trestle format, or TIFF Image(*.tif;*.tiff)|*.tif;*.tiff",
            "Ventana format(*.tif;*.bif)|*.tif;*.bif",
            "MIRAX format(*.mrxs)|*.mrxs",
            "Leica format(*.scn)|*.scn",
            "DICOM format(*.dcm)|*.dcm",
            "MZKit Slide Pack File(*.hds;*.mzPack)|*.hds;*.mzPack"
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
                Using file As New SaveFileDialog With {.Filter = "Imaging Pack File(*.mzPack)|*.mzPack"}
                    If file.ShowDialog = DialogResult.OK Then
                        Dim pars = cfg.GetParameters

                        Call ConvertH5ad(file.FileName, pars.spots, pars.h5ad, pars.tag, pars.targets)
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

                Call MyApplication.host.showMzPackMSI(file_out)
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
        Dim matrix As Array = mzkitTool.matrix

        If matrix Is Nothing Then
            Call MyApplication.host.warning("No chromatogram data is loaded into the MZKit data viewer!")
        ElseIf Not TypeOf matrix Is ChromatogramTick() Then
            Call MyApplication.host.warning("Peak finding application only works on the Chromatogram data matrix!")
        Else
            Dim app = VisualStudio.ShowDocument(Of frmPeakFinding)(DockState.Document, $"Peak Finding [{mzkitTool.matrixName}]")
            app.LoadMatrix(mzkitTool.matrixName, DirectCast(matrix, ChromatogramTick()))
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
        Dim img As Image = MyApplication.host.mzkitTool.PictureBox1.BackgroundImage

        If Not img Is Nothing Then
            Call Clipboard.Clear()
            Call Clipboard.SetImage(img)
            Call MyApplication.host.showStatusMessage($"Plot image '{MyApplication.host.mzkitTool.matrixName}' is copy to clipboard!")
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
        Dim obj As Object = WindowModules.propertyWin.propertyGrid.SelectedObject

        If obj Is Nothing Then
            Return
        ElseIf Not obj.GetType.ImplementInterface(Of ICopyProperties) Then
            Return
        Else
            Call DirectCast(obj, ICopyProperties).Copy()
            Call MyApplication.host.showStatusMessage("Property data is copy to clipboard!")
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
                                                      Call WindowModules.viewer.loadimzML(filepath)
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
                                              Call WindowModules.viewer.loadimzML(filepath)
                                          End Sub)
                    End If
                End Using
            End If
        End Using
    End Sub

    ''' <summary>
    ''' open the ms-imaging raw data file
    ''' </summary>
    Public Sub OpenMSIRaw()
        Using file As New OpenFileDialog() With {
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
            If file.ShowDialog = DialogResult.OK Then
                Call OpenMSIRaw(file.FileName)
            End If
        End Using
    End Sub

    Public Sub OpenMSIRaw(file As String)
        Call showMsImaging()

        Select Case file.ExtensionSuffix.ToLower
            Case "mzml", "raw", "udp" : Call WindowModules.viewer.loadRaw(file, Nothing)
            Case "imzml", "mzpack" : Call WindowModules.viewer.loadimzML(file)
            Case "cdf" : Call WindowModules.msImageParameters.loadRenderFromCDF(file)
            Case "h5" : Call WindowModules.viewer.loadimzML(file)
            Case Else
                Call Workbench.AppHost.Warning($"File type(*.{file.ExtensionSuffix}) is not yet supported!")
        End Select
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
