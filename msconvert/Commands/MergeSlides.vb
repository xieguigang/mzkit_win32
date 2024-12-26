Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Public Module MergeSlides

    Private Function LoadRaw(path As String, fileNameAsSourceTag As Boolean) As NamedValue(Of mzPack)
        Call RunSlavePipeline.SendMessage($"read {path}...")

        Using buf As Stream = path.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim data = mzPack.ReadAll(buf, ignoreThumbnail:=True, skipMsn:=True)

            If fileNameAsSourceTag Then
                data.source = path.BaseName
            End If

            Return New NamedValue(Of mzPack)(path.BaseName, data)
        End Using
    End Function

    ''' <summary>
    ''' merge multiple ms-imaging slide
    ''' </summary>
    ''' <param name="file"></param>
    ''' <param name="layout"></param>
    ''' <param name="fileNameAsSourceTag"></param>
    ''' <returns></returns>
    Public Function JoinDataSet(file As IEnumerable(Of String), layout As String, fileNameAsSourceTag As Boolean,
                                Optional ByRef offsets As Dictionary(Of String, Integer()) = Nothing) As mzPack

        Call VBDebugger.EchoLine("load all ms-imaging rawdata file into memory.")

        Dim rawfiles As Dictionary(Of String, mzPack) = file _
            .Select(Function(path) LoadRaw(path, fileNameAsSourceTag)) _
            .ToDictionary(Function(path) path.Name,
                          Function(path)
                              Return path.Value
                          End Function)
        Dim total As Long = Aggregate raw As mzPack In rawfiles.Values Into Sum(CLng(raw.MS.TryCount))

        Call VBDebugger.EchoLine("load rawdata job done!")
        Call VBDebugger.EchoLine("view rawdata scan information:")
        Call VBDebugger.EchoLine(rawfiles.ToDictionary(Function(a) a.Key, Function(a) a.Value.MS.TryCount).GetJson)
        Call VBDebugger.EchoLine("total rawdata scan numbers:")
        Call VBDebugger.EchoLine(total)

        If layout.StringEmpty Then
            VBDebugger.EchoLine("no layout information input, going to merge data in linear!")

            If rawfiles.Values.Any(Function(m) m.Application = FileApplicationClass.STImaging) Then
                Return MergeFakeSTImagingSliders.JoinSTImagingSamples(rawfiles.Values, println:=AddressOf RunSlavePipeline.SendMessage)
            Else
                ' merge data in linear
                Return rawfiles.Values.JoinMSISamples(println:=AddressOf RunSlavePipeline.SendMessage)
            End If
        Else
            Dim layoutData As String()() = layout _
                .SolveStream _
                .LineTokens _
                .Select(Function(line) line.Split(","c)) _
                .ToArray

            Call VBDebugger.EchoLine("merge rawdata with layout information:")

            For Each line As String() In layoutData
                Call VBDebugger.EchoLine(line.GetJson)
            Next

            If rawfiles.Values.Any(Function(m) m.Application = FileApplicationClass.STImaging) Then
                Return MergeFakeSTImagingSliders.MergeDataWithLayout(rawfiles, layoutData)
            Else
                Return rawfiles.MergeDataWithLayout(layoutData, offsets:=offsets)
            End If
        End If
    End Function
End Module