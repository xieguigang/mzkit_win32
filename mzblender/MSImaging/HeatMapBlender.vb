Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Task

Public Class HeatMapBlender : Inherits MSImagingBlender

    Dim layer As PixelData()
    Dim dimension As Size

    Public Sub New(layer As PixelData(), dimension As Size, host As MsImageProperty)
        MyBase.New(host)

        Me.layer = layer
        Me.dimension = dimension
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim blender As New PixelRender(params.colors.Description, params.mapLevels, defaultColor:=params.background)
        Dim img As Image = blender.RenderRasterImage(layer, dimension, fillRect:=True)

        If params.showPhysicalRuler Then
            Call New Ruler(args.GetTheme).DrawOnImage(img, dimension, Color.White, params.resolution)
        End If

        Return img
    End Function
End Class
