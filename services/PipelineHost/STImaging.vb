#Region "Microsoft.VisualBasic::d4763a6ce614ae469c3983103e763232, G:/mzkit/src/mzkit/services/PipelineHost//STImaging.vb"

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

    '   Total Lines: 123
    '    Code Lines: 108
    ' Comment Lines: 0
    '   Blank Lines: 15
    '     File Size: 4.87 KB


    ' Module STImagingTools
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: convertMzPack, extract_h5ad, mapGeneNames
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports STImaging
Imports STRaid
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports std = System.Math

<Package("STImaging")>
Module STImagingTools

    Sub New()
        VectorTask.n_threads = std.Max(8, App.CPUCoreNumbers)
    End Sub

    <ExportAPI("ST_spaceranger.mzpack")>
    Public Function convertMzPack(spots As SpatialSpot(), matrix As Matrix, Optional tag As String = Nothing) As mzPack
        Dim pack = spots.ST_spacerangerToMzPack(matrix)

        If Not tag.StringEmpty Then
            pack.source = tag
        End If

        Return pack
    End Function

    <ExportAPI("extract_h5ad")>
    <RApiReturn("gene_exprs", "tissue")>
    Public Function extract_h5ad(raw As AnnData, Optional env As Environment = Nothing) As Object
        Dim tag As String = raw.source
        Dim matrix As Matrix = raw.ExportExpression
        Dim spots As SpatialSpot() = raw.obsm.spatial _
            .Select(Function(si)
                        Return New SpatialSpot With {
                            .px = si.X,
                            .py = si.Y,
                            .x = .px,
                            .y = .py,
                            .flag = 1,
                            .barcode = $"{ .x},{ .y}"
                        }
                    End Function) _
            .ToArray

        If matrix.sampleID.IsNullOrEmpty Then
            matrix.sampleID = If(
                raw.var.gene_ids.IsNullOrEmpty,
                raw.var.gene_short_name,
                raw.var.gene_ids
            )
        End If

        Dim exp As mzPack = spots.ST_spacerangerToMzPack(matrix)
        Dim tissue As New List(Of TissueRegion)
        Dim labels = raw.obs.class_labels
        Dim colors = Designer.GetColors("paper", labels.Length)

        For i As Integer = 0 To spots.Length - 1
            spots(i).barcode = labels(raw.obs.clusters(i))
            spots(i).x = exp.MS(i).meta!x
            spots(i).y = exp.MS(i).meta!y
            exp.MS(i).meta(mzStreamWriter.SampleMetaName) = spots(i).barcode
        Next

        tissue.AddRange(spots _
             .GroupBy(Function(t) t.barcode) _
             .Select(Function(region, i)
                         Return New TissueRegion With {
                             .label = region.Key,
                             .color = colors(i),
                             .points = region.Select(Function(p) New Point(p.x, p.y)).ToArray,
                             .tags = region.Key.Replicate(.points.Length).ToArray
                         }
                     End Function))
        exp.source = tag

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"gene_exprs", exp},
                {"tissue", tissue.ToArray}
            }
        }
    End Function

    <ExportAPI("map_geneNames")>
    Public Function mapGeneNames(<RRawVectorArgument> geneIds As Object, maps As list,
                                 <RRawVectorArgument>
                                 Optional target_names As Object = Nothing) As String()

        Dim targets As Index(Of String) = CLRVector.asCharacter(target_names).Indexing
        Dim ids As String() = CLRVector.asCharacter(geneIds)
        Dim map = ids _
            .Select(Function(i)
                        If maps.hasName(i) Then
                            Dim geneNames As String() = CLRVector.asCharacter(maps.getByName(i))

                            If geneNames.IsNullOrEmpty Then
                                Return i
                            ElseIf geneNames.Length = 1 Then
                                Return geneNames(0)
                            ElseIf targets.Count > 0 AndAlso geneNames.Any(Function(name) name Like targets) Then
                                Return geneNames.Where(Function(name) name Like targets).First
                            Else
                                Return geneNames(0)
                            End If
                        Else
                            Return i
                        End If
                    End Function) _
            .ToArray

        Return map
    End Function
End Module
