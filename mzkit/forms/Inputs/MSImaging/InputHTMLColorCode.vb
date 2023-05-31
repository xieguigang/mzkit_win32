Imports Microsoft.VisualBasic.Imaging

Public Class InputHTMLColorCode

    Public ReadOnly Property UserInputColor As Color
        Get
            Return TextBox1.Text.Trim.TranslateColor(throwEx:=False)
        End Get
    End Property

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text.StringEmpty Then
            MessageBox.Show("Input a color code!", "No input", MessageBoxButtons.OK, MessageBoxIcon.Hand)
            Return
        End If

        Dim check_success As Boolean = False
        Dim color As Color = TextBox1.Text.Trim.TranslateColor(throwEx:=False, success:=check_success)

        If check_success Then
            Me.DialogResult = DialogResult.OK
        Else
            MessageBox.Show("Not a valid color code value!", "Invalid input", MessageBoxButtons.OK, MessageBoxIcon.Hand)
        End If
    End Sub
End Class