Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices

Public Enum KpZoom
        ZoomIn
        ZoomOut
    End Enum

    Public Partial Class KpImageViewer
        Inherits UserControl
        <DllImport("user32.dll")>
        Private Shared Function GetKeyState(ByVal key As Integer) As Short
        End Function

        Private drawEngine As KP_DrawEngine
        Private drawing As KP_DrawObject
        Private previewField As Bitmap

        Private animationEnabled As Boolean = False
        Private selectMode As Boolean = False
        Private shiftSelecting As Boolean = False
        Private ptSelectionStart As Point = New Point()
        Private ptSelectionEnd As Point = New Point()

        Private panelDragging As Boolean = False
        Private showPreviewField As Boolean = True
        Private grabCursor As Cursor = Nothing
        Private dragCursor As Cursor = Nothing

        Public Delegate Sub ImageViewerRotationEventHandler(ByVal sender As Object, ByVal e As ImageViewerRotationEventArgs)
        Public Event AfterRotation As ImageViewerRotationEventHandler

        Protected Overridable Sub OnRotation(ByVal e As ImageViewerRotationEventArgs)
            RaiseEvent AfterRotation(Me, e)
        End Sub

        Public Delegate Sub ImageViewerZoomEventHandler(ByVal sender As Object, ByVal e As ImageViewerZoomEventArgs)
        Public Event AfterZoom As ImageViewerZoomEventHandler

        Protected Overridable Sub OnZoom(ByVal e As ImageViewerZoomEventArgs)
            RaiseEvent AfterZoom(Me, e)
        End Sub

        Public Sub InvalidatePanel()
            pbFull.Invalidate()
        End Sub

        Public Property GifAnimation As Boolean
            Get
                Return animationEnabled
            End Get
            Set(ByVal value As Boolean)
                animationEnabled = value

                If drawing IsNot Nothing Then
                    If drawing.Gif IsNot Nothing Then
                        drawing.Gif.UpdateAnimator()
                    End If
                End If
            End Set
        End Property

        Private Function IsKeyPressed(ByVal key As Integer) As Boolean
            Dim keyPressed = False
            Dim result = GetKeyState(key)

            Select Case result
                Case 0
                    ' Not pressed and not toggled
                    keyPressed = False

                Case 1
                    ' Not presses but toggled
                    keyPressed = False
                Case Else
                    ' Pressed
                    keyPressed = True
            End Select

            Return keyPressed
        End Function

        Public Property OpenButton As Boolean
            Get
                Return btnOpen.Visible
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    btnOpen.Show()

                    If btnOpen.Visible = True Then
                        ' Making sure it's aligned properly
                        btnPreview.Location = New Point(198, btnPreview.Location.Y)
                    Else
                        ' Making sure it's aligned properly
                        btnPreview.Location = New Point(btnOpen.Location.X, btnPreview.Location.Y)
                    End If
                Else
                    btnOpen.Hide()

                    If btnOpen.Visible = True Then
                        ' Making sure it's aligned properly
                        btnPreview.Location = New Point(198, btnPreview.Location.Y)
                    Else
                        ' Making sure it's aligned properly
                        btnPreview.Location = New Point(btnOpen.Location.X, btnPreview.Location.Y)
                    End If
                End If
            End Set
        End Property


        Public Property PreviewButton As Boolean
            Get
                Return btnPreview.Visible
            End Get
            Set(ByVal value As Boolean)
                If value Then
                    If btnOpen.Visible = True Then
                        ' Making sure it's aligned properly
                        btnPreview.Location = New Point(198, btnPreview.Location.Y)
                    Else
                        ' Making sure it's aligned properly
                        btnPreview.Location = New Point(btnOpen.Location.X, btnPreview.Location.Y)
                    End If

                    btnPreview.Show()
                Else
                    btnPreview.Hide()
                End If
            End Set
        End Property

        Public Overrides Property AllowDrop As Boolean
            Get
                Return MyBase.AllowDrop
            End Get
            Set(ByVal value As Boolean)
                pbFull.AllowDrop = value
                MyBase.AllowDrop = value
            End Set
        End Property

        Public ReadOnly Property Zoom As Double
            Get
                Return Math.Round(drawing.Zoom * 100, 0)
            End Get
        End Property

        Public ReadOnly Property OriginalSize As Size
            Get
                Return drawing.OriginalSize
            End Get
        End Property

        Public ReadOnly Property CurrentSize As Size
            Get
                Return drawing.CurrentSize
            End Get
        End Property

        Public Property MenuColor As Color
            Get
                Return panelMenu.BackColor
            End Get
            Set(ByVal value As Color)
                panelMenu.BackColor = value
                panelPreview.BackColor = value
                panelNavigation.BackColor = value
            End Set
        End Property

        Public Property MenuPanelColor As Color
            Get
                Return panelMenu.BackColor
            End Get
            Set(ByVal value As Color)
                panelMenu.BackColor = value
            End Set
        End Property

        Public Property NavigationPanelColor As Color
            Get
                Return panelNavigation.BackColor
            End Get
            Set(ByVal value As Color)
                panelNavigation.BackColor = value
            End Set
        End Property

        Public Property PreviewPanelColor As Color
            Get
                Return panelPreview.BackColor
            End Get
            Set(ByVal value As Color)
                panelPreview.BackColor = value
            End Set
        End Property

        Public Property NavigationTextColor As Color
            Get
                Return lblNavigation.ForeColor
            End Get
            Set(ByVal value As Color)
                lblNavigation.ForeColor = value
            End Set
        End Property

        Public Property TextColor As Color
            Get
                Return lblPreview.ForeColor
            End Get
            Set(ByVal value As Color)
                lblPreview.ForeColor = value
                lblNavigation.ForeColor = value
            End Set
        End Property

        Public Property PreviewTextColor As Color
            Get
                Return lblPreview.ForeColor
            End Get
            Set(ByVal value As Color)
                lblPreview.ForeColor = value
            End Set
        End Property

        Public Property BackgroundColor As Color
            Get
                Return pbFull.BackColor
            End Get
            Set(ByVal value As Color)
                pbFull.BackColor = value
            End Set
        End Property

        Public Property PreviewText As String
            Get
                Return lblPreview.Text
            End Get
            Set(ByVal value As String)
                lblPreview.Text = value
            End Set
        End Property

        Public WriteOnly Property ImagePath As String
            Set(ByVal value As String)
                drawing.ImagePath = value

                UpdatePanels(True)
                ToggleMultiPage()
            End Set
        End Property

        Public Property Image As Bitmap
            Get
                Return drawing.Image
            End Get
            Set(ByVal value As Bitmap)
                drawing.Image = value

                UpdatePanels(True)
                ToggleMultiPage()
            End Set
        End Property

        Public Property Rotation As Integer
            Get
                Return drawing.Rotation
            End Get
            Set(ByVal value As Integer)
                ' Making sure the rotation is 0, 90, 180 or 270 degrees!
                If value = 90 OrElse value = 180 OrElse value = 270 OrElse value = 0 Then
                    drawing.Rotation = value
                End If
            End Set
        End Property

        Private Sub Preview()
            ' Hide preview panel mechanics
            ' Making sure that UpdatePanels doesn't get called when it's hidden!

            If showPreviewField <> pbPanel.Visible Then
                If showPreviewField = False Then
                    panelPreview.Hide()
                    pbPanel.Hide()

                    pbFull.Width = pbFull.Width + (4 + panelPreview.Width)

                    If drawing.MultiPage Then
                        panelNavigation.Location = panelPreview.Location
                    Else
                        panelMenu.Width = pbFull.Width
                    End If

                    InitControl()
                    drawing.AvoidOutOfScreen()
                    pbFull.Refresh()
                Else
                    panelPreview.Show()
                    pbPanel.Show()

                    pbFull.Width = pbFull.Width - (4 + panelPreview.Width)

                    If drawing.MultiPage Then
                        panelNavigation.Location = New Point(panelPreview.Location.X, pbPanel.Location.Y + (pbPanel.Size.Height + 5))
                    Else
                        panelMenu.Width = pbFull.Width
                    End If

                    InitControl()
                    drawing.AvoidOutOfScreen()
                    pbFull.Refresh()

                    UpdatePanels(True)
                End If
            End If
        End Sub

        Public Property ShowPreview As Boolean
            Get
                Return showPreviewField
            End Get
            Set(ByVal value As Boolean)
                If showPreviewField <> value Then
                    showPreviewField = value
                    Preview()
                End If
            End Set
        End Property

        Public Sub New()
            ' DrawEngine & DrawObject initiralization
            drawEngine = New KP_DrawEngine()
            drawing = New KP_DrawObject(Me)

            ' Stream to initialize the cursors.
            Dim imgStream As Stream = Nothing

            Try
                Dim a As Assembly = Assembly.GetExecutingAssembly()

                imgStream = a.GetManifestResourceStream("KaiwaProjects.Resources.Grab.cur")
                If imgStream IsNot Nothing Then
                    grabCursor = New Cursor(imgStream)
                    imgStream = Nothing
                End If

                imgStream = a.GetManifestResourceStream("KaiwaProjects.Resources.Drag.cur")
                If imgStream IsNot Nothing Then
                    dragCursor = New Cursor(imgStream)
                    imgStream = Nothing
                End If

            Catch
                ' Cursors could not be found
            End Try

            InitializeComponent()

            InitControl()

            Preview()
        End Sub

        Private Sub DisposeControl()
            ' No memory leaks here
            If drawing IsNot Nothing Then
                drawing.Dispose()
            End If

            If drawEngine IsNot Nothing Then
                drawEngine.Dispose()
            End If

            If previewField IsNot Nothing Then
                previewField.Dispose()
            End If
        End Sub

        Public Sub InitControl()
            ' Make sure panel is DoubleBuffering
            drawEngine.CreateDoubleBuffer(pbFull.CreateGraphics(), pbFull.Size.Width, pbFull.Size.Height, pbPanel.Size.Width, pbPanel.Size.Height)
        End Sub

        Private Sub FocusOnMe()
            ' Do not lose focus! ("Fix" for the Scrolling issue)
            Focus()
        End Sub

        Private Sub KP_ImageViewerV2_Load(ByVal sender As Object, ByVal e As EventArgs)
            ' Loop for ComboBox Items! Increments by 25%
            Dim z = 0.25

            While z <= 4.0
                cbZoom.Items.Add(z * 100 & "%")
                z = z + 0.25
            End While

            cbZoom.SelectedIndex = 3
        End Sub

        Private Sub ToggleMultiPage()
            If drawing.MultiPage Then
                If Not showPreviewField Then
                    panelNavigation.Location = panelPreview.Location

                    panelMenu.Width = panelPreview.Right - 2 - (4 + panelPreview.Width)
                    pbFull.Width = panelPreview.Right - 2
                Else
                    panelNavigation.Location = New Point(panelPreview.Location.X, pbPanel.Location.Y + (pbPanel.Size.Height + 5))

                    panelMenu.Width = panelPreview.Right - 2 - (4 + panelPreview.Width)
                    pbFull.Width = panelPreview.Right - 2 - (4 + panelPreview.Width)
                End If

                panelNavigation.Show()
                lblNavigation.Text = "/ " & drawing.Pages.ToString()
                tbNavigation.Text = (drawing.CurrentPage + 1).ToString()
            Else
                If Not showPreviewField Then
                    panelMenu.Width = panelPreview.Right - 2
                Else
                    panelMenu.Width = pbFull.Width
                End If

                panelNavigation.Hide()
                lblNavigation.Text = "/ 0"
                tbNavigation.Text = "0"
            End If
        End Sub

        Private Sub KP_ImageViewerV2_Resize(ByVal sender As Object, ByVal e As EventArgs)
            InitControl()
            drawing.AvoidOutOfScreen()
            UpdatePanels(True)
        End Sub

        Private Sub pbFull_Paint(ByVal sender As Object, ByVal e As PaintEventArgs)
            ' Can I double buffer?
            If drawEngine.CanDoubleBuffer() Then
                ' Yes I can!
                drawEngine.g.FillRectangle(New SolidBrush(pbFull.BackColor), e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height)

                ' Drawing to backBuffer
                drawing.Draw(drawEngine.g)

                If animationEnabled Then
                    Call ImageAnimator.UpdateFrames()
                End If

                ' Drawing to Panel
                drawEngine.Render(e.Graphics)
            End If
        End Sub

        Private Sub pbFull_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
            If e.Button = MouseButtons.Left Then
                ' Left Shift or Right Shift pressed? Or is select mode one?
                If IsKeyPressed(&HA0) OrElse IsKeyPressed(&HA1) OrElse selectMode = True Then
                    ' Fancy cursor
                    pbFull.Cursor = Cursors.Cross

                    shiftSelecting = True

                    ' Initial seleciton
                    ptSelectionStart.X = e.X
                    ptSelectionStart.Y = e.Y

                    ' No selection end
                    ptSelectionEnd.X = -1
                    ptSelectionEnd.Y = -1
                Else
                    ' Start dragging
                    drawing.BeginDrag(New Point(e.X, e.Y))

                    ' Fancy cursor
                    If grabCursor IsNot Nothing Then
                        pbFull.Cursor = grabCursor
                    End If
                End If
            End If
        End Sub

        Private Sub pbFull_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
            ' Am i dragging or selecting?
            If shiftSelecting = True Then
                ' Calculate my selection rectangle
                Dim rect = CalculateReversibleRectangle(ptSelectionStart, ptSelectionEnd)

                ' Clear the selection rectangle
                ptSelectionEnd.X = -1
                ptSelectionEnd.Y = -1
                ptSelectionStart.X = -1
                ptSelectionStart.Y = -1

                ' Stop selecting
                shiftSelecting = False

                ' Position of the panel to the screen
                Dim ptPbFull = PointToScreen(pbFull.Location)

                ' Zoom to my selection
                drawing.ZoomToSelection(rect, ptPbFull)

                ' Refresh my screen & update my preview panel
                pbFull.Refresh()
                UpdatePanels(True)
            Else
                ' Stop dragging and update my panels
                drawing.EndDrag()
                UpdatePanels(True)

                ' Fancy cursor
                If dragCursor IsNot Nothing Then
                    pbFull.Cursor = dragCursor
                End If
            End If
        End Sub

        Private Sub pbFull_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
            ' Am I dragging or selecting?
            If shiftSelecting = True Then
                ' Keep selecting
                ptSelectionEnd.X = e.X
                ptSelectionEnd.Y = e.Y

                Dim pbFullRect As Rectangle = New Rectangle(0, 0, pbFull.Width - 1, pbFull.Height - 1)

                ' Am I still selecting within my panel?
                If pbFullRect.Contains(New Point(e.X, e.Y)) Then
                    ' If so, draw my Rubber Band Rectangle!
                    Dim rect = CalculateReversibleRectangle(ptSelectionStart, ptSelectionEnd)
                    DrawReversibleRectangle(rect)
                End If
            Else
                ' Keep dragging
                drawing.Drag(New Point(e.X, e.Y))
                If drawing.IsDragging Then
                    UpdatePanels(False)
                Else
                    ' I'm not dragging OR selecting
                    ' Make sure if left or right shift is pressed to change cursor

                    If IsKeyPressed(&HA0) OrElse IsKeyPressed(&HA1) OrElse selectMode = True Then
                        ' Fancy Cursor
                        If pbFull.Cursor IsNot Cursors.Cross Then
                            pbFull.Cursor = Cursors.Cross
                        End If
                    Else
                        ' Fancy Cursor
                        If pbFull.Cursor IsNot dragCursor Then
                            pbFull.Cursor = dragCursor
                        End If
                    End If
                End If
            End If
        End Sub

        Private Sub KP_ImageViewerV2_MouseWheel(ByVal sender As Object, ByVal e As MouseEventArgs)
            drawing.Scroll(sender, e)

            If drawing.Image IsNot Nothing Then
                If e.Delta < 0 Then
                    OnZoom(New ImageViewerZoomEventArgs(drawing.Zoom, KpZoom.ZoomOut))
                Else
                    OnZoom(New ImageViewerZoomEventArgs(drawing.Zoom, KpZoom.ZoomIn))
                End If
            End If

            UpdatePanels(True)
        End Sub

        Private Sub btnOpen_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim openFileDialog As OpenFileDialog = New OpenFileDialog()

            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.gif;*.bmp;*.png;*.tif;*.tiff;*.wmf;*.emf|JPEG Files (*.jpg)|*.jpg;*.jpeg|GIF Files (*.gif)|*.gif|BMP Files (*.bmp)|*.bmp|PNG Files (*.png)|*.png|TIF files (*.tif;*.tiff)|*.tif;*.tiff|EMF/WMF Files (*.wmf;*.emf)|*.wmf;*.emf|All files (*.*)|*.*"

            If openFileDialog.ShowDialog(Me) = DialogResult.OK Then
                ImagePath = openFileDialog.FileName
            End If

            UpdatePanels(True)
        End Sub

        Private Sub btnRotate270_Click(ByVal sender As Object, ByVal e As EventArgs)
            If drawing IsNot Nothing Then
                drawing.Rotate270()

                ' AfterRotation Event
                OnRotation(New ImageViewerRotationEventArgs(drawing.Rotation))
                UpdatePanels(True)
                ToggleMultiPage()
            End If
        End Sub

        Private Sub btnRotate90_Click(ByVal sender As Object, ByVal e As EventArgs)
            If drawing IsNot Nothing Then
                drawing.Rotate90()

                ' AfterRotation Event
                OnRotation(New ImageViewerRotationEventArgs(drawing.Rotation))
                UpdatePanels(True)
                ToggleMultiPage()
            End If
        End Sub

        Public Sub Rotate90()
            If drawing IsNot Nothing Then
                drawing.Rotate90()

                ' AfterRotation Event
                OnRotation(New ImageViewerRotationEventArgs(drawing.Rotation))
                UpdatePanels(True)
                ToggleMultiPage()
            End If
        End Sub

        Public Sub Rotate180()
            If drawing IsNot Nothing Then
                drawing.Rotate180()

                ' AfterRotation Event
                OnRotation(New ImageViewerRotationEventArgs(drawing.Rotation))
                UpdatePanels(True)
                ToggleMultiPage()
            End If
        End Sub

        Public Sub Rotate270()
            If drawing IsNot Nothing Then
                drawing.Rotate270()

                ' AfterRotation Event
                OnRotation(New ImageViewerRotationEventArgs(drawing.Rotation))
                UpdatePanels(True)
                ToggleMultiPage()
            End If
        End Sub

        Private Sub btnZoomOut_Click(ByVal sender As Object, ByVal e As EventArgs)
            drawing.ZoomOut()

            ' AfterZoom Event
            If drawing.Image IsNot Nothing Then
                OnZoom(New ImageViewerZoomEventArgs(drawing.Zoom, KpZoom.ZoomOut))
            End If
            UpdatePanels(True)
        End Sub

        Private Sub btnZoomIn_Click(ByVal sender As Object, ByVal e As EventArgs)
            drawing.ZoomIn()

            ' AfterZoom Event
            If drawing.Image IsNot Nothing Then
                OnZoom(New ImageViewerZoomEventArgs(drawing.Zoom, KpZoom.ZoomIn))
            End If
            UpdatePanels(True)
        End Sub

        Private Sub btnFitToScreen_Click(ByVal sender As Object, ByVal e As EventArgs)
            drawing.FitToScreen()
            UpdatePanels(True)
        End Sub

        Private Sub cbZoom_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
            Dim zoom = (cbZoom.SelectedIndex + 1) * 0.25
            Dim originalZoom = drawing.Zoom

            If drawing.Zoom <> zoom Then
                drawing.SetZoom(zoom)

                If drawing.Image IsNot Nothing Then
                    If zoom > originalZoom Then
                        OnZoom(New ImageViewerZoomEventArgs(drawing.Zoom, KpZoom.ZoomIn))
                    Else
                        OnZoom(New ImageViewerZoomEventArgs(drawing.Zoom, KpZoom.ZoomOut))
                    End If
                End If

                UpdatePanels(True)
            End If
        End Sub

        Private Sub UpdatePanels(ByVal updatePreview As Boolean)
            If drawing.CurrentSize.Width > 0 AndAlso drawing.OriginalSize.Width > 0 Then
                ' Make sure panel is up to date
                pbFull.Refresh()

                ' Calculate zoom
                Dim zoom = Math.Round(drawing.CurrentSize.Width / drawing.OriginalSize.Width, 2)

                ' Display zoom in percentages
                cbZoom.Text = zoom * 100 & "%"

                If updatePreview AndAlso drawing.PreviewImage IsNot Nothing AndAlso pbPanel.Visible = True Then
                    ' No memory leaks here
                    If previewField IsNot Nothing Then
                        previewField.Dispose()
                        previewField = Nothing
                    End If

                    ' New preview
                    previewField = New Bitmap(drawing.PreviewImage.Size.Width, drawing.PreviewImage.Size.Height)

                    ' Make sure panel is the same size as the bitmap
                    If pbPanel.Size <> drawing.PreviewImage.Size Then
                        pbPanel.Size = drawing.PreviewImage.Size
                    End If

                    ' New Graphics from the new bitmap we created (Empty)
                    Using g = Graphics.FromImage(previewField)
                        ' Draw the image on the bitmap
                        g.DrawImage(drawing.PreviewImage, 0, 0, drawing.PreviewImage.Size.Width, drawing.PreviewImage.Size.Height)

                        Dim ratioX = drawing.PreviewImage.Size.Width / drawing.CurrentSize.Width
                        Dim ratioY = drawing.PreviewImage.Size.Height / drawing.CurrentSize.Height

                        Dim boxWidth = pbFull.Width * ratioX
                        Dim boxHeight = pbFull.Height * ratioY
                        Dim positionX = (drawing.BoundingBox.X - drawing.BoundingBox.X * 2) * ratioX
                        Dim positionY = (drawing.BoundingBox.Y - drawing.BoundingBox.Y * 2) * ratioY

                        ' Making the red pen
                        Dim pen As Pen = New Pen(Color.Red, 1)

                        If boxHeight >= drawing.PreviewImage.Size.Height Then
                            boxHeight = drawing.PreviewImage.Size.Height - 1
                        ElseIf boxHeight + positionY > drawing.PreviewImage.Size.Height Then
                            boxHeight = drawing.PreviewImage.Size.Height - positionY
                        End If

                        If boxWidth >= drawing.PreviewImage.Size.Width Then
                            boxWidth = drawing.PreviewImage.Size.Width - 1
                        ElseIf boxWidth + positionX > drawing.PreviewImage.Size.Width Then
                            boxWidth = drawing.PreviewImage.Size.Width - positionX
                        End If

                        ' Draw the rectangle on the bitmap
                        g.DrawRectangle(pen, New Rectangle(positionX, positionY, boxWidth, boxHeight))
                    End Using

                    ' Display the bitmap
                    pbPanel.Image = previewField
                End If
            End If
        End Sub

        Private Sub pbPanel_MouseDown(ByVal sender As Object, ByVal e As MouseEventArgs)
            If panelDragging = False Then
                drawing.JumpToOrigin(e.X, e.Y, pbPanel.Width, pbPanel.Height, pbFull.Width, pbFull.Height)
                UpdatePanels(True)

                panelDragging = True
            End If
        End Sub

        Private Sub pbFull_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs)
            drawing.JumpToOrigin(e.X + (drawing.BoundingBox.X - drawing.BoundingBox.X * 2), e.Y + (drawing.BoundingBox.Y - drawing.BoundingBox.Y * 2), pbFull.Width, pbFull.Height)
            UpdatePanels(True)
        End Sub

        Private Sub pbFull_MouseHover(ByVal sender As Object, ByVal e As EventArgs)
            ' Left shift or Right shift!
            If IsKeyPressed(&HA0) OrElse IsKeyPressed(&HA1) Then
                ' Fancy cursor
                pbFull.Cursor = Cursors.Cross
            Else
                ' Fancy cursor if not dragging
                If Not drawing.IsDragging Then
                    pbFull.Cursor = dragCursor
                End If
            End If
        End Sub

        Private Sub KpImageViewer_Click(ByVal sender As Object, ByVal e As EventArgs)
            FocusOnMe()
        End Sub

        Private Sub pbFull_Click(ByVal sender As Object, ByVal e As EventArgs)
            FocusOnMe()
        End Sub

        Private Sub pbPanel_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs)
            If panelDragging Then
                drawing.JumpToOrigin(e.X, e.Y, pbPanel.Width, pbPanel.Height, pbFull.Width, pbFull.Height)
                UpdatePanels(True)
            End If
        End Sub

        Private Sub pbPanel_MouseUp(ByVal sender As Object, ByVal e As MouseEventArgs)
            panelDragging = False
        End Sub

        Private Sub pbFull_MouseEnter(ByVal sender As Object, ByVal e As EventArgs)
            If IsKeyPressed(&HA0) OrElse IsKeyPressed(&HA1) OrElse selectMode = True Then
                pbFull.Cursor = Cursors.Cross
            Else
                If dragCursor IsNot Nothing Then
                    pbFull.Cursor = dragCursor
                End If
            End If
        End Sub

        Private Sub pbFull_MouseLeave(ByVal sender As Object, ByVal e As EventArgs)
            pbFull.Cursor = Cursors.Default
        End Sub

        Private Sub btnPreview_Click(ByVal sender As Object, ByVal e As EventArgs)
            If ShowPreview Then
                ShowPreview = False
            Else
                ShowPreview = True
            End If
        End Sub

        Private Sub cbZoom_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs)
            Try
                ' If it's not a digit, delete or backspace then make sure the input is being handled with. (Suppressed)
                If Not Char.IsDigit(e.KeyChar) AndAlso Asc(e.KeyChar) <> Keys.Delete AndAlso Asc(e.KeyChar) <> Keys.Back Then
                    ' If enter is pressed apply the entered zoom
                    If Asc(e.KeyChar) = Keys.Return Then
                        Dim zoom = 0

                        ' Make sure the percent sign is out of the cbZoom.Text
                        Integer.TryParse(cbZoom.Text.Replace("%", ""), zoom)

                        ' If zoom is higher than zero
                        If zoom > 0 Then
                            ' Make it a double!
                            Dim zoomDouble = zoom / 100

                            drawing.SetZoom(zoomDouble)
                            UpdatePanels(True)

                            btnZoomIn.Focus()
                        End If
                    End If

                    e.Handled = True
                End If
            Catch ex As Exception
                MessageBox.Show("ImageViewer error: " & ex.ToString())
            End Try
        End Sub

        Private Function CalculateReversibleRectangle(ByVal ptSelectStart As Point, ByVal ptSelectEnd As Point) As Rectangle
            Dim rect As Rectangle = New Rectangle()

            ptSelectStart = pbFull.PointToScreen(ptSelectStart)
            ptSelectEnd = pbFull.PointToScreen(ptSelectEnd)

            If ptSelectStart.X < ptSelectEnd.X Then
                rect.X = ptSelectStart.X
                rect.Width = ptSelectEnd.X - ptSelectStart.X
            Else
                rect.X = ptSelectEnd.X
                rect.Width = ptSelectStart.X - ptSelectEnd.X
            End If
            If ptSelectStart.Y < ptSelectEnd.Y Then
                rect.Y = ptSelectStart.Y
                rect.Height = ptSelectEnd.Y - ptSelectStart.Y
            Else
                rect.Y = ptSelectEnd.Y
                rect.Height = ptSelectStart.Y - ptSelectEnd.Y
            End If

            Return rect
        End Function

        Private Sub DrawReversibleRectangle(ByVal rect As Rectangle)
            pbFull.Refresh()
            ControlPaint.DrawReversibleFrame(rect, Color.LightGray, FrameStyle.Dashed)
        End Sub

        Private Sub pbFull_DragDrop(ByVal sender As Object, ByVal e As DragEventArgs)
            Try
                ' Get The file(s) you dragged into an array. (We'll just pick the first image anyway)
                Dim FileList = CType(e.Data.GetData(DataFormats.FileDrop, False), String())

                Dim newBmp As Image = Nothing

                For f = 0 To FileList.Length - 1
                    ' Make sure the file exists!
                    If File.Exists(FileList(f)) Then
                        Dim ext As String = Path.GetExtension(FileList(f)).ToLower()

                        ' Checking the extensions to be Image formats
                        If Equals(ext, ".jpg") OrElse Equals(ext, ".jpeg") OrElse Equals(ext, ".gif") OrElse Equals(ext, ".wmf") OrElse Equals(ext, ".emf") OrElse Equals(ext, ".bmp") OrElse Equals(ext, ".png") OrElse Equals(ext, ".tif") OrElse Equals(ext, ".tiff") Then
                            Try
                                ' Try to load it into a bitmap
                                'newBmp = Bitmap.FromFile(FileList[f]);
                                newBmp = New Bitmap(FileList(f))
                                Image = CType(newBmp, Bitmap)

                                ' If succeeded stop the loop
                                Exit For
                            Catch
                                ' Not an image?
                            End Try
                        End If
                    End If
                Next
            Catch ex As Exception
                MessageBox.Show("ImageViewer error: " & ex.ToString())
            End Try
        End Sub

        Private Sub pbFull_DragEnter(ByVal sender As Object, ByVal e As DragEventArgs)
            Try
                If e.Data.GetDataPresent(DataFormats.FileDrop) Then
                    ' Drop the file
                    e.Effect = DragDropEffects.Copy
                Else
                    ' I'm not going to accept this unknown format!
                    e.Effect = DragDropEffects.None
                End If
            Catch ex As Exception
                MessageBox.Show("ImageViewer error: " & ex.ToString())
            End Try
        End Sub

        Private Sub btnMode_Click(ByVal sender As Object, ByVal e As EventArgs)
            If selectMode = False Then
                selectMode = True
                ' btnMode.Image = Properties.Resources.btnDrag
            Else
                selectMode = False
                '  btnMode.Image = Properties.Resources.btnSelect
            End If
        End Sub

        Private Sub btnNext_Click(ByVal sender As Object, ByVal e As EventArgs)
            drawing.NextPage()
            tbNavigation.Text = (drawing.CurrentPage + 1).ToString()

            pbFull.Refresh()
            UpdatePanels(True)
        End Sub

        Private Sub btnBack_Click(ByVal sender As Object, ByVal e As EventArgs)
            drawing.PreviousPage()
            tbNavigation.Text = (drawing.CurrentPage + 1).ToString()

            pbFull.Refresh()
            UpdatePanels(True)
        End Sub

        Private Sub tbNavigation_KeyPress(ByVal sender As Object, ByVal e As KeyPressEventArgs)
            Try
                ' If it's not a digit, delete or backspace then make sure the input is being handled with. (Suppressed)
                If Not Char.IsDigit(e.KeyChar) AndAlso Asc(e.KeyChar) <> Keys.Delete AndAlso Asc(e.KeyChar) <> Keys.Back Then
                    ' If enter is pressed apply the entered zoom
                    If Asc(e.KeyChar) = Keys.Return Then
                        Dim page = 0

                        Integer.TryParse(tbNavigation.Text, page)

                        ' If zoom is higher than zero
                        If page > 0 AndAlso page <= drawing.Pages Then
                            drawing.SetPage(page)
                            UpdatePanels(True)

                            btnZoomIn.Focus()
                        Else
                            tbNavigation.Text = drawing.CurrentPage.ToString()
                        End If
                    End If

                    e.Handled = True
                End If
            Catch ex As Exception
                MessageBox.Show("ImageViewer error: " & ex.ToString())
            End Try
        End Sub
    End Class

    Public Class ImageViewerRotationEventArgs
        Inherits EventArgs
        Private rotationField As Integer
        Public ReadOnly Property Rotation As Integer
            Get
                Return rotationField
            End Get
        End Property

        Public Sub New(ByVal rotation As Integer)
            rotationField = rotation
        End Sub
    End Class

Public Class ImageViewerZoomEventArgs
    Inherits EventArgs
    Private zoomField As Integer
    Public ReadOnly Property Zoom As Integer
        Get
            Return zoomField
        End Get
    End Property

    Private inOutField As KpZoom
    Public ReadOnly Property InOut As KpZoom
        Get
            Return inOutField
        End Get
    End Property

    Public Sub New(ByVal zoom As Double, ByVal inOut As KpZoom)
        zoomField = Convert.ToInt32(Math.Round(zoom * 100, 0))
        inOutField = inOut
    End Sub
End Class
