Public Class ViewScatter3DAction : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return "View cluster data in 3d scatter style, the input data should be contains 3 dimensions at least."
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, table As DataTable)

    End Sub
End Class
