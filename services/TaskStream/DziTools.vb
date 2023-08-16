Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Mzkit_win32.BasicMDIForm

Public Module DziTools

    ReadOnly tool As String = $"{App.HOME}/tools/dzitools/dzitool.exe"

    Public Function CreateDziImages(source As String, save_dzi As String) As Boolean
        Dim cli As String = $"/parse --file {source.CLIPath} --export {save_dzi.CLIPath}"
        Dim task As New RunSlavePipeline(tool, cli, workdir:=App.HOME)

        Call TaskProgress.RunAction(
            run:=Sub(p As ITaskProgress)
                     AddHandler task.SetMessage, AddressOf p.SetInfo
                     Call task.Run()
                 End Sub,
            title:="Create image tiles",
            info:=$"Extract tiles from slide source '{source.FileName}'")

        Return save_dzi.FileExists
    End Function
End Module
