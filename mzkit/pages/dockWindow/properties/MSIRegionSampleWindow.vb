Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Globalization
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.mzkit_win32.My
Imports ControlLibrary
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Public Class MSIRegionSampleWindow

    Public ReadOnly Property IsNullOrEmpty As Boolean
        Get
            Return FlowLayoutPanel1.Controls.Count = 0
        End Get
    End Property

    Friend dimension As Size
    Friend canvas As PixelSelector
    Friend sample_bounds As Point()

    Friend Sub Clear()
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
        Me.canvas = canvas
        Me.dimension = canvas.dimension_size

        Call Clear()

        For Each region As TissueRegion In tissues
            Dim card As New RegionSampleCard

            Call card.SetPolygons(region, callback:=AddressOf updateLayerRendering)
            Call FlowLayoutPanel1.Controls.Add(card)

            ' card.Anchor = AnchorStyles.Left Or AnchorStyles.Right
            card.SampleColor = region.color
            card.SampleInfo = region.label

            AddHandler card.RemoveSampleGroup, AddressOf removeSampleGroup
        Next
    End Sub

    Private Sub removeSampleGroup(polygon As RegionSampleCard)
        Call FlowLayoutPanel1.Controls.Remove(polygon)
        Call updateLayerRendering()
    End Sub

    ''' <summary>
    ''' 某一个样本区域可能是由多个不连续的区域所组成的
    ''' </summary>
    Friend Sub Add(selector As PixelSelector)
        dimension = selector.dimension_size
        canvas = selector

        Call Add(selector.GetPolygons(popAll:=True))
    End Sub

    Private Sub Add(sample_group As IEnumerable(Of Polygon2D))
        Dim card As New RegionSampleCard

        Call card.SetPolygons(sample_group, callback:=AddressOf updateLayerRendering)
        Call FlowLayoutPanel1.Controls.Add(card)

        AddHandler card.RemoveSampleGroup, AddressOf removeSampleGroup
    End Sub

    Private Sub MSIRegionSampleWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = Text

        Call ApplyVsTheme(ToolStrip1)
    End Sub

    Public Function ExportTissueMaps(dimension As Size, file As Stream) As Boolean
        Dim tissueMorphology As New List(Of TissueRegion)

        For Each region As TissueRegion In GetRegions(dimension)
            Call tissueMorphology.Add(region)
        Next

        Return tissueMorphology _
            .ToArray _
            .WriteCDF(file)
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
        Dim layer As Image = LayerRender.Draw(GetRegions(dimension), layerSize, alphaLevel, dotSize:=1)

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

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        If MessageBox.Show("All of the sample name and color will be generated with unique id fill?", "Tissue Map", buttons:=MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
            Dim i As i32 = 1
            Dim colors As LoopArray(Of Color) = Designer.GetColors(colorSet, FlowLayoutPanel1.Controls.Count)

            For Each item As Control In FlowLayoutPanel1.Controls
                Dim card = DirectCast(item, RegionSampleCard)

                card.SampleColor = ++colors
                card.SampleInfo = $"{prefix}{++i}"
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
        Using file As New SaveFileDialog With {
            .Filter = "Tissue Morphology Matrix(*.cdf)|*.cdf"
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
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

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

    Private Sub ToolStripButton5_Click(sender As Object, e As EventArgs) Handles ToolStripButton5.Click
        Dim polygons = GetRegions(dimension).ToArray
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

        Call Add({New Polygon2D(x.ToArray, y.ToArray)})
    End Sub

    ''' <summary>
    ''' open merge tool
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton6_Click(sender As Object, e As EventArgs) Handles ToolStripButton6.Click
        Dim polygons = GetRegions(dimension).ToArray
        Dim getFormula As New SampleRegionMergeTool
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

        Call getFormula.LoadRegions(polygons)

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
End Class