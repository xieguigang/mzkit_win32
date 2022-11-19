Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Task

Public Class SingleIonMSIBlender : Inherits MSImagingBlender

    ReadOnly layer As SingleIonLayer
    ReadOnly intensity As Double()
    ReadOnly TIC As Image

    Public ReadOnly Property range As DoubleRange

    Sub New(layer As PixelData(), TIC As PixelScanIntensity(), params As MsImageProperty)
        Call MyBase.New(params)

        Me.layer = New SingleIonLayer With {
            .MSILayer = layer,
            .DimensionSize = New Size(
                width:=params.scan_x,
                height:=params.scan_y
            )
        }
        ' Me.TIC = SummaryMSIBlender.Rendering(TIC, Me.layer.DimensionSize, "gray", 250)
        Me.intensity = layer.Select(Function(i) i.intensity).ToArray
        Me.range = intensity.Range
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim dimensionSize As New Size(params.scan_x, params.scan_y)
        Dim pixels As PixelData() = TakePixels(layer.MSILayer)
        ' denoise_scale() > TrIQ_scale(0.8) > knn_scale() > soften_scale()
        Dim filter As RasterPipeline = New DenoiseScaler() _
            .Then(New TrIQScaler(params.TrIQ)) _
            .Then(New KNNScaler(params.knn, params.knn_qcut)) _
            .Then(New SoftenScaler())
        Dim pixelFilter As New SingleIonLayer With {.DimensionSize = layer.DimensionSize, .IonMz = -1, .MSILayer = pixels}
        'Dim cut As Double = New TrIQThreshold(params.TrIQ) With {
        '    .levels = params.mapLevels
        '}.ThresholdValue(intensity)

        If params.enableFilter Then
            ' pixelFilter = MsImaging.Drawer.ScalePixels(pixels, params.GetTolerance, cut:={0, cut})
            pixelFilter = filter(pixelFilter)
        End If

        Dim drawer As New PixelRender(heatmapRender:=False)
        Dim image As Image = drawer.RenderPixels(
            pixels:=MsImaging.Drawer.GetPixelsMatrix(pixelFilter),
            dimension:=dimensionSize,
            mapLevels:=params.mapLevels,
            colorSet:=params.colors.Description,
            scale:=params.scale
        ).AsGDIImage

        'If params.overlap_TIC AndAlso Not TIC Is Nothing Then
        '    Using g As IGraphics = New Size(image.Width, image.Height).CreateGDIDevice
        '        Call g.DrawImage(TIC, New Rectangle(New Point(0, 0), g.Size))
        '        Call g.DrawImageUnscaled(image, 0, 0)

        '        image = DirectCast(g, Graphics2D).ImageResource
        '    End Using
        'End If

        image = New HeatMap.RasterScaler(image).Scale(hqx:=params.Hqx)

        Return image
    End Function
End Class
