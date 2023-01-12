Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem

Public Class FormMain

    Dim mzpack As StreamPack

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        App.Exit(0)
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Using file As New OpenFileDialog With {.Filter = "mzPack Data File(*.mzPack)"}
            If file.ShowDialog = DialogResult.OK Then
                mzpack = New StreamPack(file.FileName, [readonly]:=True)
            End If
        End Using
    End Sub
End Class
