Imports System.ComponentModel
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ApplicationServices
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

    Private Sub FormSelectTable_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.RowIndex > 0 AndAlso e.RowIndex < DataGridView1.Rows.Count Then
            If e.ColumnIndex = 1 Then
                Dim biodeep_id As String = DataGridView1.Rows(e.RowIndex).Cells(1).Value
                Dim url As String = $"https://query.biodeep.cn/metabolite/{biodeep_id}"

                Call OpenBrowser(url)
            End If
        End If
    End Sub

    Public Sub OpenBrowser(url As String)
        Dim startInfo As New ProcessStartInfo
        startInfo.FileName = "cmd.exe"
        startInfo.Arguments = "/c start " & url.CLIPath
        startInfo.UseShellExecute = True

        Call Process.Start(startInfo)
    End Sub
End Class