Public Class InputSetUILooks

    Public Property SpotOpacity As Integer
        Get
            Return NumericUpDown1.Value
        End Get
        Set(value As Integer)
            NumericUpDown1.Value = value
        End Set
    End Property

    Public Property BackgroundContrast As Integer
        Get
            Return NumericUpDown2.Value
        End Get
        Set(value As Integer)
            NumericUpDown2.Value = value
        End Set
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class