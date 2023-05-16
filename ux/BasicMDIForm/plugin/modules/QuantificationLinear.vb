Public Interface QuantificationLinearPage

    ''' <summary>
    ''' Set reference files
    ''' </summary>
    ''' <param name="fileNames">a vector of the file full path</param>
    ''' <param name="type">MRM/GCMS_SIM</param>
    Sub RunLinearFileImports(fileNames As String(), type As TargetTypes?)

End Interface

Public Enum TargetTypes
    MRM
    GCMS_SIM
End Enum

Public Module QuantificationLinear

    Public Function ShowDocument() As QuantificationLinearPage
        Return Pages.OpenDocument(NameOf(QuantificationLinearPage))
    End Function
End Module
