Public Interface QuantificationLinearPage

    ''' <summary>
    ''' Set reference files
    ''' </summary>
    ''' <param name="fileNames">a vector of the file full path</param>
    ''' <param name="type">MRM/GCMS_SIM</param>
    Sub RunLinearFileImports(fileNames As String(), type As TargetTypes?)

End Interface

Public Interface MRMLibraryPage

    Sub SaveLibrary()
    Sub Add(id As String, name As String, q1 As Double, q2 As Double, rt As Double)

End Interface

Public Enum TargetTypes
    MRM
    GCMS_SIM
End Enum

Public Module QuantificationLinear

    Public Function ShowDocument() As QuantificationLinearPage
        Return Pages.OpenDocument(NameOf(QuantificationLinearPage))
    End Function

    Public Function ShowMRMLibrary() As MRMLibraryPage
        Return Pages.OpenDocument(NameOf(MRMLibraryPage))
    End Function
End Module
