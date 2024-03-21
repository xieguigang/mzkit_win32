Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports Microsoft.VisualBasic.Parallel

Public Module MSIRequest

    Public Function LoadMSIRawdata(raw As String) As RequestStream
        Return New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.LoadMSI, Encoding.UTF8.GetBytes(raw))
    End Function

    Public Function LoadSummaryLayer(summary As IntensitySummary) As RequestStream
        Return New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.LoadSummaryLayer, BitConverter.GetBytes(CInt(summary)))
    End Function
End Module
