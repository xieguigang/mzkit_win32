Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler

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

        Public Shared Narrowing Operator CType(configs As Filters) As RasterPipeline
            If configs Is Nothing OrElse configs.filters.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim filters As New RasterPipeline

            For i As Integer = 0 To configs.filters.Length - 1
                If configs.flags(i) Then
                    Call filters.Add(Scaler.Parse(configs.filters(i)))
                End If
            Next

            Return filters
        End Operator

    End Class
End Namespace