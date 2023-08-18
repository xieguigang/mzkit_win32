Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports STImaging

Public Class HEStainViewer

    Dim MSIMatrix As PixelData()
    Dim MSIDims As Size
    Dim HEImageSize As Size
    Dim HEBitmap As Bitmap
    Dim WithEvents tile As SpatialTile

    Private Sub HEStainViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.DoubleBuffered = True
    End Sub

    Public Function LoadRawData(MSI As IEnumerable(Of PixelData), msi_dims As Size, HEstain As Image) As SpatialTile
        Me.MSIMatrix = MSI.ToArray
        Me.MSIDims = msi_dims
        Me.HEImageSize = HEstain.Size
        Me.BackgroundImage = HEstain
        Me.Refresh()
        Me.tile = LoadUI()
        Me.HEBitmap = New Bitmap(HEstain)

        Return tile
    End Function

    Private Function LoadUI() As SpatialTile
        Dim tile As New SpatialTile With {.DragMode = 1, .SaveExport = AddressOf SaveExport}
        Dim matrix As SpatialSpot() = MSIMatrix _
            .Select(Function(pi)
                        Return New SpatialSpot With {
                            .barcode = $"{pi.X},{pi.Y}",
                            .flag = 1,
                            .px = pi.X,
                            .py = pi.Y,
                            .x = pi.X,
                            .y = pi.Y
                        }
                    End Function) _
            .ToArray

        Call tile.ShowMatrix(matrix)
        Call tile.SetHeatmapData(From m In MSIMatrix Select m.Scale)

        Call Me.Controls.Add(tile)

        ' AddHandler tile.GetSpatialMetabolismPoint, AddressOf getPoint
        ' AddHandler tile.ClickSpatialMetabolismPixel, Sub(e, ByRef px, ByRef py) Call clickGetPoint(e)

        Return tile
    End Function

    Private Sub SaveExport()

    End Sub

    Dim xy As Point

    Private Sub tile_GetSpatialMetabolismPoint(smXY As Point, ByRef x As Integer, ByRef y As Integer, ByRef tissueMorphology As String) Handles tile.GetSpatialMetabolismPoint
        Me.xy = smXY

        Call PixelSelector.getPoint(smXY, HEImageSize, Me.Size, x, y)

        Dim c As Color = HEBitmap.GetPixel(x, y)
        tissueMorphology = c.ARGBExpression
    End Sub

    Private Sub OnBoardPaint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        If xy.IsEmpty Then
            Return
        End If

        Dim g = e.Graphics
        Dim dashLine As New Pen(Brushes.Black, 1) With {.DashStyle = DashStyle.Dash}

        g.DrawLine(dashLine, New Point(0, xy.Y), New Point(Width, xy.Y))
        g.DrawLine(dashLine, New Point(xy.X, 0), New Point(xy.X, Height))
        g.Flush()
    End Sub
End Class
