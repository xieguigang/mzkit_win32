Imports System.IO
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports Path = System.IO.Path

Namespace DeepZoomBuilder

    Public Enum ImageType
        Png
        Jpeg
    End Enum

    ''' <summary>
    ''' A very quick and simple app which generates a deep zoom image from a source image.
    ''' 
    ''' > https://github.com/JimLynn/DeepZoomBuilder
    ''' </summary>
    Public Class DeepZoomCreator
        ''' <summary>
        ''' Default public constructor
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Create a deep zoom image from a single source image
        ''' </summary>
        ''' <param name="sourceImage"> Source image path</param>
        ''' <param name="destinationImage"> Destination path (must be .dzi or .xml)</param>
        Public Sub CreateSingleComposition(sourceImage As String, destinationImage As String, type As ImageType)
            imageType = type
            Dim source = sourceImage
            Dim destDirectory = Path.GetDirectoryName(destinationImage)
            Dim leafname = Path.GetFileNameWithoutExtension(destinationImage)
            Dim root = Path.Combine(destDirectory, leafname)
            Dim filesdir = root & "_files"

            Directory.CreateDirectory(filesdir)
            Dim img As BitmapImage = New BitmapImage(New Uri(source))
            Dim dWidth As Double = img.PixelWidth
            Dim dHeight As Double = img.PixelHeight
            Dim AspectRatio = dWidth / dHeight

            ' The Maximum level for the pyramid of images is
            ' Log2(maxdimension)

            Dim maxdimension = Math.Max(dWidth, dHeight)
            Dim logvalue = Math.Log(maxdimension, 2)
            Dim MaxLevel As Integer = Math.Ceiling(logvalue)
            Dim topleveldir As String = Path.Combine(filesdir, MaxLevel.ToString())

            ' Create the directory for the top level tiles
            Directory.CreateDirectory(topleveldir)

            ' Calculate how many tiles across and down
            Dim maxcols As Integer = img.PixelWidth / 256
            Dim maxrows As Integer = img.PixelHeight / 256

            ' Get the bounding rectangle of the source image, for clipping
            Dim MainRect As Rect = New Rect(0, 0, img.PixelWidth, img.PixelHeight)
            For j As Integer = 0 To maxrows
                For i As Integer = 0 To maxcols
                    ' Calculate the bounds of the tile
                    ' including a 1 pixel overlap each side
                    Dim smallrect As Rect = New Rect(CDbl(i * 256) - 1, CDbl(j * 256) - 1, 258.0, 258.0)

                    ' Adjust for the rectangles at the edges by intersecting
                    smallrect.Intersect(MainRect)

                    If smallrect.IsEmpty Then
                        Continue For
                    End If

                    ' We want a RenderTargetBitmap to render this tile into
                    ' Create one with the dimensions of this tile
                    Dim outbmp As RenderTargetBitmap = New RenderTargetBitmap(smallrect.Width, smallrect.Height, 96, 96, PixelFormats.Pbgra32)
                    Dim visual As DrawingVisual = New DrawingVisual()
                    Dim context As DrawingContext = visual.RenderOpen()

                    ' Set the offset of the source image into the destination bitmap
                    ' and render it
                    Dim rect As Rect = New Rect(-smallrect.Left, -smallrect.Top, img.PixelWidth, img.PixelHeight)
                    context.DrawImage(img, rect)
                    context.Close()
                    outbmp.Render(visual)

                    ' Save the bitmap tile
                    Dim destination = Path.Combine(topleveldir, String.Format("{0}_{1}", i, j))
                    EncodeBitmap(outbmp, destination)

                    ' null out everything we've used so the Garbage Collector
                    ' knows they're free. This could easily be voodoo since they'll go
                    ' out of scope, but it can't hurt.
                    outbmp = Nothing
                    context = Nothing
                    visual = Nothing
                Next
                GC.Collect()
                GC.WaitForPendingFinalizers()
            Next

            ' clear the source image since we don't need it anymore
            img = Nothing
            GC.Collect()
            GC.WaitForPendingFinalizers()

            ' Now render the lower levels by rendering the tiles from the level
            ' above to the next level down
            For level = MaxLevel - 1 To 0 Step -1
                RenderSubtiles(filesdir, dWidth, dHeight, MaxLevel, level)
            Next

            ' Now generate the .dzi file

            Dim format = "png"
            If imageType = ImageType.Jpeg Then
                format = "jpg"
            End If

            Dim dzi As New DeepZoomImage With {
                .TileSize = 256,
                .Overlap = 1,
                .Format = format,
                .Size = New ImageSize With {.Width = dWidth, .Height = dHeight},
                .DisplayRects = {
                    New DisplayRect With {.MinLevel = 1, .MaxLevel = MaxLevel, .Rect = New TileRect With {.X = 0, .Y = 0, .Width = dWidth, .Height = dHeight}}
                }
            }

            dzi.GetXml.SaveTo(destinationImage)
        End Sub

        ''' <summary>
        ''' Save the output bitmap as either Png or Jpeg
        ''' </summary>
        ''' <param name="outbmp"> Bitmap to save</param>
        ''' <param name="destination"> Path to save to, without the file extension</param>
        Private Sub EncodeBitmap(outbmp As RenderTargetBitmap, destination As String)
            If imageType = ImageType.Png Then
                Dim encoder As PngBitmapEncoder = New PngBitmapEncoder()
                encoder.Frames.Add(BitmapFrame.Create(outbmp))
                Dim fs As FileStream = New FileStream(destination & ".png", FileMode.Create)
                encoder.Save(fs)
                fs.Close()
            Else
                Dim encoder As JpegBitmapEncoder = New JpegBitmapEncoder()
                encoder.QualityLevel = 95
                encoder.Frames.Add(BitmapFrame.Create(outbmp))
                Dim fs As FileStream = New FileStream(destination & ".jpg", FileMode.Create)
                encoder.Save(fs)
                fs.Close()
            End If
        End Sub

        ''' <summary>
        ''' Specifies the output filetype
        ''' </summary>
        Private imageType As ImageType = ImageType.Jpeg

        ''' <summary>
        ''' Render the subtiles given a fully rendered top-level
        ''' </summary>
        ''' <param name="subfiles"> Path to the xxx_files directory</param>
        ''' <param name="imageWidth"> Width of the source image</param>
        ''' <param name="imageHeight"> Height of the source image</param>
        ''' <param name="maxlevel"> Top level of the tileset</param>
        ''' <param name="desiredlevel"> Level we want to render. Note it requires
        ''' that the level above this has already been rendered.</param>
        Private Sub RenderSubtiles(subfiles As String, imageWidth As Double, imageHeight As Double, maxlevel As Integer, desiredlevel As Integer)
            Dim formatextension = ".png"
            If imageType = ImageType.Jpeg Then
                formatextension = ".jpg"
            End If
            Dim uponelevel = desiredlevel + 1
            Dim desiredfactor = Math.Pow(2, maxlevel - desiredlevel)
            Dim higherfactor = Math.Pow(2, maxlevel - (desiredlevel + 1))
            Dim renderlevel As String = Path.Combine(subfiles, desiredlevel.ToString())
            Directory.CreateDirectory(renderlevel)
            Dim upperlevel As String = Path.Combine(subfiles, (desiredlevel + 1).ToString())

            ' Calculate the tiles we want to translate down
            Dim MainBounds As Rect = New Rect(0, 0, imageWidth, imageHeight)
            Dim OriginalRect As Rect = New Rect(0, 0, imageWidth, imageHeight)

            ' Scale down this rectangle to the scale factor of the level we want
            MainBounds.X = Math.Ceiling(MainBounds.X / desiredfactor)
            MainBounds.Y = Math.Ceiling(MainBounds.Y / desiredfactor)
            MainBounds.Width = Math.Ceiling(MainBounds.Width / desiredfactor)
            MainBounds.Height = Math.Ceiling(MainBounds.Height / desiredfactor)

            Dim lowx As Integer = Math.Floor(MainBounds.X / 256)
            Dim lowy As Integer = Math.Floor(MainBounds.Y / 256)
            Dim highx As Integer = Math.Floor(MainBounds.Right / 256)
            Dim highy As Integer = Math.Floor(MainBounds.Bottom / 256)

            For x = lowx To highx
                For y = lowy To highy
                    Dim smallrect As Rect = New Rect(CDbl(x * 256) - 1, CDbl(y * 256) - 1, 258.0, 258.0)
                    smallrect.Intersect(MainBounds)
                    Dim outbmp As RenderTargetBitmap = New RenderTargetBitmap(smallrect.Width, smallrect.Height, 96, 96, PixelFormats.Pbgra32)
                    Dim visual As DrawingVisual = New DrawingVisual()
                    Dim context As DrawingContext = visual.RenderOpen()

                    ' Calculate the bounds of this tile

                    Dim rect = smallrect
                    ' This is the rect of this tile. Now render any appropriate tiles onto it
                    ' The upper level tiles are twice as big, so they have to be shrunk down

                    Dim scaledRect As Rect = New Rect(rect.X * 2, rect.Y * 2, rect.Width * 2, rect.Height * 2)
                    For tx = lowx * 2 To highx * 2 + 1
                        For ty = lowy * 2 To highy * 2 + 1
                            ' See if this tile overlaps
                            Dim subrect = GetTileRectangle(tx, ty)
                            If scaledRect.IntersectsWith(subrect) Then
                                subrect.X -= scaledRect.X
                                subrect.Y -= scaledRect.Y
                                RenderTile(context, Path.Combine(upperlevel, tx.ToString() & "_" & ty.ToString() & formatextension), subrect)
                            End If
                        Next
                    Next
                    context.Close()
                    outbmp.Render(visual)

                    ' Render the completed tile and clear all resources used
                    Dim destination = Path.Combine(renderlevel, String.Format("{0}_{1}", x, y))
                    EncodeBitmap(outbmp, destination)
                    outbmp = Nothing
                    visual = Nothing
                    context = Nothing
                Next
                GC.Collect()
                GC.WaitForPendingFinalizers()
            Next

        End Sub

        ''' <summary>
        ''' Get the bounds of the given tile rectangle
        ''' </summary>
        ''' <param name="x"> x index of the tile</param>
        ''' <param name="y"> y index of the tile</param>
        ''' <returns>Bounding rectangle for the tile at the given indices</returns>
        Private Shared Function GetTileRectangle(x As Integer, y As Integer) As Rect
            Dim rect As Rect = New Rect(256 * x - 1, 256 * y - 1, 258, 258)
            If x = 0 Then
                rect.X = 0
                rect.Width = rect.Width - 1
            End If
            If y = 0 Then
                rect.Y = 0
                rect.Width = rect.Width - 1
            End If

            Return rect
        End Function

        ''' <summary>
        ''' Render the given tile rectangle, shrunk down by half to fit the next
        ''' lower level
        ''' </summary>
        ''' <param name="context"> DrawingContext for the DrawingVisual to render into</param>
        ''' <param name="path"> path to the tile we're rendering</param>
        ''' <param name="rect"> Rectangle to render this tile.</param>
        Private Sub RenderTile(context As DrawingContext, path As String, rect As Rect)
            If File.Exists(path) Then
                Dim img As BitmapImage = New BitmapImage(New Uri(path))
                rect = New Rect(rect.X / 2.0, rect.Y / 2.0, img.PixelWidth / 2.0, img.PixelHeight / 2.0)
                context.DrawImage(img, rect)
            End If
        End Sub

    End Class
End Namespace
