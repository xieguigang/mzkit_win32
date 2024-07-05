Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports RibbonLib.Interop
Imports TaskStream

Module ggplotVisual

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <returns></returns>
    Friend Function encodeJSON(data As TissueRegion()) As String
        Dim json As New JsonObject
        Dim sample As JsonObject

        For Each tissue_group As TissueRegion In data
            sample = New JsonObject
            sample.Add("color", New JsonValue(tissue_group.color.ToHtmlColor))
            sample.Add("data", New JsonArray(tissue_group.tags))
            sample.Add("x", New JsonArray(tissue_group.points.Select(Function(t) t.X)))
            sample.Add("y", New JsonArray(tissue_group.points.Select(Function(t) t.Y)))
            json.Add(tissue_group.label, sample)
        Next

        Return json.BuildJsonString
    End Function

    Friend Function encodeJSON(data As Dictionary(Of String, (color As String, exp As Double()))) As String
        Dim json As New JsonObject
        Dim sample As JsonObject

        For Each sample_group In data.SafeQuery
            sample = New JsonObject
            sample.Add("color", New JsonValue(sample_group.Value.color))
            sample.Add("data", New JsonArray(sample_group.Value.exp))
            json.Add(sample_group.Key, sample)
        Next

        Return json.BuildJsonString
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function ggplot(pack As String, title As String, size As Integer(), Optional type As String = "violin|box|bar") As Image
        Return RscriptProgressTask.PlotStats(pack,
             type:=type.Split("|"c).First,
             title:=title.Replace(""""c, "'"),
             size:=size.JoinBy(","))
    End Function
End Module
