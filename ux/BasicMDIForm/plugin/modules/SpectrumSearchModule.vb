Public Interface SpectrumSearchPage

    ''' <summary>
    ''' the spectrum title is generated based on the different <paramref name="ms2"/> object type
    ''' </summary>
    ''' <param name="ms2"></param>
    Sub LoadMs2(ms2 As Object)
    Sub RunSearch(Optional showUI As Boolean = True)

End Interface

''' <summary>
''' Helper module for plugin access the spectrum search and alignment data visualization
''' </summary>
Public Module SpectrumSearchModule

    Public AlignmentViewer As Action(Of Object, String, String)

    ''' <summary>
    ''' show the spectrum search result
    ''' </summary>
    ''' <returns></returns>
    Public Function ShowDocument() As SpectrumSearchPage
        Return Pages.OpenDocument(NameOf(SpectrumSearchPage))
    End Function

    ''' <summary>
    ''' Data visual of the spectrum alignment result
    ''' </summary>
    ''' <param name="alignment">the spectrum alignment details</param>
    ''' <param name="query">the metadata of sample query data</param>
    ''' <param name="ref">the metadata of reference subject data.</param>
    Public Sub ViewAlignment(alignment As Object, query As String, ref As String)
        If Not AlignmentViewer Is Nothing Then
            Call AlignmentViewer(alignment, query, ref)
        End If
    End Sub

End Module