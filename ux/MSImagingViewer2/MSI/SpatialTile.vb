Imports System.ComponentModel
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports CommonDialogs
Imports Microsoft.VisualBasic.ApplicationServices.Development.XmlDoc.Serialization
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Text.Xml.Models

Public Class SpatialTile

    Dim spatialMatrix As PixelData()
    Dim dimensions As Size
    Dim offset As Point
    Dim moveTile As Boolean = False
    Dim p As Point

    Private Const WS_EX_TRANSPARENT As Integer = &H20

    Private m_opacity As Integer = 50

    <DefaultValue(50)>
    Public Property Opacity() As Integer
        Get
            Return Me.m_opacity
        End Get
        Set
            If Value < 0 OrElse Value > 100 Then
                Throw New ArgumentException("value must be between 0 and 100")
            End If

            Me.m_opacity = Value
        End Set
    End Property

    Protected Overrides ReadOnly Property CreateParams() As CreateParams
        Get
            Dim cpar As CreateParams = MyBase.CreateParams
            cpar.ExStyle = cpar.ExStyle Or WS_EX_TRANSPARENT
            Return cpar
        End Get
    End Property

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        SetStyle(ControlStyles.Opaque, True)
    End Sub

    'Protected Overrides Sub OnPaint(e As PaintEventArgs)
    '    Using brush = New SolidBrush(Color.FromArgb(Me.Opacity * 255 / 100, Me.BackColor))
    '        e.Graphics.FillRectangle(brush, Me.ClientRectangle)
    '    End Using

    '    MyBase.OnPaint(e)
    'End Sub

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
    End Sub


    Private Sub SpatialTile_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        Me.SuspendLayout()
        moveTile = True
        p = Cursor.Position
    End Sub

    Private Sub SpatialTile_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        moveTile = False
        Me.ResumeLayout()
    End Sub

    Public Event GetSpatialMetabolismPoint(smXY As Point, ByRef x As Integer, ByRef y As Integer)

    Private Sub SpatialTile_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If moveTile Then
            Me.Location = New Point(Me.Left + Cursor.Position.X - p.X, Me.Top + Cursor.Position.Y - p.Y)
            p = Cursor.Position
            Me.Invalidate()
        Else
            ' show tooltip information
            Dim x As Integer, y As Integer
            Dim smX As Integer
            Dim smY As Integer
            Dim smXY As New Point(Left + e.X, Top + e.Y)

            RaiseEvent GetSpatialMetabolismPoint(smXY, smX, smY)

            Call PixelSelector.getPoint(New Point(e.X, e.Y), dimensions, Me.Size, x, y)
            Call ToolTip1.SetToolTip(Me, $"[ST: ({x},{y})] ~ [SM: ({smX},{smY})]")
        End If
    End Sub

    Dim allowResize As Boolean = False

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp
        Me.ResumeLayout()
        allowResize = False
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        If allowResize Then
            Me.Size = New Size(PictureBox1.Left + e.X, PictureBox1.Top + e.Y)
            Me.Invalidate()
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

        For Each spot As PixelData In spatialMatrix
            ' translate to control client XY
            Dim clientXY As New Point With {.X = spot.X * radiusX * 2, .Y = spot.Y * radiusY * 2}
            Dim pixels As New List(Of Point)

            For x As Integer = clientXY.X - radiusX To clientXY.X + radiusX
                For y As Integer = clientXY.Y - radiusY To clientXY.Y + radiusY
                    Dim SMXY As New Point(x + left, y + top)
                    Dim smX, smY As Integer

                    RaiseEvent GetSpatialMetabolismPoint(SMXY, smX, smY)

                    Call pixels.Add(New Point(smX, smY))
                Next
            Next

            Yield New SpatialMapping With {
                .STX = spot.X + offset.X,
                .STY = spot.Y + offset.Y,
                .SMX = pixels.Select(Function(p) p.X).ToArray,
                .SMY = pixels.Select(Function(p) p.Y).ToArray
            }
        Next
    End Function

    Private Sub LoadTissueImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadTissueImageToolStripMenuItem.Click
        Using file As New OpenFileDialog With {.Filter = "Raster Image(*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp"}
            If file.ShowDialog = DialogResult.OK Then
                Me.BackgroundImage = file.FileName.LoadImage
                Me.Refresh()
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
    ''' <param name="e"></param>
    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        MyBase.OnPaintBackground(e)

        Dim g = e.Graphics

        If Me.Parent IsNot Nothing Then
            Dim index = Parent.Controls.GetChildIndex(Me)
            For i As Integer = Parent.Controls.Count - 1 To index Step -1
                Dim c = Parent.Controls(i)
                If (c.Bounds.IntersectsWith(Bounds) AndAlso c.Visible) Then

                    Using bmp = New Bitmap(c.Width, c.Height, g)

                        c.DrawToBitmap(bmp, c.ClientRectangle)
                        g.TranslateTransform(c.Left - Left, c.Top - Top)
                        g.DrawImageUnscaled(bmp, Point.Empty)
                        g.TranslateTransform(Left - c.Left, Top - c.Top)
                    End Using
                End If
            Next
        End If
    End Sub
End Class
