Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

Public Class FormViewer

    Shared ReadOnly open_evt As New RibbonEventBinding(Workbench.RibbonItems.ButtonReportOpenWorkspace)
    Shared ReadOnly select_evt As New RibbonEventBinding(Workbench.RibbonItems.ButtonReportSelect)
    Shared ReadOnly showRt_evt As New ToggleEventBinding(Workbench.RibbonItems.ToggleShowRT)

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
        showRt_evt.evt = Sub(flag)
                             If Not refSet.IsNullOrEmpty Then
                                 Call RenderHtmltable()
                             End If
                         End Sub
    End Sub

    Private Sub openWorkspace()
        Dim main As Form = CObj(Workbench.AppHost)

        Using file As New OpenFileDialog With {.Filter = "MZKit workspace for biodeep workflow(*.hdms)|*.hdms"}
            If file.ShowDialog = DialogResult.OK Then
                If TaskProgress.LoadData(Of Boolean)(
                    streamLoad:=Function(p As Action(Of String))
                                    Return main.Invoke(Function() LoadWorkspace(file.FileName, p))
                                End Function,
                    title:="Load BioDeep Annotation Workspace...",
                    info:="Load annotation result data...") Then

                    Call selectIons()
                End If
            End If
        End Using
    End Sub

    Private Function LoadWorkspace(file As String, print As Action(Of String)) As Boolean
        Dim buf As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
        Dim workspace As New AnnotationWorkspace(buf, file)
        Dim pack As AnnotationPack = workspace.LoadMemory

        Call print("load report data...")

        report = New ReportRender(pack)
        viewer = New ReportViewer With {
            .report = report,
            .rawdata = rawdata
        }

        Dim rawfiles As Index(Of String) = report.annotation.samplefiles.Indexing

        Call rawdata.Clear()
        Call print("load lcms rawdata files from the current workspace...")

        ' load all mzpack into memory?
        For Each raw As MZWork.Raw In LCMSViewerModule.GetWorkspaceFiles
            If raw.source.BaseName Like rawfiles Then
                Dim load = raw.LoadMzpack(Sub(a, b) Call Workbench.StatusMessage($"[{a}] {b}"))
                Dim packraw = load.GetLoadedMzpack

                rawdata(raw.source.BaseName) = New mzPack With {
                    .MS = packraw.MS.ToArray,
                    .source = raw.source
                }

                Call print($"load rawdata [{raw.source.BaseName}]")
            Else
                Call print($"skip rawdata [{raw.source.BaseName}]")
            End If
        Next

        Try
            Call workspace.Dispose()
            Call buf.Dispose()
        Catch ex As Exception

        End Try

        Return True
    End Function

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
            Yield $"http://127.0.0.1:{Workbench.WebPort}/vendor/bootstrap-5.3.2-dist/js/bootstrap.bundle.min.js"
        End Get
    End Property

    Dim refSet As String()

    Private Sub RenderHtmltable()
        Dim lines = report.Tabular(refSet, rt_cell:=showRt_evt.Checked).ToArray
        Dim html As New StringBuilder

        Call html.AppendLine("<head>")

        Call html.AppendLine("<meta name='app' content='biodeep_report' />")

        For Each js As String In mzkit_js
            Call html.AppendLine($"<script type=""text/javascript"" src='{js}'></script>")
        Next

        Call html.AppendLine($"<link href='http://127.0.0.1:{Workbench.WebPort}/vendor/bootstrap-5.3.2-dist/css/bootstrap.min.css' rel='stylesheet' crossorigin='anonymous'>")

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

    Private Sub viewMetabolites(config As FormSelectTable)
        refSet = config.GetTargetSet.ToArray

        If refSet.IsNullOrEmpty Then
            Return
        End If

        Call RenderHtmltable()
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

    Public Async Function ShowLcmsScatter(sample_name As String) As Task(Of Boolean)
        Call Workbench.LogText($"load lcms scatter view [{sample_name}]")

        If rawdata.IsNullOrEmpty OrElse Not rawdata.ContainsKey(sample_name) Then
            Return False
        End If

        Await Task.Run(Sub() ShowLcmsScatterTask(sample_name))

        Return True
    End Function

    Private Sub ShowLcmsScatterTask(sample_name As String)
        Call LCMSViewerModule.OpenScatterViewer(Me.rawdata(sample_name), $"MS1 [{sample_name}]", AddressOf onClickMs1)

        For Each raw As MZWork.Raw In LCMSViewerModule.GetWorkspaceFiles
            If raw.source.BaseName = sample_name Then
                Call LCMSViewerModule.SetCurrentWorkFile(raw)

                Exit For
            End If
        Next
    End Sub

    Private Sub onClickMs1(id As String, mz As Double, rt As Double, flag As Boolean)

    End Sub

    Public Async Function ViewSpectral(xcms_id As String, sample As String, db_xref As String) As Task(Of Boolean)
        Call Workbench.LogText($"view lcms msn spectrum: {New Dictionary(Of String, String) From {
             {"xcms_id", xcms_id},
             {"sample", sample},
             {"biodeep_id", db_xref}
        }.GetJson}")

        Dim ion = report.GetIon(xcms_id)

        If ion Is Nothing OrElse ion.biodeep_id <> db_xref Then
            Return False
        End If
        If ion.samplefiles.IsNullOrEmpty OrElse Not ion.samplefiles.ContainsKey(sample) Then
            Return False
        End If

        Dim msn As Ms2Score = ion.samplefiles(sample)
        Dim spectrum As New PeakMs2(ion.name & "_" & ion.adducts, msn.ms2) With {
            .mz = ion.theoretical_mz,
            .file = sample,
            .precursor_type = ion.adducts,
            .rt = ion.rt,
            .scan = xcms_id
        }

        ' view spectrum
        Await Task.Run(
            Sub()
                Call SpectralViewerModule.ViewSpectral(spectrum, ion.formula)
            End Sub)

        Return True
    End Function
End Class
