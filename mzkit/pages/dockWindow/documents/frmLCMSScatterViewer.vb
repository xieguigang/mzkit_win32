Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Linq
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

        Call ProgressSpinner.DoLoading(Sub() Call Invoke(Sub() Call loadRaw()))
        Call Workbench.AppHost.SetTitle($"LCMS Scatter '{raw.source.FileName}'")
    End Sub

    Private Sub loadRaw()
        Dim meta As List(Of Meta) = raw.GetMs2Scans _
            .Where(Function(a) Not a.mz.IsNullOrEmpty) _
            .Select(Function(a) a.GetScanMeta) _
            .AsList
        Dim ms1 = raw.GetMs1Scans _
            .Select(Function(s) s.GetMs1Scans(Of Meta)) _
            .IteratesALL _
            .ToArray
        Dim maxinto As Double = ms1.Select(Function(a) a.intensity).Max
        Dim cutoff As Double = maxinto * 0.01

        Call meta.AddRange(ms1.Where(Function(a) a.intensity > cutoff))
        Call Me.ScatterViewer.LoadPeaks(meta)
    End Sub

    Private Sub frmLCMSScatterViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        ScatterViewer = New PeakScatterViewer With {
            .ColorScale = ScalerPalette.turbo,
            .Dock = DockStyle.Fill
        }
        Me.Controls.Add(ScatterViewer)

        Call ApplyVsTheme(ScatterViewer.GetMenu)
    End Sub

    Private Sub ScatterViewer_MoveOverPeak(peakId As String, mz As Double, rt As Double) Handles ScatterViewer.MoveOverPeak

    End Sub

    ''' <summary>
    ''' View ms2 peaks data
    ''' </summary>
    ''' <param name="scanId"></param>
    ''' <param name="mz"></param>
    ''' <param name="rt"></param>
    Private Sub ScatterViewer_ClickOnPeak(scanId As String, mz As Double, rt As Double) Handles ScatterViewer.ClickOnPeak
        Call MyApplication.host.mzkitTool.showSpectrum(scanId, raw)
    End Sub
End Class