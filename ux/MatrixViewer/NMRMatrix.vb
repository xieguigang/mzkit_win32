Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Task

Public Class NMRMatrix : Inherits DataMatrix

    Public Sub New(name As String, matrix As Array)
        MyBase.New(name, matrix)
    End Sub

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(ms2)
        End Get
    End Property

    Protected Overrides Sub CreateRows(table As DataTable)
        Dim matrix As ms2() = Me.matrix

        For Each tick As ms2 In matrix
            Call table.Rows.Add(tick.mz, tick.intensity)
        Next
    End Sub

    Public Overrides Function Plot(args As PlotProperty) As GraphicsData
        Dim theme As Theme = args.GetTheme
        Dim scanData As New LibraryMatrix With {.ms2 = matrix, .name = name}
        Dim app As New NMRSpectrum(scanData, theme) With {
            .main = args.title
        }

        Return app.Plot(New Size(args.width, args.height), dpi:=150)
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))
        Yield New NamedValue(Of Type)("ppm", GetType(Double))
        Yield New NamedValue(Of Type)("intensity", GetType(Double))
    End Function

    Protected Overrides Function SaveTo(s As IO.Stream) As Boolean
        Throw New NotImplementedException()
    End Function
End Class
