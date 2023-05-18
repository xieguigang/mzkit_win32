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

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot.Histogram
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.DataStorage.netCDF.DataVector
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap.hqx
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports mzblender
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports Mzkit_win32.MSImagingViewerV2
Imports RibbonLib.Interop
Imports Task
Imports TaskStream
Imports any = Microsoft.VisualBasic.Scripting
Imports DataFrame = Microsoft.VisualBasic.Data.csv.IO.DataFrame
Imports Xlsx = Microsoft.VisualBasic.MIME.Office.Excel.File

Public Class frmMsImagingTweaks

    Friend checkedMz As New List(Of TreeNode)
    Friend viewer As frmMsImagingViewer

    Public Iterator Function GetSelectedIons() As IEnumerable(Of Double)
        If Not Win7StyleTreeView1.SelectedNode Is Nothing Then
            If Not Win7StyleTreeView1.SelectedNode.Checked Then
                If Win7StyleTreeView1.SelectedNode.Tag Is Nothing Then
                    For Each node As TreeNode In Win7StyleTreeView1.SelectedNode.Nodes
                        Yield DirectCast(node.Tag, Double)
                    Next
                Else
                    Yield DirectCast(Win7StyleTreeView1.SelectedNode.Tag, Double)
                End If
            Else
                GoTo UseCheckedList
            End If
        Else
