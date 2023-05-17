Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports STImaging
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix

<Package("STImaging")>
Module STImaging

    <ExportAPI("ST_spaceranger.mzpack")>
    Public Function convertMzPack(spots As SpaceSpot(), matrix As Matrix) As mzPack
        Return spots.ST_spacerangerToMzPack(matrix)
    End Function
End Module
