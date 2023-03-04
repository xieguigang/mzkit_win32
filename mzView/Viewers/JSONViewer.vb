Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

Public Class JSONViewer

    Public Sub LoadJSON(json As JsonElement)
        Win7StyleTreeView1.Nodes.Clear()
        Dim root = Win7StyleTreeView1.Nodes.Add("/")
        root.ImageIndex = 0
        root.SelectedImageIndex = 0
        loadTree(json, root, "")
    End Sub

    Private Sub loadTree(json As JsonElement, tree As TreeNode, prefix As String)
        If TypeOf json Is JsonArray Then
            Dim arr = DirectCast(json, JsonArray)
            Dim vec = tree.Nodes.Add($"{prefix}array[{arr.Length}]")
            Dim i As i32 = 0

            vec.ImageIndex = 0
            vec.SelectedImageIndex = 0

            For Each item In arr
                Call loadTree(item, vec, $"[{++i}]: ")
            Next
        ElseIf TypeOf json Is JsonValue Then
            Dim node = tree.Nodes.Add($"{prefix}{json.ToString}")

            node.ImageIndex = 1
            node.SelectedImageIndex = 1
        ElseIf TypeOf json Is JsonObject Then
            Dim obj = DirectCast(json, JsonObject)
            Dim list = tree.Nodes.Add($"{prefix}object")

            list.ImageIndex = 0

            For Each tuple In obj
                Call loadTree(tuple.Value, list, $"'{tuple.Name}': ")
            Next
        End If
    End Sub

    Private Sub Win7StyleTreeView1_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles Win7StyleTreeView1.AfterSelect

    End Sub

    Private Sub Win7StyleTreeView1_DoubleClick(sender As Object, e As EventArgs) Handles Win7StyleTreeView1.DoubleClick
        Dim node As TreeNode = Win7StyleTreeView1.SelectedNode

        If node Is Nothing Then
            Return
        End If

        If node.Nodes.Count = 0 Then
            Dim text As String = node.Text
            Dim config As New frmTextDialog

            config.ShowText(text)

            Call InputDialog.Input(
                setConfig:=Sub(any)
                               ' do nothing
                           End Sub,
                config:=config)
        End If
    End Sub
End Class
