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
                loadTree()
            End If
        End Using
    End Sub

    Private Sub loadTree()
        Dim tree = Win7StyleTreeView1.Nodes.Add(mzpack.ToString)
        Dim root = mzpack.superBlock

        Call loadTree(tree, root)
    End Sub

    Private Sub loadTree(tree As TreeNode, dir As StreamGroup)
        For Each item As StreamObject In dir.files
            Dim current_dir = tree.Nodes.Add(item.fileName)

            If TypeOf item Is StreamGroup Then
                Call loadTree(current_dir, item)
            End If
        Next
    End Sub
End Class
