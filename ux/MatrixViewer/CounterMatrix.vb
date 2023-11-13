Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Contour
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Task

Public Class CounterMatrix : Inherits DataMatrix

    Public Sub New(name As String, raw As MZWork.raw)
        MyBase.New(name, raw.GetContourData)
    End Sub

    Public Overrides ReadOnly Property UnderlyingType As Type
        Get
            Return GetType(ContourLayer)
        End Get
    End Property

    Protected Overrides Sub CreateRows(table As DataTable)

    End Sub

    Public Overrides Function Plot(args As PlotProperty, picBox As Size) As GraphicsData
        Return GetMatrix(Of ContourLayer).Plot(
            size:=$"{args.width},{args.height}",
            padding:=args.GetPadding.ToString,
            colorSet:=args.GetColorSetName,
            ppi:=200,
            legendTitle:=args.legend_title
        )
    End Function

    Protected Overrides Iterator Function GetTitles() As IEnumerable(Of NamedValue(Of Type))

    End Function

    Protected Overrides Function SaveTo(s As Stream) As Boolean
        Return False
    End Function
End Class
