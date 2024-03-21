#Region "Microsoft.VisualBasic::dc709719d2eb0d4fdc41766495d7ac4f, mzkit\src\mzkit\services\ServiceHub\ServiceProtocols\MSI.vb"

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

'   Total Lines: 553
'    Code Lines: 402
' Comment Lines: 63
'   Blank Lines: 88
'     File Size: 23.06 KB


' Class MSI
' 
'     Properties: Protocol, TcpPort
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: CutBackground, ExportMzPack, ExtractMultipleSampleRegions, ExtractRegionSample, GetBPCIons
'               GetIonColocalization, GetIonStatList, GetMSIInformationMetadata, GetMSILayers, GetPixel
'               GetPixelRectangle, Load, LoadSummaryLayer, Quit, Run
'               TurnMirrors, TurnUpsideDown, Unload
' 
'     Sub: (+2 Overloads) Dispose
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology.HEMap
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.Data
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.application.json.Javascript
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Net.Tcp
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization.JSON
Imports MZWorkPack
Imports Parallel
Imports std = System.Math

<Protocol(GetType(ServiceProtocol))>
Public Class MSI : Implements ITaskDriver, IDisposable

    Public Shared ReadOnly Property Protocol As Long = New ProtocolAttribute(GetType(ServiceProtocol)).EntryPoint

    Dim socket As TcpServicesSocket

    Friend type As FileApplicationClass
    ''' <summary>
    ''' [mz => name]
    ''' </summary>
    Friend ion_annotations As Dictionary(Of String, String)

    Dim map_to_ion As New Dictionary(Of String, Double)

    Friend MSI As Drawer
    ' only updates when the file load function invoke
    ' which it means the session changed
    Friend metadata As Metadata
    Friend sourceName As String

    Private disposedValue As Boolean

    Public ReadOnly Property TcpPort As Integer
        Get
            Return socket.LocalPort
        End Get
    End Property

    Sub New(Optional debugPort As Integer? = Nothing, Optional masterPid As String = Nothing)
        Dim port As Integer = If(debugPort Is Nothing, GetFirstAvailablePort(), debugPort)

        Me.socket = New TcpServicesSocket(port, debug:=Not debugPort Is Nothing)
        Me.socket.ResponseHandler = AddressOf New ProtocolHandler(Me, debug:=Not debugPort Is Nothing).HandleRequest

        Call RunSlavePipeline.SendMessage($"socket={TcpPort}")
        Call BackgroundTaskUtils.BindToMaster(masterPid, Me)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Run() As Integer Implements ITaskDriver.Run
        Return socket.Run
    End Function

    ''' <summary>
    ''' just pass a degree angle, apply the matrix rotation at here
    ''' </summary>
    ''' <param name="request"></param>
    ''' <param name="remoteAddress"></param>
    ''' <returns></returns>
    <Protocol(ServiceProtocol.SetSpatial2D)>
    Public Function SetSpatial2D(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim angle As Double = BitConverter.ToDouble(request.ChunkBuffer, Scan0)
        Dim r As Double = angle.ToRadians
        Dim rawPixels As PixelScan() = MSI.LoadPixels.ToArray
        Dim matrix = rawPixels.Select(Function(p) New PointF(p.X, p.Y)).ToArray
        Dim polygon As New Polygon2D(matrix)
        Dim center As New PointF With {
            .X = polygon.xpoints.Average,
            .Y = polygon.ypoints.Average
        }

        matrix = matrix.Rotate(center, alpha:=r)

        Dim minX As Double = matrix.Select(Function(p) p.X).Min
        Dim minY As Double = matrix.Select(Function(p) p.Y).Min

        matrix = matrix _
            .Select(Function(p) New PointF(p.X - minX, p.Y)) _
            .ToArray
        matrix = matrix _
            .Select(Function(p) New PointF(p.X, p.Y - minY)) _
            .ToArray

        Return SetSpatial2D(rawPixels, matrix, Nothing)
    End Function

    Private Function SetSpatial2D(rawPixels As PixelScan(), matrix As PointF(), newDims As Size?) As BufferPipe
        Dim info As Dictionary(Of String, String)

        For i As Integer = 0 To matrix.Length - 1
            rawPixels(i) = rawPixels(i).SetXY(matrix(i).X, matrix(i).Y)
        Next

        MSI = New Drawer(rawPixels.Select(Function(d) DirectCast(d, mzPackPixel)).ToArray)

        If newDims Is Nothing Then
            metadata.scan_x = MSI.dimension.Width
            metadata.scan_y = MSI.dimension.Height
        Else
            metadata.scan_x = newDims?.Width
            metadata.scan_y = newDims?.Height
        End If

        info = MSIProtocols.GetMSIInfo(Me)
        info!source = sourceName

        Return New DataPipe(info.GetJson(indent:=False, simpleDict:=True))
    End Function

    <Protocol(ServiceProtocol.SetSpatialMapping)>
    Public Function SetSpatialMapping(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim filepath As String = request.GetUTF8String
        Dim register As SpatialRegister = SpatialRegister.ParseFile(filepath.OpenReadonly)
        Dim rawPixels As PixelScan() = MSI.LoadPixels.ToArray
        Dim newSpatial As PointF() = rawPixels _
            .Select(Function(i) New PointF(i.X, i.Y)) _
            .DoCall(Function(p)
                        Return register.SpatialTranslation(p.ToArray)
                    End Function)

        Return SetSpatial2D(rawPixels, newSpatial, register.viewSize)
    End Function

    <Protocol(ServiceProtocol.GetMSIDimensions)>
    Public Function GetMSIDimensions(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim dims As Integer() = New Integer() {MSI.dimension.Width, MSI.dimension.Height}
        Dim payload As String = dims.GetJson(indent:=False, simpleDict:=True)
        Return New DataPipe(payload)
    End Function

    ''' <summary>
    ''' this method required a size parameter for fold the raw scans into 2D matrix
    ''' </summary>
    ''' <param name="request"></param>
    ''' <param name="remoteAddress"></param>
    ''' <returns></returns>
    <Protocol(ServiceProtocol.LoadThermoRawMSI)>
    Public Function loadMzML(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        ' $"{dimSize.Width},{dimSize.Height}={raw}"
        Dim strConfig As String = request.GetUTF8String
        Dim parse = strConfig.GetTagValue("=", trim:=True)
        Dim dims As Size = parse.Name.SizeParser
        Dim filepath As String = parse.Value
        Dim msi As mzPack
        Dim info As Dictionary(Of String, String)

        If filepath.ExtensionSuffix("mzML") Then
            If RawScanParser.IsMRMData(filepath) Then
                ' load ms-imaging data via MRM mode
                Dim ions = mzML.LoadChromatogramList(filepath).ToArray
                Dim mzpack As mzPack = ions.ConvertMzMLFile(source:=filepath.BaseName, size:=dims.Area)

                msi = mzpack.ConvertToMSI(dims)
            Else
                ' load ms-imaging data via LC-MS mode
                Dim mzpack As mzPack = Converter.LoadRawFileAuto(xml:=filepath)
                msi = mzpack.ConvertToMSI(dims)
            End If
        ElseIf filepath.ExtensionSuffix("mzPack") Then
            ' the mzpack is not row scans result
            Call RunSlavePipeline.SendMessage($"read MSI dataset from the mzPack raw data file!")

            msi = MSImagingReader.UnifyReadAsMzPack(filepath).TryCast(Of mzPack).ConvertToMSI(dims)
        Else
            Return New DataPipe("invalid file type!")
        End If

        Call LoadMSIMzPackCommon(msi)

        info = MSIProtocols.GetMSIInfo(Me)
        info!source = sourceName

        Return New DataPipe(info.GetJson(indent:=False, simpleDict:=True))
    End Function

    ''' <summary>
    ''' load mzPack data
    ''' </summary>
    ''' <param name="request"></param>
    ''' <param name="remoteAddress"></param>
    ''' <returns></returns>
    <Protocol(ServiceProtocol.LoadMSI)>
    Public Function Load(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim filepath As String = request.GetString(Encoding.UTF8)
        Dim info As Dictionary(Of String, String)

        sourceName = filepath.FileName

        If filepath.ExtensionSuffix("cdf") Then
            Call RunSlavePipeline.SendMessage($"read MSI layers from the cdf file!")

            Dim readRawPack As ReadRawPack = MSImagingReader.UnifyReadAsMzPack(filepath)

            MSI = New Drawer(readRawPack)
            metadata = New Metadata With {
                .resolution = readRawPack.resolution,
                .scan_x = readRawPack.dimension.Width,
                .scan_y = readRawPack.dimension.Height,
                .mass_range = New DoubleRange(
                    vector:=readRawPack.GetScans.Select(Function(scan) scan.mz).IteratesALL
                )
            }
            type = FileApplicationClass.MSImaging
        ElseIf filepath.ExtensionSuffix("mzImage") Then
            Call RunSlavePipeline.SendMessage($"read MSI image file!")
            type = FileApplicationClass.MSImaging
            Throw New NotImplementedException
        ElseIf filepath.ExtensionSuffix("imzml") Then
            Dim mzpack As mzPack = MSImagingReader.UnifyReadAsMzPack(filepath)
            MSI = New Drawer(mzpack)
            metadata = mzpack.GetMSIMetadata
            type = FileApplicationClass.MSImaging
        ElseIf filepath.ExtensionSuffix("h5") Then
            Dim mzpack As mzPack = MSImagingReader.ReadmsiPLData(filepath)
            MSI = New Drawer(mzpack)
            metadata = mzpack.GetMSIMetadata
            type = FileApplicationClass.MSImaging
        Else
            Call RunSlavePipeline.SendMessage($"read MSI dataset from the mzPack raw data file!")
            Call LoadMSIMzPackCommon(MSImagingReader.UnifyReadAsMzPack(filepath))
        End If

        info = MSIProtocols.GetMSIInfo(Me)
        info!source = sourceName

        Return New DataPipe(info.GetJson(indent:=False, simpleDict:=True))
    End Function

    <Protocol(ServiceProtocol.AutoLocation)>
    Public Function AutoLocation(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim padStr As String = request.GetUTF8String
        Dim padVal As Padding = Padding.TryParse(padStr, "padding: 25px 25px 25px 25px;")
        Dim pack As mzPack = ExportMzPack().Reset(padVal)
        Call LoadMSIMzPackCommon(pack)
        Dim info = MSIProtocols.GetMSIInfo(Me)
        info!source = sourceName
        Return New DataPipe(info.GetJson(indent:=False, simpleDict:=True))
    End Function

    Private Sub LoadMSIMzPackCommon(mzpack As mzPack)
        sourceName = mzpack.source
        metadata = mzpack.GetMSIMetadata
        ion_annotations = mzpack.Annotations
        type = mzpack.Application

        If Not ion_annotations.IsNullOrEmpty Then
            For Each layer In ion_annotations
                map_to_ion(layer.Value) = Val(layer.Key)
            Next
        End If

        If Not mzpack.source.ExtensionSuffix("csv") Then
            Call RunSlavePipeline.SendMessage("make bugs fixed for RT pixel correction!")

            ' skip for bruker data
            mzpack = mzpack.ScanMeltdown(
                gridSize:=10,
                println:=Nothing
            )
        End If

        Call RunSlavePipeline.SendMessage("Load MS-imaging raw data into workspace!")
        MSI = New Drawer(mzpack)
        Call RunSlavePipeline.SendMessage("Load raw data job done!")
    End Sub

    <Protocol(ServiceProtocol.ExtractMultipleSampleRegions)>
    Public Function ExtractMultipleSampleRegions(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim allPixels As PixelScan() = MSI.pixelReader.AllPixels.ToArray
        Dim samples = allPixels _
            .GroupBy(Function(p) If(p.sampleTag.StringEmpty, "sample", p.sampleTag)) _
            .ToArray
        Dim regions = samples _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a _
                                  .Select(Function(p) New Point(p.X, p.Y)) _
                                  .ToArray
                          End Function)
        Dim dims = MSI.dimension
        Dim sampleRegions As New RegionLoader With {
            .width = dims.Width,
            .height = dims.Height,
            .sample_tags = regions.Keys.ToArray,
            .regions = .sample_tags _
                .Select(Function(tag) New Polygon2D(regions(tag))) _
                .ToArray
        }
        Dim json = GetType(RegionLoader).GetJsonElement(sampleRegions, New JSONSerializerOptions)

        Using buffer = BSON.GetBuffer(json)
            Return New DataPipe(buffer)
        End Using
    End Function

    <Protocol(ServiceProtocol.DeleteRegion)>
    Public Function DeleteRegion(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim regions As RegionLoader = BSON _
            .Load(request.ChunkBuffer) _
            .CreateObject(Of RegionLoader)(decodeMetachar:=False) _
            .Reload

        Return RegionFilter(regions, flag:=False)
    End Function

    Private Function RegionFilter(regions As RegionLoader, flag As Boolean) As BufferPipe
        Dim allPixels As PixelScan() = MSI.pixelReader.AllPixels.ToArray
        Dim info As Dictionary(Of String, String)
        Dim resize_canvas As Boolean = False
        Dim minX As Integer
        Dim minY As Integer

        If regions.empty Then
            Call RunSlavePipeline.SendMessage("No region data!")
            Return New DataPipe("no region data!")
        Else
            allPixels = allPixels _
                .Where(Function(i) regions.ContainsPixel(i.X, i.Y) = flag) _
                .ToArray
        End If

        If resize_canvas AndAlso allPixels.Length > 0 Then
            minX = allPixels.Select(Function(p) p.X).Min
            minY = allPixels.Select(Function(p) p.Y).Min
        Else
            minX = 0
            minY = 0
        End If

        ' 20221208 due to the reason of keeps the image position correctly
        ' when do ms-imagin rendering
        ' so the pixel location and the dimension size no changes
        ' set offset [x, y] to zero
        ' then the image of the target sample will keeps in its original location
        MSI = New Drawer(allPixels.CreatePixelReader(offsetX:=-minX, offsetY:=-minY))

        If resize_canvas Then
            ' andalso update the dimension size in metadata
            metadata.scan_x = MSI.dimension.Width
            metadata.scan_y = MSI.dimension.Height
        End If

        info = MSIProtocols.GetMSIInfo(Me)
        info!source = "in-memory<ExtractRegionSample>"

        Return New DataPipe(info.GetJson(indent:=False, simpleDict:=True))
    End Function

    <Protocol(ServiceProtocol.ExtractRegionMs1Spectrum)>
    Public Function ExtractRegionMs1Spectrum(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim regions As RegionLoader = BSON _
           .Load(request.ChunkBuffer) _
           .CreateObject(Of RegionLoader)(decodeMetachar:=False) _
           .Reload
        Dim allPixels As PixelScan() = MSI.pixelReader.AllPixels.ToArray
        Dim targets As ms2() = allPixels _
            .AsParallel _
            .Where(Function(i) regions.ContainsPixel(i.X, i.Y)) _
            .Select(Function(i) i.GetMs) _
            .IteratesALL _
            .ToArray
        Dim ms1 As ms2() = targets.Centroid(Tolerance.DeltaMass(0.3), New RelativeIntensityCutoff(0.01))
        Dim mat As New LibraryMatrix With {
            .name = regions.sample_tags.JoinBy("; "),
            .centroid = True,
            .ms2 = ms1
        }

        Return New DataPipe(mat.GetStream)
    End Function

    <Protocol(ServiceProtocol.ExtractRegionSample)>
    Public Function ExtractRegionSample(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim regions As RegionLoader = BSON _
            .Load(request.ChunkBuffer) _
            .CreateObject(Of RegionLoader)(decodeMetachar:=False) _
            .Reload

        Return RegionFilter(regions, flag:=True)
    End Function

    <Protocol(ServiceProtocol.GetMSIInformationMetadata)>
    Public Function GetMSIInformationMetadata(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim info = MSIProtocols.GetMSIInfo(Me)

        Call RunSlavePipeline.SendMessage($"A remote client({remoteAddress.ToString}) connect to this background data service as the cloud service host!")

        If info.TryGetValue("source").StringEmpty Then
            info!source = "no-data"
        End If

        Return New DataPipe(info.GetJson(indent:=False, simpleDict:=True))
    End Function

    <Protocol(ServiceProtocol.UpsideDown)>
    Public Function TurnUpsideDown(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim dims As Size = MSI.dimension
        Dim allPixels As PixelScan() = MSI.pixelReader.AllPixels.ToArray
        Dim turns = allPixels _
            .Select(Function(p)
                        Return New mzPackPixel(p.CreateMs1, p.X, dims.Height - p.Y + 1)
                    End Function) _
            .ToArray
        Dim newpack As New ReadRawPack(turns, dims, MSI.pixelReader.resolution)
        Dim info As Dictionary(Of String, String)

        MSI = New Drawer(newpack)
        info = MSIProtocols.GetMSIInfo(Me)

        Return New DataPipe(info.GetJson(indent:=False, simpleDict:=True))
    End Function

    <Protocol(ServiceProtocol.Mirrors)>
    Public Function TurnMirrors(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim dims As Size = MSI.dimension
        Dim allPixels As PixelScan() = MSI.pixelReader.AllPixels.ToArray
        Dim turns = allPixels _
            .Select(Function(p)
                        Return New mzPackPixel(p.CreateMs1, dims.Width - p.X + 1, p.Y)
                    End Function) _
            .ToArray
        Dim newpack As New ReadRawPack(turns, dims, MSI.pixelReader.resolution)
        Dim info As Dictionary(Of String, String)

        MSI = New Drawer(newpack)
        info = MSIProtocols.GetMSIInfo(Me)

        Return New DataPipe(info.GetJson(indent:=False, simpleDict:=True))
    End Function

    <Protocol(ServiceProtocol.CutBackground)>
    Public Function CutBackground(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim allPixels As PixelScan() = MSI.pixelReader.AllPixels.ToArray
        Dim reffile As String = If(request.ChunkBuffer.Length = 1 AndAlso request.ChunkBuffer(Scan0) = 0, Nothing, request.GetUTF8String)
        Dim info As Dictionary(Of String, String)
        Dim allMz As Double()

        If reffile.StringEmpty Then
            ' auto background remove by four corners
            'Dim intensity As Double() = allPixels _
            '    .Select(Function(d) d.GetMzIonIntensity) _
            '    .IteratesALL _
            '    .ToArray
            'Dim q As Double = TrIQThreshold.TrIQThreshold(intensity, 0.7)
            'Dim cut As Double = intensity.Max * q

            'allPixels = allPixels _
            '    .Where(Function(i)
            '               Return i.GetMzIonIntensity.Max <= cut
            '           End Function) _
            '    .ToArray
            Dim w As Integer = 16
            Dim topLeft As PixelScan() = MSI.pixelReader.GetPixel(New Rectangle(0, 0, w, w)).ToArray
            Dim topRight As PixelScan() = MSI.pixelReader.GetPixel(New Rectangle(MSI.dimension.Width - w, 0, w, w)).ToArray
            Dim bottomLeft As PixelScan() = MSI.pixelReader.GetPixel(New Rectangle(0, MSI.dimension.Height - w, w, w)).ToArray
            Dim bottomRight As PixelScan() = MSI.pixelReader _
                .GetPixel(New Rectangle(MSI.dimension.Width - w, MSI.dimension.Height - w, w, w)) _
                .ToArray

            allMz = topLeft _
                .JoinIterates(topRight) _
                .JoinIterates(bottomLeft) _
                .JoinIterates(bottomRight) _
                .Select(Function(p) p.GetMs) _
                .IteratesALL _
                .ToArray _
                .Centroid(Tolerance.DeltaMass(0.1), New RelativeIntensityCutoff(0.05)) _
                .Select(Function(i) i.mz) _
                .ToArray
        ElseIf reffile.IsNumeric AndAlso Not reffile.FileExists Then
            Dim mz As Double = Val(reffile)

            allPixels = allPixels _
                .Where(Function(p) p.HasAnyMzIon) _
                .Where(Function(p)
                           Dim m As ms2 = p _
                               .GetMs _
                               .OrderByDescending(Function(mi) mi.intensity) _
                               .First

                           Return std.Abs(m.mz - mz) > 0.3
                       End Function) _
                .ToArray

            MSI = New Drawer(allPixels.CreatePixelReader(excludesMz:=Nothing))
            info = MSIProtocols.GetMSIInfo(Me)

            Return New DataPipe(info.GetJson(indent:=False, simpleDict:=True))
        Else
            ' by a reference file, clean up background mz
            Dim stream = reffile.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim refdata As mzPack = mzPack.ReadAll(stream, ignoreThumbnail:=True, skipMsn:=True, verbose:=False)

            allMz = refdata.MS _
                .Select(Function(i) i.GetMs) _
                .IteratesALL _
                .ToArray _
                .Centroid(Tolerance.DeltaMass(0.1), New RelativeIntensityCutoff(0.05)) _
                .Select(Function(i) i.mz) _
                .ToArray
        End If

        If allMz.Length > 0 Then
            MSI = New Drawer(allPixels.CreatePixelReader(excludesMz:=allMz))
        End If

        info = MSIProtocols.GetMSIInfo(Me)

        Return New DataPipe(info.GetJson(indent:=False, simpleDict:=True))
    End Function

    <Protocol(ServiceProtocol.GetIonColocalization)>
    Public Function GetIonColocalization(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim allPixels As PixelScan() = MSI.pixelReader.AllPixels.ToArray
        Dim clusters = allPixels.PCAGroups(k:=18).ToArray
        Dim buffer As New MemoryStream

        Call LabeledData.SaveLabelData(clusters, buffer)
        Call buffer.Flush()

        Return New DataPipe(buffer)
    End Function

    <Protocol(ServiceProtocol.GetIonStatList)>
    Public Function GetIonStatList(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim targetMz As Double() = request.GetUTF8String.LoadJSON(Of Double())
        Dim allPixels As PixelScan() = MSI.pixelReader.AllPixels.ToArray
        Dim ions As IonStat() = IonStat.DoStat(allPixels, mz:=targetMz, parallel:=True).ToArray
        Dim json As JsonElement = ions _
            .GetType _
            .GetJsonElement(ions, New JSONSerializerOptions With {.indent = False})

        Return New DataPipe(BSON.BSONFormat.SafeGetBuffer(json))
    End Function

    ''' <summary>
    ''' get ms data of a given pixel point
    ''' </summary>
    ''' <param name="request"></param>
    ''' <param name="remoteAddress"></param>
    ''' <returns></returns>
    <Protocol(ServiceProtocol.GetPixel)>
    Public Function GetPixel(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim xy As Integer() = request.GetIntegers
        Dim pixel As PixelScan = MSI.pixelReader.GetPixel(xy(0), xy(1))

        If pixel Is Nothing Then
            Call RunSlavePipeline.SendMessage($"Missing pixel data at [{xy(0)},{xy(1)}]!")
            Return New DataPipe(New Byte() {})
        End If

        Dim annotations As String() = Nothing

        Call RunSlavePipeline.SendMessage($"read a 2D pixel [{xy(0)},{xy(1)}]")

        If Not ion_annotations.IsNullOrEmpty Then
            Dim mz As Double() = pixel.GetMs.Select(Function(i) i.mz).ToArray

            annotations = New String(mz.Length - 1) {}

            If type = FileApplicationClass.STImaging Then
                For i As Integer = 0 To mz.Length - 1
                    annotations(i) = ion_annotations.TryGetValue(CInt(mz(i)).ToString)
                Next
            Else
                For i As Integer = 0 To mz.Length - 1
                    annotations(i) = ion_annotations.TryGetValue(mz(i).ToString("F4"))
                Next
            End If
        End If

        Return New DataPipe(New InMemoryVectorPixel(pixel, annotations).GetBuffer)
    End Function

    ''' <summary>
    ''' get ms data of a given rectangle region
    ''' </summary>
    ''' <param name="request"></param>
    ''' <param name="remoteAddress"></param>
    ''' <returns></returns>
    <Protocol(ServiceProtocol.GetPixelRectangle)>
    Public Function GetPixelRectangle(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim rect As Integer() = request.GetIntegers
        Dim pixels As InMemoryVectorPixel() = MSI.pixelReader _
            .GetPixel(rect(0), rect(1), rect(2), rect(3)) _
            .Select(Function(p)
                        Return New InMemoryVectorPixel(p)
                    End Function) _
            .ToArray

        Return New DataPipe(InMemoryVectorPixel.GetBuffer(pixels))
    End Function

    <Protocol(ServiceProtocol.ExportMzpack)>
    Public Function ExportMzPack(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Call ExportMzPack(filename:=request.GetString(Encoding.UTF8))
        Return New DataPipe(Encoding.UTF8.GetBytes("OK!"))
    End Function

    Private Function ExportMzPack() As mzPack
        Dim reader = MSI.pixelReader
        Dim pack As New mzPack With {
            .MS = DirectCast(reader, ReadRawPack) _
                .GetScans _
                .ToArray,
            .source = sourceName,
            .metadata = metadata.GetMetadata,
            .Application = FileApplicationClass.MSImaging,
            .Annotations = ion_annotations
        }

        Return pack
    End Function

    Private Sub ExportMzPack(filename As String)
        Using buffer As Stream = filename.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call ExportMzPack.Write(buffer, progress:=AddressOf RunSlavePipeline.SendMessage)
        End Using
    End Sub

    <Protocol(ServiceProtocol.UnloadMSI)>
    Public Function Unload(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        MSI.Dispose()
        MSI.Free

        Return New DataPipe(Encoding.UTF8.GetBytes("OK!"))
    End Function

    <Protocol(ServiceProtocol.ExitApp)>
    Public Function Quit(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Call socket.Dispose()
        Return New DataPipe(Encoding.UTF8.GetBytes("OK!"))
    End Function

    <Protocol(ServiceProtocol.LoadGeneLayer)>
    Public Function GetGeneLayers(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim id As String = Encoding.ASCII.GetString(request.ChunkBuffer)
        Dim mz As Double = map_to_ion.TryGetValue(id, [default]:=-1)

        If mz <= 0 Then
            Return New DataPipe(PixelData.GetBuffer({}))
        Else
            Dim layers = MSI.LoadPixels({mz}, Tolerance.DeltaMass(0.3)).ToArray
            Return New DataPipe(PixelData.GetBuffer(layers))
        End If
    End Function

    ''' <summary>
    ''' get multiple layers data of a given mz list
    ''' </summary>
    ''' <param name="request"></param>
    ''' <param name="remoteAddress"></param>
    ''' <returns></returns>
    <Protocol(ServiceProtocol.LoadMSILayers)>
    Public Function GetMSILayers(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim config As LayerLoader = BSON.Load(request.ChunkBuffer).CreateObject(Of LayerLoader)(decodeMetachar:=True)
        Dim layers As PixelData()
        Dim mzdiff As Tolerance = config.GetTolerance

        Call RunSlavePipeline.SendMessage($"configuration for load ion layers: {JsonContract.GetJson(config)}")

        For Each mzi As Double In config.mz
            Call Console.WriteLine($"{mzi}: {mzdiff(mzi, mzi + 0.001)}")
        Next

        layers = MSI.LoadPixels(config.mz, mzdiff).ToArray
        ' layers = KnnInterpolation.KnnFill(layers, MSI.dimension, dx:=3, dy:=3)
        Call RunSlavePipeline.SendMessage($"get {layers.Length} pixels from the m/z matches!")

        Return New DataPipe(PixelData.GetBuffer(layers))
    End Function

    <Protocol(ServiceProtocol.GetAnnotationNames)>
    Public Function GetAllAnnotationNames() As BufferPipe
        Dim names As String

        If map_to_ion.IsNullOrEmpty Then
            names = "{}"
        Else
            names = map_to_ion.GetJson(indent:=False, simpleDict:=True)
        End If

        Return New DataPipe(names)
    End Function

    <Protocol(ServiceProtocol.LoadSummaryLayer)>
    Public Function LoadSummaryLayer(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim summaryType As IntensitySummary = BitConverter.ToInt32(request.ChunkBuffer, Scan0)
        Dim summary As PixelScanIntensity() = MSI.pixelReader _
            .GetSummary _
            .GetLayer(summaryType) _
            .ToArray
        Dim byts As Byte() = PixelScanIntensity.GetBuffer(summary)

        Return New DataPipe(byts)
    End Function

    ''' <summary>
    ''' just for get the sample regions
    ''' </summary>
    ''' <param name="request"></param>
    ''' <param name="remoteAddress"></param>
    ''' <returns></returns>
    <Protocol(ServiceProtocol.ExtractSamplePixels)>
    Public Function ExtractSamplePixels(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim pixels As Point() = MSI.LoadPixels.Select(Function(si) New Point(si.X, si.Y)).ToArray
        Dim spots As PixelScanIntensity() = pixels _
            .Select(Function(pi) New PixelScanIntensity With {.x = pi.X, .y = pi.Y, .totalIon = NextDouble()}) _
            .ToArray
        Dim byts As Byte() = PixelScanIntensity.GetBuffer(spots)

        Return New DataPipe(byts)
    End Function

    ''' <summary>
    ''' get BPC ion set from all pixels
    ''' </summary>
    ''' <param name="request"></param>
    ''' <param name="remoteAddress"></param>
    ''' <returns></returns>
    <Protocol(ServiceProtocol.GetBasePeakMzList)>
    Public Function GetBPCIons(request As RequestStream, remoteAddress As System.Net.IPEndPoint) As BufferPipe
        Dim data As New List(Of ms2)
        Dim pointTagged As New List(Of (X!, Y!, mz As ms2))

        For Each px As PixelScan In MSI.pixelReader.AllPixels
            Dim mz As ms2 = px.GetMs _
                .OrderByDescending(Function(a) a.intensity) _
                .FirstOrDefault

            If Not mz Is Nothing Then
                Call data.Add(mz)
                Call pointTagged.Add((px.X, px.Y, mz))
            End If
        Next

        data = data.ToArray _
             .Centroid(Tolerance.PPM(20), New RelativeIntensityCutoff(0.01)) _
             .AsList

        Dim da As Tolerance = Tolerance.DeltaMass(0.1)
        Dim mzGroup = pointTagged _
            .GroupBy(Function(p) p.mz.mz, da) _
            .Select(Function(a)
                        Return (Val(a.name), a.ToArray)
                    End Function) _
            .ToArray

        Dim mzList As Double() = data _
            .OrderByDescending(Function(a)
                                   Return mzGroup _
                                       .Where(Function(i) da(i.Item1, a.mz)) _
                                       .Select(Function(p) p.ToArray) _
                                       .IteratesALL _
                                       .Count
                               End Function) _
            .Select(Function(m) m.mz) _
            .ToArray

        Return New DataPipe(mzList)
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
