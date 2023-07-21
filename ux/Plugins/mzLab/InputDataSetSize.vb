Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.PackLib.Validation

Public Class InputDataSetSize

    Dim idlist As String()

    Public ReadOnly Property GetParameters As DataSetParameters
        Get
            Return New DataSetParameters With {
                .Ions = Val(TextBox1.Text),
                .RawFiles = Val(TextBox2.Text),
                .rtmin = Val(TextBox3.Text),
                .rtmax = Val(TextBox4.Text),
                .AverageNumberOfSpectrum = Val(TextBox5.Text),
                .rawname = TextBox6.Text.Trim,
                .IdRange = idlist
            }
        End Get
    End Property

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Using file As New OpenFileDialog With {.Filter = "id list(*.txt)|*.txt"}
            If file.ShowDialog = DialogResult.OK Then
                idlist = file.FileName.ReadAllLines
            End If
        End Using
    End Sub
End Class
