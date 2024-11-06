#Region "Microsoft.VisualBasic::8e1e1e12bf520574084da1793473c687, mzkit\mzblender\MSIRender\BlenderClient.vb"

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

    '   Total Lines: 138
    '    Code Lines: 97 (70.29%)
    ' Comment Lines: 19 (13.77%)
    '    - Xml Docs: 42.11%
    ' 
    '   Blank Lines: 22 (15.94%)
    '     File Size: 5.55 KB


    ' Class BlenderClient
    ' 
    '     Properties: channel, Session
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetTrIQIntensity, handleRequest, MSIRender, OpenSession, SetFilters
    '               SetHEMap, SetIntensityRange, SetSampleTag
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing.Imaging
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Darwinism.HPC.Parallel
Imports Darwinism.IPC.Networking.HTTP
Imports Darwinism.IPC.Networking.Tcp
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Task

Public Class BlenderClient : Implements IDisposable

    ReadOnly host As IPEndPoint

    ''' <summary>
    ''' the MSI render type, one of the value of:
    ''' 
    ''' + <see cref="HeatMapBlender"/>
    ''' + <see cref="RGBIonMSIBlender"/>
    ''' + <see cref="SingleIonMSIBlender"/>
    ''' + <see cref="SummaryMSIBlender"/>
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Session As Type
    Public ReadOnly Property channel As MemoryPipe

    Dim sample_tag As String
    Dim timeout_sec As Double = 6

    Private disposedValue As Boolean

    Sub New(host As IPEndPoint, Optional debug As Boolean = False)
        Dim map_name As String = If(debug, "debug-blender", Service.GetMappedChannel(App.PID))

        Me.host = host
        Me.channel = New MemoryPipe(MapObject.FromPointer(map_name, 128 * ByteSize.MB))
    End Sub

    Private Function handleRequest(req As RequestStream) As RequestStream
        Return New TcpRequest(host).SetTimeOut(TimeSpan.FromSeconds(timeout_sec)).SendMessage(req)
    End Function

    Public Function MSIRender(args As PlotProperty, params As MsImageProperty, canvas As Size) As Image
        Dim payload As New Dictionary(Of String, String)
        payload.Add("sample", sample_tag)
        payload.Add("canvas", canvas.GetJson)
        payload.Add("params", params.GetJSON)
        payload.Add("args", args.GetJSON)
        Dim req As New RequestStream(Service.protocolHandle, Protocol.MSIRender, payload.GetJson)
        Dim resp = handleRequest(req)

        If NetResponse.IsHTTP_RFC(resp) Then
            Dim err As String = resp.GetUTF8String
            Throw New Exception(err)
        Else
            Return channel.LoadImage.CTypeGdiImage
        End If
    End Function

    Public Function SetSampleTag(tag As String)
        sample_tag = tag
        Return tag
    End Function

    Public Function SetHEMap(img As Image)
        If img Is Nothing Then
            Return Nothing
        End If

        Using ms As New MemoryStream
            Call img.Save(ms, ImageFormat.Png)
            Call ms.Flush()
            Call channel.WriteBuffer(ms.ToArray)
        End Using

        Return handleRequest(New RequestStream(Service.protocolHandle, Protocol.SetHEmap, "ok!"))
    End Function

    Public Function SetFilters(filters As RasterPipeline)
        Dim shaders As String() = filters.Select(Function(f) f.ToScript).ToArray
        Return handleRequest(New RequestStream(Service.protocolHandle, Protocol.SetFilters, shaders.GetJson))
    End Function

    Public Function SetIntensityRange(range As DoubleRange)
        Return handleRequest(New RequestStream(Service.protocolHandle, Protocol.SetIntensityRange, {range.Min, range.Max}.GetJson))
    End Function

    Public Function OpenSession(ss As Type, dims As Size, args As PlotProperty, params As MsImageProperty, configs As String)
        Dim args_str As String = If(args Is Nothing, "null", args.GetJSON)
        Dim params_str As String = If(params Is Nothing, "null", params.GetJSON)
        Dim payload As New Dictionary(Of String, String) From {
            {"ss", ss.Name},
            {"args", args_str},
            {"dims", $"{dims.Width},{dims.Height}"},
            {"params", params_str},
            {"configs", configs}
        }
        Dim result = handleRequest(New RequestStream(Service.protocolHandle, Protocol.OpenSession, payload.GetJson))
        Return result
    End Function

    Public Function GetTrIQIntensity(trIQ As Double) As Double
        Dim req As New RequestStream(Service.protocolHandle, Protocol.GetTrIQIntensity, NetworkByteOrderBitConvertor.GetBytes(trIQ))
        Dim resp As RequestStream = handleRequest(req)
        Dim dbls = NetworkByteOrderBitConvertor.ToDouble(resp.ChunkBuffer)
        Return dbls
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call handleRequest(New RequestStream(Service.protocolHandle, Protocol.Shutdown, "Close"))
            End If

            ' TODO: 释放未托管的资源(未托管的对象)并重写终结器
            ' TODO: 将大型字段设置为 null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: 仅当“Dispose(disposing As Boolean)”拥有用于释放未托管资源的代码时才替代终结器
    ' Protected Overrides Sub Finalize()
    '     ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' 不要更改此代码。请将清理代码放入“Dispose(disposing As Boolean)”方法中
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
