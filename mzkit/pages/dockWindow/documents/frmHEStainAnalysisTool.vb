Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap

Public Class frmHEStainAnalysisTool

    Public Sub LoadRawData(MSI As PixelScanIntensity(), dims As Size, HEstain As Image)
        Call HeStainViewer1.LoadRawData(MSI.Select(Function(m) New PixelData(m, m.totalIon)), dims, HEstain)
    End Sub

    Private Sub frmHEStainAnalysisTool_Activated(sender As Object, e As EventArgs) Handles Me.Activated

    End Sub

    Private Sub frmHEStainAnalysisTool_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

    End Sub
End Class