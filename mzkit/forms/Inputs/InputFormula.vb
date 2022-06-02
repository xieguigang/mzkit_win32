Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType

Public Class InputFormula

    Public ReadOnly Property Polarity As Integer
        Get
            If RadioButton1.Checked Then
                Return 1
            Else
                Return -1
            End If
        End Get
    End Property

    Public ReadOnly Property GetAdducts As MzCalculator()
        Get
            If Polarity = 1 Then
                Return Provider.Positives
            Else
                Return Provider.Negatives
            End If
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If TextBox1.Text.StringEmpty Then
            Call MessageBox.Show("No formula string!", "Formula Query", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            Me.DialogResult = DialogResult.OK
        End If
    End Sub
End Class