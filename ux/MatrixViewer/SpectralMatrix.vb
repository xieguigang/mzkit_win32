#Region "Microsoft.VisualBasic::f8ea61b8cab47ac751884f35befa480d, mzkit\ux\MatrixViewer\SpectralMatrix.vb"

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

'   Total Lines: 75
'    Code Lines: 64 (85.33%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 11 (14.67%)
'     File Size: 2.67 KB


' Class SpectralMatrix
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
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Task

Public Class SpectralMatrix : Inherits DataMatrix

    ReadOnly precursor As (mz As Double, rt As Double)
    ReadOnly source As String

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(ms2)
        End Get
    End Property

    Public Sub New(name As String, matrix As ms2(), precursor As (mz As Double, rt As Double), source As String)
        MyBase.New(name, matrix)

        Me.source = source
        Me.precursor = precursor
    End Sub

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim matrix As ms2() = Me.matrix
        Dim max = matrix.Select(Function(a) a.intensity).Max

        For Each tick As ms2 In matrix
            table.Rows.Add(
                tick.mz,
                tick.intensity,
                CInt(tick.intensity / max * 100),
                tick.Annotation
            )
        Next
    End Sub

    Public Overrides Function Plot(args As PlotProperty, picBox As Size) As GraphicsData
        Dim scanData As New LibraryMatrix With {
            .name = name,
            .ms2 = matrix,
            .parentMz = precursor.mz
        }
        Dim scale As Double = 1.5
        Dim size As String

        If ResizeByCanvas Then
            size = $"{picBox.Width},{picBox.Height}"
        Else
            size = $"{args.width * scale},{args.height * scale}"
        End If

        Return PeakAssign.DrawSpectrumPeaks(
            scanData,
            padding:=args.GetPadding.ToString,
            bg:=args.background.ToHtmlColor,
            size:=size,
            labelIntensity:=If(args.show_tag, 0.25, 100),
            gridFill:=args.gridFill.ToHtmlColor,
            barStroke:=$"stroke: steelblue; stroke-width: {args.line_width}px; stroke-dash: solid;",
            dpi:=200
        )
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("m/z", GetType(Double))
        Yield New NamedValue(Of Type)("intensity", GetType(Double))
        Yield New NamedValue(Of Type)("relative", GetType(Double))
        Yield New NamedValue(Of Type)("annotation", GetType(String))
    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Return False
    End Function

    Public Overrides Function SaveTo(filepath As String) As Boolean
        Return GetMatrix(Of ms2).SaveTo(filepath)
    End Function
End Class
