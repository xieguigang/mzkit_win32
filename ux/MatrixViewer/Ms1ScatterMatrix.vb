#Region "Microsoft.VisualBasic::f898537cfafdf97a91c0844c265c4a27, mzkit\ux\MatrixViewer\Ms1ScatterMatrix.vb"

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

    '   Total Lines: 47
    '    Code Lines: 37 (78.72%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 10 (21.28%)
    '     File Size: 1.51 KB


    ' Class Ms1ScatterMatrix
    ' 
    '     Properties: UnderlyingType
    ' 
    '     Constructor: (+2 Overloads) Sub New
    ' 
    '     Function: GetTitles, Plot, SaveTo
    ' 
    '     Sub: CreateRows
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports MZKitWin32.Blender.CommonLibs
Imports PipelineHost

Public Class Ms1ScatterMatrix : Inherits DataMatrix

    Public Sub New(name As String, raw As Raw)
        MyBase.New(name, matrix:=raw.GetMs1Scans.GetMs1Points)
    End Sub

    Sub New(name As String, scatter As IEnumerable(Of ms1_scan))
        MyBase.New(name, scatter.SafeQuery.ToArray)
    End Sub

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(ms1_scan)
        End Get
    End Property

    Protected Overrides Sub CreateRows(table As DataTable)

    End Sub

    Public Overrides Function Plot(args As PlotProperty, picBox As Size) As GraphicsData
        Return RawScatterPlot.Plot(
            samples:=GetMatrix(Of ms1_scan),
            rawfile:=name,
            sampleColors:=If(args.colorSet, args.colors.Description)
        )
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))

    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Return False
    End Function
End Class
