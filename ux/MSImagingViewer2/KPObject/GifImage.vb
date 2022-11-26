Imports System
Imports System.Drawing
Imports System.Drawing.Imaging

Public Class GifImage
    Private KpViewer As KpImageViewer
    Private gif As Image
    Private dimension As FrameDimension
    Private frameCountField As Integer
    Private rotationField As Integer = 0
    Private currentFrameBmp As Bitmap = Nothing

    Public Sub New(ByVal KpViewer As KpImageViewer, ByVal img As Image)
        Me.KpViewer = KpViewer
        gif = img
        dimension = New FrameDimension(gif.FrameDimensionsList(0))
        frameCountField = gif.GetFrameCount(dimension)

        gif.SelectActiveFrame(dimension, 0)

        currentFrameBmp = CType(gif.Clone(), Bitmap)

        UpdateAnimator()
    End Sub

    Public Sub UpdateAnimator()
        If KpViewer.GifAnimation Then
            Call ImageAnimator.Animate(gif, New EventHandler(AddressOf OnFrameChanged))
        Else
            Call ImageAnimator.StopAnimate(gif, New EventHandler(AddressOf OnFrameChanged))
        End If
    End Sub

    Public ReadOnly Property Rotation As Integer
        Get
            Return rotationField
        End Get
    End Property

    Public Sub Rotate(ByVal rotation As Integer)
        rotationField = (rotationField + rotation) Mod 360
    End Sub

    Public Sub Dispose()
        gif.Dispose()
    End Sub

    Private Sub OnFrameChanged(ByVal o As Object, ByVal e As EventArgs)
        currentFrameBmp = CType(gif, Bitmap)

        KpViewer.InvalidatePanel()
    End Sub

    Public ReadOnly Property CurrentFrame As Bitmap
        Get
            Return currentFrameBmp
        End Get
    End Property

    Public ReadOnly Property FrameCount As Integer
        Get
            Return frameCountField
        End Get
    End Property
End Class
