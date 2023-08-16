Imports System.IO
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

    Public Function OpenSession(data As MemoryStream)
        Dim buf = channel.OpenFile
        buf.Write(data.ToArray, Scan0, data.Length)
        buf.Flush()
        Dim result = handleRequest(New RequestStream(Service.protocolHandle, Protocol.OpenSession, "ok!"))

    End Function

End Class
