Namespace Configuration

    Public Class Filters

        Public Property filters As String()
        Public Property flags As Boolean()

        Public Shared Function DefaultFilters() As Filters
            Return New Filters With {
                .filters = {"denoise(0.01)", "TrIQ(0.65)", "knn_fill(3,0.65)", "soften()"},
                .flags = {True, True, True, True}
            }
        End Function

    End Class
End Namespace