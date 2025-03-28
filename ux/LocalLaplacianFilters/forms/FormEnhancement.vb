﻿Imports LaplacianHDR.Filters
Imports LaplacianHDR.Helpers
Imports UMapx.Imaging


Partial Public Class FormEnhancement
    Inherits Form
#Region "Private data"
    Private gllf As GeneralizedLocalLaplacianFilter = New GeneralizedLocalLaplacianFilter()
    Private spaceField As Space = Space.YCbCr
    Private imageField As Bitmap
#End Region

#Region "Form voids"
    Public Sub New()
        InitializeComponent()

        AddHandler trackBar1.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar2.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar3.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar4.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar5.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar6.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar1.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
        AddHandler trackBar2.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
        AddHandler trackBar3.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
        AddHandler trackBar4.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
        AddHandler trackBar5.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
        AddHandler trackBar6.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles Me.Load
        pictureBox1.Image = Apply(imageField)
    End Sub

    Public Property Image As Bitmap
        Set(value As Bitmap)
            imageField = ImageHelper.Crop(value, pictureBox1.Width)
            pictureBox1.Image = imageField
        End Set
        Get
            Return imageField
        End Get
    End Property

    Public Property Space As Space
        Set(value As Space)
            spaceField = value
        End Set
        Get
            Return spaceField
        End Get
    End Property

    Public Function Apply(image As Bitmap) As Bitmap
        ' parsing
        Dim lightshadows As Single = Math.Pow(2, Single.Parse(textBox1.Text) / 100.0)
        Dim sigma = Single.Parse(textBox2.Text)
        Dim discrets = Integer.Parse(textBox3.Text)
        Dim levels = Integer.Parse(textBox4.Text)
        Dim factor = Single.Parse(textBox5.Text)
        Dim radius = 2 * (Integer.Parse(textBox6.Text) + 1)

        ' applying filter
        gllf.SetParams(radius, lightshadows, sigma, discrets, levels, factor, spaceField)
        Return gllf.Apply(image)
    End Function

    Private Sub button1_Click(sender As Object, e As EventArgs) Handles button1.Click
        DialogResult = DialogResult.OK
    End Sub
#End Region

#Region "TrackBars"
    Private Sub trackBar1_Scroll(sender As Object, e As EventArgs) Handles trackBar1.Scroll
        textBox1.Text = trackBar1.Value.ToString()
    End Sub
    Private Sub trackBar2_Scroll(sender As Object, e As EventArgs) Handles trackBar2.Scroll
        textBox2.Text = (trackBar2.Value / 1000.0 + 0.001).ToString()
    End Sub
    Private Sub trackBar3_Scroll(sender As Object, e As EventArgs) Handles trackBar3.Scroll
        textBox3.Text = trackBar3.Value.ToString()
    End Sub
    Private Sub trackBar4_Scroll(sender As Object, e As EventArgs) Handles trackBar4.Scroll
        textBox4.Text = trackBar4.Value.ToString()
    End Sub
    Private Sub trackBar5_Scroll(sender As Object, e As EventArgs) Handles trackBar5.Scroll
        Dim v = trackBar5.Value

        If v < 0 Then
            textBox5.Text = (v / 100.0).ToString()
        Else
            textBox5.Text = (v / 10.0).ToString()
        End If
    End Sub
    Private Sub trackBar6_Scroll(sender As Object, e As EventArgs) Handles trackBar6.Scroll
        textBox6.Text = trackBar6.Value.ToString()
    End Sub

    Private Sub trackBar6_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar6.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar6.Value = 0
            trackBar6_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(imageField)
    End Sub
    Private Sub trackBar5_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar5.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar5.Value = 0
            trackBar5_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(imageField)
    End Sub
    Private Sub trackBar4_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar4.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar4.Value = 20
            trackBar4_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(imageField)
    End Sub
    Private Sub trackBar3_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar3.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar3.Value = 20
            trackBar3_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(imageField)
    End Sub
    Private Sub trackBar2_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar2.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar2.Value = 49
            trackBar2_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(imageField)
    End Sub
    Private Sub trackBar1_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar1.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar1.Value = 0
            trackBar1_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(imageField)
    End Sub
#End Region
End Class

