Public Class InputLoadLCMSScatter

    Public ReadOnly Property MzField As String
        Get
            Return ComboBox1.SelectedItem.ToString
        End Get
    End Property

    Public ReadOnly Property RtField As String
        Get
            Return ComboBox2.SelectedItem.ToString
        End Get
    End Property

    Public ReadOnly Property DataField As String
        Get
            Return ComboBox3.SelectedItem.ToString
        End Get
    End Property

    Public Sub SetDataFeilds(fields As IEnumerable(Of String))
        For Each name As String In fields
            ComboBox1.Items.Add(name)
            ComboBox2.Items.Add(name)
            ComboBox3.Items.Add(name)
        Next

        ComboBox1.SelectedIndex = 0
        ComboBox2.SelectedIndex = 0
        ComboBox3.SelectedIndex = 0
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DialogResult = DialogResult.OK
    End Sub
End Class