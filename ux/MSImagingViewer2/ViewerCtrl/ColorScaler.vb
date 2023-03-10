Imports System.Drawing
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors

Public Class ColorScaler

    Dim colorSet As ScalerPalette = ScalerPalette.FlexImaging
    Dim mapLevels As Integer = 200

    Public Property ScalerPalette As ScalerPalette
        Get
            Return colorSet
        End Get
        Set(value As ScalerPalette)
            colorSet = value
            updateColors()
        End Set
    End Property

    Public Property ScalerLevels As Integer
        Get
            Return mapLevels
        End Get
        Set(value As Integer)
            mapLevels = value
            updateColors()
        End Set
    End Property

    ''' <summary>
    ''' range in [0,1]
    ''' </summary>
    ''' <returns></returns>
    Public Property ScalerRange As DoubleRange
        Get
            Dim h As Integer = Height
            Dim up = 1 - picUpperbound.Bottom / h
            Dim lower = 1 - picLowerbound.Top / h

            Return New DoubleRange(lower, up)
        End Get
        Set(value As DoubleRange)
            Dim h As Integer = Height
            Dim upperBottom As Integer = (1 - value.Max) * h
            Dim lowerTop As Integer = (1 - value.Min) * h

            picUpperbound.Location = New Point(1, upperBottom - 10)
            picLowerbound.Location = New Point(1, lowerTop)

            updateColors()
        End Set
    End Property

    Private Sub updateColors()
        Dim colors As Color() = Designer.GetColors(colorSet.Description)

    End Sub

    Private Sub ColorScaler_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        picUpperbound.Location = New Point(1, 1)
        picUpperbound.Size = New Size(Width - 2, 10)
        picLowerbound.Location = New Point(1, Height - 10)
        picLowerbound.Size = New Size(Width - 2, 10)
    End Sub

    Private Sub ColorScaler_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        picUpperbound.Size = New Size(Width - 2, 10)
        picLowerbound.Size = New Size(Width - 2, 10)
    End Sub

    Dim moveUp, moveDown As Boolean
    Dim mousePos As Point

    Private Sub picUpperbound_MouseDown(sender As Object, e As MouseEventArgs) Handles picUpperbound.MouseDown
        moveUp = True
        mousePos = e.Location
    End Sub

    Private Sub picLowerbound_MouseDown(sender As Object, e As MouseEventArgs) Handles picLowerbound.MouseDown
        moveDown = True
        mousePos = e.Location
    End Sub

    Private Sub picUpperbound_MouseMove(sender As Object, e As MouseEventArgs) Handles picUpperbound.MouseMove
        If moveUp Then
            Dim oldPos = picUpperbound.Location
            Dim delta = e.Y - mousePos.Y

            mousePos = e.Location
            picUpperbound.Location = New Point(oldPos.X, oldPos.Y + delta)
        End If
    End Sub

    Private Sub picLowerbound_MouseMove(sender As Object, e As MouseEventArgs) Handles picLowerbound.MouseMove
        If moveDown Then
            Dim oldPos = picLowerbound.Location
            Dim delta = e.Y - mousePos.Y

            mousePos = e.Location
            picLowerbound.Location = New Point(oldPos.X, oldPos.Y + delta)
        End If
    End Sub

    Private Sub picUpperbound_MouseUp(sender As Object, e As MouseEventArgs) Handles picUpperbound.MouseUp
        moveUp = False
        Call updateColors()
    End Sub

    Private Sub picLowerbound_MouseUp(sender As Object, e As MouseEventArgs) Handles picLowerbound.MouseUp
        moveDown = False
        Call updateColors()
    End Sub
End Class
