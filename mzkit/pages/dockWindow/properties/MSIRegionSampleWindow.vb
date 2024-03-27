Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports Mzkit_win32.MSImagingViewerV2
Imports ServiceHub
Imports STRaid

Public Class MSIRegionSampleWindow

    Public ReadOnly Property IsNullOrEmpty As Boolean
        Get
            Return FlowLayoutPanel1.Controls.Count = 0
        End Get
    End Property

    Friend dimension As Size
    Friend canvas As PixelSelector

    ''' <summary>
    ''' the sample region pixels
    ''' </summary>
    Friend sample_bounds As Point()
    Friend importsFile As String
    Friend viewer As frmMsImagingViewer

    ''' <summary>
    ''' Clear all regions data
    ''' </summary>
    Friend Sub Clear()
        ' the tissue region data is store in this winform control
        FlowLayoutPanel1.Controls.Clear()
    End Sub

    Friend Overloads Sub SetBounds(pixels As IEnumerable(Of Point))
        sample_bounds = pixels _
            .GroupBy(Function(a) $"{a.X},{a.Y}") _
            .Select(Function(a) a.First) _
            .ToArray
    End Sub

    Public Sub TurnUpsideDown(selector As PixelSelector)
        dimension = selector.dimension_size
        canvas = selector

        For Each card As RegionSampleCard In FlowLayoutPanel1.Controls
            If Not card.tissue Is Nothing Then
                card.tissue.points = card.tissue.points _
                    .Select(Function(p) New Point(p.X, dimension.Height - p.Y)) _
                    .ToArray
            Else
                For Each region As Polygon2D In card.regions
                    region.ypoints = region.ypoints _
                        .Select(Function(y) dimension.Height - y) _
                        .ToArray
                    region.calculateBounds(region.xpoints, region.ypoints, region.length)
                Next
            End If
        Next

        Call updateLayerRendering()
    End Sub

    Public Overloads Sub LoadTissueMaps(tissues As TissueRegion(), canvas As PixelSelector, Optional append As Boolean = False)
        If canvas Is Nothing Then
            canvas = WindowModules.viewer.PixelSelector1.MSICanvas
        End If

        Me.canvas = canvas
        Me.dimension = canvas.dimension_size
        Me.importsFile = $"Load {tissues.Length} tissue region maps!"

        If Not append Then
            Call Clear()
        End If

        For Each region As TissueRegion In tissues
            Dim card As New RegionSampleCard

            Call card.SetPolygons(region, callback:=AddressOf updateLayerRendering)
            Call FlowLayoutPanel1.Controls.Add(card)

            Call ApplyVsTheme(card.ContextMenuStrip1)

            ' card.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            card.SampleColor = region.color
            card.SampleInfo = region.label

            AddHandler card.RemoveSampleGroup, AddressOf removeSampleGroup
            AddHandler card.ViewRegionMs1Spectrum, AddressOf ViewMs1Spectrum
            AddHandler card.SetHtmlColorCode, AddressOf SetHtmlColorCode
            AddHandler card.StartMoveRegion,
                Sub(ctl)
                    Dim region_tile = canvas.AddSpatialRegion(ctl.ExportTissueRegion(dimension))

                    AddHandler region_tile.ApplySave,
                        Sub(tile)
                            Call saveManualMoveRegionLocation(tile, region, ctl)
                            Call canvas.Controls.Remove(tile)
                        End Sub
                End Sub
        Next
    End Sub

    ''' <summary>
    ''' just get the offset and make new region offsets
    ''' </summary>
    ''' <param name="tile"></param>
    Private Sub saveManualMoveRegionLocation(tile As SpatialTile, region As TissueRegion, card As RegionSampleCard)
        Dim new_points As Point() = TaskProgress.LoadData(
            Function(p As ITaskProgress)
                Call p.SetProgressMode()
                Call p.SetProgress(0)

                Dim move As SpotMap() = Me.Invoke(Function() tile.GetMapping(p, True).ToArray)

                Return move _
                    .Select(Function(s) New Point(s.SMX.Average, s.SMY.Average)) _
                    .ToArray
            End Function, title:="Save offset", info:="Save and move the tissue region to new location...")

        region.points = new_points

        Call card.SetPolygons(region, callback:=AddressOf updateLayerRendering)
        Call updateLayerRendering()
    End Sub

    Private Sub removeSampleGroup(polygon As RegionSampleCard)
        Call FlowLayoutPanel1.Controls.Remove(polygon)
        Call updateLayerRendering()
    End Sub

    Private Sub SetHtmlColorCode(region As RegionSampleCard)
        InputDialog.Input(Of InputHTMLColorCode)(
            Sub(cfg)
                region.SampleColor = cfg.UserInputColor
            End Sub)
    End Sub

    Private Sub ViewMs1Spectrum(region As RegionSampleCard)
        If viewer Is Nothing Then
            Call Workbench.Warning("Unable to view the ms1 spectrum of the specific sample region due to the reason of MSI viewer data services is not found!")
        End If

        ' get ms1 spectrum data of current region
        Dim ms1 As LibraryMatrix = Nothing
        Dim poly As New Polygon2D(region.ExportTissueRegion(dimension).points)

        Call ProgressSpinner.DoLoading(
            Sub()
                ms1 = viewer.MSIservice.ExtractRegionMs1Spectrum({poly}, region.SampleInfo)
            End Sub)

        ' and then plot on the raw data viewer
        Call SpectralViewerModule.ViewSpectral(ms1)
    End Sub

    ''' <summary>
    ''' 某一个样本区域可能是由多个不连续的区域所组成的
    ''' </summary>
    Friend Sub Add(selector As PixelSelector)
        dimension = selector.dimension_size
        canvas = selector

        Call Add(selector.GetPolygons(popAll:=True), is_raster:=False)
    End Sub

    Private Function Add(sample_group As IEnumerable(Of Polygon2D), is_raster As Boolean) As RegionSampleCard
        Dim card As New RegionSampleCard With {
            .alreadyRaster = is_raster
        }

        Call card.SetPolygons(sample_group, callback:=AddressOf updateLayerRendering)
        Call FlowLayoutPanel1.Controls.Add(card)
        Call ApplyVsTheme(card.ContextMenuStrip1)

        AddHandler card.RemoveSampleGroup, AddressOf removeSampleGroup
        AddHandler card.ViewRegionMs1Spectrum, AddressOf ViewMs1Spectrum
        AddHandler card.SetHtmlColorCode, AddressOf SetHtmlColorCode

        Return card
    End Function

    Private Sub MSIRegionSampleWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = Text

        Call ApplyVsTheme(ToolStrip1)
    End Sub

    Public Sub ShowMessage(text As String)
        Call Me.Invoke(Sub() ToolStripStatusLabel1.Text = text)
    End Sub

    Public Function ExportTissueMaps(dimension As Size, file As Stream) As Boolean
        Dim tissueMorphology As New List(Of TissueRegion)

        For Each region As TissueRegion In GetRegions(dimension)
            Call tissueMorphology.Add(region)
        Next

        Return tissueMorphology _
            .ToArray _
            .WriteCDF(file, dimension)
    End Function

    Public Function ExportSpatialTable(dimension As Size) As SpotAnnotation()
        Dim tissues = GetRegions(dimension).ToArray
        Dim spots = tissues _
            .Select(Iterator Function(seg) As IEnumerable(Of SpotAnnotation)
                        Dim colorHtml As String = seg.color.ToHtmlColor

                        For Each p As Point In seg.points
                            Yield New SpotAnnotation With {
                                .color = colorHtml,
                                .label = seg.label,
                                .x = p.X,
                                .y = p.Y
                            }
                        Next
                    End Function) _
            .IteratesALL _
            .ToArray

        Return spots
    End Function

    Public Sub ClearLayer(canvas As PixelSelector)
        canvas.tissue_layer = Nothing
        canvas.RedrawCanvas()
    End Sub

    ''' <summary>
    ''' get raster pixels data of the polygon regions
    ''' </summary>
    ''' <param name="dimension"></param>
    ''' <returns>populate new tissue regions data</returns>
    Public Iterator Function GetRegions(dimension As Size) As IEnumerable(Of TissueRegion)
        For Each item As Control In FlowLayoutPanel1.Controls
            Dim card = DirectCast(item, RegionSampleCard)
            Dim region As TissueRegion = card.ExportTissueRegion(dimension)

            Yield region
        Next
    End Function

    Public Sub RenderLayer(canvas As PixelSelector)
        Dim picCanvas As Size = canvas.Size
        Dim layerSize As Size = canvas.dimension_size
        Dim configs = InputConfigTissueMap.GetTissueMapViewerConfig
        Dim alphaLevel As Double = configs.opacity / 100
        Dim tissueMaps = GetRegions(dimension).ToArray
        Dim spotSize = configs.spot_size

        If tissueMaps.IsNullOrEmpty Then
            Return
        End If

        Dim layer As Image = LayerRender.Draw(tissueMaps, layerSize, alphaLevel, dotSize:=spotSize)

        Me.canvas = canvas

        canvas.tissue_layer = layer
        canvas.RedrawCanvas()
    End Sub

    ''' <summary>
    ''' transform scaler
    ''' </summary>
    ''' <param name="e"></param>
    Private Shared Function getPoint(e As Point, picCanvas As Size, orginal_imageSize As Size) As Point
        Dim xpoint As Integer
        Dim ypoint As Integer
        Dim Pic_width = picCanvas.Width / orginal_imageSize.Width
        Dim Pic_height = picCanvas.Height / orginal_imageSize.Height

        ' 得到图片上的坐标点
        xpoint = e.X * Pic_width
        ypoint = e.Y * Pic_height

        Return New Point(xpoint, ypoint)
    End Function

    Private Sub AutoFillSampleInfo(nsize As Integer)
        If MessageBox.Show("All of the sample name and color will be generated with unique id fill?",
                               "Tissue Map",
                               buttons:=MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
            Dim i As i32 = 1
            Dim configs = InputConfigTissueMap.GetTissueMapViewerConfig
            Dim colors As LoopArray(Of Color) = Designer.GetColors(configs.color_scaler, nsize)

            For Each item As Control In FlowLayoutPanel1.Controls
                Dim card = DirectCast(item, RegionSampleCard)

                card.SampleColor = ++colors
                card.SampleInfo = $"{configs.region_prefix}{++i}"
            Next

            Call updateLayerRendering()
        End If
    End Sub

    ''' <summary>
    ''' works wel the msi sample is combine from multiple sample files
    ''' </summary>
    Private Sub AutoExtractSampleTags()
        Dim data As RegionLoader = viewer.ExtractMultipleSampleRegions
        Dim colors As Color()
        Dim configs = InputConfigTissueMap.GetTissueMapViewerConfig

        If data Is Nothing Then
            Call Workbench.Warning("Please load MS-imaging related data set at first!")
            Return
        Else
            colors = Designer.GetColors(configs.color_scaler, data.size)
        End If

        For i As Integer = 0 To data.regions.Length - 1
            Dim card = Me.Add({data.regions(i)}, is_raster:=True)

            card.SampleColor = colors(i)
            card.SampleInfo = data.sample_tags(i)
        Next

        Call updateLayerRendering()
    End Sub

    ''' <summary>
    ''' auto sampleinfo
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.ButtonClick
        Dim nsize As Integer = FlowLayoutPanel1.Controls.Count

        If ToolStripButton2.DropDownButtonPressed Then 'OrElse ToolStripButton2.DropDownButtonSelected Then
            Return
        End If

        If nsize > 0 Then
            Call AutoFillSampleInfo(nsize)
        Else
            Call AutoExtractSampleTags()
        End If
    End Sub

    Private Sub updateLayerRendering()
        If Not canvas Is Nothing Then
            If RibbonEvents.ribbonItems.CheckShowMapLayer.BooleanValue Then
                Call RenderLayer(canvas)
            End If

            canvas.EditorConfigs = InputConfigTissueMap.GetPolygonEditorConfig
            canvas.RepaintPolygon()
        End If
    End Sub

    Public Sub SaveTissueMorphologyMatrix() Handles ToolStripButton1.Click
        Dim importsDir As String = If(importsFile.StringEmpty, "", importsFile.ParentPath)

        Using file As New SaveFileDialog With {
            .Filter = "Tissue Morphology Matrix(*.cdf)|*.cdf|Erica spatial table(*.csv)|*.csv",
            .InitialDirectory = importsDir,
            .FileName = importsFile
        }
            If file.ShowDialog = DialogResult.OK Then
                Select Case file.FileName.ExtensionSuffix.ToLower
                    Case "cdf" : Call ExportTissueMaps(dimension, file.OpenFile)
                    Case "csv" : Call ExportSpatialTable(dimension).SaveTo(file.FileName)
                End Select

                Call Workbench.SuccessMessage("Sample tissue regions has been export to file success!")
            End If
        End Using
    End Sub

    ''' <summary>
    ''' config alpha/prefix etc
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Dim getFormula As New InputConfigTissueMap
        Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)

        If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
            Call updateLayerRendering()
        End If
    End Sub

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        If MessageBox.Show(
            text:="Will removes all sample regions?",
            caption:="MSI Sample List",
            buttons:=MessageBoxButtons.OKCancel,
            icon:=MessageBoxIcon.Question) = DialogResult.OK Then

            Call FlowLayoutPanel1.Controls.Clear()
            Call updateLayerRendering()
        End If
    End Sub

    ''' <summary>
    ''' open merge tool
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton6_Click(sender As Object, e As EventArgs) Handles ToolStripButton6.Click
        Dim polygons = GetRegions(dimension).ToArray

        If polygons.IsNullOrEmpty Then
            MessageBox.Show("No tissue map region polygon was found!", "Tissue Map Editor", buttons:=MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Dim getFormula As New SampleRegionMergeTool

            Call getFormula.LoadRegions(polygons, dimension)
            Call InputDialog.Input(
                Sub(cfg)
                    ' update to new regions
                    Call LoadTissueMaps(cfg.GetMergedRegions, canvas)
                    Call updateLayerRendering()
                End Sub, config:=getFormula)
        End If
    End Sub

    ''' <summary>
    ''' 上下翻转
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton7_Click(sender As Object, e As EventArgs) Handles ToolStripButton7.Click
        Dim polygons As TissueRegion() = GetRegions(dimension).ToArray

        If polygons.IsNullOrEmpty Then
            Workbench.Warning("No tissue map polygon region was found...")
            Return
        End If

        polygons = polygons _
            .Select(Function(r)
                        r.points = r.points _
                            .Select(Function(p) New Point(p.X, dimension.Height - p.Y)) _
                            .ToArray
                        Return r
                    End Function) _
            .ToArray

        ' update to new regions
        Call LoadTissueMaps(polygons, canvas)
        Call updateLayerRendering()
    End Sub

    'Private Sub MSIRegionSampleWindow_Resize(sender As Object, e As EventArgs) Handles Me.Resize

    'End Sub

    ''' <summary>
    ''' the anchor is not wrking as expected, fallback to the
    ''' resize event to handling card size updates.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub FlowLayoutPanel1_Resize(sender As Object, e As EventArgs) Handles FlowLayoutPanel1.Resize
        Dim newW As Integer = FlowLayoutPanel1.Width * 0.95

        For Each card As RegionSampleCard In FlowLayoutPanel1.Controls
            card.Width = newW
        Next
    End Sub

    Public Function GetTissueTag(x As Integer, y As Integer) As String
        For Each card As RegionSampleCard In FlowLayoutPanel1.Controls
            If card.PixelPointInside(x, y) Then
                Return card.SampleInfo
            End If
        Next

        Return Nothing
    End Function

    Private Sub GroupUntaggedSpotToNearestRegonToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GroupUntaggedSpotToNearestRegonToolStripMenuItem.Click
        Dim polygons As TissueRegion() = GetRegions(dimension).Where(Function(r) r.nsize > 0).ToArray

        If polygons.Select(Function(r) r.label).Distinct.Count <> polygons.Length Then
            ' has duplicated tissue region label
            Call Workbench.Warning("Tissue region with duplicated label has been found, please make the label unique and then try again!")
            Return
        End If

        Dim xy = getUnLabledPixels(polygons)
        Dim labels As String() = New String(xy.x.Length - 1) {}
        Dim tissueList = polygons.ToDictionary(Function(r) r.label)
        Dim top As Integer = 6

        For i As Integer = 0 To labels.Length - 1
            Dim x As Double = xy.x(i)
            Dim y As Double = xy.y(i)
            Dim minDist As Double = Double.MaxValue
            Dim minLabel As String = Nothing
            Dim distVal As Double

            For Each region As TissueRegion In polygons
                If region.nsize = 0 Then
                    Continue For
                End If

                Dim dist As Double() = region.points _
                    .AsParallel _
                    .Select(Function(pt) (pt.X - x) ^ 2 + (pt.Y - y) ^ 2) _
                    .OrderBy(Function(di) di) _
                    .Take(top) _
                    .ToArray

                If dist.Length < top Then
                    ' 5 - 4 = 1 no changed
                    ' 5 - 4 + 1 = 2, will produce lower score result
                    distVal = dist.Average * (top - dist.Length + 1)
                Else
                    distVal = dist.Average
                End If

                If distVal < minDist Then
                    minDist = distVal
                    minLabel = region.label
                End If
            Next

            labels(i) = minLabel
            tissueList(minLabel).points = tissueList(minLabel).points _
                .Join({New Point(x, y)}) _
                .ToArray
        Next

        ' refresh the control UI
        Call Clear()
        Call LoadTissueMaps(polygons, canvas)

        If labels.Length > 0 Then
            Call Workbench.SuccessMessage($"Group {labels.Length} un-labbled spots into nearest tissue region success!")
        Else
            Call Workbench.StatusMessage("All spot pixels has tissue region label!")
        End If
    End Sub

    Private Function getUnLabledPixels(polygons As TissueRegion()) As (x As Double(), y As Double())
        Dim tagged = polygons _
            .Select(Function(p) p.points) _
            .IteratesALL _
            .Select(Function(a) $"{a.X},{a.Y}") _
            .Distinct _
            .Indexing
        Dim x As New List(Of Double)
        Dim y As New List(Of Double)

        If sample_bounds.IsNullOrEmpty Then
            ' the MS-imaging pixel is started from origin [1,1]
            For i As Integer = 1 To dimension.Width
                For j As Integer = 1 To dimension.Height
                    If Not $"{i},{j}" Like tagged Then
                        Call x.Add(i)
                        Call y.Add(j)
                    End If
                Next
            Next
        Else
            For Each p As Point In sample_bounds
                If Not $"{p.X},{p.Y}" Like tagged Then
                    Call x.Add(p.X)
                    Call y.Add(p.Y)
                End If
            Next
        End If

        Return (x.ToArray, y.ToArray)
    End Function

    Private Sub ToolStripButton5_ButtonClick(sender As Object, e As EventArgs) Handles ToolStripButton5.ButtonClick
        Dim polygons As TissueRegion() = GetRegions(dimension).Where(Function(r) r.nsize > 0).ToArray
        Dim xy = getUnLabledPixels(polygons)

        If xy.x.Length > 0 Then
            Call Add({New Polygon2D(xy.x, xy.y)}, is_raster:=True)
        Else
            Call Workbench.StatusMessage("no spots needs to assign group label.")
        End If
    End Sub

    Private Sub ResetColorsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ResetColorsToolStripMenuItem.Click
        Dim nsize As Integer = FlowLayoutPanel1.Controls.Count

        If nsize > 0 Then
            Dim i As i32 = 1
            Dim configs = InputConfigTissueMap.GetTissueMapViewerConfig
            Dim colors As LoopArray(Of Color) = Designer.GetColors(configs.color_scaler, nsize)

            For Each item As Control In FlowLayoutPanel1.Controls
                DirectCast(item, RegionSampleCard).SampleColor = ++colors
            Next

            Call updateLayerRendering()
        Else
            Call AutoExtractSampleTags()
        End If
    End Sub

End Class