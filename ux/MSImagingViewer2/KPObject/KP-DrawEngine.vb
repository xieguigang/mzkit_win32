Imports System.Drawing

Friend Class KP_DrawEngine : Implements IDisposable

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
    Private disposedValue As Boolean

    Public ReadOnly Property g As Graphics
        Get
            Return graphics
        End Get
    End Property

    Public Sub New()
        Try
            width = 0
            height = 0
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Function CreateDoubleBuffer(g As Graphics,
                                       width As Integer,
                                       height As Integer,
                                       previewWidth As Integer,
                                       previewHeight As Integer) As Boolean
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

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                If graphics IsNot Nothing Then
                    graphics.Dispose()
                End If

                If memoryBitmap IsNot Nothing Then
                    memoryBitmap.Dispose()
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
