Imports System.ComponentModel
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.MSImagingViewerV2.DeepZoomBuilder
Imports Task

Public Class frmOpenseadragonViewer

    Dim dzi As String
    Dim localfs As Process
    Dim webPort As Integer
    Dim sourcefile As String

    Public ReadOnly Property sourceURL As String
        Get
            Return $"http://127.0.0.1:{webPort}/openseadragon.html"
        End Get
    End Property

    Public Sub LoadSlide(tiff As String)
        dzi = TempFileSystem.GetAppSysTempFile(".dzi", sessionID:=App.PID.ToHexString.MD5.Substring(2, 6) & "-" & tiff.BaseName, prefix:="deep_zoom_")

        If tiff.ExtensionSuffix("xml", "dzi") Then
            Call tiff.ReadAllText.SaveTo(dzi)
            Call New Directory($"{tiff.ParentPath}/{tiff.BaseName}_files/").CopyTo(dzi.ParentPath & $"/{dzi.BaseName}_files/")
        Else
            Call New DeepZoomCreator().CreateSingleComposition(tiff, dzi, ImageType.Jpeg)
        End If

        Call startHttp()
        Call copyHtmls()
        Call frmHtmlViewer.Init(Me.WebView21)
    End Sub

    Private Sub copyHtmls()
        Call $"{AppEnvironment.getWebViewFolder}/openseadragon.html".ReadAllText.SaveTo($"{dzi.ParentPath}/openseadragon.html")
        Call New Directory($"{AppEnvironment.getWebViewFolder}/openseadragon/").CopyTo($"{dzi.ParentPath}/openseadragon/")
    End Sub

    Private Sub startHttp()
        webPort = Net.Tcp.GetFirstAvailablePort(6000)
        localfs = New Process With {
            .StartInfo = New ProcessStartInfo With {
                .FileName = $"{App.HOME}/Rstudio/bin/Rserve.exe",
                .Arguments = $"--listen /wwwroot ""{dzi.ParentPath}"" /port {webPort}",
                .CreateNoWindow = True,
                .WindowStyle = ProcessWindowStyle.Hidden,
                .UseShellExecute = True
            }
        }

        Call localfs.Start()
    End Sub

    Private Sub frmOpenseadragonViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Call localfs.Kill()
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        Call WebView21.CoreWebView2.AddHostObjectToScript("dzi", $"http://127.0.0.1:{webPort}/{dzi.FileName}")
        Call WebView21.CoreWebView2.Navigate(sourceURL)
        Call DeveloperOptions(enable:=True)
    End Sub

    Public Sub DeveloperOptions(enable As Boolean)
        WebView21.CoreWebView2.Settings.AreDevToolsEnabled = enable
        WebView21.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = enable
        WebView21.CoreWebView2.Settings.AreDefaultContextMenusEnabled = enable
        If enable Then
            Call MyApplication.host.showStatusMessage($"[{TabText}] WebView2 developer tools has been enable!")
        End If
    End Sub
End Class