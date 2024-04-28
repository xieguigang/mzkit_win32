#Region "Microsoft.VisualBasic::f9d6bbd9862febf58db0ae990ca11243, E:/mzkit/src/mzkit/services/ServiceHub//Debugger.vb"

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

    '   Total Lines: 39
    '    Code Lines: 30
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 1.60 KB


    ' Module Debugger
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: loadSession, loadSummaryLayer
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Net
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.Net.Protocols.Reflection
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.Rsharp.Runtime.Internal.Object

<Package("debugger")>
Module Debugger

    ReadOnly debug_local As IPEndPoint = New Microsoft.VisualBasic.Net.IPEndPoint("127.0.0.1", 0)

    Sub New()
        Call FrameworkInternal.ConfigMemory(FrameworkInternal.MemoryLoads.Max)
    End Sub

    <ExportAPI("load_session")>
    Public Function loadSession(raw As String) As ProtocolHandler
        Dim app As New MSI()
        Dim caller As New ProtocolHandler(app, debug:=True)

        Call caller.HandleRequest(MSIRequest.LoadMSIRawdata(raw), debug_local)

        Return caller
    End Function

    <ExportAPI("load_summary_layer")>
    Public Function loadSummaryLayer(app As ProtocolHandler) As Object
        Dim layer1 As BufferPipe = app.HandleRequest(MSIRequest.LoadSummaryLayer(IntensitySummary.BasePeak), debug_local)
        Dim layer2 As BufferPipe = app.HandleRequest(MSIRequest.LoadSummaryLayer(IntensitySummary.Average), debug_local)
        Dim layer3 As BufferPipe = app.HandleRequest(MSIRequest.LoadSummaryLayer(IntensitySummary.Total), debug_local)
        Dim layer4 As BufferPipe = app.HandleRequest(MSIRequest.LoadSummaryLayer(IntensitySummary.Median), debug_local)

        Return New list
    End Function

End Module
