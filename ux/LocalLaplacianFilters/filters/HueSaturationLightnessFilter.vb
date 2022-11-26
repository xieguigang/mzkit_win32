Imports System.Drawing
Imports UMapx.Imaging

Namespace Filters
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
        ''' <param name="h">Hue</param>
        ''' <param name="s">Saturation</param>
        ''' <param name="l">Lightness</param>
        Public Sub SetParams(h As Single, s As Single, l As Single)
            hsl.Hue = h
            hsl.Saturation = s
            hsl.Lightness = l
        End Sub
        ''' <summary>
        ''' Applies filter to bitmap.
        ''' </summary>
        ''' <param name="image">Bitmap</param>
        ''' <returns>Bitmap</returns>
        Public Function Apply(image As Bitmap) As Bitmap
            Dim clone As Bitmap = CType(image.Clone(), Bitmap)
            hsl.Apply(clone)
            Return clone
        End Function
#End Region
    End Class
End Namespace
