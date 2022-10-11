Imports System.Drawing
Imports UMapx.Imaging

Namespace LaplacianHDR.Filters
    ''' <summary>
    ''' Defines flip filter.
    ''' </summary>
    Public Class FlipFilter
#Region "Private data"
        Private flip As Flip
#End Region

#Region "Filter components"
        ''' <summary>
        ''' Initializes flip filter.
        ''' </summary>
        Public Sub New()
            flip = New Flip()
        End Sub
        ''' <summary>
        ''' Sets filter params.
        ''' </summary>
        ''' <paramname="x">Flip X</param>
        ''' <paramname="y">Flip Y</param>
        Public Sub SetParams(ByVal x As Boolean, ByVal y As Boolean)
            flip.X = x
            flip.Y = y
        End Sub
        ''' <summary>
        ''' Applies filter to bitmap.
        ''' </summary>
        ''' <paramname="image">Bitmap</param>
        ''' <returns>Bitmap</returns>
        Public Function Apply(ByVal image As Bitmap) As Bitmap
            Dim clone As Bitmap = CType(image.Clone(), Bitmap)
            flip.Apply(clone)
            Return clone
        End Function
#End Region
    End Class
End Namespace
