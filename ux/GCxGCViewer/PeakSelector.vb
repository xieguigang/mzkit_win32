Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports CommonDialogs
Imports ControlLibrary
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports std = System.Math

Public Class PeakSelector

    Public ReadOnly Property TIC2D As D2Chromatogram()
    Public ReadOnly Property ColorSet As ScalerPalette

    Dim scaleX As d3js.scale.LinearScale
    Dim scaleY As d3js.scale.LinearScale

    ''' <summary>
    ''' scan time 1
    ''' </summary>
    Dim t1 As DoubleRange
    ''' <summary>
    ''' scan time 2
    ''' </summary>
    Dim t2 As DoubleRange

    Dim t1Bins As BlockSearchFunction(Of D2Chromatogram)

    Dim WithEvents colors As New ColorScaler

    Public Property HtmlView As Boolean
        Get
            Try
                Return WebView21.Visible
            Catch ex As Exception
                Return False
            End Try
        End Get
        Set(value As Boolean)
            If Not value Then
                WebView21.Hide()
                WebView21.Visible = False
                WebView21.Dock = DockStyle.None
                PictureBox1.Visible = True
                PictureBox1.Show()
                PictureBox1.Dock = DockStyle.Fill

                Call PictureBox1_Resize()
            Else
                PictureBox1.Visible = False
                PictureBox1.Hide()
                PictureBox1.Dock = DockStyle.None
                WebView21.Show()
                WebView21.Visible = True
                WebView21.Dock = DockStyle.Fill

                ToolStripStatusLabel2.Text = "Use mouse left button to rotate, and mouse right button to pan the canvas"
            End If
        End Set
    End Property


    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        SplitContainer1.Panel2.Controls.Add(colors)
        colors.Dock = DockStyle.Fill
    End Sub

    Private Sub rescale()
        Dim xTicks As Vector = TIC2D.Select(Function(t) t.scan_time).CreateAxisTicks.AsVector
        Dim yTicks As Vector = TIC2D.Select(Function(t) t.chromatogram) _
            .IteratesALL _
            .Select(Function(ti) ti.Time) _
            .CreateAxisTicks _
            .AsVector

        scaleX = d3js.scale.linear.domain(xTicks).range(0, PictureBox1.Width)
        scaleY = d3js.scale.linear.domain(yTicks).range(0, PictureBox1.Height)
    End Sub

    Public Sub SetScans(scans As D2Chromatogram(), modtime As Double)
        _TIC2D = scans
        t1 = New DoubleRange(0, Aggregate s As D2Chromatogram In scans Into Max(s.scan_time))
        t2 = New DoubleRange(0, modtime)
        t1Bins = New BlockSearchFunction(Of D2Chromatogram)(scans, Function(i) i.scan_time, 60, fuzzy:=True)

        Call rescale()
        Call rendering()
    End Sub

    Private Sub rendering()
        Dim scaler As New DataScaler(scaleX, scaleY) With {
            .region = New Rectangle(New Point, PictureBox1.Size)
        }
        Dim scaled = GCxGCTIC2DPlot.CutSignal(TIC2D, qh:=0.995).ToArray

        colors.ScalerPalette = ColorSet
        colors.SetIntensityMax(scaled.Select(Function(d) d.intensity).Max)
        colors.ResetScaleRange()
        PictureBox1.BackgroundImage = GCxGCTIC2DPlot.FillHeatMap(scaled, PictureBox1.Size, scaler, ColorSet.Description, 255, 2, 2)

        If HtmlView Then
            Try
                ' ignores of the intiialize error
                If Not WebView21.CoreWebView2 Is Nothing Then
                    Call WebView21.CoreWebView2.Reload()
                End If
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub PictureBox1_Resize() Handles PictureBox1.SizeChanged
        If Not TIC2D.IsNullOrEmpty Then
            Call rescale()

            If PictureBox1.BackgroundImage Is Nothing Then
                Call rendering()
            End If
        End If
    End Sub

    Private Sub ChangeColorsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ChangeColorsToolStripMenuItem.Click
        Dim old = ColorSet

        InputDialog.Input(Of InputSelectColorMap)(
            Sub(colors)
                _ColorSet = colors.GetColorMap
            End Sub)

        If old <> ColorSet AndAlso Not TIC2D.IsNullOrEmpty Then
            Call ProgressSpinner.DoLoading(
                Sub()
                    Call rendering()
                End Sub)
        End If
    End Sub

    Dim p As Point

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        Dim peak As D2Chromatogram = Nothing
        Dim t1, t2 As Double
        Dim text As String

        p = Cursor.Position
        PictureBox1.Refresh()
        GetPeak(peak, t1, t2, loc:=p)

        text = $"scan time1: {StringFormats.ReadableElapsedTime(TimeSpan.FromSeconds(t1))}, scan time2: {t2.ToString("F2")}s"

        If Not peak Is Nothing Then
            text = text & "; " & peak.scan_id
        End If

        ToolStripStatusLabel2.Text = text
    End Sub

    Private Sub GetPeak(ByRef peak As D2Chromatogram, ByRef rt1 As Double, ByRef rt2 As Double, loc As Point)
        Dim pt As Point = PictureBox1.PointToClient(loc)
        Dim size As Size = PictureBox1.Size
        Dim y As New DoubleRange(0, size.Height)
        Dim x As New DoubleRange(0, size.Width)

        If t1 IsNot Nothing AndAlso t2 IsNot Nothing Then
            rt1 = x.ScaleMapping(pt.X, t1)
            rt2 = t2.Max - y.ScaleMapping(pt.Y, t2)

            Dim scantime1 As Double = rt1
            Dim t1scans = t1Bins.Search(New D2Chromatogram(rt1)) _
                .OrderBy(Function(s) std.Abs(s.scan_time - scantime1)) _
                .FirstOrDefault

            peak = t1scans
        End If
    End Sub

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown

    End Sub

    Private Sub PictureBox1_MouseUp(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseUp

    End Sub

    Private Sub PictureBox1_MouseClick(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseClick

    End Sub

    Private Sub PictureBox1_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox1.Paint
        Dim pt As Point = PictureBox1.PointToClient(p)
        Dim pen As New Pen(Color.Red, 2) With {
            .DashStyle = DashStyle.Dot
        }
        Dim size = PictureBox1.Size

        Call e.Graphics.DrawLine(pen, pt.X, 0, pt.X, size.Height)
        Call e.Graphics.DrawLine(pen, 0, pt.Y, size.Width, pt.Y)
    End Sub

    Private Sub PeakSelector_Load(sender As Object, e As EventArgs) Handles Me.Load
        WebKit.Init(WebView21)
        HtmlView = False
    End Sub

    Private Sub DViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DViewerToolStripMenuItem.Click
        HtmlView = DViewerToolStripMenuItem.Checked
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        'Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", lcms_scatter)
        'Call WebView21.CoreWebView2.Navigate($"http://127.0.0.1:{Workbench.WebPort}/LCMS-scatter.html")
        'Call WebKit.DeveloperOptions(WebView21, enable:=True,)
    End Sub
End Class
