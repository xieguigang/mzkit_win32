Imports System.Drawing

Namespace Helpers
#Region "Delegates"
    ''' <summary>
    ''' Filter delegate.
    ''' </summary>
    ''' <param name="bitmap">Bitmap</param>
    ''' <returns>Bitmap</returns>
    Public Delegate Function Filter(ByVal bitmap As Bitmap) As Bitmap
    ''' <summary>
    ''' Filter delegate.
    ''' </summary>
    ''' <param name="bitmap">Bitmap</param>
    ''' <returns>Bitmap</returns>
    Public Delegate Function MultiFilter(ByVal bitmap As Bitmap()) As Bitmap
#End Region
End Namespace
