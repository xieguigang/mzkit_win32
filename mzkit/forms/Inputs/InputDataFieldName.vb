Public Class InputDataFieldName

    Public ReadOnly Property GetInputFieldName As String
        Get
            Return ListBox1.SelectedItem.ToString
        End Get
    End Property

    Public Sub AddItems(fields As IEnumerable(Of String))
        For Each term As String In fields
            Call ListBox1.Items.Add(term)
        Next
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If ListBox1.SelectedIndex = -1 Then
            Button1.Enabled = False
        Else
            Button1.Enabled = True
        End If
    End Sub
End Class