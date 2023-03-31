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
Imports System.Text
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.Tcp
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.Data
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Net
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Serialization.JSON
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

        ''' <summary>
        ''' current task host
        ''' </summary>
        Public taskHost As Thread

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

            Call MyApplication.LogText($"Connect to the MS-Imaging cloud service!({hostReference.endPoint.ToString})")
            Workbench.SetMSIServicesAppPort(appPort:=hostReference.appPort)
            MSImagingServiceModule.ServiceEngine = hostReference

            Return hostReference
        End Function

        ''' <summary>
        ''' this method will close the engine at first
        ''' </summary>
        Public Shared Function StartMSIService(ByRef hostReference As MSIDataService) As MSIDataService
            Dim Rscript As String = RscriptPipelineTask.GetRScript("../services/MSI-host.R")

            If Not hostReference Is Nothing Then
                Call hostReference.CloseMSIEngine()
            End If

            Call MyApplication.LogText($"Start background services: {Rscript}")

            hostReference = New MSIDataService
            hostReference.MSI_pipe = Global.ServiceHub.Protocols.StartServer(Rscript, hostReference.MSI_service, MSIDataService.debugPort) ', HeartBeat.Start)

            ' hook message event handler
            AddHandler hostReference.MSI_pipe.SetMessage, AddressOf hostReference.MSI_pipe_SetMessage

            If hostReference.MSI_service <= 0 Then
                Call MyApplication.host.showStatusMessage("MS-Imaging service can not start!", My.Resources.StatusAnnotations_Warning_32xLG_color)
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
                 .CPU = 0
            })

            Return hostReference
        End Function

        Public Function DoIonCoLocalization(mz As Double()) As EntityClusterModel()
            Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.GetIonColocalization, mz.GetJson(indent:=False, simpleDict:=True)))

            If data Is Nothing Then
                Return {}
            ElseIf data.IsHTTP_RFC Then
                Call MyApplication.host.showStatusMessage(data.GetUTF8String, My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return {}
            Else
                Dim ions = LabeledData.LoadLabelData(New MemoryStream(data.ChunkBuffer)).ToArray
                Call MyApplication.LogText($"get ion stat table payload {StringFormats.Lanudry(data.ChunkBuffer.Length)}!")
                Return ions
            End If
        End Function

        Public Function DoIonStats(mz As Double()) As IonStat()
            Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.GetIonStatList, mz.GetJson(indent:=False, simpleDict:=True)))

            If data Is Nothing Then
                Return {}
            ElseIf data.IsHTTP_RFC Then
                Call MyApplication.host.showStatusMessage(data.GetUTF8String, My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return {}
            Else
                Dim ions = BSON.Load(data).CreateObject(Of IonStat())(decodeMetachar:=True)
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
            Catch ex As Exception

            End Try
        End Sub

        Public Function GetMSIInformationMetadata() As MsImageProperty
            Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.GetMSIInformationMetadata, Encoding.UTF8.GetBytes("test")))
            Dim output As MsImageProperty = data _
                .GetString(Encoding.UTF8) _
                .LoadJSON(Of Dictionary(Of String, String)) _
                .DoCall(Function(info)
                            Return New MsImageProperty(info)
                        End Function)

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
            Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.LoadThermoRawMSI, Encoding.UTF8.GetBytes(config)))
            Dim output As MsImageProperty = data _
                .GetString(Encoding.UTF8) _
                .LoadJSON(Of Dictionary(Of String, String)) _
                .DoCall(Function(info)
                            Return New MsImageProperty(info)
                        End Function)

            MessageCallback = Nothing

            Return output
        End Function

        Public Function LoadPixels(mz As IEnumerable(Of Double), mzErr As Tolerance) As PixelData()
            Return MSIProtocols.LoadPixels(mz, mzErr, AddressOf handleServiceRequest)
        End Function

        Public Function TurnUpsideDown() As MsImageProperty
            Dim op As Byte() = BitConverter.GetBytes(ServiceProtocol.UpsideDown)
            Dim payload As New RequestStream(MSI.Protocol, ServiceProtocol.UpsideDown, op)
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
            Dim payload As New RequestStream(MSI.Protocol, ServiceProtocol.CutBackground, refdata)
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

            Return output
        End Function

        Public Function ExtractMultipleSampleRegions() As RegionLoader
            Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.ExtractMultipleSampleRegions, "get"))

            If data Is Nothing Then
                Call MyApplication.host.warning($"Failure to load MS-imaging raw data sample regions...")
                Return Nothing
            ElseIf data.IsHTTP_RFC Then
                Call MyApplication.host.showStatusMessage(data.GetUTF8String, My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return Nothing
            End If

            Dim regions As RegionLoader = BSON _
               .Load(data.ChunkBuffer) _
               .CreateObject(Of RegionLoader)(decodeMetachar:=False) _
               .Reload

            Return regions
        End Function

        Public Function DeleteRegionDataPolygon(regions As Polygon2D(), dims As Size) As MsImageProperty
            Dim payload As New RegionLoader With {
                .height = dims.Height,
                .width = dims.Width,
                .regions = regions
            }
            Dim buffer = BSON.GetBuffer(GetType(RegionLoader).GetJsonElement(payload, New JSONSerializerOptions))
            Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.DeleteRegion, buffer.ToArray))
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

            Return output
        End Function

        Public Function ExtractRegionSample(regions As Polygon2D(), dims As Size) As MsImageProperty
            Dim payload As New RegionLoader With {
                .height = dims.Height,
                .width = dims.Width,
                .regions = regions
            }
            Dim buffer = BSON.GetBuffer(GetType(RegionLoader).GetJsonElement(payload, New JSONSerializerOptions))
            Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.ExtractRegionSample, buffer.ToArray))
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

            Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.LoadMSI, Encoding.UTF8.GetBytes(raw)))

            MessageCallback = Nothing

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

            Return output
        End Function

        ''' <summary>
        ''' core method for handling the tcp network data protocol
        ''' </summary>
        ''' <param name="request"></param>
        ''' <returns></returns>
        Private Function handleServiceRequest(request As RequestStream) As RequestStream
            If MSI_service <= 0 Then
                Call Workbench.StatusMessage("MS-imaging services is not started yet!", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return Nothing
            Else
                Return New TcpRequest(endPoint) _
                    .SetTimeOut(TimeSpan.FromMinutes(30)) _
                    .SendMessage(request)
            End If
        End Function

        Public Sub ExportMzpack(savefile As String)
            Call handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.ExportMzpack, Encoding.UTF8.GetBytes(savefile)))
        End Sub

        Public Function GetPixel(x As Integer, y As Integer, w As Integer, h As Integer) As InMemoryVectorPixel()
            Dim xy As Byte() = {x, y, w, h}.Select(AddressOf BitConverter.GetBytes).IteratesALL.ToArray
            Dim output As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.GetPixelRectangle, xy))

            If output Is Nothing Then
                Return Nothing
            ElseIf output.IsHTTP_RFC Then
                Call MyApplication.host.showStatusMessage(output.GetUTF8String, My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return Nothing
            Else
                Call MyApplication.LogText($"get pixel data payload {StringFormats.Lanudry(output.ChunkBuffer.Length)}!")
                Return InMemoryVectorPixel.ParseVector(output.ChunkBuffer).ToArray
            End If
        End Function

        Public Function GetPixel(x As Integer, y As Integer) As PixelScan
            Dim xy As Byte() = BitConverter.GetBytes(x).JoinIterates(BitConverter.GetBytes(y)).ToArray
            Dim output As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.GetPixel, xy))

            If output Is Nothing Then
                Return Nothing
            ElseIf HTTP_RFC.RFC_OK <> output.Protocol AndAlso output.Protocol <> 0 Then
                Call MyApplication.host.showStatusMessage("MSI service backend panic.", My.Resources.StatusAnnotations_Warning_32xLG_color)
                Call MyApplication.LogText(output.GetUTF8String)
                Return Nothing
            Else
                Return InMemoryVectorPixel.Parse(output.ChunkBuffer)
            End If
        End Function

        Public Function LoadBasePeakMzList() As Double()
            Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.GetBasePeakMzList, {}))

            If data Is Nothing Then
                Return {}
            ElseIf data.IsHTTP_RFC Then
                Call MyApplication.host.showStatusMessage(data.GetUTF8String, My.Resources.StatusAnnotations_Warning_32xLG_color)
                Return {}
            Else
                Return data.GetDoubles
            End If
        End Function

        Public Function LoadSummaryLayer(summary As IntensitySummary, ByRef panic As Boolean) As PixelScanIntensity()
            Dim data As RequestStream = handleServiceRequest(New RequestStream(MSI.Protocol, ServiceProtocol.LoadSummaryLayer, BitConverter.GetBytes(CInt(summary))))

            panic = False

            If data Is Nothing Then
                Return {}
            ElseIf data.IsHTTP_RFC Then
                Call Workbench.StatusMessage(data.GetUTF8String, My.Resources.StatusAnnotations_Warning_32xLG_color)
                panic = True
                Return {}
            Else
                Return PixelScanIntensity.Parse(data.ChunkBuffer)
            End If
        End Function

        Public Sub CloseMSIEngine() Implements MSIServicePlugin.CloseEngine
            If MSI_service > 0 AndAlso Not IsCloudBackend Then
                Dim request As New RequestStream(MSI.Protocol, ServiceProtocol.ExitApp, Encoding.UTF8.GetBytes("shut down!"))

                Call handleServiceRequest(request)

                ' detach message event handler
                RemoveHandler MSI_pipe.SetMessage, AddressOf MSI_pipe_SetMessage

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
                MyApplication.host.showStatusMessage("the MS-imaging backend service panic...", My.Resources.StatusAnnotations_Warning_32xLG_color)

                If MSI_pipe IsNot Nothing Then
                    ' detach message event handler
                    RemoveHandler MSI_pipe.SetMessage, AddressOf MSI_pipe_SetMessage
                End If

                MSI_pipe = Nothing
                MSI_service = -1
            End If
        End Sub
    End Class
End Namespace