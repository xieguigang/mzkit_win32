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
