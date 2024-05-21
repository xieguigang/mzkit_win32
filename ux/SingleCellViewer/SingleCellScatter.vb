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

    Dim umap_x, umap_y As Vector
    Dim umap_width As DoubleRange
    Dim umap_height As DoubleRange

    ''' <summary>
    ''' does the data has been initialized?
    ''' </summary>
    Dim hasData As Boolean = False

    Public Sub LoadCells(umap As IEnumerable(Of UMAPPoint))
        Me.umap_scatter = umap.ToArray
        Me.umap_width = New DoubleRange(umap.Select(Function(c) c.x))
        Me.umap_height = New DoubleRange(umap.Select(Function(c) c.y))
        Me.umap_x = umap_width.CreateAxisTicks.AsVector
        Me.umap_y = umap_height.CreateAxisTicks.AsVector
        Me.hasData = Not umap_scatter.IsNullOrEmpty
        Me.clusters_plot = Nothing
    End Sub

    Public Sub SetRender(args As SingleCellViewerArguments)
        Dim colors As LoopArray(Of Color) = Designer.GetColors(args.colorSet.Description)

        If Not hasData Then
            Return
        End If

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

        Call RenderScatter()
    End Sub

    Private Sub RenderScatter()
        Dim size As Size = PictureBox1.Size

        If clusters_plot.IsNullOrEmpty Then
            Return
        End If

        PictureBox1.BackgroundImage = RenderScatter(size, GetCanvas)
    End Sub

    Private Sub PictureBox1_SizeChanged(sender As Object, e As EventArgs) Handles PictureBox1.SizeChanged
        If Not hasData Then
            Return
        End If

        RenderScatter()
        client_region = GetCanvas()
    End Sub

    Dim client_region As GraphicsRegion

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        Dim xy As Point = PictureBox1.PointToClient(Cursor.Position)

        If Not hasData Then
            ToolStripStatusLabel1.Text = $"[{xy.X}, {xy.Y}] -> {0},{0}"

            Return
        End If

        Dim canvas As Size = PictureBox1.Size
        Dim width As New DoubleRange(0, canvas.Width)
        Dim height As New DoubleRange(0, canvas.Height)
        Dim umap_x As Double = width.ScaleMapping(xy.X, umap_width)
        Dim umap_y As Double = height.ScaleMapping(xy.Y, umap_height)

        ToolStripStatusLabel1.Text = $"[{xy.X}, {xy.Y}] -> {umap_x},{umap_y}"
    End Sub

    Private Function GetCanvas() As GraphicsRegion
        Return New GraphicsRegion(PictureBox1.Size, {10, 10, 10, 10})
    End Function

    Private Function RenderScatter(size As Size, canvas As GraphicsRegion) As Image
        Dim rect = canvas.PlotRegion
        Dim scale As New DataScaler With {
            .AxisTicks = (umap_x, umap_y),
            .region = rect,
            .X = d3js.scale.linear.domain(umap_x).range(values:=New Double() {rect.Left, rect.Right}),
            .Y = d3js.scale.linear.domain(umap_y).range(values:=New Double() {rect.Top, rect.Bottom})
        }

        Using g As Graphics2D = size.CreateGDIDevice(filled:=BackColor)
            For Each cluster As SerialData In clusters_plot
                Call Scatter2D.DrawScatter(g, cluster, scale).ToArray
            Next

            Return g.ImageResource
        End Using
    End Function

End Class
