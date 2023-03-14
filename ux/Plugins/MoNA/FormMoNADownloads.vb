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
            Call libitem.SubItems.Add(StringFormats.Lanudry(CDbl(libdata.sdfExport.size)))
            Call Application.DoEvents()
        Next
    End Sub

    Private Sub InstallToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles InstallToolStripMenuItem.Click
        If ListView1.SelectedItems.Count = 0 Then
            Return
        End If

        Dim targetLib As Export = ListView1.SelectedItems.Item(0).Tag

        Call Workbench.LogText(targetLib.GetJson)
        Call TaskProgress.LoadData(
            streamLoad:=Function(a As ITaskProgress) As Boolean
                            Return True
                        End Function,
            title:=$"Download & Install MoNA",
            info:=$"",
            canbeCancel:=True,
            host:=Me
        )
    End Sub
End Class