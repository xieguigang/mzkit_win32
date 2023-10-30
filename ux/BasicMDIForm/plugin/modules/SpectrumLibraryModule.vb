Public Module SpectrumLibraryModule

    Public ReadOnly Property Repository As String
        Get
            Return $"{App.ProductProgramData}/lcms/"
        End Get
    End Property

    Public Function ScanLibraries() As IEnumerable(Of String)
        Return $"{App.ProductProgramData}/lcms/".ListFiles("*.lcms-pack")
    End Function

End Module
