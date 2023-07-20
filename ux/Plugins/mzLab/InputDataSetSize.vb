Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.PackLib.Validation

Public Class InputDataSetSize

    Public ReadOnly Property GetParameters As DataSetParameters
        Get
            Return New DataSetParameters With {
                .Ions = Val(TextBox1.Text),
                .RawFiles = Val(TextBox2.Text),
                .rtmin = Val(TextBox3.Text),
                .rtmax = Val(TextBox4.Text),
                .AverageNumberOfSpectrum = Val(TextBox5.Text)
            }
        End Get
    End Property

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub
End Class
