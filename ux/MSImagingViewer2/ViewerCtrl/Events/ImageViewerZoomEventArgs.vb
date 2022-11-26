
Public Class ImageViewerZoomEventArgs : Inherits EventArgs

    Private zoomField As Integer
    Public ReadOnly Property Zoom As Integer
        Get
            Return zoomField
        End Get
    End Property

    Private inOutField As KpZoom
    Public ReadOnly Property InOut As KpZoom
        Get
            Return inOutField
        End Get
    End Property

    Public Sub New(zoom As Double, inOut As KpZoom)
        zoomField = Convert.ToInt32(Math.Round(zoom * 100, 0))
        inOutField = inOut
    End Sub
End Class
