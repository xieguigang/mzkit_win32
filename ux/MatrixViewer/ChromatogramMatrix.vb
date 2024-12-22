#Region "Microsoft.VisualBasic::9dc3626df9074540a7916fda1792bc0a, mzkit\ux\MatrixViewer\ChromatogramMatrix.vb"

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

'   Total Lines: 50
'    Code Lines: 40 (80.00%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 10 (20.00%)
'     File Size: 1.71 KB


' Class ChromatogramMatrix
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
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging.Driver
Imports mzblender
Imports MZKitWin32.Blender.CommonLibs

Public Class ChromatogramMatrix : Inherits DataMatrix

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(ChromatogramTick)
        End Get
    End Property

    Public Sub New(name As String, matrix As ChromatogramTick())
        MyBase.New(name, matrix)
    End Sub

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim matrix As ChromatogramTick() = Me.matrix

        For Each tick As ChromatogramTick In matrix
            table.Rows.Add(tick.Time, tick.Intensity)
        Next
    End Sub

    Public Overrides Function Plot(args As PlotProperty, picBox As Size) As GraphicsData
        Dim blender As New ChromatogramBlender(name, GetMatrix(Of ChromatogramTick))
        Dim img = blender.Rendering(args, picBox)
        Dim raster As New ImageData(img)

        Return raster
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("time", GetType(Double))
        Yield New NamedValue(Of Type)("intensity", GetType(Double))
    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Return False
    End Function

    Public Overrides Function SaveTo(filepath As String) As Boolean
        Return GetMatrix(Of ChromatogramTick).SaveTo(filepath)
    End Function
End Class
