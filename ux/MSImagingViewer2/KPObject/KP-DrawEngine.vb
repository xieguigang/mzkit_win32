Imports System.Drawing
Imports System

Friend Class KP_DrawEngine
    ''' <summary>
    ''' Original class to implement Double Buffering by
    ''' NT Almond 
    ''' 24 July 2003
    ''' 
    ''' Extended and adjusted by
    ''' Jordy "Kaiwa" Ruiter
    ''' </summary>
    ''' 
    Private graphics As Graphics
    Private memoryBitmap As Bitmap
    Private width As Integer
    Private height As Integer

    Public Sub Dispose()
        If graphics IsNot Nothing Then
            graphics.Dispose()
        End If

        If memoryBitmap IsNot Nothing Then
            memoryBitmap.Dispose()
        End If
    End Sub

    Public Sub New()
        Try
            width = 0
            height = 0
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Function CreateDoubleBuffer(g As Graphics, width As Integer, height As Integer, previewWidth As Integer, previewHeight As Integer) As Boolean
        Try
            KP_DrawObject.UpdatePanelsize(width, height)
            KP_DrawObject.UpdatePreviewPanelsize(previewWidth, previewHeight)

            If memoryBitmap IsNot Nothing Then
                memoryBitmap.Dispose()
                memoryBitmap = Nothing
            End If

            If graphics IsNot Nothing Then
                graphics.Dispose()
                graphics = Nothing
            End If

            If width = 0 OrElse height = 0 Then Return False

            Me.width = width
            Me.height = height

            memoryBitmap = New Bitmap(width, height)
            graphics = Graphics.FromImage(memoryBitmap)

            Return True
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
            Return False
        End Try
    End Function

    Public Sub Render(g As Graphics)
        Try
            If memoryBitmap IsNot Nothing Then
                g.DrawImage(memoryBitmap, New Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel)
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Function CanDoubleBuffer() As Boolean
        Try
            Return graphics IsNot Nothing
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
            Return False
        End Try
    End Function

    Public ReadOnly Property g As Graphics
        Get
            Return graphics
        End Get
    End Property
End Class
