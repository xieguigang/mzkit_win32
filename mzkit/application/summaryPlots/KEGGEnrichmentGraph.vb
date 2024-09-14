Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports any = Microsoft.VisualBasic.Scripting

Public Class KEGGEnrichmentGraph : Inherits SummaryPlot

    Public Overrides ReadOnly Property requiredFields As Dictionary(Of String(), String)
        Get
            Dim list As New Dictionary(Of String(), String)

            list({"term"}) = "the kegg pathway map id vector."
            list({"name"}) = "the kegg pathway names."
            list({"IDs"}) = "the fisher enrichment hits in a pathway cluster."
            list({"pvalue"}) = "the enrichment p-value."

            Return list
        End Get
    End Property

    Public Overrides ReadOnly Property appName As String
        Get
            Return "KEGG Enrichment Graph"
        End Get
    End Property

    Public Overrides Function Plot(table As DataTable) As Object
        Dim term As String() = getFieldVector(table, {"term"}).AsObjectEnumerator.Select(AddressOf any.ToString).ToArray
        Dim name As String() = getFieldVector(table, {"name"}).AsObjectEnumerator.Select(AddressOf any.ToString).ToArray
        Dim pvalue As Double() = CLRVector.asNumeric(getFieldVector(table, {"pvalue", "p-value"}))
        Dim geneSet As String()() = getFieldVector(table, {"IDs"}) _
            .AsObjectEnumerator _
            .Select(AddressOf any.ToString) _
            .Select(Function(r) r.StringSplit("[,;]\s+")) _
            .ToArray
        Dim g As NetworkGraph = Nothing

        InputDialog.Input(Of InputConfigEnrichmentGraph)(
            Sub(config)
                Dim terms As EnrichmentResult() = New EnrichmentResult(term.Length - 1) {}
                Dim fdr_cut As Double = config.fdr

                For i As Integer = 0 To term.Length - 1
                    terms(i) = New EnrichmentResult With {
                        .term = term(i),
                        .name = name(i),
                        .pvalue = pvalue(i),
                        .IDs = geneSet(i)
                    }
                Next

                terms = terms.OrderBy(Function(p) p.pvalue).ToArray
                terms = terms.FDRCorrection _
                    .Where(Function(f) f.FDR < fdr_cut) _
                    .OrderBy(Function(f) f.FDR) _
                    .Take(config.topN) _
                    .ToArray

                g = CreateGraph(terms)
            End Sub)

        Return g
    End Function

    Protected Function CreateGraph(terms As EnrichmentResult()) As NetworkGraph
        Dim g As New NetworkGraph
        Dim geneset = terms.Select(Function(a) a.IDs).ToArray

        For i As Integer = 0 To terms.Length - 1
            Call g.CreateNode(terms(i).term, New NodeData With {
                              .origID = terms(i).term,
                              .label = terms(i).name,
                              .color = Brushes.Blue,
                              .Properties = New Dictionary(Of String, String) From {
                {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "pathway"}
            }})
        Next
        For Each id As String In geneSet.IteratesALL.Distinct
            Call g.CreateNode(id, New NodeData With {
                              .color = Brushes.Red,
                              .Properties = New Dictionary(Of String, String) From {
                {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, "cluster_member"}
            }})
        Next

        For i As Integer = 0 To terms.Length - 1
            Dim mapUNode = g.GetElementByID(terms(i).term)

            For Each id As String In geneset(i)
                Call g.CreateEdge(mapUNode, g.GetElementByID(id))
            Next
        Next

        Call g.ApplyAnalysis
        Call g.RemovesIsolatedNodes()

        Return g
    End Function
End Class
