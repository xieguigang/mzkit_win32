Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap

Public Class SpatialTile

    Dim spatialMatrix As PixelData()
    Dim colors As ScalerPalette
    Dim radius As Integer = 10

    Private Sub AdjustRadius() Handles AdjustRadiusToolStripMenuItem.Click

    End Sub

    Public Sub Plot()
        Dim colors As SolidBrush() = Designer _
            .GetColors(Me.colors.Description, 24) _
            .Select(Function(c) New SolidBrush(c)) _
            .ToArray
        Dim range As New DoubleRange(spatialMatrix.Select(Function(i) i.Scale))
        Dim index As New DoubleRange(0, colors.Length)

        Using g = Me.Size.CreateGDIDevice(filled:=Color.Transparent)
            For Each spot As PixelData In spatialMatrix
                Call g.DrawCircle(New Point(spot.X, spot.Y), radius, colors(CInt(range.ScaleMapping(spot.Scale, index))))
            Next

            Call g.Flush()

        End Using
    End Sub
End Class
