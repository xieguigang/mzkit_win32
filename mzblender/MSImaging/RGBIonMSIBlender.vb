Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Linq
Imports Task

Public Class RGBIonMSIBlender : Inherits MSImagingBlender

    ReadOnly R As PixelData(), G As PixelData(), B As PixelData()
    ReadOnly pixelSize$
    ReadOnly originalSize As Size

    Public ReadOnly Property dotSize As New Size(3, 3)
    Public ReadOnly Property dimensions As Size
        Get
            Return New Size(params.scan_x, params.scan_y)
        End Get
    End Property

    Sub New(r As PixelData(), g As PixelData(), b As PixelData(), pixel_size As String, params As MsImageProperty)
        Call MyBase.New(params)

        Dim joinX = r.JoinIterates(g).JoinIterates(b).Select(Function(i) i.x).Max
        Dim joinY = r.JoinIterates(g).JoinIterates(b).Select(Function(i) i.y).Max

        Me.originalSize = New Size(joinX, joinY)
        Me.R = r
        Me.G = g
        Me.B = b
        Me.pixelSize = pixel_size
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim drawer As New PixelRender(heatmapRender:=False)
        Dim q1 As New TrIQThreshold(params.TrIQ)
        Dim dimensionSize As New Size(params.scan_x, params.scan_y)
        Dim r = KnnInterpolation.KnnFill(TakePixels(Me.R), originalSize, params.knn, params.knn, params.knn_qcut)
        Dim g = KnnInterpolation.KnnFill(TakePixels(Me.G), originalSize, params.knn, params.knn, params.knn_qcut)
        Dim b = KnnInterpolation.KnnFill(TakePixels(Me.B), originalSize, params.knn, params.knn, params.knn_qcut)
        Dim qr As Double = q1.ThresholdValue(r.Select(Function(p) p.intensity).ToArray)
        Dim qg As Double = q1.ThresholdValue(g.Select(Function(p) p.intensity).ToArray)
        Dim qb As Double = q1.ThresholdValue(b.Select(Function(p) p.intensity).ToArray)
        Dim cutoff = (New DoubleRange(0, qr), New DoubleRange(0, qg), New DoubleRange(0, qb))
        Dim image As Image = drawer.ChannelCompositions(
            R:=r, G:=g, B:=b,
            dimension:=dimensionSize,
            dimSize:=Nothing,
            scale:=params.scale,
            cut:=cutoff,
            background:=params.background.ToHtmlColor
        ).AsGDIImage

        image = New RasterScaler(image).Scale(hqx:=HqxScales.Hqx_4x)

        Return image
    End Function
End Class
