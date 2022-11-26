Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.IO

Public Class KP_DrawObject

    Private KpViewer As KpImageViewer
    Private boundingRect As Rectangle
    Private dragPoint As Point
    Private dragging As Boolean
    Private bmp As Bitmap
    Private bmpPreview As Bitmap
    Private multiBmp As MultiPageImage
    Private gifBmp As GifImage

    Private zoomField As Double = 1.0
    Private Shared panelWidth As Integer = 0
    Private Shared panelHeight As Integer = 0
    Private Shared previewPanelWidth As Integer = 0
    Private Shared previewPanelHeight As Integer = 0
    Private rotationField As Integer = 0

    Private multiFrame As Boolean = False
    Private multiPageField As Boolean = False
    Private pagesField As Integer = 1
    Private currentPageField As Integer = 0

    Public ReadOnly Property BoundingBox As Rectangle
        Get
            Return boundingRect
        End Get
    End Property

    Public Sub Dispose()
        If Image IsNot Nothing Then
            Image.Dispose()
        End If
    End Sub

    Public ReadOnly Property IsDragging As Boolean
        Get
            Return dragging
        End Get
    End Property

    Public ReadOnly Property Gif As GifImage
        Get
            Return gifBmp
        End Get
    End Property

    Public ReadOnly Property OriginalSize As Size
        Get
            If Image IsNot Nothing Then
                If multiFrame = True Then
                    If gifBmp IsNot Nothing Then
                        If gifBmp.Rotation = 0 OrElse gifBmp.Rotation = 180 Then
                            Return gifBmp.CurrentFrame.Size
                        Else
                            Return New Size(gifBmp.CurrentFrame.Height, gifBmp.CurrentFrame.Width)
                        End If
                    End If

                    Return Size.Empty
                Else
                    Return Image.Size
                End If
            Else
                Return Size.Empty
            End If
        End Get
    End Property

    Public ReadOnly Property CurrentSize As Size
        Get

            If Not boundingRect.IsEmpty Then
                Return New Size(boundingRect.Width, boundingRect.Height)
            Else
                Return Size.Empty
            End If
        End Get
    End Property

    Public ReadOnly Property MultiPage As Boolean
        Get
            Return multiPageField
        End Get
    End Property

    Public ReadOnly Property Pages As Integer
        Get
            Return pagesField
        End Get
    End Property

    Public ReadOnly Property CurrentPage As Integer
        Get
            Return currentPageField
        End Get
    End Property

    Public ReadOnly Property Zoom As Double
        Get
            Return zoomField
        End Get
    End Property

    Public Property Rotation As Integer
        Get
            Return rotationField
        End Get
        Set(value As Integer)
            ' Making sure that the rotation is only 0, 90, 180 or 270 degrees!
            If value = 90 OrElse value = 180 OrElse value = 270 OrElse value = 0 Then
                rotationField = value
            End If
        End Set
    End Property

    Public Function GetPage(pageNumber As Integer) As Bitmap
        If multiBmp Is Nothing Then
            Return Nothing
        End If

        Dim pages = multiBmp.Image.GetFrameCount(FrameDimension.Page)
        If pages > pageNumber AndAlso pageNumber >= 0 Then
            multiBmp.Image.SelectActiveFrame(FrameDimension.Page, pageNumber)
            Return New Bitmap(multiBmp.Image)
        End If

        Return Nothing
    End Function
    Public ReadOnly Property ImageWidth As Integer
        Get
            If multiFrame = True Then
                If gifBmp IsNot Nothing Then
                    If gifBmp.Rotation = 0 OrElse gifBmp.Rotation = 180 Then
                        Return gifBmp.CurrentFrame.Width
                    Else
                        Return gifBmp.CurrentFrame.Height
                    End If
                End If

                Return 0
            Else
                Return Image.Width
            End If
        End Get
    End Property

    Public ReadOnly Property ImageHeight As Integer
        Get
            If multiFrame = True Then
                If gifBmp IsNot Nothing Then
                    If gifBmp.Rotation = 0 OrElse gifBmp.Rotation = 180 Then
                        Return gifBmp.CurrentFrame.Height
                    Else
                        Return gifBmp.CurrentFrame.Width
                    End If
                End If

                Return 0
            Else
                Return Image.Height
            End If
        End Get
    End Property

    Public Property Image As Bitmap
        Get
            If multiFrame = True Then
                Return gifBmp.CurrentFrame
            ElseIf multiPageField = True Then
                If multiBmp IsNot Nothing Then
                    Return multiBmp.Page
                Else
                    Return Nothing
                End If
            Else
                Return bmp
            End If
        End Get
        Set(value As Bitmap)
            Try
                If value IsNot Nothing Then
                    Call setImageValue(value)
                End If
            Catch ex As Exception
                Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
            End Try
        End Set
    End Property

    Private Sub setImageValue(value As Bitmap)
        currentPageField = 0

        ' No memory leaks here!
        If bmp IsNot Nothing Then
            bmp.Dispose()
            bmp = Nothing
        End If

        If multiBmp IsNot Nothing Then
            multiBmp.Dispose()
            multiBmp = Nothing
        End If

        Try
            Dim gifDimension As FrameDimension = New FrameDimension(value.FrameDimensionsList(0))
            Dim gifFrames = value.GetFrameCount(gifDimension)

            If gifFrames > 1 Then
                multiFrame = True
            Else
                multiFrame = False
            End If

            If Not multiFrame Then
                'Gets the total number of frames in the .tiff file
                pagesField = value.GetFrameCount(FrameDimension.Page)
                If pagesField > 1 Then
                    multiPageField = True
                Else
                    multiPageField = False
                End If
            End If

        Catch
            multiPageField = False
            pagesField = 1
        End Try

        If multiFrame = True Then
            gifBmp = New GifImage(KpViewer, value)
        ElseIf multiPageField = True Then
            bmp = Nothing

            multiBmp = New MultiPageImage(value)
        Else
            bmp = value
            multiBmp = Nothing
        End If

        ' Initial rotation adjustments
        If rotationField <> 0 Then
            If rotationField = 180 Then
                Image.RotateFlip(RotateFlipType.Rotate180FlipNone)
                boundingRect = New Rectangle(0, 0, ImageWidth * zoomField, ImageHeight * zoomField)
            Else
                If rotationField = 90 Then
                    Image.RotateFlip(RotateFlipType.Rotate90FlipNone)
                ElseIf rotationField = 270 Then
                    Image.RotateFlip(RotateFlipType.Rotate270FlipNone)
                End If

                ' Flip the X and Y values
                boundingRect = New Rectangle(0, 0, ImageHeight * zoomField, ImageWidth * zoomField)
            End If
        Else
            Image.RotateFlip(RotateFlipType.RotateNoneFlipNone)
            boundingRect = New Rectangle(0, 0, ImageWidth * zoomField, ImageHeight * zoomField)
        End If

        zoomField = 1.0
        bmpPreview = CreatePreviewImage()
        FitToScreen()
    End Sub

    Public ReadOnly Property PreviewImage As Image
        Get
            Return bmpPreview
        End Get
    End Property

    Private Sub setImageFilePath(value As String, temp As Bitmap)
        currentPageField = 0

        Try
            Dim extension = Path.GetExtension(value)

            If Equals(extension, ".gif") Then
                Dim gifDimension As FrameDimension = New FrameDimension(temp.FrameDimensionsList(0))
                Dim gifFrames = temp.GetFrameCount(gifDimension)

                If gifFrames > 1 Then
                    multiFrame = True
                Else
                    multiFrame = False
                End If
            Else
                multiFrame = False

                'Gets the total number of frames in the .tiff file
                pagesField = temp.GetFrameCount(FrameDimension.Page)
                If pagesField > 1 Then
                    multiPageField = True
                Else
                    multiPageField = False
                End If
            End If

        Catch
            multiPageField = False
            pagesField = 1
        End Try

        If multiFrame = True Then
            gifBmp = New GifImage(KpViewer, temp)
        ElseIf multiPageField = True Then
            bmp = Nothing

            multiBmp = New MultiPageImage(temp)
        Else
            bmp = temp
            multiBmp = Nothing
        End If

        ' Initial rotation
        If rotationField <> 0 Then
            If rotationField = 180 Then
                Image.RotateFlip(RotateFlipType.Rotate180FlipNone)
                boundingRect = New Rectangle(0, 0, ImageWidth * zoomField, ImageHeight * zoomField)
            Else
                If rotationField = 90 Then
                    Image.RotateFlip(RotateFlipType.Rotate90FlipNone)
                ElseIf rotationField = 270 Then
                    Image.RotateFlip(RotateFlipType.Rotate270FlipNone)
                End If

                ' Flipping X and Y values!
                boundingRect = New Rectangle(0, 0, ImageHeight * zoomField, ImageWidth * zoomField)
            End If
        Else
            Image.RotateFlip(RotateFlipType.RotateNoneFlipNone)
            boundingRect = New Rectangle(0, 0, ImageWidth * zoomField, ImageHeight * zoomField)
        End If

        zoomField = 1.0
        bmpPreview = CreatePreviewImage()
        FitToScreen()
    End Sub

    Public WriteOnly Property ImagePath As String
        Set(value As String)
            Try
                ' No memory leaks here!
                If bmp IsNot Nothing Then
                    bmp.Dispose()
                    bmp = Nothing
                End If

                If multiBmp IsNot Nothing Then
                    multiBmp.Dispose()
                    multiBmp = Nothing
                End If

                Dim temp As Bitmap = Nothing

                ' Make sure it does not crash on incorrect image formats
                Try
                    'temp = (Bitmap)Bitmap.FromFile(value);
                    temp = New Bitmap(value)
                Catch
                    temp = Nothing
                    Windows.Forms.MessageBox.Show("ImageViewer error: Incorrect image format!")
                End Try

                If temp IsNot Nothing Then
                    Call setImageFilePath(value, temp)
                End If
            Catch ex As Exception
                Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
            End Try
        End Set
    End Property

    Public Sub New(KpViewer As KpImageViewer, bmp As Bitmap)
        Try
            Me.KpViewer = KpViewer

            ' Initial dragging to false and an Image.
            dragging = False
            Image = bmp
            Image.RotateFlip(RotateFlipType.RotateNoneFlipNone)

            boundingRect = New Rectangle(0, 0, ImageWidth * zoomField, ImageHeight * zoomField)
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Private Function GetCodec(type As String) As ImageCodecInfo
        Dim info As ImageCodecInfo() = ImageCodecInfo.GetImageEncoders()

        For i = 0 To info.Length - 1
            Dim EnumName As String = type.ToString()
            If info(i).FormatDescription.Equals(EnumName) Then
                Return info(i)
            End If
        Next
        Return Nothing
    End Function

    Public Sub SetPage(page As Integer)
        Dim p = page - 1

        Try
            If Image IsNot Nothing AndAlso multiPageField = True Then
                If p < pagesField AndAlso p >= 0 Then
                    currentPageField = p

                    multiBmp.SetPage(p)
                    multiBmp.Rotate(rotationField)

                    ' No memory leaks here!
                    If bmpPreview IsNot Nothing Then
                        bmpPreview.Dispose()
                        bmpPreview = Nothing
                    End If

                    bmpPreview = CreatePreviewImage()
                    AvoidOutOfScreen()
                End If
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub NextPage()
        Try
            If Image IsNot Nothing AndAlso multiPageField = True Then
                Dim lNextPage = currentPageField + 1

                If lNextPage < pagesField Then
                    currentPageField = lNextPage

                    multiBmp.SetPage(currentPageField)
                    multiBmp.Rotate(rotationField)

                    ' No memory leaks here!
                    If bmpPreview IsNot Nothing Then
                        bmpPreview.Dispose()
                        bmpPreview = Nothing
                    End If

                    bmpPreview = CreatePreviewImage()
                    AvoidOutOfScreen()
                End If
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub PreviousPage()
        Try
            If Image IsNot Nothing AndAlso multiPageField = True Then
                Dim prevPage = currentPageField - 1

                If prevPage >= 0 Then
                    currentPageField = prevPage

                    multiBmp.SetPage(currentPageField)
                    multiBmp.Rotate(rotationField)

                    ' No memory leaks here!
                    If bmpPreview IsNot Nothing Then
                        bmpPreview.Dispose()
                        bmpPreview = Nothing
                    End If

                    bmpPreview = CreatePreviewImage()
                    AvoidOutOfScreen()
                End If
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub New(KpViewer As KpImageViewer)
        Try
            Me.KpViewer = KpViewer
            ' Initial dragging to false and No image.
            dragging = False
            bmp = Nothing
            multiBmp = Nothing
            gifBmp = Nothing
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Shared Sub UpdatePanelsize(w As Integer, h As Integer)
        Try
            ' Making sure panel size stays the same
            panelWidth = w
            panelHeight = h
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Shared Sub UpdatePreviewPanelsize(w As Integer, h As Integer)
        Try
            ' Making sure preview panel size stays the same
            previewPanelWidth = w
            previewPanelHeight = h
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub Rotate90()
        Try
            If Image IsNot Nothing Then
                Dim tempWidth = boundingRect.Width
                Dim tempHeight = boundingRect.Height

                boundingRect.Width = tempHeight
                boundingRect.Height = tempWidth

                rotationField = (rotationField + 90) Mod 360

                If multiFrame = True Then
                    gifBmp.Rotate(90)
                ElseIf MultiPage = True Then
                    If multiBmp IsNot Nothing Then
                        multiBmp.Rotate(90)
                    End If
                Else
                    Image.RotateFlip(RotateFlipType.Rotate90FlipNone)
                End If

                AvoidOutOfScreen()

                ' No memory leaks here!
                If bmpPreview IsNot Nothing Then
                    bmpPreview.Dispose()
                    bmpPreview = Nothing
                End If

                bmpPreview = CreatePreviewImage()
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub Rotate180()
        Try
            If Image IsNot Nothing Then
                Dim tempWidth = boundingRect.Width
                Dim tempHeight = boundingRect.Height

                boundingRect.Width = tempHeight
                boundingRect.Height = tempWidth

                rotationField = (rotationField + 180) Mod 360

                If multiFrame = True Then
                    gifBmp.Rotate(180)
                ElseIf MultiPage = True Then
                    If multiBmp IsNot Nothing Then
                        multiBmp.Rotate(180)
                    End If
                Else
                    Image.RotateFlip(RotateFlipType.Rotate180FlipNone)
                End If

                AvoidOutOfScreen()

                ' No memory leaks here!
                If bmpPreview IsNot Nothing Then
                    bmpPreview.Dispose()
                    bmpPreview = Nothing
                End If

                bmpPreview = CreatePreviewImage()
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub Rotate270()
        Try
            If Image IsNot Nothing Then
                Dim tempWidth = boundingRect.Width
                Dim tempHeight = boundingRect.Height

                boundingRect.Width = tempHeight
                boundingRect.Height = tempWidth

                rotationField = (rotationField + 270) Mod 360

                If multiFrame = True Then
                    gifBmp.Rotate(270)
                ElseIf MultiPage = True Then
                    If multiBmp IsNot Nothing Then
                        multiBmp.Rotate(270)
                    End If
                Else
                    Image.RotateFlip(RotateFlipType.Rotate270FlipNone)
                End If

                AvoidOutOfScreen()

                ' No memory leaks here!
                If bmpPreview IsNot Nothing Then
                    bmpPreview.Dispose()
                    bmpPreview = Nothing
                End If

                bmpPreview = CreatePreviewImage()
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Private Function RotateCenter(bmpSrc As Bitmap, theta As Single) As Bitmap
        Dim mRotate As Matrix = New Matrix()
        mRotate.Translate(bmpSrc.Width / -2, bmpSrc.Height / -2, MatrixOrder.Append)
        mRotate.RotateAt(theta, New Point(0, 0), MatrixOrder.Append)

        Using gp As GraphicsPath = New GraphicsPath()  ' transform image points by rotation matrix
            gp.AddPolygon(New Point() {New Point(0, 0), New Point(bmpSrc.Width, 0), New Point(0, bmpSrc.Height)})
            gp.Transform(mRotate)
            Dim pts = gp.PathPoints

            ' create destination bitmap sized to contain rotated source image
            Dim bbox = RotateBoundingBox(bmpSrc, mRotate)
            Dim bmpDest As Bitmap = New Bitmap(bbox.Width, bbox.Height)

            Using gDest = Graphics.FromImage(bmpDest)  ' draw source into dest
                Dim mDest As Matrix = New Matrix()
                mDest.Translate(bmpDest.Width / 2, bmpDest.Height / 2, MatrixOrder.Append)
                gDest.Transform = mDest
                gDest.DrawImage(bmpSrc, pts)
                gDest.DrawRectangle(Pens.Red, bbox)
                'drawAxes(gDest, Color.Red, 0, 0, 1, 100, "");
                Return bmpDest
            End Using
        End Using
    End Function

    Private Shared Function RotateBoundingBox(img As Image, matrix As Matrix) As Rectangle
        Dim gu As GraphicsUnit = New GraphicsUnit()
        Dim rImg = Rectangle.Round(img.GetBounds(gu))

        ' Transform the four points of the image, to get the resized bounding box.
        Dim topLeft As Point = New Point(rImg.Left, rImg.Top)
        Dim topRight As Point = New Point(rImg.Right, rImg.Top)
        Dim bottomRight As Point = New Point(rImg.Right, rImg.Bottom)
        Dim bottomLeft As Point = New Point(rImg.Left, rImg.Bottom)
        Dim points = New Point() {topLeft, topRight, bottomRight, bottomLeft}
        Dim gp As New GraphicsPath(points, New Byte() {
             PathPointType.Start,
             PathPointType.Line,
             PathPointType.Line,
             PathPointType.Line
        })
        gp.Transform(matrix)
        Return Rectangle.Round(gp.GetBounds())
    End Function

    Private Function CreatePreviewImage() As Bitmap
        ' 148 && 117 as initial and default size for the preview panel.
        Dim previewRect As Rectangle = New Rectangle(0, 0, 148, 117)

        Dim x_ratio = previewRect.Width / BoundingBox.Width
        Dim y_ratio = previewRect.Height / BoundingBox.Height

        If BoundingBox.Width <= previewRect.Width AndAlso BoundingBox.Height <= previewRect.Height Then
            previewRect.Width = BoundingBox.Width
            previewRect.Height = BoundingBox.Height
        ElseIf x_ratio * BoundingBox.Height < previewRect.Height Then
            previewRect.Height = Convert.ToInt32(Math.Ceiling(x_ratio * BoundingBox.Height))
            previewRect.Width = previewRect.Width
        Else
            previewRect.Width = Convert.ToInt32(Math.Ceiling(y_ratio * BoundingBox.Width))
            previewRect.Height = previewRect.Height
        End If

        Dim bmp As Bitmap = New Bitmap(previewRect.Width, previewRect.Height)

        If multiFrame = True Then
            If gifBmp IsNot Nothing Then
                Using g = Graphics.FromImage(bmp)
                    If gifBmp.Rotation <> 0 Then
                        g.DrawImage(RotateCenter(gifBmp.CurrentFrame, gifBmp.Rotation), previewRect)
                    Else
                        g.DrawImage(gifBmp.CurrentFrame, previewRect)
                    End If
                End Using
            End If
        Else
            Using g = Graphics.FromImage(bmp)
                If Image IsNot Nothing Then
                    g.DrawImage(Image, previewRect)
                End If
            End Using
        End If

        Return bmp
    End Function

    Public Sub ZoomToSelection(selection As Rectangle, ptPbFull As Point)
        Dim x = selection.X - ptPbFull.X
        Dim y = selection.Y - ptPbFull.Y
        Dim width = selection.Width
        Dim height = selection.Height

        ' So, where did my selection start on the entire picture?
        Dim selectedX As Integer = (boundingRect.X - CDbl(boundingRect.X) * 2 + x) / zoomField
        Dim selectedY As Integer = (boundingRect.Y - CDbl(boundingRect.Y) * 2 + y) / zoomField
        Dim selectedWidth = width
        Dim selectedHeight = height

        ' The selection width on the scale of the Original size!
        If zoomField < 1.0 OrElse zoomField > 1.0 Then
            selectedWidth = Convert.ToInt32(width / zoomField)
            selectedHeight = Convert.ToInt32(height / zoomField)
        End If

        ' What is the highest possible zoomrate?
        Dim zoomX = panelWidth / selectedWidth
        Dim zoomY = panelHeight / selectedHeight

        Dim newZoom = Math.Min(zoomX, zoomY)

        ' Avoid Int32 crashes!
        If newZoom * 100 < Integer.MaxValue AndAlso newZoom * 100 > Integer.MinValue Then
            SetZoom(newZoom)

            selectedWidth = CInt(selectedWidth * newZoom)
            selectedHeight = CInt(selectedHeight * newZoom)

            ' Center the selected area
            Dim offsetX = 0
            Dim offsetY = 0
            If selectedWidth < panelWidth Then
                offsetX = (panelWidth - selectedWidth) / 2
            End If
            If selectedHeight < panelHeight Then
                offsetY = (panelHeight - selectedHeight) / 2
            End If

            boundingRect.X = CInt(selectedX * newZoom) - CInt(selectedX * newZoom) * 2 + offsetX
            boundingRect.Y = CInt(selectedY * newZoom) - CInt(selectedY * newZoom) * 2 + offsetY

            AvoidOutOfScreen()
        End If
    End Sub

    Public Sub JumpToOrigin(x As Integer, y As Integer, width As Integer, height As Integer, pWidth As Integer, pHeight As Integer)
        Try
            Dim zoom = boundingRect.Width / width

            Dim originX As Integer = x * zoom
            Dim originY As Integer = y * zoom

            originX = originX - originX * 2
            originY = originY - originY * 2

            boundingRect.X = originX + pWidth / 2
            boundingRect.Y = originY + pHeight / 2

            AvoidOutOfScreen()
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub JumpToOrigin(x As Integer, y As Integer, width As Integer, height As Integer)
        Try
            boundingRect.X = x - width / 2 - (x - width / 2) * 2
            boundingRect.Y = y - height / 2 - (y - height / 2) * 2

            AvoidOutOfScreen()
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Function PointToOrigin(x As Integer, y As Integer, width As Integer, height As Integer) As Point
        Try
            Dim zoomX = width / boundingRect.Width
            Dim zoomY = height / boundingRect.Height

            If width > panelWidth Then
                Dim oldX As Integer = boundingRect.X - boundingRect.X * 2 + panelWidth / 2
                Dim oldY As Integer = boundingRect.Y - boundingRect.Y * 2 + panelHeight / 2

                Dim newX As Integer = oldX * zoomX
                Dim newY As Integer = oldY * zoomY

                Dim originX As Integer = newX - panelWidth / 2 - (newX - panelWidth / 2) * 2
                Dim originY As Integer = newY - panelHeight / 2 - (newY - panelHeight / 2) * 2

                Return New Point(originX, originY)
            Else
                If height > panelHeight Then
                    Dim oldY As Integer = boundingRect.Y - boundingRect.Y * 2 + panelHeight / 2

                    Dim newY As Integer = oldY * zoomY

                    Dim originY As Integer = newY - panelHeight / 2 - (newY - panelHeight / 2) * 2

                    Return New Point(0, originY)
                Else
                    Return New Point(0, 0)
                End If
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
            Return New Point(0, 0)
        End Try
    End Function

    Public Sub ZoomIn()
        Try
            If Image IsNot Nothing Then
                ' Make sure zoom steps are with 25%
                Dim index = 0.25 - zoomField Mod 0.25

                If index <> 0 Then
                    zoomField += index
                Else
                    zoomField += 0.25
                End If

                Dim p = PointToOrigin(boundingRect.X, boundingRect.Y, ImageWidth * zoomField, ImageHeight * zoomField)

                boundingRect = New Rectangle(p.X, p.Y, ImageWidth * zoomField, ImageHeight * zoomField)
                AvoidOutOfScreen()

            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub ZoomOut()
        Try
            If Image IsNot Nothing Then
                ' Make sure zoom steps are with 25% and higher than 0%
                If zoomField - 0.25 > 0 Then
                    If (zoomField - 0.25) Mod 0.25 <> 0 Then
                        zoomField -= zoomField Mod 0.25
                    Else
                        zoomField -= 0.25
                    End If
                End If

                Dim p = PointToOrigin(boundingRect.X, boundingRect.Y, ImageWidth * zoomField, ImageHeight * zoomField)

                boundingRect = New Rectangle(p.X, p.Y, ImageWidth * zoomField, ImageHeight * zoomField)
                AvoidOutOfScreen()
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub SetZoom(z As Double)
        Try
            If Image IsNot Nothing Then
                zoomField = z

                Dim p = PointToOrigin(boundingRect.X, boundingRect.Y, ImageWidth * zoomField, ImageHeight * zoomField)

                boundingRect = New Rectangle(p.X, p.Y, ImageWidth * zoomField, ImageHeight * zoomField)
                AvoidOutOfScreen()
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub Scroll(sender As Object, e As Windows.Forms.MouseEventArgs)
        Try
            If Image IsNot Nothing Then
                If e.Delta < 0 Then
                    ZoomOut()
                Else
                    ZoomIn()
                End If
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub FitToScreen()
        Try
            If Image IsNot Nothing Then
                Dim x_ratio = panelWidth / ImageWidth
                Dim y_ratio = panelHeight / ImageHeight

                If ImageWidth <= panelWidth AndAlso ImageHeight <= panelHeight Then
                    boundingRect.Width = ImageWidth
                    boundingRect.Height = ImageHeight
                ElseIf x_ratio * ImageHeight < panelHeight Then
                    boundingRect.Height = Convert.ToInt32(Math.Ceiling(x_ratio * ImageHeight))
                    boundingRect.Width = panelWidth
                Else
                    boundingRect.Width = Convert.ToInt32(Math.Ceiling(y_ratio * ImageWidth))
                    boundingRect.Height = panelHeight
                End If

                boundingRect.X = 0
                boundingRect.Y = 0

                zoomField = boundingRect.Width / ImageWidth
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub AvoidOutOfScreen()
        Try
            ' Am I lined out to the left?
            If boundingRect.X >= 0 Then
                boundingRect.X = 0
            ElseIf boundingRect.X <= boundingRect.Width - panelWidth - (boundingRect.Width - panelWidth) * 2 Then
                If boundingRect.Width - panelWidth - (boundingRect.Width - panelWidth) * 2 <= 0 Then
                    ' I am too far to the left!
                    boundingRect.X = boundingRect.Width - panelWidth - (boundingRect.Width - panelWidth) * 2
                Else
                    ' I am too far to the right!
                    boundingRect.X = 0
                End If
            End If

            ' Am I lined out to the top?
            If boundingRect.Y >= 0 Then
                boundingRect.Y = 0
            ElseIf boundingRect.Y <= boundingRect.Height - panelHeight - (boundingRect.Height - panelHeight) * 2 Then
                If boundingRect.Height - panelHeight - (boundingRect.Height - panelHeight) * 2 <= 0 Then
                    ' I am too far to the top!
                    boundingRect.Y = boundingRect.Height - panelHeight - (boundingRect.Height - panelHeight) * 2
                Else
                    ' I am too far to the bottom!
                    boundingRect.Y = 0
                End If
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub Drag(pt As Point)
        Try
            If Image IsNot Nothing Then
                If dragging = True Then
                    ' Am I dragging it outside of the panel?
                    If pt.X - dragPoint.X > boundingRect.Width - panelWidth - (boundingRect.Width - panelWidth) * 2 AndAlso pt.X - dragPoint.X < 0 Then
                        ' No, everything is just fine
                        boundingRect.X = pt.X - dragPoint.X
                    ElseIf pt.X - dragPoint.X > 0 Then
                        ' Now don't drag it out of the panel please
                        boundingRect.X = 0
                    ElseIf pt.X - dragPoint.X < boundingRect.Width - panelWidth - (boundingRect.Width - panelWidth) * 2 Then
                        ' I am dragging it out of my panel. How many pixels do I have left?
                        If boundingRect.Width - panelWidth - (boundingRect.Width - panelWidth) * 2 <= 0 Then
                            ' Make it fit perfectly
                            boundingRect.X = boundingRect.Width - panelWidth - (boundingRect.Width - panelWidth) * 2
                        End If
                    End If

                    ' Am I dragging it outside of the panel?
                    If pt.Y - dragPoint.Y > boundingRect.Height - panelHeight - (boundingRect.Height - panelHeight) * 2 AndAlso pt.Y - dragPoint.Y < 0 Then
                        ' No, everything is just fine
                        boundingRect.Y = pt.Y - dragPoint.Y
                    ElseIf pt.Y - dragPoint.Y > 0 Then
                        ' Now don't drag it out of the panel please
                        boundingRect.Y = 0
                    ElseIf pt.Y - dragPoint.Y < boundingRect.Height - panelHeight - (boundingRect.Height - panelHeight) * 2 Then
                        ' I am dragging it out of my panel. How many pixels do I have left?
                        If boundingRect.Height - panelHeight - (boundingRect.Height - panelHeight) * 2 <= 0 Then
                            ' Make it fit perfectly
                            boundingRect.Y = boundingRect.Height - panelHeight - (boundingRect.Height - panelHeight) * 2
                        End If
                    End If
                End If
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub BeginDrag(pt As Point)
        Try
            If Image IsNot Nothing Then
                ' Initial drag position
                dragPoint.X = pt.X - boundingRect.X
                dragPoint.Y = pt.Y - boundingRect.Y
                dragging = True
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub EndDrag()
        Try
            If Image IsNot Nothing Then
                dragging = False
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Public Sub Draw(g As Graphics)
        Try
            If multiFrame = True Then
                If gifBmp.CurrentFrame IsNot Nothing Then
                    If gifBmp.Rotation <> 0 Then
                        g.DrawImage(RotateCenter(gifBmp.CurrentFrame, gifBmp.Rotation), boundingRect)
                    Else
                        g.DrawImage(gifBmp.CurrentFrame, boundingRect)
                    End If
                End If
            End If
            If multiPageField = True Then
                If multiBmp IsNot Nothing Then
                    If multiBmp.Image IsNot Nothing Then
                        g.DrawImage(multiBmp.Image, boundingRect)
                    End If
                End If
            Else
                If bmp IsNot Nothing Then
                    g.DrawImage(bmp, boundingRect)
                End If
            End If
        Catch ex As Exception
            Windows.Forms.MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub
End Class
