Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Threading
Imports BioNovoGene.mzkit_win32.ServiceHub.Manager
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.Container
Imports RibbonLib.Interop
Imports SMRUCC.genomics.Data.RCSB.PDB
Imports TaskStream

Public Class frmMolstarViewer

    Dim localfs As Process
    Dim webPort As Integer = -1
    Dim pdb As PDB

    Public ReadOnly Property sourceURL As String
        Get
            Return $"http://127.0.0.1:{webPort}/molstar/index.html"
        End Get
    End Property

    ' 所有需要在JavaScript环境中暴露的对象
    ' 都需要标记上下面的两个自定义属性
    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class DataSource

    End Class

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        AutoScaleMode = AutoScaleMode.Dpi
    End Sub

    Private Sub startHttp()
        webPort = Net.Tcp.GetFirstAvailablePort(9001)
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
        Call Hub.Register(New Service With {
            .CPU = 0,
            .Name = "molstar molecule viewer",
            .Description = "Host the molstar molecular viewer model data(pdb files) read/loading from the local filesystem, and then rendering on the 3d model viewer.",
            .isAlive = True,
            .Memory = 0,
            .PID = localfs.Id,
            .Port = webPort,
            .Protocol = "HTTP 1.0",
            .StartTime = Now.ToString,
            .CommandLine = Service.GetCommandLine(localfs)
        })

        Call WorkStudio.LogCommandLine(localfs)
    End Sub

    Shared ReadOnly openButton As New RibbonEventBinding(ribbonItems.ButtonMolmilOpenFile)
    Shared ReadOnly resetCameraButton As New RibbonEventBinding(ribbonItems.ButtonMolstarResetCamera)
    Shared ReadOnly clearCanvasButton As New RibbonEventBinding(ribbonItems.ButtonMolstarClearCanvas)
    Shared ReadOnly snapshotButton As New RibbonEventBinding(ribbonItems.ButtonMolstarMakeSnapshot)

    Private Sub frm3DMALDIViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Call localfs.Kill()

        ribbonItems.MenuMolmil.ContextAvailable = ContextAvailability.NotAvailable
    End Sub

    Private Sub frm3DMALDIViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call startHttp()
        Call WebKit.Init(Me.WebView21)
        Call GetActivated()
    End Sub

    Private Sub OpenPdb()
        Using file As New OpenFileDialog With {.Filter = "Protein Structure Data(*.pdb)|*.pdb"}
            If file.ShowDialog = DialogResult.OK Then
                Dim pdb_txt As String = file.FileName.ReadAllText
                ' 发送消息到 JavaScript
                Dim jsonString As String

                Call ProgressSpinner.DoLoading(
                    Sub()
                        ' 自动处理特殊字符
                        jsonString = pdb_txt.GetJson

                        Me.Invoke(Sub()
                                      Me.pdb = PDB.Load(file.FileName)
                                  End Sub)
                    End Sub)

                WebView21.CoreWebView2.PostWebMessageAsString(jsonString)
            End If
        End Using
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        Do While webPort <= 0
            Call System.Windows.Forms.Application.DoEvents()
            Call Thread.Sleep(10)
        Loop

        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", New DataSource)
        Call WebView21.CoreWebView2.Navigate(sourceURL)
        Call WebKit.DeveloperOptions(WebView21, enable:=True, TabText:=TabText)
    End Sub

    Private Sub resetCamera()
        Call WebView21.CoreWebView2.ExecuteScriptAsync("viewer.resetCamera();")
    End Sub

    Private Sub clearCanvas()
        Call WebView21.CoreWebView2.ExecuteScriptAsync("viewer.clear();")
    End Sub

    Private Sub takeSnapshot()
        Call WebView21.CoreWebView2.ExecuteScriptAsync("requestSnapshotImage();")
    End Sub

    Private Sub frmMolstarViewer_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        openButton.evt = Nothing
        resetCameraButton.evt = Nothing
        clearCanvasButton.evt = Nothing
        snapshotButton.evt = Nothing

        ribbonItems.MenuMolmil.ContextAvailable = ContextAvailability.NotAvailable
    End Sub

    Private Sub GetActivated() Handles Me.Activated
        ribbonItems.MenuMolmil.ContextAvailable = ContextAvailability.Active

        snapshotButton.evt = AddressOf takeSnapshot
        openButton.evt = AddressOf OpenPdb
        resetCameraButton.evt = AddressOf resetCamera
        clearCanvasButton.evt = AddressOf clearCanvas
    End Sub

    Private Sub frmMolstarViewer_GotFocus(sender As Object, e As EventArgs) Handles Me.GotFocus
        Call GetActivated()
    End Sub
End Class