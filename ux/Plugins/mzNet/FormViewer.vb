Imports BioNovoGene.Analytical.MassSpectrometry.Math.MoleculeNetworking.PoolData

Public Class FormViewer

    Dim tree As HttpTreeFs

    Private Sub FormViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim root = TreeView1.Nodes.Add("Spectrum Pool")

        Me.tree = New HttpTreeFs("http://192.168.0.207:83/taxonomy")

        Dim childs = Me.tree.GetTreeChilds("/").ToArray

        For Each dir As String In childs
            Dim node = root.Nodes.Add(dir.BaseName)
            node.Tag = dir
        Next
    End Sub
End Class
