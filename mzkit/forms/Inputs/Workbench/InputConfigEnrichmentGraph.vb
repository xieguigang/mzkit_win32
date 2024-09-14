Public Class InputConfigEnrichmentGraph

    Public ReadOnly Property fdr As Double
        Get
            Return Val(ComboBox1.Text)
        End Get
    End Property

    Public ReadOnly Property topN As Integer
        Get
            If ComboBox2.Text = "*" OrElse ComboBox2.Text.StringEmpty(, True) Then
                ' no cutoff
                Return Integer.MaxValue
            End If

            Return Val(ComboBox2.Text)
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If fdr <= 0.0 Then
            MessageBox.Show("The pvalue FDR cutoff should not be zero or negative!",
                            "Invalid cutoff",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information)
            Return
        End If

        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub InputConfigEnrichmentGraph_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox2.SelectedIndex = 0
    End Sub
End Class