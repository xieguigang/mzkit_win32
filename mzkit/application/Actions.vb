#Region "Microsoft.VisualBasic::295ba5f2f1918f08eb52f012f54d7943, mzkit\src\mzkit\mzkit\application\Actions.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 102
'    Code Lines: 88
' Comment Lines: 0
'   Blank Lines: 14
'     File Size: 5.16 KB


' Module Actions
' 
'     Properties: allActions
' 
'     Constructor: (+1 Overloads) Sub New
'     Sub: Register, registerKEGGEnrichment, registerMs1Search, RunAction
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.mzkit_win32.My
Imports ControlLibrary
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.GCModeller.Workbench.KEGGReport
Imports stdNum = System.Math

Public Delegate Sub TableAction(fieldName As String, data As Array, table As DataTable)

Module Actions

    ReadOnly actions As New Dictionary(Of String, (action As TableAction, desc As String))

    Public ReadOnly Property allActions As IEnumerable(Of NamedValue(Of String))
        Get
            Return actions _
                .Keys _
                .Select(Function(ref)
                            Return New NamedValue(Of String)(ref, actions(ref).desc)
                        End Function)
        End Get
    End Property

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub Register(name As String, action As TableAction, description As String)
        actions(name) = (action, description)
    End Sub

    Public Sub RunAction(name As String, fieldName As String, data As Array, table As DataTable)
        If actions.ContainsKey(name) Then
            Call actions(name).action(fieldName, data, table)
        Else
            Call MyApplication.host.warning($"missing action '{name}'!")
        End If
    End Sub

    Sub New()
        Call registerMs1Search()
        Call registerKEGGEnrichment()
        Call registerFormulaQuery()
    End Sub

    Private Sub registerPeakFinding()
        Call Register("Peak Finding",
             Sub(key, data, tbl)

             End Sub, "Do peak finding, the selected column data should be the signal intensity value.")
    End Sub

    Private Sub registerMs1Search()
        Call Register("Peak List Annotation",
             Sub(key, data, tbl)
                 MyApplication.host.mzkitSearch.TextBox3.Text = data.AsObjectEnumerator.JoinBy(vbCrLf)

                 Call MyApplication.host.mzkitSearch.TabControl1.SelectTab(MyApplication.host.mzkitSearch.TabPage3)
                 Call MyApplication.host.ShowPage(MyApplication.host.mzkitSearch)
             End Sub, "Run peak list annotation based on a given set of m/z data list.")
    End Sub

    Private Sub registerFormulaQuery()
        Call Register("Formula Query",
            Sub(key, data, tbl)
                Dim getFormula As New InputFormula
                Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

                If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
                    Dim formula As Formula = FormulaScanner.ScanFormula(getFormula.TextBox1.Text)
                    Dim name As String = getFormula.TextBox2.Text
                    Dim ppm As Integer = getFormula.NumericUpDown1.Value
                    Dim adducts As MzCalculator() = getFormula.GetAdducts
                    Dim mz As Double() = data.AsObjectEnumerator.Select(Function(o) Val(o)).ToArray
                    Dim headers As New Dictionary(Of String, Type)
                    Dim tblView = VisualStudio.ShowDocument(Of frmTableViewer)(title:=$"Formula Query[{formula}]")
                    Dim rows As New List(Of (DataRow, MzCalculator, ppm As Double, mztarget As Double))

                    headers.Add("mz_theoretical", GetType(Double))
                    headers.Add("precursor_type", GetType(String))
                    headers.Add("ppm", GetType(Double))
                    headers.Add("name", GetType(String))
                    headers.Add("formula", GetType(String))

                    For i As Integer = 0 To tbl.Columns.Count - 1
                        Dim tag As String = tbl.Columns.Item(i).ColumnName

                        If headers.ContainsKey(tag) Then
                            tag = $"{tag}_1"
                        End If

                        headers.Add(tag, tbl.Columns.Item(i).DataType)
                    Next

                    For Each type As MzCalculator In adducts
                        Dim mzTarget As Double = type.CalcMZ(formula.ExactMass)
                        Dim query = mz _
                            .Select(Function(mzi, idx) (PPMmethod.PPM(mzi, mzTarget), idx)) _
                            .Where(Function(t) t.Item1 <= ppm) _
                            .ToArray

                        If query.Length > 0 Then
                            For Each idx In query
                                Dim row = tbl.Rows.Item(idx.idx)
                                rows.Add((row, type, idx.Item1, mzTarget))
                            Next
                        End If
                    Next

                    Call tblView.LoadTable(
                        Sub(subView)
                            For Each field In headers
                                Call subView.Columns.Add(field.Key, field.Value)
                            Next

                            For Each row In rows
                                Dim values As New List(Of Object)
                                Dim subData = row.Item1

                                values.Add(stdNum.Round(row.mztarget, 4))
                                values.Add(row.Item2.ToString)
                                values.Add(stdNum.Round(row.ppm))
                                values.Add(name)
                                values.Add(formula.EmpiricalFormula)
                                values.AddRange(subData.ItemArray)

                                Call subView.Rows.Add(values.ToArray)
                            Next
                        End Sub)
                End If
            End Sub, "Run formula query on a given set of m/z data list, the theoretically m/z is evaluated from the input formula data by combine different ion precursr and then match theoretical m/z with the input m/z set.")
    End Sub

    Private Sub registerKEGGEnrichment()
        Call Register("KEGG Enrichment",
             Sub(key, data, tbl)
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
             End Sub, "Run kegg enrichment on a given set of KEGG compound id list. The selected column field is the kegg compound id list.")
    End Sub

End Module
