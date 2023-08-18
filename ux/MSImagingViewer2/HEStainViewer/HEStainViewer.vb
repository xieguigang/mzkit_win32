Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports STImaging

Public Class HEStainViewer

    Dim MSIMatrix As PixelData()
    Dim MSIDims As Size
    Dim HEImage As Size
    Dim tile As SpatialTile

    Private Sub HEStainViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.DoubleBuffered = True
    End Sub

    Public Function LoadRawData(MSI As IEnumerable(Of PixelData), msi_dims As Size, HEstain As Image) As SpatialTile
        Me.MSIMatrix = MSI.ToArray
        Me.MSIDims = msi_dims
        Me.HEImage = HEstain.Size
        Me.BackgroundImage = HEstain
        Me.Refresh()
        Me.tile = LoadUI()

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

End Class
