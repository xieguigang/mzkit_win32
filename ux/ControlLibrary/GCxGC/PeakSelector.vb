Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra

Public Class PeakSelector

    Public ReadOnly Property TIC2D As D2Chromatogram()

    Dim scaleX As d3js.scale.LinearScale
    Dim scaleY As d3js.scale.LinearScale

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

    Public Sub SetScans(scans As D2Chromatogram())
        _TIC2D = scans

        Call rescale()
        Call rendering()
    End Sub

    Private Sub rendering()
        PictureBox1.BackgroundImage = GCxGCTIC2DPlot.FillHeatMap(TIC2D, PictureBox1.Size, New DataScaler(scaleX, scaleY), "jet", 255, 1, 1, Color.Blue)
    End Sub

    Private Sub PictureBox1_Resize(sender As Object, e As EventArgs) Handles PictureBox1.Resize
        If Not TIC2D.IsNullOrEmpty Then
            Call rescale()
            Call rendering()
        End If
    End Sub
End Class
