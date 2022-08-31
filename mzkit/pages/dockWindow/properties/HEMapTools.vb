Public Class HEMapTools

    Private Sub HEMapTools_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TabText = "HEMap Tools"
    End Sub

    ''' <summary>
    ''' add new color channel
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked

    End Sub

    Private Sub ColorComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ColorComboBox1.SelectedIndexChanged
        If ColorComboBox1.SelectedIndex > -1 Then
            Dim color As Color = ColorComboBox1.SelectedItem

        End If
    End Sub
End Class