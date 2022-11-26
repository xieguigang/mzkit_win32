Namespace Helpers

#Region "Delegates"
    ''' <summary>
    ''' Filter delegate.
    ''' </summary>
    ''' <param name="bitmap">Bitmap</param>
    ''' <returns>Bitmap</returns>
    Public Delegate Function Filter(bitmap As Bitmap) As Bitmap
    ''' <summary>
    ''' Filter delegate.
    ''' </summary>
    ''' <param name="bitmap">Bitmap</param>
    ''' <returns>Bitmap</returns>
    Public Delegate Function MultiFilter(bitmap As Bitmap()) As Bitmap
#End Region
End Namespace
