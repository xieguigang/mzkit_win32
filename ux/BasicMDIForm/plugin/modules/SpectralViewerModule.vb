Public Module SpectralViewerModule

    Dim viewMatrix As Action(Of Object)
    Dim runmassDiffAnalysis As Action(Of Double, Array)
    Dim showClusterSpectrum As Action(Of Array, String)

    Public Sub HookViewer(view As Action(Of Object))
        viewMatrix = view
    End Sub

    Public Sub HookAnalysis(analysis As Action(Of Double, Array))
        runmassDiffAnalysis = analysis
    End Sub

    Public Sub HookClusterLoader(load As Action(Of Array, String))
        showClusterSpectrum = load
    End Sub

    Public Sub showCluster(raw As Array, vlabel As String)
        If showClusterSpectrum IsNot Nothing Then
            Call showClusterSpectrum(raw, vlabel)
        End If
    End Sub

    Public Sub ViewSpectral(data As Object)
        If Not viewMatrix Is Nothing Then
            Call viewMatrix(data)
        End If
    End Sub

    Public Sub RunMassDiff(mz As Double, ms2 As Array)
        If Not runmassDiffAnalysis Is Nothing Then
            Call runmassDiffAnalysis(mz, ms2)
        End If
    End Sub
End Module
