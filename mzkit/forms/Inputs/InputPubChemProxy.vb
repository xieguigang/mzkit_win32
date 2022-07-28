Public Class InputPubChemProxy

    ''' <summary>
    ''' do pubchem search
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Strings.Trim(TextBox1.Text).StringEmpty Then
            Call MessageBox.Show("No query text input!", "PubChem Query", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            Call doSearch(TextBox1.Text)
        End If
    End Sub

    Private Sub doSearch(text As String)
        ' text to cid
        ' then query by cid

    End Sub
End Class