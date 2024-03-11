Public Class InputPCADialog

    Public ReadOnly Property ncomp As Integer
        Get
            Return TrackBar1.Value
        End Get
    End Property

    Public ReadOnly Property showSampleLable As Boolean
        Get
            Return CheckBox1.Checked
        End Get
    End Property

    Public Function SetMaxComponent(n As Integer) As InputPCADialog
        TrackBar1.Maximum = n
        Return Me
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Label2.Text = $"{TrackBar1.Value}/max:{TrackBar1.Maximum}"
    End Sub
End Class