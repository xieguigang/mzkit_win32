Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Public Class XICFeatureViewer

    Dim XIC As ChromatogramTick()
    Dim features As PeakMs2()
    Dim time_range As DoubleRange
    Dim intomax As Double
    Dim mouse_cur As Point

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
        Using g = canvasXIC.CreateGraphics
            Call g.FillRectangle(New SolidBrush(BackColor), New RectangleF(0, 0, Width, Height))

            If Not XIC.IsNullOrEmpty Then
                Call RtRangeSelector.DrawTIC(g, XIC, time_range, intomax, FillColor, canvasXIC)
            End If

            If mouse_cur.X > 0 AndAlso mouse_cur.X < canvasXIC.Width Then
                Call g.FillRectangle(Brushes.Red, New RectangleF(mouse_cur.X, 0, 3, canvasXIC.Height))
            End If
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
        mouse_cur = canvasXIC.PointToClient(e.Location)
    End Sub
End Class
