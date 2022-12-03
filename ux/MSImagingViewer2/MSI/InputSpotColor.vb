Imports System.Drawing

Public Class InputSpotColor

    Public ReadOnly Property SpotColor As Color
        Get
            Return ThemeColorPicker1.Color
        End Get
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub ThemeColorPicker1_ColorSelected(sender As Object, e As ColorSelectedArg) Handles ThemeColorPicker1.ColorSelected
        PictureBox1.BackColor = SpotColor
    End Sub
End Class