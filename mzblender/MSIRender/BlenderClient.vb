Imports System.Drawing.Imaging
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Parallel

Public Class BlenderClient

    ReadOnly host As IPEndPoint
    ReadOnly channel As MapObject

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

    Sub New(host As IPEndPoint)
        Me.host = host
        Me.channel = MapObject.FromPointer(Service.GetMappedChannel(App.PID), 128 * 1024 * 1024)
    End Sub

    Private Function handleRequest(req As RequestStream) As RequestStream
        Return New TcpRequest(host).SetTimeOut(TimeSpan.FromSeconds(60)).SendMessage(req)
    End Function

    Public Sub WriteBuffer(ByRef data As Byte())
        Dim buf As Stream = channel.OpenFile

        Call buf.Seek(Scan0, SeekOrigin.Begin)
        Call buf.Write(BitConverter.GetBytes(data.Length), 0, 4)
        Call buf.Write(data, Scan0, data.Length)
        Call buf.Flush()

        Erase data
    End Sub

    Public Function SetSampleTag(tag As String)
        Return handleRequest(New RequestStream(Service.protocolHandle, Protocol.SetSampleTag, tag))
    End Function

    Public Function SetHEMap(img As Image)
        Using ms As New MemoryStream
            Call img.Save(ms, ImageFormat.Png)
            Call ms.Flush()
            Call WriteBuffer(ms.ToArray)
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

    Public Function OpenSession()
        Dim result = handleRequest(New RequestStream(Service.protocolHandle, Protocol.OpenSession, "ok!"))
        Return result
    End Function

    Public Function GetTrIQIntensity(trIQ As Double) As Double
        Dim req As New RequestStream(Service.protocolHandle, Protocol.GetTrIQIntensity, NetworkByteOrderBitConvertor.GetBytes(trIQ))
        Dim resp As RequestStream = handleRequest(req)
        Dim dbls = NetworkByteOrderBitConvertor.ToDouble(resp.ChunkBuffer)
        Return dbls
    End Function
End Class
