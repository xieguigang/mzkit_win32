Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Imaging
Imports Task

Module ConvertToMzPack

    ''' <summary>
    ''' convert any kind of raw data file as mzPack
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="cacheFile"></param>
    Public Sub CreateMzpack(raw As String, cacheFile As String)
        Dim mzpack As mzPack

        If raw.ExtensionSuffix("raw") Then
            Using msraw As New MSFileReader(raw)
                mzpack = msraw.LoadFromXRaw(AddressOf RunSlavePipeline.SendMessage)
            End Using
        Else
            mzpack = Converter.LoadRawFileAuto(raw, "ppm:20", , AddressOf RunSlavePipeline.SendMessage)
        End If

        If Not mzpack.MS.IsNullOrEmpty Then
            RunSlavePipeline.SendMessage("Create snapshot...")
            mzpack.Thumbnail = mzpack.DrawScatter
        End If

        Using file As Stream = cacheFile.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call RunSlavePipeline.SendMessage("Write mzPack cache data...")
            Call mzpack.Write(file, version:=2)
        End Using

        Call Thread.Sleep(1500)
        Call RunSlavePipeline.SendMessage("Job Done!")
    End Sub
End Module
