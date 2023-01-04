Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm

Public Class PkgPage

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        AutoScaleMode = AutoScaleMode.Dpi
    End Sub

    Private Sub PkgPage_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = "Create MZKit Plugin Package"

        DoubleBuffered = True
        WebKit.Init(WebView21)
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        Call WebView21.CoreWebView2.Navigate($"http://127.0.0.1:{Workbench.WebPort}/pluginPkgTool.html")
        Call WebKit.DeveloperOptions(WebView21, enable:=False)
    End Sub
End Class
