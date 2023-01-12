Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.application.json.Javascript

Public Class JSONViewer

    Public Sub LoadJSON(json As JsonElement)
        Win7StyleTreeView1.Nodes.Clear()
        Dim root = Win7StyleTreeView1.Nodes.Add("/")
        root.ImageIndex = 0
        loadTree(json, root, "")
    End Sub

    Private Sub loadTree(json As JsonElement, tree As TreeNode, prefix As String)
        If TypeOf json Is JsonArray Then
            Dim arr = DirectCast(json, JsonArray)
            Dim vec = tree.Nodes.Add($"{prefix}array[{arr.Length}]")
            Dim i As i32 = 0

            vec.ImageIndex = 0

            For Each item In arr
                Call loadTree(item, vec, $"[{++i}]: ")
            Next
        ElseIf TypeOf json Is JsonValue Then
            tree.Nodes.Add($"{prefix}{json.ToString}").ImageIndex = 1
        ElseIf TypeOf json Is JsonObject Then
            Dim obj = DirectCast(json, JsonObject)
            Dim list = tree.Nodes.Add($"{prefix}object")

            list.ImageIndex = 0

            For Each tuple In obj
                Call loadTree(tuple.Value, list, $"'{tuple.Name}': ")
            Next
        End If
    End Sub
End Class
