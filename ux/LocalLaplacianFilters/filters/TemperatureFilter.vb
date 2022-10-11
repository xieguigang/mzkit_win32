Imports System.Drawing
Imports UMapx.Imaging

Namespace LaplacianHDR.Filters
    ''' <summary>
    ''' Defines temperature filter.
    ''' </summary>
    Public Class TemperatureFilter
#Region "Private data"
        Private temp As TemperatureCorrection
#End Region

#Region "Filter components"
        ''' <summary>
        ''' Initializes temperature filter.
        ''' </summary>
        Public Sub New()
            temp = New TemperatureCorrection()
        End Sub
        ''' <summary>
        ''' Sets filter params.
        ''' </summary>
        ''' <param name="temperature">Temperature</param>
        ''' <param name="strenght">Strength</param>
        Public Sub SetParams(ByVal temperature As Single, ByVal strenght As Single)
            temp.Temperature = temperature
            temp.Strength = strenght
        End Sub
        ''' <summary>
        ''' Applies filter to bitmap.
        ''' </summary>
        ''' <param name="image">Bitmap</param>
        ''' <returns>Bitmap</returns>
        Public Function Apply(ByVal image As Bitmap) As Bitmap
            Dim clone As Bitmap = CType(image.Clone(), Bitmap)

            If temp.Strength <> 0 Then
                temp.Apply(clone)
            End If

            Return clone
        End Function
#End Region
    End Class
End Namespace
