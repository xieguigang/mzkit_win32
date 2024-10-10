Imports Microsoft.VisualBasic.Imaging
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Namespace ImageUtils

    Public Module TransparencySupport
        Public Function HasTransparency(img As Image) As Boolean
            Dim bitmap As Bitmap = New Bitmap(img)
            ' Not an alpha-capable color format. Note that GDI+ indexed images are alpha-capable on the palette.
            If (CType(bitmap.Flags, ImageFlags) And ImageFlags.HasAlpha) = 0 Then Return False
            ' Indexed format, and no alpha colours in the image's palette: immediate pass.
            If (bitmap.PixelFormat And PixelFormat.Indexed) <> 0 AndAlso bitmap.Palette.Entries.All(Function(c) c.A = 255) Then Return False
            ' Get the byte data 'as 32-bit ARGB'. This offers a converted version of the image data without modifying the original image.
            Dim data As BitmapData = bitmap.LockBits(New Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)
            Dim len = bitmap.Height * data.Stride
            Dim bytes = New Byte(len - 1) {}
            Marshal.Copy(data.Scan0, bytes, 0, len)
            bitmap.UnlockBits(data)
            bitmap.Dispose()
            ' Check the alpha bytes in the data. Since the data is little-endian, the actual byte order is [BB GG RR AA]
            For i = 3 To len - 1 Step 4
                If bytes(i) <> 255 Then Return True
            Next
            Return False
        End Function

        Public Function UpscaleWithAlpha(upscaled_path As String, ip As String, resolution As Double, startUpscale As Func(Of String, Boolean, String, String)) As Bitmap
            Dim alpha_path = ip & "_alpha"

            Using source_img As Image = ip.LoadImage()
                ' Convert all image's pixel to white - preserveing only alpha values
                Dim alpha_img As Bitmap = ToAlphaChannel(New Bitmap(source_img))

                alpha_img.Save(alpha_path)
                alpha_img.Dispose()
            End Using

            ' Upscale the alpha channel image (using color - as the edges looks more clean that way)
            Dim alphaUpscale_command = $"{alpha_path} {resolution} 2"
            Dim alphaUpscale_path = startUpscale(alphaUpscale_command, False, "alpha")
            Dim fullImage = MergeAlphaChannel(upscaled_path, alphaUpscale_path)

            ' CleanUp
            File.Delete(upscaled_path)
            File.Delete(alpha_path)
            File.Delete(alphaUpscale_path)

            Return fullImage

        End Function

        Private Function MergeAlphaChannel(origin_path As String, alphaChannel_path As String) As Bitmap
            ' Load images to memory and directly merge alpha value from grayscale into the full image.

            ' Loads full image and locks data to memory
            Dim fullImage_bit As Bitmap = New Bitmap(origin_path.LoadImage())
            Dim fullImage_bitData As BitmapData = fullImage_bit.LockBits(rect:=New Rectangle(0, 0, fullImage_bit.Width, fullImage_bit.Height), flags:=ImageLockMode.ReadWrite, format:=PixelFormat.Format32bppArgb)

            ' Loads alpha image and locks data to memory
            Dim alphaImage_img = Image.FromFile(alphaChannel_path)
            Dim alphaImage_bit As Bitmap = New Bitmap(alphaImage_img)
            Dim alphaImage_bitData As BitmapData = alphaImage_bit.LockBits(New Rectangle(0, 0, alphaImage_bit.Width, alphaImage_bit.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

            Dim Height = fullImage_bit.Height
            ' Change alpha value of a pixel within fullImage_bitData to the R value of the same location pixel in alphaImage_bitData
            ' note: R value in pixel within alphaImage_bitData will be 0-255 and represnt the alpha of the image.
            Dim Width = fullImage_bit.Width
                        ''' Cannot convert UnsafeStatementSyntax, CONVERSION ERROR: Conversion for UnsafeStatement not implemented, please report this issue in 'unsafe\r\n  {\r\n   for (in...' at character 4259
''' 
''' 
''' Input:
''' 
'''         unsafe
'''         {
'''             for (int y = 0; y < Height; ++y)
'''             {
'''                 byte* fullImage_row = (byte*)fullImage_bitData.Scan0 + y * fullImage_bitData.Stride;
'''                 byte* alphaImage_row = (byte*)alphaImage_bitData.Scan0 + y * alphaImage_bitData.Stride;
'''                 int columnOffset = 0;
'''                 for (int x = 0; x < Width; ++x)
'''                 {
'''                     // Change alpha value of a pixel within fullImage_bitData to the R value of the same location pixel in alphaImage_bitData
'''                     // note: R value in pixel within alphaImage_bitData will be 0-255 and represnt the alpha of the image.
'''                     fullImage_row[columnOffset + 3] = alphaImage_row[columnOffset + 0];
'''                     columnOffset += 4;
'''                 }
'''             }
'''         }
''' 
''' 
            ' Unlocks images memory
            fullImage_bit.UnlockBits(fullImage_bitData)
            alphaImage_bit.UnlockBits(alphaImage_bitData)

            ' Cleanup
            alphaImage_bit.Dispose()
            alphaImage_img.Dispose()

            Return fullImage_bit
        End Function

        Private Function ToAlphaChannel(Image As Bitmap) As Bitmap
            Dim newBitmap As Bitmap = CType(Image.Clone(), Bitmap)
            Dim data As BitmapData = newBitmap.LockBits(New Rectangle(0, 0, newBitmap.Width, newBitmap.Height), ImageLockMode.ReadWrite, newBitmap.PixelFormat)
            Dim Height = newBitmap.Height
            Dim Width = newBitmap.Width
                        ''' Cannot convert UnsafeStatementSyntax, CONVERSION ERROR: Conversion for UnsafeStatement not implemented, please report this issue in 'unsafe\r\n  {\r\n   for (in...' at character 5896
''' 
''' 
''' Input:
''' 
'''         unsafe
'''         {
'''             for (int y = 0; y < Height; ++y)
'''             {
'''                 byte* row = (byte*)data.Scan0 + y * data.Stride;
'''                 int columnOffset = 0;
'''                 for (int x = 0; x < Width; ++x)
'''                 {
'''                     row[columnOffset + 0] = 255;
'''                     row[columnOffset + 1] = 255;
'''                     row[columnOffset + 2] = 255;
'''                     columnOffset += 4;
'''                 }
'''             }
'''         }
''' 
''' 
            newBitmap.UnlockBits(data)
            Return newBitmap
        End Function

        Private Function PremultiplyAlpha(source As Byte, alpha As Byte, y As Integer) As Byte
            Return source * CSng(alpha) / Byte.MaxValue + 0.5F
        End Function

    End Module
End Namespace
