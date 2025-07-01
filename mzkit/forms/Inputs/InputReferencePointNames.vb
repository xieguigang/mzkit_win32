Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleNames

Public Class InputReferencePointNames

    Public Iterator Function GetReferencePointNames(err As Value(Of String)) As IEnumerable(Of String)
        For i As Integer = 0 To CheckedListBox1.Items.Count - 1
            Dim line As String = CheckedListBox1.Items(i)

            If Not CheckedListBox1.GetItemChecked(i) Then
                Continue For
            End If

            If Not line Like inputNames Then
                err.Value = line
                Return
            Else
                Yield line
            End If
        Next
    End Function

    Dim inputNames As Index(Of String)
    Dim groups As Dictionary(Of String, String())

    Public Sub SetNames(names As IEnumerable(Of String))
        inputNames = names.Distinct.Where(Function(s) Not s.StringEmpty(, True)).Indexing
        groups = inputNames.Objects _
            .GuessPossibleGroups _
            .ToDictionary(Function(a) a.name,
                          Function(a)
                              Return a.ToArray
                          End Function)

        Call CheckedListBox1.Items.Clear()

        For Each name As String In inputNames.Objects
            Call CheckedListBox1.Items.Add(name)
        Next
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class