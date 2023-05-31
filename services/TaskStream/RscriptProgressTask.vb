#Region "Microsoft.VisualBasic::8f29783761ba25785e2e473b80281f16, mzkit\src\mzkit\mzkit\forms\Task\RscriptProgressTask.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 103
'    Code Lines: 73
' Comment Lines: 5
'   Blank Lines: 25
'     File Size: 5.05 KB


' Class RscriptProgressTask
' 
'     Function: CreateMSIIndex
' 
'     Sub: CreateMSIPeakTable, CreateMSIRawFromRowBinds
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Text
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Mzkit_win32.BasicMDIForm

Public NotInheritable Class RscriptProgressTask

    Private Sub New()
    End Sub

    Public Shared Function ConvertSTData(spot As String, matrix As String, tag As String, targets As String(), save As String) As String
        Dim Rscript As String = RscriptPipelineTask.GetRScript("10x_genomics/convert_h5ad_st_to_mzpack.R")
        Dim targetfile As String = TempFileSystem.GetAppSysTempFile(".txt")

        Call targets.SaveTo(targetfile)

        Dim cli As String = $"""{Rscript}"" 
--spots ""{spot}""
--expr ""{matrix}""
--tag ""{tag}""
--save ""{save}""
--targets ""{targetfile}""
--SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}
"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli, RscriptPipelineTask.Root)
        Call Workbench.LogText(pipeline.CommandLine)

        Call TaskProgress.RunAction(
                run:=Sub(p)
                         p.SetProgressMode()

                         AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                         AddHandler pipeline.Finish, AddressOf p.TaskFinish

                         Call pipeline.Run()
                     End Sub,
                title:="Import 10X Genomics h5ad matrix",
                info:="Do file data converts to mzpack data file...")

        Return save
    End Function

    Public Shared Function ExportLinearReport(linear As String, export As String, onHost As Boolean) As String
        Dim Rscript As String = RscriptPipelineTask.GetRScript("linearReport.R")
        Dim cli As String = $"""{Rscript}"" 
