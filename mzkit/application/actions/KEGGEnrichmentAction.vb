Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.GCModeller.Workbench.KEGGReport

Public Class KEGGEnrichmentAction : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Run kegg enrichment on a given set of KEGG compound id list. The selected column field is the kegg compound id list."
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, tbl As DataTable)
        Dim maps As Map() = Nothing
        Dim kegg As Background = Globals.loadBackground(maps)
        Dim enrich = frmTaskProgress.LoadData(
                    Function(msg)
                        Dim all = kegg.Enrichment(data.AsObjectEnumerator.Where(Function(c) Not c Is Nothing).Select(Function(c) c.ToString), outputAll:=True, showProgress:=True, doProgress:=msg).ToArray
                        Call msg("Do FDR...")
                        Dim fdr = all.FDRCorrection.OrderBy(Function(p) p.pvalue).ToArray

                        Return fdr
                    End Function, title:="Run KEGG Enrichment", info:="Run fisher test...")
        Dim table = VisualStudio.ShowDocument(Of frmTableViewer)(title:="KEGG Enrichment Result")
        Dim mapIndex = maps.ToDictionary(Function(m) m.id)

        table.ViewRow =
                     Sub(row)
                         Dim id As String = row("term")
                         Dim map As Map = mapIndex.TryGetValue(id)

                         If map Is Nothing Then
                             map = mapIndex.TryGetValue(id.Match("\d+"))
                         End If

                         If map Is Nothing Then
                             Call MyApplication.host.warning($"No kegg pathway map available for reference id: {id}!")
                             Return
                         End If

                         Dim geneIds = row("geneIDs").ToString.StringSplit(",\s+").Select(Function(gid) New NamedValue(Of String)(gid, "blue")).ToArray
                         Dim image As String = ReportRender.Render(map, geneIds)
                         Dim temp As String = TempFileSystem.GetAppSysTempFile(".html", sessionID:=App.PID, prefix:="kegg_pathway")
                         Dim browser = VisualStudio.ShowDocument(Of frmHtmlViewer)(title:=map.Name)

                         Call image.SaveTo(temp)
                         Call browser.LoadHtml(temp)
                     End Sub
        table.LoadTable(Sub(grid)
                            grid.Columns.Add(NameOf(EnrichmentResult.term), GetType(String))
                            grid.Columns.Add(NameOf(EnrichmentResult.name), GetType(String))
                            grid.Columns.Add(NameOf(EnrichmentResult.description), GetType(String))
                            grid.Columns.Add(NameOf(EnrichmentResult.cluster), GetType(Integer))
                            grid.Columns.Add(NameOf(EnrichmentResult.enriched), GetType(String))
                            grid.Columns.Add(NameOf(EnrichmentResult.score), GetType(Double))
                            grid.Columns.Add(NameOf(EnrichmentResult.pvalue), GetType(Double))
                            grid.Columns.Add(NameOf(EnrichmentResult.FDR), GetType(Double))
                            grid.Columns.Add(NameOf(EnrichmentResult.geneIDs), GetType(String))

                            For Each item As EnrichmentResult In enrich
                                Call grid.Rows.Add(
                                            If(item.term.IsPattern("\d+"), $"map{item.term}", item.term),
                                            item.name,
                                            item.description,
                                            item.cluster,
                                            item.enriched,
                                            item.score,
                                            item.pvalue,
                                            item.FDR,
                                            item.geneIDs.JoinBy(", ")
                                         )
                            Next
                        End Sub)
    End Sub
End Class