UseCheckedList:
            If checkedMz.Count > 0 Then
                For Each node In checkedMz
                    Yield DirectCast(node.Tag, Double)
                Next
            Else

            End If
        End If
    End Function

    Public Const Ion_Layers As String = "Ion Layers"
    Public Const Pinned_Pixels As String = "Pinned Pixels"

    Public ReadOnly Property Parameters As MsImageProperty
        Get
            Return PropertyGrid1.SelectedObject
        End Get
    End Property

    Private Sub frmMsImagingTweaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = "MsImage Parameters"

        Call ApplyVsTheme(ContextMenuStrip1, ToolStrip1)

        Win7StyleTreeView1.Nodes.Add(Ion_Layers)
        Win7StyleTreeView1.Nodes.Add(Pinned_Pixels)
        RibbonEvents.ribbonItems.TabGroupMSI.ContextAvailable = ContextAvailability.Active
    End Sub

    Public Sub ClearIons() Handles ClearSelectionToolStripMenuItem.Click, ToolStripButton3.Click
        checkedMz.Clear()
        Win7StyleTreeView1.Nodes.Item(0).Nodes.Clear()
        ' Win7StyleTreeView1.Nodes.Item(1).Nodes.Clear()
    End Sub

    Public Sub LoadPinnedIons(ions As IEnumerable(Of ms2))
        Win7StyleTreeView1.Nodes.Item(1).Nodes.Clear()

        For Each i As ms2 In ions _
            .ToArray _
            .Centroid(Tolerance.DeltaMass(0.0001), New RelativeIntensityCutoff(0.001)) _
            .OrderByDescending(Function(m) m.intensity)

            Call AddIonMzLayer(i.mz, index:=1)
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
    ''' load m/z layer
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
        ElseIf WindowModules.viewer.params.app = FileApplicationClass.STImaging Then
            ' is a gene id
            Call WindowModules.viewer.renderByName(ToolStripSpringTextBox1.Text, $"STImaging of '{ToolStripSpringTextBox1.Text}'")
        Else
            ' formula
            Dim formula As String = ToolStripSpringTextBox1.Text
            Dim exactMass As Double = Math.EvaluateFormula(formula)

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

    Private Sub AddIonMzLayer(mz As Double, Optional index As Integer = 0)
        Dim node As TreeNode = Win7StyleTreeView1.Nodes.Item(index).Nodes.Add(mz.ToString("F4"))
        node.Tag = mz
    End Sub

    ''' <summary>
    ''' 只渲染前三个选中的m/z
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RGBLayers(sender As Object, e As EventArgs) Handles RenderLayerCompositionModeToolStripMenuItem.Click
        Dim mz3 As Double() = GetSelectedIons.ToArray

        If mz3.Length = 0 Then
            Call MyApplication.host.showStatusMessage("no ions data...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        ElseIf Parameters Is Nothing Then
            Call MyApplication.host.warning("MS-imaging data is not loaded yet!")
            Return
        End If

        Dim getFormula As New InputIonRGB
        Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)

        For Each ion As Double In mz3
            Dim ionStr As String = ion.ToString("F4")

            Call getFormula.cR.Items.Add(ionStr)
            Call getFormula.cG.Items.Add(ionStr)
            Call getFormula.cB.Items.Add(ionStr)
        Next

        If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
            Dim r As Double = getFormula.R
            Dim g As Double = getFormula.G
            Dim b As Double = getFormula.B
            Dim viewer = WindowModules.viewer

            Call viewer.renderRGB(r, g, b)
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

    Public Sub loadRenderFromCDF(firstFile As String)
        Dim pixels As PixelData()
        Dim size As Size
        Dim tolerance As Tolerance
        Dim rgb As RGBConfigs = Nothing

        If viewer Is Nothing Then
            viewer = WindowModules.viewer
        End If

        Using cdf As New netCDFReader(firstFile)
            If Not {"mz", "intensity", "x", "y"}.All(AddressOf cdf.dataVariableExists) Then
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

                Return
            End If

            size = cdf.GetMsiDimension
            pixels = cdf.LoadPixelsData.ToArray
            tolerance = cdf.GetMzTolerance

            If cdf.dataVariableExists("rgb") Then
                ' load rgb configs
                rgb = RGBConfigs.ParseJSON(DirectCast(cdf.getDataVariable("rgb"), chars))
            End If
        End Using

        'Call ProgressSpinner.DoLoading(
        '    Sub()
        '        Call Me.Invoke(Sub()
        '                           viewer.LoadRender(firstFile, firstFile)
        '                           viewer.renderByPixelsData(pixels, size)
        '                       End Sub)
        '    End Sub)

        Call viewer.LoadRender(firstFile, firstFile)
        Call viewer.renderByPixelsData(pixels, size, rgb)

        For Each mz As Double In pixels _
            .GroupBy(Function(p) p.mz, tolerance) _
            .Select(Function(a)
                        Return Val(a.name)
                    End Function)

            Call AddIonMzLayer(mz)
        Next
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

    Private Function encodeJSON(data As Dictionary(Of String, NamedValue(Of Double()))) As String
        Dim json As New JsonObject
        Dim sample As JsonObject

        For Each groupId As String In data.Keys
            sample = New JsonObject
            sample.Add("color", New JsonValue(data(groupId).Name))
            sample.Add("data", New JsonArray(data(groupId).Value))
            json.Add(groupId, sample)
        Next

        Return json.BuildJsonString
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <returns>[region_label => [color => expression_vector]]</returns>
    Private Function getVector(ByRef mz As Double) As Dictionary(Of String, NamedValue(Of Double()))
        Dim errMsg As String = Nothing
        Dim layer As PixelData() = getLayer(mz, needsRegions:=True, msg:=errMsg)

        If layer Is Nothing Then
            Call MyApplication.host.warning($"No ion layer data for ${mz.ToString("F4")}: {errMsg}")
            Return Nothing
        End If

        Dim regions As TissueRegion() = viewer.sampleRegions _
            .GetRegions(viewer.PixelSelector1.MSICanvas.dimension_size) _
            .ToArray

        Return SampleData.ExtractSample(layer, regions, n:=32, coverage:=0.35)
    End Function

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
            msg = "No ions m/z is selected!"
            Return Nothing
        End If

        Dim mzdiff = Tolerance.DeltaMass(0.01)
        Dim layer As PixelData() = viewer.MSIservice.LoadPixels({mz}, mzdiff)

        If layer.IsNullOrEmpty Then
            msg = "ion layer is empty!"
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
    Private Sub showPlot(data As Dictionary(Of String, NamedValue(Of Double())), type As String, mz As Double)
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
                colorSet:=viewer.params.colors.Description
            )
        Else
            image = RscriptProgressTask.PlotStats(pack, type, title:=viewer.GetTitle(mz))
        End If

        If image Is Nothing Then
            MyApplication.host.showStatusMessage("Error while run ggplot...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            MyApplication.host.mzkitTool.ShowPlotImage(image, ImageLayout.Zoom)
            MyApplication.host.ShowMzkitToolkit()
        End If
    End Sub

    Private Sub IntensityHistogramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles IntensityHistogramToolStripMenuItem.Click
        Dim mz As Double
        Dim errMsg As String = Nothing
        Dim layer = getLayer(mz, needsRegions:=False, msg:=errMsg)

        If layer Is Nothing Then
            Call MyApplication.host.warning($"No ion layer data for ${mz.ToString("F4")}: {errMsg}")
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
            MyApplication.host.showStatusMessage("Error while run signal intensity histogram plot...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            MyApplication.host.mzkitTool.ShowPlotImage(image, ImageLayout.Zoom)
            MyApplication.host.ShowMzkitToolkit()
        End If
    End Sub

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
                    Dim table As DataFrame

                    If file.FileName.ExtensionSuffix("csv") Then
                        table = DataFrame.Load(file.FileName)
                    Else
                        table = DataFrame.CreateObject(Xlsx.Open(file.FileName).GetTable("Sheet1"))
                    End If

                    Dim mz As Double() = table(table.GetOrdinal("mz")).AsDouble
                    Dim name As String() = table(table.GetOrdinal("name")).ToArray

                    For i As Integer = 0 To mz.Length - 1
                        Dim label As String = $"{name(i)} [m/z {mz(i).ToString("F4")}]"
                        Dim node = folder.Nodes.Add(label)

                        node.Tag = mz(i)
                    Next

                    Call Workbench.StatusMessage($"Load {mz.Length} ions for run data visualization.")
                End If
            End If
        End Using
    End Sub

    Private Sub ExportEachSelectedLayersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportEachSelectedLayersToolStripMenuItem.Click
        Dim list = Win7StyleTreeView1.Nodes(0)

        Using folder As New FolderBrowserDialog With {.Description = "Select a folder to export images"}
            If folder.ShowDialog = DialogResult.OK Then
                Dim dir As String = folder.SelectedPath
                Dim params = WindowModules.viewer.params
                Dim size As String = $"{params.scan_x},{params.scan_y}"
                Dim args As New PlotProperty
                Dim canvas As New Size(params.scan_x * 3, params.scan_y * 3)

                params.Hqx = HqxScales.Hqx_4x

                Call TaskProgress.LoadData(
                    Function(echo As Action(Of String))
                        For i As Integer = 0 To list.Nodes.Count - 1
                            Dim n = list.Nodes(i)

                            If n.Checked Then
                                Dim val As String = any.ToString(If(n.Tag, CObj(n.Text)))
                                Dim path As String = $"{dir}/{val}.png"

                                Call echo($"processing '{val}'")

                                Dim pixels = WindowModules.viewer.MSIservice.LoadGeneLayer(val)

                                If pixels.IsNullOrEmpty Then

                                Else
                                    Dim maxInto = pixels.Select(Function(a) a.intensity).Max
                                    params.SetIntensityMax(maxInto, New Point())
                                    Dim blender As New SingleIonMSIBlender(pixels, Nothing, params)
                                    Dim range As DoubleRange = blender.range
                                    Dim image As Image = blender.Rendering(args, canvas)

                                    Call image.SaveAs(path)
                                End If
                            End If
                        Next

                        Return True
                    End Function,
                    title:="Plot each selected image layers...",
                    host:=WindowModules.viewer)
            End If
        End Using
    End Sub

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
                                    For Each name As String In nameList
                                        list.Nodes.Add(name).Tag = name
                                    Next
                                End Sub)
                End If
            End Sub)
        Call Workbench.SuccessMessage($"fetch {list.Nodes.Count} annotation layers!")
    End Sub
End Class
