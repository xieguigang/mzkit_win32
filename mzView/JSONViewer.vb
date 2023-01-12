Imports Microsoft.VisualBasic.MIME.application.json.Javascript

Public Class JSONViewer

    Public Sub LoadJSON(json As JsonElement)
        Win7StyleTreeView1.Nodes.Clear()
        Dim root = Win7StyleTreeView1.Nodes.Add("/")
        loadTree(json, root)
    End Sub

    Private Sub loadTree(json As JsonElement, tree As TreeNode)
        If TypeOf json Is JsonArray Then
            Dim arr = DirectCast(json, JsonArray)
            Dim vec = tree.Nodes.Add($"array[{arr.Length}]")

            For Each item In arr
                Call loadTree(item, vec)
            Next
        ElseIf TypeOf json Is JsonValue Then
            tree.Nodes.Add(json.ToString)
        ElseIf TypeOf json Is JsonObject Then
            Dim obj = DirectCast(json, JsonObject)
            Dim list = tree.Nodes.Add($"object")

            For Each tuple In obj
                Call loadTree(tuple.Value, list)
            Next
        End If
    End Sub
End Class
