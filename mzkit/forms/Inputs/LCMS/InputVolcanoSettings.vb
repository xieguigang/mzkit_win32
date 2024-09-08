Public Class InputVolcanoSettings

    Public ReadOnly Property Trial As String
        Get
            If ComboBox1.SelectedIndex < 0 Then
                Return Nothing
            End If
            Return ComboBox1.Items(ComboBox1.SelectedIndex).ToString
        End Get
    End Property

    Public ReadOnly Property ControlGroup As String
        Get
            If ComboBox2.SelectedIndex < 0 Then
                Return Nothing
            End If
            Return ComboBox2.Items(ComboBox2.SelectedIndex).ToString
        End Get
    End Property

    Public ReadOnly Property pvalue As Double
        Get
            Return Val(ComboBox3.Text)
        End Get
    End Property

    Public ReadOnly Property log2fc As Double
        Get
            Return NumericUpDown1.Value
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not ComboBox3.Text.IsNumeric Then
            MessageBox.Show("Invalid number format for t-test p-value cutoff settings.", "Invalid Format", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        If Trial.StringEmpty(, ) OrElse ControlGroup.StringEmpty Then
            MessageBox.Show("Please select the data group comparision for volcano plot analysis!", "No group selection", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        If Trial = ControlGroup Then
            MessageBox.Show("Trial should not equals to Control!", "Invalid comparision", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If
        If log2fc < 0 Then
            MessageBox.Show("No significant expression cutoff due to zero or negative cutoff was selected.", "No cutoff", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

        DialogResult = DialogResult.OK
    End Sub

    Public Sub SetGroups(names As IEnumerable(Of String))
        For Each name As String In names
            Call ComboBox1.Items.Add(name)
            Call ComboBox2.Items.Add(name)
        Next
    End Sub

    Private Sub InputVolcanoSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox3.SelectedIndex = 0
    End Sub
End Class