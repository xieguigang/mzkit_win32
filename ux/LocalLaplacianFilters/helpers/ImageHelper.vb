Imports System
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports UMapx.Imaging

Namespace Helpers
    ''' <summary>
    ''' Image helper class.
    ''' </summary>
    Public Module ImageHelper
#Region "Static voids"
        ''' <summary>
        ''' Creates a bitmap from the file.
        ''' </summary>
        ''' <param name="filename">Filename</param>
        ''' <returns>Bitmap</returns>
        Public Function Open(ByVal filename As String) As Bitmap
            Dim bitmap As Bitmap
            Try
                ' try to open image
                Dim stream As Stream = New FileStream(filename, FileMode.Open)
                bitmap = New Bitmap(stream)
                stream.Close()
                stream.Dispose()
            Catch
                Throw New FormatException("Incorrect input image format or file.")
            End Try

            ' check image size
            If Not IsTrueSize(bitmap.Width, bitmap.Height) Then
                Throw New Exception("Input image must be equal or greater than " & minDimension & " px and equal or lower than " & maxDimension & " px.")
            End If

            Return bitmap
        End Function
        ''' <summary>
        ''' Returns bitmap array from the files.
        ''' </summary>
        ''' <param name="files">Files</param>
        ''' <returns>Bitmap array</returns>
        Public Function Open(ByVal files As String()) As Bitmap()
            Dim length = files.Length
            If length = 0 Then
                Return Nothing
            Else
                Dim data = New Bitmap(length - 1) {}

                For i = 0 To length - 1
                    data(i) = Open(files(i))
                Next

                If Not AreEqualSizes(data) Then Throw New Exception("Input images must be same size.")

                Return data
            End If
        End Function
        ''' <summary>
        ''' Saves bitmap to the file.
        ''' </summary>
        ''' <param name="bitmap">Bitmap</param>
        ''' <param name="filename">Filename</param>
        ''' <param name="format">ImageFormat</param>
        Public Sub Save(ByVal bitmap As Bitmap, ByVal filename As String, ByVal format As ImageFormat)
            Try
                ' try to save image
                Dim stream As Stream = New FileStream(filename, FileMode.OpenOrCreate)
                bitmap.Save(stream, format)
                stream.Close()
                stream.Dispose()
            Catch
                Throw New FormatException("Incorrect output image format or file")
            End Try
            Return
        End Sub
        ''' <summary>
        ''' Returns image format.
        ''' </summary>
        ''' <param name="index">Index</param>
        ''' <returns>Image format</returns>
        Public Function GetImageFormat(ByVal index As Integer) As ImageFormat
            Select Case index
                Case 1
                    Return ImageFormat.Bmp
                Case 2
                    Return ImageFormat.Jpeg
                Case 3
                    Return ImageFormat.Png
                Case 4
                    Return ImageFormat.Gif
                Case Else
                    Return ImageFormat.Tiff
            End Select
        End Function
        ''' <summary>
        ''' Returns colorspace.
        ''' </summary>
        ''' <param name="index">Index</param>
        ''' <returns>Colorspace</returns>
        Public Function GetSpace(ByVal index As Integer) As Space
            Select Case index
                Case 0
                    Return Space.YCbCr
                Case 1
                    Return Space.HSB
                Case 2
                    Return Space.HSL
                Case Else
                    Return Space.Grayscale
            End Select
        End Function
        ''' <summary>
        ''' Returns cropped bitmap.
        ''' </summary>
        ''' <param name="bitmap">Bitmap</param>
        ''' <param name="box">Box size</param>
        ''' <returns>Bitmap</returns>
        Public Function Crop(ByVal bitmap As Bitmap, ByVal box As Integer) As Bitmap
            Dim width = bitmap.Width
            Dim height = bitmap.Height
            Dim min = Math.Min(width, height)
            Dim k = min / box

            Return (New Bitmap(bitmap, width / k + 1, height / k + 1)).Clone(New Rectangle(0, 0, box, box), PixelFormat.Format32bppArgb)
        End Function
        ''' <summary>
        ''' Checks if bitmaps in array have the same sizes.
        ''' </summary>
        ''' <param name="bitmaps">Bitmap array</param>
        ''' <returns>Boolean value</returns>
        Private Function AreEqualSizes(ParamArray bitmaps As Bitmap()) As Boolean
            Dim length = bitmaps.Length
            Dim equals = False

            If length > 0 Then
                Dim size = bitmaps(0).Size
                equals = True

                For i = 1 To length - 1
                    If size <> bitmaps(i).Size Then
                        equals = False
                        Exit For
                    End If
                Next
            End If

            Return equals
        End Function
        ''' <summary>
        ''' Checks bitmap sizes.
        ''' </summary>
        ''' <param name="width">Width</param>
        ''' <param name="height">Height</param>
        ''' <returns>Bool</returns>
        Private Function IsTrueSize(ByVal width As Integer, ByVal height As Integer) As Boolean
            ' check this
            Return IsRange(width, minDimension, maxDimension) AndAlso IsRange(height, minDimension, maxDimension)
        End Function
        ''' <summary>
        ''' Checks is value in range.
        ''' </summary>
        ''' <param name="x">Value</param>
        ''' <param name="min">Min</param>
        ''' <param name="max">Max</param>
        ''' <returns>Bool</returns>
        Private Function IsRange(ByVal x As Integer, ByVal min As Integer, ByVal max As Integer) As Boolean
            Return x >= min AndAlso x <= max
        End Function
        ''' <summary>
        ''' Minimum dimension.
        ''' </summary>
        Private Const minDimension As Integer = 1e2
        ''' <summary>
        ''' Maximum dimension.
        ''' </summary>
        Private Const maxDimension As Integer = 4e3
#End Region
    End Module
End Namespace
