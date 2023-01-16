Public Module MSImagingServiceModule

    Friend m_StartEngine As Action

    Public Property ServiceEngine As MSIServicePlugin

    Public Sub StartEngine()
        Call CloseEngine()

        If Not m_StartEngine Is Nothing Then
            Call m_StartEngine()
        End If
    End Sub

    Public Sub CloseEngine()
        If Not MSImagingServiceModule.ServiceEngine Is Nothing Then
            Call MSImagingServiceModule.ServiceEngine.CloseEngine()
        End If
    End Sub
End Module
