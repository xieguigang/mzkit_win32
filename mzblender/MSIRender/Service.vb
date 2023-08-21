Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Parallel
Imports Task
Imports HeatMap = Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports TcpEndPoint = System.Net.IPEndPoint

<Protocol(GetType(Protocol))>
Public Class Service : Implements IDisposable

    Dim disposedValue As Boolean
    Dim socket As TcpServicesSocket
    Dim channel As MemoryPipe
    Dim blender As MSImagingBlender
    Dim filters As RasterPipeline
    Dim TIC As PixelScanIntensity()

    Public Shared ReadOnly protocolHandle As Long = ProtocolAttribute.GetProtocolCategory(GetType(Service)).EntryPoint

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
        Me.filters = filters
        Return New DataPipe("OK")
    End Function

    <Protocol(Protocol.SetHEmap)>
    Public Function SetHEmap(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        blender.HEMap = channel.LoadImage
        Return New DataPipe("OK")
    End Function

    <Protocol(Protocol.OpenSession)>
    Public Function OpenSession(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        Dim data = request.LoadObject(Of Dictionary(Of String, String))
        Dim ss As String = data!ss

        ' load pixels data
        Select Case ss
            Case NameOf(HeatMapBlender)
                Dim pixels = HeatMap.PixelData.ParseStream(channel.LoadStream)
                Dim dims As Size = data!args.SizeParser

                blender = New HeatMapBlender(pixels, dims, filters)
            Case NameOf(RGBIonMSIBlender)
                Dim pixels = PixelData.Parse(channel.LoadStream)
                data = data!args.LoadJSON(Of Dictionary(Of String, String))
                Dim rgb As RGBConfigs = data!args.LoadJSON(Of RGBConfigs)
                Dim mzdiff As Tolerance = Tolerance.ParseScript(data!mzdiff)
                Dim Rpixels = pixels.Where(Function(p) mzdiff(p.mz, rgb.R)).ToArray
                Dim Gpixels = pixels.Where(Function(p) mzdiff(p.mz, rgb.G)).ToArray
                Dim Bpixels = pixels.Where(Function(p) mzdiff(p.mz, rgb.B)).ToArray

                blender = New RGBIonMSIBlender(Rpixels, Gpixels, Bpixels, TIC, filters)
            Case NameOf(SingleIonMSIBlender)
                Dim dims As Size = data!args.SizeParser
                Dim pixels As PixelData() = PixelData.Parse(channel.LoadStream)
                Dim layer As New SingleIonLayer With {
                    .DimensionSize = dims,
                    .MSILayer = pixels,
                    .IonMz = ""
                }

                blender = New SingleIonMSIBlender(pixels, filters, Nothing)
            Case NameOf(SummaryMSIBlender)
                Dim pixels As PixelScanIntensity() = PixelScanIntensity.Parse(channel.LoadStream)

                blender = New SummaryMSIBlender(pixels, filters)
            Case Else
                Throw New InvalidDataException("invalid session open parameter!")
        End Select

        Return New DataPipe("OK")
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
        Dim json As Dictionary(Of String, String) = request.LoadObject(Of Dictionary(Of String, String))
        Dim args As PlotProperty = PlotProperty.ParseJSON(json!args)
        Dim params As MsImageProperty = json!params.LoadJSON(Of MsImageProperty)
        Dim canvas As Size = json!canvas.LoadJSON(Of Size)
        Dim sample As String = json!sample
        Dim msi As Image = blender.Rendering(args, canvas, params, sample)

        Using ms As New MemoryStream
            Call msi.Save(ms, ImageFormat.Png)
            Call ms.Flush()
            Call channel.WriteBuffer(ms.ToArray)
        End Using

        Return New DataPipe("OK!")
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
