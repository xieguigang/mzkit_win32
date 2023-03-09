Imports BioNovoGene.mzkit_win32.ServiceHub.Manager
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel

Public NotInheritable Class RenderService

    Private Sub New()
    End Sub

    Public Shared Sub Start()
        Dim port As Integer = Net.Tcp.GetFirstAvailablePort(1043)
        Dim slave = TaskStream.CLI.mzblender.FromEnvironment(App.HOME)
        Dim args = slave.GetStartServiceCommandLine(port, App.PID)
        Dim localhost As New RunSlavePipeline(slave.Path, args, workdir:=App.HOME)
        Dim background As Process = localhost.Start()

        Call App.AddExitCleanHook(
            Sub()
                Call Shutdown(port)
                Call background.Kill()
            End Sub)
        Call Hub.Register(New Service With {
            .CPU = 0,
            .Name = "Data Visualization &amp; Rendering",
            .Description = "Background host for do mass spectral data rendering and plot data visualization output png/pdf/svg.",
            .isAlive = True,
            .Memory = 0,
            .PID = background.Id,
            .Port = port,
            .Protocol = "TCP",
            .StartTime = Now.ToString
        })
    End Sub

    Private Shared Sub Shutdown(port As Integer)
        Dim req As New RequestStream(mzblender.Service.protocolHandle, mzblender.Protocol.Shutdown, "exit")
        Call New TcpRequest(port).SendMessage(req)
    End Sub
End Class
