﻿#Region "Microsoft.VisualBasic::b81a81a21caeed006728d7a528d1c73f, mzkit\ux\MatrixViewer\NMRMatrix.vb"

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

    '   Total Lines: 56
    '    Code Lines: 45 (80.36%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 11 (19.64%)
    '     File Size: 1.93 KB


    ' Class NMRMatrix
    ' 
    '     Properties: UnderlyingType
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetTitles, Plot, SaveTo
    ' 
    '     Sub: CreateRows
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging.Driver
Imports MZKitWin32.Blender.CommonLibs

Public Class NMRMatrix : Inherits DataMatrix

    Public Sub New(name As String, matrix As Array)
        MyBase.New(name, matrix)
    End Sub

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(ms2)
        End Get
    End Property

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim matrix As ms2() = Me.matrix

        For Each tick As ms2 In matrix
            Call table.Rows.Add(tick.mz, tick.intensity)
        Next
    End Sub

    Public Overrides Function Plot(args As PlotProperty, picBox As Size) As GraphicsData
        Dim theme As Theme = args.GetTheme
        Dim scanData As New LibraryMatrix With {.ms2 = matrix, .name = name}
        Dim app As New NMRSpectrum(scanData, theme) With {
            .main = args.title
        }

        Return app.Plot(New Size(args.width, args.height), dpi:=150)
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("ppm", GetType(Double))
        Yield New NamedValue(Of Type)("intensity", GetType(Double))
    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Using text As New StreamWriter(s)
            Call text.WriteLine("ppm,intensity")

            For Each point As ms2 In GetMatrix(Of ms2)()
                Call text.WriteLine($"{point.mz},{point.intensity}")
            Next
        End Using

        Return True
    End Function
End Class
