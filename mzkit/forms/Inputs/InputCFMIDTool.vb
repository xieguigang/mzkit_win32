Public Class InputCFMIDTool
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text.StringEmpty Then
            MessageBox.Show("No structure text!", "CFM-ID Tool", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Me.DialogResult = DialogResult.OK
    End Sub
End Class