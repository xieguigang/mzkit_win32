Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Parallel

Public Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/start")>
    <Description("Start the data visualization rendering background service for the mass spectral data rendering.")>
    <Usage("/start --port <tcp_port> --master <mzkit_win32 PID>")>
    Public Function StartService(args As CommandLine) As Integer
        Dim port As Integer = args <= "--port"
        Dim master As Integer = args <= "--master"
        Dim localhost As New Service(port)

        If master > 0 Then
            Call BackgroundTaskUtils.BindToMaster(parentId:=master, kill:=localhost)
        End If

        Return localhost.Run
    End Function
End Module
