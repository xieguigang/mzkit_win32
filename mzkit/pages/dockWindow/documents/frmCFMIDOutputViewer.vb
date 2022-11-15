Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MSP
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.mzkit_win32.My

Public Class frmCFMIDOutputViewer

    Dim msp As New Dictionary(Of String, MspData)

    Public Sub SetCFMIDoutput(data As IEnumerable(Of MspData))
        For Each matrix As MspData In data
            Dim name As String = matrix.Name
            Dim id As String = matrix.DB_id
            Dim energy As String = matrix.Comments.Get("Comment")

            Call msp.Add(energy, matrix)

            Dim row = TreeListView1.Items.Add(energy)

            row.Tag = matrix
            row.SubItems.Add(id)
            row.SubItems.Add(name)
            row.SubItems.Add(matrix.Peaks.OrderByDescending(Function(i) i.intensity).Take(3).Select(Function(mzi) mzi.mz.ToString("F3")).JoinBy("; "))
        Next
    End Sub

    ''' <summary>
    ''' search spectrum
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        Dim cluster As TreeListViewItem

        If TreeListView1.SelectedItems.Count = 0 Then
            Return
        Else
            cluster = TreeListView1.SelectedItems(0)
        End If

        Dim searchPage As New frmSpectrumSearch
        Dim msp As MspData = cluster.Tag

        searchPage.Show(MyApplication.host.dockPanel)
        searchPage.page.loadMs2(msp.Peaks)
        searchPage.page.runSearch()
    End Sub

    ''' <summary>
    ''' view spectrum
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click
        Dim cluster As TreeListViewItem

        If TreeListView1.SelectedItems.Count = 0 Then
            Return
        Else
            cluster = TreeListView1.SelectedItems(0)
        End If

        Dim msp As MspData = cluster.Tag
        Dim ms As New LibraryMatrix With {
            .centroid = True,
            .ms2 = msp.Peaks,
            .name = msp.Name & "@" & msp.Comments("Comment")
        }

        Call MyApplication.host.mzkitTool.showMatrix(msp.Peaks, ms.name)
        Call MyApplication.host.mzkitTool.PlotSpectrum(MS, focusOn:=True)
    End Sub

    Private Sub frmCFMIDOutputViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = Me.Text

        ApplyVsTheme(ContextMenuStrip1)
    End Sub
End Class