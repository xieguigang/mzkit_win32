Imports System.Text
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG
Imports SMRUCC.genomics.Visualize.CatalogProfiling
Imports any = Microsoft.VisualBasic.Scripting
Imports stdNum = System.Math

Public Class KEGGEnrichmentBarSummary : Inherits SummaryPlot

    Public Overrides ReadOnly Property requiredFields As Dictionary(Of String(), String)
        Get
            Dim list As New Dictionary(Of String(), String)

            list({"term"}) = "the kegg pathway map id vector."
            list({"pvalue"}) = "the fisher enrichment analysis pvalue result."

            Return list
        End Get
    End Property

    Public Overrides ReadOnly Property appName As String
        Get
            Return "KEGG enrichment bar"
        End Get
    End Property

    Public Overrides Function ToString() As String
        Dim sb As New StringBuilder(appName & vbCrLf)
        sb.AppendLine()

        For Each item In requiredFields
            Call sb.AppendLine($"{item.Key.JoinBy(", ")}: {item.Value}")
        Next

        Return sb.ToString
    End Function

    Public Overrides Function Plot(table As DataTable) As Image
        Dim term As String() = getFieldVector(table, {"term"}).AsObjectEnumerator.Select(AddressOf any.ToString).ToArray
        Dim pvalue As Double() = getFieldVector(table, {"pvalue"}) _
            .AsObjectEnumerator _
            .Select(Function(o) -stdNum.Log10(Val(o))) _
            .ToArray
        Dim raw = term.SeqIterator.ToDictionary(Function(f) f.value, Function(i) pvalue(i))
        Dim profiles = raw.DoKeggProfiles(5)

        Return profiles.ProfilesPlot(
            title:="KEGG Enrichment",
            size:="2300,2000",
            tick:=-1,
            axisTitle:="-log10(pvalue)",
            labelRightAlignment:=False,
            valueFormat:="F2",
            colorSchema:="#E41A1C,#377EB8,#4DAF4A,#984EA3,#FF7F00,#CECE00",
            dpi:=300
        ).AsGDIImage
    End Function
End Class
