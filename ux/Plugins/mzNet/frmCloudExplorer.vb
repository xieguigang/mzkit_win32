﻿Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MoleculeNetworking.PoolData
Imports Microsoft.VisualBasic.My.JavaScript
Imports Mzkit_win32.BasicMDIForm

Public Class frmCloudExplorer

    Public tree As HttpTreeFs
    Public loadTable As Action(Of String)

    Private Sub frmCloudExplorer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TabText = "Cloud Explorer"
        tree = New HttpTreeFs("http://192.168.0.207:83/taxonomy")

        Dim childs = Me.tree.GetTreeChilds("/").ToArray
        Dim root = TreeView1.Nodes.Add($"Spectrum Pool [{tree.HttpServices.TrimEnd("/"c)}/ connected!]").Nodes.Add("/")

        root.Tag = "/"
        root.ImageIndex = 1
        root.SelectedImageIndex = 1

        Call addNodes(root, childs)
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
End Class