#Region "Microsoft.VisualBasic::934792fd0478867f4700a2341119a837, mzkit\src\mzkit\services\PipelineHost\Utils.vb"

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

    '   Total Lines: 90
    '    Code Lines: 80
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 3.26 KB


    ' Module Utils
    ' 
    '     Function: GetMs1Points, ST_spacerangerToMzPack
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
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
    Public Function ST_spacerangerToMzPack(spots As SpaceSpot(), matrix As Matrix) As mzPack
        Dim ms As New List(Of ScanMS1)
        Dim spatial As Dictionary(Of String, Point) = spots _
            .ToDictionary(Function(p) p.barcode,
                          Function(p)
                              Return p.GetPoint
                          End Function)
        Dim spot As DataFrameRow
        Dim point As Point
        Dim scan As ScanMS1
        Dim mz As New List(Of Double)
        Dim anno As New List(Of String)
        Dim into As New List(Of Double)
        Dim metadata As Dictionary(Of String, String)
        Dim geneID As String() = matrix.sampleID

        For i As Integer = 0 To spots.Length - 1
            spot = matrix.expression(i)
            point = spatial(spot.geneID)
            metadata = New Dictionary(Of String, String) From {
                {"x", point.X},
                {"y", point.Y}
            }

            For j As Integer = 0 To geneID.Length - 1
                If spot.experiments(j) > 0 Then
                    Call metadata.Add(j, geneID(j))
                    Call mz.Add(j)
                    Call into.Add(spot.experiments(j))
                End If
            Next

            scan = New ScanMS1 With {
                .mz = mz.ToArray,
                .into = into.ToArray,
                .scan_id = $"[MS1] [{point.X},{point.Y}] {spot.geneID}",
                .meta = metadata
            }

            Call mz.Clear()
            Call into.Clear()
            Call ms.Add(scan)
        Next

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
            .Application = FileApplicationClass.MSImaging
        }
    End Function
End Module
