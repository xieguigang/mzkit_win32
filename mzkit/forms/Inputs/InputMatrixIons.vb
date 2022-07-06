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

                If j = n Then
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
End Class