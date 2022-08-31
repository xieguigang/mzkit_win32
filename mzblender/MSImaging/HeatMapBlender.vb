Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Task

Public Class HeatMapBlender : Inherits MSImagingBlender

    Dim layer As PixelData()
    Dim dimension As Size

    Public ReadOnly Property dotsize As Size
        Get
            Return New Size(params.pixel_width, params.pixel_height)
        End Get
    End Property

    Public Sub New(layer As PixelData(), dimension As Size, host As MsImageProperty)
        MyBase.New(host)

        Me.layer = layer
        Me.dimension = dimension
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim blender As New PixelRender(params.colors.Description, params.mapLevels, defaultColor:=params.background)
        Dim img As Bitmap = blender.RenderRasterImage(layer, dimension, fillRect:=True)

        Return img
    End Function
End Class
