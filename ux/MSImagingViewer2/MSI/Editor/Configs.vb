Imports System.Drawing

Namespace PolygonEditor

    Public Class PolygonEditorConfigs

        Public Property point_size As Single
        Public Property point_color As String
        Public Property show_points As Boolean
        Public Property line_width As Single
        Public Property dash As Boolean
        Public Property line_color As String

        Public Shared Function GetDefault() As PolygonEditorConfigs
            Return New PolygonEditorConfigs With {
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