Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MoleculeNetworking.PoolData
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Microsoft.VisualBasic.My.JavaScript
Imports Mzkit_win32.BasicMDIForm
Imports any = Microsoft.VisualBasic.Scripting

Public Class FormViewer

    Dim tree As HttpTreeFs
    Dim memoryData As New DataSet
    Dim search As GridSearchHandler

    Private Sub FormViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.tree = New HttpTreeFs("http://192.168.0.207:83/taxonomy")
        Me.TabText = "Spectrum Pool Viewer"
        Me.search = New GridSearchHandler(AdvancedDataGridView1)

        Dim childs = Me.tree.GetTreeChilds("/").ToArray
        Dim root = TreeView1.Nodes.Add($"Spectrum Pool [{tree.HttpServices.TrimEnd("/"c)}/ connected!]").Nodes.Add("/")

        root.Tag = "/"
        root.ImageIndex = 1
        root.SelectedImageIndex = 1

        Call addNodes(root, childs)

        AddHandler AdvancedDataGridViewSearchToolBar1.Search, AddressOf search.AdvancedDataGridViewSearchToolBar1_Search

        ApplyVsTheme(AdvancedDataGridViewSearchToolBar1)
    End Sub

    Private Sub addNodes(root As TreeNode, childs As String())
        root.ImageIndex = 1
        root.SelectedImageIndex = 1

        For Each dir As String In childs
            Dim data As JavaScriptObject = tree.GetCluster(HttpTreeFs.ClusterHashIndex(dir))
            Dim annotations As String = data!annotations
            Dim n_childs As String = data!n_childs
            Dim n_spectrum As String = data!n_spectrum

            If annotations.StringEmpty(testEmptyFactor:=True) Then
                annotations = dir.BaseName
            End If

            Dim node = root.Nodes.Add($"{annotations} [{n_childs} childs, {n_spectrum} spectrum]")

            node.ImageIndex = 2
            node.SelectedImageIndex = 2
            node.Tag = dir
        Next
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

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect
        Dim sel = TreeView1.SelectedNode

        If sel Is Nothing OrElse sel.Tag Is Nothing Then
            Return
        End If

        If sel.Nodes.Count = 0 Then
            Call addNodes(sel, tree.GetTreeChilds(CStr(sel.Tag)).ToArray)
        End If

        Call loadMetadata(sel.Tag)
        Call loadTable(sel)
    End Sub

    Private Sub loadMetadata(dir As String)
        Dim data As New Dictionary(Of String, Object)
        Dim js = tree.GetCluster(HttpTreeFs.ClusterHashIndex(dir))

        If js Is Nothing Then
            Return
        End If

        For Each name_str In js
            data.Add(name_str, CStr(js(name_str)))
        Next

        data.Add("tree_path", dir)

        Dim obj = DynamicType.Create(data)

        Call Workbench.AppHost.ShowProperties(obj)
    End Sub

    Private Sub loadTable(node As TreeNode)
        Dim getMetadata As Metadata() = HttpRESTMetadataPool.FetchClusterData(
            url_get:=$"{tree.HttpServices}/get/metadata/",
            hash_index:=HttpTreeFs.ClusterHashIndex(node.Tag)
        ).ToArray

        Call LoadTable(
            Sub(tbl)
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

                For Each meta As Metadata In getMetadata
                    tbl.Rows.Add(meta.guid, meta.mz, meta.rt, meta.intensity, meta.source_file, meta.sample_source, meta.organism, meta.name, meta.biodeep_id, meta.formula, meta.adducts)
                Next
            End Sub)
    End Sub

    Private Shared Function getTitle(meta As DataGridViewRow) As String
        Const no_id = "unknown conserved"

        If no_id = any.ToString(meta.Cells.Item(8).Value) Then
            Return $"{no_id} [{meta.Cells.Item(5).Value}@{meta.Cells.Item(6).Value}]"
        Else
            Return $"{meta.Cells.Item(7).Value}_{meta.Cells.Item(10).Value} [{meta.Cells.Item(5).Value}@{meta.Cells.Item(6).Value}]"
        End If
    End Function

    Private Sub ViewSpectralToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewSpectralToolStripMenuItem.Click
        Dim rows = AdvancedDataGridView1.SelectedRows

        If rows.Count = 0 Then
            Return
        End If

        Dim metadataRow = rows.Item(0)

        If metadataRow.Cells.Count = 0 Then
            Return
        End If

        Dim guid As String = CStr(metadataRow.Cells.Item(0).Value)
        Dim spectral As PeakMs2 = Me.tree.ReadSpectrum(guid)

        spectral.lib_guid = getTitle(metadataRow)

        Call SpectralViewerModule.ViewSpectral(spectral)
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening

    End Sub
End Class
