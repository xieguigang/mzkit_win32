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
Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.mzkit_win32.My
Imports ControlLibrary
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization.JSON
Imports mzblender
Imports Task
Imports WeifenLuo.WinFormsUI.Docking
Imports File = Microsoft.VisualBasic.Data.csv.IO.File
Imports stdNum = System.Math

Public Class frmMsImagingViewer
    Implements IFileReference

    Public Property FilePath As String Implements IFileReference.FilePath

    Dim WithEvents checks As ToolStripMenuItem
    Dim WithEvents tweaks As PropertyGrid
    Dim rendering As Action
    Dim guid As String
    Dim blender As MSImagingBlender

    Friend MSIservice As ServiceHub.MSIDataService
    Friend params As MsImageProperty
    Friend HEMap As HEMapTools
    Friend DrawHeMapRegion As Boolean = False

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

    Public Sub StartMSIService()
        Call ServiceHub.MSIDataService.StartMSIService(hostOld:=MSIservice)
    End Sub

    Private Sub frmMsImagingViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = Text

        WindowModules.msImageParameters.DockState = DockState.DockLeft

        AddHandler RibbonEvents.ribbonItems.ButtonMSITotalIon.ExecuteEvent, Sub() Call RenderSummary(IntensitySummary.Total)
        AddHandler RibbonEvents.ribbonItems.ButtonMSIBasePeakIon.ExecuteEvent, Sub() Call RenderSummary(IntensitySummary.BasePeak)
        AddHandler RibbonEvents.ribbonItems.ButtonMSIAverageIon.ExecuteEvent, Sub() Call RenderSummary(IntensitySummary.Average)

        AddHandler RibbonEvents.ribbonItems.ButtonExportSample.ExecuteEvent, Sub() Call exportMSISampleTable()
        AddHandler RibbonEvents.ribbonItems.ButtonExportMSIMzpack.ExecuteEvent, Sub() Call exportMzPack()
        AddHandler RibbonEvents.ribbonItems.ButtonTogglePolygon.ExecuteEvent, Sub() Call TogglePolygonMode()
        AddHandler RibbonEvents.ribbonItems.CleanBackgroundAutoReference.ExecuteEvent, Sub() Call cleanBackground(addReference:=False)
        AddHandler RibbonEvents.ribbonItems.CleanBackgroundByReference.ExecuteEvent, Sub() Call cleanBackground(addReference:=True)
        AddHandler RibbonEvents.ribbonItems.ButtonMSIRawIonStat.ExecuteEvent, Sub() Call DoIonStats()
        AddHandler RibbonEvents.ribbonItems.ButtonMSIMatrixVisual.ExecuteEvent, Sub() Call OpenHeatmapMatrixPlot()
        AddHandler RibbonEvents.ribbonItems.ButtonUpsideDown.ExecuteEvent, Sub() Call TurnUpsideDown()
        AddHandler RibbonEvents.ribbonItems.ButtonImportsTissueMorphology.ExecuteEvent, Sub() Call ImportsTissueMorphology()
        AddHandler RibbonEvents.ribbonItems.ButtonExportRegions.ExecuteEvent, Sub() Call ExportRegions()
        AddHandler RibbonEvents.ribbonItems.ButtonMSISearchPubChem.ExecuteEvent, Sub() Call SearchPubChem()
        AddHandler RibbonEvents.ribbonItems.ButtonLoadHEMap.ExecuteEvent, Sub() Call loadHEMap()

        AddHandler RibbonEvents.ribbonItems.CheckShowMapLayer.ExecuteEvent,
            Sub()
                If RibbonEvents.ribbonItems.CheckShowMapLayer.BooleanValue Then
                    Call sampleRegions.RenderLayer(PixelSelector1)
                Else
                    Call sampleRegions.ClearLayer(PixelSelector1)
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

        sampleRegions.Show(MyApplication.host.dockPanel)
        sampleRegions.DockState = DockState.Hidden
    End Sub

    Sub loadHEMap()
        Using file As New OpenFileDialog With {.Filter = "HE Stain Image(*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp"}
            If file.ShowDialog = DialogResult.OK Then
                PixelSelector1.HEMap = New Bitmap(file.FileName.LoadImage)

                If blender IsNot Nothing AndAlso TypeOf blender IsNot HeatMapBlender Then
                    blender.HEMap = PixelSelector1.HEMap
                    rendering()
                Else
                    ' just display hemap on the canvas
                    PixelSelector1.SetMsImagingOutput(PixelSelector1.HEMap, PixelSelector1.HEMap.Size, New Size(1, 1), Drawing2D.Colors.ScalerPalette.Jet, {0, 255}, 120)
                End If

                If HEMap Is Nothing Then
                    HEMap = New HEMapTools
                    HEMap.Show(VisualStudio.DockPanel)
                    HEMap.DockState = DockState.Hidden
                End If

                HEMap.Clear(PixelSelector1.HEMap)

                VisualStudio.Dock(HEMap, DockState.DockRight)
            End If
        End Using
    End Sub

    Sub TurnUpsideDown()
        If Not checkService() Then
            Return
        ElseIf MessageBox.Show("This operation will makes the entire MSImaging plot upside down.", "MSI Data Services", buttons:=MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
            Call frmTaskProgress.LoadData(
                Function(msg As Action(Of String))
                    Dim info = MSIservice.TurnUpsideDown

                    If Not info Is Nothing Then
                        Call Me.Invoke(Sub() LoadRender(info, FilePath))
                        Call Me.Invoke(Sub() RenderSummary(IntensitySummary.BasePeak))
                        Call Me.Invoke(Sub() Call StartNewPolygon())
                        Call Me.Invoke(Sub() Call sampleRegions.TurnUpsideDown(Me.PixelSelector1))
                    End If

                    Return 0
                End Function, taskAssign:=MSIservice.taskHost)
        End If
    End Sub

    Sub SearchPubChem()
        Dim getFormula As New InputPubChemProxy
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

        If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
            If checkService() Then
                Dim formula As String = getFormula.GetAnnotation.formula
                Dim mass As Double = FormulaScanner.ScanFormula(formula).ExactMass
                Dim progress As New frmProgressSpinner
                ' evaluate m/z
                Dim mz As Double() = Provider.Positives.Select(Function(t) t.CalcMZ(mass)).ToArray

                Call New Thread(
                    Sub()
                        Call Thread.Sleep(500)

                        Dim ions As IonStat() = MSIservice.DoIonStats(mz)

                        If ions.IsNullOrEmpty Then
                            Call MyApplication.host.warning("No ions result...")
                        Else
                            Call Me.Invoke(Sub()
                                               Call DoIonStats(ions, getFormula.GetAnnotation.name, formula, Provider.Positives)
                                           End Sub)
                        End If

                        Call progress.CloseWindow()
                    End Sub).Start()

                Call progress.ShowDialog()
            Else
                Call MyApplication.host.showStatusMessage($"The MS-imaging backend services is not running for rendering {getFormula.GetAnnotation.name}!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            End If
        End If
    End Sub

#Region "Tissue Map"
    Sub ImportsTissueMorphology()
        If Not checkService() Then
            Return
        ElseIf PixelSelector1.dimension_size.IsEmpty Then
            Call MyApplication.host.showStatusMessage("No ms-imaging rendering output!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Using file As New OpenFileDialog With {.Filter = "Tissue Morphology Matrix(*.cdf)|*.cdf"}
            If file.ShowDialog = DialogResult.OK Then
                Dim tissues = file.OpenFile.ReadTissueMorphology

                sampleRegions.Clear()
                sampleRegions.LoadTissueMaps(tissues, PixelSelector1)
                sampleRegions.RenderLayer(PixelSelector1)

                RibbonEvents.ribbonItems.CheckShowMapLayer.BooleanValue = True

                If sampleRegions.DockState = DockState.Hidden Then
                    sampleRegions.DockState = DockState.DockRight
                End If
            End If
        End Using
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
            Call MyApplication.host.showStatusMessage("No MSI raw data was loaded!", My.Resources.StatusAnnotations_Warning_32xLG_color)
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
        Dim docs = MyApplication.host.dockPanel _
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
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

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
            MessageBox.Show("No ion annotation, the plot image will only display the m/z value!", "Heatmap Matrix Plot", MessageBoxButtons.OK, MessageBoxIcon.Warning)

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
                    debug:=AddressOf getFormula.TextBox1.AppendText
                )
                Call getFormula.TextBox1.AppendText(vbCrLf)
            End If
        End Using
    End Sub

    Sub MSIFeatureDetections()

    End Sub

    Sub DoIonStats()
        Dim progress As New frmProgressSpinner

        If Not checkService() Then
            Return
        End If

        Call New Thread(
            Sub()
                Call Thread.Sleep(500)

                Dim ions As IonStat() = MSIservice.DoIonStats({})

                If ions.IsNullOrEmpty Then
                    Call MyApplication.host.warning("No ions result...")
                Else
                    Call Me.Invoke(Sub() Call DoIonStats(ions, Nothing, Nothing, Nothing))
                End If

                Call progress.CloseWindow()
            End Sub).Start()

        Call progress.ShowDialog()
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

                    Call Application.DoEvents()
                Next
            End Sub)
    End Sub

    Private Sub MovePolygonMode()
        ribbonItems.ButtonPolygonEditorMoveVertex.BooleanValue = False
        ribbonItems.ButtonAddNewPolygon.BooleanValue = False
        ribbonItems.ButtonPolygonDeleteVertex.BooleanValue = False
        ribbonItems.ButtonRemovePolygon.BooleanValue = False

        PixelSelector1.OnMovePolygonMenuItemClick()
    End Sub

    Private Sub AddNewPolygonMode()
        ribbonItems.ButtonMovePolygon.BooleanValue = False
        ribbonItems.ButtonPolygonEditorMoveVertex.BooleanValue = False
        ribbonItems.ButtonPolygonDeleteVertex.BooleanValue = False
        ribbonItems.ButtonRemovePolygon.BooleanValue = False

        PixelSelector1.OnAddVertexMenuItemClick()
    End Sub

    Private Sub StartNewPolygon()
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

                PixelSelector1.OnMoveComponentMenuItemClick()
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

                PixelSelector1.OnRemoveVertexMenuItemClick()
            End Sub

        AddHandler ribbonItems.ButtonRemovePolygon.ExecuteEvent,
            Sub()
                ribbonItems.ButtonMovePolygon.BooleanValue = False
                ribbonItems.ButtonPolygonEditorMoveVertex.BooleanValue = False
                ribbonItems.ButtonAddNewPolygon.BooleanValue = False
                ribbonItems.ButtonPolygonDeleteVertex.BooleanValue = False

                PixelSelector1.OnRemovePolygonMenuItemClick()
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

                Call frmTaskProgress.RunAction(
                    Sub(update)
                        MSIservice.MessageCallback = update
                        MSIservice.ExportMzpack(savefile:=fileName)
                    End Sub, title:="Export mzPack data...", info:="Save mzPack!")
                Call MessageBox.Show($"Export mzPack data at location: {vbCrLf}{fileName}!", "BioNovoGene MSI Viewer", MessageBoxButtons.OK, MessageBoxIcon.Information)

                MSIservice.MessageCallback = Nothing
            End If
        End Using
    End Sub

    ''' <summary>
    ''' load thermo raw
    ''' </summary>
    ''' <param name="file"></param>
    Public Sub loadRaw(file As String)
        Dim getSize As New InputMSIDimension
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

        If mask.ShowDialogForm(getSize) = DialogResult.OK Then
            Dim progress As New frmProgressSpinner

            guid = file.MD5
            FilePath = file

            Call WindowModules.viewer.Show(DockPanel)
            Call WindowModules.msImageParameters.Show(DockPanel)
            Call ServiceHub.MSIDataService.StartMSIService(MSIservice)

            Call New Thread(
                Sub()
                    Dim info As MsImageProperty = MSIservice.LoadMSI(file, getSize.Dims.SizeParser, Sub(msg) MyApplication.host.showStatusMessage(msg))

                    Call WindowModules.viewer.Invoke(Sub() Call LoadRender(info, file))
                    Call WindowModules.viewer.Invoke(Sub() WindowModules.viewer.DockState = DockState.Document)

                    Call progress.CloseWindow()

                    MyApplication.host.Invoke(
                        Sub()
                            MyApplication.host.Text = $"BioNovoGene Mzkit [{WindowModules.viewer.Text} {file.FileName}]"
                        End Sub)
                End Sub).Start()

            WindowModules.msImageParameters.DockState = DockState.DockLeft

            Call progress.ShowDialog()
        Else
            Call MyApplication.host.showStatusMessage("User cancel load MSI raw data file...")
        End If
    End Sub

    Public Sub loadmzML(file As String)
        Dim getSize As New InputMSIDimension

        If getSize.ShowDialog = DialogResult.OK Then
            guid = file.MD5
        End If
    End Sub

    Public Sub loadimzML(file As String)
        MyApplication.host.showMsImaging(imzML:=file)
        guid = file.MD5
    End Sub

    ''' <summary>
    ''' load mzpack into MSI engine services
    ''' </summary>
    ''' <param name="filePath"></param>
    Public Sub LoadRender(mzpack As String, filePath As String)
        guid = $"{mzpack}+{filePath}".MD5

        Call frmTaskProgress.LoadData(
            Function(msg As Action(Of String))
                Call ServiceHub.MSIDataService.StartMSIService(MSIservice)
                Call Me.Invoke(Sub() LoadRender(MSIservice.LoadMSI(mzpack, msg), filePath))

                Return 0
            End Function)
    End Sub

    Sub cleanBackground(addReference As Boolean)
        If checkService() Then
            Dim filePath = Me.FilePath

            If addReference Then
                Using file As New OpenFileDialog With {.Filter = "All raw data file(*.raw;*.mzML;*.mzPack)|*.raw;*.mzML;*.mzPack"}
                    If file.ShowDialog = DialogResult.OK Then
                        Call frmTaskProgress.LoadData(
                            Function(msg As Action(Of String))
                                Dim info = MSIservice.CutBackground(file.FileName)

                                Call Me.Invoke(Sub() LoadRender(info, filePath))
                                Call Me.Invoke(Sub() RenderSummary(IntensitySummary.BasePeak))

                                Return 0
                            End Function, taskAssign:=MSIservice.taskHost)
                    End If
                End Using
            Else
                Call frmTaskProgress.LoadData(
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

    Private Sub ExtractRegionSampleToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExtractRegionSampleToolStripMenuItem.Click
        If Not checkService() Then
            Return
        End If

        Dim regions = PixelSelector1 _
            .GetPolygons(popAll:=False) _
            .ToArray

        If regions.Length = 0 Then
            Call MyApplication.host.showStatusMessage("No region polygon data was found from polygon editor, draw some region polygon at first!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Call frmTaskProgress.LoadData(
                Function(msg As Action(Of String))
                    Dim info = MSIservice.ExtractRegionSample(regions, New Size(params.scan_x, params.scan_y))

                    If info Is Nothing Then
                        Return -1
                    End If

                    Call Me.Invoke(Sub() LoadRender(info, FilePath))
                    Call Me.Invoke(Sub() RenderSummary(IntensitySummary.BasePeak))

                    Return 0
                End Function)
    End Sub

    ''' <summary>
    ''' load mzpack into MSI engine services
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
            Call MyApplication.host.showStatusMessage("No ions selected for rendering!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call renderByMzList(mz, Nothing)
        End If
    End Sub

    Friend clickPixel As Action(Of Integer, Integer, Color)

    Private Sub showPixel(x As Integer, y As Integer, color As Color) Handles PixelSelector1.SelectPixel
        If Not clickPixel Is Nothing Then
            clickPixel(x, y, color)
            Return
        End If
        If Not checkService() Then
            Return
        ElseIf WindowModules.MSIPixelProperty.DockState = DockState.Hidden Then
            WindowModules.MSIPixelProperty.DockState = DockState.DockRight
        End If

        Dim pixel As PixelScan = MSIservice.GetPixel(x, y)
        Dim info As PixelProperty = Nothing

        If pixel Is Nothing Then
            Call MyApplication.host.showStatusMessage($"Pixels [{x}, {y}] not contains any data.", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Call WindowModules.MSIPixelProperty.SetPixel(New InMemoryPixel(x, y, {}), info)
            Call PixelSelector1.ShowMessage($"Pixels [{x}, {y}] not contains any data.")

            Return
        Else
            Call WindowModules.MSIPixelProperty.SetPixel(pixel, info)
            Call PixelSelector1.ShowMessage($"Select {pixel.scanId}, totalIons: {info.TotalIon.ToString("G3")}, basePeak m/z: {info.TopIonMz.ToString("F4")}")
        End If

        Dim ms As New LibraryMatrix With {
            .ms2 = pixel.GetMs,
            .name = $"Pixel[{x}, {y}]"
        }

        If pinedPixel Is Nothing Then
            Call MyApplication.host.mzkitTool.showMatrix(ms.ms2, $"Pixel[{x}, {y}]")
            Call MyApplication.host.mzkitTool.PlotSpectrum(ms, focusOn:=False)
        Else
            Dim handler As New CosAlignment(Tolerance.PPM(20), New RelativeIntensityCutoff(0.05))
            Dim align As AlignmentOutput = handler.CreateAlignment(ms.ms2, pinedPixel.ms2)

            align.query = New Meta With {.id = ms.name}
            align.reference = New Meta With {.id = pinedPixel.name}

            Call MyApplication.host.mzkitTool.showAlignment(align, showScore:=True)
        End If
    End Sub

    Private Sub PixelSelector1_SelectPixelRegion(region As Rectangle) Handles PixelSelector1.SelectPixelRegion
        If Not checkService() Then
            Return
        End If

        Dim progress As New frmProgressSpinner

        Call New Thread(
            Sub()
                Call ShowRegion(region)
                Call progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        Call progress.ShowDialog()
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
            Call MyApplication.host.showStatusMessage($"target region [{x1}, {y1}, {x2}, {y2}] not contains any data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Friend Sub RenderSummary(summary As IntensitySummary)
        If Not checkService() Then
            Return
        Else
            Call frmTaskProgress.RunAction(
                Sub()
                    Call Invoke(Sub() rendering = registerSummaryRender(summary))
                    Call Invoke(rendering)
                End Sub, "Render MSI", $"Rendering MSI in {summary.Description} mode...")
        End If

        Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
        Call PixelSelector1.ShowMessage($"Render MSI in {summary.Description} mode.")
    End Sub

    Private Function registerSummaryRender(summary As IntensitySummary) As Action
        Dim summaryLayer As PixelScanIntensity() = MSIservice.LoadSummaryLayer(summary)
        Dim range As DoubleRange = summaryLayer.Select(Function(i) i.totalIon).Range
        Dim blender As New SummaryMSIBlender(summaryLayer, params)

        Me.blender = blender
        Me.sampleRegions.SetBounds(summaryLayer.Select(Function(a) New Point(a.x, a.y)))

        Return Sub()
                   Call MyApplication.RegisterPlot(
                       Sub(args)
                           Dim image As Image = blender.Rendering(args, PixelSelector1.CanvasSize)
                           Dim mapLevels As Integer = params.mapLevels

                           PixelSelector1.SetMsImagingOutput(image, blender.dimensions, blender.dotSize, params.colors, {range.Min, range.Max}, mapLevels)
                           PixelSelector1.BackColor = params.background
                           PixelSelector1.SetColorMapVisible(visible:=params.showColorMap)
                       End Sub)
               End Sub
    End Function

    Friend Sub renderRGB(r As Double, g As Double, b As Double)
        Dim selectedMz As Double() = {r, g, b}.Where(Function(mz) mz > 0).ToArray
        Dim progress As New frmProgressSpinner

        If params Is Nothing Then
            Call MyApplication.host.warning("No MS-imaging data is loaded yet!")
            Return
        End If

        Dim size As String = $"{params.pixel_width},{params.pixel_height}"

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

        Call New Thread(
            Sub()
                Dim pixels As PixelData() = MSIservice.LoadPixels(selectedMz, mzdiff)

                If pixels.IsNullOrEmpty Then
                    Call MyApplication.host.showStatusMessage($"No ion hits!", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Else
                    Dim maxInto As Double = Aggregate pm As PixelData
                                            In pixels
                                            Into Max(pm.intensity)
                    Dim Rpixels = pixels.Where(Function(p) mzdiff(p.mz, r)).ToArray
                    Dim Gpixels = pixels.Where(Function(p) mzdiff(p.mz, g)).ToArray
                    Dim Bpixels = pixels.Where(Function(p) mzdiff(p.mz, b)).ToArray

                    Call Invoke(Sub() params.SetIntensityMax(maxInto))
                    Call Invoke(Sub() rendering = createRenderTask(Rpixels, Gpixels, Bpixels, size))
                    Call Invoke(rendering)
                    Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
                End If

                Call progress.Invoke(Sub() progress.Close())
            End Sub).Start()

        Call progress.ShowDialog()
        Call PixelSelector1.ShowMessage($"Render in RGB Channel Composition Mode: {selectedMz.Select(Function(d) stdNum.Round(d, 4)).JoinBy(", ")}")
    End Sub

    Private Function createRenderTask(R As PixelData(), G As PixelData(), B As PixelData(), pixelSize$) As Action
        Dim blender As New RGBIonMSIBlender(R, G, B, pixelSize, params)

        Me.blender = blender
        Me.loadedPixels = R _
            .JoinIterates(G) _
            .JoinIterates(B) _
            .ToArray

        Return Sub()
                   Call MyApplication.RegisterPlot(
                       Sub(args)
                           Dim image As Image = blender.Rendering(args, PixelSelector1.CanvasSize)

                           PixelSelector1.SetMsImagingOutput(image, blender.dimensions, blender.dotSize, Nothing, Nothing, Nothing)
                           PixelSelector1.BackColor = params.background
                           PixelSelector1.SetColorMapVisible(visible:=params.showColorMap)
                       End Sub)
               End Sub
    End Function

    Friend Sub renderByMzList(mz As Double(), titleName As String)
        Dim selectedMz As New List(Of Double)
        Dim size As String = $"{params.scan_x},{params.scan_y}"
        Dim dotSize As New Size(3, 3)

        For i As Integer = 0 To mz.Length - 1
            selectedMz.Add(Val(CStr(mz(i))))
        Next

        If selectedMz.Count = 1 Then
            MyApplication.host.showStatusMessage($"Run MS-Image rendering for selected ion m/z {selectedMz(Scan0)}...")
        Else
            MyApplication.host.showStatusMessage($"Run MS-Image rendering for {selectedMz.Count} selected ions...")
        End If

        mzdiff = params.GetTolerance

        Call SetTitle(selectedMz, titleName)
        Call frmProgressSpinner.DoLoading(
            Sub()
                Dim pixels As PixelData() = MSIservice.LoadPixels(selectedMz, mzdiff)

                If pixels.IsNullOrEmpty Then
                    Call MyApplication.host.showStatusMessage("no pixel data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
                    Call Invoke(Sub()
                                    rendering = New Action(Sub()
                                                           End Sub)
                                End Sub)
                    Call Invoke(Sub()
                                    PixelSelector1.SetMsImagingOutput(New Bitmap(1, 1), New Size(params.scan_x, params.scan_y), dotSize, params.colors, {0, 1}, 1)
                                End Sub)
                Else
                    Dim maxInto As Double = Aggregate pm As PixelData
                                            In pixels
                                            Into Max(pm.intensity)

                    Call Invoke(Sub() params.SetIntensityMax(maxInto))
                    Call Invoke(Sub() rendering = createRenderTask(pixels, size))
                    Call Invoke(rendering)
                    Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
                End If
            End Sub)

        Call PixelSelector1.ShowMessage($"Render in Layer Pixels Composition Mode: {selectedMz.Select(Function(d) stdNum.Round(d, 4)).JoinBy(", ")}")
    End Sub

    Dim loadedPixels As PixelData()
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

    Private Sub SetTitle(mz As IEnumerable(Of Double), title As String)
        Me.title = title
        Me.targetMz = mz.ToArray
        Me.titles(targetMz(Scan0).ToString("F3")) = title
    End Sub

    Public Sub renderByPixelsData(pixels As PixelData(), MsiDim As Size)
        If params Is Nothing Then
            Me.params = New MsImageProperty
            Me.checks = WindowModules.msImageParameters.RenderingToolStripMenuItem
            Me.tweaks = WindowModules.msImageParameters.PropertyGrid1
            Me.FilePath = FilePath

            WindowModules.msImageParameters.PropertyGrid1.SelectedObject = params
            WindowModules.msImageParameters.Win7StyleTreeView1.Nodes.Clear()
        End If

        Call params.SetIntensityMax(Aggregate pm As PixelData In pixels Into Max(pm.intensity))
        Call params.Reset(MsiDim, "N/A", "N/A")
        Call sampleRegions.SetBounds(pixels.Select(Function(a) New Point(a.x, a.y)))

        rendering = createRenderTask(pixels, $"{params.scan_x},{params.scan_y}")
        rendering()

        Call MyApplication.host.showStatusMessage("Rendering Complete!", My.Resources.preferences_system_notifications)
    End Sub

    Public Sub BlendingHEMap(layer As HeatMap.PixelData(), dimensions As Size)
        If params Is Nothing Then
            params = MsImageProperty.Empty(dimensions)
            WindowModules.msImageParameters.PropertyGrid1.SelectedObject = params
        End If

        Dim blender As New HeatMapBlender(layer, dimensions, params)

        Me.blender = blender
        Me.rendering =
            Sub()
                Call MyApplication.RegisterPlot(
                    Sub(args)
                        Dim image As Image = blender.Rendering(args, PixelSelector1.CanvasSize)

                        PixelSelector1.SetMsImagingOutput(image, dimensions, blender.dotsize, params.colors, {0, 1}, params.mapLevels)
                        PixelSelector1.BackColor = params.background
                        PixelSelector1.SetColorMapVisible(visible:=params.showColorMap)
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
    Private Function createRenderTask(pixels As PixelData(), dimensions$) As Action
        Dim blender As New SingleIonMSIBlender(pixels, params)
        Dim range As DoubleRange = blender.range

        Me.loadedPixels = pixels
        Me.blender = blender
        Me.PixelSelector1.LoadSampleTags(pixels.Select(Function(i) i.sampleTag).Where(Function(str) Not str Is Nothing).Distinct)

        Return Sub()
                   Call MyApplication.RegisterPlot(
                       Sub(args)
                           Dim image As Image = blender.Rendering(args, PixelSelector1.CanvasSize)

                           PixelSelector1.SetMsImagingOutput(image, dimensions.SizeParser, blender.dotSize, params.colors, {range.Min, range.Max}, params.mapLevels)
                           PixelSelector1.BackColor = params.background
                           PixelSelector1.SetColorMapVisible(visible:=params.showColorMap)
                       End Sub)
               End Sub
    End Function

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
        If e.ChangedItem.Label.TextEquals("background") AndAlso (blender Is Nothing OrElse Not TypeOf blender Is RGBIonMSIBlender) Then
            PixelSelector1.BackColor = params.background
        ElseIf Not rendering Is Nothing Then
            Dim grid As PropertyGrid = DirectCast(s, PropertyGrid)
            Dim reason As String = MsImageProperty.Validation(grid.SelectedObject, e)

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
        End If

        If MessageBox.Show("Going to close current MS-imaging viewer?", FilePath.FileName, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
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
        If loadedPixels.IsNullOrEmpty Then
            Call MyApplication.host.showStatusMessage("No loaded pixels data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Dim dimension As New Size(params.scan_x, params.scan_y)

        Using file As New SaveFileDialog With {.Filter = "NetCDF(*.cdf)|*.cdf", .Title = "Save MS-Imaging Matrix"}
            If file.ShowDialog = DialogResult.OK Then
                Using filesave As FileStream = file.FileName.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                    Call loadedPixels.CreateCDF(filesave, dimension, params.GetTolerance)
                End Using
            End If
        End Using
    End Sub

    Dim pinedPixel As LibraryMatrix

    Private Sub PinToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PinToolStripMenuItem.Click
        Dim pos As Point = PixelSelector1.Pixel
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
                MyApplication.host.showStatusMessage("There is no MS data in current pixel?", My.Resources.StatusAnnotations_Warning_32xLG_color)
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
        If PixelSelector1.HasRegionSelection Then
            If Not DrawHeMapRegion Then
                Call sampleRegions.Add(PixelSelector1)
            Else
                Dim regions = PixelSelector1 _
                  .GetPolygons(popAll:=True) _
                  .ToArray

                Call HEMap.Add(regions)
            End If

            Call StartNewPolygon()
        Else
            Call MyApplication.host.showStatusMessage("No sample region was selected!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Private Sub exportMSISampleTable()
        If sampleRegions.IsNullOrEmpty Then
            Call MyApplication.host.showStatusMessage("No sample spot regions!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Using file As New SaveFileDialog With {.Filter = "Excel Table(*.csv)|*.csv"}
                If file.ShowDialog = DialogResult.OK Then
                    Call RscriptProgressTask.CreateMSIPeakTable(sampleRegions, mzpack:=FilePath, saveAs:=file.FileName)
                End If
            End Using
        End If
    End Sub

    Private Sub ExportPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportPlotToolStripMenuItem.Click
        If Not checkService() Then
            Return
        ElseIf targetMz.IsNullOrEmpty Then
            Call MyApplication.host.warning("No ion was selected to export MS-Imaging plot!")
            Return
        End If

        Using file As New SaveFileDialog With {.Filter = "Plot Image(*.png)|*.png"}
            If file.ShowDialog = DialogResult.OK Then
                If targetMz.Length > 1 Then
                    Call RscriptProgressTask.ExportRGBIonsPlot(targetMz, mzdiff.GetScript, saveAs:=file.FileName)
                Else
                    Call RscriptProgressTask.ExportSingleIonPlot(
                        mz:=targetMz(0),
                        tolerance:=mzdiff.GetScript,
                        saveAs:=file.FileName,
                        title:=title,
                        background:=params.background.ToHtmlColor,
                        colorSet:=params.colors.Description
                    )
                End If
            End If
        End Using
    End Sub

    Private Sub ImageProcessingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImageProcessingToolStripMenuItem.Click
        Dim getConfig As New InputImageProcessor
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

        If mask.ShowDialogForm(getConfig) = DialogResult.OK Then
            Dim levels As Integer = CInt(getConfig.TrackBar1.Value)
            Dim contract As Double = getConfig.TrackBar2.Value

            If levels > 0 OrElse contract <> 0.0 Then
                Dim progress As New frmTaskProgress

                ' just exit image progress
                progress.TaskCancel = Sub() PixelSelector1.cancelBlur = True
                progress.ShowProgressTitle("Image Processing", True)
                progress.ShowProgressDetails("Do gauss blur...", True)
                progress.SetProgressMode()

                Call New Thread(Sub()
                                    Call Thread.Sleep(1000)
                                    Call progress.SetProgress(0, "Do gauss blur...")
                                    Call Me.Invoke(Sub() PixelSelector1.doGauss(levels * 13, contract, Sub(p) progress.SetProgress(p, $"Do gauss blur... {p.ToString("F2")}%")))
                                    Call progress.Invoke(Sub() progress.Close())
                                End Sub).Start()

                Call progress.ShowDialog()
            End If
        End If
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
            blender.sample_tag = tag
            Call rendering()
        End If
    End Sub
End Class
