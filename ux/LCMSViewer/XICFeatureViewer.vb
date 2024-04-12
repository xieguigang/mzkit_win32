Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports std = System.Math

Public Class XICFeatureViewer

    Dim XIC As ChromatogramTick()
    Dim features As PeakMs2()
    Dim time_range As DoubleRange
    Dim intomax As Double
    Dim mouse_cur As Point
    Dim selected_peak As PeakMs2
    Dim highlight As Boolean

    Public Property FillColor As Color = Color.SkyBlue

    Public Sub SetFeatures(source As String(), xic As IEnumerable(Of ChromatogramTick), features As IEnumerable(Of PeakMs2), rt_range As DoubleRange)
        Me.XIC = xic.ToArray
        Me.features = features.ToArray

        If Not Me.XIC.IsNullOrEmpty Then
            Me.time_range = Me.XIC.TimeRange
            Me.intomax = Me.XIC.IntensityArray.Max

            Dim hipeak = Me.XIC.Shadows _
                .PopulateROI(peakwidth:=New DoubleRange(3, 20)) _
                .OrderByDescending(Function(r) r.maxInto) _
                .FirstOrDefault

            If Not hipeak Is Nothing Then
                source = source _
                    .JoinIterates({
                                  "RT: " & hipeak.rt & "sec",
                                 $"RT(min): {(hipeak.rt / 60).ToString("F1")}min"}) _
                    .ToArray
            End If
        End If
        If Not rt_range Is Nothing Then
            Me.time_range = New DoubleRange(rt_range)
        End If

        TextBox1.Text = source.JoinBy(vbCrLf)

        Call RenderViewer()

        If Not Me.features.IsNullOrEmpty Then
            Call RenderSpectrum(Me.features(0))
        End If
    End Sub

    Private Sub RenderViewer()
        Dim size = canvasXIC.Size
        Dim width As New DoubleRange(0, size.Width)

        Using g = size.CreateGDIDevice
            Call g.FillRectangle(New SolidBrush(BackColor), New RectangleF(0, 0, size.Width, size.Height))

            If Not XIC.IsNullOrEmpty Then
                Call RtRangeSelector.DrawTIC(g.Graphics, XIC, time_range, intomax, FillColor, canvasXIC)
            End If

            For Each peak As PeakMs2 In features
                Call g.FillRectangle(Brushes.Black, New RectangleF(time_range.ScaleMapping(peak.rt, width), size.Height - 5, 2, 5))
            Next

            If mouse_cur.X > 0 AndAlso mouse_cur.X < size.Width Then
                Call g.FillRectangle(Brushes.Red, New RectangleF(mouse_cur.X, 0, 2, size.Height))
            End If

            canvasXIC.BackgroundImage = g.ImageResource
        End Using
    End Sub

    Private Sub XICFeatureViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Timer1.Interval = 100
        Timer1.Enabled = True
        Timer1.Start()

        ResizeRedraw = True
        DoubleBuffered = False
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim cur = PointToClient(Cursor.Position)

        If cur.X < 0 OrElse cur.X > Width OrElse cur.Y < 0 OrElse cur.Y > Height Then
            highlight = False
        End If

        Call RenderViewer()

        If highlight Then
            Me.BorderStyle = BorderStyle.FixedSingle
        Else
            Me.BorderStyle = BorderStyle.None
        End If
    End Sub

    Private Sub canvasXIC_MouseMove(sender As Object, e As MouseEventArgs) Handles canvasXIC.MouseMove
        mouse_cur = e.Location
        highlight = True
    End Sub

    Private Sub canvasXIC_MouseHover() Handles canvasXIC.MouseClick
        Dim rt As Double = New DoubleRange(0, Width).ScaleMapping(mouse_cur.X, time_range)
        Dim peak As PeakMs2 = features _
            .Where(Function(i) std.Abs(i.rt - rt) < 60) _
            .OrderBy(Function(i) std.Abs(i.rt - rt)) _
            .FirstOrDefault

        highlight = True
        RenderSpectrum(peak)
    End Sub

    Private Sub RenderSpectrum(peak As PeakMs2)
        If Not peak Is Nothing Then
            Dim scale As Double = 8.5
            Dim msLib As New LibraryMatrix(peak.lib_guid, peak.mzInto)
            Dim size As New Size(PictureBox2.Width * scale, PictureBox2.Height * scale)
            Dim plot As Image = PeakAssign _
                .DrawSpectrumPeaks(msLib, size:=$"{size.Width},{size.Height}", dpi:=200) _
                .AsGDIImage

            selected_peak = peak
            PictureBox2.BackgroundImage = plot
        End If
    End Sub

    Private Sub PictureBox2_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox2.DoubleClick

    End Sub

    Public Event ViewSpectrum(spec As PeakMs2)

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        If Not selected_peak Is Nothing Then
            RaiseEvent ViewSpectrum(selected_peak)
        End If
    End Sub

    Private Sub PictureBox2_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox2.MouseMove
        highlight = True
    End Sub
End Class
