Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.HTTP
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Parallel
Imports TcpEndPoint = System.Net.IPEndPoint

''' <summary>
''' 常住于内存中的对象数据库服务
''' </summary>
<Protocol(GetType(Protocols))>
Public Class Service : Implements IDisposable

    Public Shared ReadOnly Property Protocol As Long = New ProtocolAttribute(GetType(Protocols)).EntryPoint

    ReadOnly host As TcpServicesSocket
    ReadOnly background As Task
    ReadOnly redisObjs As New Dictionary(Of String, UnmanageMemoryRegion)

    Private disposedValue As Boolean

    Public ReadOnly Property TcpPort As Integer
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return host.LocalPort
        End Get
    End Property

    Private Sub New(port As Integer)
        host = New TcpServicesSocket(port)
        host.ResponseHandler = AddressOf New ProtocolHandler(Me, debug:=False).HandleRequest

        Call RunSlavePipeline.SendMessage($"socket={TcpPort}")
    End Sub

    Public Function Run() As Integer
        Return host.Run
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Start(port As Integer) As Service
        Return New Service(port)
    End Function

    <Protocol(Protocols.GetValue)>
    Public Function GetValue(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        Dim key As String = request.GetUTF8String.Trim

        If Not redisObjs.ContainsKey(key) Then
            Return New DataPipe(NetResponse.RFC_NOT_FOUND)
        End If

        Dim redisObj As UnmanageMemoryRegion = redisObjs(key)
        Dim json As String = redisObj.GetJson

        Return New DataPipe(json)
    End Function

    <Protocol(Protocols.LoadMzPack)>
    Public Function LoadMzPack(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        Dim path As String = request.GetUTF8String.Trim
        Dim keys1 As New List(Of String)

        If Not path.FileExists Then
            Return New DataPipe(NetResponse.RFC_NOT_FOUND)
        End If

        Using file As Stream = path.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim mzpack As mzPack = mzPack.ReadAll(file)
            Dim base As String = path.FileName
            Dim i As Integer = 0

            For Each scan As ScanMS1 In mzpack.MS
                Dim key As String = $"{base}#{scan.scan_id}"
                Dim keys2 As New List(Of String)

                For Each ms2 As ScanMS2 In scan.products.SafeQuery
                    Dim key2 As String = $"{base}#{ms2.scan_id}"

                    If MapObject.Exists(key2) Then
                        Continue For
                    End If

                    keys2.Add(key2)
                    redisObjs.Add(key2, MapObject.FromObject(ms2, hMemP:=key2))
                Next

                If Not MapObject.Exists(key) Then
                    redisObjs.Add(key, MapObject.FromObject(RedisScan1.FromData(scan, keys2), key))
                End If

                Call keys1.Add(key)

                If CInt(i / mzpack.MS.Length * 100) Mod 5 = 0 Then
                    Call RunSlavePipeline.SendProgress(i / mzpack.MS.Length, $"[redis_add] {key}")
                End If

                i += 1
            Next
        End Using

        Return New DataPipe(keys1.ToArray.GetJson)
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: 释放托管状态(托管对象)
                Call host.Dispose()
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
