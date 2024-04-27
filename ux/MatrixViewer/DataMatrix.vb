#Region "Microsoft.VisualBasic::649f9a8ea73b7a9e10edc87e37cb2a88, G:/mzkit/src/mzkit/ux/MatrixViewer//DataMatrix.vb"

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

    '   Total Lines: 100
    '    Code Lines: 59
    ' Comment Lines: 22
    '   Blank Lines: 19
    '     File Size: 3.09 KB


    ' Class DataMatrix
    ' 
    '     Properties: name, size
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetMatrix, SaveTo, SetName
    ' 
    '     Sub: LoadMatrix
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Task

Public MustInherit Class DataMatrix

    ''' <summary>
    ''' any matrix data for display on current page
    ''' </summary>
    Protected ReadOnly matrix As Array
    ''' <summary>
    ''' the name or display title of the current matrix
    ''' </summary>
    Public ReadOnly Property name As String

    Public ReadOnly Property size As Integer
        Get
            If matrix Is Nothing Then
                Return 0
            End If

            Return matrix.Length
        End Get
    End Property

    Public MustOverride ReadOnly Property UnderlyingType As Type

    Sub New(name As String, matrix As Array)
        Me.name = name
        Me.matrix = matrix
    End Sub

    Public Function SetName(name As String) As DataMatrix
        Me._name = name
        Return Me
    End Function

    ''' <summary>
    ''' returns nothing means method not implementted
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    Public MustOverride Function Plot(args As PlotProperty, picBox As Size) As GraphicsData

    Public Sub LoadMatrix(ByRef DataGridView1 As DataGridView, ByRef BindingSource1 As BindingSource)
        Dim memoryData As New DataSet
        Dim table As DataTable = memoryData.Tables.Add("memoryData")

        Try
            Call DataGridView1.Columns.Clear()
            Call DataGridView1.Rows.Clear()
        Catch ex As Exception

        End Try

        For Each title As NamedValue(Of Type) In GetTitles()
            Call table.Columns.Add(title.Name, title.Value)
        Next

        If size > 0 Then
            Call CreateRows(table)
        End If

        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName
        DataGridView1.DataSource = BindingSource1
    End Sub

    ''' <summary>
    ''' set column title of <see cref="DataGridView"/>
    ''' </summary>
    ''' <returns></returns>
    Protected MustOverride Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
    ''' <summary>
    ''' add rows into target table
    ''' </summary>
    ''' <param name="table"></param>
    ''' <remarks>
    ''' this function ensure that the input matrix is always not empty
    ''' </remarks>
    Protected MustOverride Sub CreateRows(table As DataTable)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function GetMatrix(Of T)() As T()
        Return DirectCast(matrix, T())
    End Function

    Public Overridable Function SaveTo(filepath As String) As Boolean
        Using file As Stream = filepath.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Return SaveTo(s:=file)
        End Using
    End Function

    Protected MustOverride Function SaveTo(s As Stream) As Boolean

End Class

