Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Parallel
Imports Task
Imports TcpEndPoint = System.Net.IPEndPoint

<Protocol(GetType(Protocol))>
Public Class Service : Implements IDisposable

    Dim disposedValue As Boolean
    Dim socket As TcpServicesSocket
    Dim channel As MemoryPipe
    Dim blender As MSImagingBlender

    Public Shared ReadOnly protocolHandle As Long = ProtocolAttribute.GetProtocolCategory(GetType(Protocol)).EntryPoint

    Sub New(port As Integer, masterChannel As String)
        socket = New TcpServicesSocket(port)
        socket.ResponseHandler = AddressOf New ProtocolHandler(Me).HandleRequest
        channel = New MemoryPipe(MapObject.Allocate(128 * 1024 * 1024, hMemP:=GetMappedChannel(masterChannel)))
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function GetMappedChannel(master As String) As String
        Return $"mzblender_{master}"
    End Function

    Public Function Run() As Integer
        Return socket.Run
    End Function

    <Protocol(Protocol.SetFilters)>
    Public Function SetFilters(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        Dim shaders As String() = request.LoadObject(Of String())
        Dim filters As RasterPipeline = RasterPipeline.Parse(shaders)
        blender.filters = filters
        Return New DataPipe("OK")
    End Function

    <Protocol(Protocol.SetHEmap)>
    Public Function SetHEmap(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        blender.HEMap = channel.LoadImage
        Return New DataPipe("OK")
    End Function

    <Protocol(Protocol.OpenSession)>
    Public Function OpenSession(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        Dim ss As String = request.GetUTF8String

        ' load pixels data
        Select Case ss
            Case NameOf(HeatMapBlender)
            Case NameOf(RGBIonMSIBlender)
            Case NameOf(SingleIonMSIBlender)
            Case NameOf(SummaryMSIBlender)
            Case Else
                Throw New InvalidDataException("invalid session open parameter!")
        End Select
    End Function

    <Protocol(Protocol.SetIntensityRange)>
    Public Function SetIntensityRange(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        Dim range As Double() = request.LoadObject(Of Double())
        blender.SetIntensityRange(New DoubleRange(range))
        Return New DataPipe("OK")
    End Function

    <Protocol(Protocol.GetTrIQIntensity)>
    Public Function GetTrIQIntensity(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        Dim cutoff As Double = NetworkByteOrderBitConvertor.ToDouble(request.ChunkBuffer)
        Dim q As Double = blender.GetTrIQIntensity(cutoff)
        Return New DataPipe(NetworkByteOrderBitConvertor.GetBytes(q))
    End Function

    <Protocol(Protocol.MSIRender)>
    Public Function MSIRender(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        ' get parameters from the request
        Dim args As PlotProperty
        Dim canvas As Size
        Dim sample As String
        Dim msi As Image = blender.Rendering(args, canvas, sample)

        Using ms As New MemoryStream
            Call msi.Save(ms, ImageFormat.Png)
            Call ms.Flush()


        End Using
    End Function

    <Protocol(Protocol.Shutdown)>
    Public Function Shutdown(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        Call Me.Dispose()
        Return New DataPipe("OK!")
    End Function

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects)
                Call socket.Dispose()
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override finalizer
            ' TODO: set large fields to null
            disposedValue = True
        End If
    End Sub

    ' ' TODO: override finalizer only if 'Dispose(disposing As Boolean)' has code to free unmanaged resources
    ' Protected Overrides Sub Finalize()
    '     ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
    '     Dispose(disposing:=False)
    '     MyBase.Finalize()
    ' End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code. Put cleanup code in 'Dispose(disposing As Boolean)' method
        Dispose(disposing:=True)
        GC.SuppressFinalize(Me)
    End Sub
End Class
