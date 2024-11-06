#Region "Microsoft.VisualBasic::34ba12661fc709fd39aed69e61a79aec, mzkit\ux\NVIDIA\ImageUtils\TransparencySupport.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 108
    '    Code Lines: 72 (66.67%)
    ' Comment Lines: 15 (13.89%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 21 (19.44%)
    '     File Size: 5.27 KB


    '     Module TransparencySupport
    ' 
    '         Function: HasTransparency, MergeAlphaChannel, PremultiplyAlpha, ToAlphaChannel, UpscaleWithAlpha
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Bitmap = System.Drawing.Bitmap
Imports Image = System.Drawing.Image
Imports PixelFormat = System.Drawing.Imaging.PixelFormat

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
            Call System.IO.File.Delete(upscaled_path)
            Call System.IO.File.Delete(alpha_path)
            Call System.IO.File.Delete(alphaUpscale_path)

            Return fullImage
        End Function

        Private Function MergeAlphaChannel(origin_path As String, alphaChannel_path As String) As Bitmap
            ' Load images to memory and directly merge alpha value from grayscale into the full image.

            ' Loads full image and locks data to memory
            Dim fullImage_bit As New Bitmap(origin_path.LoadImage())
            Dim fullImage_bitData As BitmapBuffer = BitmapBuffer.FromBitmap(fullImage_bit)

            ' Loads alpha image and locks data to memory
            Dim alphaImage_img = Image.FromFile(alphaChannel_path)
            Dim alphaImage_bit As New Bitmap(alphaImage_img)
            Dim alphaImage_bitData As BitmapBuffer = BitmapBuffer.FromBitmap(alphaImage_bit)

            Dim Height = fullImage_bit.Height
            ' Change alpha value of a pixel within fullImage_bitData to the R value of the same location pixel in alphaImage_bitData
            ' note: R value in pixel within alphaImage_bitData will be 0-255 and represnt the alpha of the image.
            Dim Width = fullImage_bit.Width

            For y As Integer = 0 To Height
                For x As Integer = 0 To Width
                    ' Change alpha value of a pixel within fullImage_bitData to the R value of the same location pixel in alphaImage_bitData
                    ' note: R value in pixel within alphaImage_bitData will be 0-255 and represnt the alpha of the image.
                    fullImage_bitData.SetAlpha(x, y, alphaImage_bitData.GetRed(x, y))
                Next
            Next

            ' Cleanup
            alphaImage_bit.Dispose()
            alphaImage_img.Dispose()

            Return fullImage_bit
        End Function

        Private Function ToAlphaChannel(Image As Bitmap) As Bitmap
            Dim newBitmap As New Bitmap(Image)
            Dim buf As BitmapBuffer = BitmapBuffer.FromBitmap(newBitmap, ImageLockMode.ReadWrite)
            Dim Height = newBitmap.Height
            Dim Width = newBitmap.Width

            For y As Integer = 0 To Height
                For x As Integer = 0 To Width
                    Call buf.SetPixel(x, y, 255, 255, 255, buf.GetAlpha(x, y))
                Next
            Next

            Call buf.Dispose()

            Return newBitmap
        End Function

        Private Function PremultiplyAlpha(source As Byte, alpha As Byte, y As Integer) As Byte
            Return source * CSng(alpha) / Byte.MaxValue + 0.5F
        End Function

    End Module
End Namespace

