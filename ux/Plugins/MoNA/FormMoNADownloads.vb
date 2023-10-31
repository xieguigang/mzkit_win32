Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Mzkit_win32.BasicMDIForm

Public Class FormMoNADownloads

    Private Sub FormMoNADownloads_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "MassBank of North America"
        TabText = Text

        Call ProgressSpinner.DoLoading(
            Sub()
                Call Me.Invoke(Sub() loadMoNA())
            End Sub
        )
        Call ApplyVsTheme(ContextMenuStrip1)
    End Sub

    Private Sub loadMoNA()
        Dim libs = Library.LoadLibraries

        For Each libdata In libs
            Dim libitem As ListViewItem = ListView1.Items.Add(libdata.label)

            libitem.Tag = libdata.mspExport

            Call libitem.SubItems.Add(libdata.description)
            Call libitem.SubItems.Add(libdata.queryCount)
            Call libitem.SubItems.Add(StringFormats.Lanudry(CDbl(libdata.mspExport.size)))
            Call System.Windows.Forms.Application.DoEvents()
        Next
    End Sub

    Private Sub InstallToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InstallToolStripMenuItem.Click
        If ListView1.SelectedItems.Count = 0 Then
            Return
        End If

        Dim targetLib As Export = ListView1.SelectedItems.Item(0).Tag
        Dim tmp_zip As String = TempFileSystem.GetAppSysTempFile(, "/.cache/mona_download/", "").ParentPath & $"/{targetLib.id}.zip"
        Dim msp_file As String = $"{tmp_zip.ParentPath}/{targetLib.id}.msp"
        Dim file_url As String = $"https://mona.fiehnlab.ucdavis.edu/rest/downloads/retrieve/{targetLib.id}"

        Call Workbench.LogText($"from_url: {file_url}")
        Call Workbench.LogText($"temp_file: {tmp_zip}")
        Call Workbench.LogText(targetLib.GetJson)
        Call TaskProgress.LoadData(
            streamLoad:=AddressOf New Install With {
                .file_url = file_url,
                .msp_file = msp_file,
                .tmp_zip = tmp_zip,
                .targetLib = targetLib
            }.InstallMoNA,
            title:=$"Download & Install MoNA",
            info:=$"[HTTP/GET] {targetLib.exportFile} {StringFormats.Lanudry(CDbl(targetLib.size))}...",
            canbeCancel:=True,
            host:=Me
        )

        Call MessageBox.Show("Database installation task complete!", "Install MoNA distribution", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub
End Class