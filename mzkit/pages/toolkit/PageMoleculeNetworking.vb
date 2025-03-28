﻿#Region "Microsoft.VisualBasic::0d5250942a956247ab7c1c459936e338, mzkit\src\mzkit\mzkit\pages\toolkit\PageMoleculeNetworking.vb"

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

'   Total Lines: 326
'    Code Lines: 262
' Comment Lines: 7
'   Blank Lines: 57
'     File Size: 16.10 KB


' Class PageMoleculeNetworking
' 
'     Sub: DataGridView1_CellContentClick, loadNetwork, PageMoleculeNetworking_Load, PageMoleculeNetworking_VisibleChanged, RefreshNetwork
'          RenderNetwork, SaveImageToolStripMenuItem_Click, saveNetwork
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Threading
Imports System.Windows.Forms.ListViewItem
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.MoleculeNetworking
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.mzkit_win32.cooldatagridview
Imports BioNovoGene.mzkit_win32.MSdata
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports RibbonLib.Interop
Imports TaskStream
Imports any = Microsoft.VisualBasic.Scripting
Imports std = System.Math

Public Class PageMoleculeNetworking

    Dim g As NetworkGraph
    Dim rawMatrix As EntityClusterModel()
    Dim nodeInfo As Protocols
    Dim rawLinks As Dictionary(Of String, LinkSet)
    Dim tooltip As New PlotTooltip

    Public Shared Sub RunUMAP()
        Dim MNtool = MyApplication.host.mzkitMNtools
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".csv", sessionID:=App.PID.ToHexString, prefix:="MNTools_clusters_")

        If MNtool.rawMatrix.IsNullOrEmpty Then
            MessageBox.Show("Sorry, run molecular networking analysis at first...", "No cluster data", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Call MNtool.rawMatrix.SaveTo(tempfile)

        Dim umap3 As String = RscriptProgressTask.CreateUMAPCluster(
            tempfile,
            knn:=16, knniter:=64, localConnectivity:=1, bandwidth:=1, learningRate:=0.99, spectral_cos:=True,
            readBinary:=False, noUI:=False)

        If umap3.StringEmpty Then
            MessageBox.Show("Sorry, run umap task error...", "UMAP error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return
        End If

        Dim df As DataFrameResolver = DataFrameResolver.Load(umap3)
        Dim labels As String() = df.Column(0).ToArray
        Dim x As Double() = df.GetColumnValues("x").Select(AddressOf Val).ToArray
        Dim y As Double() = df.GetColumnValues("y").Select(AddressOf Val).ToArray
        Dim z As Double() = df.GetColumnValues("z").Select(AddressOf Val).ToArray
        ' "Noise"
        Dim [class] As String() = df.GetColumnValues("class").SafeQuery.ToArray
        Dim scatter As UMAPPoint() = labels _
            .Select(Function(lbtext, i)
                        Return New UMAPPoint With {
                            .label = lbtext,
                            .[class] = [class](i),
                            .x = x(i),
                            .y = y(i),
                            .z = z(i)
                        }
                    End Function) _
            .ToArray

        Dim viewer As New frm3DScatterPlotView()
        Dim clusters = MNtool.nodeInfo

        Call viewer.LoadScatter(
            data:=scatter,
            onclick:=Sub(id)
                         Call Workbench.StatusMessage($"View spectrum cluster: {id}", My.Resources.mintupdate_up_to_date)
                         Call MNtool.showCluster(info:=clusters.Cluster(id), vlabel:=id)
                         Call PageMzkitTools.ShowSpectral(clusters.Cluster(id).representation)
                     End Sub)
        Call VisualStudio.ShowDocument(viewer)
    End Sub

    Public Sub RenderNetwork()
        If g Is Nothing Then
            Workbench.Warning("You should run molecular networking at first!")
            Return
        ElseIf g.vertex.Count > 500 OrElse g.graphEdges.Count > 700 Then
            Workbench.Warning("The network size is huge for create layout, entire progress will be very slow...")
        End If

        Dim viewer As frmNetworkViewer = VisualStudio.ShowDocument(Of frmNetworkViewer)(title:="Molecular Networking Viewer")
        Dim showSingle As Boolean = False
        Dim graph As NetworkGraph = g.Copy

        If Not showSingle Then
            Dim links = graph.connectedNodes.ToList

            For Each node In graph.vertex.ToArray
                If links.IndexOf(node) = -1 Then
                    graph.RemoveNode(node)
                End If
            Next
        End If

        Call graph.ComputeNodeDegrees

        Dim minRadius As Single = Globals.Settings.network.nodeRadius.min
        Dim degreeRange As New DoubleRange(graph.vertex.Select(Function(a) (a.data.mass + 1) * (a.degree.In + a.degree.Out)).ToArray)
        Dim similarityRange As New DoubleRange(graph.graphEdges.Select(Function(a) a.weight).ToArray)
        Dim nodeRadiusRange As DoubleRange = Globals.Settings.network.nodeRadius.AsDoubleRange
        Dim linkWidthRange As DoubleRange = Globals.Settings.network.linkWidth.AsDoubleRange
        Dim nodeRadius As Func(Of Graph.Node, Single) = Function(v) degreeRange.ScaleMapping((v.degree.In + v.degree.Out) * (v.data.mass + 1), nodeRadiusRange)
        Dim linkWidth As Func(Of Graph.Edge, Single) = Function(l) similarityRange.ScaleMapping(l.weight, linkWidthRange)
        Dim nodeClusters = graph.vertex.Select(Function(a) a.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)).Distinct.Indexing
        Dim colorSet As SolidBrush() = Designer.GetColors("Paper", nodeClusters.Count, alpha:=120).Select(Function(a) New SolidBrush(a)).ToArray
        Dim cancel As Value(Of Boolean) = False

        For Each v In graph.vertex
            v.data.color = colorSet(nodeClusters.IndexOf(v.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)))
            v.data.size = New Double() {nodeRadius(v)}
        Next

        Dim linkColor As Color = Color.FromArgb(161, 168, 172)

        For Each l In graph.graphEdges
            l.data.style = New Pen(linkColor, linkWidth(l))
        Next

        viewer.showTarget = AddressOf showCluster

        Call viewer.SetGraph(graph, layout:=Globals.Settings.network.layout)
        Call viewer.Show(MyApplication.host.m_dockPanel)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Sub showCluster(info As NetworkingNode, vlabel As String)
        Call MSdata.ShowCluster(info.members, vlabel)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Sub showCluster(v As Graph.Node)
        Call showCluster(info:=nodeInfo.Cluster(v.label), vlabel:=v.label)
    End Sub

    Public Sub RefreshNetwork()
        If rawMatrix.IsNullOrEmpty Then
            Workbench.Warning("No network graph data! Probably you should select some interested mass spectrum in ms2 level by check the checkbox in feature explorer at first.")
            Return
        End If

        Dim similarityCutoff As Double = ribbonItems.SpinnerSimilarity.DecimalValue

        Call ProgressSpinner.DoLoading(
            Sub()
                Call Thread.Sleep(500)
                Call Me.Invoke(Sub() loadNetwork(rawMatrix, nodeInfo, rawLinks, similarityCutoff))

                Workbench.StatusMessage($"Refresh network with new similarity filter {similarityCutoff} success!")
            End Sub)
    End Sub

    Public Sub loadNetworkData(MN As IEnumerable(Of EntityClusterModel),
                               nodes As Protocols,
                               rawLinks As Dictionary(Of String, LinkSet),
                               cutoff As Double)

        Me.rawMatrix = MN.ToArray
        Me.nodeInfo = nodes
        Me.rawLinks = rawLinks

        ribbonItems.SpinnerSimilarity.DecimalValue = 0.98
    End Sub

    Private Sub loadNetwork(MN As IEnumerable(Of EntityClusterModel),
                           nodes As Protocols,
                           rawLinks As Dictionary(Of String, LinkSet),
                           cutoff As Double)

        DataGridView1.Rows.Clear()
        ' DataGridView2.Rows.Clear()
        TreeListView1.Items.Clear()

        ' g = TreeGraph(Of PeakMs2, PeakMs2).CreateGraph(MN.getRoot, Function(a) a.lib_guid, Function(a) $"M{CInt(a.mz)}T{CInt(a.rt)}")
        '.doRandomLayout _
        '.doForceLayout(iterations:=100)
        g = New NetworkGraph
        rawMatrix = MN.ToArray
        nodeInfo = nodes
        tooltip.LoadInfo(nodeInfo)

        Dim colors As LoopArray(Of String) = Designer.GetColors("Set1:c9", 10).Select(AddressOf ToHtmlColor).AsLoop
        Dim colorIndex As New Dictionary(Of String, String)

        Me.rawLinks = rawLinks

        For Each row As EntityClusterModel In rawMatrix
            Dim info As NetworkingNode = nodeInfo.Cluster(row.ID)
            Dim rt As Double() = info.members.Select(Function(a) a.rt).ToArray
            Dim maxrt As Double = info.members.OrderByDescending(Function(a) a.Ms2Intensity).First.rt
            Dim color As String = colorIndex.ComputeIfAbsent(row.Cluster, Function(cl) colors.Next)
            Dim node As Graph.Node = g.CreateNode(row.ID, New NodeData With {
                .Properties = New Dictionary(Of String, String) From {
                    {NamesOf.REFLECTION_ID_MAPPING_NODETYPE, row.Cluster},
                    {"member_size", info.size},
                    {"m/z", info.mz},
                    {"rt", maxrt},
                    {"rtmin", rt.Min},
                    {"rtmax", rt.Max},
                    {"area", info.members.Sum(Function(a) a.Ms2Intensity)},
                    {"color", color}
                },
                .color = color.GetBrush,
                .label = $"{row.ID} member_size:{info.size}",
                .mass = info.size,
                .origID = info.representation.name
            })

            Call System.Windows.Forms.Application.DoEvents()
        Next

        Dim duplicatedEdges As New Index(Of String)
        Dim uniqueKey As String
        Dim edgeProps As EdgeData
        Dim score As (forward As Double, reverse As Double)

        ' show molecular network links in the table UI
        For Each row As EntityClusterModel In rawMatrix
            Dim rawLink As LinkSet = rawLinks(row.ID)

            For Each link In row.Properties.Where(Function(l) l.Value >= cutoff AndAlso l.Key <> row.ID)
                uniqueKey = {row.ID, link.Key}.OrderBy(Function(str) str).JoinBy(" vs ")

                If Not uniqueKey Like duplicatedEdges Then
                    score = rawLink(link.Key)
                    edgeProps = New EdgeData With {
                        .Properties = New Dictionary(Of String, String) From {
                            {"forward", score.forward},
                            {"reverse", score.reverse}
                        }
                    }

                    Call duplicatedEdges.Add(uniqueKey)
                    Call g.CreateEdge(row.ID, link.Key, link.Value, edgeProps)
                End If
            Next

            Call System.Windows.Forms.Application.DoEvents()
        Next

        If g.graphEdges.Count >= 8000 AndAlso MessageBox.Show("There are two many edges in your network, do you wan to increase the similarity threshold for reduce network size?",
                                                              "To many edges",
                                                              MessageBoxButtons.YesNo,
                                                              MessageBoxIcon.Question) = DialogResult.Yes Then

            ribbonItems.SpinnerSimilarity.DecimalValue = 0.98
            Call RefreshNetwork()
            Return
        End If

        Call g.ComputeNodeDegrees
        Call g.ComputeBetweennessCentrality

        ' show the molecular cluster graph nodes in the table UI
        For Each node As Graph.Node In g.vertex
            Dim info = nodeInfo.Cluster(node.label)
            ' create UI for cluster node
            Dim row As New TreeListViewItem With {
                .Text = node.label,
                .ImageIndex = 0,
                .ToolTipText = node.label
            }

            For Each member In info.members
                Dim ion As New TreeListViewItem(member.lib_guid) With {.ImageIndex = 1, .ToolTipText = member.lib_guid}

                ion.SubItems.Add(New ListViewSubItem With {.Text = member.file})
                ion.SubItems.Add(New ListViewSubItem With {.Text = member.mzInto.Length})
                ion.SubItems.Add(New ListViewSubItem With {.Text = member.mz})
                ion.SubItems.Add(New ListViewSubItem With {.Text = member.rt})
                ion.SubItems.Add(New ListViewSubItem With {.Text = "n/a"})
                ion.SubItems.Add(New ListViewSubItem With {.Text = "n/a"})
                ion.SubItems.Add(New ListViewSubItem With {.Text = member.Ms2Intensity})

                row.Items.Add(ion)
            Next

            row.SubItems.Add(New ListViewSubItem With {.Text = node.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE)})
            row.SubItems.Add(New ListViewSubItem With {.Text = info.size})
            row.SubItems.Add(New ListViewSubItem With {.Text = info.mz})
            row.SubItems.Add(New ListViewSubItem With {.Text = node.data("rt")})
            row.SubItems.Add(New ListViewSubItem With {.Text = node.data("rtmin")})
            row.SubItems.Add(New ListViewSubItem With {.Text = node.data("rtmax")})
            row.SubItems.Add(New ListViewSubItem With {.Text = node.data("area")})

            Call TreeListView1.Items.Add(row)
            Call System.Windows.Forms.Application.DoEvents()
        Next
        For Each edge As Edge In g.graphEdges
            Call DataGridView1.Rows.Add(
                edge.U.label,
                edge.V.label,
                std.Min(Val(edge.data!forward), Val(edge.data!reverse)).ToString("F4"),
                Val(edge.data!forward).ToString("F4"),
                Val(edge.data!reverse).ToString("F4"),
                "View Alignment"
            )
            Call System.Windows.Forms.Application.DoEvents()
        Next

        DataGridView2.Rows.Clear()
        DataGridView2.Rows.Add("nodes", g.vertex.Count)
        DataGridView2.Rows.Add("edges", g.graphEdges.Count)
        DataGridView2.Rows.Add("single nodes", g.vertex.Count - g.connectedNodes.Length)
        DataGridView2.Rows.Add("similarity_threshold", cutoff)

        For Each cluster In rawMatrix.GroupBy(Function(a) a.Cluster).OrderBy(Function(a) Val(a.Key))
            Call DataGridView2.Rows.Add($"#Cluster_{cluster.Key}", cluster.Count)
            Call System.Windows.Forms.Application.DoEvents()
        Next
    End Sub

    Public Sub saveNetwork()
        If Not g Is Nothing Then
            Using file As New FolderBrowserDialog With {.ShowNewFolderButton = True}
                If file.ShowDialog = DialogResult.OK Then
                    Dim meta As New Dictionary(Of String, String)

                    For i As Integer = 0 To DataGridView2.Rows.Count - 1
                        Dim key As String = any.ToString(DataGridView2.Rows(i).Cells(0).Value)
                        Dim val As String = any.ToString(DataGridView2.Rows(i).Cells(1).Value)

                        meta(key) = val
                    Next

                    Call g.Tabular(
                        propertyNames:={"member_size", "m/z", "rt", "rtmin", "rtmax", "area", "forward", "reverse", "color"},
                        creators:={My.User.Name},
                        title:="Molecular Networking",
                        description:="Molecular Networking Model",
                        keywords:={"spectrum"},
                        links:={"http://mzkit.org"},
                        meta:=meta
                    ).Save(output:=file.SelectedPath)
                    Call Process.Start(file.SelectedPath)
                End If
            End Using
        Else
            MessageBox.Show("No network graph Object Is found! Please GoTo raw file viewer page And Select a raw file To run [Molecule Networking] analysis!",
                            "No network graph object!", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    ''' <summary>
    ''' show cluster member spectrum into the feature explorer
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ShowSpectrumToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowSpectrumToolStripMenuItem.Click
        Dim v As NetworkingNode = GetSelectedCluster()

        If Not v Is Nothing Then
            Call showCluster(v, v.referenceId)
        End If
    End Sub

    Private Function GetSelectedCluster() As NetworkingNode
        Dim cluster As TreeListViewItem
        Dim host = MyApplication.host
        Dim v As NetworkingNode

        If TreeListView1.SelectedItems.Count = 0 Then
            Return Nothing
        Else
            cluster = TreeListView1.SelectedItems(0)
        End If

        If cluster.ChildrenCount > 0 Then
            ' 是一个cluster
            v = nodeInfo.Cluster(cluster.Text)
        Else
            ' 是一个spectrum
            v = nodeInfo.Cluster(cluster.Parent.Text)
        End If

        Return v
    End Function

    Private Sub FilterSimilarClustersToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FilterSimilarClustersToolStripMenuItem.Click
        Dim current As NetworkingNode = GetSelectedCluster()
        Dim links As LinkSet = rawLinks(current.referenceId)
        Dim clusterSet As New List(Of PeakMs2)

        Call InputDialog.Input(Of InputMNSimilarityScore)(
            Sub(cfg)
                Dim cutoff As Double = cfg.GetCutoff
                Dim mz As New List(Of ms2)

                Call mz.Add(New ms2 With {.mz = current.mz, .intensity = current.size})
                Call clusterSet.AddRange(current.members)

                For Each cluster In links.links
                    If cluster.Value.GetScore >= cutoff Then
                        Dim target = nodeInfo.Cluster(cluster.Key)

                        Call clusterSet.AddRange(target.members)
                        Call mz.Add(New ms2 With {.mz = target.mz, .intensity = target.size})
                    End If
                Next

                ' load into feature explorer
                Call MSdata.ShowCluster(clusterSet.ToArray, $"cos({current.referenceId},y) > {cutoff}")
                ' show mass diff analysis
                Call showMasssdiff(current.mz, mz.ToArray)
            End Sub)
    End Sub

    Private Sub SaveImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowImageToolStripMenuItem.Click
        Dim cluster As TreeListViewItem
        Dim host = MyApplication.host

        If TreeListView1.SelectedItems.Count = 0 Then
            Return
        Else
            cluster = TreeListView1.SelectedItems(0)
        End If

        If cluster.ChildrenCount > 0 Then
            ' 是一个cluster
            Dim clusterId As String = cluster.Text
            Dim clusterSpectrum = nodeInfo.Cluster(clusterId).representation

            Call PageMzkitTools.ShowSpectral(clusterSpectrum)
        Else
            ' 是一个spectrum
            Dim spectrumName As String = cluster.Text
            Dim spectrum = nodeInfo.GetSpectrum(spectrumName)

            Call PageMzkitTools.ShowSpectral(spectrum)
        End If
    End Sub

    ''' <summary>
    ''' Show current spectrum alignment with the cluster representive spectrum
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ShowClusterAlignmentToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowClusterAlignmentToolStripMenuItem.Click
        Dim cluster As TreeListViewItem
        Dim host = MyApplication.host

        If TreeListView1.SelectedItems.Count = 0 Then
            Return
        Else
            cluster = TreeListView1.SelectedItems(0)
        End If

        If cluster.ChildrenCount > 0 Then
            ' is a cluster, not working for the cluster
            Return
        End If

        ' get spectrum data
        Dim spectrumName As String = cluster.Text
        Dim spectrum = nodeInfo.GetSpectrum(spectrumName)
        Dim clusterId = cluster.Parent.Text
        Dim cluster_representive = nodeInfo.Cluster(clusterId).representation
        ' create spectrum matrix alignment
        Dim alignment As AlignmentOutput = AlignmentProvider _
            .Cosine(Tolerance.DeltaMass(0.3), New RelativeIntensityCutoff(0.05)) _
            .CreateAlignment(spectrum, cluster_representive)

        Call MyApplication.host.mzkitTool.showAlignment(alignment, showScore:=True)
        Call MyApplication.host.ShowMzkitToolkit()
    End Sub

    Private Sub PageMoleculeNetworking_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        Dim mnTools = ribbonItems.TabGroupNetworkTools

        If Visible Then
            mnTools.ContextAvailable = ContextAvailability.Active
        Else
            mnTools.ContextAvailable = ContextAvailability.NotAvailable
        End If
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.ColumnIndex = 5 AndAlso e.RowIndex > -1 Then
            Dim row = DataGridView1.Rows(e.RowIndex)
            Dim a = CStr(row.Cells(0).Value)
            Dim b = CStr(row.Cells(1).Value)
            Dim host = MyApplication.host

            If a Is Nothing OrElse b Is Nothing Then
                Return
            End If

            Dim nodeA = nodeInfo.Cluster(a)
            Dim nodeB = nodeInfo.Cluster(b)
            Dim matrix As SSM2MatrixFragment() = GlobalAlignment.CreateAlignment(nodeA.representation.ms2, nodeB.representation.ms2, Tolerance.DeltaMass(0.3)).ToArray
            Dim alignment As New AlignmentOutput With {.alignments = matrix}

            host.mzkitTool.showMatrix(matrix, $"{row.Cells(0).Value}_vs_{row.Cells(1).Value}")

            host.mzkitTool.PictureBox1.BackgroundImage = MassSpectra.AlignMirrorPlot(nodeA.representation, nodeB.representation).AsGDIImage
            host.mzkitTool.CustomTabControl1.SelectedTab = host.mzkitTool.TabPage5

            host.ShowPage(host.mzkitTool)
        End If
    End Sub

    Private Sub PageMoleculeNetworking_Load(sender As Object, e As EventArgs) Handles Me.Load
        DataGridView1.CoolGrid
        DataGridView2.CoolGrid
        tooltip.OwnerDraw = True
    End Sub

    Private Sub ExportClusterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportClusterToolStripMenuItem.Click
        Using file As New SaveFileDialog With {.Filter = "MGF file export(*.mgf)|*.mgf"}
            If file.ShowDialog = DialogResult.OK Then
                Dim cluster = GetSelectedCluster()
                Dim peaks As PeakMs2() = cluster.members

                If peaks.SaveAsMgfIons(file.FileName) Then
                    Call MessageBox.Show("Export target cluster member spectrum as mgf ion text file success!",
                                         "Export Cluster",
                                         buttons:=MessageBoxButtons.OK,
                                         icon:=MessageBoxIcon.Information)
                End If
            End If
        End Using
    End Sub
End Class
