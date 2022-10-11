Imports System.Drawing
Imports UMapx.Imaging

Namespace LaplacianHDR.Filters
    ''' <summary>
    ''' Defines colors filter filter.
    ''' </summary>
    Public Class HueSaturationLightnessFilter
#Region "Private data"
        Private hsl As HSLFilter
#End Region

#Region "Filter components"
        ''' <summary>
        ''' Initializes colors filter.
        ''' </summary>
        Public Sub New()
            hsl = New HSLFilter(0, 0, 0)
        End Sub
        ''' <summary>
        ''' Sets filter params.
        ''' </summary>
        ''' <paramname="h">Hue</param>
        ''' <paramname="s">Saturation</param>
        ''' <paramname="l">Lightness</param>
        Public Sub SetParams(ByVal h As Single, ByVal s As Single, ByVal l As Single)
            hsl.Hue = h
            hsl.Saturation = s
            hsl.Lightness = l
        End Sub
        ''' <summary>
        ''' Applies filter to bitmap.
        ''' </summary>
        ''' <paramname="image">Bitmap</param>
        ''' <returns>Bitmap</returns>
        Public Function Apply(ByVal image As Bitmap) As Bitmap
            Dim clone As Bitmap = CType(image.Clone(), Bitmap)
            hsl.Apply(clone)
            Return clone
        End Function
#End Region
    End Class
End Namespace
