#Region "Microsoft.VisualBasic::d7f6b8c258a25110bb1380ad32725304, E:/mzkit/src/mzkit/services/PipelineHost//Utils.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 120
    '    Code Lines: 92
    ' Comment Lines: 13
    '   Blank Lines: 15
    '     File Size: 4.69 KB


    ' Module Utils
    ' 
    '     Function: GetMs1Points, SpotConvertAsScans, ST_spacerangerToMzPack
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports STImaging

Public Module Utils

    <Extension>
    Public Function GetMs1Points(raw As IEnumerable(Of ScanMS1)) As ms1_scan()
        Return raw _
            .Select(Function(m1)
                        Return m1.mz _
                            .Select(Function(mzi, i)
                                        Return New ms1_scan With {
                                            .mz = mzi,
                                            .intensity = m1.into(i),
                                            .scan_time = m1.rt
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
    End Function

    <Extension>
    Private Iterator Function SpotConvertAsScans(spots As SpatialSpot(),
                                                 matrix As Matrix,
                                                 annotations As Dictionary(Of String, String)) As IEnumerable(Of ScanMS1)

        Dim geneID As String() = matrix.sampleID
        Dim spatial As Dictionary(Of String, Point) = spots _
            .ToDictionary(Function(p) p.barcode,
                          Function(p)
                              Return p.GetPoint
                          End Function)
        Dim mz As New List(Of Double)
        Dim anno As New List(Of String)
        Dim into As New List(Of Double)
        Dim geneIndex As Index(Of String) = geneID

        Call annotations.Clear()

        For Each v As KeyValuePair(Of String, Integer) In geneIndex.Map
            Call annotations.Add(v.Value.ToString, v.Key)
        Next

        For i As Integer = 0 To spots.Length - 1
            Dim spot = matrix.expression(i)
            Dim point = spatial(spot.geneID)
            Dim metadata As New Dictionary(Of String, String) From {
                {"x", point.X},
                {"y", point.Y},
                {mzStreamWriter.SampleMetaName, If(spots(i).flag > 0, "sample", "background")},
                {"ST-spot", $"{point.X},{point.Y}"}
            }

            ' length of experiments is equals to the geneIDs
            For j As Integer = 0 To geneID.Length - 1
                If spot.experiments(j) > 0 Then
                    Dim guid As Integer = geneIndex(geneID(j))

                    ' Call metadata.Add(j, geneID(j))
                    Call mz.Add(guid)
                    Call into.Add(spot.experiments(j))
                End If
            Next

            Dim hi_gene As Integer = which.Max(into)
            Dim max_guid As String = mz(hi_gene)

            Yield New ScanMS1 With {
                .mz = mz.ToArray,
                .into = into.ToArray,
                .scan_id = $"[MS1] [{point.X},{point.Y}] {spot.geneID} <{mz.Count} genes>; top_gene {annotations(max_guid)}={into(hi_gene)}",
                .meta = metadata
            }

            Call mz.Clear()
            Call into.Clear()
        Next
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="spots">The generated scan data element order 
    ''' keeps the same as this given spatial spot element orders.
    ''' </param>
    ''' <param name="matrix"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' the original spot location is tagged as ``ST-spot`` inside the metadata
    ''' </remarks>
    <Extension>
    Public Function ST_spacerangerToMzPack(spots As SpatialSpot(), matrix As Matrix) As mzPack
        Dim annotations As New Dictionary(Of String, String)
        Dim ms As ScanMS1() = spots.SpotConvertAsScans(matrix, annotations).ToArray
        Dim pixels As Point() = ms _
            .Select(Function(s) s.GetMSIPixel) _
            .ToArray _
            .DoCall(AddressOf STImaging.Render.ScaleSpots)

        For i As Integer = 0 To pixels.Length - 1
            ms(i).meta!x = pixels(i).X
            ms(i).meta!y = pixels(i).Y
        Next

        Return New mzPack With {
            .MS = ms.ToArray.ScalePixels(flip:=False),
            .source = matrix.tag,
            .Application = FileApplicationClass.STImaging,
            .Annotations = annotations
        }
    End Function
End Module
