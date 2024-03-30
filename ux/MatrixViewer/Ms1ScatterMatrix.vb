Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports PipelineHost
Imports Task

Public Class Ms1ScatterMatrix : Inherits DataMatrix

    Public Sub New(name As String, raw As Raw)
        MyBase.New(name, matrix:=raw.GetMs1Scans.GetMs1Points)
    End Sub

    Sub New(name As String, scatter As IEnumerable(Of ms1_scan))
        MyBase.New(name, scatter.SafeQuery.ToArray)
    End Sub

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(ms1_scan)
        End Get
    End Property

    Protected Overrides Sub CreateRows(table As DataTable)

    End Sub

    Public Overrides Function Plot(args As PlotProperty, picBox As Size) As GraphicsData
        Return RawScatterPlot.Plot(
            samples:=GetMatrix(Of ms1_scan),
            rawfile:=name,
            sampleColors:=args.colorSet
        )
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))

    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Return False
    End Function
End Class
