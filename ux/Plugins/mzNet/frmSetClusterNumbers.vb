Public Class frmSetClusterNumbers

    Public ReadOnly Property TopNClusters As Integer
        Get
            Return CInt(Val(TextBox1.Text))
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TopNClusters <= 0 Then
            MessageBox.Show("Invalid cluster numbers! Number specificed must be a positive integer number!", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        Else

        End If

        Me.DialogResult = DialogResult.OK
    End Sub
End Class