Public Class InputBasePeakIon

    Public ReadOnly Property IonMz As Double
        Get
            Return Val(TextBox1.Text)
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text.StringEmpty Then
            MessageBox.Show("No mz value was input!", "Input ion m/z", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        ElseIf Not TextBox1.Text.IsNumeric(includesNaNFactor:=False) Then
            MessageBox.Show($"Input text '{TextBox1.Text}' must be a number!", "Input ion m/z", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        DialogResult = DialogResult.OK
    End Sub
End Class