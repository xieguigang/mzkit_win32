#Region "Microsoft.VisualBasic::35d2da831b9106f43d3cccfb122f2eed, mzkit\services\MZWorkRedis\App.vb"

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

    '   Total Lines: 75
    '    Code Lines: 57 (76.00%)
    ' Comment Lines: 8 (10.67%)
    '    - Xml Docs: 87.50%
    ' 
    '   Blank Lines: 10 (13.33%)
    '     File Size: 2.54 KB


    ' Module App
    ' 
    '     Function: GetRedisMemoryMapping, getScan1, getScan2, LoadMzPack, Start
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Parallel

<Package("RedisApp")>
Public Module App

    <ExportAPI("run")>
    Public Function Start(port As Integer, masterPid As Integer) As Boolean
        Return Service.Start(port, masterPid).Run
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="path"></param>
    ''' <param name="app"></param>
    ''' <returns>
    ''' all the redis key of the scan ms1 object
    ''' </returns>
    <ExportAPI("load.mzpack")>
    Public Function LoadMzPack(path As String, app As Integer) As Object
        Dim resq As New RequestStream(Service.Protocol, Protocols.LoadMzPack, path)
        Dim data = New TcpRequest(app).SendMessage(resq)

        If data.IsHTTP_RFC Then
            Return data.GetUTF8String
        Else
            Return data.GetUTF8String.LoadJSON(Of String())
        End If
    End Function

    <ExportAPI("scan1")>
    Public Function getScan1(key As String, app As Integer) As Object
        Dim hMemPointer = GetRedisMemoryMapping(key, app)

        If hMemPointer.Invalid Then
            Return hMemPointer.GetMappingFileName
        Else
            Return hMemPointer.GetObject(GetType(RedisScan1))
        End If
    End Function

    Public Function GetRedisMemoryMapping(key As String, app As Integer) As MapObject
        Dim res As New RequestStream(Service.Protocol, Protocols.GetValue, key)
        Dim data = New TcpRequest(app).SendMessage(res)

        If data.IsHTTP_RFC Then
            Return New UnmanageMemoryRegion With {
                .memoryFile = data.GetUTF8String,
                .size = -1
            }
        Else
            Return data.GetUTF8String _
                .LoadJSON(Of UnmanageMemoryRegion) _
                .GetMemoryMap
        End If
    End Function

    <ExportAPI("scan2")>
    Public Function getScan2(key As String, app As Integer) As Object
        Dim hMemPointer = GetRedisMemoryMapping(key, app)

        If hMemPointer.Invalid Then
            Return hMemPointer.GetMappingFileName
        Else
            Return hMemPointer.GetObject(GetType(ScanMS2))
        End If
    End Function
End Module
