Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging

Public Class SpectrumViewer

    Dim spectrum As ms2()
    Dim highlights As Integer
    Dim mzscale As d3js.scale.LinearScale
    Dim intensityscale As d3js.scale.LinearScale

    Dim mz_index As MzPool
    Dim mz_range As DoubleRange
    Dim into_range As DoubleRange

    Public Property title As String = "MS Spectrum"

    Public Sub SetSpectrum(spec As LibraryMatrix)
        spectrum = spec.ms2
        title = If(spec.name, "MS Spectrum")
        highlights = -1
        mz_range = spectrum.Select(Function(m) m.mz).Range
        into_range = spectrum.Select(Function(m) m.intensity).Range
        mz_index = New MzPool(spectrum)

        Call Scaling()
        Call Rendering()
    End Sub

    Private Sub SpectrumViewer_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        Call Scaling()
        Call Rendering()
    End Sub

    Private Sub Scaling()
        Try
            mzscale = d3js.scale.linear().domain(mz_range).range(values:=New Double() {0, PictureBox1.Width})
            intensityscale = d3js.scale.linear().domain(into_range).range(values:=New Double() {0, PictureBox1.Height})
        Catch ex As Exception
            Call App.LogException(ex)
        End Try
    End Sub

    Private Sub Rendering()
        Using g As Graphics2D = PictureBox1.Size.CreateGDIDevice(filled:=BackColor)

        End Using
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        Dim pos = e.Location
    End Sub
End Class
