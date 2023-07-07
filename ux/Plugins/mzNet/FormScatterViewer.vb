Imports BioNovoGene.Analytical.MassSpectrometry.Math.MoleculeNetworking.PoolData
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.My.JavaScript
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

                     Call Me.Invoke(Sub() Call LoadClusters(p, data.info))
                 End Sub,
            info:=$"Load top {topN} clusters..."
        )
    End Sub

    Private Sub LoadClusters(p As ITaskProgress, clusters As Object())
        Dim precursorList As New List(Of MetaIon)

        For Each obj As Object In clusters.SafeQuery
            Dim js As JavaScriptObject = DirectCast(obj, JavaScriptObject)
            Dim id As String = js!id
            Dim url As String = $"{model.HttpServices}/load_cluster/?id={id}"
            Dim json = Restful.ParseJSON(url.GET)

            If json.code <> 0 Then
                Continue For
            End If

            Dim data As Object() = json.info!metabolites
            Dim metaIons = data.SafeQuery _
                .AsParallel _
                .Select(Function(o) HttpRESTMetadataPool.ParseMetadata(DirectCast(o, JavaScriptObject))) _
                .ToArray
            Dim precursors = metaIons.GroupBy(Function(m) m.mz, offsets:=0.3) _
                .AsParallel _
                .Select(Function(mzset)
                            Return mzset.GroupBy(Function(m) m.rt, offsets:=30)
                        End Function) _
                .IteratesALL

            For Each ion As NamedCollection(Of Metadata) In precursors
                Dim mz As Double = ion.Select(Function(i) i.mz).Average
                Dim rt As Double() = ion.Select(Function(i) i.rt).ToArray

                Call precursorList.Add(New MetaIon With {
                    .id = $"[{id}] {js!annotation} {mz.ToString("F4")}@{(rt.Average / 60).ToString("F2")}min",
                    .mz = mz,
                    .rtmin = rt.Min,
                    .rtmax = rt.Max,
                    .scan_time = rt.Average,
                    .intensity = ion.Length,
                    .metaList = ion.value
                })
            Next
        Next
    End Sub
End Class

Public Class MetaIon : Inherits Meta

    Public Property metaList As Metadata()
    Public Property rtmin As Double
    Public Property rtmax As Double

End Class
