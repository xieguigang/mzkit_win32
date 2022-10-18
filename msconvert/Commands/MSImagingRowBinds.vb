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
        Return pip.MSICombineRowScans(
            correction:=cor,
            intocutoff:=0.0,
            sumNorm:=False,
            yscale:=1,
            progress:=AddressOf RunSlavePipeline.SendMessage
        )
    End Function

    Private Iterator Function loadXRaw(files As IEnumerable(Of String)) As IEnumerable(Of MSFileReader)
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

    Private Sub combineRaw(files As String(), file As Stream)
        Dim correction As Correction = MSIMeasurement.Measure(loadXRaw(files)).GetCorrection
        Call combineMzPack(LoadThermoRaw(files), correction).Write(file, version:=2)
    End Sub

    Private Iterator Function LoadThermoRaw(files As String()) As IEnumerable(Of mzPack)
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
    End Function

    Private Sub combineWiffRaw(files As String(), file As Stream)
        Dim rawfiles As New List(Of mzPack)
        Dim i As i32 = 0
        Dim println As Action(Of String)

        For Each wiff As String In files
            Call RunSlavePipeline.SendProgress(CInt((++i / files.Length) * 100), $"Load wiff waw data files... {wiff.BaseName}")

            println = Sub(msg)
                          Call RunSlavePipeline.SendProgress(CInt((++i / files.Length) * 100), $"[{wiff.BaseName}] {msg}")
                      End Sub

            Dim wiffRaw As New sciexWiffReader.WiffScanFileReader(wiff)
            Dim mzPack As mzPack = wiffRaw.LoadFromWiffRaw(println)

            Call rawfiles.Add(mzPack)
        Next

        Call RunSlavePipeline.SendMessage($"Measuring MSI Information...")

        Dim correction As Correction = MSIMeasurement.Measure(rawfiles).GetCorrection

        Call RunSlavePipeline.SendMessage($"Combine MS-imaging file!")
        Call combineMzPack(rawfiles, correction).Write(file, version:=2)
    End Sub

    Private Iterator Function loadRaw(files As IEnumerable(Of String)) As IEnumerable(Of BinaryStreamReader)
        For Each path As String In files
            Using bin As New BinaryStreamReader(path)
                Yield bin
            End Using

            Call RunSlavePipeline.SendProgress(0, $"Measuring MSI Information... {path.BaseName}")
        Next
    End Function

    Private Sub combineMzPack(files As String(), file As Stream)
        Dim correction As Correction = MSIMeasurement.Measure(loadRaw(files)).GetCorrection

        Call combineMzPack(
            Iterator Function() As IEnumerable(Of mzPack)
                Dim i As i32 = 0

                For Each path As String In files
                    Using buffer As Stream = path.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                        Call RunSlavePipeline.SendProgress(CInt((++i / files.Length) * 100), $"Combine Raw Data Files... {path.BaseName}")
                        Yield mzPack.ReadAll(buffer, ignoreThumbnail:=True)
                    End Using
                Next
            End Function(), correction).Write(file, version:=2)
    End Sub

    <Extension>
    Public Sub MSI_rowbind(files As String(), save As String)
        Dim exttype As String() = (From path As String
                                   In files
                                   Select path.ExtensionSuffix.ToLower
                                   Distinct).ToArray

        If exttype.Length > 1 Then
            Call RunSlavePipeline.SendMessage($"Multipe file type is not allowed!")
            Return
        End If

        Using file As FileStream = save.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Select Case exttype(Scan0)
                Case "raw" : Call combineRaw(files, file)
                Case "mzpack" : Call combineMzPack(files, file)
                Case "wiff" : Call combineWiffRaw(files, file)

                Case Else
                    Call RunSlavePipeline.SendMessage($"Unsupported file type: {exttype(Scan0)}!")
            End Select
        End Using
    End Sub
End Module
