Imports WeifenLuo.WinFormsUI.Docking

Partial Public Class ImageDocumentWindow
    Inherits DockContent

    Public Property Main As FormEditMain

    Public Sub New()
        InitializeComponent()
        AutoScaleMode = AutoScaleMode.Dpi
        DockAreas = DockAreas.Document Or DockAreas.Float
    End Sub

    Private Sub ImageDocumentWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        pictureBox1.AllowDrop = True
    End Sub

    Private Sub pictureBox1_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs) Handles pictureBox1.DragDrop
        Call Main.TryOpen(CType(e.Data.GetData(DataFormats.FileDrop, True), String()))
    End Sub

    Private Sub pictureBox1_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs) Handles pictureBox1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.All
        Else
            e.Effect = DragDropEffects.None
        End If
    End Sub

    Private Sub pictureBox1_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) Handles pictureBox1.MouseDoubleClick
        Call Main.openToolStripMenuItem_Click(sender, e)
    End Sub
End Class

