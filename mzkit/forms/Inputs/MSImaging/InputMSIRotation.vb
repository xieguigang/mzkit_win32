Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Imaging

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

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        NumericUpDown1.Value = 0
        SetImage(previews_raw)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim g As Graphics2D = Graphics2D.CreateDevice(New Size(previews_raw.Width, previews_raw.Height), filled:=Color.Transparent)
        Dim mat As New Matrix
        mat.RotateAt(GetAngle, New PointF(g.Width / 2, g.Height / 2))
        g.SetTransformMatrix(mat)
        g.DrawImage(previews_raw, New PointF)
        PictureBox1.BackgroundImage = g.ImageResource
        g.Dispose()
    End Sub
End Class