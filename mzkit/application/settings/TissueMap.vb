Imports Mzkit_win32.MSImagingViewerV2.PolygonEditor

Namespace Configuration

    Public Class TissueMap
        Public Property editor As PolygonEditorConfigs
        Public Property region_prefix As String
        Public Property opacity As Single
        Public Property spot_size As Single
        Public Property color_scaler As String

        Public Shared Function GetDefault() As TissueMap
            Return New TissueMap With {
                .editor = PolygonEditorConfigs.GetDefault,
                .color_scaler = "paper",
                .opacity = 0.8,
                .region_prefix = "region_",
                .spot_size = 8
            }
        End Function
    End Class

End Namespace