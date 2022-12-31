Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm

Public Class frmPluginMgr

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AutoScaleMode = AutoScaleMode.Dpi
    End Sub

    Private Sub frmPluginMgr_Load(sender As Object, e As EventArgs) Handles Me.Load
        WebKit.Init(WebView21)
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        Call WebView21.CoreWebView2.Navigate("")
        Call WebKit.DeveloperOptions(WebView21, enable:=True)
    End Sub
End Class