Imports System.Text
Imports Microsoft.VisualBasic.Parallel

Public Module MSIRequest

    Public Function LoadMSIRawdata(raw As String) As RequestStream
        Return New RequestStream(Global.ServiceHub.MSI.Protocol, ServiceProtocol.LoadMSI, Encoding.UTF8.GetBytes(raw))
    End Function
End Module
