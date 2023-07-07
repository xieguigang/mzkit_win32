Imports Mzkit_win32.LCMSViewer

Public Class FormScatterViewer

    Dim WithEvents scatterViewer As PeakScatterViewer

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        scatterViewer = New PeakScatterViewer With {
            .Dock = DockStyle.Fill
        }

        Call Controls.Add(scatterViewer)
    End Sub

    Private Sub FormScatterViewer_Load(sender As Object, e As EventArgs) Handles Me.Load

    End Sub
End Class