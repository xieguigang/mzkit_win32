Imports System.Windows.Forms
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Text
Imports Mzkit_win32.BasicMDIForm

Public NotInheritable Class MSConvertTask

    Private Sub New()
    End Sub

    Public Shared Sub MergeMultipleSlides(msData As String(),
                                          layoutData As String,
                                          savefile As String,
                                          fileName_tag As Boolean,
                                          echo As Action(Of String))

        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".input_files", sessionID:=App.PID.ToHexString, prefix:="merge_slides_")
        Dim layoutfile As String = TempFileSystem.GetAppSysTempFile(".input_files", sessionID:=App.PID.ToHexString, prefix:="slide_layout_")
        Dim cli As String = PipelineTask.Task.GetJoinSlidesCommandLine(tempfile, layoutfile, savefile, fileName_tag)

        Call msData.SaveTo(tempfile)
        Call layoutData.SaveTo(layoutfile)

        Dim pipeline As New RunSlavePipeline(PipelineTask.Host, cli)

        Call WorkStudio.LogCommandLine(PipelineTask.Host, cli, App.CurrentDirectory)
        Call Workbench.LogText(pipeline.CommandLine)

        AddHandler pipeline.SetMessage, Sub(msg) echo(msg)

        Call pipeline.Run()
    End Sub

    Public Shared Sub ImportsSCiLSLab(msData As (sportIndex$, msData$)(), savefile As String, loadCallback As Action(Of String))
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".input_files", sessionID:=App.PID.ToHexString, prefix:="SCiLSLab_Imports_")
        Dim cli As String = PipelineTask.Task.GetImportsSCiLSLabCommandLine(tempfile, savefile)
        Dim pipeline As New RunSlavePipeline(PipelineTask.Host, cli)

        Call msData _
            .Select(Function(t)
                        Return {t.sportIndex, t.msData}.JoinBy(vbTab)
                    End Function) _
            .SaveTo(tempfile, encoding:=Encodings.UTF8.CodePage)

        Call WorkStudio.LogCommandLine(PipelineTask.Host, cli, App.CurrentDirectory)
        Call Workbench.LogText(pipeline.CommandLine)
        Call TaskProgress.RunAction(
            run:=Sub(p)
                     p.SetProgressMode()

                     AddHandler pipeline.SetMessage, AddressOf p.SetInfo
                     AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                     AddHandler pipeline.Finish, AddressOf p.TaskFinish

                     Call pipeline.Run()
                 End Sub,
            title:="Imports MSI Matrix...",
            info:="Imports SCiLS Lab MSImaging matrix data into viewer workspace...")

        If MessageBox.Show("MSI Raw Convert Job Done!" & vbCrLf & "Open MSI raw data file in MSI Viewer?",
                           "MSI Viewer",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Information) = DialogResult.Yes Then

            Call loadCallback(savefile)
        End If
    End Sub

    Public Shared Sub CreateMSIRawFromRowBinds(files As String(), savefile As String,
                                               cutoff As Double,
                                               basePeak As Double,
                                               resoltuion As Double,
                                               loadCallback As Action(Of String))

        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".input_files", sessionID:=App.PID.ToHexString, prefix:="CombineRowScans_")
        Dim commandline As String = PipelineTask.Task.GetMSIRowCombineCommandLine(
            files:=tempfile,
            save:=savefile,
            cutoff:=cutoff,
            matrix_basepeak:=basePeak,
            resolution:=resoltuion
        )
        Dim pipeline As New RunSlavePipeline(PipelineTask.Host, commandline)

        Call files.SaveTo(tempfile, encoding:=Encodings.UTF8.CodePage)
        Call WorkStudio.LogCommandLine(PipelineTask.Host, commandline, App.CurrentDirectory)
        Call Workbench.LogText(pipeline.CommandLine)
        Call TaskProgress.RunAction(
            run:=Sub(p)
                     p.SetProgressMode()

                     AddHandler pipeline.SetMessage, AddressOf p.SetInfo
                     AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                     AddHandler pipeline.Finish, AddressOf p.TaskFinish

                     Call pipeline.Run()

                 End Sub,
            title:="Convert MSI Raw...",
            info:="Loading MSI raw data file into viewer workspace...")

        If MessageBox.Show("MSI Raw Convert Job Done!" & vbCrLf & "Open MSI raw data file in MSI Viewer?",
                           "MSI Viewer",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Information) = DialogResult.Yes Then

            Call loadCallback(savefile)
        End If
    End Sub
End Class
