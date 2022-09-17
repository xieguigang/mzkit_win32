Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Task

Public Class ChromatogramBlender : Inherits Blender

    ReadOnly TICList As NamedCollection(Of ChromatogramTick)()

    Sub New(TICList As NamedCollection(Of ChromatogramTick)())
        Me.TICList = TICList
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Return ChromatogramPlot.TICplot(
            ionData:=TICList.ToArray,
            colorsSchema:=args.GetColorSetName,
            fillCurve:=args.fill_curve,
            size:=$"{args.width},{args.height}",
            margin:=args.GetPadding.ToString,
            gridFill:=args.gridFill.ToHtmlColor,
            bg:=args.background.ToHtmlColor,
            showGrid:=args.show_grid,
            showLegends:=args.show_legend,
            showLabels:=args.show_tag,
            xlabel:=args.xlabel,
            ylabel:=args.ylabel,
            labelFontStyle:=New CSSFont(args.label_font).ToString
        ).AsGDIImage
    End Function
End Class
