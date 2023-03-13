Public Class FormMoNADownloads

    Private Sub FormMoNADownloads_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim libs = Library.LoadLibraries

        For Each libdata In libs
            Dim libitem = ListView1.Items.Add(libdata.label)
            libitem.SubItems.Add(libdata.description)
        Next
    End Sub
End Class