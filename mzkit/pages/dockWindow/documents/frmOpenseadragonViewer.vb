Imports System.ComponentModel
Imports System.Threading
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.MSImagingViewerV2.DeepZoomBuilder
Imports Task

Public Class frmOpenseadragonViewer

    Dim dzi As String
    Dim localfs As Process
    Dim webPort As Integer = -1
    Dim sourcefile As String

    Public ReadOnly Property sourceURL As String
        Get
            Return $"http://127.0.0.1:{webPort}/openseadragon.html"
        End Get
    End Property

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AutoScaleMode = AutoScaleMode.Dpi
    End Sub

    Public Sub LoadSlide(tiff As String)
        If tiff.ExtensionSuffix("xml", "dzi") Then
            dzi = tiff
        Else
            dzi = TempFileSystem.GetAppSysTempFile(".dzi", sessionID:=App.PID.ToHexString.MD5.Substring(2, 6) & "-" & tiff.BaseName, prefix:="deep_zoom_")
            Call New DeepZoomCreator().CreateSingleComposition(tiff, dzi, ImageType.Jpeg)
        End If

        Call startHttp()
    End Sub

    Private Sub startHttp()
        webPort = Net.Tcp.GetFirstAvailablePort(6001)
        localfs = New Process With {
            .StartInfo = New ProcessStartInfo With {
                .FileName = $"{App.HOME}/Rstudio/bin/Rserve.exe",
                .Arguments = $"--listen /wwwroot ""{AppEnvironment.getWebViewFolder}"" /port {webPort} --attach ""{dzi.ParentPath}"" --parent={App.PID}",
                .CreateNoWindow = True,
                .WindowStyle = ProcessWindowStyle.Hidden,
                .UseShellExecute = True
            }
        }

        Call localfs.Start()
        Call App.AddExitCleanHook(Sub() Call localfs.Kill())
    End Sub

    Private Sub frmOpenseadragonViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Call localfs.Kill()
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        Do While webPort <= 0
            Call Application.DoEvents()
            Call Thread.Sleep(10)
        Loop

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

    Private Sub frmOpenseadragonViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        frmHtmlViewer.Init(Me.WebView21)
    End Sub
End Class