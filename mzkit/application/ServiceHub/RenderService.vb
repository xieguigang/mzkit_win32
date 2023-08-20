Imports BioNovoGene.mzkit_win32.ServiceHub
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports IPEndPoint = Microsoft.VisualBasic.Net.IPEndPoint

Public NotInheritable Class RenderService

    Friend Shared BlenderHost As Process
    Friend Shared MSIBlender As IPEndPoint

    Private Sub New()
    End Sub

    Public Shared Sub Start()
        MSIBlender = New IPEndPoint("127.0.0.1", TCPExtensions.GetFirstAvailablePort(8000))
        BlenderHost = New Process With {
            .StartInfo = New ProcessStartInfo With {
                .FileName = $"{App.HOME}/plugins\blender\BlenderHost.exe",
                .Arguments = $"/start --port {MSIBlender.port} --master {App.PID}",
                .CreateNoWindow = True,
                .WindowStyle = ProcessWindowStyle.Hidden,
                .UseShellExecute = True
            }
        }

        Call BlenderHost.Start()
        Call ServiceHub.Manager.Hub.RegisterSingle(New Manager.Service With {
            .Name = "MSI Blender",
            .Description = "MS-Imaging blendering backend for mzkit workbench",
            .isAlive = True,
            .PID = BlenderHost.Id,
            .port = MSIBlender.port,
            .Protocol = "TCP",
            .StartTime = Now.ToString
        })
    End Sub

    Private Shared Sub Shutdown(port As Integer)
        Dim req As New RequestStream(mzblender.Service.protocolHandle, mzblender.Protocol.Shutdown, "exit")
        Call New TcpRequest(port).SendMessage(req)
    End Sub
End Class
