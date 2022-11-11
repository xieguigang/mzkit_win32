Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.DataStorage.netCDF.Components
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Patterns
Imports stdNum = System.Math

Public Class MSImagingRowBinds

    Dim cutoff As Double
    Dim basePeak As Double
    Dim labelPrefix As String

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function combineMzPack(pip As IEnumerable(Of mzPack), cor As Correction) As mzPack
        Return pip.MSICombineRowScans(
            correction:=cor,
            intocutoff:=cutoff,
            sumNorm:=False,
            yscale:=1,
            progress:=AddressOf RunSlavePipeline.SendMessage,
            labelPrefix:=labelPrefix
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

    Private Function CutBasePeak(raw As mzPack) As mzPack
        If basePeak <= 0 Then
            Return raw
        End If

        For i As Integer = 0 To raw.MS.Length - 1
            Dim filter = raw.MS(i) _
                .GetMs _
                .Where(Function(d) stdNum.Abs(d.mz - basePeak) > 0.3) _
                .ToArray
            Dim mz As Double() = filter.Select(Function(mzi) mzi.mz).ToArray
            Dim into As Double() = filter.Select(Function(mzi) mzi.intensity).ToArray

            raw.MS(i).mz = mz
            raw.MS(i).into = into
        Next

        Return raw
    End Function

    Private Iterator Function LoadThermoRaw(files As String()) As IEnumerable(Of mzPack)
        Dim i As i32 = 0

        For Each path As String In files
            Dim raw As New MSFileReader(path)
            Dim cache As mzPack = raw.LoadFromXRaw

            Yield CutBasePeak(cache)

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

        For Each wiff As String In files ' .Take(3)
            Dim p As Integer = CInt((++i / files.Length) * 100)

            println = Sub(msg) RunSlavePipeline.SendProgress(p, $"[{wiff.BaseName}] {msg}")
            println($"Load wiff waw data files... {wiff.BaseName}")

            ' 20221018 if check noise, then maybe loose too much ions
            Dim wiffRaw As New sciexWiffReader.WiffScanFileReader(wiff)
            Dim mzPack As mzPack = wiffRaw.LoadFromWiffRaw(checkNoise:=False, println:=println)

            Call rawfiles.Add(CutBasePeak(mzPack))
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
                        Yield CutBasePeak(mzPack.ReadAll(buffer, ignoreThumbnail:=True))
                    End Using
                Next
            End Function(), correction).Write(file, version:=2)
    End Sub

    Public Shared Sub MSI_rowbind(files As String(), save As String, cutoff As Double, basePeak As Double)
        Dim exttype As String() = (From path As String
                                   In files
                                   Select path.ExtensionSuffix.ToLower
                                   Distinct).ToArray
        Dim sampleTag As New CommonTagParser(files.Select(Function(path) path.BaseName))
        Dim union As New MSImagingRowBinds With {
            .basePeak = basePeak,
            .cutoff = cutoff,
            .labelPrefix = sampleTag.LabelPrefix
        }

        If exttype.Length > 1 Then
            Call RunSlavePipeline.SendMessage($"Multipe file type is not allowed!")
            Return
        End If

        Using file As FileStream = save.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Select Case exttype(Scan0)
                Case "raw" : Call union.combineRaw(files, file)
                Case "mzpack" : Call union.combineMzPack(files, file)
                Case "wiff" : Call union.combineWiffRaw(files, file)

                Case Else
                    Call RunSlavePipeline.SendMessage($"Unsupported file type: {exttype(Scan0)}!")
            End Select
        End Using
    End Sub
End Class
