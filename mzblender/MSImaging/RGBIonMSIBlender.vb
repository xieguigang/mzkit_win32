Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Task

Public Class RGBIonMSIBlender : Inherits Blender

    ReadOnly R As PixelData(), G As PixelData(), B As PixelData()
    ReadOnly pixelSize$
    ReadOnly params As MsImageProperty

    Public ReadOnly Property dotSize As New Size(2, 2)

    Sub New(r As PixelData(), g As PixelData(), b As PixelData(), pixel_size As String, params As MsImageProperty)
        Me.R = r
        Me.G = g
        Me.B = b
        Me.pixelSize = pixel_size
        Me.params = params
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim drawer As New PixelRender(heatmapRender:=False)
        Dim q1 As New TrIQThreshold
        Dim threshold As IThreshold = AddressOf q1.ThresholdValue
        Dim dimensionSize As New Size(params.scan_x, params.scan_y)
        Dim qr As Double = threshold(R.Select(Function(p) p.intensity).ToArray, params.TrIQ)
        Dim qg As Double = threshold(G.Select(Function(p) p.intensity).ToArray, params.TrIQ)
        Dim qb As Double = threshold(B.Select(Function(p) p.intensity).ToArray, params.TrIQ)
        Dim cutoff = (New DoubleRange(0, qr), New DoubleRange(0, qg), New DoubleRange(0, qb))
        Dim image As Image = drawer.ChannelCompositions(
            R:=R, G:=G, B:=B,
            dimension:=dimensionSize,
            dimSize:=dotSize,
            scale:=params.scale,
            cut:=cutoff,
            background:=params.background.ToHtmlColor
        ).AsGDIImage

        Return image
    End Function
End Class
