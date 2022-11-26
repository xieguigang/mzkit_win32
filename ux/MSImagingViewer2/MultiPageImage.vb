Imports System.Drawing

Public Class MultiPageImage
    Public Sub New(ByVal image As Bitmap)
        Me.Image = image
        currentPage = 0
    End Sub

    Private rotationField As Integer = 0
    Public ReadOnly Property Rotation As Integer
        Get
            Return rotationField
        End Get
    End Property

    Private currentPage As Integer = 0

    Private imageField As Bitmap
    Public Property Image As Bitmap
        Get
            Return bmp
        End Get
        Set(ByVal value As Bitmap)
            If imageField IsNot Nothing Then
                imageField.Dispose()
                imageField = Nothing
            End If

            imageField = value

            If bmp IsNot Nothing Then
                bmp.Dispose()
                bmp = Nothing
            End If

            bmp = New Bitmap(imageField)
        End Set
    End Property

    Private bmp As Bitmap

    Public ReadOnly Property Page As Bitmap
        Get
            If bmp Is Nothing Then
                bmp = New Bitmap(imageField)
            End If

            Return bmp
        End Get
    End Property

    Public Sub Rotate(ByVal rotation As Integer)
        If rotation = 90 OrElse rotation = 180 OrElse rotation = 270 OrElse rotation = 0 Then
            rotationField = rotation

            If rotationField = 90 Then
                bmp.RotateFlip(RotateFlipType.Rotate90FlipNone)
            ElseIf rotationField = 180 Then
                bmp.RotateFlip(RotateFlipType.Rotate180FlipNone)
            ElseIf rotationField = 270 Then
                bmp.RotateFlip(RotateFlipType.Rotate270FlipNone)
            End If
        End If
    End Sub

    Public Sub SetPage(ByVal pageNumber As Integer)
        If imageField IsNot Nothing Then
            If currentPage <> pageNumber Then
                Dim pages = imageField.GetFrameCount(Imaging.FrameDimension.Page)
                If pages > pageNumber AndAlso pageNumber >= 0 Then
                    currentPage = pageNumber

                    imageField.SelectActiveFrame(Imaging.FrameDimension.Page, pageNumber)

                    If bmp IsNot Nothing Then
                        bmp.Dispose()
                        bmp = Nothing
                    End If

                    bmp = New Bitmap(imageField)
                End If
            End If
        End If
    End Sub

    Public Function GetBitmap(ByVal pageNumber As Integer) As Bitmap
        If imageField Is Nothing Then
            Return Nothing
        End If

        If currentPage <> pageNumber Then
            Dim pages = imageField.GetFrameCount(Imaging.FrameDimension.Page)
            If pages > pageNumber AndAlso pageNumber >= 0 Then
                currentPage = pageNumber

                imageField.SelectActiveFrame(Imaging.FrameDimension.Page, pageNumber)

                If bmp IsNot Nothing Then
                    bmp.Dispose()
                    bmp = Nothing
                End If

                bmp = New Bitmap(imageField)
            End If
        End If

        Return bmp
    End Function

    Public Sub Dispose()
        If Image IsNot Nothing Then
            imageField.Dispose()
            imageField = Nothing
        End If

        If bmp IsNot Nothing Then
            bmp.Dispose()
            bmp = Nothing
        End If
    End Sub
End Class
