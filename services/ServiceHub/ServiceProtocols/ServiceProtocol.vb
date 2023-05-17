#Region "Microsoft.VisualBasic::bacd8d20bc88822dbf2517abdcb6d8c1, mzkit\src\mzkit\services\ServiceHub\ServiceProtocols\ServiceProtocol.vb"

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

'   Total Lines: 84
'    Code Lines: 68
' Comment Lines: 8
'   Blank Lines: 8
'     File Size: 2.89 KB


' Enum ServiceProtocol
' 
'     CutBackground, ExitApp, ExportMzpack, ExtractMultipleSampleRegions, ExtractRegionSample
'     GetBasePeakMzList, GetIonColocalization, GetIonStatList, GetMSIInformationMetadata, GetPixel
'     GetPixelPolygon, GetPixelRectangle, LoadMSI, LoadMSILayers, LoadSummaryLayer
'     LoadThermoRawMSI, Mirrors, UnloadMSI, UpsideDown
' 
'  
' 
' 
' 
' Module MSIProtocols
' 
'     Function: GetMSIInfo, LoadPixels
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Net.HTTP
Imports Microsoft.VisualBasic.Parallel

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
    UnloadMSI
    ExportMzpack
    LoadMSILayers
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
    DeleteRegion
    ExtractRegionMs1Spectrum
    ExtractRegionSample
    ExtractMultipleSampleRegions
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
            {"ion_annotations", If(render.ion_annotations Is Nothing, 0, render.ion_annotations.Count)}
        }
    End Function

    Public Function GetTotalIons(handleServiceRequest As Func(Of RequestStream, RequestStream)) As PixelScanIntensity()
        Dim request As New RequestStream(MSI.Protocol, ServiceProtocol.LoadSummaryLayer, BitConverter.GetBytes(CInt(IntensitySummary.Total)))
        Dim data As RequestStream = handleServiceRequest(request)

        If data Is Nothing Then
            Return {}
        ElseIf data.IsHTTP_RFC Then
            Return {}
        Else
            Return PixelScanIntensity.Parse(data.ChunkBuffer)
        End If
    End Function

    Public Function LoadPixels(mz As IEnumerable(Of Double),
                               mzErr As Tolerance,
                               handleServiceRequest As Func(Of RequestStream, RequestStream)) As PixelData()

        Dim config As New LayerLoader With {
            .mz = mz.ToArray,
            .method = If(TypeOf mzErr Is PPMmethod, "ppm", "da"),
            .mzErr = mzErr.DeltaTolerance
        }
        Dim configBytes As Byte() = BSON.GetBuffer(config.GetType.GetJsonElement(config, New JSONSerializerOptions)).ToArray
        Dim data As RequestStream = handleServiceRequest(New RequestStream(
            protocolCategory:=MSI.Protocol,
            protocol:=ServiceProtocol.LoadMSILayers,
            buffer:=configBytes
        ))

        If data Is Nothing Then
            Return {}
        Else
            Dim pixels As PixelData() = PixelData.Parse(data.ChunkBuffer)
            'Dim points = pixels.Select(Function(p) New ClusterEntity With {.uid = $"{p.x},{p.y}", .entityVector = {p.x, p.y}}).ToArray
            'Dim densityList = Density.GetDensity(points, k:=stdNum.Min(points.Length / 10, 150), query:=New KDQuery(points)).ToArray
            Return pixels
        End If
    End Function
End Module
