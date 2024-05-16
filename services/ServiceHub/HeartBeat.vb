#Region "Microsoft.VisualBasic::2bbc2a4761035fdc26c2fe073c11f9bf, mzkit\services\ServiceHub\HeartBeat.vb"

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

    '   Total Lines: 37
    '    Code Lines: 28
    ' Comment Lines: 0
    '   Blank Lines: 9
    '     File Size: 936 B


    ' Module HeartBeat
    ' 
    '     Sub: Listen, Register, Start
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Net.Tcp

Module HeartBeat

    Dim port As Integer
    Dim service As IDisposable
    Dim thread As New ThreadStart(AddressOf HeartBeat.Listen)

    Public Sub Register(Of T As {ITaskDriver, IDisposable})(service As T)
        HeartBeat.service = service
    End Sub

    Public Sub Start(master As Integer)
        Dim thread As New Thread(HeartBeat.thread)

        thread.Start()
        HeartBeat.port = master
    End Sub

    Private Sub Listen()
        Call Threading.Thread.Sleep(5 * 1000)

        Do While True
            Dim req = New TcpRequest(port).SendMessage("check")

            If Not req = "OK!" Then
                Exit Do
            Else
                Call Threading.Thread.Sleep(1000)
            End If
        Loop

        Call service.Dispose()
    End Sub
End Module
