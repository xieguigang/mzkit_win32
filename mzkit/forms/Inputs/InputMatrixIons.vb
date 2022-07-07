Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class InputMatrixIons

    Public ReadOnly Property matrixSize As Size
        Get
            Return New Size(Integer.Parse(txtColumns.Text), Integer.Parse(txtRows.Text))
        End Get
    End Property

    Public Iterator Function GetSelectedIons() As IEnumerable(Of NamedValue(Of Double))
        Dim mz As Double() = DataGridView1.getFieldVector("mz")
        Dim name As Array = DataGridView1.getFieldVector("name")
        Dim precursor As Array = DataGridView1.getFieldVector("precursor_type")
        Dim selects As Boolean() = DataGridView1.getFieldVector("select")
        Dim n As Integer = matrixSize.Width * matrixSize.Height
        Dim j As Integer = 1

        For i As Integer = 0 To selects.Length - 1
            If selects(i) Then
                Yield New NamedValue(Of Double)(name(i), mz(i), precursor(i))

                j += 1

                If j > n Then
                    Exit For
                End If
            End If
        Next
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DialogResult = DialogResult.OK
    End Sub

    Private Sub InputMatrixIons_Load(sender As Object, e As EventArgs) Handles Me.Load
        ToolStripStatusLabel1.Text = "Please select 9 ions to visual data..."
    End Sub

    Dim n As Integer = 1

    Private Sub DataGridView1_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellValueChanged
        If e.ColumnIndex = 0 AndAlso e.RowIndex >= 0 Then
            Dim r = DataGridView1.Rows(e.RowIndex)
            Dim value As Boolean = r.Cells(e.ColumnIndex).Value
            Dim size = Me.matrixSize
            Dim total As Integer = size.Width * size.Height

            If value = True Then
                n += 1
            Else
                n -= 1
            End If

            If n < 0 Then
                n = 0
            End If

            Dim text As String = $"there are {n}/{total} ions has been selected."

            If n > total Then
                text &= $" ions that after #{total} will be ignores."
            End If

            ToolStripStatusLabel1.Text = text
        End If
    End Sub

    Private Sub DataGridView1_CellValuePushed(sender As Object, e As DataGridViewCellValueEventArgs) Handles DataGridView1.CellValuePushed

    End Sub
End Class