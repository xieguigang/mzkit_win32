Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports STImaging
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

<Package("STImaging")>
Module STImagingTools

    <ExportAPI("ST_spaceranger.mzpack")>
    Public Function convertMzPack(spots As SpaceSpot(), matrix As Matrix) As mzPack
        Return spots.ST_spacerangerToMzPack(matrix)
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
