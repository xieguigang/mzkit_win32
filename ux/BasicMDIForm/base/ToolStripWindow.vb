Public Class ToolStripWindow

    Private Sub ToolStripWindow_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call ApplyVsTheme(ToolStrip1)
    End Sub

    Public Function GetMainToolStrip() As ToolStrip
        Return ToolStrip1
    End Function
End Class