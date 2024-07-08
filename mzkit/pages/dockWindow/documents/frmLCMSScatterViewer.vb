Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math
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
    Dim clickPoint As Action(Of String, Double, Double, Boolean)

    Public Sub Hook(click As Action(Of String, Double, Double, Boolean))
        clickPoint = click
    End Sub

    ''' <summary>
    ''' ms1 peaks
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <returns></returns>
    Public Function loadRaw(raw As Raw) As frmLCMSScatterViewer
        Me.raw = raw
        Me.raw.LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache))
        Me.TabText = raw.source.FileName

        Call ProgressSpinner.DoLoading(Sub() Call Invoke(Sub() Call loadRaw(msn:=False)))
        Call Workbench.AppHost.SetTitle($"LCMS Scatter '{raw.source.FileName}'")

        Return Me
    End Function

    ''' <summary>
    ''' msn precursors
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <returns></returns>
    Public Function LoadRawMSn(raw As Raw) As frmLCMSScatterViewer
        Me.raw = raw
        Me.raw.LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache))
        Me.TabText = raw.source.FileName

        Call ProgressSpinner.DoLoading(Sub() Call Invoke(Sub() Call loadRaw(msn:=True)))
        Call Workbench.AppHost.SetTitle($"LCMS MSn Scatter '{raw.source.FileName}'")

        Return Me
    End Function

    Public Function loadRaw(data As Meta()) As frmLCMSScatterViewer
        Call Me.ScatterViewer.LoadPeaks(data)
        Return Me
    End Function

    Public Sub LoadRaw(points As IEnumerable(Of xcms2), samples As String())
        Dim rawdata As New List(Of Meta)
        Dim v As Double

        For Each roi As xcms2 In points.SafeQuery
            v = roi(samples).Sum

            If v > 0 Then
                rawdata.Add(New Meta With {.id = roi.ID, .mz = roi.mz, .scan_time = roi.rt, .intensity = v})
            End If
        Next

        Call Me.ScatterViewer.LoadPeaks(rawdata)
    End Sub

    Private Sub loadRaw(msn As Boolean)
        ' MSn
        Dim meta As List(Of Meta) = raw.GetMs2Scans _
            .Where(Function(a) Not a.mz.IsNullOrEmpty) _
            .Select(Function(a) a.GetScanMeta) _
            .AsList

        If Not msn Then
            Dim ms1 = raw.GetMs1Scans _
               .Select(Function(s) s.GetMs1Scans(Of Meta)) _
               .IteratesALL _
               .ToArray
            Dim maxinto As Double = ms1.Select(Function(a) a.intensity).Max
            Dim cutoff As Double = maxinto * 0.001

            ' and also add ms1 peaks if not msn
            Call meta.AddRange(ms1.Where(Function(a) a.intensity > cutoff))
        End If

        Call Me.ScatterViewer.LoadPeaks(meta)
    End Sub

    Private Sub frmLCMSScatterViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.ScatterViewer = New PeakScatterViewer With {
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
    Private Sub ScatterViewer_ClickOnPeak(scanId As String, mz As Double, rt As Double, progress As Boolean) Handles ScatterViewer.ClickOnPeak
        If Not raw Is Nothing Then
            Call MyApplication.host.mzkitTool.showSpectrum(scanId, raw, Nothing)
        ElseIf Not clickPoint Is Nothing Then
            Call clickPoint(scanId, mz, rt, progress)
        End If
    End Sub
End Class