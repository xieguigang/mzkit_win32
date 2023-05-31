Public Interface SpectrumSearchPage

    Sub LoadMs2(ms2 As Object)
    Sub RunSearch()

End Interface

Public Module SpectrumSearchModule

    Public Function ShowDocument() As SpectrumSearchPage
        Return Pages.OpenDocument(NameOf(SpectrumSearchPage))
    End Function

End Module