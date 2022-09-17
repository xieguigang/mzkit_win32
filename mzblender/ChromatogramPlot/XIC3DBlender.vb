Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Task

Public Class XIC3DBlender : Inherits Blender


    ReadOnly TICList As NamedCollection(Of ChromatogramTick)()

    Sub New(TICList As NamedCollection(Of ChromatogramTick)())
        Me.TICList = TICList
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Return New ScanVisual3D(scans:=TICList, angle:=60, fillCurve:=True, fillAlpha:=120, drawParallelAxis:=True, theme:=New Theme With {
            .colorSet = args.GetColorSetName,
            .gridFill = args.gridFill.ToHtmlColor,
            .padding = args.GetPadding.ToString,
            .drawLegend = args.show_legend,
            .drawLabels = args.show_tag,
            .drawGrid = args.show_grid,
            .tagCSS = New CSSFont(args.label_font).ToString
        }) With {
            .xlabel = args.xlabel,
            .ylabel = args.ylabel,
            .main = args.title
        }.Plot($"{args.width},{args.height}", ppi:=100) _
            .AsGDIImage
    End Function
End Class
