Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Linq
Imports TaskStream
Imports any = Microsoft.VisualBasic.Scripting

Public Class PCA3d : Inherits SummaryPlot

    Public Overrides ReadOnly Property requiredFields As Dictionary(Of String(), String)
        Get
            Dim list As New Dictionary(Of String(), String)

            list({"PC1"}) = "the PC1 component"
            list({"PC2"}) = "the PC2 component"
            list({"PC3"}) = "the PC3 component"

            Return list
        End Get
    End Property

    Public Overrides ReadOnly Property appName As String
        Get
            Return "PCA 3d"
        End Get
    End Property

    Public Overrides Function Plot(table As DataTable) As Object
        Dim pc1 = getFieldVector(table, {"PC1"}).AsObjectEnumerator.Select(Function(a) CDbl(a)).ToArray
        Dim pc2 = getFieldVector(table, {"PC2"}).AsObjectEnumerator.Select(Function(a) CDbl(a)).ToArray
        Dim pc3 = getFieldVector(table, {"PC3"}).AsObjectEnumerator.Select(Function(a) CDbl(a)).ToArray
        Dim data As New List(Of EntityObject)

        For i As Integer = 0 To pc1.Length - 1
            data.Add(New EntityObject With {
                    .ID = i + 1,
                    .Properties = New Dictionary(Of String, String) From {
                        {"X", pc1(i)},
                        {"Y", pc2(i)},
                        {"Z", pc3(i)}
                    }
                })
        Next

        If table.Columns.Contains("group") OrElse table.Columns.Contains("pattern") Then
            Dim groups = getFieldVector(table, {"group", "pattern"}) _
                .AsObjectEnumerator _
                .Select(Function(a) any.ToString(a)) _
                .ToArray

            For i As Integer = 0 To data.Count - 1
                data(i).Add("class", groups(i))
            Next
        Else
            For i As Integer = 0 To data.Count - 1
                data(i).Add("class", "n/a")
            Next
        End If

        Dim matrix As String = TempFileSystem.GetAppSysTempFile(".mat", sessionID:=App.PID, prefix:="pca_3d_scatter___")
        Call data.SaveTo(matrix)
        Return RscriptProgressTask.PlotScatter3DStats(matrix, "PCA 3d")
    End Function
End Class
