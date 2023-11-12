Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Task

Public Class NMRMatrix : Inherits DataMatrix

    Public Sub New(name As String, matrix As Array)
        MyBase.New(name, matrix)
    End Sub

    Public Overrides Function Plot(args As PlotProperty) As GraphicsData
        Dim theme As Theme = args.GetTheme
        Dim scanData As New LibraryMatrix With {.ms2 = matrix, .name = name}
        Dim app As New NMRSpectrum(scanData, theme) With {
            .main = args.title
        }

        Return app.Plot(New Size(args.width, args.height), dpi:=150)
    End Function
End Class
