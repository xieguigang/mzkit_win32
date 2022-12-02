Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports CommonDialogs
Imports Microsoft.VisualBasic.ApplicationServices.Development.XmlDoc.Serialization
Imports Microsoft.VisualBasic.Data.GraphTheory
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports STImaging

Public Class SpatialTile

    Dim spatialMatrix As Grid(Of SpaceSpot)
    Dim dimensions As Size
    Dim offset As Point
    Dim moveTile As Boolean = False
    Dim p As Point

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
        Me.spatialMatrix = Grid(Of SpaceSpot).Create(spatialMatrix, Function(spot) spot.px, Function(spot) spot.py)
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

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        Me.ResumeLayout()
        allowResize = False
        Me.CanvasOnPaintBackground()
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If allowResize Then
            Me.Size = New Size(PictureBox1.Left + e.X, PictureBox1.Top + e.Y)
            ' Me.Invalidate()
            ' Call PictureBox2.Refresh()
        End If
    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown
        Me.SuspendLayout()
        allowResize = True
    End Sub

    Private Sub ExportSpatialMappingToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportSpatialMappingToolStripMenuItem.Click
        Using file As New SaveFileDialog With {.Filter = "Spatial Mapping Matrix(*.xml)|*.xml"}
            If file.ShowDialog = DialogResult.OK Then
                Call New XmlList(Of SpatialMapping) With {
                    .items = GetMapping.ToArray
                } _
                .GetXml _
                .SaveTo(file.FileName)
            End If
        End Using
    End Sub

    Private Iterator Function GetMapping() As IEnumerable(Of SpatialMapping)
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

            Yield New SpatialMapping With {
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

    Dim imageLoad As Boolean = False

    Private Sub LoadTissueImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadTissueImageToolStripMenuItem.Click
        Using file As New OpenFileDialog With {.Filter = "Raster Image(*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp"}
            If file.ShowDialog = DialogResult.OK Then
                Me.BackgroundImage = file.FileName.LoadImage
                Me.Refresh()

                imageLoad = True
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

    ''' <summary>
    ''' make this spatial tile transparent
    ''' </summary>
    Private Sub CanvasOnPaintBackground()
        If Me.Parent IsNot Nothing Then
            Dim index = Me.Parent.Controls.GetChildIndex(Me)
            Dim g As Graphics2D = Me.Size.CreateGDIDevice

            For i As Integer = Me.Parent.Controls.Count - 1 To index Step -1
                Dim c = Me.Parent.Controls(i)

                If c.Bounds.IntersectsWith(Bounds) AndAlso c.Visible Then

                    Using bmp = New Bitmap(c.Width, c.Height, g.Graphics)
                        c.DrawToBitmap(bmp, c.ClientRectangle)
                        g.TranslateTransform(c.Left - Left, c.Top - Top)
                        bmp.AdjustContrast(-10)
                        g.DrawImageUnscaled(bmp, Point.Empty)
                        g.TranslateTransform(Left - c.Left, Top - c.Top)
                    End Using
                End If
            Next

            g.Flush()
            Me.BackgroundImage = g.ImageResource
            g.Dispose()
        End If
    End Sub

    Private Sub SpatialTile_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' PictureBox2.onDraw = AddressOf CanvasOnPaintBackground
        ' PictureBox2.Refresh()
        Call CanvasOnPaintBackground()
    End Sub

    Private Sub RemoveTissueImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveTissueImageToolStripMenuItem.Click
        Me.CanvasOnPaintBackground()
        imageLoad = False
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click

    End Sub
End Class
