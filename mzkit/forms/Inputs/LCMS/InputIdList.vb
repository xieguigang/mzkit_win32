Public Class InputIdList

    Public Property IdSet As String()
        Get
            Return Strings.Trim(TextBox1.Text).LineTokens.Where(Function(s) s.Length > 0).ToArray
        End Get
        Set(value As String())
            TextBox1.Text = value.JoinBy(vbCrLf)
        End Set
    End Property

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class