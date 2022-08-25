Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports PipelineHost
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
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

        If keggId.Any(Function(cid) cid.IsPattern("[CDG]\d+")) Then
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
        Dim maps = KEGGRepo.RequestKEGGMaps _
            .GroupBy(Function(map) map.id) _
            .Select(Function(map) map.First) _
            .ToArray
        Dim pathways = maps.ToDictionary(
            Function(m) m.id,
            Function(m)
                Return m.shapes _
                    .Select(Function(a) a.IDVector) _
                    .IteratesALL _
                    .Distinct _
                    .Where(Function(id) id.IsPattern("C\d+")) _
                    .Indexing
            End Function)
        Dim profiles = pathways _
            .ToDictionary(Function(p) p.Key,
                          Function(p)
                              Return p.Value _
                                  .Intersect(collection:=cid) _
                                  .Distinct _
                                  .ToArray
                          End Function)
        Dim profileResult = GetProfileResultTable(profiles.ToDictionary(Function(d) d.Key, Function(d) d.Value.Length), countMap:=False)
        Dim mapHits = profiles.ToDictionary(Function(d) "map" & d.Key.Match("\d+"), Function(d) d.Value.JoinBy("; "))

        For Each item In profileResult
            item("hits") = mapHits.TryGetValue(item("mapid"))
        Next

        Return profileResult
    End Function

    Private Function StatPathways(pid As String()) As EntityObject()
        Dim data = pid _
            .GroupBy(Function(id) id) _
            .ToDictionary(Function(map) map.Key, Function(any) 1) _
            .DoCall(Function(profile) GetProfileResultTable(profile, countMap:=True))
        Dim category = data _
            .GroupBy(Function(i) i("category")) _
            .Select(Function(cat)
                        Return New EntityObject With {
                            .ID = cat.Key,
                            .Properties = New Dictionary(Of String, String) From {
                                {"class", cat.First.ItemValue("class")},
                                {"hits", cat.Count},
                                {"mapid", cat.Select(Function(i) i("mapid")).JoinBy("; ")}
                            }
                        }
                    End Function) _
            .ToArray

        Return category
    End Function

    Private Function GetProfileResultTable(profiles As Dictionary(Of String, Integer), countMap As Boolean) As EntityObject()
        Return profiles _
            .AsNumeric _
            .KEGGCategoryProfiles _
            .Select(Function(category)
                        Return category.Value _
                            .Select(Function(term)
                                        If countMap Then
                                            Return New EntityObject With {
                                                .ID = term.Name,
                                                .Properties = New Dictionary(Of String, String) From {
                                                    {"category", term.Description.GetTagValue(":").Name},
                                                    {"class", category.Key},
                                                    {"mapid", "map" & term.Description.GetTagValue(":").Value}
                                                }
                                            }
                                        Else
                                            Return New EntityObject With {
                                                .ID = term.Name,
                                                .Properties = New Dictionary(Of String, String) From {
                                                    {"category", term.Description.GetTagValue(":").Name},
                                                    {"class", category.Key},
                                                    {"mapid", "map" & term.Description.GetTagValue(":").Value},
                                                    {"count", term.Value}
                                                }
                                            }
                                        End If
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
    End Function
End Class
