Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.ChartPlots.Plots
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Class SingleCellScatter

    Dim umap_scatter As UMAPPoint()
    Dim clusters_plot As SerialData()

    Dim umap_width As DoubleRange
    Dim umap_height As DoubleRange

    Public Sub LoadCells(umap As IEnumerable(Of UMAPPoint))
        Me.umap_scatter = umap.ToArray
        Me.umap_width = New DoubleRange(umap.Select(Function(c) c.x))
        Me.umap_height = New DoubleRange(umap.Select(Function(c) c.y))
    End Sub

    Public Sub SetRender(args As SingleCellViewerArguments)
        Dim colors As LoopArray(Of Color) = Designer.GetColors(args.colorSet.Description)

        clusters_plot = umap_scatter _
            .GroupBy(Function(c) c.class) _
            .Select(Function(s)
                        Return New SerialData With {
                            .color = ++colors,
                            .lineType = DashStyle.Dot,
                            .pointSize = args.pointSize,
                            .shape = LegendStyles.Circle,
                            .title = s.Key,
                            .width = 0,
                            .pts = s _
                                .Select(Function(c) New PointData(c.x, c.y)) _
                                .ToArray
                        }
                    End Function) _
            .ToArray
    End Sub

    Private Sub RenderScatter()
        Dim size As Size = PictureBox1.Size

        PictureBox1.BackgroundImage = RenderScatter(size, New GraphicsRegion(size, {10, 10, 10, 10}))
    End Sub

    Private Function RenderScatter(size As Size, canvas As GraphicsRegion) As Image
        Dim x As Vector = umap_width.CreateAxisTicks.AsVector
        Dim y As Vector = umap_height.CreateAxisTicks.AsVector
        Dim rect = canvas.PlotRegion
        Dim scale As New DataScaler With {
            .AxisTicks = (x, y),
            .region = rect,
            .X = d3js.scale.linear.domain(x).range(values:=New Double() {rect.Left, rect.Right}),
            .Y = d3js.scale.linear.domain(y).range(values:=New Double() {rect.Top, rect.Bottom})
        }

        Using g As Graphics2D = size.CreateGDIDevice(filled:=BackColor)
            For Each cluster As SerialData In clusters_plot
                Call Scatter2D.DrawScatter(g, cluster, scale).ToArray
            Next

            Return g.ImageResource
        End Using
    End Function

End Class
