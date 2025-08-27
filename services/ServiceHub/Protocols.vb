﻿#Region "Microsoft.VisualBasic::be886f1e3952fe018f0344bd2b77a29b, mzkit\services\ServiceHub\Protocols.vb"

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

    '   Total Lines: 99
    '    Code Lines: 68 (68.69%)
    ' Comment Lines: 12 (12.12%)
    '    - Xml Docs: 91.67%
    ' 
    '   Blank Lines: 19 (19.19%)
    '     File Size: 3.43 KB


    ' Module Protocols
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: getArgumentString, StartServer
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Linq

Public Module Protocols

    ''' <summary>
    ''' the exe app path of the Rscript host
    ''' </summary>
    ReadOnly Rscript As String
    ReadOnly hostDll As String

    Sub New()
        Rscript = $"{App.HOME}/Rstudio/bin/Rscript.exe".GetFullPath
        hostDll = $"{App.HOME}/Rstudio/host/ServiceHub.dll".GetFullPath
    End Sub

    Private Function getArgumentString(Rscript As String, debugPort As Integer?, heartbeats As Integer?, buf_size As Single) As String
        Dim args As New List(Of String)

        Call args.Add(Rscript.CLIPath)
        Call args.Add($"--master {App.PID}")
        Call args.Add($"--SetDllDirectory {hostDll.ParentPath.CLIPath}")
        Call args.Add($"/@set buffer_size={buf_size}MB")

        If Not debugPort Is Nothing AndAlso debugPort > 0 Then
            Call args.Add($"--debug={debugPort}")
        End If

        If Not heartbeats Is Nothing AndAlso heartbeats > 0 Then
            ' Call args.Add($"--heartbeats={heartbeats}")
        End If

        Return args.JoinBy(" ")
    End Function

    ''' <summary>
    ''' folk a new task process in a new thread
    ''' </summary>
    ''' <param name="Rscript"></param>
    ''' <param name="service"></param>
    ''' <param name="debugPort"></param>
    ''' <param name="heartbeats"></param>
    ''' <returns></returns>
    Public Function StartServer(Rscript As String,
                                ByRef service As Integer,
                                debugPort As Integer?,
                                buf_size As Single,
                                Optional heartbeats As Integer? = Nothing) As RunSlavePipeline

        Dim cli As String = getArgumentString(Rscript, debugPort, heartbeats, buf_size)
        Dim workdir As String = Protocols.Rscript.ParentPath

        If debugPort Is Nothing Then
            Dim pipeline As New RunSlavePipeline(Protocols.Rscript, cli, workdir:=workdir)
            Dim tcpPort As Integer = -1

            Call pipeline.CommandLine.debug

            AddHandler pipeline.SetMessage,
                Sub(msg)
                    If msg.StartsWith("socket=") Then
                        tcpPort = msg.Match("\d+").DoCall(AddressOf Integer.Parse)
                    Else
                        Call msg.debug
                    End If
                End Sub

            Call pipeline.Start()

            For i As Integer = 0 To 1000
                service = tcpPort

                If service > 0 Then
                    Exit For
                Else
                    Thread.Sleep(500)
                End If
            Next

            Return pipeline
        Else
            Dim cmdl As New Process With {.StartInfo = New ProcessStartInfo With {
                .Arguments = cli,
                .CreateNoWindow = False,
                .FileName = Protocols.Rscript,
                .UseShellExecute = True,
                .WorkingDirectory = workdir,
                .RedirectStandardOutput = False
            }}

            service = debugPort
            cmdl.Start()

            Return RunSlavePipeline.Bind(cmdl)
        End If
    End Function
End Module
