Public Class InputPeakTime

    Public ReadOnly Property TimeField As String

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged

    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        ComboBox1.Enabled = False
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        ComboBox1.Enabled = True
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If RadioButton1.Checked Then
            If ComboBox1.SelectedIndex = -1 Then
                MessageBox.Show("A data field which is represent the time ticks must be selected!", "Select Time", buttons:=MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            Else
                _TimeField = ComboBox1.SelectedItem.ToString
            End If
        Else
            _TimeField = Nothing
        End If

        Me.DialogResult = DialogResult.OK
    End Sub
End Class