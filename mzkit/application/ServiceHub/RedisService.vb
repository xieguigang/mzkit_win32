Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.ServiceHub.Manager
Imports Microsoft.VisualBasic.ApplicationServices
Imports Task
Imports TaskStream

''' <summary>
''' 因为是常驻内存的数据库服务，所以在这里就只使用一个静态实例对象来开发操作接口
''' </summary>
Public NotInheritable Class RedisService

    Private Sub New()
    End Sub

    Public Shared Sub Start()
        Dim Rscript As String = RscriptPipelineTask.GetRScript("../services/mzwork.Redis.R")
        Dim port As Integer = Net.Tcp.GetFirstAvailablePort(15003)
        Dim localRedis = New Process With {
            .StartInfo = New ProcessStartInfo With {
                .FileName = $"{App.HOME}/Rstudio/bin/Rscript.exe",
                .Arguments = $"{Rscript.CLIPath} --port {port} --master {App.PID} --SetDllDirectory {TaskEngine.redisDll.ParentPath.CLIPath}",
                .CreateNoWindow = Not MyApplication.debugMode,
                .WindowStyle = If(MyApplication.debugMode, ProcessWindowStyle.Normal, ProcessWindowStyle.Hidden),
                .UseShellExecute = True
            }
        }

        Call localRedis.Start()
        Call App.AddExitCleanHook(Sub() Call localRedis.Kill())
        Call Hub.Register(New Service With {
            .CPU = 0,
            .Name = "mzPack Redis",
            .Description = "In-memory redis database services for view mzpack LC-MS raw data file.",
            .isAlive = True,
            .Memory = 0,
            .PID = localRedis.Id,
            .Port = port,
            .Protocol = "TCP",
            .StartTime = Now.ToString,
            .HouseKeeping = True,
            .CommandLine = Service.GetCommandLine(localRedis)
        })

        Call WorkStudio.LogCommandLine(localRedis)
    End Sub
End Class