--linear ""{linear}"" 
--export ""{export}"" 
--SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}
"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli, RscriptPipelineTask.Root)
        Call Workbench.LogText(pipeline.CommandLine)

        If onHost Then
            AddHandler pipeline.SetMessage, AddressOf Workbench.StatusMessage
            Call pipeline.Run()
        Else
            Call TaskProgress.RunAction(
                run:=Sub(p)
                         p.SetProgressMode()

                         AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                         AddHandler pipeline.Finish, AddressOf p.TaskFinish

                         Call pipeline.Run()
                     End Sub,
                title:="Export Report",
                info:="Do exports of the html report for the linear regression and targetted quantification...")
        End If

        Return export
    End Function

    ''' <summary>
    ''' convert imzML to mzpack
    ''' </summary>
    ''' <param name="imzML"></param>
    ''' <returns></returns>
    Public Shared Function CreateMSIIndex(imzML As String, getGuid As Func(Of String, String)) As String
        Dim Rscript As String = RscriptPipelineTask.GetRScript("buildMSIIndex.R")
        Dim uid As String = getGuid(imzML.ChangeSuffix("ibd"))
        Dim cachefile As String = App.AppSystemTemp & "/MSI_imzML/" & uid
        Dim cli As String = $"""{Rscript}"" 
--imzML ""{imzML}"" 
--cache ""{cachefile}"" 
--SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}
"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        If cachefile.FileLength > 1024 Then
            Return cachefile
        Else
            Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli, RscriptPipelineTask.Root)
            Call Workbench.LogText(pipeline.CommandLine)
            Call TaskProgress.RunAction(
                run:=Sub(p)
                         p.SetProgressMode()

                         AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                         AddHandler pipeline.Finish, AddressOf p.TaskFinish

                         Call pipeline.Run()
                     End Sub,
                title:="Open imzML...",
                info:="Loading MSI raw data file into viewer workspace...")
        End If

        Return cachefile
    End Function

    Public Shared Sub ExportRGBIonsPlot(mz As Double(), tolerance As String, saveAs As String)
        Dim Rscript As String = RscriptPipelineTask.GetRScript("MSImaging/tripleIon.R")
        Dim cli As String = $"""{Rscript}"" 
--app {Workbench.MSIServiceAppPort} 
--mzlist ""{mz.JoinBy(",")}"" 
--save ""{saveAs}"" 
--mzdiff ""{tolerance}"" 
--SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}
"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli, RscriptPipelineTask.Root)
        Call Workbench.LogText(pipeline.CommandLine)
        Call TaskProgress.RunAction(
            run:=Sub(p)
                     AddHandler pipeline.SetMessage, AddressOf p.SetInfo
                     AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                     AddHandler pipeline.Finish, AddressOf p.TaskFinish

                     Call pipeline.Run()
                 End Sub,
            title:="RGB Ions MS-Imaging",
            info:="Do plot of target ion m/z set...")

        If saveAs.FileExists(ZERO_Nonexists:=True) Then
            If MessageBox.Show("RGB Ions MS-Imaging Job Done!" & vbCrLf & "Open MSImaging result plot file?",
                               "Open Image",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Information) = DialogResult.Yes Then

                Call Process.Start(saveAs.GetFullPath)
            End If
        Else
            Call MessageBox.Show("RGB Ions MS-Imaging Task Error!", "Task Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Public Shared Sub ExportSingleIonPlot(mz As Double,
                                          tolerance As String,
                                          saveAs As String,
                                          background As String,
                                          colorSet As String,
                                          overlapTotalIons As Boolean,
                                          Optional title As String = "")

        Dim Rscript As String = RscriptPipelineTask.GetRScript("MSImaging/singleIon.R")
        Dim overlapFlag As String = If(overlapTotalIons, "--overlap-tic", "")
        Dim cli As String = $"""{Rscript}"" 
--app {Workbench.MSIServiceAppPort} 
--mzlist ""{mz}"" 
--save ""{saveAs}"" 
--backcolor ""{background}"" 
--colors ""{colorSet}"" 
--mzdiff ""{tolerance}"" 
--title ""{title}"" {overlapFlag} 
--SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}
"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli, RscriptPipelineTask.Root)
        Call Workbench.LogText(pipeline.CommandLine)
        Call TaskProgress.RunAction(
            run:=Sub(p)
                     p.SetProgressMode()

                     AddHandler pipeline.SetMessage, AddressOf p.SetInfo
                     AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                     AddHandler pipeline.Finish, AddressOf p.TaskFinish

                     Call pipeline.Run()

                 End Sub,
            title:="Single Ion MSImaging",
            info:="Do plot of target ion m/z...")

        If saveAs.FileExists Then
            If MessageBox.Show("Single Ion MSImaging Job Done!" & vbCrLf & "Open MSImaging result plot file?",
                               "Open Image",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Information) = DialogResult.Yes Then

                Call Process.Start(saveAs.GetFullPath)
            End If
        Else
            Call MessageBox.Show("Single Ion MSImaging Task Error!", "Task Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="bitmap"></param>
    ''' <param name="channels"></param>
    ''' <returns>
    ''' result json text
    ''' </returns>
    Public Shared Function ScanBitmap(bitmap As Bitmap, channels As IEnumerable(Of Color)) As String
        Dim Rscript As String = RscriptPipelineTask.GetRScript("HEScan.R")
        Dim imagetmp As String = TempFileSystem.GetAppSysTempFile(".png")
        Dim jsontmp As String = TempFileSystem.GetAppSysTempFile(".heatmap")
        Dim cli As String = $"""{Rscript}"" 
--bitmap ""{imagetmp}"" 
--channels {channels.Select(Function(c) c.ToHtmlColor).JoinBy(";")} 
--save ""{jsontmp}""
"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        Call bitmap.SaveAs(imagetmp)
        Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli, RscriptPipelineTask.Root)
        Call Workbench.LogText(pipeline.CommandLine)

        Call TaskProgress.RunAction(
            run:=Sub(p)
                     p.SetProgressMode()

                     AddHandler pipeline.SetMessage, AddressOf p.SetInfo
                     AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                     AddHandler pipeline.Finish, AddressOf p.TaskFinish

                     Call pipeline.Run()

                 End Sub,
            title:="Run Heatmap Scanning...",
            info:="The image analysis may be takes a long time, please wait for a while...")

        If jsontmp.FileExists Then
            Try
                ' result json text
                Return jsontmp.ReadAllText
            Catch ex As Exception
                Return Nothing
            End Try
        Else
            Call MessageBox.Show("Heatmap scanning Task Error!", "Task Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End If
    End Function

    Public Shared Sub ExportHeatMapMatrixPlot(mzSet As Dictionary(Of String, Dictionary(Of String, String)),
                                              tolerance As String,
                                              saveAs As String,
                                              size As Size,
                                              layout As Size,
                                              scaler As String,
                                              debug As Action(Of String))

        Dim Rscript As String = RscriptPipelineTask.GetRScript("MSImaging/HeatMapMatrix.R")
        Dim mzfile As String = TempFileSystem.GetAppSysTempFile(".json", sessionID:=App.PID.ToHexString, prefix:="matrix_mzset___")
        Dim cli As String = $"""{Rscript}"" 
--app {Workbench.MSIServiceAppPort} 
--mzlist ""{mzfile}"" 
--size ""{New Integer() {size.Width, size.Height}.JoinBy(",")}""
--layout ""{ New Integer() {layout.Width, layout.Height}.JoinBy(",")}""
--scaler ""{scaler}""
--save ""{saveAs}"" 
--mzdiff ""{tolerance}"" 
--SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        Call mzSet.GetJson.SaveTo(mzfile)
        Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli, RscriptPipelineTask.Root)
        Call Workbench.LogText(pipeline.CommandLine)
        Call debug(pipeline.CommandLine)

        Call TaskProgress.RunAction(
            run:=Sub(p)
                     p.SetProgressMode()

                     AddHandler pipeline.SetMessage, AddressOf p.SetInfo
                     AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                     AddHandler pipeline.Finish, AddressOf p.TaskFinish

                     Call pipeline.Run()
                 End Sub,
            title:="Single Ion MSImaging",
            info:="Do plot of target ion m/z...")

        If saveAs.FileExists Then
            If MessageBox.Show("MSImaging matrix heatmap rendering job done!" & vbCrLf & "Open MSImaging result plot file?",
                               "Open Image",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Information) = DialogResult.Yes Then

                Call Process.Start(saveAs.GetFullPath)
            End If
        Else
            Call MessageBox.Show("MSImaging matrix heatmap rendering task error!", "Task Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    ''' <summary>
    ''' Create MSI peaktable without ptissue region maps
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="saveAs"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="intocutoff"></param>
    ''' <param name="TrIQ"></param>
    Public Shared Sub CreateMSIPeakTable(mzpack As String, saveAs As String, mzdiff As String, intocutoff As Double, TrIQ As Double)
        Dim Rscript As String = RscriptPipelineTask.GetRScript("MSI_peaktable.R")
        Dim cli As String = $"""{Rscript}"" 
--raw ""{mzpack}"" 
--save ""{saveAs}"" 
--mzdiff ""{mzdiff}""
--into.cutoff ""{intocutoff}""
--TrIQ ""{TrIQ}""
--SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}
"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli, RscriptPipelineTask.Root)
        Call Workbench.LogText(pipeline.CommandLine)
        Call TaskProgress.RunAction(
            run:=Sub(p)
                     AddHandler pipeline.SetMessage, AddressOf p.SetInfo
                     AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                     AddHandler pipeline.Finish, AddressOf p.TaskFinish

                     Call pipeline.Run()
                 End Sub,
            title:="Create MSI sampletable...",
            info:="Loading MSI raw data file into viewer workspace...")

        Call CommonPeaktableFilePrompt(saveAs)
    End Sub

    Private Shared Sub CommonPeaktableFilePrompt(saveAs As String)
        If Not saveAs.FileExists(ZERO_Nonexists:=True) Then
            Dim errMsg As String = "Sorry, the MS-Imaging feature peaktable export is not success."

            Call Workbench.Warning(errMsg)
            Call MessageBox.Show(errMsg, "Rscript Task Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

        ElseIf MessageBox.Show("Export MSI sampletable Job Done!" & vbCrLf & "Open MSI sample table data file?",
                               "Open Excel",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Information) = DialogResult.Yes Then

            Call Workbench.SuccessMessage($"Export MSI sampletable Job Done! [{saveAs}]")
            Call Process.Start(saveAs.GetFullPath)
        End If
    End Sub

    ''' <summary>
    ''' Create MSI peaktable with a specific ptissue region maps
    ''' </summary>
    ''' <param name="mzpack"></param>
    ''' <param name="saveAs"></param>
    ''' <param name="exportTissueMaps"></param>
    Public Shared Sub CreateMSIPeakTable(mzpack As String, saveAs As String, exportTissueMaps As Action(Of Stream))
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".cdf", App.PID.ToHexString, prefix:="MSI_regions__")
        Dim Rscript As String = RscriptPipelineTask.GetRScript("MSI_peaktable.R")
        Dim cli As String = $"""{Rscript}"" 
