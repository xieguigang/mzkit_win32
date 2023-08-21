Imports System.ComponentModel
Imports System.Threading
Imports BioNovoGene.mzkit_win32.ServiceHub
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.MSImagingViewerV2.DeepZoomBuilder
Imports RibbonLib.Interop
Imports Task.Container

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

    Shared exportImage As Action

    Private Shared Sub DoWebCapture()
        If Not exportImage Is Nothing Then
            Call exportImage()
        End If
    End Sub

    Public Sub WebInvokeExportImage()
        WebView21.ExecuteScriptAsync("apps.viewer.OpenseadragonSlideViewer.ExportViewImage()")
    End Sub


    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AutoScaleMode = AutoScaleMode.Dpi

        AddHandler ribbonItems.ButtonOpenseadragonWebCapture.ExecuteEvent, Sub() Call DoWebCapture()
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
        Call ServiceHub.Manager.Hub.Register(New Manager.Service With {
            .Name = "Open seadragon",
            .Description = "Http services for host the deep zoom image for open seadragon viewer",
            .isAlive = True,
            .PID = localfs.Id,
            .Port = webPort,
            .CPU = 0,
            .Memory = 0,
            .Protocol = "HTTP 1.0",
            .StartTime = Now.ToString
        })
    End Sub

    Private Sub frmOpenseadragonViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Call localfs.Kill()
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        Do While webPort <= 0
            Call System.Windows.Forms.Application.DoEvents()
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
            Call Workbench.StatusMessage($"[{TabText}] WebView2 developer tools has been enable!")
        End If
    End Sub

    Private Sub frmOpenseadragonViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        frmOpenseadragonViewer_Activated()
        WebKit.Init(Me.WebView21)
    End Sub

    Private Sub frmOpenseadragonViewer_Activated() Handles Me.Activated
        exportImage = AddressOf WebInvokeExportImage
        ribbonItems.MenuOpenseadragon.ContextAvailable = ContextAvailability.Available
        ribbonItems.MenuOpenseadragon.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub frmOpenseadragonViewer_LostFocus(sender As Object, e As EventArgs) Handles Me.LostFocus
        exportImage = Nothing
        ribbonItems.MenuOpenseadragon.ContextAvailable = ContextAvailability.NotAvailable
    End Sub
End Class