Public Class InputMoNAHits

    Private Sub Button1_Click() Handles Button1.Click

    End Sub

    Public Sub DoMoNAQuery(name As String)
        TextBox1.Text = name
        Call Button1_Click()
    End Sub
End Class