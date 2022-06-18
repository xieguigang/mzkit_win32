Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.mzkit_win32.My
Imports ControlLibrary
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports WeifenLuo.WinFormsUI.Docking
Imports any = Microsoft.VisualBasic.Scripting

Public Class KEGGStatsAction : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Count kegg pathway in different class/category."
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, table As DataTable)
        Dim keggId As String() = data _
            .AsObjectEnumerator() _
            .Select(Function(o) any.ToString(o)) _
            .ToArray
        Dim stats As EntityObject()

        If keggId.All(Function(cid) cid.IsPattern("[CDG]\d+")) Then
            ' is the compound list
            stats = StatCompounds(keggId)
        Else
            ' is the pathway map id list
            stats = StatPathways(keggId)
        End If

        Dim tbl = VisualStudio.ShowDocument(Of frmTableViewer)(title:="KEGG Stats Result")
        Dim names As String() = stats.PropertyNames

        tbl.LoadTable(
            Sub(grid)
                Call grid.Columns.Add("term", GetType(String))

                For Each name As String In names
                    Call grid.Columns.Add(name, GetType(String))
                Next

                For Each item As EntityObject In stats
                    Call grid.Rows.Add({CObj(item.ID)}.JoinIterates(names.Select(Function(i) item(i))).ToArray)
                Next
            End Sub)
    End Sub

    Private Function StatCompounds(cid As String()) As EntityObject()

    End Function

    Private Function StatPathways(pid As String()) As EntityObject()

    End Function

End Class
