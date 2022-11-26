Imports System.Drawing
Imports UMapx.Imaging

Namespace Filters
    ''' <summary>
    ''' Defines saturation/contrast/exposure/gamma/brightess filter.
    ''' </summary>
    Public Class SaturationContrastBrightnessFilter
#Region "Private data"
        Private sc As SaturationCorrection
        Private ce As ContrastEnhancement
        Private bc As BrightnessCorrection
        Private ec As ShiftCorrection
        Private gc As GammaCorrection
#End Region

#Region "Filter components"
        ''' <summary>
        ''' Initializes saturation/contrast/exposure/gamma/brightess filter.
        ''' </summary>
        Public Sub New()
            ce = New ContrastEnhancement(0, Space.YCbCr)
            sc = New SaturationCorrection(0)
            bc = New BrightnessCorrection(0, Space.YCbCr)
            ec = New ShiftCorrection(0, Space.YCbCr)
            gc = New GammaCorrection(0, Space.YCbCr)
        End Sub
        ''' <summary>
        ''' Sets filter params.
        ''' </summary>
        ''' <param name="saturation">Saturation</param>
        ''' <param name="contrast">Contrast</param>
        ''' <param name="brightness">Brightness</param>
        ''' <param name="exposure">Exposure</param>
        ''' <param name="gamma">Gamma</param>
        ''' <param name="space">Colorspace</param>
        Public Sub SetParams(saturation As Single, contrast As Single, brightness As Single, exposure As Single, gamma As Single, space As Space)
            sc.Saturation = saturation

            ce.Contrast = contrast
            ce.Space = space

            bc.Brightness = brightness
            bc.Space = space

            ec.Offset = exposure
            ec.Space = space

            gc.Gamma = gamma
            gc.Space = space
        End Sub
        ''' <summary>
        ''' Applies filter to bitmap.
        ''' </summary>
        ''' <param name="image">Bitmap</param>
        ''' <returns>Bitmap</returns>
        Public Function Apply(image As Bitmap) As Bitmap
            Dim clone As Bitmap = CType(image.Clone(), Bitmap)

            If sc.Saturation <> 0 Then sc.Apply(clone)

            If ce.Contrast <> 0 Then ce.Apply(clone)

            If bc.Brightness <> 0 Then bc.Apply(clone)

            If ec.Offset <> 0 Then ec.Apply(clone)

            If gc.Gamma <> 1.0 Then gc.Apply(clone)

            Return clone
        End Function
#End Region
    End Class
End Namespace
