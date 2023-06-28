Imports Microsoft.VisualBasic.Windows.Forms.DataValidation

Public Class InputMNSimilarityScore

    Public ReadOnly Property GetCutoff As Double
        Get
            Return Val(TextBox1.Text)
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.ValidateDouble("cluster similarity") IsNot Nothing Then
            DialogResult = DialogResult.OK
        End If
    End Sub
End Class