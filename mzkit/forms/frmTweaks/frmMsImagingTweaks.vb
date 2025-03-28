#Region "Microsoft.VisualBasic::a3c3a8a51471fa789759ba9dcba6b66e, mzkit\src\mzkit\mzkit\forms\frmTweaks\frmMsImagingTweaks.vb"

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

'   Total Lines: 339
'    Code Lines: 258
' Comment Lines: 23
'   Blank Lines: 58
'     File Size: 12.19 KB


' Class frmMsImagingTweaks
' 
'     Properties: Parameters
' 
'     Function: GetSelectedIons
' 
'     Sub: AddIonMzLayer, checkNode, ClearIons, frmMsImagingTweaks_Load, LoadBasePeakIonsToolStripMenuItem_Click
'          loadBasePeakMz, LoadPinnedIons, loadRenderFromCDF, PropertyGrid1_DragDrop, PropertyGrid1_DragEnter
'          RGBLayers, ToolStripButton1_Click, ToolStripButton2_Click, ToolStripSpringTextBox1_Click, uncheckNode
'          Win7StyleTreeView1_AfterCheck
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot.Histogram
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap.hqx
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports mzblender
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports MZKitWin32.Blender.CommonLibs
Imports RibbonLib.Interop
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports TaskStream
Imports any = Microsoft.VisualBasic.Scripting
Imports Xlsx = Microsoft.VisualBasic.MIME.Office.Excel.XLSX.File

Public Class frmMsImagingTweaks

    Friend checkedMz As New List(Of TreeNode)
    Friend viewer As frmMsImagingViewer
    Friend renderList As New frmMSIHistoryList

    ''' <summary>
    ''' title of viewer current ion will be updates
    ''' </summary>
    ''' <returns></returns>
    Public Iterator Function GetSelectedIons() As IEnumerable(Of Double)
        Dim target = Win7StyleTreeView1.SelectedNode

        If Not target Is Nothing Then
            If target.Checked Then
                ' is root node
                If target.Tag Is Nothing Then
                    ' use all ions below this root node
                    For Each node As TreeNode In target.Nodes
                        Dim ion_mz As Double = Val(node.Tag)
                        Call viewer.SetTitle({ion_mz}, node.Text)
                        Yield ion_mz
                    Next
                Else
                    ' is a selected single ion
                    Dim ion_mz As Double = Val(target.Tag)
                    Call viewer.SetTitle({ion_mz}, target.Text)
                    Yield ion_mz
                End If
            ElseIf Not target.Tag Is Nothing Then
                ' is a selected single ion
                ' use current ion
                Dim ion_mz As Double = Val(target.Tag)
                Call viewer.SetTitle({ion_mz}, target.Text)
                Yield ion_mz
            Else
                GoTo UseCheckedList
            End If
        Else
