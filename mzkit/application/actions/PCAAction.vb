Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.ANOVA
Imports Mzkit_win32.BasicMDIForm
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports TaskStream

Public Class PCAAction : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return $"Do Principal Component Analysis"
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, table As DataTable)
        Dim names As String() = CLRVector.asCharacter(data)
        Dim vals As New List(Of DataSet)
        Dim propNames As New List(Of String)
        Dim id_offset As Integer = -1

        For i As Integer = 0 To table.Columns.Count - 1
            Dim col = table.Columns(i)

            If col.ColumnName <> fieldName Then
                Call propNames.Add(col.ColumnName)
            Else
                id_offset = i
            End If
        Next

        For i As Integer = 0 To table.Rows.Count - 1
            Dim row As DataRow = table.Rows(i)
            Dim d As New DataSet With {.ID = names(i)}
            Dim offset As Integer = 0

            If row.ItemArray.All(Function(v) IsDBNull(v)) Then
                Exit For
            End If

            For j As Integer = 0 To propNames.Count
                If j <> id_offset Then
                    d(propNames(offset)) = Val(row(j))
                    offset += 1
                End If
            Next

            Call vals.Add(d)
        Next

        Dim matrixfile As String = App.GetTempFile & ".csv"

        Call vals.Transpose.SaveTo(matrixfile)
        Call New InputPCADialog() _
            .SetMaxComponent(9) _
            .Input(Sub(cfg)
                       Dim config As InputPCADialog = DirectCast(cfg, InputPCADialog)
                       Dim score As String = $"{matrixfile.ParentPath}/pca/pca_score.csv"

                       Call RscriptProgressTask.RunComponentTask(matrixfile, "no_groups.csv", config.ncomp, config.showSampleLable, GetType(PCA))

                       If score.FileExists Then
                           Call WindowModules.ShowTable(DataFrameResolver.Load(score), "PCA Result")
                       Else
                           Call Workbench.Warning("Run PCA analysis error...")
                       End If
                   End Sub)
    End Sub
End Class
