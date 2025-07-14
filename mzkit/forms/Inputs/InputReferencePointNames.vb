Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleNames

Public Class InputReferencePointNames

    Public Iterator Function GetReferencePointNames(Optional err As Value(Of String) = Nothing) As IEnumerable(Of String)
        If err Is Nothing Then err = ""

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

    Public Function SetNames(names As IEnumerable(Of String)) As InputReferencePointNames
        inputNames = names.Distinct.Where(Function(s) Not s.StringEmpty(, True)).Indexing
        groups = inputNames.Objects _
            .GuessPossibleGroups(maxDepth:=True) _
            .ToDictionary(Function(a) a.name,
                          Function(a)
                              Return a.ToArray
                          End Function)

        Call CheckedListBox1.Items.Clear()

        For Each name As String In inputNames.Objects
            Call CheckedListBox1.Items.Add(name)
        Next

        Call ComboBox1.Items.Clear()
        Call ComboBox1.Items.Add("*[Clear All]")

        For Each name As String In groups.Keys
            Call ComboBox1.Items.Add(name)
        Next

        Return Me
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex < 0 Then
            Return
        ElseIf ComboBox1.SelectedIndex = 0 Then
            For i As Integer = 0 To CheckedListBox1.Items.Count - 1
                CheckedListBox1.SetItemChecked(i, False)
            Next
        Else
            Dim name As String = ComboBox1.Items(ComboBox1.SelectedIndex).ToString
            Dim group As Index(Of String) = groups(name).Indexing

            For i As Integer = 0 To CheckedListBox1.Items.Count - 1
                If CheckedListBox1.Items(i).ToString Like group Then
                    CheckedListBox1.SetItemChecked(i, True)
                Else
                    CheckedListBox1.SetItemChecked(i, False)
                End If
            Next
        End If
    End Sub
End Class