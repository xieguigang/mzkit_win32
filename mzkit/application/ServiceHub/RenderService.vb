Imports System.IO
Imports System.Threading
Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.ServiceHub
Imports Darwinism.IPC.Networking.Tcp
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Mzkit_win32.BasicMDIForm
Imports TaskStream
Imports IPEndPoint = Microsoft.VisualBasic.Net.IPEndPoint

Public NotInheritable Class RenderService

    Friend Shared BlenderHost As Process
    Friend Shared MSIBlender As IPEndPoint

    Public Shared ReadOnly Property debug As Boolean = False

    Private Sub New()
    End Sub

    Public Shared Sub Start()
        Dim cli As CommandLine = App.CommandLine
        Dim bindChannel As String = App.PID
        Dim debugBlender As Boolean = cli.HavebFlag("--blender")

        If cli.Name.TextEquals("--debug") Then
            MSIBlender = New IPEndPoint("127.0.0.1", cli("--blender") Or TCPExtensions.GetFirstAvailablePort(8000))
            bindChannel = "debug-blender"
            _debug = True
        Else
            MSIBlender = New IPEndPoint("127.0.0.1", TCPExtensions.GetFirstAvailablePort(8000))
        End If

        If debugBlender AndAlso cli.Name.TextEquals("--debug") AndAlso bindChannel = "debug-blender" Then
            ' just debug, do nothing
        Else
            Call Start(bindChannel)
        End If
    End Sub

    Private Shared Sub Start(bindChannel As String)
        Dim mb As Double = MyApplication.buffer_size / ByteSize.MB

        BlenderHost = New Process With {
            .StartInfo = New ProcessStartInfo With {
                .FileName = $"{App.HOME}/plugins\blender\BlenderHost.exe",
                .Arguments = $"/start --port {MSIBlender.port} --master {bindChannel} {If(debug, "--debug", "")} /@set buffer_size={mb}MB",
                .CreateNoWindow = Not MyApplication.debugMode,
                .WindowStyle = If(MyApplication.debugMode, ProcessWindowStyle.Normal, ProcessWindowStyle.Hidden),
                .UseShellExecute = False,
                .RedirectStandardOutput = Not MyApplication.debugMode
            }
        }

        Call BlenderHost.Start()

        If Not MyApplication.debugMode Then
            Call New Thread(Sub() readLines(BlenderHost)).Start()
        End If

        Call ServiceHub.Manager.Hub.RegisterSingle(New Manager.Service With {
            .Name = "MSI Blender",
            .Description = "MS-Imaging blendering backend for mzkit workbench",
            .isAlive = True,
            .PID = BlenderHost.Id,
            .Port = MSIBlender.port,
            .Protocol = "TCP",
            .StartTime = Now.ToString,
            .HouseKeeping = True,
            .CommandLine = Manager.Service.GetCommandLine(BlenderHost)
        })

        Call WorkStudio.LogCommandLine(BlenderHost.StartInfo.FileName, BlenderHost.StartInfo.Arguments, App.HOME)
    End Sub

    Public Shared Sub readLines(host As Process)
        Dim reader As StreamReader = host.StandardOutput

        Do While App.Running AndAlso Not host.HasExited
            Call RunSlavePipeline.ProcessMessage(reader.ReadLine, AddressOf Workbench.LogText, Sub(p, msg) Workbench.LogText($"{msg} ... {p}%"))
        Loop
    End Sub

    Private Shared Sub Shutdown(port As Integer)
        Dim req As New RequestStream(mzblender.Service.protocolHandle, mzblender.Protocol.Shutdown, "exit")
        Call New TcpRequest(port).SendMessage(req)
    End Sub
End Class
