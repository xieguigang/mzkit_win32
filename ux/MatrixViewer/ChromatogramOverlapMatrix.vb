#Region "Microsoft.VisualBasic::253907d9e9d7625bd10286bdec9b27cb, mzkit\ux\MatrixViewer\ChromatogramOverlapMatrix.vb"

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

'   Total Lines: 91
'    Code Lines: 70 (76.92%)
' Comment Lines: 0 (0.00%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 21 (23.08%)
'     File Size: 3.14 KB


' Class ChromatogramOverlapMatrix
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
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.SignalReader
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports mzblender
Imports MZKitWin32.Blender.CommonLibs

Public Class ChromatogramOverlapMatrix : Inherits DataMatrix

    ReadOnly spatial3D As Boolean

    Public Sub New(name As String, matrix As ChromatogramSerial(), spatial3D As Boolean)
        MyBase.New(name, matrix.FileAlignment(dt:=1).ToArray)

        Me.spatial3D = spatial3D
    End Sub

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(ChromatogramSerial)
        End Get
    End Property

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim lines = GetMatrix(Of ChromatogramSerial)()
        Dim ticks As Integer = lines(0).size
        Dim data As Object()
        Dim rt As Double() = lines(0).GetTime.ToArray

        For i As Integer = 0 To ticks - 1
            data = New Object(lines.Length) {}
            data(0) = rt(i)

            For j As Integer = 0 To lines.Length - 1
                data(j + 1) = lines(j)(i).Intensity
            Next

            Call table.Rows.Add(data)
        Next
    End Sub

    Public Overrides Function Plot(args As PlotProperty, picBox As Size) As GraphicsData
        Dim blender As Blender
        Dim tic = GetMatrix(Of ChromatogramSerial)()

        If spatial3D Then
            blender = New XIC3DBlender(tic.Select(Function(c) c.GetTuple))
        Else
            blender = New ChromatogramBlender(tic.Select(Function(c) c.GetTuple))
        End If

        Return New ImageData(blender.Rendering(args, picBox))
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("retention time", GetType(Double))

        For Each line In GetMatrix(Of ChromatogramSerial)()
            Yield New NamedValue(Of Type)(line.Name, GetType(Double))
        Next
    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Using text As New StreamWriter(s)
            Dim lines = GetMatrix(Of ChromatogramSerial)()
            Dim ticks As Integer = lines(0).size
            Dim data As String()
            Dim rt As Double() = lines(0).GetTime.ToArray

            data = {"retention time"}.JoinIterates(From line As ChromatogramSerial In lines Select line.Name).ToArray

            Call text.WriteLine(New RowObject(data).AsLine)

            For i As Integer = 0 To ticks - 1
                data = New String(lines.Length) {}
                data(0) = rt(i)

                For j As Integer = 0 To lines.Length - 1
                    data(j + 1) = lines(j)(i).Intensity
                Next

                Call text.WriteLine(data.JoinBy(","))
            Next
        End Using

        Return True
    End Function
End Class
