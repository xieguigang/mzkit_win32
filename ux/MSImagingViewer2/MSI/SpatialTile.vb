Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports CommonDialogs
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports STImaging

Public Class SpatialTile

    Dim spatialMatrix As Grid(Of SpaceSpot)
    Dim dimensions As Size
    Dim offset As Point
    Dim moveTile As Boolean = False
    Dim p As Point

    Public Property DrawOffset As Integer = 25

    'Private Const WS_EX_TRANSPARENT As Integer = &H20

    'Private m_opacity As Integer = 50

    '<DefaultValue(50)>
    'Public Property Opacity() As Integer
    '    Get
    '        Return Me.m_opacity
    '    End Get
    '    Set
    '        If Value < 0 OrElse Value > 100 Then
    '            Throw New ArgumentException("value must be between 0 and 100")
    '        End If

    '        Me.m_opacity = Value
    '    End Set
    'End Property

    'Protected Overrides ReadOnly Property CreateParams() As CreateParams
    '    Get
    '        Dim cpar As CreateParams = MyBase.CreateParams
    '        cpar.ExStyle = cpar.ExStyle Or WS_EX_TRANSPARENT
    '        Return cpar
    '    End Get
    'End Property

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        ' SetStyle(ControlStyles.Opaque, True)
    End Sub

    'Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
    '    Using brush = New SolidBrush(Color.FromArgb(Me.Opacity * 255 / 100, Me.BackColor))
    '        e.Graphics.FillRectangle(brush, Me.ClientRectangle)
    '    End Using

    '    MyBase.OnPaintBackground(e)
    'End Sub

    Public Sub ShowMatrix(matrix As IEnumerable(Of SpaceSpot))
        Dim spatialMatrix = matrix.ToArray
        Dim polygon As New Polygon2D(spatialMatrix.Select(Function(t) New Point(t.px, t.py)))

        Me.dimensions = New Size(polygon.xpoints.Max, polygon.ypoints.Max)
        Me.offset = New Point(polygon.xpoints.Min, polygon.ypoints.Min)

        spatialMatrix = spatialMatrix _
            .Select(Function(p)
                        Return New SpaceSpot With {
                            .px = p.px - offset.X,
                            .py = p.py - offset.Y,
                            .flag = p.flag,
                            .barcode = p.barcode,
                            .x = p.x,
                            .y = p.y
                        }
                    End Function) _
            .ToArray

        polygon = New Polygon2D(spatialMatrix.Select(Function(t) New Point(t.px, t.py)))

        Me.dimensions = New Size(polygon.xpoints.Max, polygon.ypoints.Max)

        spatialMatrix = spatialMatrix _
            .Select(Function(p)
                        Return New SpaceSpot With {
                            .px = dimensions.Width - p.px,
                            .py = dimensions.Height - p.py,
                            .flag = p.flag,
                            .barcode = p.barcode,
                            .x = p.x,
                            .y = p.y
                        }
                    End Function) _
            .ToArray

        Me.spatialMatrix = Grid(Of SpaceSpot).Create(
            data:=spatialMatrix,
            getX:=Function(spot) spot.px,
            getY:=Function(spot) spot.py
        )
    End Sub


    Private Sub SpatialTile_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        Me.SuspendLayout()
        moveTile = True
        p = Cursor.Position
    End Sub

    Private Sub SpatialTile_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        moveTile = False
        Me.ResumeLayout()
        Me.CanvasOnPaintBackground()
    End Sub

    Public Event GetSpatialMetabolismPoint(smXY As Point, ByRef x As Integer, ByRef y As Integer)
    Public Event ClickSpatialMetabolismPixel(smXY As Point, ByRef x As Integer, ByRef y As Integer)

    Private Sub SpatialTile_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
        Dim smXY As New Point(Left + e.X, Top + e.Y)
        Dim smX, smY As Integer

        RaiseEvent ClickSpatialMetabolismPixel(smXY, smX, smY)
    End Sub

    Private Sub SpatialTile_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If moveTile Then
            Me.Location = New Point(Me.Left + Cursor.Position.X - p.X, Me.Top + Cursor.Position.Y - p.Y)
            p = Cursor.Position
            ' Me.Invalidate()
            ' Call PictureBox2.Refresh()
        Else
            ' show tooltip information
            Dim x As Integer, y As Integer
            Dim smX As Integer
            Dim smY As Integer
            Dim smXY As New Point(Left + e.X, Top + e.Y)
            Dim barcode As String
            Dim spot As SpaceSpot

            RaiseEvent GetSpatialMetabolismPoint(smXY, smX, smY)

            Call PixelSelector.getPoint(New Point(e.X, e.Y), dimensions, Me.Size, x, y)

            spot = spatialMatrix.GetData(x, y)

            If spot Is Nothing Then
                barcode = "<missing_barcode>"
            Else
                barcode = spot.barcode
            End If

            Call ToolTip1.SetToolTip(Me, $"[STdata spot: ({x + offset.X},{y + offset.Y}) {barcode}] -> [MALDI pixel: ({smX},{smY})]")
        End If
    End Sub

    Dim allowResize As Boolean = False

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles AnchorResize.MouseUp
        Me.ResumeLayout()
        allowResize = False
        Me.CanvasOnPaintBackground()
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles AnchorResize.MouseMove
        If allowResize Then
            Me.Size = New Size(AnchorResize.Left + e.X, AnchorResize.Top + e.Y)
            ' Me.Invalidate()
            ' Call PictureBox2.Refresh()
        End If
    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles AnchorResize.MouseDown
        Me.SuspendLayout()
        allowResize = True
    End Sub

    Private Sub ExportSpatialMappingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportSpatialMappingToolStripMenuItem.Click
        Using file As New SaveFileDialog With {.Filter = "Spatial Mapping Matrix(*.xml)|*.xml", .FileName = $"{Label1.Text}.xml"}
            If file.ShowDialog = DialogResult.OK Then
                Call New SpatialMapping With {
                    .spots = GetMapping.ToArray,
                    .label = Label1.Text
                } _
                .GetXml _
                .SaveTo(file.FileName)
            End If
        End Using
    End Sub

    Private Iterator Function GetMapping() As IEnumerable(Of SpotMap)
        Dim radiusX = Me.Width / dimensions.Width / 2
        Dim radiusY = Me.Height / dimensions.Height / 2
        Dim left = Me.Left
        Dim top = Me.Top

        For Each spot As SpaceSpot In spatialMatrix.EnumerateData
            ' translate to control client XY
            Dim clientXY As New Point With {.X = spot.px * radiusX * 2, .Y = spot.py * radiusY * 2}
            Dim pixels As New List(Of Point)

            For x As Integer = clientXY.X - radiusX To clientXY.X + radiusX
                For y As Integer = clientXY.Y - radiusY To clientXY.Y + radiusY
                    Dim SMXY As New Point(x + left, y + top)
                    Dim smX, smY As Integer

                    RaiseEvent GetSpatialMetabolismPoint(SMXY, smX, smY)

                    Call pixels.Add(New Point(smX, smY))
                Next
            Next

            pixels = pixels _
                .Distinct _
                .AsList

            Yield New SpotMap With {
                .STX = spot.px + offset.X,
                .STY = spot.py + offset.Y,
                .SMX = pixels.Select(Function(p) p.X).ToArray,
                .SMY = pixels.Select(Function(p) p.Y).ToArray,
                .barcode = spot.barcode,
                .flag = spot.flag,
                .physicalXY = {spot.x, spot.y}
            }
        Next
    End Function

    Dim imageLoad As Image = Nothing

    Private Sub LoadTissueImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadTissueImageToolStripMenuItem.Click
        Using file As New OpenFileDialog With {.Filter = "Raster Image(*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp"}
            If file.ShowDialog = DialogResult.OK Then
                Me.BackgroundImage = file.FileName.LoadImage
                Me.Refresh()

                imageLoad = file.FileName.LoadImage
            End If
        End Using
    End Sub

    Private Sub EditLabelToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditLabelToolStripMenuItem.Click, Label1.Click
        Dim input As New InputLabelText With {.Label = Label1.Text}

        Call InputDialog.Input(
            Sub(config)
                Label1.Text = config.Label
            End Sub,, config:=input)
    End Sub

    Public Property SpotColor As Color = Color.Red

    Private Sub onDrawSpots(g As Graphics2D, alpha As Integer)
        Dim d As New SizeF(g.Width / dimensions.Width, g.Height / dimensions.Height)
        Dim r As New SizeF(d.Width / 2, d.Height / 2)
        Dim black As New SolidBrush(Color.Black.Alpha(alpha))
        Dim red As New SolidBrush(SpotColor.Alpha(alpha))

        ' draw spatial matrix
        For Each spot As SpaceSpot In spatialMatrix.EnumerateData
            Dim x = spot.px * d.Width
            Dim y = spot.py * d.Height

            If spot.flag = 0 Then
                Call g.FillEllipse(black, New RectangleF(New PointF(x, y), d))
            Else
                Call g.FillEllipse(red, New RectangleF(New PointF(x, y), d))
            End If
        Next
    End Sub

    ''' <summary>
    ''' make this spatial tile transparent
    ''' </summary>
    Private Sub CanvasOnPaintBackground()
        Dim g As Graphics2D

        If imageLoad IsNot Nothing Then
            g = New Graphics2D(New Bitmap(imageLoad))
        Else
            g = Me.Size.CreateGDIDevice
        End If

        If Me.Parent IsNot Nothing AndAlso imageLoad Is Nothing Then
            Dim index = Me.Parent.Controls.GetChildIndex(Me) - 1

            Me.Visible = False

            For i As Integer = Me.Parent.Controls.Count - 1 To index Step -1
                Dim c As Control

                If i < 0 Then
                    c = Me.Parent
                Else
                    c = Me.Parent.Controls(i)
                End If

                If c Is Me Then
                    Continue For
                End If

                If c.Bounds.IntersectsWith(Bounds) AndAlso c.Visible Then
                    Dim clientRect As Rectangle = c.ClientRectangle
                    ' clientRect = New Rectangle(clientRect.X, clientRect.Y - DrawOffset, clientRect.Width, clientRect.Height)
                    Using bmp = New Bitmap(c.Width, c.Height, g.Graphics)
                        c.DrawToBitmap(bmp, clientRect)
                        g.TranslateTransform(c.Left - Left, c.Top - Top - DrawOffset)
                        bmp.AdjustContrast(-30)
                        g.DrawImageUnscaled(bmp, Point.Empty)
                        g.TranslateTransform(Left - c.Left, Top - c.Top - DrawOffset)
                    End Using
                End If
            Next

            Dim size As Size = Me.Size
            size = New Size(size.Width - 4, size.Height - 4)

            g.ResetTransform()
            g.DrawRectangle(New Pen(Brushes.White, 2) With {.DashStyle = DashStyle.Dash}, New Rectangle(New Point(2, 2), size))

            onDrawSpots(g, 150)
        Else
            onDrawSpots(g, 60)
        End If

        g.Flush()

        Me.BackgroundImage = g.ImageResource
        Me.Visible = True

        g.Dispose()
    End Sub

    Private Sub SpatialTile_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' PictureBox2.onDraw = AddressOf CanvasOnPaintBackground
        ' PictureBox2.Refresh()
        Call CanvasOnPaintBackground()
    End Sub

    Private Sub RemoveTissueImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveTissueImageToolStripMenuItem.Click
        imageLoad = Nothing
        Me.CanvasOnPaintBackground()
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        If Not Parent Is Nothing Then
            Call Parent.Controls.Remove(Me)
        End If
    End Sub

    Private Sub SetSpotColorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetSpotColorToolStripMenuItem.Click
        Call InputDialog.Input(Of InputSpotColor)(
            Sub(cnfig)
                Me.SpotColor = cnfig.SpotColor
                Me.CanvasOnPaintBackground()
            End Sub)
    End Sub

    Private Sub SpatialTile_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Delete Then
            If Not Parent Is Nothing Then
                If MessageBox.Show($"Going to remove current spatial tile({Label1.Text})?",
                                   $"Spatial Tile Mapping Tool",
                                   buttons:=MessageBoxButtons.OKCancel,
                                   icon:=MessageBoxIcon.Question
                    ) = DialogResult.OK Then

                    Call Parent.Controls.Remove(Me)
                End If
            End If
        End If
    End Sub

    Private Sub RotateToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RotateToolStripMenuItem.Click

    End Sub
End Class
