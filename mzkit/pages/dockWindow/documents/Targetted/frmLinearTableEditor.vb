Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

Public Class frmLinearTableEditor

    Dim is_list As String()

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Dim editList As New InputIdList With {.IdSet = is_list}
        InputDialog.Input(Sub(editor) Call SetIdList(editor.IdSet), config:=editList)
    End Sub

    Private Sub SetIdList(ids As IEnumerable(Of String))
        is_list = ids.SafeQuery.Where(Function(s) Strings.Len(s) > 0).ToArray

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            Dim row = DataGridView1.Rows(i)

            If row Is Nothing Then
                Continue For
            End If

            Dim combo As DataGridViewComboBoxCell = row.Cells(1)

            Call combo.Items.Clear()

            For Each id As String In is_list
                Call combo.Items.Add(id)
            Next
        Next
    End Sub
End Class