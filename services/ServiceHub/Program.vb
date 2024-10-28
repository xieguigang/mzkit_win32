#Region "Microsoft.VisualBasic::47ac468fa977750d484a06cbab9efb5b, mzkit\services\ServiceHub\Program.vb"

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

    '   Total Lines: 98
    '    Code Lines: 60 (61.22%)
    ' Comment Lines: 21 (21.43%)
    '    - Xml Docs: 95.24%
    ' 
    '   Blank Lines: 17 (17.35%)
    '     File Size: 3.40 KB


    ' Module Program
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: getData, getMSIDimensions, getTotalIons, tcpGet
    ' 
    '     Sub: Main
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Darwinism.IPC.Networking.Tcp
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports std = System.Math

<Package("app")>
Module Program

    Sub New()
        VectorTask.n_threads = std.Max(8, App.CPUCoreNumbers)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="service"></param>
    ''' <param name="debugPort">
    ''' --debug 33361
    ''' </param>
    <ExportAPI("run")>
    Public Sub Main(Optional service As String = "MS-Imaging",
                    Optional debugPort As Integer? = Nothing,
                    Optional masetrPID As String = Nothing)

        Select Case service.ToLower
            Case "ms-imaging"
                Call New MSI(debugPort, masetrPID).Run()
            Case Else

        End Select
    End Sub

    <ExportAPI("getMSIDimensions")>
    Public Function getMSIDimensions(MSI_service As Integer, Optional host As String = "localhost") As Integer()
        Dim tcpRequest = tcpGet(host, MSI_service)
        Dim pull = MSIProtocols.GetDimensions(tcpRequest)

        Return pull
    End Function

    ''' <summary>
    ''' get MSI layer data for run MS-imaging rendering
    ''' </summary>
    ''' <param name="MSI_service">
    ''' the background services TCP port
    ''' </param>
    ''' <param name="mz"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="env"></param>
    ''' <returns>
    ''' a vector of the pixel data, each element in this vector 
    ''' that contains the [x,y] point and the corresponding 
    ''' signal intensity value.
    ''' </returns>
    <ExportAPI("getMSIData")>
    <RApiReturn(GetType(PixelData))>
    Public Function getData(MSI_service As Integer,
                            mz As Double(),
                            mzdiff As Object,
                            Optional host As String = "localhost",
                            Optional env As Environment = Nothing) As Object

        Dim mzErr = Math.getTolerance(mzdiff, env, [default]:="da:0.1")

        If mzErr Like GetType(Message) Then
            Return mzErr.TryCast(Of Message)
        End If

        Dim tcpRequest = tcpGet(host, MSI_service)
        Dim pull = MSIProtocols.LoadPixels(mz, mzErr.TryCast(Of Tolerance), Nothing, tcpRequest)

        Return pull
    End Function

    Private Function tcpGet(host As String, MSI_service As Integer) As Func(Of RequestStream, RequestStream)
        Return Function(request As RequestStream) As RequestStream
                   Return New TcpRequest(host, MSI_service).SendMessage(request)
               End Function
    End Function

    <ExportAPI("getTotalIons")>
    Public Function getTotalIons(MSI_service As Integer,
                                 Optional host As String = "localhost",
                                 Optional env As Environment = Nothing) As Object

        Dim tcpRequest = tcpGet(host, MSI_service)
        Dim pull = MSIProtocols.GetTotalIons(tcpRequest)

        Return pull
    End Function

End Module
