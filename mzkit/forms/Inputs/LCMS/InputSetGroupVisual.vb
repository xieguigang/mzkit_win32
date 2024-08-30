Public Class InputSetGroupVisual

    Public Iterator Function GetGroupNames() As IEnumerable(Of String)
        For i As Integer = 0 To CheckedListBox1.Items.Count - 1
            If CheckedListBox1.GetItemChecked(i) Then
                Yield CStr(CheckedListBox1.Items(i))
            End If
        Next
    End Function

    Public Sub SetGroupLabels(names As IEnumerable(Of String))
        For Each name As String In names
            CheckedListBox1.Items.Add(name)
        Next
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If GetGroupNames.Count < 2 Then
            MessageBox.Show("Two sample groups must be choosen to make data visualization!", "No data", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
            Return
        End If

        DialogResult = DialogResult.OK
    End Sub
End Class