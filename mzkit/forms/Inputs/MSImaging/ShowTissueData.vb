Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology

Public Class ShowTissueData

    Public Sub SetTissueClusters(data As IEnumerable(Of UMAPPoint))
        Dim clusters = data.GroupBy(Function(i) i.class).ToArray
        Dim sb As New StringBuilder

        Call sb.AppendLine($"There are {clusters.Length} clusters found on the tissue data.")
        Call sb.AppendLine()

        For Each cluster In clusters
            Call sb.AppendLine($"cluster {cluster.Key}: {cluster.Count} spots")
        Next

        TextBox1.Text = sb.ToString
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class