Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Task

Public Class SummaryMSIBlender : Inherits MSImagingBlender

    ReadOnly summaryLayer As PixelScanIntensity()
    ReadOnly intensity As Double()

    Public ReadOnly Property dimensions As Size
        Get
            Return New Size(params.scan_x, params.scan_y)
        End Get
    End Property

    Sub New(summaryLayer As PixelScanIntensity(), params As MsImageProperty)
        Call MyBase.New(params)

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
            mapLevels:=mapLevels
        ).AsGDIImage

        image = New RasterScaler(image).Scale(hqx:=params.Hqx)

        Return image
    End Function
End Class
