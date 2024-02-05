Imports System.Drawing.Drawing2D
Imports System.Windows.Forms
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports CommonDialogs
Imports ControlLibrary
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

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

    Dim WithEvents colors As New ColorScaler

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

        Call rescale()
        Call rendering()
    End Sub

    Private Sub rendering()
        Dim scaler As New DataScaler(scaleX, scaleY) With {
            .region = New Rectangle(New Point, PictureBox1.Size)
        }
        Dim scaled = GCxGCTIC2DPlot.CutSignal(TIC2D, qh:=0.99).ToArray

        colors.ScalerPalette = ColorSet
        colors.SetIntensityMax(scaled.Select(Function(d) d.intensity).Max)
        colors.ResetScaleRange()
        PictureBox1.BackgroundImage = GCxGCTIC2DPlot.FillHeatMap(scaled, PictureBox1.Size, scaler, ColorSet.Description, 255, 2, 2)
    End Sub

    Private Sub PictureBox1_Resize(sender As Object, e As EventArgs) Handles PictureBox1.Resize
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

        If old <> ColorSet Then
            Call ProgressSpinner.DoLoading(
                Sub()
                    Call rendering()
                End Sub)
        End If
    End Sub

    Dim p As Point

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        Dim id As String = Nothing
        Dim t1, t2 As Double

        p = Cursor.Position
        PictureBox1.Refresh()
        GetPeak(id, t1, t2, loc:=p)

        ToolStripStatusLabel1.Text = $"GCxGC scan time1: {StringFormats.ReadableElapsedTime(TimeSpan.FromSeconds(t1))}, scan time2: {t2}s"
    End Sub

    Private Sub GetPeak(ByRef peakId As String, ByRef rt1 As Double, ByRef rt2 As Double, loc As Point)
        Dim pt As Point = PictureBox1.PointToClient(loc)
        Dim size As Size = PictureBox1.Size
        Dim y As New DoubleRange(0, size.Height)
        Dim x As New DoubleRange(0, size.Width)

        If t1 IsNot Nothing AndAlso t2 IsNot Nothing Then
            rt1 = x.ScaleMapping(pt.X, t1)
            rt2 = y.ScaleMapping(pt.Y, t2)
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
End Class
