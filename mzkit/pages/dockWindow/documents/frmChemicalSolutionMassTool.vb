Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content

Public Class frmChemicalSolutionMassTool

    Dim calc As SolutionMassCalculator

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Call ListBox1.Items.Clear()
    End Sub

    Private Sub frmChemicalSolutionMassTool_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = Text
        Call ApplyVsTheme(ToolStrip1)
    End Sub
End Class