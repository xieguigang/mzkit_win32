﻿#Region "Microsoft.VisualBasic::0059c601a6860d7480e0e0852600b176, mzkit\mzblender\MSIRender\Service.vb"

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

    '   Total Lines: 272
    '    Code Lines: 204 (75.00%)
    ' Comment Lines: 23 (8.46%)
    '    - Xml Docs: 17.39%
    ' 
    '   Blank Lines: 45 (16.54%)
    '     File Size: 12.04 KB


    ' Class Service
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: CreateHandler, GetMappedChannel, GetTrIQIntensity, MSIRender, OpenSession
    '               Run, SetFilters, SetHEmap, SetIntensityRange, Shutdown
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
Imports MZKitWin32.Blender.CommonLibs
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
        Call Me.New(masterChannel)

        socket = New TcpServicesSocket(port) With {.KeepsAlive = False}
        socket.ResponseHandler = AddressOf New ProtocolHandler(Me).HandleRequest
    End Sub

    ''' <summary>
    ''' construct a local services handler
    ''' </summary>
    ''' <param name="masterChannel"></param>
    Sub New(masterChannel As String)
        channel = New MemoryPipe(MapObject.Allocate(128 * ByteSize.MB, hMemP:=GetMappedChannel(masterChannel)))
    End Sub

    Public Shared Function CreateHandler(masterChannel As String) As ProtocolHandler
        Return New ProtocolHandler(New Service(masterChannel))
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function GetMappedChannel(master As String) As String
        Return If(master = "debug-blender", master, If(master.IsInteger, $"mzblender_{master}", master))
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
                Dim pixels = HeatMap.PixelData.ParseStream(channel.LoadStream).ToArray
                Dim dims As Size = data!dims.SizeParser

                Call RunSlavePipeline.SendMessage($" * heatmap rendering based on {pixels.Length} pixels data!")

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

                Call RunSlavePipeline.SendMessage($" * rgb rendering: {rgb.GetJSON}")
                Call RunSlavePipeline.SendMessage($" *** heatmap R: {Rpixels.Length} pixels!")
                Call RunSlavePipeline.SendMessage($" *** heatmap G: {Gpixels.Length} pixels!")
                Call RunSlavePipeline.SendMessage($" *** heatmap B: {Bpixels.Length} pixels!")

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

                Call RunSlavePipeline.SendMessage($" * single ion rendering: {pixels} pixels!")

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

                Call RunSlavePipeline.SendMessage($" * summary pixels: {pixels.Length}")

                If Not filters Is Nothing Then
                    pixels = filters _
                        .DoIntensityScale(pixels.CreatePixelData(Of PixelData), dims) _
                        .ExtractPixels _
                        .ToArray
                End If

                TIC = pixels
                TICImage = SummaryMSIBlender.Rendering(TIC, dims, "gray", 60, "transparent")
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

        If blender Is Nothing Then
            Throw New InvalidOperationException("needs open a blender session at first!")
        End If

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
