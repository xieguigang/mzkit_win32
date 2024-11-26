Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Windows.Forms.DataValidation.UIInteractive
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

Public Class frmLinearTableEditor

    Dim is_list As String()

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Dim editList As New InputIdList With {.IdSet = is_list}
        InputDialog.Input(Sub(editor) Call SetIdList(editor.IdSet), config:=editList)
    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = System.Windows.Forms.Keys.V AndAlso e.Control AndAlso Clipboard.ContainsText Then
            Call DataGridView1.PasteTextData()
        End If
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

        Call DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        If DataGridView1.Columns.Count > 2 Then
            If MessageBox.Show("The last column for the reference point will be removed from this linear table?",
                               "Delete data",
                               MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then

                DataGridView1.Columns.RemoveAt(DataGridView1.Columns.Count - 1)
            End If
        Else
            Call Workbench.Warning("no more reference point column could be removes.")
        End If
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Dim n = DataGridView1.Columns.Count
        Dim level = n - 2 + 1

        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.Name = $"L{level}", .HeaderText = .Name})
    End Sub

    Private Sub DataGridView1_RowsAdded(sender As Object, e As DataGridViewRowsAddedEventArgs) Handles DataGridView1.RowsAdded
        Dim row = DataGridView1.Rows(e.RowIndex)
        Dim combo As DataGridViewComboBoxCell = row.Cells(1)

        Call combo.Items.Clear()

        For Each id As String In is_list.SafeQuery
            Call combo.Items.Add(id)
        Next
    End Sub
End Class