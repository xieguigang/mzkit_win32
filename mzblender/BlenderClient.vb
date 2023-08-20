Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Parallel

Public Class BlenderClient

    ReadOnly host As IPEndPoint
    ReadOnly channel As MapObject

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

    Public Function SetFilters(filters As RasterPipeline)

    End Function

    Public Function OpenSession()
        Dim result = handleRequest(New RequestStream(Service.protocolHandle, Protocol.OpenSession, "ok!"))

    End Function

End Class
