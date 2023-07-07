Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork

Public Class frmLCMSScatterViewer

    ''' <summary>
    ''' 已经加载的Raw文件
    ''' </summary>
    Dim raw As Raw

    Public Sub loadRaw(raw As Raw)
        Me.raw = raw
        Me.raw.LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache))
        Me.TabText = raw.source.FileName


    End Sub

    Private Sub frmLCMSScatterViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        ApplyVsTheme(StatusStrip1)
    End Sub
End Class