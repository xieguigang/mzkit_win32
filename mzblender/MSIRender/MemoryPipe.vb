Imports System.IO
Imports System.Runtime.CompilerServices
Imports Parallel

Public Class MemoryPipe

    Dim channel As MapObject

    Sub New(channel As MapObject)
        Me.channel = channel

        If Not MapObject.Exists(channel) Then
            Call MapObject.Allocate(channel)
        End If
    End Sub

    Public Function LoadStream() As Byte()
        Dim file As Stream = channel.OpenFile
        Dim bytes As Byte() = New Byte(4 - 1) {}

        Call file.Seek(Scan0, SeekOrigin.Begin)
        Call file.Read(bytes, Scan0, bytes.Length)

        bytes = New Byte(BitConverter.ToInt32(bytes, Scan0) - 1) {}

        Call file.Read(bytes, Scan0, bytes.Length)

        Return bytes
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function LoadImage() As Image
        Return Image.FromStream(New MemoryStream(LoadStream))
    End Function

    Public Sub WriteBuffer(data As MemoryStream)
        Dim buf As Stream = channel.OpenFile

        Call buf.Seek(Scan0, SeekOrigin.Begin)
        Call buf.Write(BitConverter.GetBytes(data.Length), 0, 4)
        Call buf.Write(data.ToArray, Scan0, data.Length)
        Call buf.Flush()

        data.Dispose()
    End Sub

    Public Sub WriteBuffer(ByRef data As Byte())
        Dim buf As Stream = channel.OpenFile

        Call buf.Seek(Scan0, SeekOrigin.Begin)
        Call buf.Write(BitConverter.GetBytes(data.Length), 0, 4)
        Call buf.Write(data, Scan0, data.Length)
        Call buf.Flush()

        Erase data
    End Sub

    Public Overrides Function ToString() As String
        Return channel.ToString
    End Function
End Class
