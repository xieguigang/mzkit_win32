Public Class InputMSIRotation

    Public ReadOnly Property GetAngle As Double
        Get
            Return NumericUpDown1.Value
        End Get
    End Property

    Dim previews_raw As Image

    Public Sub SetImage(img As Image)
        previews_raw = img
        PictureBox1.BackgroundImage = img
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class