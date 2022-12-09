Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Math

Public Class InputRotateMatrix

    Public Property Tile As SpatialTile

    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        Dim angle As Double = NumericUpDown1.Value
        Dim matrix = Tile.spatialSpots
        Dim spots = matrix.Select(Function(s) New PointF(s.px, s.py)).ToArray
        Dim r As Double = angle.ToRadians
        Dim polygon As New Polygon2D(spots)
        Dim center As New PointF With {
            .X = polygon.xpoints.Average,
            .Y = polygon.ypoints.Average
        }

        spots = spots.Rotate(center, alpha:=r)

        For i As Integer = 0 To spots.Length - 1
            matrix(i).px = spots(i).X
            matrix(i).py = spots(i).Y
        Next

        ' update plot
        Call Tile.CanvasOnPaintBackground()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class