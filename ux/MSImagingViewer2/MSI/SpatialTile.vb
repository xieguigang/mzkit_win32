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
    Dim moveTile As Boolean = False
    Dim p As Point

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

    Private Sub SpatialTile_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        moveTile = True
        p = Cursor.Position
    End Sub

    Private Sub SpatialTile_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        moveTile = False
    End Sub

    Private Sub SpatialTile_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If moveTile Then
            Me.Location = New Point(Me.Left + Cursor.Position.X - p.X, Me.Top + Cursor.Position.Y - p.Y)
            p = Cursor.Position
        End If
    End Sub

    Dim allowResize As Boolean = False

    Private Sub SpatialTile_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        allowResize = False
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If allowResize Then
            Me.Size = New Size(PictureBox1.Left + e.X, PictureBox1.Top + e.Y)
        End If
    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown
        allowResize = True
    End Sub
End Class
