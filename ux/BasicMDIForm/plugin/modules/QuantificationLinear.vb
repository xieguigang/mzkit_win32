Public Interface QuantificationLinearPage

End Interface

Public Module QuantificationLinear

    Public Function ShowDocument() As QuantificationLinearPage
        Return Pages.OpenDocument(NameOf(QuantificationLinearPage))
    End Function
End Module
