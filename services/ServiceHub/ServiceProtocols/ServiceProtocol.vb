﻿#Region "Microsoft.VisualBasic::cbea8dcce79bf34b8d010dffcc8ddf9a, mzkit\services\ServiceHub\ServiceProtocols\ServiceProtocol.vb"

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

    '   Total Lines: 188
    '    Code Lines: 127 (67.55%)
    ' Comment Lines: 41 (21.81%)
    '    - Xml Docs: 90.24%
    ' 
    '   Blank Lines: 20 (10.64%)
    '     File Size: 6.78 KB


    ' Enum ServiceProtocol
    ' 
    '     AutoLocation, Bootstrapping, CutBackground, DeleteRegion, ExitApp
    '     ExportMzpack, ExtractMultipleSampleRegions, ExtractRegionMs1Spectrum, ExtractRegionSample, ExtractSamplePixels
    '     GetAnnotationNames, GetBasePeakMzList, GetIonColocalization, GetIonStatList, GetMSIDimensions
    '     GetMSIInformationMetadata, GetPixel, GetPixelPolygon, GetPixelRectangle, LoadGeneLayer
    '     LoadMSI, LoadMSILayers, LoadSummaryLayer, LoadThermoRawMSI, Mirrors
    '     SetSpatial2D, SetSpatialMapping, UnloadMSI, UpsideDown
    ' 
    '  
    ' 
    ' 
    ' 
    ' Module MSIProtocols
    ' 
    '     Function: GetDimensions, GetMSIInfo, GetTotalIons, (+2 Overloads) LoadPixels
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Darwinism.IPC.Networking.HTTP
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON

''' <summary>
''' The MS-imaging data handler protocols
''' </summary>
Public Enum ServiceProtocol
    ''' <summary>
    ''' load MSI engine from mzpack
    ''' </summary>
    LoadMSI
    ''' <summary>
    ''' load MSI engine from thermo raw
    ''' </summary>
    LoadThermoRawMSI
    GetMSIInformationMetadata
    ''' <summary>
    ''' get width/height dimension size metadata of the ms-imaging rawdata file.
    ''' </summary>
    GetMSIDimensions
    GetAnnotationNames
    UnloadMSI
    ExportMzpack
    LoadMSILayers
    LoadGeneLayer
    GetIonStatList
    GetIonColocalization
    GetBasePeakMzList
    GetPixel
    GetPixelRectangle
    GetPixelPolygon
    LoadSummaryLayer
    CutBackground
    UpsideDown
    Mirrors
    AutoLocation
    DeleteRegion

    ''' <summary>
    ''' Extract the data samples for run the downstream data analsyis via the single cell bootstrapping method.
    ''' </summary>
    Bootstrapping

    ''' <summary>
    ''' Extract an overview ms1 spectrum from the specific regions
    ''' </summary>
    ExtractRegionMs1Spectrum
    ''' <summary>
    ''' Extract the spatial rawdata for construct a new MSI rawdata set based on a given region polygon data.
    ''' </summary>
    ExtractRegionSample
    ExtractMultipleSampleRegions

    ''' <summary>
    ''' set spot pixels 2d rotation angle data for do matix rotation
    ''' </summary>
    SetSpatial2D
    ''' <summary>
    ''' set spot spatial mapping
    ''' </summary>
    SetSpatialMapping

    ''' <summary>
    ''' Just call for extract the sample pixels point data
    ''' </summary>
    ExtractSamplePixels
    ExitApp
End Enum

