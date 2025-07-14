﻿Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Interface QuantificationLinearPage

    ''' <summary>
    ''' Set reference files
    ''' </summary>
    ''' <param name="fileNames">a vector of the file full path</param>
    ''' <param name="type">MRM/GCMS_SIM</param>
    Sub RunLinearFileImports(fileNames As Array, type As TargetTypes?)
    Sub RunLinearmzPackImports(cals As String(), mzpack As Object)

    ''' <summary>
    ''' set the sample names of the cals points
    ''' </summary>
    ''' <param name="filenames"></param>
    Sub SetCals(filenames As NamedValue(Of String)())

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

    Sub SetSampleNames(names As IEnumerable(Of String))
    Sub LoadSampleFiles(FileNames As Array, echo As Action(Of String))
    Sub LoadSampleMzpack(samples As String(), mzpack As Object, echo As Action(Of String))

    Sub ViewLinearModelReport(onHost As Boolean, ignoreErr As Boolean)

    Function PullQuantifyResult() As IEnumerable(Of NamedValue(Of DynamicPropertyBase(Of Double)))

End Interface

Public Interface DocumentPageLoader : Inherits IFileReference

    Property AutoSaveOnClose As Boolean

    Function LoadDocument(file As String) As Boolean

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
            .ListFiles("*.csv") _
            .Select(AddressOf BaseName) _
            .ToArray
    End Function

    Public Function ShowDocument() As QuantificationLinearPage
        Return Pages.OpenDocument(NameOf(QuantificationLinearPage))
    End Function

    Public Function ShowMRMLibrary() As MRMLibraryPage
        Return Pages.OpenDocument(NameOf(MRMLibraryPage))
    End Function

    Public Function LinearTableEditor() As DocumentPageLoader
        Return Pages.OpenDocument(NameOf(QuantificationLinear.LinearTableEditor))
    End Function
End Module
