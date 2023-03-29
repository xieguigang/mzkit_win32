Public Module SpectralViewerModule

    Dim viewMatrix As Action(Of Object)

    Public Sub HookViewer(view As Action(Of Object))
        viewMatrix = view
    End Sub

    Public Sub ViewSpectral(data As Object)
        If Not viewMatrix Is Nothing Then
            Call viewMatrix(data)
        End If
    End Sub
End Module
