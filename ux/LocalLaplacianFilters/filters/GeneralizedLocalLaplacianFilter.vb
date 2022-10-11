Imports System.Drawing
Imports UMapx.Imaging
Imports UMapx.Transform

Namespace LaplacianHDR.Filters
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
        ''' <paramname="radius">Radius</param>
        ''' <paramname="lightshadows">Lights and shadows</param>
        ''' <paramname="sigma">Sigma</param>
        ''' <paramname="discrets">Number of samples</param>
        ''' <paramname="levels">Number of levels</param>
        ''' <paramname="factor">Factor</param>
        ''' <paramname="space">Colorspace</param>
        Public Sub SetParams(ByVal radius As Integer, ByVal lightshadows As Single, ByVal sigma As Single, ByVal discrets As Integer, ByVal levels As Integer, ByVal factor As Single, ByVal space As Space)
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
        ''' <paramname="image">Bitmap</param>
        ''' <returns>Bitmap</returns>
        Public Function Apply(ByVal image As Bitmap) As Bitmap
            Dim clone As Bitmap = CType(image.Clone(), Bitmap)

            If bgc.Value <> 1 Then bgc.Apply(clone)

            If llf.Factor <> 0 Then filter.Apply(clone)

            Return clone
        End Function
#End Region
    End Class
End Namespace
