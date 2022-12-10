Imports System.ComponentModel
Imports System.Threading
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.Web.WebView2.Core
Imports Task

Public Class frm3DMALDIViewer

    Dim sourceMALDI As String
    Dim localfs As Process
    Dim webPort As Integer = -1

    Public ReadOnly Property sourceURL As String
        Get
            Return $"http://127.0.0.1:{webPort}/3d-maldi.html"
        End Get
    End Property

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AutoScaleMode = AutoScaleMode.Dpi
    End Sub

    Public Sub LoadModel(maldi As String)
        sourceMALDI = maldi

    End Sub

    Private Sub startHttp()
        webPort = Net.Tcp.GetFirstAvailablePort(6001)
        localfs = New Process With {
            .StartInfo = New ProcessStartInfo With {
                .FileName = $"{App.HOME}/Rstudio/bin/Rserve.exe",
                .Arguments = $"--listen /wwwroot ""{AppEnvironment.getWebViewFolder}"" /port {webPort} --parent={App.PID}",
                .CreateNoWindow = True,
                .WindowStyle = ProcessWindowStyle.Hidden,
                .UseShellExecute = True
            }
        }

        Call localfs.Start()
        Call App.AddExitCleanHook(Sub() Call localfs.Kill())
    End Sub

    Private Sub frm3DMALDIViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Call localfs.Kill()
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        Do While webPort <= 0
            Call Application.DoEvents()
            Call Thread.Sleep(10)
        Loop

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

    Private Sub frm3DMALDIViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        frmHtmlViewer.Init(Me.WebView21)
        Me.TabText = Me.Text
    End Sub
End Class