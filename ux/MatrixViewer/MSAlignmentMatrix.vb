#Region "Microsoft.VisualBasic::4152387eb604282bea95990ec507081c, mzkit\ux\MatrixViewer\MSAlignmentMatrix.vb"

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

'   Total Lines: 82
'    Code Lines: 68 (82.93%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 14 (17.07%)
'     File Size: 2.81 KB


' Class MSAlignmentMatrix
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
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports MZKitWin32.Blender.CommonLibs

Public Class MSAlignmentMatrix : Inherits DataMatrix

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(SSM2MatrixFragment)
        End Get
    End Property

    Dim alignment As AlignmentOutput

    Public Sub New(name As String, matrix As SSM2MatrixFragment())
        MyBase.New(name, matrix)

        Dim vs = name.StringSplit("[\s_]+vs[\s_]+")

        If vs.Length <> 2 Then
            vs = {name, "reference"}
        End If

        alignment = New AlignmentOutput With {
            .alignments = matrix,
            .query = New Meta(vs(0)),
            .reference = New Meta(vs(1))
        }
    End Sub

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim matrix As SSM2MatrixFragment() = Me.matrix

        For Each tick As SSM2MatrixFragment In matrix
            table.Rows.Add(tick.mz, tick.query, tick.ref, tick.da)
        Next
    End Sub

    Public Overrides Function Plot(args As PlotProperty, pixBox As Size) As GraphicsData
        Dim x = alignment.GetAlignmentMirror
        Dim sizeVal = $"{pixBox.Width},{pixBox.Height}"

        Return MassSpectra.AlignMirrorPlot(
            x.query, x.ref,
            size:=sizeVal,
            margin:=args.GetPadding.ToString,
            bg:=args.background.ToHtmlColor,
            intoCutoff:=0.01,
            title:=name,
            labelDisplayIntensity:=0.15,
            drawLegend:=True,
            xlab:=args.xlabel,
            ylab:=args.ylabel,
            tagXFormat:="F4",
            bw:=2,
            legendLayout:="title",
            color1:=Color.DarkRed.ToHtmlColor,
            color2:=Color.DarkBlue.ToHtmlColor
        )
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("m/z", GetType(Double))
        Yield New NamedValue(Of Type)("intensity(query)", GetType(Double))
        Yield New NamedValue(Of Type)("intensity(target)", GetType(Double))
        Yield New NamedValue(Of Type)("tolerance", GetType(String))
    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Return False
    End Function

    Public Overrides Function SaveTo(filepath As String) As Boolean
        Return GetMatrix(Of SSM2MatrixFragment).SaveTo(filepath)
    End Function
End Class
