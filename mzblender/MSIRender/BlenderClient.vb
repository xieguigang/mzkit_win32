﻿Imports System.Drawing.Imaging
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.HTTP
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Parallel
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

    Private disposedValue As Boolean

    Sub New(host As IPEndPoint, Optional debug As Boolean = False)
        Dim map_name As String = If(debug, "debug-blender", Service.GetMappedChannel(App.PID))

        Me.host = host
        Me.channel = New MemoryPipe(MapObject.FromPointer(map_name, 128 * 1024 * 1024))
    End Sub

    Private Function handleRequest(req As RequestStream) As RequestStream
        Return New TcpRequest(host).SetTimeOut(TimeSpan.FromSeconds(60)).SendMessage(req)
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
            Return channel.LoadImage
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