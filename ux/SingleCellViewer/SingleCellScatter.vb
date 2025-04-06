#Region "Microsoft.VisualBasic::f2c9f22655e39ba06d2aef7b1c82d2be, mzkit\ux\SingleCellViewer\SingleCellScatter.vb"

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

    '   Total Lines: 275
    '    Code Lines: 211 (76.73%)
    ' Comment Lines: 14 (5.09%)
    '    - Xml Docs: 92.86%
    ' 
    '   Blank Lines: 50 (18.18%)
    '     File Size: 10.13 KB


    ' Class SingleCellScatter
    ' 
    '     Function: GetCanvas, GetCell, GetEmbedding, RenderScatter
    ' 
    '     Sub: FilterByCluster, FilterByUMAPSpace, LoadCells, LoadCellViews, LoadClusterData
    '          LoadHeatmapData, PictureBox1_MouseClick, PictureBox1_MouseMove, PictureBox1_SizeChanged, RenderScatter
    '          ResetDataView, SetRender, ShowMessage
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
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
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports SMRUCC.genomics.Analysis.SingleCell
Imports std = System.Math

Public Class SingleCellScatter

    ''' <summary>
    ''' the raw input data list
    ''' </summary>
    Dim rawDataList As IEmbeddingScatter()
    Dim rawClusters As Dictionary(Of String, IEmbeddingScatter())

    ''' <summary>
    ''' the data view list of current scatter data selection
    ''' </summary>
    Dim umap_scatter As IEmbeddingScatter()
    Dim clusters_plot As SerialData()

    Dim umap_x, umap_y As Vector
    Dim umap_width As DoubleRange
    Dim umap_height As DoubleRange

    Dim x_axis As BlockSearchFunction(Of IEmbeddingScatter)
    Dim y_axis As BlockSearchFunction(Of IEmbeddingScatter)

    ''' <summary>
    ''' does the data has been initialized?
    ''' </summary>
    Dim hasData As Boolean = False

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetEmbedding() As IEnumerable(Of IEmbeddingScatter)
        Return rawDataList.SafeQuery
    End Function

    Public Sub LoadCells(umap As IEnumerable(Of IEmbeddingScatter))
        rawDataList = umap.ToArray
        rawClusters = rawDataList _
            .GroupBy(Function(c) c.cluster) _
            .ToDictionary(Function(c) c.Key,
                          Function(c)
                              Return c.ToArray
                          End Function)

        LoadCellViews(rawDataList)
    End Sub

    Public Sub ResetDataView()
        Call LoadCellViews(rawDataList)
    End Sub

    Public Sub FilterByUMAPSpace(space As RectangleF)
        Call LoadCellViews(rawDataList.AsParallel.Where(Function(c) space.Contains(c.x, c.y)))
    End Sub

    Public Sub FilterByCluster(clusters As IEnumerable(Of String))
        Call LoadCellViews(clusters.Select(Function(key) rawClusters(key)).IteratesALL)
    End Sub

    Private Sub LoadCellViews(umap As IEnumerable(Of IEmbeddingScatter))
        Me.umap_scatter = umap.ToArray
        Me.umap_width = New DoubleRange(umap.Select(Function(c) c.x))
        Me.umap_height = New DoubleRange(umap.Select(Function(c) c.y))
        Me.umap_height = New DoubleRange(umap_height.Min * 1.5, umap_height.Max)
        Me.umap_x = umap_width.CreateAxisTicks.AsVector
        Me.umap_y = umap_height.CreateAxisTicks.AsVector
        Me.hasData = Not umap_scatter.IsNullOrEmpty
        Me.clusters_plot = Nothing

        If hasData Then
            Dim blocks As Integer = 20
            Dim dx As Double = umap_width.Length / blocks
            Dim dy As Double = umap_height.Length / blocks

            x_axis = New BlockSearchFunction(Of IEmbeddingScatter)(umap_scatter, Function(i) i.x, dx, fuzzy:=True)
            y_axis = New BlockSearchFunction(Of IEmbeddingScatter)(umap_scatter, Function(i) i.y, dy, fuzzy:=True)
        End If
    End Sub

    ''' <summary>
    ''' set render arguments and rendering of the data view
    ''' </summary>
    ''' <param name="args"></param>
    Public Sub SetRender(args As SingleCellViewerArguments)
        If Not hasData Then
            Return
        Else
            BackColor = args.background
        End If

        If args.heatmap Then
            Call LoadHeatmapData(args)
        Else
            Call LoadClusterData(args)
        End If

        Call RenderScatter()
    End Sub

    Private Sub LoadHeatmapData(args As SingleCellViewerArguments)
        Dim scatter = umap_scatter.OfType(Of SingleExpression).ToArray
        Dim range As New DoubleRange(scatter.Select(Function(a) a.expression))
        Dim colors As String() = Designer.GetColors(ScalerPalette.Seismic.Description, 30) _
            .Select(Function(c) c.ToHtmlColor) _
            .ToArray
        Dim offset As New DoubleRange(0, colors.Length - 1)
        Dim points As New List(Of PointData)

        For Each exp As SingleExpression In scatter
            Call points.Add(New PointData(exp.Get2dEmbedding) With {
                .color = colors(CInt(range.ScaleMapping(exp.expression, offset))),
                .value = exp.expression
            })
        Next

        clusters_plot = {New SerialData With {
            .lineType = DashStyle.Dot,
            .pointSize = args.pointSize,
            .shape = LegendStyles.Circle,
            .pts = points.ToArray,
            .width = 0,
            .color = Color.Black,
            .title = "Expression HeatMap"
        }}
    End Sub

    Private Sub LoadClusterData(args As SingleCellViewerArguments)
        Dim colors As LoopArray(Of Color) = Designer.GetColors(args.colorSet.Description)

        clusters_plot = umap_scatter _
            .GroupBy(Function(c) c.cluster) _
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

    Public Sub ShowMessage(msg As String)
        ToolStripStatusLabel1.Text = msg
    End Sub

    Private Function GetCell(xy As Point, Optional ByRef umap_x As Double = 0, Optional ByRef umap_y As Double = 0) As UMAPPoint
        Dim canvas As Size = PictureBox1.Size
        Dim width As New DoubleRange(0, canvas.Width)
        Dim height As New DoubleRange(0, canvas.Height)

        umap_x = width.ScaleMapping(xy.X, umap_width)
        umap_y = height.ScaleMapping(xy.Y, umap_height)

        Dim filter1 = x_axis.Search(New UMAPPoint("", umap_x, 0, 0))
        Dim sx = umap_x
        Dim sy = umap_y
        Dim union = filter1 _
            .OrderBy(Function(i) (std.Abs(i.x - sx) + std.Abs(i.y - sy)) / 2) _
            .FirstOrDefault

        If union Is Nothing Then
            Return Nothing
        End If

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

        Dim umap_x, umap_y As Double
        Dim cell = GetCell(xy, umap_x, umap_y)

        If Not cell Is Nothing Then
            ' RaiseEvent SelectCell(union.label, union)

            ToolStripStatusLabel1.Text = $"[{xy.X}, {xy.Y}] -> {cell.x.ToString("F3")},{cell.y.ToString("F3")} {cell.label}"
        Else
            ToolStripStatusLabel1.Text = $"[{xy.X}, {xy.Y}] -> {umap_x},{umap_y}"
        End If
    End Sub

    Private Sub PictureBox1_MouseClick(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseClick
        Dim xy As Point = PictureBox1.PointToClient(Cursor.Position)

        If Not hasData Then
            Return
        End If

        Dim cell = GetCell(xy)

        If Not cell Is Nothing Then
            RaiseEvent SelectCell(cell.label, cell)
        End If
    End Sub

    Private Function GetCanvas() As GraphicsRegion
        Return New GraphicsRegion(PictureBox1.Size, {10, 10, 10, 10})
    End Function

    Private Function RenderScatter(size As Size, canvas As GraphicsRegion) As Image
        Dim css As New CSSEnvirnment(size)
        Dim rect As Rectangle = canvas.PlotRegion(css)
        Dim scale As New DataScaler With {
            .AxisTicks = (umap_x, umap_y),
            .region = rect,
            .X = d3js.scale.linear.domain(umap_x).range(values:=New Double() {rect.Left, rect.Right}),
            .Y = d3js.scale.linear.domain(umap_y).range(values:=New Double() {rect.Top, rect.Bottom})
        }

        Using g As IGraphics = DriverLoad.CreateDefaultRasterGraphics(size, BackColor)
            For Each cluster As SerialData In clusters_plot
                Call Scatter2D.DrawScatter(g, cluster, scale).ToArray
            Next

            Return DirectCast(g, GdiRasterGraphics).ImageResource
        End Using
    End Function
End Class
