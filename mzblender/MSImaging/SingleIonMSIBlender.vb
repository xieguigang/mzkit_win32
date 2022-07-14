Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Task

Public Class SingleIonMSIBlender : Inherits Blender

    ReadOnly layer As SingleIonLayer
    ReadOnly params As MsImageProperty
    ReadOnly intensity As Double()

    Public ReadOnly Property range As DoubleRange

    Sub New(layer As PixelData(), params As MsImageProperty)
        Me.layer = New SingleIonLayer With {.MSILayer = layer}
        Me.params = params
        Me.intensity = layer.Select(Function(i) i.intensity).ToArray
        Me.range = intensity.Range
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim dimensionSize As New Size(params.scan_x, params.scan_y)
        Dim size As String = "2,2"
        Dim pixels As PixelData() = layer.MSILayer
        Dim pixelFilter As PixelData() = pixels
        Dim cut As Double = New TrIQThreshold(params.TrIQ) With {
            .levels = params.mapLevels
        }.ThresholdValue(intensity)

        ' pixelFilter = MsImaging.Drawer.ScalePixels(pixels, params.GetTolerance, cut:={0, cut})
        pixelFilter = MsImaging.Drawer.GetPixelsMatrix(pixelFilter)

        Dim drawer As New PixelRender(heatmapRender:=False)
        Dim image As Image = drawer.RenderPixels(
            pixels:=pixelFilter,
            dimension:=dimensionSize,
            dimSize:=size.SizeParser,
            mapLevels:=params.mapLevels,
            colorSet:=params.colors.Description,
            scale:=params.scale,
            cutoff:={0, cut}
        ).AsGDIImage

        Return image
    End Function
End Class
