﻿Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Threading
Imports BioNovoGene.mzkit_win32.ServiceHub
Imports HEView
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.BSON
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.MSImagingViewerV2.DeepZoomBuilder
Imports RibbonLib.Interop
Imports Task.Container
Imports TaskStream
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmOpenseadragonViewer

    Dim dzi As String
    Dim dziIndex As String
    Dim localfs As Process
    Dim webPort As Integer = -1
    Dim sourcefile As String

    Public ReadOnly Property sourceURL As String
        Get
            Return $"http://127.0.0.1:{webPort}/openseadragon.html"
        End Get
    End Property

    Public Sub WebInvokeExportImage()
        WebView21.ExecuteScriptAsync("apps.viewer.OpenseadragonSlideViewer.ExportViewImage()")
    End Sub

    Public Sub SwitchToFullScreen()
        Me.DockState = DockState.Float
        FormBorderStyle = FormBorderStyle.None
        TopMost = True
        WindowState = FormWindowState.Maximized
    End Sub

    Public Sub ExportSlidePackFile()
        Using file As New SaveFileDialog With {.Filter = "MZKit Slide StreamPack File(*.hds)|*.hds"}
            If file.ShowDialog = DialogResult.OK Then
                Select Case dzi.ExtensionSuffix
                    Case "hds"
                        Call dzi.FileCopy(file.FileName)
                    Case Else
                        ' dzi file
                        Dim dir As Directory = Directory.FromLocalFileSystem(dzi.ParentPath)
                        Dim pack As New StreamPack(file.OpenFile,, meta_size:=8 * 1024 * 1024)

                        For Each path As String In dir.GetFiles
                            Dim rel As String = "/" & dir.GetRelativePath(path)
                            Dim open As Byte() = path.ReadBinary
                            Dim s = pack.OpenFile(rel, IO.FileMode.OpenOrCreate, IO.FileAccess.Write)

                            Call s.Write(open, Scan0, open.Length)
                            Call s.Flush()
                            Call s.Dispose()
                        Next

                        Call DirectCast(pack, IFileSystemEnvironment).WriteText(dzi.FileName, "/index.txt")
                        Call pack.Dispose()
                End Select

                Call MessageBox.Show($"The slide file pack save to {file.FileName} success!",
                                     "Export Slide Success",
                                     MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    Shared ReadOnly exportImage As New RibbonEventBinding(ribbonItems.ButtonOpenseadragonWebCapture)
    Shared ReadOnly fullScreen As New RibbonEventBinding(ribbonItems.ButtonViewerFullScreen)
    Shared ReadOnly exportPack As New RibbonEventBinding(ribbonItems.ButtonExportSlidePack)
    Shared ReadOnly scanCells As New RibbonEventBinding(ribbonItems.ButtonScanSingleCells)

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AutoScaleMode = AutoScaleMode.Dpi
        ribbonItems.ButtonViewerFullScreen.Enabled = False
    End Sub

    Public Sub LoadSlide(tiff As String)
        If tiff.ExtensionSuffix("xml", "dzi", "hds") Then
            dzi = tiff
        Else
            dzi = TempFileSystem.GetAppSysTempFile(".dzi", sessionID:=App.PID.ToHexString.MD5.Substring(2, 6) & "-" & tiff.BaseName, prefix:="deep_zoom_")
            Call New DeepZoomCreator().CreateSingleComposition(tiff, dzi, ImageType.Jpeg)
        End If

        Call startHttp()
    End Sub

    Private Sub startHttp()
        Dim res As String

        If dzi.ExtensionSuffix("hds") Then
            res = dzi

            Using pack As StreamPack = StreamPack.OpenReadOnly(dzi)
                Try
                    dziIndex = DirectCast(pack, IFileSystemEnvironment) _
                        .ReadAllText("/index.txt") _
                        .DoCall(AddressOf Strings.Trim) _
                        .BaseName
                Catch ex As Exception
                    Call App.LogException(ex)
                    Call Workbench.Warning("error while open the slide image file!")
                    Return
                End Try
            End Using
        Else
            res = dzi.ParentPath
            dziIndex = dzi.BaseName
        End If

        webPort = Net.Tcp.GetFirstAvailablePort(6001)
        localfs = New Process With {
            .StartInfo = New ProcessStartInfo With {
                .FileName = $"{App.HOME}/Rstudio/bin/Rserve.exe",
                .Arguments = $"--listen /wwwroot ""{AppEnvironment.getWebViewFolder}"" /port {webPort} --attach ""{res}"" --parent={App.PID}",
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
            .StartTime = Now.ToString,
            .CommandLine = Manager.Service.GetCommandLine(localfs)
        })

        Call WorkStudio.LogCommandLine(localfs)
    End Sub

    Private Sub frmOpenseadragonViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Call localfs.Kill()
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        Do While webPort <= 0
            Call System.Windows.Forms.Application.DoEvents()
            Call Thread.Sleep(10)
        Loop

        Call WebView21.CoreWebView2.AddHostObjectToScript("dzi", $"http://127.0.0.1:{webPort}/{dziIndex}.dzi")
        ' Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", New WebRunner)
        Call WebView21.CoreWebView2.Navigate(sourceURL)
        Call WebKit.DeveloperOptions(WebView21, enable:=True, TabText)
    End Sub

    Private Sub frmOpenseadragonViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        DoActivated()
        WebKit.Init(Me.WebView21)
    End Sub

    Private Async Sub scanCellTask()
        Dim datauri As String = Await WebView21.ExecuteScriptAsync("apps.viewer.OpenseadragonSlideViewer.CaptureSlideImage()")

        Call WebRunner.ProcessImage(datauri)
    End Sub

    Public Sub DoActivated()
        exportImage.evt = AddressOf WebInvokeExportImage
        fullScreen.evt = AddressOf SwitchToFullScreen
        exportPack.evt = AddressOf ExportSlidePackFile
        scanCells.evt = AddressOf scanCellTask

        ribbonItems.MenuOpenseadragon.ContextAvailable = ContextAvailability.Available
        ribbonItems.MenuOpenseadragon.ContextAvailable = ContextAvailability.Active
    End Sub

    Public Sub DoLostFocus()
        exportImage.evt = Nothing
        fullScreen.evt = Nothing
        exportPack.evt = Nothing
        scanCells.evt = Nothing

        ribbonItems.MenuOpenseadragon.ContextAvailable = ContextAvailability.NotAvailable
    End Sub

    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class WebRunner

        Public Shared Sub ProcessImage(imgUri As String)
            Dim data As DataURI = DataURI.URIParser(imgUri)
            Dim img As String = TempFileSystem.GetAppSysTempFile(".jpg", sessionID:=App.PID, prefix:="capture_")

            Call data.ToStream.FlushStream(img)

            Dim output = RscriptProgressTask.ScanHESingleCells(img)
            Dim table As frmTableViewer = VisualStudio.ShowDocument(Of frmTableViewer)(title:="Cell Scan Result")
            Dim cells As CellScan() = BSONFormat.Load(output.ReadBinary) _
                .ToJsonArray _
                .CreateObject(Of CellScan())(False)

            table.AppSource = GetType(WebRunner)
            table.InstanceGuid = Guid.NewGuid.ToString
            table.SourceName = "Cell Scan Result"
            table.ViewRow = Sub(row)
                            End Sub
            table.LoadTable(
                Sub(grid)
                    Dim v As Object()

                    Call grid.Columns.Add("x", GetType(Single))
                    Call grid.Columns.Add("y", GetType(Single))
                    Call grid.Columns.Add("area", GetType(Double))
                    Call grid.Columns.Add("ratio", GetType(Double))
                    Call grid.Columns.Add("points", GetType(Integer))
                    Call grid.Columns.Add("width", GetType(Double))
                    Call grid.Columns.Add("height", GetType(Double))
                    Call grid.Columns.Add("density", GetType(Double))
                    Call grid.Columns.Add("moran-I", GetType(Double))
                    Call grid.Columns.Add("pvalue", GetType(Double))

                    For Each cell As CellScan In cells
                        v = New Object() {
                            cell.physical.X, cell.physical.Y,
                            cell.area,
                            cell.ratio,
                            cell.points,
                            cell.width,
                            cell.height,
                            cell.density,
                            cell.moranI,
                            cell.pvalue
                        }

                        Call grid.Rows.Add(v)
                        Call System.Windows.Forms.Application.DoEvents()
                    Next
                End Sub)
        End Sub
    End Class
End Class