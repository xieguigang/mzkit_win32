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
    Protected name As String

    Sub New(name As String, matrix As Array)
        Me.name = name
        Me.matrix = matrix
    End Sub

    Public Function SetName(name As String) As DataMatrix
        Me.name = name
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
    Protected MustOverride Sub CreateRows(table As DataTable)

End Class
