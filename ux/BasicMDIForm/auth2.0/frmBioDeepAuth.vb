Imports Microsoft.Web.WebView2.Core

Public Class frmBioDeepAuth

    Private Sub frmBioDeepAuth_Load(sender As Object, e As EventArgs) Handles Me.Load
        WebKit.Init(WebView21)
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        Call WebView21.CoreWebView2.Navigate("https://query.biodeep.cn/login/")
        Call WebKit.DeveloperOptions(WebView21, enable:=False)
    End Sub
End Class