--raw ""{mzpack}"" 
--save ""{saveAs}"" 
--regions ""{tempfile}"" 
--SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}
"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        Using buffer As Stream = tempfile.Open(FileMode.OpenOrCreate)
            Call exportTissueMaps(buffer)
        End Using

        Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli, RscriptPipelineTask.Root)
        Call Workbench.LogText(pipeline.CommandLine)
        Call TaskProgress.RunAction(
            run:=Sub(p)
                     AddHandler pipeline.SetMessage, AddressOf p.SetInfo
                     AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                     AddHandler pipeline.Finish, AddressOf p.TaskFinish

                     Call pipeline.Run()
                 End Sub,
            title:="Create MSI sampletable...",
            info:="Loading MSI raw data file into viewer workspace...")

        Call CommonPeaktableFilePrompt(saveAs)
    End Sub

    Public Shared Function PlotSingleMSIStats(data As String, type As String, title As String,
                                              mz As Double,
                                              tolerance As String,
                                              background As String,
                                              colorSet As String) As Image

        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".json", App.PID.ToHexString, prefix:="MSI_regions__")
        Dim imageOut As String = $"{tempfile.ParentPath}/Rplot.png"
        Dim Rscript As String = RscriptPipelineTask.GetRScript("ggplot/ggplot_ionStatMSI.R")
        Dim cli As String = $"""{Rscript}"" 
--app {Workbench.MSIServiceAppPort} 
--mzlist ""{mz}"" 
--backcolor ""{background}""
--colors ""{colorSet}"" 
--mzdiff ""{tolerance}"" 
--data ""{tempfile}"" 
--save ""{imageOut}"" 
--title ""{title}"" 
--plot ""{type}"" 
--SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}
"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        Call data.SaveTo(tempfile)
        Call Workbench.LogText(pipeline.CommandLine)
        Call Workbench.LogText(data)
        Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli, RscriptPipelineTask.Root)

        Call TaskProgress.RunAction(
            run:=Sub(p)
                     AddHandler pipeline.SetMessage, AddressOf p.SetInfo
                     AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                     AddHandler pipeline.Finish, AddressOf p.TaskFinish

                     Call pipeline.Run()

                 End Sub,
            title:="Plot Single MSI Ion",
            info:=$"Plot({type}) for target ion: {title}, m/z={mz}"
        )

        If Not imageOut.FileExists(ZERO_Nonexists:=True) Then
            Return Nothing
        Else
            Return imageOut.LoadImage
        End If
    End Function

    Public Shared Function PlotStats(data As String, type As String, title As String) As Image
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".json", App.PID.ToHexString, prefix:="MSI_regions__")
        Dim imageOut As String = $"{tempfile.ParentPath}/Rplot.png"
        Dim Rscript As String = RscriptPipelineTask.GetRScript("ggplot/ggplot2.R")
        Dim cli As String = $"""{Rscript}"" 
