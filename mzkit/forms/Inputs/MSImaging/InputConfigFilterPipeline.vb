Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler

Public Class InputConfigFilterPipeline

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DialogResult = DialogResult.OK
    End Sub

    Public Sub ConfigPipeline(filters As Scaler(), flags As Boolean())
        CheckedListBox1.Items.Clear()

        For i As Integer = 0 To filters.Length - 1
            CheckedListBox1.Items.Add(filters(i), flags(i))
        Next
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        Dim i As Integer = CheckedListBox1.SelectedIndex

        If i > -1 Then
            CheckedListBox1.SetItemChecked(i, Not CheckedListBox1.GetItemChecked(i))
        End If
    End Sub
End Class