Public Module MSIProtocols

    Public Function GetMSIInfo(render As MSI) As Dictionary(Of String, String)
        Dim uuid As String = ""
        Dim fileSize As String = ""

        If TypeOf render.MSI.pixelReader Is ReadIbd Then
            uuid = DirectCast(render.MSI.pixelReader, ReadIbd).UUID
            fileSize = DirectCast(render.MSI.pixelReader, ReadIbd) _
                .ibd _
                .size _
                .DoCall(AddressOf StringFormats.Lanudry)
        End If

        Return New Dictionary(Of String, String) From {
            {"scan_x", render.metadata.scan_x},
            {"scan_y", render.metadata.scan_y},
            {"uuid", uuid},
            {"fileSize", fileSize},
            {"resolution", render.metadata.resolution},
            {"ion_annotations", If(render.ion_annotations Is Nothing, 0, render.ion_annotations.Count)},
            {"app", render.type.ToString}
        }
    End Function

    Public Function GetTotalIons(handleServiceRequest As Func(Of RequestStream, RequestStream)) As PixelScanIntensity()
        Dim request As RequestStream = MSIRequest.LoadSummaryLayer(IntensitySummary.Total)
        Dim data As RequestStream = handleServiceRequest(request)

        If data Is Nothing Then
            Return {}
        ElseIf data.IsHTTP_RFC Then
            Return {}
        Else
            Return PixelScanIntensity.Parse(data.ChunkBuffer)
        End If
    End Function

    Public Function LoadPixels(id As String, ByRef getBuf As Byte(), handleServiceRequest As Func(Of RequestStream, RequestStream)) As PixelData()
        Dim data As RequestStream = handleServiceRequest(New RequestStream(
            protocolCategory:=MSI.Protocol,
            protocol:=ServiceProtocol.LoadGeneLayer,
            buffer:=Encoding.ASCII.GetBytes(id)
        ))

        getBuf = {}

        If data Is Nothing Then
            Return {}
        Else
            Dim pixels As PixelData() = PixelData.Parse(data.ChunkBuffer)
            'Dim points = pixels.Select(Function(p) New ClusterEntity With {.uid = $"{p.x},{p.y}", .entityVector = {p.x, p.y}}).ToArray
            'Dim densityList = Density.GetDensity(points, k:=stdNum.Min(points.Length / 10, 150), query:=New KDQuery(points)).ToArray
            getBuf = data.ChunkBuffer
            Return pixels
        End If
    End Function

    Public Function GetDimensions(handleServiceRequest As Func(Of RequestStream, RequestStream)) As Integer()
        Dim data As RequestStream = handleServiceRequest(New RequestStream(
            protocolCategory:=MSI.Protocol,
            protocol:=ServiceProtocol.GetMSIDimensions,
            buffer:=Encoding.ASCII.GetBytes("OK!")
        ))

        If data Is Nothing Then
            Return {}
        Else
            Return data.GetUTF8String.LoadJSON(Of Integer())
        End If
    End Function

    ''' <summary>
    ''' load pixels layer via a given set of ion m/z value
    ''' </summary>
    ''' <param name="mz"></param>
    ''' <param name="mzErr"></param>
    ''' <param name="handleServiceRequest"></param>
    ''' <returns></returns>
    Public Function LoadPixels(mz As IEnumerable(Of Double),
                               mzErr As Tolerance,
                               ByRef getBuf As Byte(),
                               handleServiceRequest As Func(Of RequestStream, RequestStream)) As PixelData()

        Dim config As New LayerLoader With {
            .mz = mz.ToArray,
            .method = If(TypeOf mzErr Is PPMmethod, "ppm", "da"),
            .mzErr = mzErr.DeltaTolerance
        }
        Dim configBytes As Byte() = BSON _
            .GetBuffer(config.GetType.GetJsonElement(config, New JSONSerializerOptions)) _
            .ToArray
        Dim data As RequestStream = handleServiceRequest(New RequestStream(
            protocolCategory:=MSI.Protocol,
            protocol:=ServiceProtocol.LoadMSILayers,
            buffer:=configBytes
        ))

        getBuf = {}

        If data Is Nothing Then
            Return {}
        Else
            Dim pixels As PixelData() = PixelData.Parse(data.ChunkBuffer)
            'Dim points = pixels.Select(Function(p) New ClusterEntity With {.uid = $"{p.x},{p.y}", .entityVector = {p.x, p.y}}).ToArray
            'Dim densityList = Density.GetDensity(points, k:=stdNum.Min(points.Length / 10, 150), query:=New KDQuery(points)).ToArray
            getBuf = data.ChunkBuffer
            Return pixels
        End If
    End Function
End Module
