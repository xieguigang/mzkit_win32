Imports System.Drawing
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
        Using g = Me.Size.CreateGDIDevice(filled:=Color.Transparent)

        End Using
    End Sub
End Class
