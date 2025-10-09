﻿Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Threading
Imports BioNovoGene.mzkit_win32.ServiceHub.Manager
Imports Galaxy.Workbench
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.Web.WebView2.Core
Imports SMRUCC.DICOM.LASer
Imports SMRUCC.DICOM.LASer.Model
Imports SMRUCC.DICOM.NRRD
Imports Task.Container
Imports TaskStream

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
            Return $"http://127.0.0.1:{host.webPort}/cloud.js"
        End Function

        Public Sub open_MALDI_model()
            Using file As New OpenFileDialog With {.Filter = "3D MALDI model(*.maldi)|*.maldi|NRRD Raster Image(*.nrrd)|*.nrrd"}
                If file.ShowDialog = DialogResult.OK Then

                End If
            End Using
        End Sub
    End Class

    Dim localfs As Process
    Dim webPort As Integer = -1
    Dim sourceMALDI As New MALDISource(Me)
    Dim modelTempDir As String = TempFileSystem.GetAppSysTempFile(".js", sessionID:=App.PID.ToString.MD5, prefix:="potree_las_model_").ParentPath

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
        DockContextMenuStrip1.Items.Add(OpenDeveloperToolToolStripMenuItem)
        DockContextMenuStrip1.Items.Add(RefreshToolStripMenuItem)
    End Sub

    Public Sub LoadModel(maldi As String)
        If maldi.ExtensionSuffix("nrrd") Then
            Dim nrrd = New FileReader(maldi.Open(IO.FileMode.Open, doClear:=False, [readOnly]:=True)).LoadRaster

            If TypeOf nrrd Is RasterImage Then
                Return
            Else
                Dim points As LasPoint() = DirectCast(nrrd, RasterPointCloud) _
                    .GetPointCloud(Of LasPoint)(skipZero:=True) _
                    .ToArray

                Call PotreeModel.ExportModel(points, dir:=modelTempDir)
            End If
        End If

        Try
            sourceMALDI.source = maldi
            WebView21.Reload()
        Catch ex As Exception

        End Try
    End Sub

    Private Sub startHttp()
        webPort = Net.Tcp.GetFirstAvailablePort(6001)
        localfs = New Process With {
            .StartInfo = New ProcessStartInfo With {
                .FileName = $"{App.HOME}/Rstudio/bin/Rserve.exe",
                .Arguments = $"--listen /wwwroot ""{AppEnvironment.getWebViewFolder}"" /port {webPort} --parent={App.PID} --attach {modelTempDir.CLIPath}",
                .CreateNoWindow = True,
                .WindowStyle = ProcessWindowStyle.Hidden,
                .UseShellExecute = True
            }
        }

        Call localfs.Start()
        Call App.AddExitCleanHook(Sub() Call localfs.Kill())
        Call Hub.Register(New Service With {
            .CPU = 0,
            .Name = "MALDI scan in 3D",
            .Description = "Host the 3d model data read/loading from the local filesystem, and then rendering on the 3d model viewer.",
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

    Private Sub frm3DMALDIViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Call localfs.Kill()
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        Do While webPort <= 0
            Call System.Windows.Forms.Application.DoEvents()
            Call Thread.Sleep(10)
        Loop

        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", sourceMALDI)
        Call WebView21.CoreWebView2.Navigate(sourceURL)
        Call WebViewLoader.DeveloperOptions(WebView21, enable:=True, TabText:=TabText)
    End Sub

    Private Sub frm3DMALDIViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = Me.Text

        Call startHttp()
        Call WebViewLoader.Init(Me.WebView21)
    End Sub

    Private Sub OpenDeveloperToolToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenDeveloperToolToolStripMenuItem.Click
        WebView21.CoreWebView2.OpenDevToolsWindow()
    End Sub

    Private Sub RefreshToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem.Click
        WebView21.Reload()
    End Sub
End Class