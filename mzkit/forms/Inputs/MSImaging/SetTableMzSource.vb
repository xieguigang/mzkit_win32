Imports SMRUCC.Rsharp.Runtime.Vectorization

Public Class SetTableMzSource

    Public Function GetMz(tbl As DataGridView) As Double()
        Dim tar = ComboBox1.Items(ComboBox1.SelectedIndex).ToString
        Return CLRVector.asNumeric(tbl.getFieldVector(tar))
    End Function

    Public Function GetNames(tbl As DataGridView) As String()
        Dim tar = ComboBox2.Items(ComboBox2.SelectedIndex).ToString

        If ComboBox2.SelectedIndex = 0 Then
            Return Nothing
        End If

        Return CLRVector.asCharacter(tbl.getFieldVector(tar))
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ComboBox1.SelectedIndex < 0 Then
            MessageBox.Show("A column for read as ion m/z source must be selected!",
                            "No ion source",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation)
            Return
        End If

        DialogResult = DialogResult.OK
    End Sub

    Public Sub SetColumns(cols As IEnumerable(Of String))
        ComboBox2.Items.Add("NO_NAMES")

        For Each name As String In cols
            ComboBox1.Items.Add(name)
            ComboBox2.Items.Add(name)
        Next

        ComboBox2.SelectedIndex = 0
    End Sub
End Class