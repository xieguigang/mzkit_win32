Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq

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

    Public Sub SetRawdataInformation(raw As mzPack)
        Dim sb As New StringBuilder

        Text = "View Rawdata File Information"
        GroupBox1.Text = "Rawdata Information:"

        If raw.MS.IsNullOrEmpty Then
            Call sb.AppendLine("empty rawdata!")
        Else
            Dim mass_data As DoubleRange = raw.MS _
                .AsParallel _
                .Select(Iterator Function(si) As IEnumerable(Of Double)
                            If si.size > 0 Then
                                Yield si.mz.Min
                                Yield si.mz.Max
                            End If
                        End Function) _
                .IteratesALL _
                .Range

            Call sb.AppendLine($"MS1: {raw.MS.Length} scans")
            Call sb.AppendLine($"MS2: {Aggregate m1 In raw.MS Into Sum(m1.products.TryCount)} scans")
            Call sb.AppendLine($"rt range: {(raw.MS.First.rt / 60).ToString("F2")}min ~ {(raw.MS.Last.rt / 60).ToString("F2")}min")
            Call sb.AppendLine($"mass range: {mass_data.Min.ToString("F4")} ~ {mass_data.Max.ToString("F4")}")
            Call sb.AppendLine($"application: {raw.Application.ToString}({raw.Application.Description})")
        End If

        TextBox1.Text = sb.ToString
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class