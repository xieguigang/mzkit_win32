Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Math

Public Class InputRotateMatrix

    Public Property Tile As SpatialTile

    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged
        Dim angle As Double = NumericUpDown1.Value
        Dim matrix = Tile.rotationRaw.ToArray
        Dim r As Double = angle.ToRadians
        Dim polygon As New Polygon2D(matrix)
        Dim center As New PointF With {
            .X = polygon.xpoints.Average,
            .Y = polygon.ypoints.Average
        }

        Call updateCanvas(matrix.Rotate(center, alpha:=r))
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Call updateCanvas(Tile.rotationRaw)
    End Sub

    Private Sub updateCanvas(matrix As PointF())
        Dim spots = Tile.rotationMatrix

        For i As Integer = 0 To matrix.Length - 1
            spots(i).px = matrix(i).X
            spots(i).py = matrix(i).Y
        Next

        ' update plot
        Call Tile.CanvasOnPaintBackground()
    End Sub
End Class