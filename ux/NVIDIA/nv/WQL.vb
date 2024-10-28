#Region "Microsoft.VisualBasic::52ba2b1af304d6ef02a4119e6154e8f7, mzkit\ux\NVIDIA\nv\WQL.vb"

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

    '   Total Lines: 81
    '    Code Lines: 52 (64.20%)
    ' Comment Lines: 14 (17.28%)
    '    - Xml Docs: 57.14%
    ' 
    '   Blank Lines: 15 (18.52%)
    '     File Size: 3.55 KB


    '     Class WQL
    ' 
    '         Function: DetectGraphicsAdapter, FullySupportedGraphicsAdapters, GetGraphicsAdapter, PartiallySupportedGraphicsAdapters
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Management

Namespace nv

    Friend Class WQL

        ''' <summary>
        ''' Returns the most suitable graphics adapter in the system whiles also displaying to the user what graphics adapter they have.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function DetectGraphicsAdapter() As GraphicsAdapter
            Dim graphicsAdapter As GraphicsAdapter = GetGraphicsAdapter()

            Dim graphicsAdapterName = graphicsAdapter.Name
            Program.message(graphicsAdapterName)

            Return graphicsAdapter
        End Function

        ''' <summary>
        ''' Returns the most suitable graphics adapter present in the system.
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function GetGraphicsAdapter() As GraphicsAdapter
            ' Gets every graphics adapter in use by the system.
            Dim searcher As ManagementObjectSearcher = New ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration")

            Dim graphicsAdapters As List(Of GraphicsAdapter) = New List(Of GraphicsAdapter)()

            ' Saves the name of every found graphics card in a list.
            For Each mo As ManagementObject In searcher.Get()
                For Each [property] In mo.Properties
                    If Equals([property].Name, "Description") Then graphicsAdapters.Add(New GraphicsAdapter([property].Value.ToString()))
                Next
            Next

            ' If no graphics adapters could be found...
            If graphicsAdapters.Count < 1 Then
                Program.message("For some unknown reason, your display adapter could not be found." & vbLf & vbLf & "Perhaps you have some security program blocking me access?" & vbLf & vbLf & "If you feel like this is a mistake, enable 'Force Mode'.")
                Return New GraphicsAdapter("N/A")
            End If

            ' For every graphics adapter, find its support level by looking up its name.
            For Each graphicsAdapter In graphicsAdapters

                If FullySupportedGraphicsAdapters().Any(Function(s) graphicsAdapter.Name.ToLower().Contains(s)) Then
                    graphicsAdapter.SupportLevel = SupportLevel.Full
                ElseIf PartiallySupportedGraphicsAdapters().Any(Function(s) graphicsAdapter.Name.ToLower().Contains(s)) Then
                    graphicsAdapter.SupportLevel = SupportLevel.Partial
                Else
                    graphicsAdapter.SupportLevel = SupportLevel.None
                End If
            Next

            ' Orders the graphics adapters by support level (first graphics adapter should be most suitable).
            graphicsAdapters.OrderBy(Function(o) o.SupportLevel).Reverse().ToList()

            ' Returns the most suitable graphics adapter.
            Return graphicsAdapters(0)
        End Function

        Private Shared Iterator Function FullySupportedGraphicsAdapters() As IEnumerable(Of String)
            Yield "rtx"
        End Function

        Private Shared Iterator Function PartiallySupportedGraphicsAdapters() As IEnumerable(Of String)
            Yield "1030"
            Yield "1040"
            Yield "1050"
            Yield "1060"
            Yield "1070"
            Yield "1080"
            Yield "titan"
            Yield "1640"
            Yield "1650"
            Yield "1660"
            Yield "1670"
            Yield "1680"
        End Function
    End Class
End Namespace

