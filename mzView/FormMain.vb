Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Mzkit_win32.BasicMDIForm

Public Class FormMain

    Dim mzpack As StreamPack
    Dim viewers As New Dictionary(Of String, Control)

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        App.Exit(0)
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Using file As New OpenFileDialog With {.Filter = "mzPack Data File(*.mzPack)|*.mzPack"}
            If file.ShowDialog = DialogResult.OK Then
                mzpack = New StreamPack(file.FileName, [readonly]:=True)
                loadTree()
            End If
        End Using
    End Sub

    Private Sub loadTree()
        Dim tree = Win7StyleTreeView1.Nodes.Add(mzpack.ToString)
        Dim root = mzpack.superBlock

        Call TaskProgress.RunAction(
            run:=Sub(msg)
                     Call loadTree(tree, root, msg.Echo)
                 End Sub,
            title:="Parse mzPack Tree",
            info:="Parse file..."
        )
        Call Workbench.StatusMessage("Parse mzPack success!")
    End Sub

    Private Sub loadTree(tree As TreeNode, dir As StreamGroup, echo As Action(Of String))
        For Each item As StreamObject In dir.files
            Dim current_dir = tree.Nodes.Add(item.fileName)
            current_dir.Tag = item

            If TypeOf item Is StreamGroup Then
                Call Application.DoEvents()
                Call echo(item.ToString)
                Call loadTree(current_dir, item, echo)
            End If
        Next
    End Sub

    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click
        Dim node As TreeNode = Win7StyleTreeView1.SelectedNode

        If node Is Nothing Then
            Return
        End If

        Dim data As StreamObject = node.Tag

        If data Is Nothing Then
            Return
        End If

        If Not TypeOf data Is StreamBlock Then
            Return
        End If

        Select Case data.referencePath.ToString.ExtensionSuffix.ToLower
            Case "json"
                Dim text As String = mzpack.ReadText(data.referencePath.ToString)
                Dim json As JsonElement = JsonParser.Parse(text)

                DirectCast(showViewer("json"), JSONViewer).LoadJSON(json)
            Case "bson"
            Case "txt"
                Dim text As String = mzpack.ReadText(data.referencePath.ToString)
                DirectCast(showViewer("txt"), TextViewer).LoadText(text)
            Case Else
                ' do nothing
        End Select
    End Sub

    Private Function showViewer(key As String) As Control
        For Each viewer In viewers.Values
            viewer.Visible = False
        Next

        viewers(key).Visible = True
        viewers(key).Dock = DockStyle.Fill

        Return viewers(key)
    End Function

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        viewers.Add("json", New JSONViewer)
        viewers.Add("bson", New JSONViewer)
        viewers.Add("txt", New TextViewer)

        For Each viewer In viewers.Values
            SplitContainer1.Panel2.Controls.Add(viewer)
            viewer.Dock = DockStyle.Fill
            viewer.Visible = False
        Next

        Call ApplyVsTheme(MenuStrip1, ContextMenuStrip1)
    End Sub
End Class
