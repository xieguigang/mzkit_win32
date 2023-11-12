Imports BioNovoGene.Analytical.MassSpectrometry.Math.UV
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class UVScanMatrix : Inherits DataMatrix

    Public Sub New(name As String, matrix As Array)
        MyBase.New(name, matrix)
    End Sub

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim UVscan As UVScanPoint() = matrix
        Dim max As Double = UVscan.Select(Function(a) a.intensity).Max

        For Each tick As UVScanPoint In UVscan
            table.Rows.Add(tick.wavelength, tick.intensity, tick.intensity / max * 100)
        Next
    End Sub

    Public Overrides Function Plot(args As Task.PlotProperty) As Imaging.Driver.GraphicsData
        Throw New NotImplementedException()
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("wavelength(nm)", GetType(Double))
        Yield New NamedValue(Of Type)("intensity", GetType(Double))
        Yield New NamedValue(Of Type)("relative", GetType(Double))
    End Function
End Class
