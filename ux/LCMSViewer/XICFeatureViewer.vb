Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra

Public Class XICFeatureViewer

    Dim XIC As ChromatogramTick()
    Dim features As PeakMs2()

    Public Sub SetFeatures(xic As IEnumerable(Of ChromatogramTick), features As IEnumerable(Of PeakMs2))
        Me.XIC = xic.ToArray
        Me.features = features.ToArray

        Call RenderViewer()
    End Sub

    Private Sub RenderViewer()

    End Sub
End Class
