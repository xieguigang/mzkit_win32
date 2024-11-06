#Region "Microsoft.VisualBasic::589c0a5879d52867539aa5aba5587bf0, mzkit\mzblender\MSIRender\Service.vb"

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

    '   Total Lines: 236
    '    Code Lines: 182 (77.12%)
    ' Comment Lines: 19 (8.05%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 35 (14.83%)
    '     File Size: 10.46 KB


    ' Class Service
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetMappedChannel, GetTrIQIntensity, MSIRender, OpenSession, Run
    '               SetFilters, SetHEmap, SetIntensityRange, Shutdown
    ' 
    '     Sub: (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing.Imaging
Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Darwinism.HPC.Parallel
Imports Darwinism.IPC.Networking.Protocols.Reflection
Imports Darwinism.IPC.Networking.Tcp
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization.JSON
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
    Dim TICImage As Image
    Dim sample_outlines As GeneralPath

    Public Shared ReadOnly protocolHandle As Long = ProtocolAttribute.GetProtocolCategory(GetType(Service)).EntryPoint

    Sub New(port As Integer, masterChannel As String)
        socket = New TcpServicesSocket(port) With {.KeepsAlive = False}
        socket.ResponseHandler = AddressOf New ProtocolHandler(Me).HandleRequest
        channel = New MemoryPipe(MapObject.Allocate(128 * ByteSize.MB, hMemP:=GetMappedChannel(masterChannel)))
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function GetMappedChannel(master As String) As String
        Return If(master = "debug-blender", master, $"mzblender_{master}")
    End Function

    Public Function Run() As Integer
        Return socket.Run
    End Function

    <Protocol(Protocol.SetFilters)>
    Public Function SetFilters(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        Dim shaders As String() = request.LoadObject(Of String())
        Dim filters As RasterPipeline = RasterPipeline.Parse(shaders)

        If blender IsNot Nothing Then
            blender.filters = filters
        End If

        RunSlavePipeline.SendMessage($"set filter: {filters.ToScript}")
        Me.filters = filters

        Return New DataPipe("OK")
    End Function

    <Protocol(Protocol.SetHEmap)>
    Public Function SetHEmap(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        If blender IsNot Nothing Then
            blender.HEMap = channel.LoadImage.CTypeGdiImage
        End If

        RunSlavePipeline.SendMessage($"set HE-stain map image: w:{blender.HEMap.Width},h:{blender.HEMap.Height}")
        Return New DataPipe("OK")
    End Function

    <Protocol(Protocol.OpenSession)>
    Public Function OpenSession(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        Dim data = request.LoadObject(Of Dictionary(Of String, String))
        Dim ss As String = data!ss
        Dim params As MsImageProperty = MsImageProperty.ParseJSON(data.TryGetValue("params"))
        Dim args As PlotProperty = PlotProperty.ParseJSON(data.TryGetValue("args"))

        RunSlavePipeline.SendMessage($"open a new session: {ss}")
        RunSlavePipeline.SendMessage(data.TryGetValue("args"))
        RunSlavePipeline.SendMessage(data.TryGetValue("params"))
        RunSlavePipeline.SendMessage(data.TryGetValue("dims"))

        ' load pixels data
        Select Case ss
            Case NameOf(HeatMapBlender)
                Dim pixels = HeatMap.PixelData.ParseStream(channel.LoadStream)
                Dim dims As Size = data!dims.SizeParser

                blender = New HeatMapBlender(pixels, dims, filters) With {
                    .filters = filters,
                    .sample_outline = sample_outlines
                }
            Case NameOf(RGBIonMSIBlender)
                Dim pixels = PixelData.Parse(channel.LoadStream)
                data = data!configs.LoadJSON(Of Dictionary(Of String, String))
                Dim rgb As RGBConfigs = RGBConfigs.ParseJSON(data!rgb)
                Dim mzdiff As Tolerance = Tolerance.ParseScript(data!mzdiff)
                Dim Rpixels = pixels.Where(Function(p) mzdiff(p.mz, rgb.R)).ToArray
                Dim Gpixels = pixels.Where(Function(p) mzdiff(p.mz, rgb.G)).ToArray
                Dim Bpixels = pixels.Where(Function(p) mzdiff(p.mz, rgb.B)).ToArray

                blender = New RGBIonMSIBlender(Rpixels, Gpixels, Bpixels, TICImage, filters) With {
                    .filters = filters,
                    .sample_outline = sample_outlines
                }
            Case NameOf(SingleIonMSIBlender)
                Dim dims As Size = data!dims.SizeParser
                Dim pixels As PixelData() = PixelData.Parse(channel.LoadStream)
                Dim layer As New SingleIonLayer With {
                    .DimensionSize = dims,
                    .MSILayer = pixels,
                    .IonMz = ""
                }

                blender = New SingleIonMSIBlender(pixels, filters, params, TICImage) With {
                    .filters = filters,
                    .sample_outline = sample_outlines
                }
            Case NameOf(SummaryMSIBlender)
                Dim pixels As PixelScanIntensity() = PixelScanIntensity.Parse(channel.LoadStream)
                Dim dims As Size = data!dims.SizeParser
                ' 20240702 too much cpu load for calculate the outline path
                '
                ' Dim xi = pixels.Select(Function(a) a.x).ToArray
                ' Dim yi = pixels.Select(Function(a) a.y).ToArray
                ' Dim shapes = ContourLayer.GetOutline(xi, yi, 5)

                ' shapes = shapes.Bspline(degree:=5, 100).FilterSmallPolygon(0.1)

                ' sample_outlines = shapes
                TIC = pixels
                TICImage = SummaryMSIBlender.Rendering(TIC, dims, "gray", 250, "transparent")
                blender = New SummaryMSIBlender(pixels, filters) With {
                    .filters = filters,
                    .sample_outline = sample_outlines
                }
            Case Else
                Throw New InvalidDataException("invalid session open parameter!")
        End Select

        RunSlavePipeline.SendMessage("OK!")

        Return New DataPipe("OK")
    End Function

    <Protocol(Protocol.SetIntensityRange)>
    Public Function SetIntensityRange(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        Dim range As Double() = request.LoadObject(Of Double())
        If blender IsNot Nothing Then
            blender.SetIntensityRange(New DoubleRange(range))
        End If
        RunSlavePipeline.SendMessage($"set intensity range: {range.GetJson}")
        Return New DataPipe("OK")
    End Function

    <Protocol(Protocol.GetTrIQIntensity)>
    Public Function GetTrIQIntensity(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        Dim cutoff As Double = NetworkByteOrderBitConvertor.ToDouble(request.ChunkBuffer)
        Dim q As Double
        If blender IsNot Nothing Then
            q = blender.GetTrIQIntensity(cutoff)
        Else
            q = -1
        End If
        RunSlavePipeline.SendMessage($"get TrIQ intensity cutoff: {q}@{cutoff}!")
        Return New DataPipe(NetworkByteOrderBitConvertor.GetBytes(q))
    End Function

    <Protocol(Protocol.MSIRender)>
    Public Function MSIRender(request As RequestStream, remoteDevcie As TcpEndPoint) As BufferPipe
        ' get parameters from the request
        Dim json As Dictionary(Of String, String) = request.LoadObject(Of Dictionary(Of String, String))
        Dim args As PlotProperty = PlotProperty.ParseJSON(json!args)
        Dim params As MsImageProperty = MsImageProperty.ParseJSON(json!params)
        Dim canvas As Size = json!canvas.LoadJSON(Of Size)
        Dim sample As String = json!sample

        RunSlavePipeline.SendMessage("Do ms-imaging rendering:")
        RunSlavePipeline.SendMessage(json!args)
        RunSlavePipeline.SendMessage(json!params)
        RunSlavePipeline.SendMessage(json!canvas)
        RunSlavePipeline.SendMessage(json!sample)

        Dim msi As Image = blender.Rendering(args, canvas, params, sample)

        Using ms As New MemoryStream
            Call msi.Save(ms, ImageFormat.Png)
            Call ms.Flush()
            Call RunSlavePipeline.SendMessage($"MSI: w{msi.Width};h{msi.Height};{StringFormats.Lanudry(ms.Length)}")
            Call channel.WriteBuffer(ms.ToArray)
        End Using

        RunSlavePipeline.SendMessage("OK!")

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
