Public Class InputCloudEndPoint

    Public ReadOnly Property IP As String
        Get
            Return TextBox1.Text
        End Get
    End Property

    Public ReadOnly Property Port As Integer
        Get
            Try
                Return Integer.Parse(TextBox2.Text)
            Catch ex As Exception
                Return -1
            End Try
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If IP.StringEmpty Then
            MessageBox.Show("Empty value of the cloud service host!", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        ElseIf Port <= 0 Then
            MessageBox.Show("Invalid tcp port value!", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub
End Class