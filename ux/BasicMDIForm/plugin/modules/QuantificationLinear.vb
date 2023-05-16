Public Interface QuantificationLinearPage

    ''' <summary>
    ''' Set reference files
    ''' </summary>
    ''' <param name="fileNames">a vector of the file full path</param>
    ''' <param name="type">MRM/GCMS_SIM</param>
    Sub RunLinearFileImports(fileNames As String(), type As TargetTypes?)

    ''' <summary>
    ''' set linear reference in current profile table
    ''' </summary>
    ''' <param name="key"></param>
    ''' <param name="is_key"></param>
    ''' <param name="reference"></param>
    Sub SetLinear(key As String, is_key As String, reference As Dictionary(Of String, Double))

    ''' <summary>
    ''' create linear models under a given linear reference profile 
    ''' </summary>
    ''' <param name="profile">target linear reference profile name</param>
    Sub RunLinearRegression(profile As String)

    Sub LoadSampleFiles(FileNames As String())
    Sub ViewLinearModelReport()

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

    Public Function LinearProfileNames() As String()
        Return (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/") _
            .ListFiles("*.linearPack") _
            .Select(AddressOf BaseName) _
            .ToArray
    End Function

    Public Function ShowDocument() As QuantificationLinearPage
        Return Pages.OpenDocument(NameOf(QuantificationLinearPage))
    End Function

    Public Function ShowMRMLibrary() As MRMLibraryPage
        Return Pages.OpenDocument(NameOf(MRMLibraryPage))
    End Function
End Module
