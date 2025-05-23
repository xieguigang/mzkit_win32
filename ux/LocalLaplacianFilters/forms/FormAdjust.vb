﻿Imports LaplacianHDR.Filters
Imports LaplacianHDR.Helpers


Partial Public Class FormAdjust
    Inherits Form
#Region "Private data"
    Private hsl As HueSaturationLightnessFilter = New HueSaturationLightnessFilter()
    Private imageField As Bitmap
#End Region

#Region "Form voids"
    Public Sub New()
        InitializeComponent()

        AddHandler trackBar1.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar2.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar3.MouseWheel, Sub(sender, e) CType(e, HandledMouseEventArgs).Handled = True
        AddHandler trackBar1.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
        AddHandler trackBar2.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
        AddHandler trackBar3.KeyDown, Sub(sender, e) CType(e, KeyEventArgs).Handled = True
    End Sub

    Private Sub Form4_Load(sender As Object, e As EventArgs) Handles Me.Load
        pictureBox1.Image = Apply(imageField)
    End Sub

    Public Function Apply(image As Bitmap) As Bitmap
        ' parsing
        Dim h = Single.Parse(textBox1.Text)
        Dim s = Single.Parse(textBox2.Text) / 100.0F
        Dim l = Single.Parse(textBox3.Text) / 100.0F

        ' applying filter
        hsl.SetParams(h, s, l)
        Return hsl.Apply(image)
    End Function

    Public Property Image As Bitmap
        Set(value As Bitmap)
            imageField = Crop(value, pictureBox1.Width)
            pictureBox1.Image = imageField
        End Set
        Get
            Return imageField
        End Get
    End Property

    Private Sub button1_Click(sender As Object, e As EventArgs) Handles button1.Click
        DialogResult = DialogResult.OK
    End Sub
#End Region

#Region "TrackBars"
    Private Sub trackBar1_Scroll(sender As Object, e As EventArgs) Handles trackBar1.Scroll
        textBox1.Text = trackBar1.Value.ToString()
    End Sub
    Private Sub trackBar2_Scroll(sender As Object, e As EventArgs) Handles trackBar2.Scroll
        textBox2.Text = trackBar2.Value.ToString()
    End Sub
    Private Sub trackBar3_Scroll(sender As Object, e As EventArgs) Handles trackBar3.Scroll
        textBox3.Text = trackBar3.Value.ToString()
    End Sub

    Private Sub trackBar1_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar1.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar1.Value = 0
            trackBar1_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(imageField)
    End Sub
    Private Sub trackBar2_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar2.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar2.Value = 0
            trackBar2_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(imageField)
    End Sub
    Private Sub trackBar3_MouseUp(sender As Object, e As MouseEventArgs) Handles trackBar3.MouseUp
        If e.Button = MouseButtons.Right Then
            trackBar3.Value = 0
            trackBar3_Scroll(sender, e)
        End If
        pictureBox1.Image = Apply(imageField)
    End Sub
#End Region
End Class

