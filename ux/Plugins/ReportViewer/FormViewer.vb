Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

Public Class FormViewer

    Shared ReadOnly open_evt As New RibbonEventBinding(Workbench.RibbonItems.ButtonReportOpenWorkspace)
    Shared ReadOnly select_evt As New RibbonEventBinding(Workbench.RibbonItems.ButtonReportSelect)

    Dim report As ReportRender
    Dim viewer As ReportViewer
    Dim rawdata As New Dictionary(Of String, mzPack)

    Private Sub FormViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call WebKit.Init(WebView21)
        Call FormViewer_Activated()
    End Sub

    Private Sub FormViewer_Activated() Handles Me.Activated
        Workbench.RibbonItems.GroupReport.ContextAvailable = RibbonLib.Interop.ContextAvailability.Active

        open_evt.evt = Sub() Call openWorkspace()
        select_evt.evt = Sub() Call selectIons()
    End Sub

    Private Sub openWorkspace()
        Using file As New OpenFileDialog With {.Filter = "MZKit workspace for biodeep workflow(*.hdms)|*.hdms"}
            If file.ShowDialog = DialogResult.OK Then
                Dim buf As Stream = file.FileName.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                Dim workspace As New AnnotationWorkspace(buf, file.FileName)
                Dim pack As AnnotationPack = workspace.LoadMemory

                report = New ReportRender(pack)
                viewer = New ReportViewer With {
                    .report = report,
                    .rawdata = rawdata
                }

                Dim rawfiles As Index(Of String) = report.annotation.samplefiles.Indexing

                Call rawdata.Clear()

                ' load all mzpack into memory?
                For Each raw As MZWork.Raw In LCMSViewerModule.GetWorkspaceFiles
                    If raw.source.BaseName Like rawfiles Then
                        Dim load = raw.LoadMzpack(Sub(a, b) Call Workbench.StatusMessage($"[{a}] {b}"))
                        Dim packraw = load.GetLoadedMzpack

                        rawdata(raw.source.BaseName) = packraw
                    End If
                Next

                Try
                    Call workspace.Dispose()
                    Call buf.Dispose()
                Catch ex As Exception

                End Try

                Call selectIons()
            End If
        End Using
    End Sub

    Private Sub selectIons()
        If report Is Nothing Then
            Return
        End If

        Dim show As New FormSelectTable

        Call show.SetAnnotation(report.annotation)
        Call InputDialog.Input(Sub(config) viewMetabolites(config), config:=show)
    End Sub

    Private ReadOnly Iterator Property mzkit_js As IEnumerable(Of String)
        Get
            Yield $"http://127.0.0.1:{Workbench.WebPort}/assets/js/linq.js"
            Yield $"http://127.0.0.1:{Workbench.WebPort}/assets/js/mzkit_desktop.js"
        End Get
    End Property

    Private Sub viewMetabolites(config As FormSelectTable)
        Dim html As New StringBuilder
        Dim refSet As String() = config.GetTargetSet.ToArray

        If refSet.IsNullOrEmpty Then
            Return
        End If

        Dim lines = report.Tabular(refSet, rt_cell:=False).ToArray

        Call html.AppendLine("<head>")

        Call html.AppendLine("<meta name='app' content='biodeep_report' />")

        For Each js As String In mzkit_js
            Call html.AppendLine($"<script type=""text/javascript"" src='{js}'></script>")
        Next

        Call html.AppendLine("</head>")

        Call html.AppendLine("<table class='table' style='width:100%;'>")
        Call html.AppendLine("<thead>")
        Call html.AppendLine("<tr>")
        Call html.AppendLine(lines(0))
        Call html.AppendLine("</tr>")
        Call html.AppendLine("</thead>")
        Call html.AppendLine("<tbody>")

        For Each row As String In lines.Skip(1)
            Call html.AppendLine("<tr>")
            Call html.AppendLine(row)
            Call html.AppendLine("</tr>")
        Next

        Call html.AppendLine("</tbody>")
        Call html.AppendLine("</table>")

        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", viewer)
        Call WebView21.NavigateToString(html.ToString)
    End Sub

    Private Sub FormViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Workbench.RibbonItems.GroupReport.ContextAvailable = RibbonLib.Interop.ContextAvailability.NotAvailable
    End Sub

    Private Sub EventDeactivate() Handles Me.Deactivate
        Workbench.RibbonItems.GroupReport.ContextAvailable = RibbonLib.Interop.ContextAvailability.NotAvailable

        open_evt.evt = Nothing
        select_evt.evt = Nothing
    End Sub
End Class

<ClassInterface(ClassInterfaceType.AutoDual)>
<ComVisible(True)>
Public Class ReportViewer

    Friend report As ReportRender
    Friend rawdata As Dictionary(Of String, mzPack)

    Public Async Function ShowXic(data_id As String) As Task(Of Boolean)
        Call Workbench.LogText($"show xic data for ion: {data_id}")

        If rawdata.IsNullOrEmpty Then
            Return False
        End If

        ' get ion by xcms_id -> mz -> xic in each rawdata
        Await Task.Run(Sub() LoadXicTask(data_id))

        Return True
    End Function

    Private Sub LoadXicTask(data_id As String)
        Dim xic As New List(Of NamedCollection(Of ChromatogramTick))
        Dim ion As AlignmentHit = report.GetIon(data_id)
        Dim mz As Double = If(ion Is Nothing, -1, ion.theoretical_mz)
        Dim da As Tolerance = Tolerance.DeltaMass(0.05)

        If mz <= 0 Then
            Return
        End If

        For Each raw In rawdata
            Call xic.Add(New NamedCollection(Of ChromatogramTick) With {
                .name = raw.Key,
                .value = raw.Value.GetXIC(mz, da)
            })
        Next

        ' view xic in viewer
        Call LCMSViewerModule.ShowTICOverlaps(xic.ToArray)
    End Sub

End Class
