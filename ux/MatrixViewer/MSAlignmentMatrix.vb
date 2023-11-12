Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

Public Class MSAlignmentMatrix : Inherits DataMatrix

    Public Sub New(name As String, matrix As Array)
        MyBase.New(name, matrix)
    End Sub

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim matrix As SSM2MatrixFragment() = Me.matrix

        For Each tick As SSM2MatrixFragment In matrix
            table.Rows.Add(tick.mz, tick.query, tick.ref, tick.da)
        Next
    End Sub

    Public Overrides Function Plot(args As Task.PlotProperty) As Imaging.Driver.GraphicsData
        Throw New NotImplementedException()
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("m/z", GetType(Double))
        Yield New NamedValue(Of Type)("intensity(query)", GetType(Double))
        Yield New NamedValue(Of Type)("intensity(target)", GetType(Double))
        Yield New NamedValue(Of Type)("tolerance", GetType(Double))
    End Function
End Class
