Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv

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

    Public Overrides Function Plot(args As Task.PlotProperty) As Imaging.Driver.GraphicsData
        Throw New NotImplementedException()
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
