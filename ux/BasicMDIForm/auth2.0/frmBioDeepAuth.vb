Imports Microsoft.VisualBasic.Net.Http
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

    Private Async Sub WebView21_NavigationStarting(sender As Object, e As CoreWebView2NavigationStartingEventArgs) Handles WebView21.NavigationStarting
        Dim url As New URL(e.Uri)

        'If url.hostName <> "127.0.0.1" AndAlso url.hostName <> "localhost" Then
        '    e.Cancel = True
        '    Process.Start(e.Uri)
        'End If
        If url.hostName = "query.biodeep.cn" AndAlso (url.path.StringEmpty OrElse url.path = "/") Then
            ' login success
            ' get session id from cookies
            Dim cookies = Await WebView21.CoreWebView2.CookieManager.GetCookiesAsync("https://query.biodeep.cn")

            For Each cookie In cookies
                If cookie.Name = "PHPSESSID" Then
                    e.Cancel = True

                    Call Workbench.SetSessionId(cookie.Value)
                    Call Close()
                End If
            Next
        End If
    End Sub
End Class