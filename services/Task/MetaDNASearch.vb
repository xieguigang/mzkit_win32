#Region "Microsoft.VisualBasic::18b47fdf779d8bc0d43b8019a3e52f5d, mzkit\src\mzkit\Task\MetaDNASearch.vb"

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

'   Total Lines: 29
'    Code Lines: 24
' Comment Lines: 0
'   Blank Lines: 5
'     File Size: 1.35 KB


' Module MetaDNASearch
' 
'     Sub: RunDIA
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioDeep
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports BioNovoGene.BioDeep.MSEngine.Mummichog
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.Serialization.JSON
Imports TaskStream

Public Module MetaDNASearch

    <Extension>
    Public Sub RunDIA(raw As Raw, println As Action(Of String), ByRef output As MetaDNAResult(), ByRef infer As CandidateInfer())
        Dim cacheRaw As String = raw.cache
        Dim ssid As String = SingletonHolder(Of BioDeepSession).Instance.ssid
        Dim outputdir As String = TempFileSystem.GetAppSysTempFile("__save", App.PID.ToHexString, "metadna_")
        Dim cli As String = $"""{RscriptPipelineTask.GetRScript("metadna.R")}"" --biodeep_ssid ""{ssid}"" --raw ""{cacheRaw}"" --save ""{outputdir}"" --SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        AddHandler pipeline.SetMessage, AddressOf println.Invoke

        Call cli.__DEBUG_ECHO
        Call pipeline.Run()

        output = $"{outputdir}/metaDNA_annotation.csv".LoadCsv(Of MetaDNAResult)
        infer = $"{outputdir}/infer_network.json".LoadJsonFile(Of CandidateInfer())
    End Sub

    Public Sub RunMummichogDIA(raw As Raw, println As Action(Of String), ByRef output As ActivityEnrichment())
        Dim cacheRaw As String = raw.cache
        Dim ssid As String = SingletonHolder(Of BioDeepSession).Instance.ssid
        Dim outputdir As String = TempFileSystem.GetAppSysTempFile("__save", App.PID.ToHexString, "Mummichog_")
        Dim cli As String = $"""{RscriptPipelineTask.GetRScript("Mummichog.R")}"" --biodeep_ssid ""{ssid}"" --raw ""{cacheRaw}"" --save ""{outputdir}"" --SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        AddHandler pipeline.SetMessage, AddressOf println.Invoke

        Call cli.__DEBUG_ECHO
        Call pipeline.Run()

        output = $"{outputdir}/Mummichog.json".LoadJsonFile(Of ActivityEnrichment())
    End Sub

    Public Sub RunMummichogDIA(mz As Double(), println As Action(Of String), ByRef output As ActivityEnrichment())
        Dim cacheRaw As String = TempFileSystem.GetAppSysTempFile(".txt", App.PID, "mz_peaklist")
        Dim ssid As String = SingletonHolder(Of BioDeepSession).Instance.ssid
        Dim outputdir As String = TempFileSystem.GetAppSysTempFile("__save", App.PID.ToHexString, "Mummichog_")
        Dim cli As String = $"""{RscriptPipelineTask.GetRScript("Mummichog.R")}"" 
--biodeep_ssid ""{ssid}"" 
--raw ""{cacheRaw}"" 
--save ""{outputdir}"" 
--SetDllDirectory {TaskEngine.hostDll.ParentPath.CLIPath}
"
        Dim pipeline As New RunSlavePipeline(RscriptPipelineTask.Host, cli, workdir:=RscriptPipelineTask.Root)

        AddHandler pipeline.SetMessage, AddressOf println.Invoke

        Call mz.FlushAllLines(cacheRaw)
        Call println("Run mummichog DIA:")
        Call println(mz.GetJson)
        Call cli.__DEBUG_ECHO
        Call pipeline.Run()

        output = $"{outputdir}/Mummichog.json".LoadJsonFile(Of ActivityEnrichment())
    End Sub
End Module
