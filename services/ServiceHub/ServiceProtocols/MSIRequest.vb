#Region "Microsoft.VisualBasic::98047fa3ef4f4007147a2a3a62ea856b, mzkit\services\ServiceHub\ServiceProtocols\MSIRequest.vb"

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

    '   Total Lines: 33
    '    Code Lines: 25
    ' Comment Lines: 0
    '   Blank Lines: 8
    '     File Size: 1.28 KB


    ' Module MSIRequest
    ' 
    '     Function: IonStats, LoadMSIRawdata, LoadSummaryLayer
    ' 
    ' Class IonStatsParameter
    ' 
    '     Properties: da, mz
    ' 
    ' /********************************************************************************/

#End Region

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
