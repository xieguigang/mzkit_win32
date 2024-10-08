﻿Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MassSpectrometry.MoleculeNetworking
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Mzkit_win32.BasicMDIForm
Imports WeifenLuo.WinFormsUI.Docking
Imports any = Microsoft.VisualBasic.Scripting

Public Class FormViewer

    Dim memoryData As New DataSet
    Dim search As GridSearchHandler
    Dim cloud As frmCloudExplorer
    Dim current_ptr As String

    Private Sub FormViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = "Spectrum Pool Viewer"
        Me.search = New GridSearchHandler(AdvancedDataGridView1)
        Me.cloud = New frmCloudExplorer() With {
            .loadTable = AddressOf loadTable2,
            .host = Me
        }

        Me.cloud.Show(Workbench.AppHost.DockPanel)
        Me.cloud.DockState = DockState.DockLeft

        AddHandler AdvancedDataGridViewSearchToolBar1.Search, AddressOf search.AdvancedDataGridViewSearchToolBar1_Search

        ApplyVsTheme(AdvancedDataGridViewSearchToolBar1, ContextMenuStrip1)
    End Sub

    Public Sub LoadTable(apply As Action(Of DataTable))
        memoryData = New DataSet

        Dim table As DataTable = memoryData.Tables.Add("memoryData")

        Try
            Call Me.AdvancedDataGridView1.Columns.Clear()
            Call Me.AdvancedDataGridView1.Rows.Clear()

            'Dim tbl = Me.AdvancedDataGridView1

            'tbl.Columns.Add("guid", "guid")
            'tbl.Columns.Add("mz", "mz")
            'tbl.Columns.Add("rt", "rt")
            'tbl.Columns.Add("intensity", "intensity")
            'tbl.Columns.Add("source_file", "source_file")
            'tbl.Columns.Add("sample_source", "sample_source")
            'tbl.Columns.Add("organism", "organism")
            'tbl.Columns.Add("name", "name")
            'tbl.Columns.Add("biodeep_id", "biodeep_id")
            'tbl.Columns.Add("formula", "formula")
            'tbl.Columns.Add("adducts", "adducts")
        Catch ex As Exception

        End Try

        Call apply(table)
        Call AdvancedDataGridView1.SetDoubleBuffered()

        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName

        AdvancedDataGridView1.DataSource = BindingSource1
        AdvancedDataGridViewSearchToolBar1.SetColumns(AdvancedDataGridView1.Columns)

        'For Each column As DataGridViewColumn In AdvancedDataGridView1.Columns
        '    'Select Case table.Columns.Item(column.HeaderText).DataType
        '    '    Case GetType(String)
        '    '        AdvancedDataGridView1.SetSortEnabled(column, True)
        '    '    Case GetType(Double)
        '    '    Case GetType(Integer)
        '    '    Case Else
        '    '        ' do nothing 
        '    'End Select

        '    AdvancedDataGridView1.ShowMenuStrip(column)
        'Next
    End Sub

    Private Sub loadTable2(node As String)
        Call LoadDataSet(ptr:=node, cloud.FetchMetadata(node))
    End Sub

    Public Sub LoadDataSet(ptr As String, metaSet As IEnumerable(Of PoolData.Metadata))
        Call LoadTable(
            apply:=Sub(tbl)
                       tbl.Columns.Add("guid", GetType(String)) '0
                       tbl.Columns.Add("mz", GetType(Double)) '1
                       tbl.Columns.Add("rt", GetType(Double)) '2
                       tbl.Columns.Add("intensity", GetType(Double)) '3
                       tbl.Columns.Add("source_file", GetType(String)) '4
                       tbl.Columns.Add("sample_source", GetType(String)) '5
                       tbl.Columns.Add("organism", GetType(String)) '6
                       tbl.Columns.Add("name", GetType(String)) '7
                       tbl.Columns.Add("biodeep_id", GetType(String)) '8
                       tbl.Columns.Add("formula", GetType(String)) '9
                       tbl.Columns.Add("adducts", GetType(String)) '10

                       For Each meta As PoolData.Metadata In metaSet
                           Call tbl.Rows.Add(meta.guid, meta.mz, meta.rt, meta.intensity, meta.source_file,
                                 meta.sample_source, meta.organism, meta.name, meta.biodeep_id,
                                 meta.formula, meta.adducts)
                       Next
                   End Sub)

        current_ptr = ptr
    End Sub

    Private Sub ExportSpectrumToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportSpectrumToolStripMenuItem.Click
        Using file As New SaveFileDialog With {.Filter = "MGF(*.mgf)|*.mgf"}
            If file.ShowDialog = DialogResult.OK Then
                Call TaskProgress.RunAction(
                    Sub(p As ITaskProgress)
                        Call p.SetProgressMode()
                        Call p.SetProgress(0)
                        Call exportMgfFile(file.FileName, p)
                    End Sub, title:="Fetch data from cloud", info:="Download spectrum data from the remote server...")

                Call MessageBox.Show("Export spectrum in mgf file format success!", "Export Spectrum Pool",
                                     buttons:=MessageBoxButtons.OK,
                                     icon:=MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    Private Sub exportMgfFile(filepath As String, p As ITaskProgress)
        Dim rows = AdvancedDataGridView1.Rows
        Dim data As New List(Of PeakMs2)
        Dim total As Integer = rows.Count
        Dim d As Integer = total / 25

        For i As Integer = 0 To total - 1
            Dim row = rows.Item(i)

            If row.Cells.Count = 0 Then
                Exit For
            End If

            Dim guid As String = CStr(row.Cells.Item(0).Value)

            If guid.StringEmpty Then
                Continue For
            End If

            Dim spectral As PeakMs2 = cloud.tree.ReadSpectrum(guid)
            Dim mz As Double = Val(row.Cells.Item(1).Value)
            Dim rt As Double = Val(row.Cells.Item(2).Value)
            Dim into As Double = Val(row.Cells.Item(3).Value)
            Dim filename As String = CStr(row.Cells.Item(4).Value)
            Dim id As String = CStr(row.Cells.Item(8).Value)
            Dim adducts As String = CStr(row.Cells.Item(10).Value)

            spectral.lib_guid = getTitle(row)
            spectral.file = filename
            spectral.precursor_type = adducts
            spectral.mz = mz
            spectral.rt = rt
            spectral.intensity = into
            spectral.scan = guid
            spectral.meta = New Dictionary(Of String, String) From {{"id", id}}

            data.Add(spectral)

            Call Application.DoEvents()

            If i Mod d = 0 Then
                Call p.SetProgress(i / total * 100, spectral.ToString)
            End If
        Next

        Call p.SetInfo($"Export ions data to file: {filepath}!")
        Call data.Select(Function(a) a.MgfIon).SaveTo(filepath)
    End Sub

    Private Shared Function getTitle(meta As DataGridViewRow) As String
        Const no_id = "unknown conserved"

        If no_id = any.ToString(meta.Cells.Item(8).Value) Then
            Return $"{no_id} [{meta.Cells.Item(5).Value}@{meta.Cells.Item(6).Value}]"
        Else
            Return $"{meta.Cells.Item(7).Value}_{meta.Cells.Item(10).Value} [{meta.Cells.Item(5).Value}@{meta.Cells.Item(6).Value}]"
        End If
    End Function

    Private Sub ViewSpectralToolStripMenuItem_Click() Handles ViewSpectralToolStripMenuItem.Click
        Dim rows = AdvancedDataGridView1.SelectedRows

        If rows.Count = 0 Then
            Return
        End If

        Dim metadataRow = rows.Item(0)

        If metadataRow.Cells.Count = 0 Then
            Return
        End If

        Dim guid As String = CStr(metadataRow.Cells.Item(0).Value)
        Dim spectral As PeakMs2 = cloud.tree.ReadSpectrum(guid)

        If guid.StringEmpty OrElse spectral Is Nothing Then
            Return
        Else
            spectral.lib_guid = getTitle(metadataRow)
        End If

        Call SpectralViewerModule.ViewSpectral(spectral)
    End Sub

    Private Sub AdvancedDataGridView1_RowStateChanged(sender As Object, e As DataGridViewRowStateChangedEventArgs) Handles AdvancedDataGridView1.RowStateChanged
        If AutoPlotSpectrumToolStripMenuItem.Checked Then
            If e.StateChanged = DataGridViewElementStates.Selected Then
                Call ViewSpectralToolStripMenuItem_Click()
            End If
        End If
    End Sub

    Private Sub MassDifferenceAnalysisToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MassDifferenceAnalysisToolStripMenuItem.Click
        Dim rows = AdvancedDataGridView1.Rows
        Dim mz As New List(Of ms2)

        For i As Integer = 0 To rows.Count - 1
            Dim row = rows.Item(i)

            If row.Cells.Count = 0 Then
                Exit For
            Else
                Dim mzi = Val(row.Cells.Item(1).Value)

                If mzi > 0 Then
                    Call mz.Add(New ms2 With {
                        .mz = Val(row.Cells.Item(1).Value),
                        .intensity = Val(row.Cells.Item(3).Value),
                        .Annotation = Nothing
                    })
                End If
            End If
        Next

        If mz.Count > 0 Then
            Call SpectralViewerModule.RunMassDiff(mz.Select(Function(i) i.mz).Min, mz.ToArray)
        End If
    End Sub

    Private Sub FormViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        DockState = DockState.Hidden
    End Sub

    Private Sub ViewClusterInBrowserToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewClusterInBrowserToolStripMenuItem.Click
        Try
            Dim url As String = $"http://novocell.mzkit.org/spectrum/cluster/?id={current_ptr}&model={cloud.tree.model_id}"
            Call Process.Start(url)
        Catch ex As Exception

        End Try
    End Sub

    Private Sub ViewBioDeepMetabolitesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewBioDeepMetabolitesToolStripMenuItem.Click
        Dim biodeep_id As New List(Of String)
        Dim index As Integer = 8
        Dim rows = AdvancedDataGridView1.Rows
        Dim url As String = Nothing

        Static pattern As New Regex("BioDeep_\d+")

        For i As Integer = 0 To rows.Count - 1
            Dim row = rows.Item(i)

            If row.Cells.Count = 0 Then
                Exit For
            Else
                Try
                    Call biodeep_id.Add(CStr(row.Cells.Item(index).Value))
                Catch ex As Exception

                End Try
            End If
        Next

        biodeep_id = biodeep_id.Where(Function(si) si.IsPattern(pattern)).Distinct.AsList

        If biodeep_id.Count > 13 Then
            Dim ssid As String = $"mzkit_win32_{$"{App.PID}-{Now.ToString}-{biodeep_id.JoinBy("+")}".MD5}"
            Dim payload As New Dictionary(Of String, String()) From {
                {"ssid", {ssid}},
                {"biodeep_id", {biodeep_id.ToArray.GetJson}}
            }
            Dim err As String = Nothing

            url = $"http://novocell.mzkit.org/kb/put_list/?ssid={ssid}"
            url.POST(payload, unsafe:=False, [error]:=err) _
               .DoCall(AddressOf Workbench.LogText)
            url = $"http://novocell.mzkit.org/kb/metabolites/?list=query:{ssid}"

            If Not err.StringEmpty Then
                Call Workbench.LogText(err)
            End If
        Else
            url = $"http://novocell.mzkit.org/kb/metabolites/?list={biodeep_id.JoinBy(",")}"
        End If

        Call Process.Start(url)
    End Sub
End Class
