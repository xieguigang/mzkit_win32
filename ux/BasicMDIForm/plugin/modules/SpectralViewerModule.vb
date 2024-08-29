Public Module SpectralViewerModule

    ''' <summary>
    ''' view spectrum handler, the parameter 1 should be the spectrum data object, 
    ''' and parameter 2 is optional, should be the formula for make the 
    ''' annotation
    ''' </summary>
    Dim viewMatrix As Action(Of Object, String)
    Dim runmassDiffAnalysis As Action(Of Double, Array)
    Dim showClusterSpectrum As Action(Of Array, String)

    ''' <summary>
    ''' hook the spectral matrix viewer
    ''' </summary>
    ''' <param name="view"></param>
    Public Sub HookViewer(view As Action(Of Object, String))
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

    Public Sub ViewSpectral(data As Object, Optional formula As String = Nothing)
        If Not viewMatrix Is Nothing Then
            Call viewMatrix(data, formula)
        End If
    End Sub

    Public Sub RunMassDiff(mz As Double, ms2 As Array)
        If Not runmassDiffAnalysis Is Nothing Then
            Call runmassDiffAnalysis(mz, ms2)
        End If
    End Sub
End Module
