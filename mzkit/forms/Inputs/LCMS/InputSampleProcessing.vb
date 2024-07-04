Public Class InputSampleProcessing

    Public ReadOnly Property MissingPercentage As Double
        Get
            Return NumericUpDown1.Value / 100
        End Get
    End Property

    Public ReadOnly Property NormScale As Double
        Get
            Return Val(TextBox1.Text)
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If NormScale <= 0 Then
            MessageBox.Show("The scale factor for the total peak sum normalization result must be a positive value!", "Invalid scale factor", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Me.DialogResult = DialogResult.OK
    End Sub
End Class