Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
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

    Dim dimension As Size

    Friend Sub Clear()
        FlowLayoutPanel1.Controls.Clear()
    End Sub

    ''' <summary>
    ''' 某一个样本区域可能是由多个不连续的区域所组成的
    ''' </summary>
    Friend Sub Add(selector As PixelSelector)
        Dim card As New RegionSampleCard

        card.SetPolygons(selector.GetPolygons)
        dimension = selector.dimension_size

        FlowLayoutPanel1.Controls.Add(card)
    End Sub

    Private Sub MSIRegionSampleWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = Text

        Call ApplyVsTheme(ToolStrip1)
    End Sub

    Friend Function ToArray() As Rectangle()
        Throw New NotImplementedException()
    End Function

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
        Dim dotSize As New Size(3, 3)
        Dim layer As New Bitmap(canvas.dimension_size.Width * 3, canvas.dimension_size.Height * 3, format:=PixelFormat.Format32bppArgb)
        Dim g As Graphics = Graphics.FromImage(layer)

        g.CompositingQuality = CompositingQuality.HighQuality
        g.InterpolationMode = InterpolationMode.HighQualityBilinear
        g.Clear(Color.Transparent)

        For Each item As Control In FlowLayoutPanel1.Controls
            Dim card = DirectCast(item, RegionSampleCard)
            Dim region As TissueRegion = card.ExportTissueRegion(dimension)
            Dim fill As New SolidBrush(region.color.Alpha(255 * 0.65))

            For Each p As Point In region.points
                Call g.FillRectangle(fill, New Rectangle(p, dotSize))
            Next
        Next

        g.Flush()
        g.Dispose()

        canvas.tissue_layer = layer
        canvas.RedrawCanvas()
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        If MessageBox.Show("All of the sample name and color will be generated with unique id fill?", "Tissue Map", buttons:=MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
            Dim i As i32 = 1
            Dim colors As LoopArray(Of Color) = Designer.GetColors("Paper", FlowLayoutPanel1.Controls.Count)

            For Each item As Control In FlowLayoutPanel1.Controls
                Dim card = DirectCast(item, RegionSampleCard)

                card.SampleColor = ++colors
                card.SampleInfo = $"region_{++i}"
            Next
        End If
    End Sub

    Public Sub SaveTissueMorphologyMatrix() Handles ToolStripButton1.Click
        Using file As New SaveFileDialog With {
            .Filter = "Tissue Morphology Matrix(*.cdf)|*.cdf"
        }
            If file.ShowDialog = DialogResult.OK Then
                Call ExportTissueMaps(dimension, file.OpenFile)
            End If
        End Using
    End Sub
End Class