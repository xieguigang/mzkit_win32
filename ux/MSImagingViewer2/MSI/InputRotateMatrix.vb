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
        Tile.offset = Tile.offset_origin
        ' raw is zero based
        ' after minus the origin offset value
        Call updateCanvas(Tile.rotationRaw)
    End Sub

    Private Sub updateCanvas(matrix As PointF())
        Dim spots = Tile.rotationMatrix
        Dim poly As New Polygon2D(matrix)
        Dim left As Double = poly.xpoints.Min
        Dim top As Double = poly.ypoints.Min

        For i As Integer = 0 To matrix.Length - 1
            spots(i).px = matrix(i).X - left
            spots(i).py = matrix(i).Y - top
        Next

        poly = New Polygon2D(spots.Select(Function(s) New PointF(s.px, s.py)).ToArray)

        ' update plot
        Tile.dimensions = New Size(poly.xpoints.Max, poly.ypoints.Max)
        ' offset is always zero after rotation
        Tile.offset = New Point
        Tile.CanvasOnPaintBackground()
        Tile.buildGrid()
    End Sub

    ''' <summary>
    ''' mirror
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        ' to array for break the reference
        Dim matrix = Tile.rotationRaw.ToArray
        Dim dims = Tile.dimensions

        For i As Integer = 0 To matrix.Length - 1
            matrix(i) = New PointF(dims.Width - matrix(i).X, matrix(i).Y)
        Next

        Call updateCanvas(matrix)
    End Sub
End Class