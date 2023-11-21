Imports BioNovoGene.mzkit_win32.ServiceHub
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
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
        BlenderHost = New Process With {
            .StartInfo = New ProcessStartInfo With {
                .FileName = $"{App.HOME}/plugins\blender\BlenderHost.exe",
                .Arguments = $"/start --port {MSIBlender.port} --master {bindChannel}",
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
            .Port = MSIBlender.port,
            .Protocol = "TCP",
            .StartTime = Now.ToString,
            .HouseKeeping = True,
            .CommandLine = Manager.Service.GetCommandLine(BlenderHost)
        })

        Call WorkStudio.LogCommandLine(BlenderHost.StartInfo.FileName, BlenderHost.StartInfo.Arguments, App.HOME)
    End Sub

    Private Shared Sub Shutdown(port As Integer)
        Dim req As New RequestStream(mzblender.Service.protocolHandle, mzblender.Protocol.Shutdown, "exit")
        Call New TcpRequest(port).SendMessage(req)
    End Sub
End Class
