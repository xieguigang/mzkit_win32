Public Class ImageViewerRotationEventArgs : Inherits EventArgs

    Public ReadOnly Property Rotation As Integer

    Public Sub New(rotation As Integer)
        _Rotation = rotation
    End Sub

End Class