Imports Microsoft.VisualBasic.Imaging

Namespace Configuration

    Public Class TissueMap
        Public Property editor As PolygonEditor
        Public Property region_prefix As String
        Public Property opacity As Single
        Public Property spot_size As Single
        Public Property color_scaler As String

        Public Shared Function GetDefault() As TissueMap
            Return New TissueMap With {
                .editor = PolygonEditor.GetDefault,
                .color_scaler = "paper",
                .opacity = 0.8,
                .region_prefix = "region_",
                .spot_size = 8
            }
        End Function
    End Class

    Public Class PolygonEditor

        Public Property point_size As Single
        Public Property point_color As String
        Public Property show_points As Boolean
        Public Property line_width As Single
        Public Property dash As Boolean
        Public Property line_color As String

        Public Shared Function GetDefault() As PolygonEditor
            Return New PolygonEditor With {
                .dash = False,
                .line_color = Color.Black.ToHtmlColor,
                .line_width = 3,
                .point_color = Color.Red.ToHtmlColor,
                .point_size = 8,
                .show_points = True
            }
        End Function
    End Class
End Namespace