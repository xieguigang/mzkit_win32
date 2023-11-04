Public Class InputMSIPeakTableParameters

    Public Property TrIQCutoff As Double
        Get
            Return Val(TextBox3.Text)
        End Get
        Set(value As Double)
            TextBox3.Text = value
        End Set
    End Property

    Public Property IntoCutoff As Double
        Get
            Return Val(TextBox2.Text)
        End Get
        Set(value As Double)
            TextBox2.Text = value
        End Set
    End Property

    Public Property Mzdiff As Double
        Get
            Return Val(TextBox1.Text)
        End Get
        Set(value As Double)
            TextBox1.Text = value
        End Set
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub TextBox1_GotFocus(sender As Object, e As EventArgs) Handles TextBox1.GotFocus
        txtDesc.Text = "The mass tolerance error for export the spatial expression matrix, value in range from 0 to 1, data unit of this parameter is dalton. For export the expression matrix for the metabonomics analysis, 0.01 da is recommended, and for export the matrix for clustering analysis, 0.5 da is recommended."
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged

    End Sub

    Private Sub TextBox2_GotFocus(sender As Object, e As EventArgs) Handles TextBox2.GotFocus
        txtDesc.Text = "The peak data intensity cutoff threshold value. This parameter value is a percentage cutoff value, set value in range [0,1] which means from 0% to 100%. The higher cutoff value of this parameter, the less signal data that we keeps in the result matrix."
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged

    End Sub

    Private Sub TextBox3_GotFocus(sender As Object, e As EventArgs) Handles TextBox3.GotFocus
        txtDesc.Text = "The max intensity value cutoff threshold value. This parameter value is a percentage cutoff value, set value in range [0,1] which means from 0% to 100%. The smaller cutoff value of this parameter, the lower of the max intensity value, the less difference between the signals."
    End Sub
End Class