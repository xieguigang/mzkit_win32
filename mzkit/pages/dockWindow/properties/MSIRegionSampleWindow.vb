Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports Mzkit_win32.MSImagingViewerV2
Imports ServiceHub

Public Class MSIRegionSampleWindow

    Public ReadOnly Property IsNullOrEmpty As Boolean
        Get
            Return FlowLayoutPanel1.Controls.Count = 0
        End Get
    End Property

    Friend dimension As Size
    Friend canvas As PixelSelector
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

    Public Overloads Sub LoadTissueMaps(tissues As TissueRegion(), canvas As PixelSelector)
        If canvas Is Nothing Then
            canvas = WindowModules.viewer.PixelSelector1.MSICanvas
        End If

        Me.canvas = canvas
        Me.dimension = canvas.dimension_size
        Me.importsFile = $"Load {tissues.Length} tissue region maps!"

        Call Clear()

        For Each region As TissueRegion In tissues
            Dim card As New RegionSampleCard

            Call card.SetPolygons(region, callback:=AddressOf updateLayerRendering)
            Call FlowLayoutPanel1.Controls.Add(card)

            ' card.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            card.SampleColor = region.color
            card.SampleInfo = region.label

            AddHandler card.RemoveSampleGroup, AddressOf removeSampleGroup
            AddHandler card.ViewRegionMs1Spectrum, AddressOf ViewMs1Spectrum
        Next
    End Sub

    Private Sub removeSampleGroup(polygon As RegionSampleCard)
        Call FlowLayoutPanel1.Controls.Remove(polygon)
        Call updateLayerRendering()
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

        Call Add(selector.GetPolygons(popAll:=True))
    End Sub

    Private Function Add(sample_group As IEnumerable(Of Polygon2D)) As RegionSampleCard
        Dim card As New RegionSampleCard

        card.SetPolygons(sample_group, callback:=AddressOf updateLayerRendering)
        FlowLayoutPanel1.Controls.Add(card)

        AddHandler card.RemoveSampleGroup, AddressOf removeSampleGroup
        AddHandler card.ViewRegionMs1Spectrum, AddressOf ViewMs1Spectrum

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

    Public Sub ClearLayer(canvas As PixelSelector)
        canvas.tissue_layer = Nothing
        canvas.RedrawCanvas()
    End Sub

    ''' <summary>
    ''' get raster pixels data of the polygon regions
    ''' </summary>
    ''' <param name="dimension"></param>
    ''' <returns></returns>
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
        Dim alphaLevel As Double = Me.alpha / 100
        Dim tissueMaps = GetRegions(dimension).ToArray

        If tissueMaps.IsNullOrEmpty Then
            Return
        End If

        Dim layer As Image = LayerRender.Draw(tissueMaps, layerSize, alphaLevel, dotSize:=1)

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

    ''' <summary>
    ''' auto sampleinfo
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Dim nsize As Integer = FlowLayoutPanel1.Controls.Count

        If nsize > 0 Then
            If MessageBox.Show("All of the sample name and color will be generated with unique id fill?", "Tissue Map", buttons:=MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
                Dim i As i32 = 1
                Dim colors As LoopArray(Of Color) = Designer.GetColors(colorSet, nsize)

                For Each item As Control In FlowLayoutPanel1.Controls
                    Dim card = DirectCast(item, RegionSampleCard)

                    card.SampleColor = ++colors
                    card.SampleInfo = $"{prefix}{++i}"
                Next

                Call updateLayerRendering()
            End If
        Else
            Dim data As RegionLoader = viewer.ExtractMultipleSampleRegions
            Dim colors As Color() = Designer.GetColors(colorSet, data.size)

            For i As Integer = 0 To data.regions.Length - 1
                Dim card = Me.Add({data.regions(i)})

                card.SampleColor = colors(i)
                card.SampleInfo = data.sample_tags(i)
            Next

            Call updateLayerRendering()
        End If
    End Sub

    Private Sub updateLayerRendering()
        If Not canvas Is Nothing AndAlso RibbonEvents.ribbonItems.CheckShowMapLayer.BooleanValue Then
            Call RenderLayer(canvas)
        End If
    End Sub

    Public Sub SaveTissueMorphologyMatrix() Handles ToolStripButton1.Click
        Dim importsDir As String = If(importsFile.StringEmpty, "", importsFile.ParentPath)

        Using file As New SaveFileDialog With {
            .Filter = "Tissue Morphology Matrix(*.cdf)|*.cdf",
            .InitialDirectory = importsDir,
            .FileName = importsFile
        }
            If file.ShowDialog = DialogResult.OK Then
                Call ExportTissueMaps(dimension, file.OpenFile)
                Call MyApplication.host.showStatusMessage("Sample tissue regions has been export to file success!")
            End If
        End Using
    End Sub

    Dim prefix As String = "region_"
    Dim colorSet As String = "paper"
    Dim alpha As Double = 80

    ''' <summary>
    ''' config alpha/prefix etc
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Dim getFormula As New InputConfigTissueMap
        Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)

        getFormula.AlphaLevel = alpha
        getFormula.RegionPrefix = prefix

        If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
            prefix = getFormula.RegionPrefix
            colorSet = getFormula.ColorSet
            alpha = getFormula.AlphaLevel

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
            Return
        End If

        Dim getFormula As New SampleRegionMergeTool
        Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)

        Call getFormula.LoadRegions(polygons, dimension)

        If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
            ' update to new regions
            Call LoadTissueMaps(getFormula.GetMergedRegions, canvas)
            Call updateLayerRendering()
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
            MyApplication.host.warning("No tissue map polygon region was found...")
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
        Dim polygons As TissueRegion() = GetRegions(dimension).ToArray
        Dim xy = getUnLabledPixels(polygons)
        Dim labels As String() = New String(xy.x.Length - 1) {}
        Dim tissueList = polygons.ToDictionary(Function(r) r.label)

        For i As Integer = 0 To labels.Length - 1
            Dim x As Double = xy.x(i)
            Dim y As Double = xy.y(i)
            Dim minDist As Double = Double.MaxValue
            Dim minLabel As String = Nothing

            For Each region As TissueRegion In polygons
                Dim dist As Double = region.points.Select(Function(pt) (pt.X - x) ^ 2 + (pt.Y - y) ^ 2).Min

                If dist < minDist Then
                    minDist = dist
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
            For i As Integer = 0 To dimension.Width
                For j As Integer = 0 To dimension.Height
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
        Dim polygons As TissueRegion() = GetRegions(dimension).ToArray
        Dim xy = getUnLabledPixels(polygons)

        Call Add({New Polygon2D(xy.x, xy.y)})
    End Sub
End Class