Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

Public Class frmLinearTableEditor

    Dim is_list As String()

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Dim editList As New InputIdList With {.IdSet = is_list}
        InputDialog.Input(Sub(editor) Call SetIdList(editor.IdSet), config:=editList)
    End Sub

    Private Sub SetIdList(id As IEnumerable(Of String))
        is_list = id.SafeQuery.Where(Function(s) Strings.Len(s) > 0).ToArray

    End Sub
End Class