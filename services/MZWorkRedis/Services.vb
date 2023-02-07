Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Net.Tcp

''' <summary>
''' 常住于内存中的对象数据库服务
''' </summary>
<Protocol(GetType(Protocols))>
Public Class Service : Implements IDisposable

    Public Shared ReadOnly Property Protocol As Long = New ProtocolAttribute(GetType(Protocols)).EntryPoint

    ReadOnly host As TcpServicesSocket
    ReadOnly background As Task

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
        background = Task.Run(AddressOf host.Run)

        Call RunSlavePipeline.SendMessage($"socket={TcpPort}")
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function Start(port As Integer) As Service
        Return New Service(port)
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
