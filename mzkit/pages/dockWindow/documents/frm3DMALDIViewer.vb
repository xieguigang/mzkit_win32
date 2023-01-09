Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Threading
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports Task.Container

Public Class frm3DMALDIViewer

    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class MALDISource

        Public Property source As String

        ReadOnly host As frm3DMALDIViewer

        Sub New(host As frm3DMALDIViewer)
            Me.host = host
        End Sub

        Public Function get_3d_MALDI_url() As String
            Return $"http://127.0.0.1:{host.webPort}/{source.FileName}"
        End Function

        Public Sub open_MALDI_model()
            Using file As New OpenFileDialog With {.Filter = "3D MALDI model(*.maldi)|*.maldi"}
                If file.ShowDialog = DialogResult.OK Then
                    Me.source = file.FileName
                End If
            End Using
        End Sub
    End Class

    Dim localfs As Process
    Dim webPort As Integer = -1
    Dim sourceMALDI As New MALDISource(Me)

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
        sourceMALDI.source = AppEnvironment.get3DMALDIDemoFolder.GetDirectoryFullPath & "/3DMouseKidney.maldi"
    End Sub

    Public Sub LoadModel(maldi As String)
        sourceMALDI.source = maldi
        WebView21.Reload()
    End Sub

    Private Sub startHttp()
        webPort = Net.Tcp.GetFirstAvailablePort(6001)
        localfs = New Process With {
            .StartInfo = New ProcessStartInfo With {
                .FileName = $"{App.HOME}/Rstudio/bin/Rserve.exe",
                .Arguments = $"--listen /wwwroot ""{AppEnvironment.getWebViewFolder}"" /port {webPort} --parent={App.PID} --attach {sourceMALDI.source.ParentPath.CLIPath}",
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

        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", sourceMALDI)
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
        Me.startHttp()
        Me.TabText = Me.Text

        Call WebKit.Init(Me.WebView21)
    End Sub
End Class