#Region "Microsoft.VisualBasic::c6e8955a151bcc99c9b8555ffea3954b, G:/mzkit/src/mzkit/ux/MatrixViewer//ExpressionMatrix.vb"

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
    '    Code Lines: 40
    ' Comment Lines: 0
    '   Blank Lines: 12
    '     File Size: 1.59 KB


    ' Class ExpressionMatrix
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
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Task

Public Class ExpressionMatrix : Inherits DataMatrix

    ReadOnly n As Integer
    ReadOnly colname As String()

    Public Sub New(name As String, n As Integer, matrix As Dictionary(Of String, Double()))
        MyBase.New(name, matrix.ToArray)

        Me.n = n
        Me.colname = matrix.Keys.ToArray
    End Sub

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(KeyValuePair(Of String, Double()))
        End Get
    End Property

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim expr = GetMatrix(Of KeyValuePair(Of String, Double()))().ToDictionary

        For i As Integer = 0 To n - 1
            Dim v As Object() = New Object(colname.Length - 1) {}

            For j As Integer = 0 To colname.Length - 1
                v(j) = expr(colname(j))(i)
            Next

            Call table.Rows.Add(v)
        Next
    End Sub

    Public Overrides Function Plot(args As PlotProperty, picBox As Size) As GraphicsData
        Return Nothing
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        For Each key As String In colname
            Yield New NamedValue(Of Type)(key, GetType(Double))
        Next
    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Return False
    End Function
End Class
