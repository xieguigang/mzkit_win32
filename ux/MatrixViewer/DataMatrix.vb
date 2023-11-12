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

    Public MustOverride Function Plot(args As PlotProperty) As GraphicsData

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
