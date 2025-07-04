#Region "Microsoft.VisualBasic::987bbd5d402686f3ae1e5439f73d5d28, mzkit\ux\MatrixViewer\ChromatogramMatrix.vb"

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

    '   Total Lines: 99
    '    Code Lines: 80 (80.81%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 19 (19.19%)
    '     File Size: 3.55 KB


    ' Class ChromatogramMatrix
    ' 
    '     Properties: peaksOverlaps, UnderlyingType
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetTitles, Plot, (+2 Overloads) SaveTo
    ' 
    '     Sub: CreateRows, SetAbsoluteTimeAxis
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
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

    Public Property peaksOverlaps As NamedValue(Of DoubleRange)()

    Public Sub SetAbsoluteTimeAxis(maxrt As Double)
        Dim matrix As ChromatogramTick() = Me.matrix
        Dim t As Double() = matrix.TimeArray

        If t.Any Then
            Dim range As New DoubleRange(t)

            If range.Min > 0 Then
                matrix = matrix.JoinIterates({New ChromatogramTick(0, 0), New ChromatogramTick(range.Min, 0)}).ToArray
            End If
            If range.Max < maxrt Then
                matrix = matrix.JoinIterates({New ChromatogramTick(range.Max, 0), New ChromatogramTick(maxrt, 0)}).ToArray
            End If

            Me.matrix = matrix.OrderBy(Function(a) a.Time).ToArray
        Else
            Me.matrix = New ChromatogramTick() {
                New ChromatogramTick(0, 0),
                New ChromatogramTick(maxrt, 0)
            }
        End If
    End Sub

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim matrix As ChromatogramTick() = Me.matrix

        For Each tick As ChromatogramTick In matrix
            table.Rows.Add(tick.Time, tick.Intensity)
        Next
    End Sub

    Public Overrides Function Plot(args As PlotProperty, picBox As Size) As GraphicsData
        Dim blender As ChromatogramBlender

        If peaksOverlaps.IsNullOrEmpty Then
            blender = New ChromatogramBlender(name, GetMatrix(Of ChromatogramTick))
        Else
            Dim tic = GetMatrix(Of ChromatogramTick)()
            Dim overlaps As New List(Of NamedCollection(Of ChromatogramTick)) From {
                New NamedCollection(Of ChromatogramTick)(name, tic)
            }

            For Each peak As NamedValue(Of DoubleRange) In peaksOverlaps
                Call overlaps.Add(New NamedCollection(Of ChromatogramTick)(
                    peak.Name,
                    From t As ChromatogramTick
                    In tic
                    Where peak.Value.IsInside(t.Time)
                    Order By t.Time
                ))
            Next

            blender = New ChromatogramBlender(overlaps)
        End If

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
