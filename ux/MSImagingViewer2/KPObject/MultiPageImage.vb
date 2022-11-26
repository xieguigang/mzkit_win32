Imports System.Drawing

Public Class MultiPageImage : Implements IDisposable

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
        Set(value As Bitmap)
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
    Private disposedValue As Boolean

    Public ReadOnly Property Page As Bitmap
        Get
            If bmp Is Nothing Then
                bmp = New Bitmap(imageField)
            End If

            Return bmp
        End Get
    End Property

    Public Sub New(image As Bitmap)
        Me.Image = image
        currentPage = 0
    End Sub

    Public Sub Rotate(rotation As Integer)
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

    Public Sub SetPage(pageNumber As Integer)
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

    Public Function GetBitmap(pageNumber As Integer) As Bitmap
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

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                If Image IsNot Nothing Then
                    imageField.Dispose()
                    imageField = Nothing
                End If

                If bmp IsNot Nothing Then
                    bmp.Dispose()
                    bmp = Nothing
                End If
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
            ' TODO: 将大型字段设置为 null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
    ' Protected Overrides Sub Finalize()
    '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
