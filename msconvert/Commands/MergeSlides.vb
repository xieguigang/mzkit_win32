Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline

Public Module MergeSlides

    Public Function JoinDataSet(file As IEnumerable(Of String), layout As String, fileNameAsSourceTag As Boolean) As mzPack
        Dim rawfiles As Dictionary(Of String, mzPack) = file _
            .ToDictionary(Function(path) path.BaseName,
                          Function(path)
                              Using buf As Stream = path.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                                  Dim data = mzPack.ReadAll(buf, ignoreThumbnail:=True, skipMsn:=True)
                                  If fileNameAsSourceTag Then
                                      data.source = path.BaseName
                                  End If
                                  Return data
                              End Using
                          End Function)

        If layout.StringEmpty Then
            ' merge data in linear
            Return rawfiles.Values.JoinMSISamples(println:=AddressOf RunSlavePipeline.SendMessage)
        Else
            Dim layoutData As String()() = layout _
                .SolveStream _
                .LineTokens _
                .Select(Function(line) line.Split(","c)) _
                .ToArray

            Return rawfiles.MergeDataWithLayout(layoutData)
        End If
    End Function

    <Extension>
    Public Function MergeDataWithLayout(raw As Dictionary(Of String, mzPack), layout As String()()) As mzPack

    End Function
End Module