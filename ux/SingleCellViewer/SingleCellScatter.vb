Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Legend
Imports Microsoft.VisualBasic.Data.ChartPlots.Plots
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports std = System.Math

Public Class SingleCellScatter

    Dim umap_scatter As UMAPPoint()
    Dim clusters_plot As SerialData()

    Dim umap_x, umap_y As Vector
    Dim umap_width As DoubleRange
    Dim umap_height As DoubleRange

    Dim x_axis As BlockSearchFunction(Of UMAPPoint)
    Dim y_axis As BlockSearchFunction(Of UMAPPoint)

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

        If hasData Then
            Dim dx As Double = umap_width.Length / 5
            Dim dy As Double = umap_height.Length / 5

            x_axis = New BlockSearchFunction(Of UMAPPoint)(umap_scatter, Function(i) i.x, dx, fuzzy:=True)
            y_axis = New BlockSearchFunction(Of UMAPPoint)(umap_scatter, Function(i) i.y, dy, fuzzy:=True)
        End If
    End Sub

    ''' <summary>
    ''' set render arguments and rendering of the data view
    ''' </summary>
    ''' <param name="args"></param>
    Public Sub SetRender(args As SingleCellViewerArguments)
        Dim colors As LoopArray(Of Color) = Designer.GetColors(args.colorSet.Description)

        If Not hasData Then
            Return
        End If

        BackColor = args.background
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

        If clusters_plot.IsNullOrEmpty OrElse size.Width = 0 OrElse size.Height = 0 Then
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

    Public Event SelectCell(cell_id As String, umap As UMAPPoint)

    Private Function GetCell() As UMAPPoint
        Dim canvas As Size = PictureBox1.Size
        Dim width As New DoubleRange(0, canvas.Width)
        Dim height As New DoubleRange(0, canvas.Height)
        Dim umap_x As Double = width.ScaleMapping(xy.X, umap_width)
        Dim umap_y As Double = height.ScaleMapping(xy.Y, umap_height)
        Dim filter1 = x_axis.Search(New UMAPPoint("", umap_x, 0, 0))
        Dim union = filter1 _
            .OrderBy(Function(i) (std.Abs(i.x - umap_x) + std.Abs(i.y - umap_y)) / 2) _
            .FirstOrDefault

        If std.Abs(union.y - umap_y) < 0.1 Then
            Return union
        Else
            Return Nothing
        End If
    End Function

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        Dim xy As Point = PictureBox1.PointToClient(Cursor.Position)

        If Not hasData Then
            ToolStripStatusLabel1.Text = $"[{xy.X}, {xy.Y}] -> {0},{0}"

            Return
        End If

        Dim cell = GetCell()

        If Not cell Is Nothing Then
            ' RaiseEvent SelectCell(union.label, union)

            ToolStripStatusLabel1.Text = $"[{xy.X}, {xy.Y}] -> {umap_x.ToString("F3")},{umap_y.ToString("F3")} {cell.label}"
        Else
            ToolStripStatusLabel1.Text = $"[{xy.X}, {xy.Y}] -> {umap_x},{umap_y}"
        End If
    End Sub

    Private Sub PictureBox1_MouseClick(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseClick
        Dim xy As Point = PictureBox1.PointToClient(Cursor.Position)

        If Not hasData Then
            Return
        End If

        Dim cell = GetCell()

        If Not cell Is Nothing Then
            RaiseEvent SelectCell(cell.label, cell)
        End If
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
