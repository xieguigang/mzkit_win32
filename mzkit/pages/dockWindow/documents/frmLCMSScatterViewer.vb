Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.LCMSViewer

Public Class frmLCMSScatterViewer

    ''' <summary>
    ''' 已经加载的Raw文件
    ''' </summary>
    Dim raw As Raw

    Public Sub loadRaw(raw As Raw)
        Me.raw = raw
        Me.raw.LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache))
        Me.TabText = raw.source.FileName

        Call loadRaw()
        Call Workbench.AppHost.SetTitle($"LCMS Scatter '{raw.source.FileName}'")
    End Sub

    Private Sub loadRaw()
        Dim meta As Meta() = raw.GetMs2Scans.Select(Function(a) a.GetScanMeta).ToArray

        Call Me.ScatterViewer.LoadPeaks(meta)
    End Sub

    Private Sub frmLCMSScatterViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        ScatterViewer = New PeakScatterViewer With {
            .ColorScale = ScalerPalette.viridis,
            .Dock = DockStyle.Fill
        }
        Me.Controls.Add(ScatterViewer)
    End Sub

    Private Sub ScatterViewer_MoveOverPeak(peakId As String, mz As Double, rt As Double) Handles ScatterViewer.MoveOverPeak

    End Sub

    Private Sub ScatterViewer_ClickOnPeak(peakId As String, mz As Double, rt As Double) Handles ScatterViewer.ClickOnPeak

    End Sub
End Class