Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Parallel

Public Module MSIRequest

    Public Function LoadMSIRawdata(raw As String) As RequestStream
        Return New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.LoadMSI, Encoding.UTF8.GetBytes(raw))
    End Function

    Public Function LoadSummaryLayer(summary As IntensitySummary) As RequestStream
        Return New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.LoadSummaryLayer, BitConverter.GetBytes(CInt(summary)))
    End Function

    Public Function IonStats(mz As IEnumerable(Of Double), mzdiff As String) As RequestStream
        Dim pars As New IonStatsParameter With {
            .da = Tolerance.ParseScript(mzdiff).GetErrorDalton,
            .mz = mz.SafeQuery.ToArray
        }

        Return New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.GetIonStatList, pars.GetJson)
    End Function
End Module

Public Class IonStatsParameter

    Public Property mz As Double()
    Public Property da As Double

End Class