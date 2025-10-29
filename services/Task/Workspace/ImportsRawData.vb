﻿#Region "Microsoft.VisualBasic::9f97a5ce6224a263f4df28570df577c7, mzkit\src\mzkit\Task\Imports\ImportsRawData.vb"

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

'   Total Lines: 40
'    Code Lines: 31
' Comment Lines: 0
'   Blank Lines: 9
'     File Size: 1.43 KB


' Class ImportsRawData
' 
'     Properties: raw
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: GetCachePath
' 
'     Sub: RunImports
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Galaxy.Workbench
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Language.UnixBash
Imports TaskStream

Public Class ImportsRawData

    ReadOnly source As String
    ReadOnly cache As String
    ReadOnly showProgress As Action(Of String)
    ReadOnly setProgress As Action(Of Integer)
    ReadOnly success As Action
    ReadOnly snapshot As Boolean

    Public ReadOnly Property raw As MZWork.Raw

    Public Property arguments As Dictionary(Of String, String)
    Public Property protocol As FileApplicationClass = FileApplicationClass.LCMS

    Sub New(file As String, progress As Action(Of String), finished As Action,
            Optional cachePath As String = Nothing,
            Optional create_snapshot As Boolean = True,
            Optional writeProgress As Action(Of Integer) = Nothing)

        source = file
        cache = If(cachePath, GetCachePath(file))
        showProgress = progress
        setProgress = writeProgress
        success = finished
        raw = New MZWork.Raw With {
            .cache = cache.GetFullPath,
            .source = source.GetFullPath.Replace("\", "/")
        }
        snapshot = create_snapshot
    End Sub

    Public Shared Function GetCachePath(file As String) As String
        ' 20240823 due to the reason of LCMS deconvolution
        ' needs the file name as the sample id
        ' so cache file name no encoded in md5 at here anymore.
        ' use the original file name
        Dim cacheKey As String = file.GetFullPath.MD5
        Dim path As String = App.AppSystemTemp & $"/.cache/{cacheKey.Substring(0, 3)}/" & file.BaseName & ".mzPack"

        Return path
    End Function

    Public Shared Function EnumerateRawtDataFiles(sourceFolder As String) As IEnumerable(Of String)
        ' Return ls - l - r - {"*.raw", "*.wiff", "*.mzpack", "*.mzml", "*.mzxml"} <= sourceFolder
        Return ls - l - r - "*.raw" <= sourceFolder
    End Function

    Private Function getCliArguments() As String
        If arguments Is Nothing Then
            arguments = New Dictionary(Of String, String)
        End If

        Select Case protocol
            Case FileApplicationClass.LCMS, FileApplicationClass.GCMS, FileApplicationClass.GCxGC
                If raw.source.ExtensionSuffix("cdf", "netcdf") Then
                    Return PipelineTask.Task.GetconvertGCMSCDFCommandLine(raw.source, raw.cache, no_thumbnail:=Not snapshot)
                Else
                    Return PipelineTask.Task.GetconvertAnyRawCommandLine(raw.source, raw.cache, no_thumbnail:=Not snapshot)
                End If
            Case FileApplicationClass.MSImaging
                Dim rawfiles As String() = EnumerateRawtDataFiles(raw.source).ToArray
                Dim tempfile As String = TempFileSystem.GetAppSysTempFile("/", sessionID:=App.PID.ToHexString, prefix:="ms-imaging_raw") & $"/{raw.source.BaseName}.txt"
                Dim cutoff As String = arguments.TryGetValue("cutoff", [default]:="0")
                Dim matrix_basepeak As String = arguments.TryGetValue("matrix_basepeak", [default]:="0")
                Dim resolution As String = arguments.TryGetValue("resolution", [default]:=17)
                Dim norm As Boolean = arguments.TryGetValue("norm", [default]:=True).ParseBoolean

                Call rawfiles.SaveTo(tempfile)

                If raw.cache.ExtensionSuffix("imzml") Then
                    ' do row combines and then convert to imzml
                    Return PipelineTask.Task.GetMSIToimzMLCommandLine(tempfile, raw.cache, cutoff, matrix_basepeak, resolution, tic_norm:=norm)
                Else
                    Return PipelineTask.Task.GetMSIRowCombineCommandLine(tempfile, raw.cache, scan:="raw", cutoff, matrix_basepeak, resolution, tic_norm:=norm)
                End If
            Case Else
                Throw New NotImplementedException(protocol.Description)
        End Select
    End Function

    ''' <summary>
    ''' msconvert
    ''' </summary>
    Public Sub RunImports()
        Dim cli As String = getCliArguments()
        Dim pipeline As New RunSlavePipeline(PipelineTask.Host, cli)

        AddHandler pipeline.Finish, AddressOf success.Invoke
        AddHandler pipeline.SetMessage, AddressOf showProgress.Invoke

        If setProgress IsNot Nothing Then
            AddHandler pipeline.SetProgress, Sub(p, txt) setProgress(p)
        End If

        Call WorkStudio.LogCommandLine(PipelineTask.Host, cli, App.CurrentDirectory)

        Call pipeline.Run()
    End Sub
End Class
