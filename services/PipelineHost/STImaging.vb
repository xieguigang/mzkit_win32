Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports Microsoft.VisualBasic.CommandLine.Reflection
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
    Public Function mapGeneNames(<RRawVectorArgument> geneIds As Object, maps As list) As String()
        Dim ids As String() = CLRVector.asCharacter(geneIds)
        Dim map = ids _
            .Select(Function(i)
                        If maps.hasName(i) Then
                            Return CLRVector.asCharacter(maps.getByName(i)).ElementAtOrDefault(0, [default]:=i)
                        Else
                            Return i
                        End If
                    End Function) _
            .ToArray

        Return map
    End Function
End Module
