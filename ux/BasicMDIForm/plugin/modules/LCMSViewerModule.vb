Public Module LCMSViewerModule

    Public lcmsViewerhHandle As Action(Of Object, Action(Of String))

    Public Sub OpenScatterViewer(data As Object, click As Action(Of String))
        If Not lcmsViewerhHandle Is Nothing Then
            Call lcmsViewerhHandle(data, click)
        End If
    End Sub

End Module
