Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.MoleculeNetworking
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

    End Sub
End Class