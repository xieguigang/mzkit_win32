Imports System.Drawing
Imports CommonDialogs
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Imaging.Math2D

Public Class SpatialTile

    Dim spatialMatrix As PixelData()
    Dim colors As ScalerPalette = ScalerPalette.turbo
    Dim radius As Integer = 10
    Dim dimensions As Size
    Dim offset As Point

    Public Sub ShowMatrix(matrix As IEnumerable(Of PixelData))
        Me.spatialMatrix = matrix.ToArray

        Dim polygon As New Polygon2D(Me.spatialMatrix.Select(Function(t) New Point(t.X, t.Y)))

        Me.dimensions = New Size(polygon.xpoints.Max, polygon.ypoints.Max)
        Me.offset = New Point(polygon.xpoints.Min, polygon.ypoints.Min)
        Me.spatialMatrix = Me.spatialMatrix _
            .Select(Function(p)
                        Return New PixelData(p.X - offset.X, p.Y - offset.Y, p.Scale)
                    End Function) _
            .ToArray

        polygon = New Polygon2D(Me.spatialMatrix.Select(Function(t) New Point(t.X, t.Y)))
        dimensions = New Size(polygon.xpoints.Max, polygon.ypoints.Max)

        Call Plot()
    End Sub

    Private Sub AdjustRadius() Handles AdjustRadiusToolStripMenuItem.Click
        Dim input As New InputSpotRadius
        input.TextBox1.Text = radius

        InputDialog.Input(
            Sub(config)
                radius = Val(config.TextBox1.Text)
                Plot()
            End Sub,, config:=input)
    End Sub

    ''' <summary>
    ''' do matrix rendering and then show plot image
    ''' </summary>
    Public Sub Plot()
        Dim colors As SolidBrush() = Designer _
            .GetColors(Me.colors.Description, 24) _
            .Select(Function(c) New SolidBrush(c)) _
            .ToArray
        Dim range As New DoubleRange(spatialMatrix.Select(Function(i) i.Scale))
        Dim index As New DoubleRange(0, colors.Length - 1)

        Using g As Graphics2D = dimensions.CreateGDIDevice(filled:=Color.Transparent)
            For Each spot As PixelData In spatialMatrix
                Call g.DrawCircle(New Point(spot.X, spot.Y), radius, colors(CInt(range.ScaleMapping(spot.Scale, index))))
            Next

            Call g.Flush()

            Me.BackgroundImage = g.ImageResource
        End Using
    End Sub
End Class
