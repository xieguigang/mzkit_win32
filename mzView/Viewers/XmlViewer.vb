Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.application.xml

Public Class XmlViewer

    Public Sub LoadXmlDocument(xml As XmlElement)
        Win7StyleTreeView1.Nodes.Clear()
        Dim root = Win7StyleTreeView1.Nodes.Add("/")
        root.ImageIndex = 0
        root.SelectedImageIndex = 0
        loadTree(xml, root, "")
    End Sub

    Private Sub loadTree(xml As XmlElement, tree As TreeNode, prefix As String)
        Dim isArray As Boolean = Not xml.elements.IsNullOrEmpty

        If isArray Then
            Dim arr = xml.elements
            Dim vec = tree.Nodes.Add($"{prefix}array[{arr.Length}]")
            Dim i As i32 = 0

            vec.ImageIndex = 0
            vec.SelectedImageIndex = 0

            For Each item In arr
                Call loadTree(item, vec, $"[{++i}]: ")
            Next
        Else
            Dim node = tree.Nodes.Add($"{prefix}{xml.ToString}")

            node.ImageIndex = 1
            node.SelectedImageIndex = 1
        End If
    End Sub
End Class
