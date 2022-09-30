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

    Public Overrides Function Plot(table As DataTable) As Image
        Dim term As String() = getFieldVector(table, {"term"}).AsObjectEnumerator.Select(AddressOf any.ToString).ToArray
        Dim pvalue As Double() = getFieldVector(table, {"pvalue"}) _
            .AsObjectEnumerator _
            .Select(Function(o)
                        Return Val(o)
                    End Function) _
            .ToArray

        If pvalue.Length > 0 Then
            pvalue = pvalue _
                .Select(Function(p)
                            If p <= 0 Then
                                p = pvalue.Where(Function(pi) pi > 0).First
                            End If

                            Return -stdNum.Log10(p)
                        End Function) _
                .ToArray
        End If

        Dim raw As Dictionary(Of String, Double) = term _
            .SeqIterator _
            .ToDictionary(Function(f) f.value,
                          Function(i)
                              Return pvalue(i)
                          End Function)
        Dim profiles = raw.DoKeggProfiles(7)

        Return profiles.ProfilesPlot(
            title:="KEGG Enrichment",
            size:="2400,2000",
            tick:=-1,
            axisTitle:="-log10(pvalue)",
            labelRightAlignment:=False,
            valueFormat:="F2",
            colorSchema:="#E41A1C,#377EB8,#4DAF4A,#984EA3,#FF7F00,#CECE00",
            dpi:=100
        ).AsGDIImage
    End Function
End Class
