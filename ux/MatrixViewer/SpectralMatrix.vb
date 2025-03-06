#Region "Microsoft.VisualBasic::c41edcdccca0f9907722babb9f97c7e3, mzkit\ux\MatrixViewer\SpectralMatrix.vb"

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

    '   Total Lines: 98
    '    Code Lines: 84 (85.71%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 14 (14.29%)
    '     File Size: 3.57 KB


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
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports MZKitWin32.Blender.CommonLibs

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
        Dim scale As Double = 2.5
        Dim w, h As Double
        Dim ppi As Double = 200

        If ResizeByCanvas Then
            w = picBox.Width
            h = picBox.Height
        Else
            w = args.width * scale
            h = args.height * scale
        End If

        If h < 800 Then
            w = w * (1200 / h)
            h = 1200
            ppi = 300
        End If

        Return PeakAssign.DrawSpectrumPeaks(
            scanData,
            padding:=args.GetPadding.ToString,
            bg:=args.background.ToHtmlColor,
            size:=$"{w},{h}",
            labelIntensity:=If(args.show_tag, 0.25, 100),
            gridFill:=args.gridFill.ToHtmlColor,
            barStroke:=$"stroke: steelblue; stroke-width: {args.line_width}px; stroke-dash: solid;",
            dpi:=ppi,
            titleCSS:="font-style: normal; font-size: 12; font-family: " & FontFace.MicrosoftYaHei & ";",
            labelCSS:="font-style: normal; font-size: 6; font-family: " & FontFace.MicrosoftYaHei & ";",
            axisLabelCSS:="font-style: normal; font-size: 8; font-family: " & FontFace.MicrosoftYaHei & ";",
            axisTicksCSS:="font-style: normal; font-size: 8; font-family: " & FontFace.SegoeUI & ";",
            showAnnotationText:=DrawAnnotation
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
