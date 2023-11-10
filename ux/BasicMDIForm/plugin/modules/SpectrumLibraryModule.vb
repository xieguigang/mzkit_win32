Imports System.Runtime.CompilerServices

Public Module SpectrumLibraryModule

    Public ReadOnly Property Repository As String
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return $"{App.ProductProgramData}/lcms/"
        End Get
    End Property

    ''' <summary>
    ''' populate the file path of the library files
    ''' </summary>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ScanLibraries() As IEnumerable(Of String)
        Return $"{App.ProductProgramData}/lcms/".ListFiles("*.lcms-pack")
    End Function

    ''' <summary>
    ''' The file extension name should be ``*.lcms-pack``
    ''' </summary>
    ''' <param name="libpack">xxxxx.lcms-pack</param>
    ''' <returns></returns>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function LibraryFile(libpack As String) As String
        If libpack.ExtensionSuffix <> "lcms-pack" Then
            libpack = libpack & ".lcms-pack"
        End If

        Return $"{Repository}/{libpack}"
    End Function

End Module
