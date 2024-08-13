Imports System.ComponentModel
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.BioDeep.MSEngine
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

Public Class FormViewer

    Shared ReadOnly open_evt As New RibbonEventBinding(Workbench.RibbonItems.ButtonReportOpenWorkspace)
    Shared ReadOnly select_evt As New RibbonEventBinding(Workbench.RibbonItems.ButtonReportSelect)

    Dim report As ReportRender

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

                Try
                    Call workspace.Dispose()
                    Call buf.Dispose()
                Catch ex As Exception

                End Try
            End If
        End Using
    End Sub

    Private Sub selectIons()
        If report Is Nothing Then
            Return
        End If

        Dim show As New FormSelectTable
        Call show.SetAnnotation(report.annotation)

        InputDialog.Input(
            Sub(config)
                Dim html As String = report.HtmlTable(config.GetTargetSet, False)
                WebView21.NavigateToString(html)
            End Sub, config:=show)
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
