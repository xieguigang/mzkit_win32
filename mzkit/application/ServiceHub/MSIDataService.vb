#Region "Microsoft.VisualBasic::2126d619ebad7dc3f71b7e8a472efe16, mzkit\src\mzkit\mzkit\application\ServiceHub.vb"

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

'   Total Lines: 224
'    Code Lines: 163
' Comment Lines: 19
'   Blank Lines: 42
'     File Size: 8.88 KB


' Module ServiceHub
' 
'     Properties: MSIEngineRunning
' 
'     Function: DoIonStats, (+2 Overloads) GetPixel, handleServiceRequest, LoadBasePeakMzList, (+2 Overloads) LoadMSI
'               LoadPixels, LoadSummaryLayer
' 
'     Sub: CloseMSIEngine, ExportMzpack, MSI_pipe_SetMessage, MSI_pipe_SetProgress, StartMSIService
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.mzkit_win32.My
Imports Darwinism.IPC.Networking.HTTP
Imports Darwinism.IPC.Networking.Tcp
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.Data
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON
Imports mzblender
Imports Mzkit_win32.BasicMDIForm
Imports ServiceHub
Imports Task
Imports TaskStream

Namespace ServiceHub

    ''' <summary>
    ''' service client for ms-imaging backend
    ''' </summary>
    Public Class MSIDataService : Implements MSIServicePlugin

        Dim WithEvents MSI_pipe As RunSlavePipeline

        ''' <summary>
        ''' tcp port to the MSI data services
        ''' </summary>
        Dim MSI_service As Integer = -1
        ''' <summary>
        ''' the tcp ip of the MSI data services, default value is localhost services
        ''' </summary>
        Dim server As String = "127.0.0.1"
        Dim checkOffline As Integer

        ''' <summary>
        ''' current task host
        ''' </summary>
        Public taskHost As Thread
        Public blender As BlenderClient

        Public ReadOnly Property MSIEngineRunning As Boolean
            Get
                Return MSI_service > 0
            End Get
        End Property

        Public ReadOnly Property appPort As Integer
            Get
                Return MSI_service
            End Get
        End Property

        Public ReadOnly Property endPoint As IPEndPoint
            Get
                Return New IPEndPoint(server, appPort)
            End Get
        End Property

        Public MessageCallback As Action(Of String)

        ''' <summary>
        ''' --debug --port=33361
        ''' </summary>
        Public Shared debugPort As Integer?

        Public Sub New()

        End Sub

        ''' <summary>
        ''' is the data services running on a cloud server?
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsCloudBackend As Boolean
            Get
                ' 在这里不可以根据ip是否为localhost或者127.0.0.1来判断
                ' 因为可能会出现本机上不同进程的后台任务的程序调试分析的情形
                Return taskHost Is Nothing OrElse MSI_pipe Is Nothing
            End Get
        End Property

        ''' <summary>
        ''' just create a data service object
        ''' </summary>
        ''' <param name="hostReference"></param>
        ''' <param name="ip"></param>
        ''' <param name="port"></param>
        ''' <returns></returns>
        Public Shared Function ConnectCloud(ByRef hostReference As MSIDataService, ip As String, port As Integer) As MSIDataService
            If Not hostReference Is Nothing Then
                Call hostReference.CloseMSIEngine()
            End If

            hostReference = New MSIDataService() With {
                .MessageCallback = Nothing,
                .MSI_pipe = Nothing,
                .MSI_service = port,
                .server = ip,
                .taskHost = Nothing
            }
            hostReference.checkOffline = 0

            Call MyApplication.LogText($"Connect to the MS-Imaging cloud service!({hostReference.endPoint.ToString})")
            Workbench.SetMSIServicesAppPort(appPort:=hostReference.appPort)
            MSImagingServiceModule.ServiceEngine = hostReference

            Return hostReference
        End Function

        Public Shared Function GetRscript() As String
            Return RscriptPipelineTask.GetRScript("../services/MSI-host.R")
        End Function

        ''' <summary>
        ''' this method will close the engine at first
        ''' </summary>
        Public Shared Function StartMSIService(ByRef hostReference As MSIDataService) As MSIDataService
            Dim Rscript As String = GetRscript()
            Dim mb As Double = MyApplication.buffer_size / ByteSize.MB

            If Not hostReference Is Nothing Then
                Call hostReference.CloseMSIEngine()
            End If

            Call MyApplication.LogText($"Start background services: {Rscript}")

            hostReference = New MSIDataService
            hostReference.MSI_pipe = Global.ServiceHub.Protocols.StartServer(Rscript, hostReference.MSI_service, debugPort, buf_size:=mb)

            ' hook message event handler
            AddHandler hostReference.MSI_pipe.SetMessage, AddressOf hostReference.MSI_pipe_SetMessage

            If hostReference.MSI_service <= 0 Then
                Call Workbench.Warning("MS-Imaging service can not start!")
            Else
                Call MyApplication.LogText($"MS-Imaging service started!({hostReference.MSI_service})")
            End If

            hostReference.MessageCallback = Nothing
            Workbench.SetMSIServicesAppPort(appPort:=hostReference.appPort)
            MSImagingServiceModule.ServiceEngine = hostReference
            ServiceHub.Manager.Hub.RegisterSingle(New Manager.Service With {
                 .Name = "MS-Imaging Background Data",
                 .Description = "Handling of the data processing for the large scale MS-imaging raw data",
                 .isAlive = True,
                 .Port = hostReference.MSI_service,
                 .PID = hostReference.MSI_pipe.Process.Id,
                 .StartTime = Now.ToString,
                 .Protocol = "TCP",
                 .Memory = 0,
                 .CPU = 0,
                 .CommandLine = Manager.Service.GetCommandLine(hostReference.MSI_pipe.Process)
            })

            Return hostReference
        End Function

        Public Function SetSpatial2D(angle As Double) As MsImageProperty
            Dim data As RequestStream = handleServiceRequest(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.SetSpatial2D, BitConverter.GetBytes(angle)))

            If data Is Nothing Then
                Return Nothing
            ElseIf data.IsHTTP_RFC Then
                Call Workbench.Warning(data.GetUTF8String)
                Return Nothing
            Else
                Return GetMSIInformationMetadata(data)
            End If
        End Function

        Public Function SetSpatialMapping(cdf As String) As MsImageProperty
            Dim data As RequestStream = handleServiceRequest(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.SetSpatialMapping, cdf))

            If data Is Nothing Then
                Return Nothing
            ElseIf data.IsHTTP_RFC Then
                Call Workbench.Warning(data.GetUTF8String)
                Return Nothing
            Else
                Return GetMSIInformationMetadata(data)
            End If
        End Function

        Public Function DoIonCoLocalization(mz As Double()) As EntityClusterModel()
            Dim data As RequestStream = handleServiceRequest(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.GetIonColocalization, mz.GetJson(indent:=False, simpleDict:=True)))

            If data Is Nothing Then
                Return {}
            ElseIf data.IsHTTP_RFC Then
                Call Workbench.Warning(data.GetUTF8String)
                Return {}
            Else
                Dim ions = LabeledData.LoadLabelData(New MemoryStream(data.ChunkBuffer)).ToArray
                checkOffline = 0
                Call MyApplication.LogText($"get ion stat table payload {StringFormats.Lanudry(data.ChunkBuffer.Length)}!")
                Return ions
            End If
        End Function

        Public Function getAllLayerNames() As Dictionary(Of String, Double)
            Dim data As RequestStream = handleServiceRequest(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.GetAnnotationNames, "ok"))

            If data Is Nothing Then
                Return New Dictionary(Of String, Double)
            ElseIf data.IsHTTP_RFC Then
                Call Workbench.Warning(data.GetUTF8String)
                Return New Dictionary(Of String, Double)
            Else
                Return data.GetUTF8String.LoadJSON(Of Dictionary(Of String, Double))
            End If
        End Function

        Public Function DoIonStats(mz As Double(), mzdiff As String) As IonStat()
            Dim data As RequestStream = handleServiceRequest(MSIRequest.IonStats(mz, mzdiff))

            If data Is Nothing Then
                Return {}
            ElseIf data.IsHTTP_RFC Then
                Call Workbench.Warning(data.GetUTF8String)
                Return {}
            Else
                Dim ions = BSON.Load(data).CreateObject(Of IonStat())(decodeMetachar:=True)
                checkOffline = 0
                Call MyApplication.LogText($"get ion stat table payload {StringFormats.Lanudry(data.ChunkBuffer.Length)}!")
                Return ions
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="filepath">should be a mzPack file, not working for other data format</param>
        ''' <param name="message"></param>
        Private Sub LoadMSIRawDataFile(filepath As String, message As Action(Of String)) Implements MSIServicePlugin.LoadMSIRawDataFile
            Dim dataPack = LoadMSI(filepath, message)

            Try
                ' 20230110 there is some UI bug
                ' that calling from the plugin module
                ' so, comment these code lines
                '
                'Call RibbonEvents.showMsImaging()
                'Call WindowModules.viewer.Invoke(Sub() WindowModules.viewer.LoadRender(dataPack, filepath))
                'Call MyApplication.host.Invoke(Sub() MyApplication.host.Text = $"BioNovoGene Mzkit [{WindowModules.viewer.Text} {filepath.FileName}]")
                checkOffline = 0
            Catch ex As Exception

            End Try
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetMSIInformationMetadata() As MsImageProperty
            Return New RequestStream(MSI.Protocol, ServiceProtocol.GetMSIInformationMetadata, Encoding.UTF8.GetBytes("test")) _
                .DoCall(AddressOf handleServiceRequest) _
                .DoCall(AddressOf GetMSIInformationMetadata)
        End Function

        Public Function GetMSIInformationMetadata(data As RequestStream) As MsImageProperty
            Dim output As MsImageProperty = data _
               .GetString(Encoding.UTF8) _
               .LoadJSON(Of Dictionary(Of String, String)) _
               .DoCall(Function(info)
                           Return New MsImageProperty(info)
                       End Function)
            checkOffline = 0
            Return output
        End Function

        ''' <summary>
        ''' load MS-imaging raw data file
        ''' </summary>
        ''' <param name="raw">
        ''' filepath full name of the mzpack raw data file.
        ''' </param>
        Public Function LoadMSI(raw As String, dimSize As Size, message As Action(Of String)) As MsImageProperty
            MessageCallback = message

            Dim config As String = $"{dimSize.Width},{dimSize.Height}={raw}"
            Dim data As RequestStream = handleServiceRequest(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.LoadThermoRawMSI, Encoding.UTF8.GetBytes(config)))

            If data Is Nothing Then
                checkOffline += 1
                Call Workbench.Warning("The MS-imaging data backend has been shutdown?")
                Return Nothing
            ElseIf data.IsHTTP_RFC Then
                Call Workbench.Warning(data.GetUTF8String)
                Return Nothing
            End If

            Dim output As MsImageProperty = data _
                .GetString(Encoding.UTF8) _
                .LoadJSON(Of Dictionary(Of String, String)) _
                .DoCall(Function(info)
                            Return New MsImageProperty(info)
                        End Function)

            MessageCallback = Nothing
            checkOffline = 0
            Return output
        End Function

        Public Function LoadGeneLayer(id As String) As PixelData()
            Dim getBuf As Byte() = Nothing
            Dim pixels = MSIProtocols.LoadPixels(id, getBuf, AddressOf handleServiceRequest)
            Call blender.channel.WriteBuffer(getBuf)
            Return pixels
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function LoadPixels(mz As IEnumerable(Of Double), mzErr As Tolerance) As PixelData()
            Dim getBuf As Byte() = Nothing
            Dim pixels = MSIProtocols.LoadPixels(mz, mzErr, getBuf, AddressOf handleServiceRequest)
            Call blender.channel.WriteBuffer(getBuf)
            Return pixels
        End Function

        Public Function TurnUpsideDown() As MsImageProperty
            Dim op As Byte() = BitConverter.GetBytes(ServiceProtocol.UpsideDown)
            Dim payload As New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.UpsideDown, op)
            Dim data As RequestStream = handleServiceRequest(request:=payload)
            Dim output As MsImageProperty = data _
                .GetString(Encoding.UTF8) _
                .LoadJSON(Of Dictionary(Of String, String)) _
                .DoCall(Function(info)
                            Try
                                Return New MsImageProperty(info)
                            Catch ex As Exception
                                Return App.LogException(ex)
                            End Try
                        End Function)
            checkOffline = 0
            Return output
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="reference">
        ''' if this parameter is missing, then means auto reference
        ''' </param>
        ''' <returns></returns>
        Public Function CutBackground(reference As String) As MsImageProperty
            Dim refdata As Byte() = If(reference.StringEmpty, New Byte() {0}, Encoding.UTF8.GetBytes(reference))
            Dim payload As New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.CutBackground, refdata)
            Dim data As RequestStream = handleServiceRequest(request:=payload)
            Dim output As MsImageProperty = data _
                .GetString(Encoding.UTF8) _
                .LoadJSON(Of Dictionary(Of String, String)) _
                .DoCall(Function(info)
                            Try
                                Return New MsImageProperty(info)
                            Catch ex As Exception
                                Return App.LogException(ex)
                            End Try
                        End Function)
            checkOffline = 0
            Return output
        End Function

        Public Function ExtractMultipleSampleRegions() As RegionLoader
            Dim data As RequestStream = handleServiceRequest(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.ExtractMultipleSampleRegions, "get"))

            If data Is Nothing Then
                Call Workbench.Warning($"Failure to load MS-imaging raw data sample regions...")
                Return Nothing
            ElseIf data.IsHTTP_RFC Then
                Call Workbench.Warning(data.GetUTF8String)
                Return Nothing
            End If

            ' Call data.ChunkBuffer.FlushStream(App.CurrentProcessTemp & "/debug.bson")

            Dim regions As RegionLoader = BSON _
               .Load(data.ChunkBuffer) _
               .CreateObject(Of RegionLoader)(decodeMetachar:=False) _
               .Reload
            checkOffline = 0
            Return regions
        End Function

        Public Function DeleteRegionDataPolygon(regions As Polygon2D(), dims As Size) As MsImageProperty
            Dim payload As New RegionLoader With {
                .height = dims.Height,
                .width = dims.Width,
                .regions = regions
            }
            Dim buffer = BSON.GetBuffer(GetType(RegionLoader).GetJsonElement(payload, New JSONSerializerOptions))
            Dim data As RequestStream = handleServiceRequest(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.DeleteRegion, buffer.ToArray))
            Dim str As String = data.GetString(Encoding.UTF8)

            If str.StringEmpty OrElse Not str.StartsWith("{") Then
                Call Workbench.Warning(str)
                Return Nothing
            End If

            Dim output As MsImageProperty = str _
                .LoadJSON(Of Dictionary(Of String, String)) _
                .DoCall(Function(info)
                            Return New MsImageProperty(info)
                        End Function)
            checkOffline = 0
            Return output
        End Function

        Public Function ExtractRegionMs1Spectrum(region As Polygon2D(), label As String) As LibraryMatrix
            Dim payload As New RegionLoader With {
               .height = 0,
               .width = 0,
               .regions = region,
               .sample_tags = {label}
            }
            Dim buffer = BSON.GetBuffer(GetType(RegionLoader).GetJsonElement(payload, New JSONSerializerOptions))
            Dim data As RequestStream = handleServiceRequest(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.ExtractRegionMs1Spectrum, buffer.ToArray))

            If data.IsHTTP_RFC Then
                Call MyApplication.host.showStatusMessage(data.GetUTF8String, My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return Nothing
            Else
                checkOffline = 0
                Return LibraryMatrix.ParseStream(data.ChunkBuffer)
            End If
        End Function

        Public Function ExtractRegionSample(regions As Polygon2D(), dims As Size) As MsImageProperty
            Dim payload As New RegionLoader With {
                .height = dims.Height,
                .width = dims.Width,
                .regions = regions
            }
            Dim buffer = BSON.GetBuffer(GetType(RegionLoader).GetJsonElement(payload, New JSONSerializerOptions))
            Dim data As RequestStream = handleServiceRequest(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.ExtractRegionSample, buffer.ToArray))
            Dim str As String = data.GetString(Encoding.UTF8)

            If str.StringEmpty OrElse Not str.StartsWith("{") Then
                Call MyApplication.host.showStatusMessage(str, My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return Nothing
            End If

            Dim output As MsImageProperty = str _
                .LoadJSON(Of Dictionary(Of String, String)) _
                .DoCall(Function(info)
                            Return New MsImageProperty(info)
                        End Function)
            checkOffline = 0
            Return output
        End Function

        Public Function AutoLocation(Optional padding As Padding = Nothing) As MsImageProperty
            Dim css As String = If(padding.IsEmpty, "padding: 25px 25px 25px 25px;", padding.ToString)
            Dim data As RequestStream = handleServiceRequest(
                request:=New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.AutoLocation, Encoding.UTF8.GetBytes(css)))
            Return handlePropertiesReader(data, "Auto slide sample position error!")
        End Function

        Private Function handlePropertiesReader(data As RequestStream, raw As String) As MsImageProperty
            If data Is Nothing Then
                Call Workbench.Warning($"Failure to load MS-imaging raw data file: {raw}...")
                Return Nothing
            ElseIf data.IsHTTP_RFC Then
                Call Workbench.StatusMessage(data.GetUTF8String, My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return Nothing
            End If

            Dim output As MsImageProperty = data _
                .GetString(Encoding.UTF8) _
                .LoadJSON(Of Dictionary(Of String, String)) _
                .DoCall(Function(info)
                            Return New MsImageProperty(info)
                        End Function)
            checkOffline = 0
            Return output
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="raw">
        ''' filepath full name of the mzpack raw data file.
        ''' </param>
        Public Function LoadMSI(raw As String, message As Action(Of String)) As MsImageProperty
            MessageCallback = message

            Dim data As RequestStream = handleServiceRequest(MSIRequest.LoadMSIRawdata(raw))

            MessageCallback = Nothing

            Return handlePropertiesReader(data, raw)
        End Function

        ''' <summary>
        ''' core method for handling the tcp network data protocol
        ''' </summary>
        ''' <param name="request"></param>
        ''' <returns></returns>
        Private Function handleServiceRequest(request As RequestStream, Optional min As Double = 30) As RequestStream
            Call Workbench.LogText($"start to handling tcp request: {request.ToString}")
            Call Workbench.LogText($"set tcp request timeout: {min} minutes.")

            If MSI_service <= 0 Then
                Call Workbench.Warning("MS-imaging services is not started yet!")
                Return Nothing
            Else
                Return New TcpRequest(endPoint,
                    exceptionHandler:=
                        Sub(ex)
                            Call App.LogException(ex)
                            Call Workbench.LogText("error while processing the tcp request:")
                            Call Workbench.LogText(ex.ToString)
                        End Sub) _
                    .SetTimeOut(TimeSpan.FromMinutes(min)) _
                    .SendMessage(request)
            End If
        End Function

        Public Sub ExportMzpack(savefile As String)
            Call handleServiceRequest(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.ExportMzpack, Encoding.UTF8.GetBytes(savefile)))
        End Sub

        Public Function GetPixel(x As Integer, y As Integer, w As Integer, h As Integer) As InMemoryVectorPixel()
            Dim xy As Byte() = {x, y, w, h}.Select(AddressOf BitConverter.GetBytes).IteratesALL.ToArray
            Dim output As RequestStream = handleServiceRequest(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.GetPixelRectangle, xy))

            If output Is Nothing Then
                Return Nothing
            ElseIf output.IsHTTP_RFC Then
                Call MyApplication.host.showStatusMessage(output.GetUTF8String, My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return Nothing
            Else
                Call MyApplication.LogText($"get pixel data payload {StringFormats.Lanudry(output.ChunkBuffer.Length)}!")
                checkOffline = 0
                Return InMemoryVectorPixel.ParseVector(output.ChunkBuffer).ToArray
            End If
        End Function

        Public Function GetPixel(x As Integer, y As Integer) As PixelScan
            Dim xy As Byte() = BitConverter.GetBytes(x).JoinIterates(BitConverter.GetBytes(y)).ToArray
            Dim output As RequestStream = handleServiceRequest(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.GetPixel, xy), min:=0.01)

            If output Is Nothing Then
                Return Nothing
            ElseIf HTTP_RFC.RFC_OK <> output.Protocol AndAlso output.Protocol <> 0 Then
                Call Workbench.Warning("MSI service backend panic.")
                Call MyApplication.LogText(output.GetUTF8String)

                If checkOffline < 6 Then
                    checkOffline += 1
                Else
                    MessageBox.Show("MS-Imaging data service backend is panic or offline, please load raw data file again to restart the service",
                                    "Error Service Request", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End If

                Return Nothing
            Else
                checkOffline = 0
                Return InMemoryVectorPixel.Parse(output.ChunkBuffer)
            End If
        End Function

        Public Function LoadBasePeakMzList() As Double()
            Dim data As RequestStream = handleServiceRequest(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.GetBasePeakMzList, {}))

            If data Is Nothing Then
                Return {}
            ElseIf data.IsHTTP_RFC Then
                Call Workbench.Warning(data.GetUTF8String)
                Return {}
            Else
                checkOffline = 0
                Return data.GetDoubles
            End If
        End Function

        Public Function ExtractSampleRegion(ByRef panic As Boolean) As PixelScanIntensity()
            Dim getBuf As Byte() = Nothing
            Dim pixels = handleLayer(New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.ExtractSamplePixels, "OK"), getBuf, panic)
            Call blender.channel.WriteBuffer(getBuf)
            Return pixels
        End Function

        Public Function LoadSummaryLayer(summary As IntensitySummary, ByRef panic As Boolean) As PixelScanIntensity()
            Dim getBuf As Byte() = Nothing
            Dim request As RequestStream = MSIRequest.LoadSummaryLayer(summary)
            Dim pixels = handleLayer(request, getBuf, panic)
            Call blender.channel.WriteBuffer(getBuf)
            Return pixels
        End Function

        Private Function handleLayer(req As RequestStream, ByRef getBuf As Byte(), ByRef panic As Boolean) As PixelScanIntensity()
            Dim data As RequestStream = handleServiceRequest(req)

            panic = False
            getBuf = {}

            If data Is Nothing Then
                Return {}
            ElseIf data.IsHTTP_RFC Then
                Call Workbench.Warning(data.GetUTF8String)
                panic = True
                Return {}
            Else
                getBuf = data.ChunkBuffer
                checkOffline = 0

                Return PixelScanIntensity.Parse(data.ChunkBuffer)
            End If
        End Function

        Public Sub CloseMSIEngine() Implements MSIServicePlugin.CloseEngine
            If MSI_service > 0 AndAlso Not IsCloudBackend Then
                Dim request As New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.ExitApp, Encoding.UTF8.GetBytes("shut down!"))

                Call handleServiceRequest(request)

                ' detach message event handler
                RemoveHandler MSI_pipe.SetMessage, AddressOf MSI_pipe_SetMessage

                checkOffline = 0
                MSI_pipe = Nothing
                MSI_service = -1
            End If
        End Sub

        Private Sub MSI_pipe_SetMessage(message As String) Handles MSI_pipe.SetMessage
            Call Application.DoEvents()

            If MessageCallback Is Nothing Then
                Call MyApplication.LogText(message)
            Else
                Call MessageCallback(message)
            End If
        End Sub

        Private Sub MSI_pipe_SetProgress(percentage As Integer, details As String) Handles MSI_pipe.SetProgress
            Call Application.DoEvents()

            If MessageCallback Is Nothing Then
                Call MyApplication.LogText(details)
            Else
                Call MessageCallback(details)
            End If
        End Sub

        Private Sub MSI_pipe_Finish(exitCode As Integer) Handles MSI_pipe.Finish
            If exitCode <> 0 Then
                If taskHost IsNot Nothing Then
                    Try
                        Call taskHost.Abort()
                    Catch ex As Exception

                    End Try
                End If

                MSI_pipe_SetMessage("the MS-imaging backend service panic...")
                Workbench.Warning("the MS-imaging backend service panic...")

                If MSI_pipe IsNot Nothing Then
                    ' detach message event handler
                    RemoveHandler MSI_pipe.SetMessage, AddressOf MSI_pipe_SetMessage
                End If

                checkOffline = 0
                MSI_pipe = Nothing
                MSI_service = -1
            End If
        End Sub
    End Class
End Namespace