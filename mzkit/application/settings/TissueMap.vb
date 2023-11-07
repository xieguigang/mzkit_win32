Imports Mzkit_win32.MSImagingViewerV2.PolygonEditor

Namespace Configuration

    Public Class TissueMap
        Public Property editor As PolygonEditorConfigs
        Public Property region_prefix As String
        Public Property opacity As Single
        Public Property spot_size As Single
        Public Property color_scaler As String
        Public Property bootstrapping As SampleBootstrapping

        Public Shared Function GetDefault() As TissueMap
            Return New TissueMap With {
                .editor = PolygonEditorConfigs.GetDefault,
                .color_scaler = "paper",
                .opacity = 0.8,
                .region_prefix = "region_",
                .spot_size = 8,
                .bootstrapping = SampleBootstrapping.GetDefault
            }
        End Function
    End Class

    Public Class SampleBootstrapping

        Public Property nsamples As Integer = 32

        ''' <summary>
        ''' percentage value in range [0,1]
        ''' </summary>
        ''' <returns></returns>
        Public Property coverage As Double = 0.3

        Public Shared Function GetDefault() As SampleBootstrapping
            Return New SampleBootstrapping With {.coverage = 0.3, .nsamples = 32}
        End Function

    End Class

End Namespace