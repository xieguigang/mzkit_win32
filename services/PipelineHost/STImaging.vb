Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports STImaging
Imports STRaid
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

<Package("STImaging")>
Module STImagingTools

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
        Dim tissue As TissueRegion()

        exp.source = tag

        Return New list With {
            .slots = New Dictionary(Of String, Object) From {
                {"gene_exprs", exp},
                {"tissue", tissue}
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
