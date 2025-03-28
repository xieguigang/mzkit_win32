Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model

Public Class InputIntensityRange

    Public ReadOnly Property ValueRange As DoubleRange
        Get
            Return New DoubleRange(Val(TextBox1.Text), Val(TextBox2.Text))
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Dim intensityRangeValue As DoubleRange

    Public Function SetRange(intensityRange As DoubleRange, custom As DoubleRange) As InputIntensityRange
        TextBox1.Text = custom.Min
        TextBox2.Text = custom.Max
        intensityRangeValue = intensityRange

        Return Me
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ValueRange.Length <= 0 Then
            MessageBox.Show("The required of the intensity scale range for the heatmap color render should not be ZERO or negative length!",
                            "Invalid Data",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        TextBox1.Text = intensityRangeValue.Min
        TextBox2.Text = intensityRangeValue.Max
    End Sub
End Class