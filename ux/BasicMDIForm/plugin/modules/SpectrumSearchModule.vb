Public Interface SpectrumSearchPage

    ''' <summary>
    ''' the spectrum title is generated based on the different <paramref name="ms2"/> object type
    ''' </summary>
    ''' <param name="ms2"></param>
    Sub LoadMs2(ms2 As Object)
    Sub RunSearch(Optional showUI As Boolean = True)

End Interface

Public Module SpectrumSearchModule

    Public Function ShowDocument() As SpectrumSearchPage
        Return Pages.OpenDocument(NameOf(SpectrumSearchPage))
    End Function

End Module