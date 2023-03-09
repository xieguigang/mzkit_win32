Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.DataStorage.HDSPack
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.MIME.application.xml
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Mzkit_win32.BasicMDIForm

Public Class FormMain

    Dim mzpack As StreamPack
    Dim viewers As New Dictionary(Of String, Control)

    Private Sub OpenToolStripMenuItem_Click()
        Using file As New OpenFileDialog With {.Filter = "mzPack Data File(*.mzPack)|*.mzPack|All File Formats(*.*)|*.*"}
            If file.ShowDialog = DialogResult.OK Then
                mzpack = New StreamPack(file.FileName, [readonly]:=True)
                Win7StyleTreeView1.Nodes.Clear()

                Call loadTree()
            End If
        End Using
    End Sub

    Private Sub loadTree()
        Dim tree = Win7StyleTreeView1.Nodes.Add(mzpack.ToString)
        Dim root = mzpack.superBlock
        Dim multiple_samples As Boolean = Not mzpack _
            .ReadText("/.etc/sample_tags.json") _
            .LoadJSON(Of String()) _
            .IsNullOrEmpty

        tree.ImageIndex = 0
        tree.SelectedImageIndex = 0

        Call TaskProgress.RunAction(
            run:=Sub(msg)
                     Call loadTree(tree, root, msg.Echo, depth:=If(multiple_samples, 0, 1))
                 End Sub,
            title:="Parse mzPack Tree",
            info:="Parse file...",
            host:=Me
        )
        Call Workbench.StatusMessage("Parse mzPack success!")
    End Sub

    Private Sub loadTree(tree As TreeNode, dir As StreamGroup, echo As Action(Of String), depth As Integer)
        For Each item As StreamObject In dir.files
            Dim current_dir = tree.Nodes.Add(item.fileName)
            current_dir.Tag = item
            current_dir.ImageIndex = 1
            current_dir.SelectedImageIndex = 1

            If TypeOf item Is StreamGroup Then
                Call System.Windows.Forms.Application.DoEvents()

                current_dir.ImageIndex = 0
                current_dir.SelectedImageIndex = 0

                If depth < 2 Then
                    Call echo(item.ToString)
                End If

                Call loadTree(current_dir, item, echo, depth + 1)
            End If
        Next
    End Sub

    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click
        Call OpenAndView(Win7StyleTreeView1.SelectedNode, Nothing)
    End Sub

    Private Sub OpenAndView(node As TreeNode, type As String)
        If node Is Nothing Then
            Return
        End If

        Dim data As StreamObject = node.Tag

        If data Is Nothing Then
            Return
        End If

        If Not TypeOf data Is StreamBlock Then
            Return
        Else
            If type.StringEmpty Then
                type = data.referencePath.ToString.ExtensionSuffix.ToLower
            Else
                type = type.ToLower
            End If
        End If

        Select Case type
            Case "json"
                Dim text As String = mzpack.ReadText(data.referencePath.ToString)
                Dim json As JsonElement = JsonParser.Parse(text)

                DirectCast(showViewer("json"), JSONViewer).LoadJSON(json)
            Case "bson"
                Dim buf As Stream = mzpack.OpenBlock(data)
                Dim json As JsonElement = BSON.Load(buf)

                DirectCast(showViewer("json"), JSONViewer).LoadJSON(json)
            Case "xml"
                Dim text As String = mzpack.ReadText(data.referencePath.ToString)
                Dim xml As XmlElement = XmlElement.ParseXmlText(text)

                DirectCast(showViewer("xml"), XmlViewer).LoadXmlDocument(xml)
            Case "mz"
                Dim buffer As Stream = mzpack.OpenBlock(data)
                Dim reader As New BinaryDataReader(buffer) With {.ByteOrder = ByteOrder.LittleEndian}
                Dim ms1 As New ScanMS1

                ' required of read the metadata
                Call Serialization.ReadScan1(ms1, file:=reader, readmeta:=True)

                Dim mat As New LibraryMatrix With {.ms2 = ms1.GetMs.ToArray, .name = ms1.scan_id}
                Dim img As Image = PeakAssign.DrawSpectrumPeaks(mat, size:="1920,1080").AsGDIImage
                Dim pic As PictureBox = showViewer("png")

                pic.BackgroundImage = img
            Case "txt"
                Dim text As String = mzpack.ReadText(data.referencePath.ToString)
                DirectCast(showViewer("txt"), TextViewer).LoadText(text)
            Case "png", "jpg", "jpeg", "bmp", "tiff"
                Dim buf As Stream = mzpack.OpenBlock(data)
                Dim img As Image = Image.FromStream(buf)
                Dim pic As PictureBox = showViewer("png")

                pic.BackgroundImage = img
            Case "html", "htm"
                Dim text As String = mzpack.ReadText(data.referencePath.ToString)
                Dim tmp As String = TempFileSystem.GetAppSysTempFile(ext:=".html")

                Call text.SaveTo(tmp, Encoding.UTF8)
                Call DirectCast(showViewer("html"), HtmlViewer).LoadPage(tmp)
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
        Dim img As New PictureBox
        Dim html As New HtmlViewer

        img.BackgroundImageLayout = ImageLayout.Zoom
        HookOpen = AddressOf OpenToolStripMenuItem_Click

        viewers.Add("json", New JSONViewer)
        viewers.Add("bson", New JSONViewer)
        viewers.Add("xml", New XmlViewer)
        viewers.Add("txt", New TextViewer)
        viewers.Add("png", img)
        viewers.Add("jpg", img)
        viewers.Add("jpeg", img)
        viewers.Add("bmp", img)
        viewers.Add("tiff", img)
        viewers.Add("htm", html)
        viewers.Add("html", html)

        For Each viewer In viewers.Values
            SplitContainer1.Panel2.Controls.Add(viewer)
            viewer.Dock = DockStyle.Fill
            viewer.Visible = False
        Next

        Call ApplyVsTheme(ContextMenuStrip1)
    End Sub

    Private Sub CopyFullPathToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles CopyFullPathToolStripMenuItem1.Click
        Dim node As TreeNode = Win7StyleTreeView1.SelectedNode

        If node Is Nothing Then
            Return
        End If

        Dim data As StreamObject = node.Tag

        If data Is Nothing Then
            Return
        Else
            Call Clipboard.Clear()
            Call Clipboard.SetText(data.referencePath.ToString)
        End If
    End Sub

    Private Sub ViewAsTextToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewAsTextToolStripMenuItem.Click
        Call OpenAndView(Win7StyleTreeView1.SelectedNode, "txt")
    End Sub

    Private Sub ViewAsHtmlToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewAsHtmlToolStripMenuItem.Click
        Call OpenAndView(Win7StyleTreeView1.SelectedNode, "html")
    End Sub

    Private Sub ExportAsFileToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportAsFileToolStripMenuItem.Click
        Dim node = Win7StyleTreeView1.SelectedNode

        If node Is Nothing Then
            Return
        End If

        Dim data As StreamObject = node.Tag

        If data Is Nothing OrElse TypeOf data Is StreamGroup Then
            Return
        End If

        Using file As New SaveFileDialog With {.FileName = data.referencePath.ToString.FileName}
            If file.ShowDialog = DialogResult.OK Then
                Dim buf = mzpack.OpenBlock(data)

                Call buf.Seek(Scan0, SeekOrigin.Begin)

                Using output As Stream = file.FileName.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                    Call buf.CopyTo(output)
                    Call output.Flush()
                End Using

                MessageBox.Show($"Export file success!", "Export As File", buttons:=MessageBoxButtons.OK, icon:=MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    Private Sub ShowSummaryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowSummaryToolStripMenuItem.Click
        Dim node = Win7StyleTreeView1.SelectedNode

        If node Is Nothing Then
            Return
        End If

        Dim data As StreamObject = node.Tag

        If data Is Nothing Then
            Return
        Else
            MessageBox.Show(data.ToString, "File Info", buttons:=MessageBoxButtons.OK, icon:=MessageBoxIcon.Information)
        End If
    End Sub
End Class