UseCheckedList:
            For Each ion As Double In getCheckedIons().Select(Function(i) i.mz)
                Yield ion
            Next
        End If
    End Function

    Private Iterator Function getCheckedIons() As IEnumerable(Of (title As String, mz As Double))
        If checkedMz.Count > 0 Then
            For Each node In checkedMz
                Call viewer.SetTitle({node.Tag}, node.Text)
                Yield (node.Text, DirectCast(node.Tag, Double))
            Next
        Else

        End If
    End Function

    Public Const Ion_Layers As String = "Ion Layers"
    Public Const Pinned_Pixels As String = "Pinned Pixels"
    Public Const Current_Spot As String = "Current Spot"

    Public ReadOnly Property Parameters As MsImageProperty
        Get
            Return PropertyGrid1.SelectedObject
        End Get
    End Property

    Private Sub frmMsImagingTweaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = "MsImage Parameters"
        Me.renderList.Show()
        Me.renderList.Visible = False

        Call ApplyVsTheme(ContextMenuStrip1, ToolStrip1)
        Call ClearIons()

        RibbonEvents.ribbonItems.TabGroupMSI.ContextAvailable = ContextAvailability.Active
    End Sub

    Public Sub ClearIons() Handles ClearSelectionToolStripMenuItem.Click, ToolStripButton3.Click
        checkedMz.Clear()

        If Win7StyleTreeView1.Nodes.Count > 0 Then
            Win7StyleTreeView1.Nodes.Item(0).Nodes.Clear()
        Else
            Win7StyleTreeView1.Nodes.Add(Ion_Layers)
            Win7StyleTreeView1.Nodes.Add(Pinned_Pixels)
            Win7StyleTreeView1.Nodes.Add(Current_Spot)
        End If

        ' Win7StyleTreeView1.Nodes.Item(1).Nodes.Clear()
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="ions"></param>
    ''' <param name="offset">
    ''' 1 - pinned pixel
    ''' 2 - current spot
    ''' </param>
    Public Sub LoadSpotIons(ions As IEnumerable(Of ms2), Optional offset As Integer = 1)
        Dim tops = ions _
            .ToArray _
            .Centroid(Tolerance.DeltaMass(0.0001), New RelativeIntensityCutoff(0.001)) _
            .OrderByDescending(Function(m) m.intensity) _
            .ToArray
        Dim maxo As Double = If(tops.Length > 0, tops.Max(Function(a) a.intensity), 0)

        If Win7StyleTreeView1.Nodes.Count = 0 Then
            Win7StyleTreeView1.Nodes.Add(Ion_Layers)
            Win7StyleTreeView1.Nodes.Add(Pinned_Pixels)
            Win7StyleTreeView1.Nodes.Add(Current_Spot)
        End If

        Win7StyleTreeView1.Nodes.Item(offset).Nodes.Clear()

        For Each i As ms2 In tops
            If offset = 2 Then
                Call AddIonMzLayer(i.mz, index:=offset, label:=$"{i.mz.ToString("F3")} ({(100 * i.intensity / maxo).ToString("F2")}%)")
            Else
                Call AddIonMzLayer(i.mz, index:=offset)
            End If
        Next
    End Sub

    Private Sub checkNode(node As TreeNode)
        Static checked As TreeNode = Nothing

        If checked Is Nothing OrElse checked IsNot node Then
            checked = node
            node.Checked = True
        Else
            Return
        End If

        Call checkedMz.Add(node)
    End Sub

    Private Sub uncheckNode(node As TreeNode)
        Static unchecked As TreeNode = Nothing

        If unchecked Is Nothing OrElse unchecked IsNot node Then
            unchecked = node
            node.Checked = False
        Else
            Return
        End If

        checkedMz.Remove(node)
    End Sub

    Private Sub Win7StyleTreeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterCheck
        If e.Node.Checked Then
            If e.Node.Tag Is Nothing Then
                For Each mz As TreeNode In e.Node.Nodes
                    Call checkNode(mz)
                Next
            Else
                Call checkNode(e.Node)
            End If
        Else
            If e.Node.Tag Is Nothing Then
                For Each mz As TreeNode In e.Node.Nodes
                    Call uncheckNode(mz)
                Next
            Else
                Call uncheckNode(e.Node)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Do ion Rendering
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ViewLayerButton.Click
        If ToolStripSpringTextBox1.Text.StringEmpty Then
            Call MyApplication.host.showStatusMessage("no ions data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        ElseIf ToolStripSpringTextBox1.Text.IsNumeric(True) Then
            ' is mz value
            Dim mz As Double = Val(ToolStripSpringTextBox1.Text)
            Dim viewer = WindowModules.viewer

            If TypeOf viewer Is frmMsImagingViewer Then
                Call DirectCast(viewer, frmMsImagingViewer).renderByMzList({mz}, Nothing)
            End If
        ElseIf WindowModules.viewer.params.app = MZKitWin32.Blender.CommonLibs.FileApplicationClass.STImaging Then
            ' is a gene id
            Call WindowModules.viewer.renderByName(ToolStripSpringTextBox1.Text, $"STImaging of '{ToolStripSpringTextBox1.Text}'")
        Else
            ' formula
            Dim formula As String = ToolStripSpringTextBox1.Text
            Dim exactMass As Double = FormulaScanner.EvaluateExactMass(formula)

            Call Win7StyleTreeView1.Nodes.Item(0).Nodes.Clear()

            For Each type As MzCalculator In Provider.Positives
                Dim mz As Double = type.CalcMZ(exactMass)

                If mz <= 0 Then
                    Continue For
                End If

                Dim node As TreeNode = Win7StyleTreeView1.Nodes.Item(0).Nodes.Add($"{mz.ToString("F4")} {type.ToString}")

                node.Tag = mz
            Next
        End If
    End Sub

    ''' <summary>
    ''' add ion layer by m/z
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        If ToolStripSpringTextBox1.Text.StringEmpty Then
            Call MyApplication.host.showStatusMessage("no ions data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call AddIonMzLayer(mz:=Val(ToolStripSpringTextBox1.Text))
        End If

        ToolStripSpringTextBox1.Text = ""
    End Sub

    Private Sub AddIonMzLayer(mz As Double, Optional index As Integer = 0, Optional label As String = Nothing)
        Dim node As TreeNode = Win7StyleTreeView1.Nodes.Item(index).Nodes.Add(If(label, mz.ToString("F4")))
        node.Tag = mz
    End Sub

    ''' <summary>
    ''' 只渲染前三个选中的m/z
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RGBLayers(sender As Object, e As EventArgs) Handles RenderLayerCompositionModeToolStripMenuItem.Click
        Dim mz3() = getCheckedIons.ToArray

        If mz3.Length = 0 Then
            Call MyApplication.host.showStatusMessage("no ions data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        ElseIf Parameters Is Nothing Then
            Call MyApplication.host.warning("MS-imaging data is not loaded yet!")
            Return
        End If

        Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)
        Dim ionMaps = mz3.GroupBy(Function(i) i.title).ToDictionary(Function(i) i.Key, Function(i) i.First.mz)
        Dim getFormula As New InputIonRGB

        For Each ionStr As String In ionMaps.Keys
            Call getFormula.cR.Items.Add(ionStr)
            Call getFormula.cG.Items.Add(ionStr)
            Call getFormula.cB.Items.Add(ionStr)
        Next

        If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
            Dim r As String = getFormula.R
            Dim g As String = getFormula.G
            Dim b As String = getFormula.B
            Dim viewer = WindowModules.viewer
            Dim getMz = Function(key As String) (key, ionMaps.TryGetValue(key))

            Call viewer.renderRGB(getMz(r), getMz(g), getMz(b))
            Call viewer.Show(MyApplication.host.m_dockPanel)
        End If
    End Sub

    Private Sub PropertyGrid1_DragDrop(sender As Object, e As DragEventArgs) Handles PropertyGrid1.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        Dim firstFile As String = files.ElementAtOrDefault(Scan0)

        If Not firstFile Is Nothing Then
            If firstFile.ExtensionSuffix("imzML") Then
                Call MyApplication.host.OpenFile(firstFile, showDocument:=True)
            ElseIf firstFile.ExtensionSuffix("CDF") Then
                Call loadRenderFromCDF(firstFile)
            Else
                Call MyApplication.host.showStatusMessage("invalid file type!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            End If
        End If
    End Sub

    Private Sub TryRenderTissueMapPlot(cdf As netCDFReader)
        If cdf.IsTissueMorphologyCDF Then
            Dim regions = cdf.ReadTissueMorphology.ToArray

            If regions.IsNullOrEmpty Then
                MessageBox.Show("No content data!", "Tissue map viewer", buttons:=MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim dims As Size = cdf.GetDimension
            ' open tool and then save to sample regions?
            Dim getFormula As New SampleRegionMergeTool
            Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)

            Call getFormula.LoadRegions(regions, dims)

            If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
                ' update to new regions
                regions = getFormula.GetMergedRegions
            End If

            Dim Rplot As Image = LayerRender.Draw(regions, dims, alphaLevel:=1, dotSize:=3)

            Call MyApplication.host.mzkitTool.ShowPlotImage(Rplot, ImageLayout.Zoom)
            Call MyApplication.host.ShowMzkitToolkit()
        Else
            ' invalid format
            Call Workbench.Warning("Invalid cdf file format! [mz, intensity, x, y] data vector should exists inside this cdf file!")
        End If
    End Sub

    Public Sub loadRenderFromCDF(firstFile As String)
        Using cdf As New netCDFReader(firstFile)
            If Not {"mz", "intensity", "x", "y"}.All(AddressOf cdf.dataVariableExists) Then
                TryRenderTissueMapPlot(cdf)
            Else
                TryRenderMSI(cdf, firstFile)
            End If
        End Using
    End Sub

    Private Sub TryRenderMSI(cdf As netCDFReader, firstFile As String)
        Dim pixels As PixelData()
        Dim size As Size
        Dim tolerance As Tolerance
        Dim rgb As RGBConfigs = Nothing

        If viewer Is Nothing Then
            viewer = WindowModules.viewer
        End If

        ' viewer.StartMSIService()

        size = cdf.GetMsiDimension
        pixels = cdf.LoadPixelsData.ToArray
        tolerance = cdf.GetMzTolerance

        If cdf.dataVariableExists("rgb") Then
            ' load rgb configs
            rgb = RGBConfigs.ParseJSON(DirectCast(cdf.getDataVariable("rgb"), chars))
            ' viewer.OpenSession(GetType(RGBIonMSIBlender), rgb.GetJSON)
        End If

        Call viewer.LoadRender(firstFile, firstFile)
        Call viewer.MSIservice.blender.channel.WriteBuffer(pixels)
        Call viewer.renderByPixelsData(pixels, size, rgb)

        For Each mz As Double In pixels _
            .GroupBy(Function(p) p.mz, tolerance) _
            .Select(Function(a)
                        Return Val(a.name)
                    End Function)

            Call AddIonMzLayer(mz)
        Next

        viewer.TIC = pixels _
            .GroupBy(Function(p) $"{p.x},{p.y}") _
            .Select(Function(p)
                        Return New PixelScanIntensity(p.First.x, p.First.y, Aggregate pi In p Into Sum(pi.intensity))
                    End Function) _
            .ToArray
    End Sub

    Private Sub PropertyGrid1_DragEnter(sender As Object, e As DragEventArgs) Handles PropertyGrid1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub

    Private Sub LoadBasePeakIonsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadBasePeakIonsToolStripMenuItem.Click
        Call ClearIons()

        If WindowModules.viewer.checkService Then
            ProgressSpinner.DoLoading(
                Sub()
                    Call Me.Invoke(Sub() Call loadBasePeakMz())
                End Sub)
        Else
            Call MyApplication.host.showStatusMessage("No MSI raw data file was loaded!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Private Sub loadBasePeakMz()
        Dim layers As TreeNode = Win7StyleTreeView1.Nodes.Item(0)
        Dim data As Double() = WindowModules.viewer.MSIservice.LoadBasePeakMzList

        For Each p As Double In data
            layers.Nodes.Add(p.ToString("F4")).Tag = p
            Application.DoEvents()
        Next
    End Sub

    Private Sub ToolStripSpringTextBox1_Click(sender As Object, e As EventArgs) Handles ToolStripSpringTextBox1.Click
        If ToolStripSpringTextBox1.Text.IsNumeric(True) Then
            ViewLayerButton.Text = "View MS-Imaging Layer"
        Else
            ViewLayerButton.Text = "List formula ions at here"
        End If
    End Sub

    Private Sub BoxPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BoxPlotToolStripMenuItem.Click
        Dim mz As Double
        Dim data = getVector(mz)

        If data Is Nothing Then
            Return
        Else
            Call showPlot(data, "box", mz)
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <returns>[region_label => [color => expression_vector]]</returns>
    Private Function getVector(ByRef mz As Double) As TissueRegion()
        Dim regions As TissueRegion() = Nothing
        Dim mzi As Double
        Dim pars = Globals.MSIBootstrapping
        Dim nsamples As Integer = pars.nsamples
        Dim cov As Double = pars.coverage

        Call ProgressSpinner.DoLoading(
            Sub()
                Dim errMsg As String = Nothing
                Dim layer As PixelData() = getLayer(mzi, needsRegions:=True, msg:=errMsg)

                If layer Is Nothing Then
                    Call Workbench.Warning($"No ion layer data for ${mzi.ToString("F4")}: {errMsg}")
                    Return
                End If

                regions = viewer.sampleRegions _
                    .GetRegions(viewer.PixelSelector1.MSICanvas.dimension_size) _
                    .ToArray
                Dim data = SampleData.ExtractSample(layer, regions, n:=nsamples, coverage:=cov)

                For Each r As TissueRegion In regions
                    r.tags = data(r.label).AsCharacter(format:="G8").ToArray

                    ' 20250227 the label of the region should not be integer value
                    ' or the json decode of the region label key will be
                    ' treated as the integer index
                    If r.label.IsPattern("\d+") Then
                        r.label = "region_" & r.label
                    End If
                Next
            End Sub, host:=Me)

        mz = mzi

        Return regions
    End Function

    ''' <summary>
    ''' get ion layer data from the ms-imaging data back-end
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="needsRegions"></param>
    ''' <param name="msg$"></param>
    ''' <returns></returns>
    Private Function getLayer(ByRef mz As Double, needsRegions As Boolean, ByRef msg$) As PixelData()
        If viewer Is Nothing OrElse (needsRegions AndAlso viewer.sampleRegions.IsNullOrEmpty) Then
            msg = "No tissue region data was provided!"
            Return Nothing
        ElseIf Not viewer.checkService Then
            msg = "You must start the MS-imaging data services backend at first!"
            Return Nothing
        End If

        mz = GetSelectedIons().FirstOrDefault

        If mz <= 0 Then
            msg = "No ions m/z Is selected!"
            Return Nothing
        End If

        Dim mzdiff = Tolerance.DeltaMass(0.01)
        Dim layer As PixelData() = viewer.MSIservice.LoadPixels({mz}, mzdiff)

        If layer.IsNullOrEmpty Then
            msg = "ion layer Is empty!"
            Return Nothing
        Else
            Return layer
        End If
    End Function

    Private Sub BarPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BarPlotToolStripMenuItem.Click
        Dim mz As Double
        Dim data = getVector(mz)

        If data Is Nothing Then
            Return
        Else
            Call showPlot(data, "bar", mz)
        End If
    End Sub

    Private Sub ViolinPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViolinPlotToolStripMenuItem.Click
        Dim mz As Double
        Dim data = getVector(mz)

        If data Is Nothing Then
            Return
        Else
            Call showPlot(data, "violin", mz)
        End If
    End Sub

    ''' <summary>
    ''' Run plot in Rscript host for export image plot
    ''' </summary>
    ''' <param name="data">[region_label => [color => expression_vector]]</param>
    ''' <param name="type">bar/box/violin</param>
    ''' <param name="mz"></param>
    Private Sub showPlot(data As TissueRegion(), type As String, mz As Double)
        Dim pack As String = encodeJSON(data)
        Dim image As Image
        Dim mzdiff = viewer.params.GetTolerance

        If AppendMSImagingToolStripMenuItem.Checked Then
            image = RscriptProgressTask.PlotSingleMSIStats(
                data:=pack,
                type:=type,
                title:=viewer.GetTitle(mz),
                mz:=mz,
                tolerance:=mzdiff.GetScript,
                background:=viewer.params.background.ToHtmlColor,
                colorSet:=viewer.params.colors.Description,
                show_tissue:=ribbonItems.CheckShowMapLayer.BooleanValue
            )
        Else
            image = RscriptProgressTask.PlotStats(pack, type, title:=viewer.GetTitle(mz))
        End If

        If image Is Nothing Then
            MyApplication.host.showStatusMessage("Error while run ggplot...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Dim expr = data.ToDictionary(Function(t) t.label,
                                         Function(t)
                                             Return t.tags _
                                                .Select(Function(si) Val(si)) _
                                                .ToArray
                                         End Function)
            Dim pars = Globals.MSIBootstrapping
            Dim nsamples As Integer = pars.nsamples

            MyApplication.host.mzkitTool.ShowPlotImage(image, ImageLayout.Zoom)
            MyApplication.host.mzkitTool.ShowExpressionMatrix(expr, nsamples, "spatial expression of m/z " & mz.ToString("F4"))
            MyApplication.host.ShowMzkitToolkit()
        End If
    End Sub

    Private Sub IntensityHistogramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IntensityHistogramToolStripMenuItem.Click
        Dim mz As Double
        Dim errMsg As String = Nothing
        Dim layer = getLayer(mz, needsRegions:=False, msg:=errMsg)

        If layer Is Nothing Then
            Call Workbench.Warning($"No ion layer data for ${mz.ToString("F4")}: {errMsg}")
            Return
        End If

        Dim allIntensity As Double() = layer.Select(Function(i) i.intensity).ToArray
        Dim range As New DoubleRange(allIntensity)

        If range.Length = 0 Then
            Return
        End If

        Dim image As Image = allIntensity.HistogramPlot(
            [step]:=range.Length / 50,
            color:="skyblue",
            padding:="padding: 100px 200px 300px 200px",
            yLabel:="Number Of Pixels",
            xLabel:="Intensity",
            xTickFormat:="G2",
            xlabelRotate:=45
        ).AsGDIImage

        If image Is Nothing Then
            Workbench.Warning("Error while run signal intensity histogram plot...")
        Else
            MyApplication.host.mzkitTool.ShowPlotImage(image, ImageLayout.Zoom)
            MyApplication.host.ShowMzkitToolkit()
        End If
    End Sub

    ''' <summary>
    ''' imports excel table
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton1_Click_1(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Using file As New OpenFileDialog With {.Filter = "Excel Table(*.xlsx;*.csv)|*.xlsx;*.csv|Name Targets(*.txt)|*.txt"}
            If file.ShowDialog = DialogResult.OK Then
                Dim folder = Win7StyleTreeView1.Nodes(0)

                Call folder.Nodes.Clear()

                If file.FileName.ExtensionSuffix("txt") Then
                    Dim names As String() = file.FileName.ReadAllLines

                    For Each Name As String In names
                        folder.Nodes.Add(Name).Tag = Name
                    Next

                    Call Workbench.SuccessMessage($"Load {names.Length} layers for run data visualization.")
                Else
                    Dim table As DataFrameResolver

                    If file.FileName.ExtensionSuffix("csv") Then
                        table = DataFrameResolver.Load(file.FileName)
                    Else
                        table = DataFrameResolver.CreateObject(Xlsx.Open(file.FileName).GetTable(0))
                    End If

                    Dim mz As Double() = table(table.GetOrdinal("mz")).AsDouble
                    Dim name As String() = table(table.GetOrdinal("name")).ToArray

                    Call ImportsIons(name, mz)
                End If
            End If
        End Using
    End Sub

    Public Sub ImportsIons(labels As String(), mz As Double())
        Dim folder = Win7StyleTreeView1.Nodes(0)

        Call folder.Nodes.Clear()

        For i As Integer = 0 To mz.Length - 1
            If mz(i) <= 0.0 Then
                Continue For
            End If

            Dim label As String = $"{labels(i)} [m/z {mz(i).ToString("F4")}]"
            Dim node = folder.Nodes.Add(label)

            node.Tag = mz(i)
        Next

        Call Workbench.StatusMessage($"Load {mz.Length} ions for run data visualization.")
    End Sub

    Private Sub ExportEachSelectedLayersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportEachSelectedLayersToolStripMenuItem.Click
        Dim list = Win7StyleTreeView1.Nodes(0)
        Dim folder As New SetMSIPlotParameters With {.SetDir = True}

        Call folder _
            .SetDimensionSize(viewer.params.GetMSIDimension) _
            .SetFolder(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)) _
            .SetIntensityRange(viewer.PixelSelector1.CustomIntensityRange) _
            .SetUnifyPadding(10)

        Call InputDialog.Input(
            setConfig:=Sub(config)
                           Call MakeExport(config, list)
                       End Sub,
            config:=folder)
    End Sub

    Private Sub MakeExport(folder As SetMSIPlotParameters, list As TreeNode)
        Dim params = WindowModules.viewer.params
        Dim size As String = $"{params.scan_x},{params.scan_y}"
        Dim MSIservice = WindowModules.viewer.MSIservice
        Dim TIC As Image = Nothing

        If params.showTotalIonOverlap Then
            TIC = TaskProgress.LoadData(
                streamLoad:=Function(echo As Action(Of String))
                                If params.showTotalIonOverlap Then
                                    Dim layer = MSIservice.LoadSummaryLayer(IntensitySummary.Total, False)
                                    Dim render As Image = SummaryMSIBlender.Rendering(layer, New Size(params.scan_x, params.scan_y), "lightgray", 255, "transparent")

                                    Return render
                                Else
                                    Return Nothing
                                End If
                            End Function,
                title:="Load TIC layer",
                info:="Fetch layer pixels data from data serivces backend!"
            )
        End If

        Call TaskProgress.LoadData(
            streamLoad:=Function(proc As ITaskProgress)
                            Return ExportLayers(proc, list, TIC, folder)
                        End Function,
            title:="Plot selected image layers...",
            info:="Export image rendering...",
            host:=WindowModules.viewer)
    End Sub

    Private Function ExportLayers(proc As ITaskProgress, list As TreeNode, TIC As Image, config As SetMSIPlotParameters) As Boolean
        Dim params = WindowModules.viewer.params
        Dim mzdiff = params.GetTolerance
        Dim echo = proc.Echo
        Dim MSIservice = WindowModules.viewer.MSIservice
        Dim canvas As Size = config.GetPlotSize
        Dim dir As String = config.SelectedPath
        Dim padding = PaddingLayout.EvaluateFromCSS(New CSSEnvirnment(canvas), config.GetPlotPadding)
        Dim args As New PlotProperty With {
            .padding_bottom = padding.Bottom,
            .padding_left = padding.Left,
            .padding_right = padding.Right,
            .padding_top = padding.Top
        }

        Call proc.SetProgressMode()
        Call proc.SetProgress(0)

        For i As Integer = 0 To list.Nodes.Count - 1
            Dim n = list.Nodes(i)
            Dim val As String = any.ToString(If(n.Tag, CObj(n.Text)))
            Dim tick As String = $"processing '{n.Text}' ({val})"

            If Not n.Checked Then
                Continue For
            Else
                Call proc.SetProgress(CInt(i / list.Nodes.Count * 100), tick)
            End If

            Dim fileName As String = n.Text.NormalizePathString(False, ".")
            Dim path As String = $"{dir}/{If(fileName.Length > 128, fileName.Substring(0, 127) & "...", fileName)}.png"
            Dim pixels As PixelData()

            If val.IsSimpleNumber Then
                pixels = MSIservice.LoadPixels(New Double() {Conversion.Val(val)}, mzdiff)
            Else
                pixels = MSIservice.LoadGeneLayer(val)
            End If

            If pixels.IsNullOrEmpty Then
                Call Workbench.Warning($"No pixels data for {n.Text}...")
            Else
                Dim maxInto = pixels.Select(Function(a) a.intensity).Max
                params.SetIntensityMax(maxInto, New Point())
                Dim blender As New SingleIonMSIBlender(pixels, frmMsImagingViewer.loadFilters, params, TIC)
                blender.SetClampRange(config.IntensityRange)
                Dim image As Image = blender.Rendering(args, canvas)

                Call image.SaveAs(path)
                Call Workbench.SuccessMessage($"Imaging render for {n.Text} success and save at location: {path}!")
            End If
        Next

        Return True
    End Function

    Private Sub SelectAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllToolStripMenuItem.Click
        Dim list = Win7StyleTreeView1.Nodes(0)

        For i As Integer = 0 To list.Nodes.Count - 1
            Dim n = list.Nodes(i)
            n.Checked = True
        Next
    End Sub

    Private Sub LoadAllAnnotationLayersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadAllAnnotationLayersToolStripMenuItem.Click
        Dim list = Win7StyleTreeView1.Nodes(0)

        Call ProgressSpinner.DoLoading(
            Sub()
                Dim nameList = viewer.MSIservice.getAllLayerNames

                Call Invoke(Sub() list.Nodes.Clear())

                If Not nameList.IsNullOrEmpty Then
                    Call Invoke(Sub()
                                    For Each name As String In nameList.Keys
                                        list.Nodes.Add(name).Tag = nameList(name)
                                    Next
                                End Sub)
                End If
            End Sub)
        Call Workbench.SuccessMessage($"fetch {list.Nodes.Count} annotation layers!")
    End Sub

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        Dim ions = getCheckedIons.ToArray
        Dim regions As TissueRegion() = viewer.sampleRegions _
            .GetRegions(viewer.PixelSelector1.MSICanvas.dimension_size) _
            .ToArray

        If regions.IsNullOrEmpty Then
            Call Workbench.Warning("no tissue regions was found! Add some interested regions on your sample at first!")
            Return
        ElseIf ions.IsNullOrEmpty Then
            Call Workbench.Warning("no feature ions was selected, add some interested ions at first!")
            Return
        Else
            Using folder As New FolderBrowserDialog
                If folder.ShowDialog = DialogResult.OK Then
                    Call createPeaktable(regions, ions, workdir:=folder.SelectedPath)
                End If
            End Using
        End If
    End Sub

    ''' <summary>
    ''' Create sample peaktable and then open in analysis workbench page
    ''' </summary>
    ''' <param name="regions"></param>
    ''' <param name="ions"></param>
    ''' <param name="workdir"></param>
    Private Sub createPeaktable(regions As TissueRegion(), ions As (title As String, mz As Double)(), workdir As String)
        Dim pars = Globals.MSIBootstrapping
        Dim nsamples As Integer = pars.nsamples
        Dim cov As Double = pars.coverage
        Dim sampleinfo As SampleInfo() = regions _
            .Select(Iterator Function(t, batch) As IEnumerable(Of SampleInfo)
                        Dim color_str As String = t.color.ToHtmlColor
                        Dim region_label As String = t.label

                        If region_label.IsPattern("\d+") Then
                            region_label = $"region_{region_label}"
                        End If

                        For i As Integer = 1 To nsamples
                            Yield New SampleInfo With {
                                .ID = $"{region_label}.{i}",
                                .color = color_str,
                                .batch = batch + 1,
                                .injectionOrder = i,
                                .sample_info = region_label,
                                .sample_name = .ID,
                                .shape = "circle"
                            }
                        Next
                    End Function) _
            .IteratesALL _
            .ToArray
        Dim peaks As PeakSet
        Dim task As New peakTask With {
            .ions = ions.ToArray,
            .cov = cov,
            .host = Me,
            .nsamples = nsamples,
            .regions = regions.ToArray
        }

        Call TaskProgress.RunAction(AddressOf task.getPeaks, host:=Me)
        Call sampleinfo.SaveTo($"{workdir}/sampleinfo.csv")

        peaks = New PeakSet With {.peaks = task.peaks.uniqueNames.ToArray}

        Using f As Stream = $"{workdir}/peakset.xcms".Open(FileMode.OpenOrCreate, doClear:=True)
            Call SaveXcms.DumpSample(peaks, f)
            Call f.Flush()
        End Using

        Using f As Stream = $"{workdir}/mat.dat".Open(FileMode.OpenOrCreate, doClear:=True)
            Call frmMetabonomicsAnalysis.CastMatrix(peaks, sampleinfo).Save(f)
        End Using

        Call Workbench.LogText($"set workspace for metabonomics workbench: {workdir}")

        Dim page = VisualStudio.ShowDocument(Of frmMetabonomicsAnalysis)()
        page.workdir = workdir
        page.LoadWorkspace(workdir)
    End Sub

    Private Class peakTask

        Public ions As (title As String, mz As Double)()
        Public host As frmMsImagingTweaks
        Public nsamples As Integer
        Public cov As Double
        Public regions As TissueRegion()
        Public peaks As New List(Of xcms2)
        Public mzdiff As Double = 0.01

        Private Function CreateIonSet() As Dictionary(Of String, Double)
            Dim list As New List(Of NamedValue(Of Double))

            For Each ion As (title As String, mz As Double) In ions
                Dim id As String = ion.title

                If id.IsNumeric Then
                    id = $"MSI{ion.mz.ToString("F4")}"
                End If

                Call list.Add(New NamedValue(Of Double)(id, ion.mz))
            Next

            Dim uniqueIds = list.Select(Function(i) i.Name).UniqueNames
            Dim ionSet As New Dictionary(Of String, Double)

            For i As Integer = 0 To uniqueIds.Length - 1
                Call ionSet.Add(uniqueIds(i), list(i).Value)
            Next

            Return ionSet
        End Function

        Private Async Function BootstrapExpression(region As TissueRegion,
                                                   dims As Size,
                                                   target As Dictionary(Of String, Double)) As Task(Of Dictionary(Of String, Double()))

            Dim poly = region.GetPolygons.ToArray

            Return Await System.Threading.Tasks.Task.Run(
                Function()
                    Return host.viewer.MSIservice.SpatialBootstrapping(poly, dims, target, mzdiff, nsamples, cov)
                End Function)
        End Function

        Private Async Function BootstrapPeaks(p As ITaskProgress, target As Dictionary(Of String, Double)) As Task(Of Dictionary(Of String, Dictionary(Of String, Double())))
            Dim errMsg As String = Nothing
            Dim expr As New Dictionary(Of String, Dictionary(Of String, Double()))
            Dim dims As Size = host.viewer.params.GetMSIDimension

            For Each region As TissueRegion In regions
                Dim regionId As String = region.label

                If regionId.IsPattern("\d+") Then
                    regionId = $"region_{regionId}"
                End If

                Call p.SetInfo($"processing tissue region: {regionId}({region.color.ToHtmlColor})")

                Dim expression = Await BootstrapExpression(region, dims, target)

                If expression Is Nothing Then
                    Call Workbench.Warning($"No ion layer expression data for ${regionId}, this tissue region feature will be omit: {errMsg}")
                    Continue For
                End If

                Call expr.Add(regionId, expression)
            Next

            Return expr
        End Function

        Public Sub getPeaks(p As ITaskProgress)
            Dim async = getPeaksAsync(p)

            For i As Integer = 0 To Integer.MaxValue - 1
                If async.IsCompleted Then
                    Exit For
                End If

                Call Thread.Sleep(100)
                Call Application.DoEvents()
            Next
        End Sub

        Public Async Function getPeaksAsync(p As ITaskProgress) As Threading.Tasks.Task(Of Boolean)
            Dim target As Dictionary(Of String, Double) = CreateIonSet()
            Dim expr As Dictionary(Of String, Dictionary(Of String, Double())) = Await BootstrapPeaks(p, target)
            Dim proc As Integer = 0

            Call p.SetInfo($"Build expression peaktable for all selected feature ions!")

            For Each ion As KeyValuePair(Of String, Double) In target
                Dim peak As New xcms2(ion.Value, 0) With {
                    .ID = ion.Key,
                    .Properties = New Dictionary(Of String, Double)
                }

                For Each regionId As String In expr.Keys
                    Dim ions = expr(regionId)(ion.Key)

                    For i As Integer = 0 To ions.Length - 1
                        Call peak.Add($"{regionId}.{i + 1}", ions(i))
                    Next
                Next

                proc += 1

                Call peaks.Add(peak)

                If proc Mod 7 = 0 Then
                    Call p.SetInfo($"Build expression peaktable for selected feature ion: {ion.Key} ({ion.Value})")
                End If
            Next

            Return True
        End Function
    End Class
End Class
