Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Mzkit_win32.BasicMDIForm

Public Class proteowizardTask

    Shared ReadOnly msconvert As String

    Shared Sub New()
        msconvert = $"{App.HOME}/tools/proteowizard/msconvert.exe"
    End Sub

    Public Shared Sub ConvertWiffMRM(wiff As String, output As String)
        Dim cli As String = $"{wiff.CLIPath} --zlib -o {output.CLIPath}"
        Dim pipeline As New RunSlavePipeline(msconvert, cli)

        Call WorkStudio.LogCommandLine(msconvert, cli, App.CurrentDirectory)
        Call Workbench.LogText(pipeline.CommandLine)
        Call TaskProgress.RunAction(
            run:=Sub(p)
                     Call p.SetProgressMode()

                     AddHandler pipeline.SetMessage, AddressOf p.SetInfo
                     AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                     AddHandler pipeline.Finish, AddressOf p.TaskFinish

                     Call pipeline.Run()
                 End Sub,
            title:="Make Convert of the Sciex Wiff...",
            info:="Make conversion of the sciex wiff experiment batch data...")
    End Sub
End Class
