Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.mzkit_win32.My
Imports ControlLibrary
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language

Public Class MSIRegionSampleWindow

    Public ReadOnly Property IsNullOrEmpty As Boolean
        Get
            Return FlowLayoutPanel1.Controls.Count = 0
        End Get
    End Property

    Friend dimension As Size
    Friend canvas As PixelSelector

    Friend Sub Clear()
        FlowLayoutPanel1.Controls.Clear()
    End Sub

    Public Overloads Sub LoadTissueMaps(tissues As TissueRegion(), canvas As PixelSelector)
        Me.canvas = canvas
        Me.dimension = canvas.dimension_size

        For Each region As TissueRegion In tissues
            Dim card As New RegionSampleCard

            Call card.SetPolygons(region, callback:=AddressOf updateLayerRendering)
            Call FlowLayoutPanel1.Controls.Add(card)

            card.SampleColor = region.color
            card.SampleInfo = region.label
        Next
    End Sub

    ''' <summary>
    ''' 某一个样本区域可能是由多个不连续的区域所组成的
    ''' </summary>
    Friend Sub Add(selector As PixelSelector)
        Dim card As New RegionSampleCard

        dimension = selector.dimension_size
        canvas = selector

        Call card.SetPolygons(selector.GetPolygons, callback:=AddressOf updateLayerRendering)
        Call FlowLayoutPanel1.Controls.Add(card)
    End Sub

    Private Sub MSIRegionSampleWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = Text

        Call ApplyVsTheme(ToolStrip1)
    End Sub

    Public Function ExportTissueMaps(dimension As Size, file As Stream) As Boolean
        Dim tissueMorphology As New List(Of TissueRegion)

        For Each item As Control In FlowLayoutPanel1.Controls
            Dim card = DirectCast(item, RegionSampleCard)
            Dim region As TissueRegion = card.ExportTissueRegion(dimension)

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

    Public Sub RenderLayer(canvas As PixelSelector)
        Dim picCanvas As Size = canvas.Size
        Dim layerSize As Size = canvas.dimension_size
        Dim layer As New Bitmap(canvas.dimension_size.Width, canvas.dimension_size.Height, format:=PixelFormat.Format32bppArgb)
        Dim g As Graphics = Graphics.FromImage(layer)
        Dim dotSize As New Size(1, 1)

        Me.canvas = canvas

        g.CompositingQuality = CompositingQuality.HighQuality
        g.InterpolationMode = InterpolationMode.HighQualityBilinear
        g.Clear(Color.Transparent)

        For Each item As Control In FlowLayoutPanel1.Controls
            Dim card = DirectCast(item, RegionSampleCard)
            Dim region As TissueRegion = card.ExportTissueRegion(dimension)
            Dim fill As New SolidBrush(region.color.Alpha(255 * 0.9))

            For Each p As Point In region.points
                Call g.FillRectangle(fill, New Rectangle(p, dotSize))
            Next
        Next

        g.Flush()
        g.Dispose()

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
            Dim colors As LoopArray(Of Color) = Designer.GetColors("Paper", FlowLayoutPanel1.Controls.Count)

            For Each item As Control In FlowLayoutPanel1.Controls
                Dim card = DirectCast(item, RegionSampleCard)

                card.SampleColor = ++colors
                card.SampleInfo = $"region_{++i}"
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
End Class