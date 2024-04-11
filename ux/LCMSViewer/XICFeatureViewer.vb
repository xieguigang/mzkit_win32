Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports std = System.Math

Public Class XICFeatureViewer

    Dim XIC As ChromatogramTick()
    Dim features As PeakMs2()
    Dim time_range As DoubleRange
    Dim intomax As Double
    Dim mouse_cur As Point
    Dim selected_peak As PeakMs2

    Public Property FillColor As Color = Color.SkyBlue

    Public Sub SetFeatures(xic As IEnumerable(Of ChromatogramTick), features As IEnumerable(Of PeakMs2))
        Me.XIC = xic.ToArray
        Me.features = features.ToArray

        If Not Me.XIC.IsNullOrEmpty Then
            Me.time_range = Me.XIC.TimeRange
            Me.intomax = Me.XIC.IntensityArray.Max
        End If

        Call RenderViewer()
    End Sub

    Private Sub RenderViewer()
        Using g = canvasXIC.Size.CreateGDIDevice
            Call g.FillRectangle(New SolidBrush(BackColor), New RectangleF(0, 0, Width, Height))

            If Not XIC.IsNullOrEmpty Then
                Call RtRangeSelector.DrawTIC(g.Graphics, XIC, time_range, intomax, FillColor, canvasXIC)
            End If

            If mouse_cur.X > 0 AndAlso mouse_cur.X < canvasXIC.Width Then
                Call g.FillRectangle(Brushes.Red, New RectangleF(mouse_cur.X, 0, 3, canvasXIC.Height))
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
        Call RenderViewer()
    End Sub

    Private Sub canvasXIC_MouseMove(sender As Object, e As MouseEventArgs) Handles canvasXIC.MouseMove
        mouse_cur = e.Location
    End Sub

    Private Sub canvasXIC_MouseHover(sender As Object, e As EventArgs) Handles canvasXIC.MouseHover
        Dim rt As Double = New DoubleRange(0, Width).ScaleMapping(mouse_cur.X, time_range)
        Dim peak As PeakMs2 = features _
            .Where(Function(i) std.Abs(i.rt - rt) < 15) _
            .OrderBy(Function(i) std.Abs(i.rt - rt)) _
            .FirstOrDefault

        If Not peak Is Nothing Then
            Dim scale As Double = 8
            Dim msLib As New LibraryMatrix(peak.lib_guid, peak.mzInto)
            Dim size As New Size(PictureBox2.Width * scale, PictureBox2.Height * scale)
            Dim plot As Image = PeakAssign _
                .DrawSpectrumPeaks(msLib, size:=$"{size.Width},{size.Height}", dpi:=200) _
                .AsGDIImage

            selected_peak = peak
            PictureBox2.BackgroundImage = plot
        End If
    End Sub
End Class
