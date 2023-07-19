Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

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
        If txtFormula.Text.StringEmpty Then
            Call MessageBox.Show("No formula string!", "Formula Query", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Else
            Me.DialogResult = DialogResult.OK
        End If
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        InputDialog.Input(Of InputPubChemProxy)(
            Sub(cfg)
                Dim metadata = cfg.GetAnnotation

                txtFormula.Text = metadata.formula
                txtMetaboName.Text = metadata.name
            End Sub)
    End Sub
End Class