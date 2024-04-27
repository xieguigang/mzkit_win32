#Region "Microsoft.VisualBasic::3e4c510da41d28bd150a5715283ed43e, G:/mzkit/src/mzkit/ux/MatrixViewer//PDAMatrix.vb"

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

    '   Total Lines: 52
    '    Code Lines: 42
    ' Comment Lines: 0
    '   Blank Lines: 10
    '     File Size: 1.70 KB


    ' Class PDAMatrix
    ' 
    '     Properties: UnderlyingType
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetTitles, Plot, (+2 Overloads) SaveTo
    ' 
    '     Sub: CreateRows
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.UV
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Task

Public Class PDAMatrix : Inherits DataMatrix

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(PDAPoint)
        End Get
    End Property

    Public Sub New(name As String, matrix As Array)
        MyBase.New(name, matrix)
    End Sub

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim PDA As PDAPoint() = matrix

        Dim max As Double = PDA.Select(Function(a) a.total_ion).Max

        For Each tick As PDAPoint In PDA
            Call table.Rows.Add(
                tick.scan_time,
                tick.total_ion,
                tick.total_ion / max * 100
            )
        Next
    End Sub

    Public Overrides Function Plot(args As PlotProperty, picBox As Size) As GraphicsData
        Throw New NotImplementedException
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("scan_time", GetType(Double))
        Yield New NamedValue(Of Type)("total_ion", GetType(Double))
        Yield New NamedValue(Of Type)("relative", GetType(Double))
    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Return False
    End Function

    Public Overrides Function SaveTo(filepath As String) As Boolean
        Return GetMatrix(Of PDAPoint).SaveTo(filepath)
    End Function
End Class
