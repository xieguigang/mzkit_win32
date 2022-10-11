Imports LaplacianHDR.Filters
Imports LaplacianHDR.Helpers


Partial Public Class Form4
    Inherits Form
#Region "Private data"
    Private hsl As HueSaturationLightnessFilter = New HueSaturationLightnessFilter()
    Private imageField As Bitmap
#End Region

#Region "Form voids"
    Public Sub New()
        InitializeComponent()
        AddHandler trackBar1.MouseUp, New MouseEventHandler(AddressOf trackBar1_MouseUp)
        AddHandler trackBar2.MouseUp, New MouseEventHandler(AddressOf trackBar2_MouseUp)
        AddHandler trackBar3.MouseUp, New MouseEventHandler(AddressOf trackBar3_MouseUp)
        AddHandler trackBar1.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar2.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar3.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar1.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
        AddHandler trackBar2.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
        AddHandler trackBar3.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
    End Sub

    Private Sub Form4_Load(ByVal sender As Object, ByVal e As EventArgs)
        pictureBox1.Image = Apply(imageField)
    End Sub

    Public Function Apply(ByVal image As Bitmap) As Bitmap
        ' parsing
        Dim h = Single.Parse(textBox1.Text)
        Dim s = Single.Parse(textBox2.Text) / 100.0F
        Dim l = Single.Parse(textBox3.Text) / 100.0F

        ' applying filter
        hsl.SetParams(h, s, l)
        Return hsl.Apply(image)
    End Function

    Public Property Image As Bitmap
        Set(ByVal value As Bitmap)
            imageField = Crop(value, pictureBox1.Width)
            pictureBox1.Image = imageField
        End Set
        Get
            Return imageField
        End Get
    End Property

    Private Sub button1_Click(ByVal sender As Object, ByVal e As EventArgs)
        DialogResult = DialogResult.OK
    End Sub
#End Region

#Region "TrackBars"
    Private Sub trackBar1_Scroll(ByVal sender As Object, ByVal e As EventArgs)
        textBox1.Text = trackBar1.Value.ToString()
    End Sub
    Private Sub trackBar2_Scroll(ByVal sender As Object, ByVal e As EventArgs)
        textBox2.Text = trackBar2.Value.ToString()
    End Sub
    Private Sub trackBar3_Scroll(ByVal sender As Object, ByVal e As EventArgs)
        textBox3.Text = trackBar3.Value.ToString()
    End Sub

    Private Sub trackBar1_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
        If e.Button = MouseButtons.Right Then
            trackBar1.Value = 0
            trackBar1_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(imageField)
    End Sub
    Private Sub trackBar2_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
        If e.Button = MouseButtons.Right Then
            trackBar2.Value = 0
            trackBar2_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(imageField)
    End Sub
    Private Sub trackBar3_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
        If e.Button = MouseButtons.Right Then
            trackBar3.Value = 0
            trackBar3_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(imageField)
    End Sub
#End Region
End Class

