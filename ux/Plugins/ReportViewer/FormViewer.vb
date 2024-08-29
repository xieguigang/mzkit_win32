Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

Public Class FormViewer

    Shared ReadOnly open_evt As New RibbonEventBinding(Workbench.RibbonItems.ButtonReportOpenWorkspace)
    Shared ReadOnly select_evt As New RibbonEventBinding(Workbench.RibbonItems.ButtonReportSelect)

    Dim report As ReportRender
    Dim viewer As ReportViewer
    Dim rawdata As Dictionary(Of String, mzPack)

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
                    .report = report
                }

                Dim rawfiles As Index(Of String) = report.annotation.samplefiles.Indexing

                ' load all mzpack into memory?
                For Each raw As MZWork.Raw In LCMSViewerModule.GetWorkspaceFiles
                    If raw.source.BaseName Like rawfiles Then
                        rawdata(raw.source.BaseName) = raw _
                            .LoadMzpack(Sub(a, b)
                                            Call Workbench.StatusMessage($"[{a}] {b}")
                                        End Sub) _
                            .GetLoadedMzpack
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

Public Class ReportViewer

    Public report As ReportRender

    Public Async Function ShowXic(data_id As String) As Task(Of Boolean)
        Call Workbench.LogText($"show xic data for ion: {data_id}")

        Return True
    End Function

End Class
