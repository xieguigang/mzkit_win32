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
