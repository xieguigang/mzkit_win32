Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.Linq

Public Class FormSelectTable

    Public Iterator Function GetTargetSet() As IEnumerable(Of String)
        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            Dim row = DataGridView1.Rows(i)

            If row IsNot Nothing AndAlso CBool(row.Cells(0).Value) Then
                Yield $"{row.Cells(1).Value}_{row.Cells(4).Value}"
            End If
        Next
    End Function

    Public Sub SetAnnotation(report As IEnumerable(Of AlignmentHit))
        DataGridView1.Rows.Clear()

        For Each data As AlignmentHit In report.SafeQuery
            Call DataGridView1.Rows.Add(False, data.biodeep_id, data.name, data.formula, data.adducts)
        Next
    End Sub
End Class