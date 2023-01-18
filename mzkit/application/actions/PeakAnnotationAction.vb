Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm

Public Class PeakAnnotationAction : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Run peak list annotation based on a given set of m/z data list."
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, table As DataTable)
        MyApplication.host.mzkitSearch.TextBox3.Text = data.AsObjectEnumerator.JoinBy(vbCrLf)
        MyApplication.host.mzkitSearch.SourceName = table.Namespace
        MyApplication.host.mzkitSearch.InstanceGuid = frmTableViewer.TableGuid(table)

        Call MyApplication.host.mzkitSearch.TabControl1.SelectTab(MyApplication.host.mzkitSearch.TabPage3)
        Call MyApplication.host.ShowPage(MyApplication.host.mzkitSearch)
    End Sub
End Class
