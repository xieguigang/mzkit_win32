Imports System.ComponentModel
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm

Public Class frmPluginMgr

    Dim registry As PluginMgr = PluginMgr.Load

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AutoScaleMode = AutoScaleMode.Dpi
    End Sub

    Private Sub frmPluginMgr_Load(sender As Object, e As EventArgs) Handles Me.Load
        WebKit.Init(WebView21)
    End Sub

    Private Sub InitializationCompleted() Handles WebView21.CoreWebView2InitializationCompleted
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", registry)
        Call WebView21.CoreWebView2.Navigate($"http://127.0.0.1:{Globals.WebPort}/pluginManager.html")
        Call WebKit.DeveloperOptions(WebView21, enable:=True)
    End Sub

    Private Sub frmPluginMgr_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Call registry.Save()
    End Sub
End Class