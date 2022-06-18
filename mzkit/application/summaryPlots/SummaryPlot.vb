Imports Microsoft.VisualBasic.ComponentModel.Collection

Public MustInherit Class SummaryPlot

    Public MustOverride ReadOnly Property requiredFields As Dictionary(Of String(), String)

    Public Function Test(fieldNames As String()) As Boolean
        Dim nameIndex As Index(Of String) = fieldNames.Indexing

        For Each names As String() In requiredFields.Keys
            Dim hit As Boolean = False

            For Each name As String In names
                If name Like nameIndex Then
                    hit = True
                    Exit For
                End If
            Next

            If Not hit Then
                Return False
            End If
        Next

        Return True
    End Function

    Public MustOverride Function Plot(table As DataTable) As Image

End Class
