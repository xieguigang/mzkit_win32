Public Class InputMoNA

    Public ReadOnly Property MoNA_id As String
        Get
            Return Strings.Trim(TextBox1.Text)
        End Get
    End Property

    Private Sub TextBox1_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyUp
        If e.KeyCode = Keys.Enter Then
            If Strings.Trim(TextBox1.Text).StringEmpty Then
                Me.DialogResult = DialogResult.Cancel
            Else
                Me.DialogResult = DialogResult.OK
            End If
        End If
    End Sub
End Class