#Region "Microsoft.VisualBasic::d21149072fc8dada2618304c937117af, mzkit\mzblender\MSIRender\BlenderClient.vb"

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

    '   Total Lines: 175
    '    Code Lines: 116 (66.29%)
    ' Comment Lines: 30 (17.14%)
    '    - Xml Docs: 63.33%
    ' 
    '   Blank Lines: 29 (16.57%)
    '     File Size: 6.84 KB


    ' Class BlenderClient
    ' 
    '     Properties: channel, Session
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: GetTrIQIntensity, MSIRender, OpenSession, SetFilters, SetHEMap
    '               SetIntensityRange, SetSampleTag
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Darwinism.HPC.Parallel
Imports Darwinism.IPC.Networking.HTTP
Imports Darwinism.IPC.Networking.Protocols
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports MZKitWin32.Blender.CommonLibs

Public Class BlenderClient : Implements IDisposable

    ReadOnly host As IRequestClient
    ReadOnly log As Action(Of String)

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

    Private disposedValue As Boolean

    ''' <summary>
    ''' connect to remote services
    ''' </summary>
    ''' <param name="host"></param>
    ''' <param name="log"></param>
    ''' <param name="debug"></param>
    Sub New(host As IPEndPoint, log As Action(Of String), Optional debug As Boolean = False)
        Dim map_name As String = If(debug, "debug-blender", Service.GetMappedChannel(App.PID))

        Me.host = New RequestClient(host)
        Me.channel = New MemoryPipe(MapObject.FromPointer(map_name, 128 * ByteSize.MB))
        Me.log = log
    End Sub

    ''' <summary>
    ''' run in local
    ''' </summary>
    ''' <param name="log"></param>
    ''' <param name="debug"></param>
    Sub New(log As Action(Of String), Optional debug As Boolean = False)
        Dim map_name As String = If(debug, "debug-blender", Service.GetMappedChannel(App.PID))

        Me.host = New LocalRequestClient(Service.CreateHandler(map_name))
        Me.channel = New MemoryPipe(MapObject.FromPointer(map_name, 128 * ByteSize.MB))
        Me.log = log
    End Sub

    Public Function MSIRender(args As PlotProperty, params As MsImageProperty, canvas As Size) As Image
        Dim payload As New Dictionary(Of String, String) From {
            {"sample", sample_tag},
            {"canvas", canvas.GetJson},
            {"params", params.GetJSON},
            {"args", args.GetJSON}
        }
        Dim payload_jsonstr As String = payload.GetJson

        Call log($"do ms-imaging render: {host.ToString}")
        Call log($"msi_render payload: {payload_jsonstr}")

        Dim req As New RequestStream(Service.protocolHandle, Protocol.MSIRender, payload_jsonstr)
        Dim resp = host.SendMessage(req)

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

        Return host.SendMessage(New RequestStream(Service.protocolHandle, Protocol.SetHEmap, "ok!"))
    End Function

    Public Function SetFilters(filters As RasterPipeline)
        Dim shaders As String() = filters.Select(Function(f) f.ToScript).ToArray
        Dim req As New RequestStream(Service.protocolHandle, Protocol.SetFilters, shaders.GetJson)
        Return host.SendMessage(req)
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function SetIntensityRange(range As DoubleRange)
        Dim req As New RequestStream(Service.protocolHandle, Protocol.SetIntensityRange, {range.Min, range.Max}.GetJson)
        Return host.SendMessage(req)
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
        Dim payload_json As String = payload.GetJson

        Call log($"open msi-imaging render session: {host.ToString}")
        Call log($"argument payload for the session creator: {payload_json}")

        Dim req As New RequestStream(Service.protocolHandle, Protocol.OpenSession, payload_json)
        Dim result = host.SendMessage(req)

        If result.IsHTTP_RFC Then
            Throw New Exception(result.GetUTF8String)
        End If

        Return result
    End Function

    Public Function GetTrIQIntensity(trIQ As Double) As Double
        Dim req As New RequestStream(Service.protocolHandle, Protocol.GetTrIQIntensity, NetworkByteOrderBitConvertor.GetBytes(trIQ))
        Dim resp As RequestStream = host.SendMessage(req)
        Dim dbls = NetworkByteOrderBitConvertor.ToDouble(resp.ChunkBuffer)
        Return dbls
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call host.SendMessage(New RequestStream(Service.protocolHandle, Protocol.Shutdown, "Close"))
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
