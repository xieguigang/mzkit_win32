Public Interface SpectrumSearchPage

    Sub LoadMs2(ms2 As Object)
    Sub RunSearch()

End Interface

Public Module SpectrumSearchModule

    Dim documentType As Type

    Public Sub SetDocument(type As Type)
        documentType = type
    End Sub

    Public Function ShowDocument() As SpectrumSearchPage
        Return CObj(Workbench.ShowDocument(documentType))
    End Function

End Module