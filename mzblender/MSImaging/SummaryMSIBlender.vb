Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Task

Public Class SummaryMSIBlender : Inherits Blender

    ReadOnly summaryLayer As PixelScanIntensity()
    ReadOnly params As MsImageProperty
    ReadOnly intensity As Double()

    Public ReadOnly Property dotSize As New Size(3, 3)

    Sub New(summaryLayer As PixelScanIntensity(), params As MsImageProperty)
        Me.params = params
        Me.summaryLayer = summaryLayer
        Me.intensity = summaryLayer _
            .Select(Function(p) p.totalIon) _
            .ToArray
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim mapLevels As Integer = params.mapLevels
        Dim dimSize As New Size(params.scan_x, params.scan_y)
        Dim cut As Double = New TrIQThreshold(params.TrIQ) With {
            .levels = params.mapLevels
        }.ThresholdValue(intensity)
        Dim layerData As PixelScanIntensity() = summaryLayer

        If Not params.instrument.TextEquals("Bruker") Then
            layerData = layerData _
                .KnnFill(params.knn, params.knn, params.knn_qcut) _
                .ToArray
        End If

        Dim image As Image = Drawer.RenderSummaryLayer(
            layer:=layerData,
            dimension:=dimSize,
            colorSet:=params.colors.Description,
            pixelSize:=$"{dotSize.Width},{dotSize.Height}",
            mapLevels:=mapLevels,
            cutoff:=New DoubleRange(0, cut)
        ).AsGDIImage

        Return image
    End Function
End Class
