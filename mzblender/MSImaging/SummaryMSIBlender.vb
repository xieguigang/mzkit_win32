Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports Microsoft.VisualBasic.Imaging
Imports Task

Public Class SummaryMSIBlender : Inherits Blender

    ReadOnly summaryLayer As PixelScanIntensity()
    ReadOnly params As MsImageProperty

    Public ReadOnly Property dotSize As New Size(2, 2)

    Sub New(layer As PixelScanIntensity(), params As MsImageProperty)
        Me.params = params
        Me.summaryLayer = layer.KnnFill(6, 6).ToArray
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim mapLevels As Integer = params.mapLevels
        Dim dimSize As New Size(params.scan_x, params.scan_y)
        Dim image As Image = Drawer.RenderSummaryLayer(
            layer:=summaryLayer,
            dimension:=dimSize,
            colorSet:=params.colors.Description,
            pixelSize:=$"{dotSize.Width},{dotSize.Height}",
            mapLevels:=mapLevels
        ).AsGDIImage

        Return image
    End Function
End Class
