Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Linq
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

        Return CreateGraph(term, name, pvalue, geneSet)
    End Function

    Protected Function CreateGraph(term As String(), name As String(), pvalue As Double(), geneSet As String()()) As NetworkGraph
        Dim g As New NetworkGraph
        Dim pval_cut As Double = 0.01

        For i As Integer = 0 To term.Length - 1
            If pvalue(i) > pval_cut Then
                Continue For
            End If

            Call g.CreateNode(term(i), New NodeData With {
                              .origID = term(i),
                              .label = name(i),
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

        For i As Integer = 0 To term.Length - 1
            If pvalue(i) > pval_cut Then
                Continue For
            End If

            Dim mapUNode = g.GetElementByID(term(i))

            For Each id As String In geneSet(i)
                Call g.CreateEdge(mapUNode, g.GetElementByID(id))
            Next
        Next

        Call g.ApplyAnalysis
        Call g.RemovesIsolatedNodes()

        Return g
    End Function
End Class
