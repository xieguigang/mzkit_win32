Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language

Public Class InputReferencePointNames

    Public Iterator Function GetReferencePointNames(err As Value(Of String)) As IEnumerable(Of String)
        For Each line As String In TextBox1.Text _
            .LineTokens _
            .Where(Function(a) Not a.StringEmpty(, True))

            If Not line Like inputNames Then
                err.Value = line
                Return
            Else
                Yield line
            End If
        Next
    End Function

    Dim inputNames As Index(Of String)

    Public Sub SetNames(names As IEnumerable(Of String))
        inputNames = names.Distinct.Where(Function(s) Not s.StringEmpty(, True)).Indexing
        TextBox1.Text = inputNames.Objects.JoinBy(vbCrLf)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class