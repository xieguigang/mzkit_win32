Imports System.ComponentModel
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MassSpectrometry.MoleculeNetworking
Imports BioNovoGene.BioDeep.MassSpectrometry.MoleculeNetworking.PoolData
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.My.JavaScript
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmCloudExplorer

    Public tree As HttpTreeFs
    Public loadTable As Action(Of String)
    Public host As FormViewer

    Dim model As SpectrumGraphModel

    Private Sub frmCloudExplorer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TabText = "Cloud Explorer"

        Call ApplyVsTheme(ToolStrip1, ContextMenuStrip2, ContextMenuStrip1)
        Call SelectModel()
    End Sub

    Private Sub LoadGraphModel(cloud As String, model_id As String)
        tree = New HttpTreeFs(cloud, model_id)
        TreeView1.Nodes.Clear()

        Dim childs = Me.tree.GetTreeChilds("/").ToArray
        Dim root = TreeView1.Nodes.Add($"Spectrum Pool [{tree.HttpServices.TrimEnd("/"c)}/]")

        root.ContextMenuStrip = ContextMenuStrip1

        root = root.Nodes.Add("/")
        root.Tag = "/"
        root.ImageIndex = 1
        root.SelectedImageIndex = 1
        root.ContextMenuStrip = ContextMenuStrip2

        Call addNodes(root, childs)
        Call Workbench.SuccessMessage($"Connected to the cloud services: {tree.HttpServices.TrimEnd("/"c)}/")
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

            node.ContextMenuStrip = ContextMenuStrip2
            node.ImageIndex = 2
            node.SelectedImageIndex = 2
            node.Tag = dir
        Next
    End Sub

    Public Function FetchMetadata(node As String) As IEnumerable(Of PoolData.Metadata)
        Dim key As String = If(node.Contains("/"), HttpTreeFs.ClusterHashIndex(node), node)
        Dim getMetadata As IEnumerable(Of PoolData.Metadata) = HttpRESTMetadataPool.FetchClusterData(
            url_get:=$"{tree.HttpServices}/get/metadata/",
            model_id:=tree.model_id,
            hash_index:=key
        )

        Return getMetadata
    End Function

    Private Iterator Function FetchSpectrum(tag As String) As IEnumerable(Of PeakMs2)
        For Each metadata As PoolData.Metadata In FetchMetadata(tag)
            Dim guid As String = metadata.guid
            Dim spectral As PeakMs2 = tree.ReadSpectrum(guid)

            If guid.StringEmpty OrElse spectral Is Nothing Then
                ' do nothing 
            Else
                spectral.lib_guid = metadata.ToString
                Yield spectral
            End If
        Next
    End Function

    Private Sub TreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterSelect
        Dim sel = TreeView1.SelectedNode

        If sel Is Nothing OrElse sel.Tag Is Nothing Then
            Return
        End If

        If sel.Nodes.Count = 0 Then
            Call addNodes(sel, tree.GetTreeChilds(CStr(sel.Tag)).ToArray)
        End If

        Call loadMetadata(sel.Tag)
        Call loadTable(sel.Tag)

        If host.DockState = DockState.Hidden OrElse host.DockState = DockState.Unknown Then
            host.DockState = DockState.Document
            host.Show(Workbench.AppHost.DockPanel)
        End If
    End Sub

    Private Sub loadMetadata(dir As String)
        loadMetadataByhashCode(HttpTreeFs.ClusterHashIndex(dir))
    End Sub

    Public Sub loadMetadataByhashCode(hash As String)
        Dim data As New Dictionary(Of String, Object)
        Dim js = tree.GetCluster(hash)

        If js Is Nothing Then
            Workbench.Warning("incorrect cluster hashcode!")
            Return
        End If

        For Each name_str In js
            data.Add(name_str, CStr(js(name_str)))
        Next

        Dim obj = DynamicType.Create(data)

        Call Workbench.AppHost.ShowProperties(obj)
    End Sub

    Private Sub frmCloudExplorer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
    End Sub

    Private Sub ToolStripTextBox1_Click(sender As Object, e As EventArgs) Handles ToolStripTextBox1.Click

    End Sub

    ''' <summary>
    ''' view cluster data via id/hashcode/path
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Dim input As String = Strings.Trim(ToolStripTextBox1.Text)

        If input.Contains("/"c) Then
            input = HttpTreeFs.ClusterHashIndex(input)
        End If

        loadMetadataByhashCode(input)
        loadTable(input)
    End Sub

    Private Sub SelectModel() Handles ToolStripButton2.Click
        InputDialog.Input(Of InputSelectGraphModel)(
            Sub(cfg)
                model = cfg.GetModel
                Call LoadGraphModel(cfg.GetCloudRootURL, model_id:=model.id)
            End Sub)
    End Sub

    ''' <summary>
    ''' Export mzpack data of current spectrum cluster node
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ExportMzPackToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportMzPackToolStripMenuItem.Click
        Dim sel = TreeView1.SelectedNode

        If sel Is Nothing OrElse sel.Tag Is Nothing Then
            Return
        End If

        Using file As New SaveFileDialog With {.Filter = "BioNovoGene Spectrum Pack(*.mzPack)|*.mzPack"}
            If file.ShowDialog = DialogResult.OK Then
                Dim savefile As String = file.FileName
                Dim node_tag As String = sel.Tag.ToString

                Call TaskProgress.RunAction(
                    Sub(echo As ITaskProgress)
                        Dim println As Action(Of String) = AddressOf echo.SetInfo

                        Call println("Fetch spectrum data from the cloud service...")
                        Dim spectrum As PeakMs2() = FetchSpectrum(node_tag).ToArray
                        Call println("Construct the spectrum objects...")
                        Dim scan1 = spectrum.GroupBy(Function(p) p.rt, 5) _
                            .Select(Function(p)
                                        Dim scan2 = p.Select(Function(p2)
                                                                 Return New ScanMS2 With {
                                                                    .intensity = p2.intensity,
                                                                    .mz = p2.mzInto.Select(Function(m) m.mz).ToArray,
                                                                    .into = p2.mzInto.Select(Function(m) m.intensity).ToArray,
                                                                    .parentMz = p2.mz,
                                                                    .rt = p2.rt,
                                                                    .scan_id = p2.lib_guid,
                                                                    .polarity = 1
                                                                 }
                                                             End Function) _
                                                     .ToArray

                                        Return New ScanMS1 With {
                                            .rt = Val(p.name),
                                            .BPC = p.Select(Function(a) a.intensity).Max,
                                            .TIC = p.Select(Function(a) a.intensity).Sum,
                                            .products = scan2,
                                            .scan_id = p.name,
                                            .mz = scan2.Select(Function(si) si.parentMz).ToArray,
                                            .into = scan2.Select(Function(si) si.intensity).ToArray
                                        }
                                    End Function) _
                            .ToArray
                        Dim pack As New mzPack With {
                            .Application = FileApplicationClass.LCMS,
                            .MS = scan1,
                            .source = sel.Tag
                        }
                        Call println("Save mzpack data to target file...")

                        Using buf As Stream = savefile.Open(FileMode.OpenOrCreate, doClear:=True)
                            Call pack.Write(buf, progress:=println)
                        End Using
                    End Sub)
            End If
        End Using
    End Sub

    ''' <summary>
    ''' View consensus spectrum of current spectrum cluster node
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ViewConsensusSpectrumToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewConsensusSpectrumToolStripMenuItem.Click
        Dim sel = TreeView1.SelectedNode

        If sel Is Nothing OrElse sel.Tag Is Nothing Then
            Return
        End If

        Dim hashcode As String = HttpTreeFs.ClusterHashIndex(sel.Tag)
        Dim js = tree.GetCluster(hashcode)

        If js Is Nothing Then
            Workbench.Warning("incorrect cluster hashcode!")
            Return
        End If

        For Each name_str In js
            If name_str = "consensus" Then
                Dim b64_consensus = CStr(js(name_str))

                If b64_consensus = "*" Then
                    Call Workbench.Warning("No consensus spectrum data...")
                    Return
                End If

                Dim v = HttpTreeFs.DecodeConsensus(b64_consensus)
                Dim spectrum As New PeakMs2 With {
                    .mzInto = v.mz _
                        .Select(Function(mzi, i) New ms2 With {.mz = mzi, .intensity = v.into(i)}) _
                        .ToArray,
                    .lib_guid = "Consensus Spectra",
                    .file = hashcode
                }

                Call SpectralViewerModule.ViewSpectral(spectrum)

                Exit For
            End If
        Next
    End Sub

    Private Sub ViewClusterScattersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewClusterScattersToolStripMenuItem.Click
        Dim viewer = Workbench.ShowSingleDocument(Of FormScatterViewer)()

        Call InputDialog.Input(Of frmSetClusterNumbers)(
            Sub(cfg)
                Call viewer.LoadClusters(tree, topN:=cfg.TopNClusters)
            End Sub)
    End Sub
End Class