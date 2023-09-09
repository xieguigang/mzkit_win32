#Region "Microsoft.VisualBasic::75e280eada08208a035f6f6cda02f0f4, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmMsImagingViewer.vb"

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

'   Total Lines: 830
'    Code Lines: 652
' Comment Lines: 23
'   Blank Lines: 155
'     File Size: 36.57 KB


' Class frmMsImagingViewer
' 
'     Properties: FilePath, MimeType
' 
'     Function: (+2 Overloads) createRenderTask, registerSummaryRender
' 
'     Sub: AddSampleToolStripMenuItem_Click, checks_Click, cleanBackground, ClearPinToolStripMenuItem_Click, ClearSamplesToolStripMenuItem_Click
'          CopyFullPath, CopyImageToolStripMenuItem_Click, (+2 Overloads) DoIonStats, ExportMatrixToolStripMenuItem_Click, exportMSISampleTable
'          exportMzPack, ExportPlotToolStripMenuItem_Click, frmMsImagingViewer_Closing, frmMsImagingViewer_Load, ImageProcessingToolStripMenuItem_Click
'          loadimzML, loadmzML, loadRaw, (+2 Overloads) LoadRender, MSIFeatureDetections
'          OpenContainingFolder, PinToolStripMenuItem_Click, PixelSelector1_SelectPixelRegion, PixelSelector1_SelectPolygon, Plot
'          renderByMzList, renderByPixelsData, renderRGB, RenderSummary, SaveDocument
'          SaveImageToolStripMenuItem_Click, setupPolygonEditorButtons, showPixel, ShowRegion, TogglePolygonMode
'          tweaks_PropertyValueChanged
' 
' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology.HEMap
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.ServiceHub
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap.hqx
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports mzblender
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports Mzkit_win32.MSImagingViewerV2
Imports ServiceHub
Imports STImaging
Imports Task
Imports TaskStream
Imports WeifenLuo.WinFormsUI.Docking
Imports File = Microsoft.VisualBasic.Data.csv.IO.File
Imports stdNum = System.Math
Imports xlsxFile = Microsoft.VisualBasic.MIME.Office.Excel.XLSX.File

