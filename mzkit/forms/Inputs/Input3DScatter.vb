Public Class Input3DScatter

    Private Function checkInput() As Boolean
        If ComboBox1.SelectedIndex = -1 Then
            MessageBox.Show("No label information!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return False
        ElseIf ComboBox2.SelectedIndex = -1 Then
            MessageBox.Show("No cluster information!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return False
        ElseIf ComboBox3.SelectedIndex = -1 OrElse ComboBox4.SelectedIndex = -1 OrElse ComboBox5.SelectedIndex = -1 Then
            MessageBox.Show("missing of the x,y,z data!", Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return False
        End If

        Return True
    End Function

    Public Sub LoadLabels(fields As IEnumerable(Of String))
        Dim box = {ComboBox1, ComboBox2, ComboBox3, ComboBox4, ComboBox5}

        For Each label As String In fields
            For Each i As ComboBox In box
                i.Items.Add(label)
            Next
        Next
    End Sub

    Public Function GetLabels() As (labels As String, clusters As String, x As String, y As String, z As String)
        Return (
             ComboBox1.SelectedItem.ToString,
             ComboBox2.SelectedItem.ToString,
             ComboBox3.SelectedItem.ToString,
             ComboBox4.SelectedItem.ToString,
             ComboBox5.SelectedItem.ToString
        )
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If checkInput() Then
            Me.DialogResult = DialogResult.OK
        End If
    End Sub
End Class