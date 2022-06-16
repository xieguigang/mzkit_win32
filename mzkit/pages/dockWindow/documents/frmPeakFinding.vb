Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram

Public Class frmPeakFinding

    Dim matrix As ChromatogramTick()

    Public Sub LoadMatrix(data As IEnumerable(Of ChromatogramTick))
        Me.matrix = data.ToArray
        Call InitPanel()
    End Sub

    Public Sub InitPanel()

    End Sub

End Class