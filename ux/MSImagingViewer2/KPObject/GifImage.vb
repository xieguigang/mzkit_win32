Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.CompilerServices

Public Class GifImage : Implements IDisposable

    Private KpViewer As KpImageViewer
    Private gif As Image
    Private dimension As FrameDimension
    Private m_frameCount As Integer
    Private m_rotation As Integer = 0
    Private m_currentFrame As Bitmap = Nothing
    Private disposedValue As Boolean

    Public ReadOnly Property CurrentFrame As Bitmap
        Get
            Return m_currentFrame
        End Get
    End Property

    Public ReadOnly Property FrameCount As Integer
        Get
            Return m_frameCount
        End Get
    End Property

    Public ReadOnly Property Rotation As Integer
        Get
            Return m_rotation
        End Get
    End Property

    Public Sub New(KpViewer As KpImageViewer, img As Image)
        Me.KpViewer = KpViewer
        gif = img
        dimension = New FrameDimension(gif.FrameDimensionsList(0))
        m_frameCount = gif.GetFrameCount(dimension)
        gif.SelectActiveFrame(dimension, 0)
        m_currentFrame = CType(gif.Clone(), Bitmap)

        Call UpdateAnimator()
    End Sub

    Public Sub UpdateAnimator()
        If KpViewer.GifAnimation Then
            Call ImageAnimator.Animate(gif, New EventHandler(AddressOf OnFrameChanged))
        Else
            Call ImageAnimator.StopAnimate(gif, New EventHandler(AddressOf OnFrameChanged))
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Rotate(rotation As Integer)
        m_rotation = (m_rotation + rotation) Mod 360
    End Sub

    Private Sub OnFrameChanged(o As Object, e As EventArgs)
        m_currentFrame = CType(gif, Bitmap)
        KpViewer.InvalidatePanel()
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                gif.Dispose()
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
