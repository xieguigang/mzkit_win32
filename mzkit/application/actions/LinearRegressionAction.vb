Imports Mzkit_win32.BasicMDIForm

Public Class LinearRegressionAction : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Create linear regression model for make targetted quantification data"
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, table As DataTable)
        Dim fieldNames As String() = GetFieldNames(table).ToArray
        ' check of the required field names

    End Sub
End Class
