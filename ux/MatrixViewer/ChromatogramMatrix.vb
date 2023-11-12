Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class ChromatogramMatrix : Inherits DataMatrix

    Public Sub New(name As String, matrix As Array)
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
End Class
