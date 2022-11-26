Imports LaplacianHDR.Helpers
Imports UMapx.Imaging


Partial Public Class FormExposureFusion
    Inherits Form
#Region "Private data"
    Private fusion As ExposureFusion
    Private imagesField As Bitmap()
#End Region

#Region "Form voids"
    Public Sub New()
        InitializeComponent()

        AddHandler trackBar1.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar1.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
    End Sub

    Private Sub Form5_Load(sender As Object, e As EventArgs) Handles Me.Load
        pictureBox1.Image = Apply(imagesField)
    End Sub

    Public Function Apply(ParamArray images As Bitmap()) As Bitmap
        fusion = New ExposureFusion(Integer.MaxValue, Single.Parse(textBox2.Text))
        Return fusion.Apply(images)
    End Function

    Public Property Images As Bitmap()
        Set(value As Bitmap())
            Dim length = value.Length
            imagesField = New Bitmap(length - 1) {}

            For i = 0 To length - 1
                imagesField(i) = ImageHelper.Crop(value(i), pictureBox1.Width)
            Next
        End Set
        Get
            Return imagesField
        End Get
    End Property

    Private Sub button1_Click(sender As Object, e As EventArgs) Handles button1.Click
        DialogResult = DialogResult.OK
    End Sub
#End Region

#Region "TrackBars"
    Private Sub trackBar1_Scroll(sender As Object, e As EventArgs) Handles trackBar1.Scroll
        textBox2.Text = (trackBar1.Value / 100.0 + 0.1).ToString()
    End Sub

    Private Sub trackBar1_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar1.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar1.Value = 45
            trackBar1_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(imagesField)
    End Sub
#End Region
End Class

