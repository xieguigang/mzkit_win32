Imports System.ComponentModel
Imports Mzkit_win32.BasicMDIForm

Public Class FormViewer

    Private Sub FormViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call WebKit.Init(WebView21)
        Call FormViewer_Activated()
    End Sub

    Private Sub FormViewer_Activated() Handles Me.Activated
        Workbench.RibbonItems.GroupReport.ContextAvailable = RibbonLib.Interop.ContextAvailability.Active
    End Sub

    Private Sub FormViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Workbench.RibbonItems.GroupReport.ContextAvailable = RibbonLib.Interop.ContextAvailability.NotAvailable
    End Sub
End Class
