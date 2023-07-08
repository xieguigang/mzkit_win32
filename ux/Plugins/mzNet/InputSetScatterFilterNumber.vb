Public Class InputSetScatterFilterNumber

    Public Property FilterNumber As Integer
        Get
            Return CInt(Val(TextBox1.Text))
        End Get
        Set(value As Integer)
            TextBox1.Text = value
        End Set
    End Property

    Public Property RemoveSingle As Boolean
        Get
            Return CheckBox1.Checked
        End Get
        Set(value As Boolean)
            CheckBox1.Checked = value
        End Set
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If FilterNumber > 0 Then
            DialogResult = DialogResult.OK
        Else
            MessageBox.Show("A positive integer number must be set!", "Invalid value", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub
End Class