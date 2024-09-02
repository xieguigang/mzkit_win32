Imports Mzkit_win32.LCMSViewer

Public Class FormXicViewer

    Private Sub FormXicViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim viewer As New ROIGroupViewer

        Controls.Add(viewer)
        viewer.Dock = DockStyle.Fill
    End Sub
End Class