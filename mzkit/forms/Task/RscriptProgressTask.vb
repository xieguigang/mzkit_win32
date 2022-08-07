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

Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.BrukerDataReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.BrukerDataReader.SCiLSLab
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports PipelineHost
Imports SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles
Imports Task

Public Class RscriptProgressTask

    ''' <summary>
    ''' convert imzML to mzpack
    ''' </summary>
    ''' <param name="imzML"></param>
    ''' <returns></returns>
    Public Shared Function CreateMSIIndex(imzML As String) As String
        Dim Rscript As String = RscriptPipelineTask.GetRScript("buildMSIIndex.R")
        Dim ibd As ibdReader = ibdReader.Open(imzML.ChangeSuffix("ibd"))
        Dim uid As String = ibd.UUID
        Dim cachefile As String = App.AppSystemTemp & "/MSI_imzML/" & uid
        Dim cli As String = $"""{Rscript}"" --imzML ""{imzML}"" --cache ""{cachefile}"" --SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        If cachefile.FileLength > 1024 Then
            Return cachefile
        End If

        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("Open imzML...", directAccess:=True)
        progress.ShowProgressDetails("Loading MSI raw data file into viewer workspace...", directAccess:=True)
        progress.SetProgressMode()

        Call MyApplication.LogText(pipeline.CommandLine)

        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress
        AddHandler pipeline.Finish, Sub() progress.Invoke(Sub() progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        Return cachefile
    End Function

    Public Shared Sub ImportsSCiLSLab(msData As String(), savefile As String)
        Dim tuples = SCiLSLab.CheckSpotFiles(msData).ToArray

        If tuples.IsNullOrEmpty Then
            ' missing spot index file
            MessageBox.Show("invalid selected table data files!", "Imports SCiLS Lab", buttons:=MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".input_files", sessionID:=App.PID.ToHexString, prefix:="SCiLSLab_Imports_")
        Dim cli As String = PipelineTask.Task.GetImportsSCiLSLabCommandLine(tempfile, savefile)
        Dim pipeline As New RunSlavePipeline(PipelineTask.Host, cli)

        Call tuples _
            .Select(Function(t)
                        Return {t.sportIndex, t.msData}.JoinBy(vbTab)
                    End Function) _
            .SaveTo(tempfile, encoding:=Encodings.UTF8.CodePage)

        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("Imports MSI Matrix...", directAccess:=True)
        progress.ShowProgressDetails("Imports SCiLS Lab MSImaging matrix data into viewer workspace...", directAccess:=True)
        progress.SetProgressMode()

        Call MyApplication.LogText(pipeline.CommandLine)

        AddHandler pipeline.SetMessage, AddressOf progress.ShowProgressDetails
        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress
        AddHandler pipeline.Finish, Sub() progress.Invoke(Sub() progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        If MessageBox.Show("MSI Raw Convert Job Done!" & vbCrLf & "Open MSI raw data file in MSI Viewer?", "MSI Viewer", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
            Call RibbonEvents.showMsImaging()
            Call WindowModules.viewer.loadimzML(savefile)
        End If
    End Sub

    Public Shared Sub CreateMSIRawFromRowBinds(files As String(), savefile As String)
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".input_files", sessionID:=App.PID.ToHexString, prefix:="CombineRowScans_")
        Dim cli As String = PipelineTask.Task.GetMSIRowCombineCommandLine(tempfile, savefile)
        Dim pipeline As New RunSlavePipeline(PipelineTask.Host, cli)

        Call files.SaveTo(tempfile, encoding:=Encodings.UTF8.CodePage)

        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("Convert MSI Raw...", directAccess:=True)
        progress.ShowProgressDetails("Loading MSI raw data file into viewer workspace...", directAccess:=True)
        progress.SetProgressMode()

        Call MyApplication.LogText(pipeline.CommandLine)

        AddHandler pipeline.SetMessage, AddressOf progress.ShowProgressDetails
        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress
        AddHandler pipeline.Finish, Sub() progress.Invoke(Sub() progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        If MessageBox.Show("MSI Raw Convert Job Done!" & vbCrLf & "Open MSI raw data file in MSI Viewer?", "MSI Viewer", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
            Call RibbonEvents.showMsImaging()
            Call WindowModules.viewer.loadimzML(savefile)
        End If
    End Sub

    Public Shared Sub ExportRGBIonsPlot(mz As Double(), tolerance As String, saveAs As String)
        Dim Rscript As String = RscriptPipelineTask.GetRScript("MSImaging/tripleIon.R")
        Dim cli As String = $"""{Rscript}"" --app {WindowModules.viewer.MSIservice.appPort} --mzlist ""{mz.JoinBy(",")}"" --save ""{saveAs}"" --mzdiff ""{tolerance}"" --SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)
        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("RGB Ions MS-Imaging", directAccess:=True)
        progress.ShowProgressDetails("Do plot of target ion m/z set...", directAccess:=True)
        progress.SetProgressMode()

        Call MyApplication.LogText(pipeline.CommandLine)

        AddHandler pipeline.SetMessage, AddressOf progress.ShowProgressDetails
        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress
        AddHandler pipeline.Finish, Sub() progress.Invoke(Sub() progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        If saveAs.FileExists Then
            If MessageBox.Show("RGB Ions MS-Imaging Job Done!" & vbCrLf & "Open MSImaging result plot file?", "Open Image", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                Call Process.Start(saveAs.GetFullPath)
            End If
        Else
            Call MessageBox.Show("RGB Ions MS-Imaging Task Error!", "Task Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Public Shared Sub ExportSingleIonPlot(mz As Double, tolerance As String, saveAs As String, Optional title As String = "")
        Dim Rscript As String = RscriptPipelineTask.GetRScript("MSImaging/singleIon.R")
        Dim cli As String = $"""{Rscript}"" --app {WindowModules.viewer.MSIservice.appPort} --mzlist ""{mz}"" --save ""{saveAs}"" --mzdiff ""{tolerance}"" --title ""{title}"" --SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)
        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("Single Ion MSImaging", directAccess:=True)
        progress.ShowProgressDetails("Do plot of target ion m/z...", directAccess:=True)
        progress.SetProgressMode()

        Call MyApplication.LogText(pipeline.CommandLine)

        AddHandler pipeline.SetMessage, AddressOf progress.ShowProgressDetails
        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress
        AddHandler pipeline.Finish, Sub() progress.Invoke(Sub() progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        If saveAs.FileExists Then
            If MessageBox.Show("Single Ion MSImaging Job Done!" & vbCrLf & "Open MSImaging result plot file?", "Open Image", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                Call Process.Start(saveAs.GetFullPath)
            End If
        Else
            Call MessageBox.Show("Single Ion MSImaging Task Error!", "Task Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Public Shared Sub ExportHeatMapMatrixPlot(mzSet As Dictionary(Of String, Dictionary(Of String, String)),
                                              tolerance As String,
                                              saveAs As String,
                                              debug As Action(Of String))

        Dim Rscript As String = RscriptPipelineTask.GetRScript("MSImaging/HeatMapMatrix.R")
        Dim mzfile As String = TempFileSystem.GetAppSysTempFile(".json", sessionID:=App.PID.ToHexString, prefix:="matrix_mzset___")
        Dim cli As String = $"""{Rscript}"" --app {WindowModules.viewer.MSIservice.appPort} --mzlist ""{mzfile}"" --save ""{saveAs}"" --mzdiff ""{tolerance}"" --SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)
        Dim progress As New frmTaskProgress

        Call mzSet.GetJson.SaveTo(mzfile)

        progress.ShowProgressTitle("Single Ion MSImaging", directAccess:=True)
        progress.ShowProgressDetails("Do plot of target ion m/z...", directAccess:=True)
        progress.SetProgressMode()

        Call MyApplication.LogText(pipeline.CommandLine)
        Call debug(pipeline.CommandLine)

        AddHandler pipeline.SetMessage, AddressOf progress.ShowProgressDetails
        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress
        AddHandler pipeline.Finish, Sub() progress.Invoke(Sub() progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        If saveAs.FileExists Then
            If MessageBox.Show("MSImaging matrix heatmap rendering job done!" & vbCrLf & "Open MSImaging result plot file?", "Open Image", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
                Call Process.Start(saveAs.GetFullPath)
            End If
        Else
            Call MessageBox.Show("MSImaging matrix heatmap rendering task error!", "Task Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Public Shared Sub CreateMSIPeakTable(regions As MSIRegionSampleWindow, mzpack As String, saveAs As String)
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".cdf", App.PID.ToHexString, prefix:="MSI_regions__")
        Dim Rscript As String = RscriptPipelineTask.GetRScript("MSI_peaktable.R")
        Dim cli As String = $"""{Rscript}"" --raw ""{mzpack}"" --save ""{saveAs}"" --regions ""{tempfile}"" --SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        Using buffer As Stream = tempfile.Open(FileMode.OpenOrCreate)
            Call regions.ExportTissueMaps(regions.dimension, buffer)
        End Using

        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("Create MSI sampletable...", directAccess:=True)
        progress.ShowProgressDetails("Loading MSI raw data file into viewer workspace...", directAccess:=True)
        progress.SetProgressMode()

        Call MyApplication.LogText(pipeline.CommandLine)

        AddHandler pipeline.SetMessage, AddressOf progress.ShowProgressDetails
        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress
        AddHandler pipeline.Finish, Sub() progress.Invoke(Sub() progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        If MessageBox.Show("Export MSI sampletable Job Done!" & vbCrLf & "Open MSI sample table data file?", "Open Excel", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.Yes Then
            Call Process.Start(saveAs.GetFullPath)
        End If
    End Sub

    Public Shared Function PlotStats(data As String, type As String, title As String) As Image
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".json", App.PID.ToHexString, prefix:="MSI_regions__")
        Dim imageOut As String = $"{tempfile.ParentPath}/Rplot.png"
        Dim Rscript As String = RscriptPipelineTask.GetRScript("ggplot/ggplot2.R")
        Dim cli As String = $"""{Rscript}"" --data ""{tempfile}"" --save ""{imageOut}"" --title ""{title}"" --plot ""{type}"" --SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)
        Dim progress As New frmTaskProgress

        progress.ShowProgressTitle("Create MSI sampletable...", directAccess:=True)
        progress.ShowProgressDetails("Loading MSI raw data file into viewer workspace...", directAccess:=True)
        progress.SetProgressMode()

        Call data.SaveTo(tempfile)
        Call MyApplication.LogText(pipeline.CommandLine)
        Call MyApplication.LogText(data)

        AddHandler pipeline.SetMessage, AddressOf progress.ShowProgressDetails
        AddHandler pipeline.SetProgress, AddressOf progress.SetProgress
        AddHandler pipeline.Finish, Sub() progress.Invoke(Sub() progress.Close())

        Call New Thread(AddressOf pipeline.Run).Start()
        Call progress.ShowDialog()

        Return imageOut.LoadImage
    End Function
End Class
