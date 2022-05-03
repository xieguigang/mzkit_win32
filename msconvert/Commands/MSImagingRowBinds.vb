Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq

Module MSImagingRowBinds

    Private Function combineMzPack(pip As IEnumerable(Of mzPack), cor As Correction) As mzPack
        Return pip.MSICombineRowScans(cor, 0.05, progress:=AddressOf RunSlavePipeline.SendMessage)
    End Function

    Private Sub combineRaw(files As String(), file As Stream)
        Dim loadXRaw = Iterator Function() As IEnumerable(Of MSFileReader)
                           For Each path As String In files
                               If path.FileExists Then
                                   Using raw As New MSFileReader(path)
                                       Yield raw
                                   End Using

                                   Call RunSlavePipeline.SendMessage($"Measuring MSI Information... {path.BaseName}")
                               Else
                                   Call RunSlavePipeline.SendMessage($"Missing file in path: '{path}'!")
                               End If
                           Next
                       End Function
        Dim correction As Correction = MSIMeasurement.Measure(loadXRaw()).GetCorrection

        Call combineMzPack(
            Iterator Function() As IEnumerable(Of mzPack)
                Dim i As i32 = 0

                For Each path As String In files
                    Dim raw As New MSFileReader(path)
                    Dim cache As mzPack = raw.LoadFromXRaw

                    Yield cache

                    Try
                        raw.Dispose()
                    Catch ex As Exception
                    Finally
                        Call RunSlavePipeline.SendProgress(CInt((++i / files.Length) * 100), $"Combine Raw Data Files... {path.BaseName}")
                    End Try
                Next
            End Function(), correction).Write(file)
    End Sub

    Private Sub combineMzPack(files As String(), file As Stream)
        Dim loadRaw = Iterator Function() As IEnumerable(Of BinaryStreamReader)
                          For Each path As String In files
                              Using bin As New BinaryStreamReader(path)
                                  Yield bin
                              End Using

                              Call RunSlavePipeline.SendProgress(0, $"Measuring MSI Information... {path.BaseName}")
                          Next
                      End Function
        Dim correction As Correction = MSIMeasurement.Measure(loadRaw()).GetCorrection

        Call combineMzPack(
            Iterator Function() As IEnumerable(Of mzPack)
                Dim i As i32 = 0

                For Each path As String In files
                    Using buffer As Stream = path.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                        Call RunSlavePipeline.SendProgress(CInt((++i / files.Length) * 100), $"Combine Raw Data Files... {path.BaseName}")
                        Yield mzPack.ReadAll(buffer, ignoreThumbnail:=True)
                    End Using
                Next
            End Function(), correction).Write(file)
    End Sub

    <Extension>
    Public Sub MSI_rowbind(files As String(), save As String)
        Dim exttype As String() = files.Select(Function(path) path.ExtensionSuffix.ToLower).Distinct.ToArray

        If exttype.Length > 1 Then
            Call RunSlavePipeline.SendMessage($"Multipe file type is not allowed!")
            Return
        End If

        Using file As FileStream = save.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Select Case exttype(Scan0)
                Case "raw" : Call combineRaw(files, file)
                Case "mzpack" : Call combineMzPack(files, file)

                Case Else
                    Call RunSlavePipeline.SendMessage($"Unsupported file type: {exttype(Scan0)}!")
            End Select
        End Using
    End Sub
End Module
