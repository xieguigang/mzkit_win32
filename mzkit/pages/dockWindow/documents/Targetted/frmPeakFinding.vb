Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Mzkit_win32.LCMSViewer

Public Class frmPeakFinding

    Dim WithEvents peakFinding As New PeakFindingViewer

    Public Sub LoadMatrix(title As String,
                          data As IEnumerable(Of ChromatogramTick),
                          Optional args As PeakFindingParameter = Nothing)

        Call peakFinding.LoadMatrix(title, data, args)
    End Sub

    Private Sub frmPeakFinding_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(peakFinding)
        peakFinding.Dock = DockStyle.Fill

        Call ApplyVsTheme(
            peakFinding.ContextMenuStrip1,
            peakFinding.ContextMenuStrip2,
            peakFinding.ToolStrip1
        )
    End Sub
End Class