--data ""{tempfile}"" 
--save ""{imageOut}"" 
--title ""{title}"" 
--plot ""{type}"" 
--SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}
"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli, RscriptPipelineTask.Root)
        Call data.SaveTo(tempfile)
        Call Workbench.LogText(pipeline.CommandLine)
        Call Workbench.LogText(data)
        Call TaskProgress.RunAction(
            run:=Sub(p)
                     p.SetProgressMode()

                     AddHandler pipeline.SetMessage, AddressOf p.SetInfo
                     AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                     AddHandler pipeline.Finish, AddressOf p.TaskFinish

                     Call pipeline.Run()

                 End Sub,
            title:="Create MSI sample table...",
            info:="Loading MSI raw data file into viewer workspace...")

        If Not imageOut.FileExists(ZERO_Nonexists:=True) Then
            Return Nothing
        Else
            Return imageOut.LoadImage
        End If
    End Function

    Public Shared Function PlotScatter3DStats(data As String, title As String) As Image
        Dim imageOut As String = $"{data.ParentPath}/Rplot.png"
        Dim Rscript As String = RscriptPipelineTask.GetRScript("ggplot/ggplot_scatter3D.R")
        Dim cli As String = $"""{Rscript}"" 
--matrix ""{data}"" 
--png ""{imageOut}"" 
--title ""{title}"" 
--SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}
"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli, RscriptPipelineTask.Root)
        Call Workbench.LogText(pipeline.CommandLine)
        Call Workbench.LogText(data)
        Call TaskProgress.RunAction(
            run:=Sub(p)
                     p.SetProgressMode()

                     AddHandler pipeline.SetMessage, AddressOf p.SetInfo
                     AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                     AddHandler pipeline.Finish, AddressOf p.TaskFinish

                     Call pipeline.Run()
                 End Sub,
            title:="Create scatter 3d plot...",
            info:="Run scater data plot and 3d rendering...")

        If Not imageOut.FileExists(ZERO_Nonexists:=True) Then
            Return Nothing
        Else
            Return imageOut.LoadImage
        End If
    End Function

    Public Shared Sub RunRScriptPipeline(name As String, args As Dictionary(Of String, String), title As String, desc As String)
        Dim Rscript As String = RscriptPipelineTask.GetRScript(name)
        Dim cli As New StringBuilder(Rscript)

        For Each arg In args
            Call cli.AppendLine($"{arg.Key} ""{arg.Value}""")
        Next

        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli.ToString, workdir:=RscriptPipelineTask.Root)

        Call WorkStudio.LogCommandLine(RscriptPipelineTask.Host, cli.ToString, RscriptPipelineTask.Root)
        Call Workbench.LogText(pipeline.CommandLine)

        Call TaskProgress.RunAction(
            run:=Sub(p)
                     p.SetProgressMode()

                     AddHandler pipeline.SetMessage, AddressOf p.SetInfo
                     AddHandler pipeline.SetProgress, AddressOf p.SetProgress
                     AddHandler pipeline.Finish, AddressOf p.TaskFinish

                     Call pipeline.Run()
                 End Sub,
            title:=title, info:=desc
        )
    End Sub
End Class
