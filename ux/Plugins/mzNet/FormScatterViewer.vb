Imports BioNovoGene.Analytical.MassSpectrometry.Math.MoleculeNetworking.PoolData
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.LCMSViewer

Public Class FormScatterViewer

    Dim WithEvents scatterViewer As PeakScatterViewer
    Dim model As HttpTreeFs

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

    Public Sub LoadClusters(model As HttpTreeFs, topN As Integer)
        Dim url As String = $"{model.HttpServices}/get/cluster/?model_id={model.model_id}&limit_n={topN}"

        Me.model = model

        Call TaskProgress.RunAction(
            run:=Sub(p As ITaskProgress)
                     Dim data = Restful.ParseJSON(url.GET)

                     If data.code <> 0 Then
                         Return
                     End If


                 End Sub,
            info:=$"Load top {topN} clusters..."
        )
    End Sub
End Class

