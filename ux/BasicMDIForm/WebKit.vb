Imports Microsoft.Web.WebView2.Core
Imports Microsoft.Web.WebView2.WinForms

Public Module WebKit

    Public Sub DeveloperOptions(WebView21 As WebView2, enable As Boolean, Optional TabText As String = "WebKit")
        WebView21.CoreWebView2.Settings.AreDevToolsEnabled = enable
        WebView21.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = enable
        WebView21.CoreWebView2.Settings.AreDefaultContextMenusEnabled = enable

        If enable Then
            Call Workbench.AppHost.StatusMessage($"[{TabText}] WebView2 developer tools has been enable!")
        End If
    End Sub

    Public Async Sub Init(WebView21 As WebView2)
        Dim userDataFolder = (App.ProductProgramData & "/.webView2_cache/").GetDirectoryFullPath
        Dim env = Await CoreWebView2Environment.CreateAsync(Nothing, userDataFolder)

        Call Workbench.AppHost.StatusMessage($"set webview2 cache at '{userDataFolder}'.")

        Await WebView21.EnsureCoreWebView2Async(env)
    End Sub

    Private Sub Wait()

    End Sub
End Module