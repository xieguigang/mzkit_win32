﻿#Region "Microsoft.VisualBasic::0224131679af9a5d60b7c7f3645e36d2, mzkit\src\mzkit\Task\Studio\WorkStudio.vb"

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

'   Total Lines: 6
'    Code Lines: 5
' Comment Lines: 0
'   Blank Lines: 1
'     File Size: 177.00 B


' Class WorkStudio
' 
'     Sub: RunTaskScript
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging

Public Class WorkStudio

    Shared ReadOnly logfile As String = $"{App.ProductProgramData}/pipeline_calls_{Now.ToString("yyyy-MM")}.txt"

    Private Shared Function CreateLogger() As LogFile
        Return New LogFile(path:=logfile, autoFlush:=True, append:=True, appendHeader:=False)
    End Function

    Public Shared Sub RunTaskScript(file As String, args As String)
        Call CommandLine.Call($"{App.HOME}/Rstudio/bin/R#.exe", args)
    End Sub

    Public Shared Sub LogCommandLine(cmd As Process)
        Call LogCommandLine(cmd.StartInfo.FileName, cmd.StartInfo.Arguments, cmd.StartInfo.WorkingDirectory)
    End Sub

    Public Shared Sub LogCommandLine(host As String, commandline As String, workdir As String)
        Dim logText As New StringBuilder

        Call logText.AppendLine($"host: {host}")
        Call logText.AppendLine($"arguments: {commandline}")
        Call logText.AppendLine($"workdir: {workdir}")
        Call logText.AppendLine()
        Call logText.AppendLine($"***** full workflow commandline *****")
        Call logText.AppendLine()
        Call logText.AppendLine($"cd {workdir.CLIPath}")
        Call logText.AppendLine($"{host.CLIPath} {commandline.TrimNewLine}")
        Call logText.AppendLine()

        Using log As LogFile = CreateLogger()
            Call log.log(MSG_TYPES.DEBUG, logText)
        End Using
    End Sub
End Class

