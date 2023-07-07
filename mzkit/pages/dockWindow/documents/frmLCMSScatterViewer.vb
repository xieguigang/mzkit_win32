Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Mzkit_win32.BasicMDIForm

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
        ToolStripStatusLabel1.Text = "Ready!"
        ' ApplyVsTheme(StatusStrip1)
    End Sub

    Private Sub ScatterViewer_MoveOverPeak(peakId As String, mz As Double, rt As Double) Handles ScatterViewer.MoveOverPeak
        ToolStripStatusLabel1.Text = $"m/z {mz.ToString("F4")} RT {(rt / 60).ToString("F2")}min; find ion: {peakId}"
    End Sub

    Private Sub ScatterViewer_ClickOnPeak(peakId As String, mz As Double, rt As Double) Handles ScatterViewer.ClickOnPeak
        ToolStripStatusLabel1.Text = $"m/z {mz.ToString("F4")} RT {(rt / 60).ToString("F2")}min; ion: '{peakId}' has been selected!"
    End Sub
End Class