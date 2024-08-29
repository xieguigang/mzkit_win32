Public Module LCMSViewerModule

    Public lcmsViewerhHandle As Action(Of Object, String, Action(Of String, Double, Double, Boolean))
    Public lcmsWorkspace As Func(Of IEnumerable)
    Public lcmsChromatogramOverlaps As Action(Of Object)

    ''' <summary>
    ''' view of the lcms scatter viewer
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="title"></param>
    ''' <param name="click"></param>
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

    Public Sub ShowTICOverlaps(overlaps As Object)
        If lcmsChromatogramOverlaps IsNot Nothing Then
            Call lcmsChromatogramOverlaps(overlaps)
        End If
    End Sub

End Module
