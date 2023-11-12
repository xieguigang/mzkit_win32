Imports BioNovoGene.Analytical.MassSpectrometry.Math.UV
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Task

Public Class PDAMatrix : Inherits DataMatrix

    Public Sub New(name As String, matrix As Array)
        MyBase.New(name, matrix)
    End Sub

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim PDA As PDAPoint() = matrix

        Dim max As Double = PDA.Select(Function(a) a.total_ion).Max

        For Each tick As PDAPoint In PDA
            Call table.Rows.Add(
                tick.scan_time,
                tick.total_ion,
                tick.total_ion / max * 100
            )
        Next
    End Sub

    Public Overrides Function Plot(args As PlotProperty) As GraphicsData
        Throw New NotImplementedException
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("scan_time", GetType(Double))
        Yield New NamedValue(Of Type)("total_ion", GetType(Double))
        Yield New NamedValue(Of Type)("relative", GetType(Double))
    End Function
End Class
