Public Class KEGGEnrichmentBarSummary : Inherits SummaryPlot

    Public Overrides ReadOnly Property requiredFields As Dictionary(Of String(), String)
        Get
            Dim list As New Dictionary(Of String(), String)
        End Get
    End Property

    Public Overrides Function Plot(table As DataTable) As Image
        Throw New NotImplementedException()
    End Function
End Class
