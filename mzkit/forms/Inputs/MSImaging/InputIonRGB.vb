Public Class InputIonRGB

    Public ReadOnly Property R As String
        Get
            Return getIon(cR)
        End Get
    End Property

    Public ReadOnly Property G As String
        Get
            Return getIon(cG)
        End Get
    End Property

    Public ReadOnly Property B As String
        Get
            Return getIon(cB)
        End Get
    End Property

    Private Shared Function getIon(list As ComboBox) As String
        If list.SelectedIndex = -1 Then
            Return ""
        ElseIf list.SelectedIndex = 0 Then
            Return ""
        Else
            Return list.SelectedItem.ToString
        End If
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DialogResult = DialogResult.OK
    End Sub
End Class