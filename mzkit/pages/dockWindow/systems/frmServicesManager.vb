Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm

Public Class frmServicesManager

    Public ReadOnly Property sourceURL As String
        Get
            Return $"http://127.0.0.1:{Workbench.WebPort}/servicesManager.html"
        End Get
    End Property

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", New ServicesManager)
        Call WebView21.CoreWebView2.Navigate(sourceURL)
        Call WebKit.DeveloperOptions(WebView21, enable:=True)
    End Sub

    Private Sub frmServicesManager_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = Text

        WebKit.Init(WebView21)
    End Sub

    ' 所有需要在JavaScript环境中暴露的对象
    ' 都需要标记上下面的两个自定义属性
    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class ServicesManager

        Public Function GetServicesList() As String
            Return ServiceHub.Manager.Hub.ServicesList.ToArray.GetJson
        End Function
    End Class
End Class