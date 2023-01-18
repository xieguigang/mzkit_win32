''' <summary>
''' run a specific action on a data table
''' </summary>
Public MustInherit Class ActionBase

    Public MustOverride ReadOnly Property Description As String
    Public MustOverride Sub RunAction(fieldName As String, data As Array, table As DataTable)

End Class
