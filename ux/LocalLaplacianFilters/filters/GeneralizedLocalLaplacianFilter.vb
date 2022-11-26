Imports System.Drawing
Imports UMapx.Imaging
Imports UMapx.Transform

Namespace Filters
    ''' <summary>
    ''' Defines generalized local Laplacian filter.
    ''' </summary>
    Public Class GeneralizedLocalLaplacianFilter
#Region "Private data"
        Private bgc As BilateralGammaCorrection
        Private llf As LocalLaplacianFilter
        Private filter As BitmapFilter
#End Region

#Region "Filter components"
        ''' <summary>
        ''' Initializes generalized local Laplacian filter.
        ''' </summary>
        Public Sub New()
            bgc = New BilateralGammaCorrection(0, Space.YCbCr)
            llf = New LocalLaplacianFilter()
            filter = New BitmapFilter(llf, Space.YCbCr)
        End Sub
        ''' <summary>
        ''' Sets filter params.
        ''' </summary>
        ''' <param name="radius">Radius</param>
        ''' <param name="lightshadows">Lights and shadows</param>
        ''' <param name="sigma">Sigma</param>
        ''' <param name="discrets">Number of samples</param>
        ''' <param name="levels">Number of levels</param>
        ''' <param name="factor">Factor</param>
        ''' <param name="space">Colorspace</param>
        Public Sub SetParams(radius As Integer, lightshadows As Single, sigma As Single, discrets As Integer, levels As Integer, factor As Single, space As Space)
            bgc.Value = lightshadows
            bgc.Space = space

            llf.Radius = radius
            llf.Sigma = sigma
            llf.N = discrets
            llf.Levels = levels
            llf.Factor = factor

            filter.Filter = llf
            filter.Space = space
        End Sub
        ''' <summary>
        ''' Applies filter to bitmap.
        ''' </summary>
        ''' <param name="image">Bitmap</param>
        ''' <returns>Bitmap</returns>
        Public Function Apply(image As Bitmap) As Bitmap
            Dim clone As Bitmap = CType(image.Clone(), Bitmap)

            If bgc.Value <> 1 Then bgc.Apply(clone)

            If llf.Factor <> 0 Then filter.Apply(clone)

            Return clone
        End Function
#End Region
    End Class
End Namespace
