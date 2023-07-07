Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
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

        Call Workbench.AppHost.SetTitle($"LCMS Scatter '{raw.source.FileName}'")
    End Sub

    Private Sub frmLCMSScatterViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        ToolStripStatusLabel1.Text = "Ready!"
        ApplyVsTheme(StatusStrip1)
    End Sub
End Class