Public Class frmMsImagingViewer
    Implements IFileReference

    Public Property FilePath As String Implements IFileReference.FilePath

    Dim WithEvents checks As ToolStripMenuItem
    Dim WithEvents tweaks As PropertyGrid
    Dim rendering As Action
    Dim guid As String

    Friend TIC As PixelScanIntensity()

    Friend MSIservice As ServiceHub.MSIDataService
    Friend params As MsImageProperty
    Friend HEMap As HEMapTools
    Friend DrawHeMapRegion As Boolean = False

    ''' <summary>
    ''' the ms-imaging rendering service
    ''' </summary>
    Dim blender As BlenderClient

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {
                    .Details = "Image mzML",
                    .FileExt = ".imzML",
                    .MIMEType = "text/xml",
                    .Name = "Image mzML"
                }
            }
        End Get
    End Property

    Public Sub OpenSession(ss As Type, args As String)
        If MSIservice Is Nothing Then
            Call StartMSIService()
        End If

        If blender Is Nothing Then
            blender = New BlenderClient(RenderService.MSIBlender, debug:=RenderService.debug)
            MSIservice.blender = blender
            MSIservice.blender.SetFilters(loadFilters)
        End If

        blender.OpenSession(ss, params.GetMSIDimension, Nothing, params, args)
    End Sub

    Public Sub StartMSIService()
        ServiceHub.MSIDataService.StartMSIService(hostReference:=MSIservice)

        If blender Is Nothing Then
            blender = New BlenderClient(RenderService.MSIBlender, debug:=RenderService.debug)
        End If

        MSIservice.blender = blender
        MSIservice.blender.SetFilters(loadFilters)
    End Sub

    Private Sub frmMsImagingViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = Text

        WindowModules.msImageParameters.DockState = DockState.DockLeft

        AddHandler RibbonEvents.ribbonItems.ButtonMSITotalIon.ExecuteEvent, Sub() Call RenderSummary(IntensitySummary.Total)
        AddHandler RibbonEvents.ribbonItems.ButtonMSIBasePeakIon.ExecuteEvent, Sub() Call RenderSummary(IntensitySummary.BasePeak)
        AddHandler RibbonEvents.ribbonItems.ButtonMSIAverageIon.ExecuteEvent, Sub() Call RenderSummary(IntensitySummary.Average)

        AddHandler RibbonEvents.ribbonItems.ButtonMSIImportAnnotationTable.ExecuteEvent, Sub() Call LoadImportAnnotationTable()
        AddHandler RibbonEvents.ribbonItems.ButtonExportSample.ExecuteEvent, Sub() Call exportMSISampleTable()
        AddHandler RibbonEvents.ribbonItems.ButtonExportMSIMzpack.ExecuteEvent, Sub() Call exportMzPack()
        AddHandler RibbonEvents.ribbonItems.ButtonTogglePolygon.ExecuteEvent, Sub() Call TogglePolygonMode()
        AddHandler RibbonEvents.ribbonItems.CleanBackgroundAutoReference.ExecuteEvent, Sub() Call cleanBackground(addReference:=False)
        AddHandler RibbonEvents.ribbonItems.CleanBackgroundByReference.ExecuteEvent, Sub() Call cleanBackground(addReference:=True)
        AddHandler RibbonEvents.ribbonItems.CleanBackgroundByBasePeak.ExecuteEvent, Sub() Call cleanBackgroundByBasePeak()
        AddHandler RibbonEvents.ribbonItems.ButtonMSIRawIonStat.ExecuteEvent, Sub() Call DoIonStats()
        AddHandler RibbonEvents.ribbonItems.ButtonMSIMatrixVisual.ExecuteEvent, Sub() Call OpenHeatmapMatrixPlot()
        AddHandler RibbonEvents.ribbonItems.ButtonUpsideDown.ExecuteEvent, Sub() Call TurnUpsideDown()
        AddHandler RibbonEvents.ribbonItems.ButtonImportsTissueMorphology.ExecuteEvent, Sub() Call ImportsTissueMorphology()
        AddHandler RibbonEvents.ribbonItems.ButtonExportRegions.ExecuteEvent, Sub() Call ExportRegions()
        AddHandler RibbonEvents.ribbonItems.ButtonMSISearchPubChem.ExecuteEvent, Sub() Call SearchPubChem()
        AddHandler RibbonEvents.ribbonItems.ButtonLoadHEMap.ExecuteEvent, Sub() Call loadHEMap()
        AddHandler RibbonEvents.ribbonItems.ButtonIonCoLocalization.ExecuteEvent, Sub() Call DoIonColocalization()
        AddHandler RibbonEvents.ribbonItems.ButtonImportsShimadzu.ExecuteEvent, Sub() Call convertShimadzuTable()
        AddHandler RibbonEvents.ribbonItems.ButtonMergeMultipleMSISample.ExecuteEvent, Sub() Call MergeSlides()

        AddHandler RibbonEvents.ribbonItems.ButtonConnectMSIService.ExecuteEvent, Sub() Call ConnectToCloud()
        AddHandler RibbonEvents.ribbonItems.ShowTissueData.ExecuteEvent, Sub() Call ShowTissueData()
        AddHandler RibbonEvents.ribbonItems.ButtonAutoUMAP.ExecuteEvent, Sub() Call RunUMAPTissueCluster()
        AddHandler RibbonEvents.ribbonItems.ButtonMSISignalCorrection.ExecuteEvent, Sub() Call ViewMzBins()
        AddHandler RibbonEvents.ribbonItems.ButtonRotateSlide.ExecuteEvent, Sub() Call rotateSlide()
        AddHandler RibbonEvents.ribbonItems.ButtonAutoLocation.ExecuteEvent, Sub() Call autoLocation()

        AddHandler RibbonEvents.ribbonItems.ButtonMSIFilterPipeline.ExecuteEvent, Sub() Call configFilter()

        AddHandler RibbonEvents.ribbonItems.CheckShowMapLayer.ExecuteEvent,
            Sub()
                If RibbonEvents.ribbonItems.CheckShowMapLayer.BooleanValue Then
                    Call sampleRegions.RenderLayer(PixelSelector1.MSICanvas)
                Else
                    Call sampleRegions.ClearLayer(PixelSelector1.MSICanvas)
                End If
            End Sub

        AddHandler RibbonEvents.ribbonItems.ButtonShowMSISampleWindow.ExecuteEvent,
            Sub()
                If sampleRegions.DockState = DockState.Hidden Then
                    sampleRegions.DockState = DockState.DockRight
                End If
            End Sub

        Call ApplyVsTheme(ContextMenuStrip1, PixelSelector1.ToolStrip1)
        Call setupPolygonEditorButtons()
        Call PixelSelector1.ShowMessage("BioNovoGene MZKit MSImaging Viewer")

        sampleRegions.Show(MyApplication.host.m_dockPanel)
        sampleRegions.DockState = DockState.Hidden
        sampleRegions.viewer = Me

        PixelSelector1.MSICanvas.EditorConfigs = InputConfigTissueMap.GetPolygonEditorConfig
    End Sub

    Public Shared Function loadFilters() As RasterPipeline
        Dim settings = Globals.Settings.msi_filters

        If settings Is Nothing Then
            Globals.Settings.msi_filters = Configuration.Filters.DefaultFilters
            Globals.Settings.Save()

            settings = Globals.Settings.msi_filters
        End If

        Return settings
    End Function

    Private Sub configFilter()
        If Not checkService() Then
            Return
        End If

        Dim config As New InputConfigFilterPipeline
        Dim settings = Globals.Settings.msi_filters

        If settings Is Nothing Then
            Globals.Settings.msi_filters = Configuration.Filters.DefaultFilters
            Globals.Settings.Save()

            settings = Globals.Settings.msi_filters
        End If

        Dim opts As Scaler() = settings.filters.Select(Function(si) Scaler.Parse(si)).ToArray

        config.ConfigPipeline(opts.ToArray, settings.flags)

        If loadedPixels.IsNullOrEmpty Then
            If Not TIC.IsNullOrEmpty Then
                config.ConfigIntensity(TIC.Select(Function(i) i.totalIon).ToArray)
            End If
        Else
            config.ConfigIntensity(loadedPixels.Select(Function(i) i.intensity).ToArray)
        End If

        Call InputDialog.Input(
            Sub(configs)
                With configs.GetFilterConfigs
                    Globals.Settings.msi_filters.filters = .filters
                    Globals.Settings.msi_filters.flags = .flags
                    Globals.Settings.Save()
                End With

                If blender IsNot Nothing AndAlso rendering IsNot Nothing Then
                    blender.SetFilters(configs.GetFilter)
                    rendering()
                End If
            End Sub, config:=config)
    End Sub

    Private Sub autoLocation()
        If Not checkService() Then
            Return
        Else
            Call SetMSIPadding(padding:=Nothing, True)
        End If
    End Sub

    Private Sub SetMSIPadding(padding As Padding, MSIrender As Boolean)
        Dim info = TaskProgress.LoadData(
            streamLoad:=Function(echo As Action(Of String)) MSIservice.AutoLocation(padding),
            title:="Do auto location",
            info:="Apply ms-imaging slide sample matrix adjust location and padding automatically..."
        )
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".tmp.mzPack", sessionID:=App.PID, prefix:="rotate_temp_")

        If Not info Is Nothing Then
            Call TaskProgress.LoadData(
                Function(echo As Action(Of String))
                    Call MSIservice.ExportMzpack(tempfile)
                    Return True
                End Function)
            Call MyApplication.host.showMzPackMSI(tempfile)

            If MSIrender Then
                Call RenderSummary(IntensitySummary.BasePeak)
            End If

            Call Workbench.SuccessMessage($"ms-imaging slide sample matrix adjust location and padding success!")
        Else
            Call Workbench.Warning("Adjust slide sample location error!")
        End If
    End Sub

    Private Function getThumbnail(p As Action(Of String)) As Image
        Dim panic As Boolean = False
        Dim summaryLayer As PixelScanIntensity() = MSIservice.LoadSummaryLayer(IntensitySummary.BasePeak, panic)

        If panic Then
            Workbench.StatusMessage("MS-Imaging data backend panic!", My.Resources.mintupdate_error)
            Return Nothing
        Else
            TIC = summaryLayer
        End If

        Dim range As DoubleRange = summaryLayer.Select(Function(i) i.totalIon).Range
        Dim blender As Type = GetType(SummaryMSIBlender) ' (summaryLayer, params, loadFilters)

        Me.blender.OpenSession(blender, params.GetMSIDimension, Nothing, params, "")

        Return Me.blender.MSIRender(Nothing, params, params.GetMSIDimension)
    End Function

    Private Sub rotateSlide()
        If Not checkService() Then
            Return
        End If

        ' fetch the BPC views
        Dim image As Image = TaskProgress.LoadData(
            streamLoad:=AddressOf getThumbnail,
            title:="Create ms-imaging slide previews",
            info:="Loading the basepeak summary plot of your slide as previews..."
        )

        If image Is Nothing Then
            Return
        Else
            Me.sampleRegions.SetBounds(TIC.Select(Function(a) New Point(a.x, a.y)))
        End If

        ' and then set rotation
        Dim rotateCfg As New InputMSIRotation

        Call rotateCfg.SetImage(image)
        Call InputDialog.Input(
            Sub(cfg)
                Call Me.Invoke(Sub() Call SetRotation(cfg.GetAngle, True))
            End Sub, config:=rotateCfg)
    End Sub

    Private Sub SetSpatialMapping(register As SpatialRegister, filepath As String)
        Dim image As New Bitmap(register.HEstain, register.viewSize)
        Dim scan_dims As Size = register.viewSize

        ' display the HEmap image
        Call PixelSelector1.SetMsImagingOutput(image, scan_dims, params.background, params.colors, {0, 1000}, 255)

        Dim info = TaskProgress.LoadData(
            streamLoad:=Function(echo As Action(Of String)) MSIservice.SetSpatialMapping(filepath),
            title:="Register MSI to HE-stain",
            info:="Apply matrix rotation and matrix translation to the ms-imaging slide sample data..."
        )
        ' display the overlaps
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".tmp.mzPack", sessionID:=App.PID, prefix:="rotate_temp_")
        Dim summary As IntensitySummary = IntensitySummary.Total

        If info Is Nothing Then
            Call Workbench.Warning("Matrix rotation error!")
            Return
        End If

        Call TaskProgress.LoadData(
            Function(echo As Action(Of String))
                Call MSIservice.ExportMzpack(tempfile)
                Return True
            End Function)
        Call MyApplication.host.showMzPackMSI(tempfile)
        Call TaskProgress.RunAction(
                Sub()
                    Call Invoke(Sub() rendering = registerSummaryRender(summary))
                End Sub, "Render MSI", $"Rendering MSI in {summary.Description} mode...")

        If HEMap Is Nothing Then
            HEMap = New HEMapTools
            HEMap.Show(VisualStudio.DockPanel)
            HEMap.DockState = DockState.Hidden
        End If

        HEMap.Clear(PixelSelector1.MSICanvas.HEMap)

        blender.SetHEMap(image)
        params.Hqx = HqxScales.None
        rendering()

        Call Workbench.SuccessMessage("HEstain - MSI register matrix load success!")
    End Sub

    Private Function GetHEMap() As Image
        If HEMap Is Nothing Then
            Return Nothing
        Else
            Return HEMap.HEStainMap
        End If
    End Function

    Private Sub SetRotation(angle As Double, MSIrender As Boolean)
        Dim info = TaskProgress.LoadData(
            streamLoad:=Function(echo As Action(Of String)) MSIservice.SetSpatial2D(angle),
            title:="Do rotation",
            info:="Apply matrix rotation to the ms-imaging slide sample data..."
        )
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".tmp.mzPack", sessionID:=App.PID, prefix:="rotate_temp_")

        If info Is Nothing Then
            Call Workbench.Warning("Matrix rotation error!")
            Return
        End If

        Call TaskProgress.LoadData(
            Function(echo As Action(Of String))
                Call MSIservice.ExportMzpack(tempfile)
                Return True
            End Function)
        Call MyApplication.host.showMzPackMSI(tempfile)

        If MSIrender Then
            Call RenderSummary(IntensitySummary.BasePeak)
        End If

        Call Workbench.SuccessMessage($"Rotate the MS-imaging sample slide at angle {angle}.")
    End Sub

    Private Sub ViewMzBins()
        If loadedPixels.IsNullOrEmpty Then
            Call Workbench.Warning("No pixel layer data was loaded, please load a target ion at first!")
            Return
        End If

        Dim canvas As New ShowMzBins With {.Layer = loadedPixels}

        Call InputDialog.Input(Sub(cfg)
                                   ' do nothing
                               End Sub, config:=canvas)
    End Sub

    Private Sub RunUMAPTissueCluster()
        If Not checkService() Then
            Return
        End If

        Dim matrix As String = exportAllSpotSamplePeaktable(noUI:=True, filePath:=FilePath)

        If matrix.StringEmpty Then
            Call Workbench.Warning("Export feature ions peaktable task error or user canceled.")
            Return
        End If

        Dim umap3 As String = RscriptProgressTask.CreateUMAPCluster(matrix, 16)

        If umap3.StringEmpty Then
            MessageBox.Show("Sorry, run umap task error...", "UMAP error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return
        Else
            Call ImportsUmap3dFile(umap3)
        End If
    End Sub

    Private Sub ImportsUmap3dFile(umap3 As String)
        ' show umap scatter 3d
        umap3D = UMAPPoint.ParseCsvTable(umap3).ToArray

        Call ShowTissueData()

        ' ansalso convert to tissue region data
        Dim colors As LoopArray(Of Color) = Designer.GetColors("Paper")
        Dim groups = umap3D _
            .GroupBy(Function(a) a.class) _
            .Select(Function(group)
                        Return New TissueRegion With {
                            .color = ++colors,
                            .label = group.Key,
                            .points = group _
                                .Select(Function(u) u.Pixel) _
                                .ToArray
                        }
                    End Function) _
            .ToArray

        Call ImportsTissueMorphology(tissues:=groups)
    End Sub

    Private Sub LoadImportAnnotationTable()
        If Not checkService() Then
            Return
        End If

        Dim file As New OpenFileDialog With {.Filter = "Excel Table(*.csv;*.xlsx)|*.csv;*.xlsx"}

        If file.ShowDialog <> DialogResult.OK Then
            Return
        End If

        Dim annotations As DataFrame

        If file.FileName.ExtensionSuffix("csv") Then
            annotations = DataFrame.Load(file.FileName)
        Else
            annotations = DataFrame.CreateObject(xlsxFile.Open(file.FileName).GetTable(0))
        End If

        Dim name As String() = annotations.GetColumnValues("name").ToArray
        Dim formula As String() = annotations.GetColumnValues("formula").ToArray
        Dim mass As Double() = formula.Select(Function(fstr) FormulaScanner.ScanFormula(fstr).ExactMass).ToArray
        ' evaluate m/z
        Dim adducts = annotations.GetColumnValues("precursor_type").ToArray  ' If(params.polarity = IonModes.Negative, Provider.Negatives, Provider.Positives)
        'Dim mz As Double() = adducts _
        '    .Select(Function(t) mass.Select(Function(em) t.CalcMZ(em))) _
        '    .IteratesALL _
        '    .Distinct _
        '    .ToArray
        Dim mz As Double() = annotations.GetColumnValues("m/z").Select(AddressOf Val).ToArray
        Dim labels As String() = name.Select(Function(namei, i) $"{namei} {adducts(i)}").ToArray

        Call WindowModules.msImageParameters.ImportsIons(labels, mz)
        Call TaskProgress.LoadData(
            Function(println As Action(Of String))
                Call Me.Invoke(Sub() importsAnnotations(name, formula, mass, mz, adducts, println))
                Return True
            End Function, title:="Do metabolite annotation imports", info:="Running ms-imaging raw data file scanning!")
    End Sub

    ''' <summary>
    ''' name/formula/exactMass是一一对应的
    ''' </summary>
    ''' <param name="name"></param>
    ''' <param name="formula"></param>
    ''' <param name="exactMassList"></param>
    ''' <param name="mz"></param>
    Private Sub importsAnnotations(name As String(),
                                   formula As String(),
                                   exactMassList As Double(),
                                   mz As Double(),
                                   adducts As String(),
                                   println As Action(Of String))

        Call println($"Measuring for {mz.Length} ions data...")

        Dim ions As IonStat() = MSIservice.DoIonStats(mz)

        If ions.IsNullOrEmpty Then
            Call Workbench.Warning("No ions result...")
            Return
        End If

        Dim title As String = If(FilePath.StringEmpty, "MS-Imaging Ion Stats", $"[{FilePath.FileName}]Ion Stats")
        Dim table As frmTableViewer = VisualStudio.ShowDocument(Of frmTableViewer)(DockState.Hidden, title:=title)

        table.AppSource = GetType(IonStat)
        table.InstanceGuid = guid
        table.SourceName = FilePath.FileName Or "MS-Imaging".AsDefault
        table.ViewRow = Sub(row)
                            Dim mzi As Double = Val(row("mz"))
                            Dim namei As String = row("name")
                            Dim namePlot As String = $"{namei} {row("precursor_type")} {mzi.ToString("F4")}"

                            Call renderByMzList({mzi}, namePlot)
                            Call Me.Activate()
                        End Sub

        Call println($"Build search tree for {ions.Length} ions hit!")

        Dim ionList = New BlockSearchFunction(Of IonStat)(
            data:=ions,
            eval:=Function(i) i.mz,
            tolerance:=1,
            factor:=2,
            fuzzy:=True
        )

        table.LoadTable(
            Sub(grid)
                Call grid.Columns.Add("name", GetType(String))
                Call grid.Columns.Add("formula", GetType(String))
                Call grid.Columns.Add("precursor_type")
                Call grid.Columns.Add("mz", GetType(Double))
                Call grid.Columns.Add("pixels", GetType(Integer))
                Call grid.Columns.Add("density", GetType(Double))
                Call grid.Columns.Add("imaging_score", GetType(Double))
                Call grid.Columns.Add("maxIntensity", GetType(Double))
                Call grid.Columns.Add("basePixel.X", GetType(Integer))
                Call grid.Columns.Add("basePixel.Y", GetType(Integer))
                Call grid.Columns.Add("Q1_intensity", GetType(Double))
                Call grid.Columns.Add("Q2_intensity", GetType(Double))
                Call grid.Columns.Add("Q3_intensity", GetType(Double))
                Call grid.Columns.Add("RSD", GetType(Double))

                For i As Integer = 0 To name.Length - 1
                    Dim name_str = name(i)
                    Dim formula_str = formula(i)
                    Dim mzi As Double = mz(i)
                    Dim type As String = adducts(i)

                    println($"Process for {name_str}({formula_str})...")

                    Dim ion = ionList _
                        .Search(New IonStat With {.mz = mzi}, tolerance:=0.05) _
                        .OrderBy(Function(ion2) stdNum.Abs(ion2.mz - mzi)) _
                        .FirstOrDefault

                    If ion Is Nothing Then
                        Continue For
                    ElseIf stdNum.Abs(ion.mz - mzi) > 0.05 Then
                        Continue For
                    End If

                    Call grid.Rows.Add(
                       name_str,
                       formula_str,
                       type,
                       mzi.ToString("F4"),
                       ion.pixels,
                       ion.density.ToString("F2"),
                       (stdNum.Log(ion.pixels + 1) * ion.density).ToString("F2"),
                       stdNum.Round(ion.maxIntensity),
                       ion.basePixelX,
                       ion.basePixelY,
                       stdNum.Round(ion.Q1Intensity),
                       stdNum.Round(ion.Q2Intensity),
                       stdNum.Round(ion.Q3Intensity),
                       stdNum.Round(ion.RSD)
                    )

                    Call System.Windows.Forms.Application.DoEvents()
                Next
            End Sub)

        table.DockState = DockState.Document

        Me.DockState = DockState.Document
        Me.Show(Workbench.AppHost.DockPanel)

        Call Workbench.SuccessMessage($"Imports {ions.Length} ms-imaging ions target for {name.Length} metabolite annotations!")
    End Sub

    Dim umap3D As UMAPPoint()

    ''' <summary>
    ''' try to show umap 3d scatter plot
    ''' </summary>
    Public Sub ShowTissueData()
        Call ShowTissueData(umap3D)
    End Sub

    Public Sub ShowTissueData(umap3D As UMAPPoint())
        Dim summary As New ShowTissueData

        If umap3D.IsNullOrEmpty Then
            MessageBox.Show(
                text:="No tissue cluster data was loaded, run imports of tissue morphology data at first.",
                caption:="View Scatter 3D",
                buttons:=MessageBoxButtons.OK,
                icon:=MessageBoxIcon.Information
            )

            Return
        End If

        Call summary.SetTissueClusters(umap3D)
        Call InputDialog.Input(
            setConfig:=Sub(cfg)
                           Dim viewer As New frm3DScatterPlotView

                           Call viewer.LoadScatter(umap3D, AddressOf ClickOnPixel)
                           Call VisualStudio.ShowDocument(viewer)
                       End Sub,
            config:=summary
        )
    End Sub

    Private Sub ClickOnPixel(spot_id As String)
        If spot_id.StringEmpty Then
            Return
        End If

        Dim xy As Integer() = spot_id _
            .Split(","c) _
            .Select(AddressOf Integer.Parse) _
            .ToArray

        Call Me.Invoke(Sub() showPixel(xy(0), xy(1)))
    End Sub

    Private Sub ConnectToCloud(config As InputCloudEndPoint)
        Me.MSIservice = MSIDataService.ConnectCloud(Me.MSIservice, config.IP, config.Port)

        Call WindowModules.viewer.Show(DockPanel)
        Call WindowModules.msImageParameters.Show(DockPanel)

        Dim info As MsImageProperty

        Try
            info = MSIservice.GetMSIInformationMetadata
        Catch ex As Exception
            MessageBox.Show(ex.Message, "MZKit Cloud", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Call LoadRender(info, "MZKit_Cloud")

        WindowModules.viewer.DockState = DockState.Document
        MyApplication.host.Text = $"BioNovoGene Mzkit [{WindowModules.viewer.Text} {info.sourceFile}]"
        WindowModules.msImageParameters.DockState = DockState.DockLeft

        Call RenderSummary(IntensitySummary.BasePeak)
    End Sub

    Public Sub ConnectToCloud()
        Call InputDialog.Input(Of InputCloudEndPoint)(AddressOf ConnectToCloud)
    End Sub

    Public Function ExtractMultipleSampleRegions() As RegionLoader
        If Not checkService() Then
            Return Nothing
        Else
            Return MSIservice.ExtractMultipleSampleRegions
        End If
    End Function

    Private Sub MergeWidthLayout(config As InputMSISlideLayout, files As String(), savefile As String)
        If TaskProgress.LoadData(
            streamLoad:=Function(echo)
                            'Return loadfiles _
                            '    .JoinMSISamples(println:=echo) _
                            '    .Write(savefile.OpenFile, progress:=echo)
                            Call MSConvertTask.MergeMultipleSlides(
                                msData:=files,
                                layoutData:=config.layoutData,
                                savefile:=savefile,
                                fileName_tag:=config.useFileNameAsSourceTag,
                                echo:=AddressOf echo.SetInfo
                            )

                            Return True
                        End Function,
            title:="Merge Multiple Slides",
            info:="Load MS-Imaging slide files...") Then

            If MessageBox.Show("MSI Raw Convert Job Done!" & vbCrLf & "Open MSI raw data file in MSI Viewer?",
                               "MSI Viewer",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Information) = DialogResult.Yes Then

                Call RibbonEvents.showMsImaging()
                Call WindowModules.viewer.loadimzML(savefile)
            End If
        End If
    End Sub

    ''' <summary>
    ''' merge multiple sample files
    ''' </summary>
    Sub MergeSlides()
        Using file As New OpenFileDialog With {
            .Filter = "BioNovoGene mzPack(*.mzPack)|*.mzPack",
            .Multiselect = True
        }
            If file.ShowDialog = DialogResult.OK Then
                Dim files As String() = file.FileNames
                'Dim loadfiles As IEnumerable(Of mzPack) = files _
                '    .Select(Function(path)
                '                Return mzPack.Read(
                '                    filepath:=path,
                '                    ignoreThumbnail:=True,
                '                    skipMsn:=True
                '                )
                '            End Function)

                Using savefile As New SaveFileDialog With {.Filter = file.Filter}
                    If savefile.ShowDialog = DialogResult.OK Then
                        Dim input As New InputMSISlideLayout With {
                            .layoutData = files.Select(AddressOf BaseName).JoinBy(","),
                            .useFileNameAsSourceTag = True
                        }

                        Call InputDialog.Input(Sub(config) Call MergeWidthLayout(config, files, savefile.FileName),, config:=input)
                    End If
                End Using
            End If
        End Using
    End Sub

    Sub convertShimadzuTable()
        Using file As New OpenFileDialog With {.Filter = "MSI Table(*.csv)|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                Dim header As String = file.FileName.ReadFirstLine

                If Not Shimadzu.CheckTableHeader(header.Split(ASCII.TAB, " "c, ","c)) Then
                    MessageBox.Show("Invalid table file format!", "Import Shimadzu MSI Table", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Else
                    Using savefile As New SaveFileDialog With {
                        .Filter = "BioNovoGene mzPack(*.mzPack)|*.mzPack",
                        .FileName = $"{file.FileName.BaseName}.mzPack"
                    }
                        If savefile.ShowDialog = DialogResult.OK Then
                            Call TaskProgress.LoadData(
                                streamLoad:=Function(echo)
                                                Dim raw As mzPack = Shimadzu.ImportsMzPack(
                                                    file:=file.OpenFile,
                                                    sample:=file.FileName.FileName,
                                                    println:=echo.Echo
                                                )

                                                Return raw.Write(
                                                    file:=savefile.FileName.Open(FileMode.OpenOrCreate, doClear:=True),
                                                    progress:=echo.Echo
                                                )
                                            End Function,
                                title:=$"Imports [{file.FileName}]"
                            )

                            If MessageBox.Show("MSI Raw Convert Job Done!" & vbCrLf & "Open MSI raw data file in MSI Viewer?",
                                               "MSI Viewer",
                                               MessageBoxButtons.YesNo,
                                               MessageBoxIcon.Information) = DialogResult.Yes Then

                                Call RibbonEvents.showMsImaging()
                                Call WindowModules.viewer.loadimzML(savefile.FileName)
                            End If
                        End If
                    End Using
                End If
            End If
        End Using
    End Sub

    Sub loadHEMap()
        Dim files As String() = {
            "HE Stain Image(*.jpg;*.png;*.bmp;*.tif)|*.jpg;*.png;*.bmp;*.tif",
            "HE Scalar Mapping Matrix(*.csv)|*.csv",
            "MZKit spatial register matrix(*.cdf)|*.cdf"
        }

        Using file As New OpenFileDialog With {
            .Filter = files.JoinBy("|")
        }
            If file.ShowDialog = DialogResult.OK Then
                If file.FileName.ExtensionSuffix("csv") Then
                    Call loadHEMapMatrix(file.FileName)
                ElseIf file.FileName.ExtensionSuffix("cdf") Then
                    Dim register As SpatialRegister = SpatialRegister.ParseFile(file.OpenFile)

                    ' verify the MSI data dimension 
                    If Not checkService() Then
                        Return
                    ElseIf params Is Nothing Then
                        Return
                    ElseIf params.scan_x <> register.MSIdims.Width OrElse params.scan_y <> register.MSIdims.Height Then
                        Call MessageBox.Show("The dimension data between current MS-imaging viewer and the HEstain regitsered dimesion is mis-matched!", "Invalid MS-Imaging Dimensions", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return
                    Else

                    End If

                    Call SetSpatialMapping(register, file.FileName)
                ElseIf file.FileName.ExtensionSuffix("ndpi") Then
                    Call TissueSlideHandler.OpenNdpiFile(file.FileName)
                ElseIf file.FileName.ExtensionSuffix("tif", "tiff", "dzi") Then
                    'PixelSelector1.OpenImageFile(file.FileName)
                    'PixelSelector1.PreviewButton = True
                    'PixelSelector1.ShowPreview = True
                    Call TissueSlideHandler.OpenTifFile(file.FileName)
                Else
                    Call loadHEMapImage(New Bitmap(file.FileName.LoadImage))
                End If
            End If
        End Using
    End Sub

    ''' <summary>
    ''' [x,y] should be exists in the matrix, and then other
    ''' field will be mapping value to the color scaler.
    ''' </summary>
    ''' <param name="fileName"></param>
    Private Sub loadHEMapMatrix(fileName As String)
        Dim file As File = Microsoft.VisualBasic.Data.csv.IO.File.Load(fileName)
        Dim table = DataFrame.CreateObject(file)

        If table.GetOrdinal("x") = -1 OrElse table.GetOrdinal("y") = -1 Then
            MessageBox.Show(
                text:="We are unable to load your HE-stain mapping matrix due to the reason of missing pixel 'x' and 'y' fields!",
                caption:="Load HE-stain Mapping Error",
                buttons:=MessageBoxButtons.OK,
                icon:=MessageBoxIcon.Error
            )
            Return
        End If

        Dim input As New InputDataFieldName
        input.AddItems(From str As String In table.HeadTitles Where str <> "x" AndAlso str <> "y")

        InputDialog.Input(
            Sub(config)
                Dim field As String = config.GetInputFieldName
                Dim x As Integer() = table.GetColumnValues("x").Select(AddressOf Integer.Parse).ToArray
                Dim y As Integer() = table.GetColumnValues("y").Select(AddressOf Integer.Parse).ToArray
                Dim data As Double() = table.GetColumnValues(field).Select(AddressOf Double.Parse).ToArray
                Dim scan_x As Integer = If(params Is Nothing, x.Max, params.scan_x)
                Dim scan_y As Integer = If(params Is Nothing, y.Max, params.scan_y)
                Dim layer As New SingleIonLayer With {
                    .IonMz = field,
                    .DimensionSize = New Size(scan_x, scan_y),
                    .MSILayer = data _
                        .Select(Function(v, i) New PixelData(x(i), y(i), v)) _
                        .ToArray
                }
                Dim argv As New MsImageProperty(scan_x, scan_y) With {
                    .background = Color.Transparent,
                    .colors = ScalerPalette.viridis,
                    .enableFilter = False,
                    .Hqx = HqxScales.Hqx_4x,
                    .mapLevels = 255,
                    .scale = InterpolationMode.HighQualityBicubic,
                    .showTotalIonOverlap = True,
                    .showPhysicalRuler = False,
                    .TrIQ = 1,
                    .resolution = 17,
                    .knn = 0,
                    .knn_qcut = 1
                }
                Dim blender As Type = GetType(SingleIonMSIBlender) '(layer.MSILayer, Nothing, argv, loadFilters)
                Me.blender.channel.WriteBuffer(PixelData.GetBuffer(layer.MSILayer))
                Me.blender.OpenSession(blender, New Size(scan_x, scan_y), Nothing, params, Nothing)
                Dim HEMap As Image = Me.blender.MSIRender(Nothing, argv, layer.DimensionSize)

                If Me.blender IsNot Nothing AndAlso Me.blender.Session IsNot GetType(HeatMapBlender) Then
                    ' draw and overlaps on the MS-imaging rendering for CAD analysis
                    PixelSelector1.MSICanvas.tissue_layer = HEMap
                    PixelSelector1.MSICanvas.RedrawCanvas()
                Else
                    Call loadHEMapImage(HEMap)
                End If
            End Sub, config:=input)
    End Sub

    Private Sub loadHEMapImage(HEMapImg As Bitmap)
        PixelSelector1.MSICanvas.HEMap = HEMapImg

        If blender IsNot Nothing Then
            If blender.Session IsNot GetType(HeatMapBlender) Then
                blender.SetHEMap(PixelSelector1.MSICanvas.HEMap)
                rendering()
            End If

            If Not TIC.IsNullOrEmpty Then
                Call VisualStudio _
                    .ShowDocument(Of frmHEStainAnalysisTool)(DockState.Document, "HEStain Tool") _
                    .LoadRawData(
                        MSI:=TIC,
                        dims:=New Size(params.scan_x, params.scan_y),
                        HEstain:=HEMapImg
                    )
            End If
        Else
            ' just display hemap on the canvas
            PixelSelector1.SetMsImagingOutput(
                PixelSelector1.MSICanvas.HEMap,
                PixelSelector1.MSICanvas.HEMap.Size,
                Color.Black,
                ScalerPalette.Jet,
                {0, 255},
                120
            )
        End If

        If HEMap Is Nothing Then
            HEMap = New HEMapTools
            HEMap.Show(VisualStudio.DockPanel)
            HEMap.DockState = DockState.Hidden

            ExportApis._openHEMapTool(tool:=HEMap)
            ExportApis._getHEMapTool = Function() HEMap
            ExportApis._getHEMapImage = Function() PixelSelector1.MSICanvas.HEMap
        End If

        HEMap.Clear(PixelSelector1.MSICanvas.HEMap)

        VisualStudio.Dock(HEMap, DockState.DockRight)
    End Sub

    Sub TurnUpsideDown()
        If Not checkService() Then
            Return
        ElseIf MessageBox.Show("This operation will makes the entire MSImaging plot upside down.",
                               "MSI Data Services",
                               buttons:=MessageBoxButtons.OKCancel,
                               MessageBoxIcon.Information) = DialogResult.OK Then

            Call TaskProgress.LoadData(
                Function(msg As ITaskProgress)
                    Dim info = MSIservice.TurnUpsideDown

                    If Not info Is Nothing Then
                        Call Me.Invoke(Sub() LoadRender(info, FilePath))
                        Call Me.Invoke(Sub() RenderSummary(IntensitySummary.BasePeak))
                        Call Me.Invoke(Sub() Call StartNewPolygon())
                        Call Me.Invoke(Sub() Call sampleRegions.TurnUpsideDown(PixelSelector1.MSICanvas))
                    End If

                    Return 0
                End Function, taskAssign:=MSIservice.taskHost)
        End If
    End Sub

    Sub SearchPubChem()
        Dim getFormula As New InputPubChemProxy
        Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)

        If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
            If checkService() Then
                Dim formula As String = getFormula.GetAnnotation.formula
                Dim mass As Double = FormulaScanner.ScanFormula(formula).ExactMass
                ' evaluate m/z
                Dim adducts As MzCalculator() = If(params.polarity = IonModes.Negative, Provider.Negatives, Provider.Positives)
                Dim mz As Double() = adducts.Select(Function(t) t.CalcMZ(mass)).ToArray

                ProgressSpinner.DoLoading(
                    Sub()
                        Dim ions As IonStat() = MSIservice.DoIonStats(mz)

                        If ions.IsNullOrEmpty Then
                            Call Workbench.Warning("No ions result...")
                        Else
                            Call Me.Invoke(Sub()
                                               Call DoIonStats(ions, getFormula.GetAnnotation.name, formula, Provider.Positives)
                                               Call Workbench.SuccessMessage($"Load {ions.Length} ms-imaging ion targets!")
                                           End Sub)
                        End If
                    End Sub)
            Else
                Call Workbench.Warning($"The MS-imaging backend services is not running for rendering {getFormula.GetAnnotation.name}!")
            End If
        End If
    End Sub

#Region "Tissue Map"

    ''' <summary>
    ''' imports tissue morphology cdf matrix or the phenograph spot dot plot
    ''' </summary>
    Sub ImportsTissueMorphology()
        If Not checkService() Then
            Return
        ElseIf PixelSelector1.MSICanvas.dimension_size.IsEmpty Then
            Call MyApplication.host.showStatusMessage("No ms-imaging rendering output!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Using file As New OpenFileDialog With {
            .Filter = "Tissue Morphology Matrix(*.cdf)|*.cdf|Phenograph Spot Plot; UMAP scatter Plot(*.csv)|*.csv"
        }
            If file.ShowDialog = DialogResult.OK Then
                If file.FileName.ExtensionSuffix("cdf") Then
                    Call ImportsTissueMorphology(file.FileName, file.OpenFile)
                ElseIf CheckUmapTableFile(file.FileName) Then
                    Call ImportsUmap3dFile(file.FileName)
                Else
                    Call ImportsPhenographSpotPlot(filepath:=file.FileName)
                End If
            End If
        End Using
    End Sub

    Private Function CheckUmapTableFile(filepath As String) As Boolean
        Dim headers As Index(Of String) = RowObject.TryParse(filepath.ReadFirstLine).Indexing
        Dim check_xyz As Boolean = "x" Like headers AndAlso "y" Like headers AndAlso "z" Like headers

        Return check_xyz
    End Function

    Private Sub ImportsPhenographSpotPlot(filepath As String)
        Dim spots As PhenographSpot() = filepath.LoadCsv(Of PhenographSpot).ToArray
        Dim canvas As Size = PixelSelector1.MSICanvas.dimension_size
        Dim spot_pixels = spots.Select(Function(p) p.GetPixel).ToArray
        Dim spot_dims As New Size(
            width:=(Aggregate p In spot_pixels Into Max(p.X)),
            height:=(Aggregate p In spot_pixels Into Max(p.Y))
        )
        Dim dot_size As Double = stdNum.Min(canvas.Width / spot_dims.Width, canvas.Height / spot_dims.Height)
        Dim colors = PhenographSpot _
            .GetSpotColorIndex(spots) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return New SolidBrush(a.Value)
                          End Function)

        Using g As IGraphics = canvas.CreateGDIDevice(filled:=Color.Transparent)
            For i As Integer = 0 To spots.Length - 1
                Dim xy As Point = spot_pixels(i)
                Dim color As Brush = colors(spots(i).phenograph_cluster)

                Call g.DrawCircle(New PointF(xy.X, xy.Y), dot_size, color)
            Next

            Call g.Flush()

            PixelSelector1.MSICanvas.tissue_layer = DirectCast(g, Graphics2D).ImageResource
            PixelSelector1.MSICanvas.RedrawCanvas()
        End Using
    End Sub

    ''' <summary>
    ''' load pipeline cdf file output
    ''' </summary>
    ''' <param name="filepath"></param>
    ''' <param name="file"></param>
    Private Sub ImportsTissueMorphology(filepath As String, file As Stream)
        Dim tissues As TissueRegion()
        Dim dimension As Size
        Dim checkSize As Boolean = True

        Using file
            tissues = New netCDFReader(file).ReadTissueMorphology.ToArray
            file.Seek(Scan0, SeekOrigin.Begin)
            umap3D = New netCDFReader(file).ReadUMAP.ToArray
        End Using

        Using cdffile As New netCDFReader(filepath)
            dimension = cdffile.GetDimension
        End Using

        If stdNum.Abs(PixelSelector1.MSICanvas.dimension_size.Width - dimension.Width) > 5 Then
            checkSize = False
        ElseIf stdNum.Abs(PixelSelector1.MSICanvas.dimension_size.Height - dimension.Height) > 5 Then
            checkSize = False
        End If

        If Not checkSize Then
            If MessageBox.Show(text:=$"The dimension size of the tissue morphology map is very different {vbCrLf}with the MS-imaging dimension size, auto scale of your tissue morphology map raster data?",
                               caption:="Import Tissue Morphology",
                               buttons:=MessageBoxButtons.YesNo,
                               icon:=MessageBoxIcon.Warning) = DialogResult.No Then
                umap3D = Nothing
                Return
            End If

            tissues = tissues _
                .ScalePixels(
                    newDims:=PixelSelector1.MSICanvas.dimension_size,
                    currentDims:=dimension
                ) _
                .ToArray
        End If

        sampleRegions.ShowMessage($"Tissue map {filepath.FileName} has been imported.")
        sampleRegions.importsFile = filepath

        Call ImportsTissueMorphology(tissues)
    End Sub

    Private Sub ImportsTissueMorphology(tissues As TissueRegion())
        sampleRegions.Clear()
        sampleRegions.LoadTissueMaps(tissues, PixelSelector1.MSICanvas)
        sampleRegions.RenderLayer(PixelSelector1.MSICanvas)

        RibbonEvents.ribbonItems.CheckShowMapLayer.BooleanValue = True

        If sampleRegions.DockState = DockState.Hidden Then
            sampleRegions.DockState = DockState.DockRight
        End If
    End Sub

    Sub ExportRegions()
        Call sampleRegions.SaveTissueMorphologyMatrix()
    End Sub
#End Region

    ''' <summary>
    ''' check backend service and show warning message is not success
    ''' </summary>
    ''' <returns></returns>
    Public Function checkService() As Boolean
        If MSIservice Is Nothing OrElse Not MSIservice.MSIEngineRunning Then
            Call Workbench.Warning("No MSI raw data was loaded!")
            Return False
        Else
            Return True
        End If
    End Function

    ''' <summary>
    ''' 成像矩阵热图
    ''' </summary>
    Sub OpenHeatmapMatrixPlot()
        If Not checkService() Then
            Return
        End If

        ' check annotation data and ion data
        Dim docs = MyApplication.host.m_dockPanel _
            .Documents _
            .Where(Function(tab) TypeOf tab Is frmTableViewer) _
            .Select(Function(f) DirectCast(f, frmTableViewer)) _
            .ToArray
        Dim ionStat As frmTableViewer = docs _
            .Where(Function(t)
                       Return t.AppSource Is GetType(IonStat) AndAlso
                              t.InstanceGuid = guid
                   End Function) _
            .FirstOrDefault
        Dim annotation As frmTableViewer = docs _
            .Where(Function(t)
                       Return t.AppSource Is GetType(PageMzSearch) AndAlso
                              t.InstanceGuid = guid
                   End Function) _
            .FirstOrDefault
        Dim ions As New File

        Call ions.Add({"mz", "name", "precursor_type", "pixels", "density"})

        Dim mz As Double()
        Dim name As String()
        Dim precursor_type As String()
        Dim pixels As Integer()
        Dim density As Double()
        Dim getFormula As New InputMatrixIons
        Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)

        If ionStat Is Nothing Then
            Call getFormula.LoadMetabolites()

            If Not getFormula.ion_initialized Then
                Return
            Else
                If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
                    ' Call renderMatrixHeatmap(getFormula)
                End If

                Return
            End If

        ElseIf annotation Is Nothing Then
            MessageBox.Show("No ion annotation, the plot image will only display the m/z value!",
                            "Heatmap Matrix Plot",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning)

            ' no name and precursor type
            mz = ionStat.AdvancedDataGridView1.getFieldVector("mz")
            pixels = ionStat.AdvancedDataGridView1.getFieldVector("pixels")
            density = ionStat.AdvancedDataGridView1.getFieldVector("density")
            name = New String(mz.Length - 1) {}
            precursor_type = New String(mz.Length - 1) {}
        Else
            mz = annotation.AdvancedDataGridView1.getFieldVector("mz")
            name = annotation.AdvancedDataGridView1.getFieldVector("name")
            precursor_type = annotation.AdvancedDataGridView1.getFieldVector("precursorType")

            Dim mzRaw As Double() = ionStat.AdvancedDataGridView1.getFieldVector("mz")
            Dim pixelsRaw As Integer() = ionStat.AdvancedDataGridView1.getFieldVector("pixels")
            Dim density2 As Double() = ionStat.AdvancedDataGridView1.getFieldVector("density")
            Dim mzRawIndex As New Dictionary(Of String, Integer)

            For i As Integer = 0 To mzRaw.Length - 1
                mzRawIndex.Add(mzRaw(i).ToString("F4"), i)
            Next

            Dim index As New List(Of Integer)

            For Each mzi As Double In mz
                index.Add(mzRawIndex(mzi.ToString("F4")))
            Next

            pixels = index.Select(Function(i) pixelsRaw(i)).ToArray
            density = index.Select(Function(i) density2(i)).ToArray
        End If

        Call getFormula.Setup(mz, name, precursor_type, pixels, density)

        If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
            ' Call renderMatrixHeatmap(getFormula)
        End If
    End Sub

    Friend Shared Sub renderMatrixHeatmap(getFormula As InputMatrixIons)
        Dim ionList = getFormula _
            .GetSelectedIons _
            .ToDictionary(Function(a)
                              Return $"{a.Name} {a.Description} {a.Value.ToString("F4")}"
                          End Function,
                          Function(a)
                              Return New Dictionary(Of String, String) From {
                                  {"mz", a.Value},
                                  {"title", a.Name},
                                  {"type", a.Description}
                              }
                          End Function)

        Using file As New SaveFileDialog With {.Filter = "Plot image(*.png)|*.png"}
            If file.ShowDialog = DialogResult.OK Then
                Call MyApplication.LogText($"Rendering for ion list in matrix style: " & ionList.GetJson)
                Call getFormula.TextBox1.AppendText($"Rendering for ion list in matrix style: " & ionList.GetJson)
                Call getFormula.TextBox1.AppendText(vbCrLf)
                Call RscriptProgressTask.ExportHeatMapMatrixPlot(
                    mzSet:=ionList,
                    tolerance:=$"da:{getFormula.txtMzdiff.Text}",
                    saveAs:=file.FileName,
                    debug:=AddressOf getFormula.TextBox1.AppendText,
                    size:=getFormula.CanvasSize,
                    layout:=getFormula.MSILayout，
                    scaler:=getFormula.colorSet
                )
                Call getFormula.TextBox1.AppendText(vbCrLf)
            End If
        End Using
    End Sub

    Sub MSIFeatureDetections()

    End Sub

    Sub DoIonColocalization()
        If Not checkService() Then
            Return
        End If

        Call ProgressSpinner.DoLoading(
            Sub()
                Call Thread.Sleep(500)

                Dim ions As EntityClusterModel() = MSIservice.DoIonCoLocalization({})

                If ions.IsNullOrEmpty Then
                    Call Workbench.Warning("No ions result...")
                Else
                    Call Me.Invoke(Sub() Call ShowIonColocalization(ions))
                End If
            End Sub)
    End Sub

    Private Sub ShowIonColocalization(ions As EntityClusterModel())
        Dim title As String = If(FilePath.StringEmpty, "Ion Co-localization", $"[{FilePath.FileName}]Ion Co-localization")
        Dim table As frmTableViewer = VisualStudio.ShowDocument(Of frmTableViewer)(title:=title)
        Dim blockNames As String() = ions(Scan0).Properties.Keys.ToArray

        table.AppSource = GetType(EntityClusterModel)
        table.InstanceGuid = guid
        table.SourceName = FilePath.FileName Or "MS-Imaging".AsDefault
        table.ViewRow = Sub(row)
                            Dim namePlot As String = ""
                            Dim mz As Double = Val(row("mz"))

                            Call renderByMzList({mz}, namePlot)
                            Call Me.Activate()
                        End Sub

        table.LoadTable(
            Sub(grid)
                Dim v As Object()

                Call grid.Columns.Add("mz", GetType(Double))
                Call grid.Columns.Add("pattern", GetType(String))

                For Each name As String In blockNames
                    Call grid.Columns.Add(name, GetType(Double))
                Next

                For Each ion As EntityClusterModel In ions
                    v = New Object() {ion.ID, ion.Cluster} _
                        .JoinIterates(blockNames.Select(Function(name) CObj(ion(name)))) _
                        .ToArray

                    Call grid.Rows.Add(v)
                    Call System.Windows.Forms.Application.DoEvents()
                Next
            End Sub)
    End Sub

    Sub DoIonStats()
        If Not checkService() Then
            Return
        Else
            Call ProgressSpinner.DoLoading(
                Sub()
                    Call Thread.Sleep(500)
                    Call DoIonStatsInternal()
                End Sub)
        End If
    End Sub

    Private Sub DoIonStatsInternal()
        Dim ions As IonStat() = MSIservice.DoIonStats({})

        If ions.IsNullOrEmpty Then
            Call Workbench.Warning("No ions result...")
        Else
            Call Me.Invoke(Sub() Call DoIonStats(ions, Nothing, Nothing, Nothing))
        End If
    End Sub

    Private Sub DoIonStats(ions As IonStat(), name As String, formula As String, types As MzCalculator())
        Dim title As String = If(FilePath.StringEmpty, "MS-Imaging Ion Stats", $"[{If(name, FilePath.FileName)}]Ion Stats")
        Dim table As frmTableViewer = VisualStudio.ShowDocument(Of frmTableViewer)(title:=title)
        Dim exactMass As Double

        table.AppSource = GetType(IonStat)
        table.InstanceGuid = guid
        table.SourceName = FilePath.FileName Or "MS-Imaging".AsDefault
        table.ViewRow = Sub(row)
                            Dim namePlot As String = ""
                            Dim mz As Double = Val(row("mz"))

                            If Not formula.StringEmpty Then
                                namePlot = $"{name} {row("precursor_type")} {mz.ToString("F4")}"
                            End If

                            Call renderByMzList({mz}, namePlot)
                            Call Me.Activate()
                        End Sub
        table.LoadTable(
            Sub(grid)
                Call grid.Columns.Add("mz", GetType(Double))

                If Not formula.StringEmpty Then
                    exactMass = FormulaScanner.ScanFormula(formula).ExactMass

                    Call grid.Columns.Add("precursor_type")
                End If

                Call grid.Columns.Add("pixels", GetType(Integer))
                Call grid.Columns.Add("density", GetType(Double))
                Call grid.Columns.Add("imaging_score", GetType(Double))
                Call grid.Columns.Add("maxIntensity", GetType(Double))
                Call grid.Columns.Add("basePixel.X", GetType(Integer))
                Call grid.Columns.Add("basePixel.Y", GetType(Integer))
                Call grid.Columns.Add("Q1_intensity", GetType(Double))
                Call grid.Columns.Add("Q2_intensity", GetType(Double))
                Call grid.Columns.Add("Q3_intensity", GetType(Double))
                Call grid.Columns.Add("RSD", GetType(Double))

                For Each ion As IonStat In ions.OrderByDescending(Function(i) i.pixels)
                    If Not formula.StringEmpty Then
                        Dim typeName As String = "n/a"

                        For Each type In types
                            If stdNum.Abs(ion.mz - type.CalcMZ(exactMass)) <= 0.1 Then
                                typeName = type.ToString
                                Exit For
                            End If
                        Next

                        Call grid.Rows.Add(
                            ion.mz.ToString("F4"),
                            typeName,
                            ion.pixels,
                            ion.density.ToString("F2"),
                            (stdNum.Log(ion.pixels + 1) * ion.density).ToString("F2"),
                            stdNum.Round(ion.maxIntensity),
                            ion.basePixelX,
                            ion.basePixelY,
                            stdNum.Round(ion.Q1Intensity),
                            stdNum.Round(ion.Q2Intensity),
                            stdNum.Round(ion.Q3Intensity),
                            stdNum.Round(ion.RSD)
                        )
                    Else
                        Call grid.Rows.Add(
                            ion.mz.ToString("F4"),
                            ion.pixels,
                            ion.density.ToString("F2"),
                            (stdNum.Log(ion.pixels + 1) * ion.density).ToString("F2"),
                            stdNum.Round(ion.maxIntensity),
                            ion.basePixelX,
                            ion.basePixelY,
                            stdNum.Round(ion.Q1Intensity),
                            stdNum.Round(ion.Q2Intensity),
                            stdNum.Round(ion.Q3Intensity),
                            stdNum.Round(ion.RSD)
                        )
                    End If

                    Call System.Windows.Forms.Application.DoEvents()
                Next
            End Sub)
    End Sub

    Private Sub MovePolygonMode()
        ribbonItems.ButtonPolygonEditorMoveVertex.BooleanValue = False
        ribbonItems.ButtonAddNewPolygon.BooleanValue = False
        ribbonItems.ButtonPolygonDeleteVertex.BooleanValue = False
        ribbonItems.ButtonRemovePolygon.BooleanValue = False

        PixelSelector1.MSICanvas.OnMovePolygonMenuItemClick()
    End Sub

    Private Sub AddNewPolygonMode()
        ribbonItems.ButtonMovePolygon.BooleanValue = False
        ribbonItems.ButtonPolygonEditorMoveVertex.BooleanValue = False
        ribbonItems.ButtonPolygonDeleteVertex.BooleanValue = False
        ribbonItems.ButtonRemovePolygon.BooleanValue = False

        PixelSelector1.ShowPointInform = ribbonItems.ButtonShowPolygonVertexInfo.BooleanValue
        PixelSelector1.MSICanvas.OnAddVertexMenuItemClick()
    End Sub

    Friend Sub StartNewPolygon()
        Call MovePolygonMode()
        Call AddNewPolygonMode()

        ribbonItems.ButtonAddNewPolygon.BooleanValue = True
    End Sub

    Sub setupPolygonEditorButtons()
        AddHandler ribbonItems.ButtonNextPolygon.ExecuteEvent,
            Sub()
                Call StartNewPolygon()
            End Sub

        AddHandler ribbonItems.ButtonMovePolygon.ExecuteEvent,
            Sub()
                Call MovePolygonMode()
            End Sub
        AddHandler ribbonItems.ButtonPolygonEditorMoveVertex.ExecuteEvent,
            Sub()
                ribbonItems.ButtonMovePolygon.BooleanValue = False
                ribbonItems.ButtonAddNewPolygon.BooleanValue = False
                ribbonItems.ButtonPolygonDeleteVertex.BooleanValue = False
                ribbonItems.ButtonRemovePolygon.BooleanValue = False

                PixelSelector1.MSICanvas.OnMoveComponentMenuItemClick()
            End Sub
        AddHandler ribbonItems.ButtonAddNewPolygon.ExecuteEvent,
            Sub()
                AddNewPolygonMode()
            End Sub

        AddHandler ribbonItems.ButtonClosePolygonEditor.ExecuteEvent,
            Sub()
                ribbonItems.ButtonMovePolygon.BooleanValue = False
                ribbonItems.ButtonPolygonEditorMoveVertex.BooleanValue = False
                ribbonItems.ButtonAddNewPolygon.BooleanValue = False
                ribbonItems.ButtonPolygonDeleteVertex.BooleanValue = False
                ribbonItems.ButtonRemovePolygon.BooleanValue = False
                ribbonItems.ButtonTogglePolygon.BooleanValue = False

                Call MyApplication.host.Ribbon1.SetModes(0)
                Call MyApplication.host.showStatusMessage("Exit polygon editor!")

                PixelSelector1.SelectPolygonMode = False
                PixelSelector1.Cursor = Cursors.Cross
            End Sub

        AddHandler ribbonItems.ButtonPolygonDeleteVertex.ExecuteEvent,
            Sub()
                ribbonItems.ButtonMovePolygon.BooleanValue = False
                ribbonItems.ButtonPolygonEditorMoveVertex.BooleanValue = False
                ribbonItems.ButtonAddNewPolygon.BooleanValue = False
                ribbonItems.ButtonRemovePolygon.BooleanValue = False

                PixelSelector1.MSICanvas.OnRemoveVertexMenuItemClick()
            End Sub

        AddHandler ribbonItems.ButtonRemovePolygon.ExecuteEvent,
            Sub()
                ribbonItems.ButtonMovePolygon.BooleanValue = False
                ribbonItems.ButtonPolygonEditorMoveVertex.BooleanValue = False
                ribbonItems.ButtonAddNewPolygon.BooleanValue = False
                ribbonItems.ButtonPolygonDeleteVertex.BooleanValue = False

                PixelSelector1.MSICanvas.OnRemovePolygonMenuItemClick()
            End Sub

        AddHandler ribbonItems.ButtonShowPolygonVertexInfo.ExecuteEvent,
            Sub()
                PixelSelector1.ShowPointInform = ribbonItems.ButtonShowPolygonVertexInfo.BooleanValue

                If PixelSelector1.ShowPointInform Then
                    Call MyApplication.host.showStatusMessage("Turn on display point information of polygon vertex.")
                Else
                    Call MyApplication.host.showStatusMessage("Hide point information of polygon vertex!")
                End If
            End Sub

        Call MyApplication.host.Ribbon1.SetModes(0)
    End Sub

    Friend ReadOnly sampleRegions As New MSIRegionSampleWindow

    ''' <summary>
    ''' turn on polygon editor mode
    ''' </summary>
    Sub TogglePolygonMode()
        PixelSelector1.SelectPolygonMode = RibbonEvents.ribbonItems.ButtonTogglePolygon.BooleanValue

        If PixelSelector1.SelectPolygonMode Then
            If Not DrawHeMapRegion Then
                Call MyApplication.host.Ribbon1.SetModes(1)
                Call MyApplication.host.showStatusMessage("Toggle edit polygon for your MS-imaging data!")

                If sampleRegions.DockState = DockState.Hidden Then
                    sampleRegions.DockState = DockState.DockRight
                End If
            Else
                Call MyApplication.host.showStatusMessage("Select region to analysis by draw a polygon!")
            End If

            PixelSelector1.Cursor = Cursors.Default
        Else
            Call MyApplication.host.Ribbon1.SetModes(0)
            Call MyApplication.host.showStatusMessage("Exit polygon editor!")

            PixelSelector1.Cursor = Cursors.Cross
        End If
    End Sub

    Private Sub PixelSelector1_SelectPolygon(polygon() As PointF) Handles PixelSelector1.SelectPolygon
        PixelSelector1.SelectPolygonMode = False
    End Sub

    Sub exportMzPack()
        If Not checkService() Then
            Return
        End If

        Using file As New SaveFileDialog With {.Filter = "mzPack(*.mzPack)|*.mzPack"}
            If file.ShowDialog = DialogResult.OK Then
                Dim fileName As String = file.FileName

                Call TaskProgress.RunAction(
                    Sub(update)
                        MSIservice.MessageCallback = update.Echo
                        MSIservice.ExportMzpack(savefile:=fileName)
                    End Sub, title:="Export mzPack data...", info:="Save mzPack!")
                Call MessageBox.Show($"Export mzPack data at location: {vbCrLf}{fileName}!", "BioNovoGene MSI Viewer", MessageBoxButtons.OK, MessageBoxIcon.Information)

                MSIservice.MessageCallback = Nothing
            End If
        End Using
    End Sub

    ''' <summary>
    ''' load thermo *.raw or *.mzML
    ''' </summary>
    ''' <param name="file"></param>
    Public Sub loadRaw(file As String)
        Dim getSize As New InputMSIDimension
        Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)

        ' MRM data only works for the mzML file
        ' mzXML not able to contains the ion pair data
        If file.ExtensionSuffix("mzml") AndAlso RawScanParser.IsMRMData(file) Then
            Call New Thread(
                Sub()
                    Call Thread.Sleep(1000)
                    Call getSize.Invoke(
                        Sub()
                            getSize.SetTotalScans(mzML.LoadChromatogramList(file).Select(Function(c) Val(c.defaultArrayLength)).Max)
                        End Sub)
                End Sub).Start()
        End If

        If mask.ShowDialogForm(getSize) = DialogResult.OK Then
            guid = file.MD5
            FilePath = file

            ' convert raw to mzpack at first
            Dim tempCache As String = TempFileSystem.GetAppSysTempFile(".mzPack", sessionID:="", prefix:="RAW_VENDOR").ParentPath & $"/{guid}.mzPack"

            If tempCache.FileLength <= 1024 Then
                ' do raw vendor file format converts
                Call TaskProgress.RunAction(run:=Sub() Call PipelineTask.ConvertRaw(FilePath, tempCache),
                                            title:="Pre-processing",
                                            info:="Make data cache of the vendor raw data file...")
            End If

            Call WindowModules.viewer.Show(DockPanel)
            Call WindowModules.msImageParameters.Show(DockPanel)
            Call ServiceHub.MSIDataService.StartMSIService(MSIservice)

            Call TaskProgress.RunAction(
                Sub()
                    Dim info As MsImageProperty = MSIservice.LoadMSI(tempCache, getSize.Dims.SizeParser, AddressOf Workbench.StatusMessage)

                    Call WindowModules.viewer.Invoke(Sub() Call LoadRender(info, file))
                    Call WindowModules.viewer.Invoke(Sub() Call MSIViewerInit0(file))
                End Sub, title:="Load MS-Imaging Raw", info:=$"Loading [{FilePath}]...")

            WindowModules.msImageParameters.DockState = DockState.DockLeft
        Else
            Call Workbench.StatusMessage("User cancel load MSI raw data file...", My.Resources.mintupdate_installing)
        End If
    End Sub

    Const GB As Long = 1024 * 1024 * 1024

    Friend Function MSIViewerInit0(rawfile As String) As String
        If rawfile.FileLength > 1.5 * GB Then
            If MessageBox.Show("The raw data file size is too big, MZKit may takes a very long time to rendering, continute to display the default MS-imaging rendering?",
                               "Display MS-Imaging",
                               MessageBoxButtons.OKCancel,
                               MessageBoxIcon.Exclamation) = DialogResult.OK Then

                WindowModules.viewer.RenderSummary(IntensitySummary.BasePeak)
            Else
                WindowModules.viewer.SetBlank()
            End If
        Else
            WindowModules.viewer.RenderSummary(IntensitySummary.BasePeak)
        End If

        guid = rawfile.MD5

        WindowModules.viewer.DockState = DockState.Document
        WindowModules.msImageParameters.DockState = DockState.DockLeft

        Call Workbench.AppHost.SetTitle($"{WindowModules.viewer.Text} {rawfile.FileName}")

        Return guid
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub loadimzML(file As String)
        guid = MyApplication.host.showMsImaging(imzML:=file)
    End Sub

    ''' <summary>
    ''' load mzpack into MSI engine services
    ''' </summary>
    ''' <param name="filePath"></param>
    Public Sub LoadRender(mzpack As String, filePath As String)
        guid = $"{mzpack}+{filePath}".MD5

        Call TaskProgress.LoadData(
            Function(msg As ITaskProgress)
                Call StartMSIService()
                Call Me.Invoke(Sub() LoadRender(MSIservice.LoadMSI(mzpack, msg.Echo), filePath))

                Return 0
            End Function)
    End Sub

    Sub cleanBackgroundByBasePeak()
        If Not checkService() Then
            Return
        End If

        Dim input As New InputBasePeakIon
        Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)

        If mask.ShowDialogForm(input) = DialogResult.OK Then
            Dim mz As Double = input.IonMz

            Call TaskProgress.LoadData(
                    Function(msg As Action(Of String))
                        Dim info = MSIservice.CutBackground(mz.ToString)

                        If Not info Is Nothing Then
                            Call Me.Invoke(Sub() LoadRender(info, FilePath))
                            Call Me.Invoke(Sub() RenderSummary(IntensitySummary.BasePeak))
                        End If

                        Return 0
                    End Function, taskAssign:=MSIservice.taskHost)
        End If
    End Sub

    Sub cleanBackground(addReference As Boolean)
        If checkService() Then
            Dim filePath = Me.FilePath

            If addReference Then
                Using file As New OpenFileDialog With {.Filter = "All raw data file(*.raw;*.mzML;*.mzPack)|*.raw;*.mzML;*.mzPack"}
                    If file.ShowDialog = DialogResult.OK Then
                        Call TaskProgress.LoadData(
                            Function(msg As Action(Of String))
                                Dim info = MSIservice.CutBackground(file.FileName)

                                Call Me.Invoke(Sub() LoadRender(info, filePath))
                                Call Me.Invoke(Sub() RenderSummary(IntensitySummary.BasePeak))

                                Return 0
                            End Function, taskAssign:=MSIservice.taskHost)
                    End If
                End Using
            Else
                Call TaskProgress.LoadData(
                    Function(msg As Action(Of String))
                        Dim info = MSIservice.CutBackground(Nothing)

                        If Not info Is Nothing Then
                            Call Me.Invoke(Sub() LoadRender(info, filePath))
                            Call Me.Invoke(Sub() RenderSummary(IntensitySummary.BasePeak))
                        End If

                        Return 0
                    End Function, taskAssign:=MSIservice.taskHost)
            End If
        End If
    End Sub

    Private Function ExtractRegionSample(msg As Action(Of String), regions As Polygon2D()) As Integer
        Dim info = MSIservice.ExtractRegionSample(regions, New Size(params.scan_x, params.scan_y))

        If info Is Nothing Then
            Return -1
        End If

        Call Me.Invoke(Sub() LoadRender(info, FilePath))
        Call Me.Invoke(Sub() RenderSummary(IntensitySummary.BasePeak))

        Return 0
    End Function

    ''' <summary>
    ''' 手动选择样本区域生成新的原始数据
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ExtractRegionSampleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExtractRegionSampleToolStripMenuItem.Click
        If Not checkService() Then
            Return
        End If

        Dim regions As Polygon2D() = PixelSelector1.MSICanvas _
            .GetPolygons(popAll:=False) _
            .ToArray

        If regions.Length = 0 Then
            Call Workbench.Warning("No region polygon data was found from polygon editor, draw some region polygon at first!")
            Return
        Else
            PixelSelector1.MSICanvas.ClearSelection()

            If Not sampleRegions Is Nothing Then
                sampleRegions.Clear()
            End If
        End If

        Call TaskProgress.LoadData(
            streamLoad:=Function(msg As Action(Of String))
                            Return ExtractRegionSample(msg, regions)
                        End Function,
            canbeCancel:=True
        )
    End Sub

    ''' <summary>
    ''' set parameters and initialize of the UI
    ''' </summary>
    ''' <param name="filePath"></param>
    Public Sub LoadRender(info As MsImageProperty, filePath As String)
        If info Is Nothing Then
            Return
        End If

        Me.checks = WindowModules.msImageParameters.RenderingToolStripMenuItem
        Me.params = info
        Me.tweaks = WindowModules.msImageParameters.PropertyGrid1
        Me.FilePath = filePath

        PixelSelector1.MSICanvas.ClearSelection()

        If Not sampleRegions Is Nothing Then
            sampleRegions.Clear()
        End If

        WindowModules.msImageParameters.viewer = Me
        WindowModules.msImageParameters.PropertyGrid1.SelectedObject = params
        WindowModules.msImageParameters.ClearIons()
    End Sub

    ''' <summary>
    ''' 渲染多个图层的按钮
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub checks_Click(sender As Object, e As EventArgs) Handles checks.Click
        Dim mz As Double() = WindowModules.msImageParameters _
            .GetSelectedIons _
            .Distinct _
            .ToArray

        If mz.Length = 0 Then
            Call Workbench.Warning("No ions selected for rendering!")
        Else
            Call renderByMzList(mz, Nothing)
        End If
    End Sub

    ''' <summary>
    ''' will overrides <see cref="showPixel"/> if this handler is not nothing
    ''' </summary>
    Friend clickPixel As Action(Of Integer, Integer, Color)

    Private Sub PixelSelector1_GetPixelTissueMorphology(x As Integer, y As Integer, ByRef tag As String) Handles PixelSelector1.GetPixelTissueMorphology
        tag = sampleRegions.GetTissueTag(x, y)
    End Sub

    Private Sub showPixel(x As Integer, y As Integer)
        If Not checkService() Then
            Return
        ElseIf WindowModules.MSIPixelProperty.DockState = DockState.Hidden Then
            WindowModules.MSIPixelProperty.DockState = DockState.DockRight
        End If

        Dim pixel As PixelScan = MSIservice.GetPixel(x, y)
        Dim info As PixelProperty = Nothing

        If pixel Is Nothing Then
            Call Workbench.Warning($"Pixels [{x}, {y}] not contains any data.")
            Call WindowModules.MSIPixelProperty.SetPixel(New InMemoryPixel(x, y, {}), info)
            Call PixelSelector1.ShowMessage($"Pixels [{x}, {y}] not contains any data.")

            Return
        Else
            Call WindowModules.MSIPixelProperty.SetPixel(pixel, info)
            Call PixelSelector1.ShowMessage($"Select {pixel.scanId}, totalIons: {info.TotalIon.ToString("G3")}, basePeak m/z: {info.TopIonMz.ToString("F4")}")
        End If

        Dim ms As New LibraryMatrix With {
            .ms2 = pixel.GetMs,
            .name = pixel.scanId
        }

        If pinedPixel Is Nothing Then
            Call MyApplication.host.mzkitTool.showMatrix(ms.ms2, pixel.scanId)
            Call MyApplication.host.mzkitTool.PlotSpectrum(ms, focusOn:=False)
        Else
            Dim handler As New CosAlignment(Tolerance.PPM(20), New RelativeIntensityCutoff(0.05))
            Dim align As AlignmentOutput = handler.CreateAlignment(ms.ms2, pinedPixel.ms2)

            align.query = New Meta With {.id = ms.name}
            align.reference = New Meta With {.id = pinedPixel.name}

            Call MyApplication.host.mzkitTool.showAlignment(align, showScore:=True)
        End If
    End Sub

    Private Sub showPixel(x As Integer, y As Integer, color As Color) Handles PixelSelector1.SelectPixel
        If Not clickPixel Is Nothing Then
            Call clickPixel(x, y, color)
        Else
            Call showPixel(x, y)
        End If
    End Sub

    Private Sub PixelSelector1_SelectPixelRegion(region As Rectangle) Handles PixelSelector1.SelectPixelRegion
        If Not checkService() Then
            Return
        End If

        Call ProgressSpinner.DoLoading(
            Sub()
                Call ShowRegion(region)
            End Sub)
    End Sub

    Private Sub ShowRegion(region As Rectangle)
        Dim x1 As Integer = region.Left
        Dim y1 As Integer = region.Top
        Dim x2 As Integer = region.Right
        Dim y2 As Integer = region.Bottom
        Dim rangePixels As InMemoryVectorPixel() = MSIservice _
            .GetPixel(x1, y1, x2, y2) _
            .ToArray

        If Not rangePixels.IsNullOrEmpty Then
            Dim ms As New LibraryMatrix With {
                .ms2 = rangePixels _
                    .Select(Function(p) p.GetMs) _
                    .IteratesALL _
                    .ToArray _
                    .Centroid(Tolerance.DeltaMass(0.05), New RelativeIntensityCutoff(0.05)) _
                    .ToArray,
                .name = $"Pixel [{x1},{y1} ~ {x2},{y2}]"
            }

            Call MyApplication.host.Invoke(
                Sub()
                    Call MyApplication.host.mzkitTool.showMatrix(ms.ms2, ms.name)
                    Call MyApplication.host.mzkitTool.PlotSpectrum(ms, focusOn:=False)
                End Sub)
        Else
            Call Workbench.Warning($"target region [{x1}, {y1}, {x2}, {y2}] not contains any data...")
        End If
    End Sub

    Friend Sub RenderSummary(summary As IntensitySummary)
        If Not checkService() Then
            Return
        Else
            Call TaskProgress.RunAction(
                Sub()
                    Call Invoke(Sub() rendering = registerSummaryRender(summary))

                    If Not rendering Is Nothing Then
                        Call Invoke(rendering)
                    End If
                End Sub, "Render MSI", $"Rendering MSI in {summary.Description} mode...")
        End If

        If Not rendering Is Nothing Then
            Call Workbench.SuccessMessage("Rendering Complete!")
            Call PixelSelector1.ShowMessage($"Render MSI in {summary.Description} mode.")
        End If
    End Sub

    Public Sub ExtractSampleRegion()
        If Not checkService() Then
            Return
        Else
            Call TaskProgress.RunAction(
                Sub()
                    Dim panic As Boolean = False
                    Dim summaryLayer As PixelScanIntensity() = MSIservice.ExtractSampleRegion(panic)

                    Invoke(Sub() rendering = registerSummaryRender(summaryLayer, panic))

                    If Not rendering Is Nothing Then
                        Call Invoke(rendering)
                    End If
                End Sub, "Load sample region", $"Extract the sample region data for tissue region operation...")
        End If
    End Sub

    Private Function registerSummaryRender(summary As IntensitySummary) As Action
        Dim panic As Boolean = False
        Dim summaryLayer As PixelScanIntensity() = MSIservice.LoadSummaryLayer(summary, panic)

        Return registerSummaryRender(summaryLayer, panic)
    End Function

    Private Function registerSummaryRender(summaryLayer As PixelScanIntensity(), panic As Boolean) As Action
        If panic Then
            Return Nothing
        Else
            TIC = summaryLayer
        End If

        Dim range As DoubleRange = summaryLayer.Select(Function(i) i.totalIon).Range
        Dim blender As Type = GetType(SummaryMSIBlender) ' (summaryLayer, params, loadFilters)

        Me.blender.SetHEMap(GetHEMap())
        Me.blender.OpenSession(blender, params.GetMSIDimension, Nothing, params, "")
        Me.sampleRegions.SetBounds(summaryLayer.Select(Function(a) New Point(a.x, a.y)))

        Return Sub()
                   Call MyApplication.RegisterPlot(Sub(args) PlotSummary(args, range))
               End Sub
    End Function

    Private Sub PlotSummary(args As PlotProperty, range As DoubleRange)
        Dim image As Image = Me.blender.MSIRender(args, params, PixelSelector1.CanvasSize)
        Dim mapLevels As Integer = params.mapLevels

        PixelSelector1.SetMsImagingOutput(image, params.GetMSIDimension, params.background, params.colors, {range.Min, range.Max}, mapLevels)
        PixelSelector1.SetColorMapVisible(visible:=True)
    End Sub

    Public Sub SetBlank()
        Call ExtractSampleRegion()
    End Sub

    Friend Sub renderRGB(r As Double, g As Double, b As Double)
        Dim selectedMz As Double() = {r, g, b}.Where(Function(mz) mz > 0).ToArray

        If params Is Nothing Then
            Call Workbench.Warning("No MS-imaging data is loaded yet!")
            Return
        End If

        If selectedMz.Count = 1 Then
            MyApplication.host.showStatusMessage($"Run MS-Image rendering for selected ion m/z {selectedMz(Scan0)}...")
        ElseIf selectedMz.Count > 1 Then
            MyApplication.host.showStatusMessage($"Run MS-Image rendering for {selectedMz.Count} selected ions...")
        Else
            MyApplication.host.showStatusMessage("No RGB channels was selected!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        mzdiff = params.GetTolerance
        targetMz = selectedMz
        rgb_configs = New RGBConfigs With {
            .R = New NamedValue(Of Double)(r.ToString("F4"), r),
            .G = New NamedValue(Of Double)(g.ToString("F4"), g),
            .B = New NamedValue(Of Double)(b.ToString("F4"), b)
        }

        Call ProgressSpinner.DoLoading(Sub() Call createRGB(MSIservice.LoadPixels(selectedMz, mzdiff), r, g, b))
        Call PixelSelector1.ShowMessage($"Render in RGB Channel Composition Mode: {selectedMz.Select(Function(d) stdNum.Round(d, 4)).JoinBy(", ")}")
    End Sub

    Private Sub createRGB(pixels As PixelData(), r#, g#, b#)
        If pixels.IsNullOrEmpty Then
            Call Workbench.Warning($"No ion hits!")
        Else
            Dim base = pixels.OrderByDescending(Function(p) p.intensity).FirstOrDefault
            Dim maxInto As Double = base?.intensity
            Dim Rpixels = pixels.Where(Function(p) mzdiff(p.mz, r)).ToArray
            Dim Gpixels = pixels.Where(Function(p) mzdiff(p.mz, g)).ToArray
            Dim Bpixels = pixels.Where(Function(p) mzdiff(p.mz, b)).ToArray

            Call Invoke(Sub() params.SetIntensityMax(maxInto, New Point(base?.x, base?.y)))
            Call Invoke(Sub() rendering = createRenderTask(Rpixels, Gpixels, Bpixels))
            Call Invoke(rendering)
            Call Workbench.SuccessMessage("Rendering Complete!")
        End If
    End Sub

    Private Function createRenderTask(R As PixelData(), G As PixelData(), B As PixelData()) As Action
        Dim blender As Type = GetType(RGBIonMSIBlender) ' (R, G, B, TIC, params, loadFilters)
        Dim mzdiff = params.GetTolerance.GetScript
        Dim configs As New Dictionary(Of String, String) From {
            {"rgb", rgb_configs.GetJSON},
            {"mzdiff", mzdiff}
        }

        Me.params.enableFilter = False
        Me.blender.SetHEMap(GetHEMap())
        Me.blender.OpenSession(blender, params.GetMSIDimension, Nothing, params, configs.GetJson)
        Me.loadedPixels = R _
            .JoinIterates(G) _
            .JoinIterates(B) _
            .ToArray

        Return Sub()
                   Call MyApplication.RegisterPlot(
                       Sub(args)
                           Dim image As Image = Me.blender.MSIRender(args, params, PixelSelector1.CanvasSize)

                           PixelSelector1.SetMsImagingOutput(image, params.GetMSIDimension, params.background, Nothing, Nothing, Nothing)
                           PixelSelector1.SetColorMapVisible(visible:=True)
                       End Sub)
               End Sub
    End Function

    Friend Sub renderByName(name As String, titleName As String)
        title = titleName

        Call Workbench.StatusMessage($"Render layer of annotation: {titleName}")
        Call ProgressSpinner.DoLoading(
            Sub()
                Dim pixels = MSIservice.LoadGeneLayer(name)
                Call RenderPixelsLayer(pixels)
            End Sub)
        Call PixelSelector1.ShowMessage($"Render layer of annotation: {titleName}")
    End Sub

    Private Sub renderEmpty()
        rendering = New Action(Sub()
                               End Sub)
        PixelSelector1.SetMsImagingOutput(
            New Bitmap(params.scan_x, params.scan_y),
            New Size(params.scan_x, params.scan_y),
            params.background,
            params.colors,
            {0, 1},
            1
        )

        Call Workbench.Warning("no pixel data...")
    End Sub

    Private Sub RenderPixelsLayer(pixels As PixelData())
        Dim size As Size = params.GetMSIDimension

        If pixels.IsNullOrEmpty Then
            Call Invoke(Sub() Call renderEmpty())
        Else
            Dim base = pixels.OrderByDescending(Function(p) p.intensity).FirstOrDefault
            Dim maxInto As Double = base?.intensity

            Call Invoke(Sub() params.SetIntensityMax(maxInto, New Point(base?.x, base?.y)))
            Call Invoke(Sub() rendering = createRenderTask(pixels, size))
            Call Invoke(rendering)
            Call Workbench.SuccessMessage("Rendering Complete!")
        End If
    End Sub

    Friend Sub renderByMzList(mz As Double(), titleName As String)
        Dim selectedMz As New List(Of Double)
        Dim dotSize As New Size(3, 3)

        For i As Integer = 0 To mz.Length - 1
            selectedMz.Add(Val(CStr(mz(i))))
        Next

        If selectedMz.Count = 1 Then
            Workbench.StatusMessage($"Run MS-Image rendering for selected ion m/z {selectedMz(Scan0)}...")
        Else
            Workbench.StatusMessage($"Run MS-Image rendering for {selectedMz.Count} selected ions...")
        End If

        mzdiff = params.GetTolerance

        Call SetTitle(selectedMz, titleName)
        Call ProgressSpinner.DoLoading(
            Sub()
                Dim pixels = MSIservice.LoadPixels(selectedMz, mzdiff)
                Call RenderPixelsLayer(pixels)
            End Sub)

        Call PixelSelector1.ShowMessage($"Render in Layer Pixels Composition Mode: {selectedMz.Select(Function(d) stdNum.Round(d, 4)).JoinBy(", ")}")
    End Sub

    ''' <summary>
    ''' the m/z tagged pixel layer data, this data field will be updated after
    ''' the specific m/z data has been fetched from the MSI data service
    ''' </summary>
    Dim loadedPixels As PixelData()
    Dim rgb_configs As RGBConfigs
    Dim targetMz As Double()
    Dim title As String
    Dim mzdiff As Tolerance

    ''' <summary>
    ''' [mz:F3 => name]
    ''' </summary>
    Dim titles As New Dictionary(Of String, String)

    Public Function GetTitle(mz As Double) As String
        Dim key As String = mz.ToString("F3")

        If titles.ContainsKey(key) AndAlso Not titles(key).StringEmpty Then
            Return titles(key)
        Else
            Return $"M/Z: {key}"
        End If
    End Function

    Public Sub SetTitle(mz As IEnumerable(Of Double), title As String)
        Me.title = title
        Me.targetMz = mz.ToArray
        Me.titles(targetMz(Scan0).ToString("F3")) = title
    End Sub

    Public Sub renderByPixelsData(pixels As PixelData(), MsiDim As Size, rgb As RGBConfigs)
        If params Is Nothing Then
            Me.params = New MsImageProperty
            Me.checks = WindowModules.msImageParameters.RenderingToolStripMenuItem
            Me.tweaks = WindowModules.msImageParameters.PropertyGrid1
            Me.FilePath = FilePath

            WindowModules.msImageParameters.PropertyGrid1.SelectedObject = params
            WindowModules.msImageParameters.Win7StyleTreeView1.Nodes.Clear()
        End If

        Dim base = pixels.OrderByDescending(Function(p) p.intensity).FirstOrDefault

        Call params.SetIntensityMax(base?.intensity, New Point(base?.x, base.y))
        Call params.Reset(MsiDim, "N/A", "N/A", 17)
        Call sampleRegions.SetBounds(pixels.Select(Function(a) New Point(a.x, a.y)))

        If rgb Is Nothing Then
            rendering = createRenderTask(pixels, params.GetMSIDimension)
            rendering()
        Else
            rgb_configs = rgb
            mzdiff = params.GetTolerance

            createRGB(pixels, rgb.R, rgb.G, rgb.B)
        End If

        Call Workbench.StatusMessage("Rendering Complete!")
    End Sub

    Public Sub BlendingHEMap(layer As HeatMap.PixelData(), dimensions As Size)
        If params Is Nothing Then
            params = MsImageProperty.Empty(dimensions)
            WindowModules.msImageParameters.PropertyGrid1.SelectedObject = params
            Me.tweaks = WindowModules.msImageParameters.PropertyGrid1
        End If

        Dim blender As Type = GetType(HeatMapBlender) '       (layer, dimensions, params, loadFilters)

        Call Me.blender.channel.WriteBuffer(HeatMap.PixelData.CreateStream(layer))

        Me.params.enableFilter = True
        Me.blender.SetHEMap(GetHEMap())
        Me.blender.OpenSession(ss:=blender, dims:=dimensions, Nothing, params, Nothing)
        Me.rendering =
            Sub()
                Call MyApplication.RegisterPlot(
                    Sub(args)
                        Dim image As Image = Me.blender.MSIRender(args, params, PixelSelector1.CanvasSize)

                        PixelSelector1.SetMsImagingOutput(image, dimensions, params.background, params.colors, {0, 1}, params.mapLevels)
                        PixelSelector1.SetColorMapVisible(visible:=True)
                    End Sub)
            End Sub

        Call rendering()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="pixels"></param>
    ''' <param name="dimensions">the dimension size of the MSI raw data</param>
    ''' <returns></returns>
    Private Function createRenderTask(pixels As PixelData(), dimensions As Size) As Action
        Dim blender As Type = GetType(SingleIonMSIBlender) ' (pixels, TIC, params, loadFilters)
        Dim range As New DoubleRange(pixels.Select(Function(p) p.intensity))

        Me.params.enableFilter = True
        Me.rgb_configs = Nothing
        Me.loadedPixels = pixels
        Me.blender.SetHEMap(GetHEMap())
        Me.blender.OpenSession(blender, dimensions, Nothing, params, Nothing)
        Me.PixelSelector1.MSICanvas.LoadSampleTags(pixels.Select(Function(i) i.sampleTag).Where(Function(str) Not str Is Nothing).Distinct)

        Return Sub()
                   Call MyApplication.RegisterPlot(Sub(args) registerTask(args, dimensions, range))
               End Sub
    End Function

    Private Sub registerTask(args As PlotProperty, dimensions As Size, range As DoubleRange)
        Dim render As Action =
            Sub()
                Dim image As Image = Me.blender.MSIRender(args, params, PixelSelector1.CanvasSize)

                PixelSelector1.SetMsImagingOutput(image, dimensions, params.background, params.colors, {range.Min, range.Max}, params.mapLevels)
                PixelSelector1.SetColorMapVisible(visible:=True)
            End Sub

        Call ProgressSpinner.DoLoading(loading:=Sub() Call Me.Invoke(render))
    End Sub

    Protected Overrides Sub OpenContainingFolder()
        Call Process.Start(FilePath.ParentPath)
    End Sub

    Protected Overrides Sub CopyFullPath()
        Call Clipboard.SetText(FilePath)
    End Sub

    Protected Overrides Sub SaveDocument()
        If PixelSelector1.MSImage Is Nothing Then
            Call MyApplication.host.showStatusMessage("No MSI plot image for output!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Using file As New SaveFileDialog With {.Filter = "PNG image(*.png)|*.png", .Title = "Save MS-Imaging Plot"}
            If file.ShowDialog = DialogResult.OK Then
                Call PixelSelector1.MSImage.SaveAs(file.FileName)
            End If
        End Using
    End Sub

    Private Sub tweaks_PropertyValueChanged(s As Object, e As PropertyValueChangedEventArgs) Handles tweaks.PropertyValueChanged
        If e.ChangedItem.Label.TextEquals("background") AndAlso (blender Is Nothing OrElse blender.Session IsNot GetType(RGBIonMSIBlender)) Then
            PixelSelector1.MSICanvas.BackColor = params.background
        ElseIf Not rendering Is Nothing Then
            Dim grid As PropertyGrid = DirectCast(s, PropertyGrid)
            Dim reason As String = MsImageProperty.Validation(grid.SelectedObject, e)

            If e.ChangedItem.Label.TextEquals("TrIQ") AndAlso blender.Session IsNot GetType(RGBIonMSIBlender) Then
                Call PixelSelector1.UpdateColorScaler({0, blender.GetTrIQIntensity(params.TrIQ)}, params.colors, params.mapLevels)
            End If

            If reason.StringEmpty Then
                Call rendering()
            Else
                Call MessageBox.Show(reason, "MS-Imaging Viewer", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            End If
        Else
            Call MyApplication.host.showStatusMessage("No image for render...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Private Sub frmMsImagingViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True

        If Not checkService() Then
            WindowModules.msImageParameters.DockState = DockState.Hidden
            WindowModules.msImageParameters.checkedMz.Clear()
            WindowModules.msImageParameters.Win7StyleTreeView1.Nodes.Clear()

            Me.DockState = DockState.Hidden
            Return
        Else

        End If

        If MessageBox.Show("Going to close current MS-imaging viewer?",
                           FilePath.FileName,
                           MessageBoxButtons.OKCancel,
                           MessageBoxIcon.Question) = DialogResult.Cancel Then
        Else
            WindowModules.msImageParameters.DockState = DockState.Hidden
            WindowModules.msImageParameters.checkedMz.Clear()
            WindowModules.msImageParameters.Win7StyleTreeView1.Nodes.Clear()

            MSIservice.CloseMSIEngine()
            Me.DockState = DockState.Hidden
        End If
    End Sub

    Private Sub SaveImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveImageToolStripMenuItem.Click
        Call SaveDocument()
    End Sub

    Private Sub ExportMatrixToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportMatrixToolStripMenuItem.Click
        Dim loadedPixels As PixelData() = Me.loadedPixels

        If loadedPixels.IsNullOrEmpty AndAlso TIC.IsNullOrEmpty Then
            Call Workbench.Warning("No loaded pixels data...")
            Return
        End If

        Dim dimension As New Size(params.scan_x, params.scan_y)

        If loadedPixels.IsNullOrEmpty Then
            If MessageBox.Show("No ion layer data could be exports, export the summary heatmap layer of your ms-imaging data?",
                               "Export Heatmap Matrix",
                               MessageBoxButtons.OKCancel,
                               MessageBoxIcon.Question) = DialogResult.OK Then

                Dim into_range As New DoubleRange(TIC.Select(Function(i) i.totalIon))
                Dim level As New DoubleRange(0, 1)

                loadedPixels = TIC _
                    .Select(Function(pi)
                                Return New PixelData With {
                                    .intensity = pi.totalIon,
                                    .x = pi.x,
                                    .y = pi.y,
                                    .level = into_range.ScaleMapping(pi.totalIon, level),
                                    .mz = 0,
                                    .sampleTag = Nothing
                                }
                            End Function) _
                    .ToArray

                Call Workbench.Warning("No ion layer data could be exports, the summary heatmap layer of your ms-imaging data was exports!")
            Else
                Return
            End If
        End If

        Using file As New SaveFileDialog With {.Filter = "NetCDF(*.cdf)|*.cdf", .Title = "Save MS-Imaging Matrix"}
            If file.ShowDialog = DialogResult.OK Then
                Using filesave As FileStream = file.FileName.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                    Call loadedPixels.CreateCDF(filesave, dimension, params.GetTolerance, rgb:=rgb_configs)
                End Using
            End If
        End Using
    End Sub

    Dim pinedPixel As LibraryMatrix

    Private Sub PinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PinToolStripMenuItem.Click
        Dim pos As Point = PixelSelector1.MSICanvas.Pixel
        Dim pixel As PixelScan

        If Not checkService() Then
            Return
        Else
            pixel = MSIservice.GetPixel(pos.X, pos.Y)
            pinedPixel = New LibraryMatrix With {
                .ms2 = pixel?.GetMs,
                .name = $"Select Pixel: [{pos.X},{pos.Y}]"
            }

            If pixel Is Nothing OrElse pinedPixel.ms2.IsNullOrEmpty Then
                pinedPixel = Nothing
                Workbench.Warning("There is no MS data in current pixel?")
            Else
                Call WindowModules.msImageParameters.LoadPinnedIons(pinedPixel.ms2)
            End If
        End If
    End Sub

    Private Sub ClearPinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem.Click
        pinedPixel = Nothing
    End Sub

    Private Sub ClearSamplesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearToolStripMenuItem1.Click
        sampleRegions.Clear()
    End Sub

    Private Sub AddSampleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddSampleToolStripMenuItem.Click
        If PixelSelector1.MSICanvas.HasRegionSelection Then
            If Not DrawHeMapRegion Then
                Call sampleRegions.Add(PixelSelector1.MSICanvas)
            Else
                Dim regions = PixelSelector1.MSICanvas _
                  .GetPolygons(popAll:=True) _
                  .ToArray

                Call HEMap.Add(regions)
            End If

            Call StartNewPolygon()
        Else
            Call Workbench.Warning("No sample region was selected!")
        End If
    End Sub

    Private Shared Function exportAllSpotSamplePeaktable(noUI As Boolean, filePath As String) As String
        ' export all spots
        Using file As New SaveFileDialog With {.Filter = "Excel Table(*.csv)|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                Call InputDialog.Input(Of InputMSIPeakTableParameters)(
                    Sub(cfg)
                        Call RscriptProgressTask.CreateMSIPeakTable(
                            mzpack:=filePath,
                            saveAs:=file.FileName,
                            cfg.Mzdiff, cfg.IntoCutoff, cfg.TrIQCutoff,
                            noUI
                        )
                    End Sub)

                If file.FileName.FileExists(ZERO_Nonexists:=True) Then
                    Return file.FileName
                Else
                    ' user cancel or task error
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        End Using
    End Function

    Private Sub exportMSISampleTable()
        If Not checkService() Then
            ' select file and export all pixel spots
            Using file As New OpenFileDialog With {.Filter = "BioNovoGene mzPack(*.mzPack)|*.mzPack"}
                If file.ShowDialog = DialogResult.OK Then
                    Call exportAllSpotSamplePeaktable(noUI:=False, filePath:=file.FileName)
                End If
            End Using

            ' Call Workbench.Warning("No MSI raw data is loaded!")
            Return
        End If

        If sampleRegions.IsNullOrEmpty Then
            ' Call Workbench.Warning("No sample spot regions!")
            Call exportAllSpotSamplePeaktable(noUI:=False, filePath:=FilePath)
        Else
            Using file As New SaveFileDialog With {.Filter = "Excel Table(*.csv)|*.csv"}
                If file.ShowDialog = DialogResult.OK Then
                    Call RscriptProgressTask.CreateMSIPeakTable(
                        mzpack:=FilePath,
                        saveAs:=file.FileName,
                        exportTissueMaps:=Sub(buffer)
                                              Call sampleRegions.ExportTissueMaps(sampleRegions.dimension, buffer)
                                          End Sub)
                End If
            End Using
        End If
    End Sub

    Private Sub ExportPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportPlotToolStripMenuItem.Click
        If Not checkService() Then
            Return
        ElseIf targetMz.IsNullOrEmpty Then
            Call Workbench.Warning("No ion was selected to export MS-Imaging plot!")
            Return
        End If

        Dim filename As String

        If targetMz.Length > 1 Then
            filename = targetMz.Select(Function(d) d.ToString("F3")).JoinBy("+") & ".png"
        Else
            If title.StringEmpty Then
                filename = targetMz(0).ToString("F4")
            Else
                filename = title.NormalizePathString(False)
            End If
        End If

        Dim save As New SetMSIPlotParameters

        Call save.SetFileName(filename)
        Call InputDialog.Input(
            setConfig:=Sub(cfg)
                           If targetMz.Length > 1 Then
                               Call RscriptProgressTask.ExportRGBIonsPlot(targetMz, mzdiff.GetScript, saveAs:=save.FileName)
                           Else
                               Call RscriptProgressTask.ExportSingleIonPlot(
                                   mz:=targetMz(0),
                                   tolerance:=mzdiff.GetScript,
                                   saveAs:=save.FileName,
                                   title:=title,
                                   background:=params.background.ToHtmlColor,
                                   colorSet:=params.colors.Description,
                                   overlapTotalIons:=params.showTotalIonOverlap
                               )
                           End If
                       End Sub,
            config:=save
        )
    End Sub

    Private Sub ImageProcessingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImageProcessingToolStripMenuItem.Click
        Dim image As Image = PixelSelector1.MSImage
        Dim file As String = TempFileSystem.GetAppSysTempFile(".app", sessionID:=App.PID, prefix:="saveimage___") & "/MSImaging.png"
        Dim editor As New LaplacianHDR.FormEditMain(loadfile:=file)

        Call image.SaveAs(file)
        Call InputDialog.Input(Of LaplacianHDR.FormEditMain)(
            Sub()

            End Sub, config:=editor)
    End Sub

    Private Sub CopyImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyImageToolStripMenuItem.Click
        Clipboard.Clear()
        Clipboard.SetImage(PixelSelector1.MSImage)

        Call MyApplication.host.showStatusMessage("MS-imaging plot has been copied to the clipboard!")
    End Sub

    Private Sub PixelSelector1_SelectSample(tag As String) Handles PixelSelector1.SelectSample
        If blender Is Nothing Then
            Return
        Else
            blender.SetSampleTag(tag)
            Call rendering()
        End If
    End Sub

    Private Sub AddSpatialTileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddSpatialTileToolStripMenuItem.Click
        Using file As New OpenFileDialog With {
            .Filter = "10X Space Ranger Spots(*.csv)|*.csv|MZKit Spatial Mapping(*.xml)|*.xml|Erica Spatial HeatMap(*.cdf;*.netcdf;*.nc)|*.cdf;*.netcdf;*.nc",
            .Title = "Open a new tissue positions list"
        }
            If file.ShowDialog = DialogResult.OK Then
                If file.FileName.ExtensionSuffix("csv") Then
                    Dim spots As SpatialSpot() = SpaceRanger _
                        .LoadTissueSpots(file.FileName.ReadAllLines) _
                        .ToArray
                    Dim tile = PixelSelector1 _
                        .MSICanvas _
                        .AddSpatialTile(spots)

                    Call MoveToMouseLocation(tile)
                ElseIf file.FileName.ExtensionSuffix("cdf", "netcdf", "nc") Then
                    Dim heatmap As SpatialHeatMap = SpatialHeatMap.LoadCDF(file.OpenFile)
                    Dim tile = PixelSelector1 _
                        .MSICanvas _
                        .AddSpatialTile(heatmap)

                    Call MoveToMouseLocation(tile)
                Else
                    Dim maps As SpatialMapping = file.FileName.LoadXml(Of SpatialMapping)

                    Call PixelSelector1 _
                        .MSICanvas _
                        .AddSpatialMapping(maps)
                End If
            End If
        End Using
    End Sub

    Private Sub MoveToMouseLocation(tile As SpatialTile)
        ' move the spatial tile to the mouse location
        Dim tilePos = PixelSelector1.MSICanvas.PointToClient(Cursor.Position)

        tilePos = New Point With {
            .X = If(tilePos.X < 0, 0, tilePos.X),
            .Y = If(tilePos.Y < 0, 0, tilePos.Y)
        }
        tile.Location = tilePos
    End Sub

    ''' <summary>
    ''' remove region
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RemoveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveToolStripMenuItem.Click
        If Not checkService() Then
            Return
        End If

        Dim regions = PixelSelector1.MSICanvas _
            .GetPolygons(popAll:=False) _
            .ToArray

        If regions.Length = 0 Then
            Call Workbench.Warning("No region polygon data was found from polygon editor, draw some region polygon at first!")
            Return
        Else
            PixelSelector1.MSICanvas.ClearSelection()

            If Not sampleRegions Is Nothing Then
                sampleRegions.Clear()
            End If
        End If

        Call TaskProgress.LoadData(
                Function(msg As Action(Of String))
                    Dim info = MSIservice.DeleteRegionDataPolygon(regions, New Size(params.scan_x, params.scan_y))

                    If info Is Nothing Then
                        Return -1
                    End If

                    Call Me.Invoke(Sub() LoadRender(info, FilePath))
                    Call Me.Invoke(Sub() RenderSummary(IntensitySummary.BasePeak))

                    Return 0
                End Function, canbeCancel:=True)
    End Sub

    Private Sub PixelSelector1_SetRange(range As DoubleRange) Handles PixelSelector1.SetRange
        If Not blender Is Nothing Then
            Call blender.SetIntensityRange(range)
            Call rendering()
        End If
    End Sub

    Private Sub frmMsImagingViewer_LocationChanged(sender As Object, e As EventArgs) Handles Me.LocationChanged

    End Sub
End Class
