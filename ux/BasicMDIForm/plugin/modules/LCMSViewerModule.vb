Public Module LCMSViewerModule

    Public lcmsViewerhHandle As Action(Of Object, String, Action(Of String, Double, Double, Boolean))
    Public lcmsWorkspace As Func(Of IEnumerable)

    Public Sub OpenScatterViewer(data As Object, title As String, click As Action(Of String, Double, Double, Boolean))
        If Not lcmsViewerhHandle Is Nothing Then
            Call lcmsViewerhHandle(data, title, click)
        End If
    End Sub

    Public Function GetWorkspaceFiles() As IEnumerable
        If lcmsWorkspace IsNot Nothing Then
            Return lcmsWorkspace()
        Else
            Return Nothing
        End If
    End Function

End Module
