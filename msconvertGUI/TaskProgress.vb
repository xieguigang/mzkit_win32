Public Class TaskProgress

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Public Sub ShowMessage(msg As String)
        Call Me.Invoke(Sub() Label2.Text = msg)
    End Sub
End Class
