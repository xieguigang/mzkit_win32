Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

Partial Public Class KpImageViewer : Inherits UserControl

    <DllImport("user32.dll")>
    Private Shared Function GetKeyState(key As Integer) As Short
    End Function

    Private m_drawEngine As KP_DrawEngine
    Private m_drawing As KP_DrawObject
    Private m_preview As Bitmap

    Private animationEnabled As Boolean = False
    Private selectMode As Boolean = False
    Private shiftSelecting As Boolean = False
    Private ptSelectionStart As Point = New Point()
    Private ptSelectionEnd As Point = New Point()

    Private panelDragging As Boolean = False
    Private m_showPreviews As Boolean = False
    Private grabCursor As Cursor = Nothing
    Private dragCursor As Cursor = Nothing

    Public Delegate Sub ImageViewerRotationEventHandler(sender As Object, e As ImageViewerRotationEventArgs)
    Public Event AfterRotation As ImageViewerRotationEventHandler

    Protected Overridable Sub OnRotation(e As ImageViewerRotationEventArgs)
        RaiseEvent AfterRotation(Me, e)
    End Sub

    Public Delegate Sub ImageViewerZoomEventHandler(sender As Object, e As ImageViewerZoomEventArgs)
    Public Event AfterZoom As ImageViewerZoomEventHandler
    Public Event SetRange(range As DoubleRange)

    Public ReadOnly Property ColorScalerMenuUI As ContextMenuStrip
        Get
            Return ColorScaler1.MenuUI
        End Get
    End Property

    Protected Overridable Sub OnZoom(e As ImageViewerZoomEventArgs)
        RaiseEvent AfterZoom(Me, e)
    End Sub

    Public Sub InvalidatePanel()
        pbFull.Invalidate()
    End Sub

    Public Property GifAnimation As Boolean
        Get
            Return animationEnabled
        End Get
        Set(value As Boolean)
            animationEnabled = value

            If m_drawing IsNot Nothing Then
                If m_drawing.Gif IsNot Nothing Then
                    m_drawing.Gif.UpdateAnimator()
                End If
            End If
        End Set
    End Property

    Private Function IsKeyPressed(key As Integer) As Boolean
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

    'Public Property OpenButton As Boolean
    '    Get
    '        Return btnOpen.Visible
    '    End Get
    '    Set(value As Boolean)
    '        If value Then
    '            btnOpen.Show()

    '            If btnOpen.Visible = True Then
    '                ' Making sure it's aligned properly
    '                btnPreview.Location = New Point(198, btnPreview.Location.Y)
    '            Else
    '                ' Making sure it's aligned properly
    '                btnPreview.Location = New Point(btnOpen.Location.X, btnPreview.Location.Y)
    '            End If
    '        Else
    '            btnOpen.Hide()

    '            If btnOpen.Visible = True Then
    '                ' Making sure it's aligned properly
    '                btnPreview.Location = New Point(198, btnPreview.Location.Y)
    '            Else
    '                ' Making sure it's aligned properly
    '                btnPreview.Location = New Point(btnOpen.Location.X, btnPreview.Location.Y)
    '            End If
    '        End If
    '    End Set
    'End Property

    ''' <summary>
    ''' show preview button visible 
    ''' </summary>
    ''' <returns></returns>
    Public Property PreviewButton As Boolean
        Get
            Return ToolStripButtonPreview.Visible
        End Get
        Set(value As Boolean)
            ToolStripButtonPreview.Visible = value
        End Set
    End Property

    Public Overrides Property AllowDrop As Boolean
        Get
            Return MyBase.AllowDrop
        End Get
        Set(value As Boolean)
            pbFull.AllowDrop = value
            MyBase.AllowDrop = value
        End Set
    End Property

    Public ReadOnly Property Zoom As Double
        Get
            Return Math.Round(m_drawing.Zoom * 100, 0)
        End Get
    End Property

    Public ReadOnly Property OriginalSize As Size
        Get
            Return m_drawing.OriginalSize
        End Get
    End Property

    Public ReadOnly Property CurrentSize As Size
        Get
            Return m_drawing.CurrentSize
        End Get
    End Property

    'Public Property MenuColor As Color
    '    Get
    '        Return panelMenu.BackColor
    '    End Get
    '    Set(value As Color)
    '        panelMenu.BackColor = value
    '        panelPreview.BackColor = value
    '        panelNavigation.BackColor = value
    '    End Set
    'End Property

    'Public Property MenuPanelColor As Color
    '    Get
    '        Return panelMenu.BackColor
    '    End Get
    '    Set(value As Color)
    '        panelMenu.BackColor = value
    '    End Set
    'End Property

    'Public Property NavigationPanelColor As Color
    '    Get
    '        Return panelNavigation.BackColor
    '    End Get
    '    Set(value As Color)
    '        panelNavigation.BackColor = value
    '    End Set
    'End Property

    'Public Property PreviewPanelColor As Color
    '    Get
    '        Return panelPreview.BackColor
    '    End Get
    '    Set(value As Color)
    '        panelPreview.BackColor = value
    '    End Set
    'End Property

    'Public Property NavigationTextColor As Color
    '    Get
    '        Return lblNavigation.ForeColor
    '    End Get
    '    Set(value As Color)
    '        lblNavigation.ForeColor = value
    '    End Set
    'End Property

    'Public Property TextColor As Color
    '    Get
    '        Return lblPreview.ForeColor
    '    End Get
    '    Set(value As Color)
    '        lblPreview.ForeColor = value
    '        lblNavigation.ForeColor = value
    '    End Set
    'End Property

    'Public Property PreviewTextColor As Color
    '    Get
    '        Return lblPreview.ForeColor
    '    End Get
    '    Set(value As Color)
    '        lblPreview.ForeColor = value
    '    End Set
    'End Property

    Public Property BackgroundColor As Color
        Get
            Return pbFull.BackColor
        End Get
        Set(value As Color)
            pbFull.BackColor = value
        End Set
    End Property

    'Public Property PreviewText As String
    '    Get
    '        Return lblPreview.Text
    '    End Get
    '    Set(value As String)
    '        lblPreview.Text = value
    '    End Set
    'End Property

    Public Sub OpenImageFile(value As String)
        m_drawing.OpenImageByPath(value)
        UpdatePanels(True)
        ToggleMultiPage()
    End Sub

    ''' <summary>
    ''' set image on viewer for show to user
    ''' </summary>
    ''' <returns></returns>
    Public Property Image As Bitmap
        Get
            Return m_drawing.Image
        End Get
        Set(value As Bitmap)
            m_drawing.Image = value

            UpdatePanels(True)
            ToggleMultiPage()
        End Set
    End Property

    Public Property Rotation As Integer
        Get
            Return m_drawing.Rotation
        End Get
        Set(value As Integer)
            ' Making sure the rotation is 0, 90, 180 or 270 degrees!
            If value = 90 OrElse value = 180 OrElse value = 270 OrElse value = 0 Then
                m_drawing.Rotation = value
            End If
        End Set
    End Property

    Private Sub Preview()
        ' Hide preview panel mechanics
        ' Making sure that UpdatePanels doesn't get called when it's hidden!

        If m_showPreviews <> pbPanelAirscape.Visible Then
            If m_showPreviews = False Then
                ' panelPreview.Hide()
                pbPanelAirscape.Hide()
                pbFull.Hide()
                pbFull.Dock = DockStyle.None

                SplitContainer1.Show()
                SplitContainer1.Dock = DockStyle.Fill

                ' pbFull.Width = pbFull.Width + (4 + panelPreview.Width)

                If m_drawing.MultiPage Then
                    ' panelNavigation.Location = panelPreview.Location
                Else
                    ' panelMenu.Width = pbFull.Width
                End If

                InitControl()
                m_drawing.AvoidOutOfScreen()
                pbFull.Refresh()
            Else
                SplitContainer1.Dock = DockStyle.None
                SplitContainer1.Hide()

                ' panelPreview.Show()
                pbPanelAirscape.Show()
                pbFull.Show()
                pbFull.Dock = DockStyle.Fill

                ' pbFull.Width = pbFull.Width - (4 + panelPreview.Width)

                If m_drawing.MultiPage Then
                    'panelNavigation.Location = New Point(
                    '    panelPreview.Location.X, 
                    '    pbPanel.Location.Y + (pbPanel.Size.Height + 5))
                Else
                    'panelMenu.Width = pbFull.Width
                End If

                InitControl()
                m_drawing.AvoidOutOfScreen()
                pbFull.Refresh()

                UpdatePanels(True)
            End If

            ToolStripButtonZoomIn.Enabled = m_showPreviews
            ToolStripButtonZoomOut.Enabled = m_showPreviews
            ToolStripButtonRotate270.Enabled = m_showPreviews
            ToolStripButtonRotate90.Enabled = m_showPreviews
            ToolStripButtonFitToScreen.Enabled = m_showPreviews
            ToolStripButtonMode.Enabled = m_showPreviews
            ToolStripComboBoxZoom.Enabled = m_showPreviews
        End If
    End Sub

    Public Property ShowPreview As Boolean
        Get
            Return m_showPreviews
        End Get
        Set(value As Boolean)
            If m_showPreviews <> value Then
                m_showPreviews = value
                Preview()
            End If
        End Set
    End Property

    Public Property SelectPolygonMode As Boolean
        Get
            Return MSICanvas.SelectPolygonMode
        End Get
        Set(value As Boolean)
            MSICanvas.SelectPolygonMode = value
        End Set
    End Property

    Public Property ShowPointInform As Boolean
        Get
            Return MSICanvas.ShowPointInform
        End Get
        Set(value As Boolean)
            MSICanvas.ShowPointInform = value
        End Set
    End Property

    Public ReadOnly Property CanvasSize As Size
        Get
            Return MSICanvas.Size
        End Get
    End Property

    Public ReadOnly Property MSImage As Image
        Get
            Return MSICanvas.BackgroundImage
        End Get
    End Property

    Public Sub New()
        ' DrawEngine & DrawObject initiralization
        m_drawEngine = New KP_DrawEngine()
        m_drawing = New KP_DrawObject(Me)

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
        If m_drawing IsNot Nothing Then
            m_drawing.Dispose()
        End If

        If m_drawEngine IsNot Nothing Then
            m_drawEngine.Dispose()
        End If

        If m_preview IsNot Nothing Then
            m_preview.Dispose()
        End If
    End Sub

    Public Sub InitControl()
        ' Make sure panel is DoubleBuffering
        m_drawEngine.CreateDoubleBuffer(pbFull.CreateGraphics(), pbFull.Size.Width, pbFull.Size.Height, pbPanelAirscape.Size.Width, pbPanelAirscape.Size.Height)
    End Sub

    Private Sub FocusOnMe()
        ' Do not lose focus! ("Fix" for the Scrolling issue)
        Focus()
    End Sub

    Private Sub KP_ImageViewerV2_Load(sender As Object, e As EventArgs) Handles Me.Load
        ' Loop for ComboBox Items! Increments by 25%
        Dim z = 0.25

        While z <= 4.0
            ToolStripComboBoxZoom.Items.Add(z * 100 & "%")
            z = z + 0.25
        End While

        ToolStripComboBoxZoom.SelectedIndex = 3
    End Sub

    Private Sub ToggleMultiPage()
        '    If drawing.MultiPage Then
        '        If Not showPreviewField Then
        '            panelNavigation.Location = panelPreview.Location

        '            panelMenu.Width = panelPreview.Right - 2 - (4 + panelPreview.Width)
        '            pbFull.Width = panelPreview.Right - 2
        '        Else
        '            panelNavigation.Location = New Point(
        '                panelPreview.Location.X,
        '                pbPanel.Location.Y + (pbPanel.Size.Height + 5)
        '            )

        '            panelMenu.Width = panelPreview.Right - 2 - (4 + panelPreview.Width)
        '            pbFull.Width = panelPreview.Right - 2 - (4 + panelPreview.Width)
        '        End If

        '        panelNavigation.Show()
        '        lblNavigation.Text = "/ " & drawing.Pages.ToString()
        '        tbNavigation.Text = (drawing.CurrentPage + 1).ToString()
        '    Else
        '        If Not showPreviewField Then
        '            panelMenu.Width = panelPreview.Right - 2
        '        Else
        '            panelMenu.Width = pbFull.Width
        '        End If

        '        panelNavigation.Hide()
        '        lblNavigation.Text = "/ 0"
        '        tbNavigation.Text = "0"
        '    End If
    End Sub

    Private Sub KP_ImageViewerV2_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        InitControl()
        m_drawing.AvoidOutOfScreen()
        UpdatePanels(True)
    End Sub

    Private Sub pbFull_Paint(sender As Object, e As PaintEventArgs) Handles pbFull.Paint
        ' Can I double buffer?
        If m_drawEngine.CanDoubleBuffer() Then
            ' Yes I can!
            m_drawEngine.g.FillRectangle(New SolidBrush(pbFull.BackColor), e.ClipRectangle.X, e.ClipRectangle.Y, e.ClipRectangle.Width, e.ClipRectangle.Height)

            ' Drawing to backBuffer
            m_drawing.Draw(m_drawEngine.g)

            If animationEnabled Then
                Call ImageAnimator.UpdateFrames()
            End If

            ' Drawing to Panel
            m_drawEngine.Render(e.Graphics)
        End If
    End Sub

    Private Sub pbFull_MouseDown(sender As Object, e As MouseEventArgs) Handles pbFull.MouseDown
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
                m_drawing.BeginDrag(New Point(e.X, e.Y))

                ' Fancy cursor
                If grabCursor IsNot Nothing Then
                    pbFull.Cursor = grabCursor
                End If
            End If
        End If
    End Sub

    Private Sub pbFull_MouseUp(sender As Object, e As MouseEventArgs) Handles pbFull.MouseUp
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
            m_drawing.ZoomToSelection(rect, ptPbFull)

            ' Refresh my screen & update my preview panel
            pbFull.Refresh()
            UpdatePanels(True)
        Else
            ' Stop dragging and update my panels
            m_drawing.EndDrag()
            UpdatePanels(True)

            ' Fancy cursor
            If dragCursor IsNot Nothing Then
                pbFull.Cursor = dragCursor
            End If
        End If
    End Sub

    Private Sub pbFull_MouseMove(sender As Object, e As MouseEventArgs) Handles pbFull.MouseMove
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
            m_drawing.Drag(New Point(e.X, e.Y))
            If m_drawing.IsDragging Then
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

    Private Sub KP_ImageViewerV2_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
        m_drawing.Scroll(sender, e)

        If m_drawing.Image IsNot Nothing Then
            If e.Delta < 0 Then
                OnZoom(New ImageViewerZoomEventArgs(m_drawing.Zoom, KpZoom.ZoomOut))
            Else
                OnZoom(New ImageViewerZoomEventArgs(m_drawing.Zoom, KpZoom.ZoomIn))
            End If
        End If

        UpdatePanels(True)
    End Sub

    Private Sub btnOpen_Click(sender As Object, e As EventArgs)
        Dim openFileDialog As OpenFileDialog = New OpenFileDialog()

        openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.gif;*.bmp;*.png;*.tif;*.tiff;*.wmf;*.emf|JPEG Files (*.jpg)|*.jpg;*.jpeg|GIF Files (*.gif)|*.gif|BMP Files (*.bmp)|*.bmp|PNG Files (*.png)|*.png|TIF files (*.tif;*.tiff)|*.tif;*.tiff|EMF/WMF Files (*.wmf;*.emf)|*.wmf;*.emf|All files (*.*)|*.*"

        If openFileDialog.ShowDialog(Me) = DialogResult.OK Then
            OpenImageFile(openFileDialog.FileName)
        End If

        UpdatePanels(True)
    End Sub

    Public Sub Rotate90() Handles ToolStripButtonRotate90.Click
        If m_drawing IsNot Nothing Then
            m_drawing.Rotate90()

            ' AfterRotation Event
            OnRotation(New ImageViewerRotationEventArgs(m_drawing.Rotation))
            UpdatePanels(True)
            ToggleMultiPage()
        End If
    End Sub

    Public Sub Rotate180()
        If m_drawing IsNot Nothing Then
            m_drawing.Rotate180()

            ' AfterRotation Event
            OnRotation(New ImageViewerRotationEventArgs(m_drawing.Rotation))
            UpdatePanels(True)
            ToggleMultiPage()
        End If
    End Sub

    Public Sub Rotate270() Handles ToolStripButtonRotate270.Click
        If m_drawing IsNot Nothing Then
            m_drawing.Rotate270()

            ' AfterRotation Event
            OnRotation(New ImageViewerRotationEventArgs(m_drawing.Rotation))
            UpdatePanels(True)
            ToggleMultiPage()
        End If
    End Sub

    Private Sub btnZoomOut_Click(sender As Object, e As EventArgs) Handles ToolStripButtonZoomOut.Click
        m_drawing.ZoomOut()

        ' AfterZoom Event
        If m_drawing.Image IsNot Nothing Then
            OnZoom(New ImageViewerZoomEventArgs(m_drawing.Zoom, KpZoom.ZoomOut))
        End If
        UpdatePanels(True)
    End Sub

    Private Sub btnZoomIn_Click(sender As Object, e As EventArgs) Handles ToolStripButtonZoomIn.Click
        m_drawing.ZoomIn()

        ' AfterZoom Event
        If m_drawing.Image IsNot Nothing Then
            OnZoom(New ImageViewerZoomEventArgs(m_drawing.Zoom, KpZoom.ZoomIn))
        End If
        UpdatePanels(True)
    End Sub

    Private Sub btnFitToScreen_Click(sender As Object, e As EventArgs) Handles ToolStripButtonFitToScreen.Click
        m_drawing.FitToScreen()
        UpdatePanels(True)
    End Sub

    Private Sub cbZoom_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBoxZoom.SelectedIndexChanged
        Dim zoom = (ToolStripComboBoxZoom.SelectedIndex + 1) * 0.25
        Dim originalZoom = m_drawing.Zoom

        If m_drawing.Zoom <> zoom Then
            m_drawing.SetZoom(zoom)

            If m_drawing.Image IsNot Nothing Then
                If zoom > originalZoom Then
                    OnZoom(New ImageViewerZoomEventArgs(m_drawing.Zoom, KpZoom.ZoomIn))
                Else
                    OnZoom(New ImageViewerZoomEventArgs(m_drawing.Zoom, KpZoom.ZoomOut))
                End If
            End If

            UpdatePanels(True)
        End If
    End Sub

    Private Sub UpdatePanels(updatePreview As Boolean)
        If m_drawing.CurrentSize.Width > 0 AndAlso m_drawing.OriginalSize.Width > 0 Then
            ' Make sure panel is up to date
            pbFull.Refresh()

            ' Calculate zoom
            Dim zoom = Math.Round(m_drawing.CurrentSize.Width / m_drawing.OriginalSize.Width, 2)

            ' Display zoom in percentages
            ToolStripComboBoxZoom.Text = zoom * 100 & "%"

            If updatePreview AndAlso m_drawing.PreviewImage IsNot Nothing AndAlso pbPanelAirscape.Visible = True Then
                ' No memory leaks here
                If m_preview IsNot Nothing Then
                    m_preview.Dispose()
                    m_preview = Nothing
                End If

                ' New preview
                m_preview = New Bitmap(m_drawing.PreviewImage.Size.Width, m_drawing.PreviewImage.Size.Height)

                ' Make sure panel is the same size as the bitmap
                If pbPanelAirscape.Size <> m_drawing.PreviewImage.Size Then
                    pbPanelAirscape.Size = m_drawing.PreviewImage.Size
                End If

                ' New Graphics from the new bitmap we created (Empty)
                Using g = Graphics.FromImage(m_preview)
                    ' Draw the image on the bitmap
                    g.DrawImage(m_drawing.PreviewImage, 0, 0, m_drawing.PreviewImage.Size.Width, m_drawing.PreviewImage.Size.Height)

                    Dim ratioX = m_drawing.PreviewImage.Size.Width / m_drawing.CurrentSize.Width
                    Dim ratioY = m_drawing.PreviewImage.Size.Height / m_drawing.CurrentSize.Height

                    Dim boxWidth = pbFull.Width * ratioX
                    Dim boxHeight = pbFull.Height * ratioY
                    Dim positionX = (m_drawing.BoundingBox.X - m_drawing.BoundingBox.X * 2) * ratioX
                    Dim positionY = (m_drawing.BoundingBox.Y - m_drawing.BoundingBox.Y * 2) * ratioY

                    ' Making the red pen
                    Dim pen As Pen = New Pen(Color.Red, 1)

                    If boxHeight >= m_drawing.PreviewImage.Size.Height Then
                        boxHeight = m_drawing.PreviewImage.Size.Height - 1
                    ElseIf boxHeight + positionY > m_drawing.PreviewImage.Size.Height Then
                        boxHeight = m_drawing.PreviewImage.Size.Height - positionY
                    End If

                    If boxWidth >= m_drawing.PreviewImage.Size.Width Then
                        boxWidth = m_drawing.PreviewImage.Size.Width - 1
                    ElseIf boxWidth + positionX > m_drawing.PreviewImage.Size.Width Then
                        boxWidth = m_drawing.PreviewImage.Size.Width - positionX
                    End If

                    ' Draw the rectangle on the bitmap
                    g.DrawRectangle(pen, New Rectangle(positionX, positionY, boxWidth, boxHeight))
                End Using

                ' Display the bitmap
                pbPanelAirscape.Image = m_preview
            End If
        End If
    End Sub

    Private Sub pbPanel_MouseDown(sender As Object, e As MouseEventArgs) Handles pbPanelAirscape.MouseDown
        If panelDragging = False Then
            m_drawing.JumpToOrigin(
                e.X,
                e.Y,
                pbPanelAirscape.Width,
                pbPanelAirscape.Height,
                pbFull.Width,
                pbFull.Height
            )
            UpdatePanels(True)

            panelDragging = True
        End If
    End Sub

    Private Sub pbFull_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles pbFull.MouseDoubleClick
        m_drawing.JumpToOrigin(
            e.X + (m_drawing.BoundingBox.X - m_drawing.BoundingBox.X * 2),
            e.Y + (m_drawing.BoundingBox.Y - m_drawing.BoundingBox.Y * 2),
            pbFull.Width,
            pbFull.Height
        )

        UpdatePanels(True)
    End Sub

    Private Sub pbFull_MouseHover(sender As Object, e As EventArgs) Handles pbFull.MouseHover
        ' Left shift or Right shift!
        If IsKeyPressed(&HA0) OrElse IsKeyPressed(&HA1) Then
            ' Fancy cursor
            pbFull.Cursor = Cursors.Cross
        Else
            ' Fancy cursor if not dragging
            If Not m_drawing.IsDragging Then
                pbFull.Cursor = dragCursor
            End If
        End If
    End Sub

    Private Sub KpImageViewer_Click(sender As Object, e As EventArgs) Handles Me.Click
        FocusOnMe()
    End Sub

    Private Sub pbFull_Click(sender As Object, e As EventArgs) Handles pbFull.Click
        FocusOnMe()
    End Sub

    Private Sub pbPanel_MouseMove(sender As Object, e As MouseEventArgs) Handles pbPanelAirscape.MouseMove
        If panelDragging Then
            m_drawing.JumpToOrigin(e.X, e.Y, pbPanelAirscape.Width, pbPanelAirscape.Height, pbFull.Width, pbFull.Height)
            UpdatePanels(True)
        End If
    End Sub

    Private Sub pbPanel_MouseUp(sender As Object, e As MouseEventArgs) Handles pbPanelAirscape.MouseUp
        panelDragging = False
    End Sub

    Private Sub pbFull_MouseEnter(sender As Object, e As EventArgs) Handles pbFull.MouseEnter
        If IsKeyPressed(&HA0) OrElse IsKeyPressed(&HA1) OrElse selectMode = True Then
            pbFull.Cursor = Cursors.Cross
        Else
            If dragCursor IsNot Nothing Then
                pbFull.Cursor = dragCursor
            End If
        End If
    End Sub

    Private Sub pbFull_MouseLeave(sender As Object, e As EventArgs) Handles pbFull.MouseLeave
        pbFull.Cursor = Cursors.Default
    End Sub

    Private Sub btnPreview_Click(sender As Object, e As EventArgs) Handles ToolStripButtonPreview.Click
        ShowPreview = Not ShowPreview
    End Sub

    Private Sub cbZoom_KeyPress(sender As Object, e As KeyPressEventArgs) Handles ToolStripComboBoxZoom.KeyPress
        Try
            ' If it's not a digit, delete or backspace then make sure the input is being handled with. (Suppressed)
            If Not Char.IsDigit(e.KeyChar) AndAlso Asc(e.KeyChar) <> Keys.Delete AndAlso Asc(e.KeyChar) <> Keys.Back Then
                ' If enter is pressed apply the entered zoom
                If Asc(e.KeyChar) = Keys.Return Then
                    Dim zoom = 0

                    ' Make sure the percent sign is out of the cbZoom.Text
                    Integer.TryParse(ToolStripComboBoxZoom.Text.Replace("%", ""), zoom)

                    ' If zoom is higher than zero
                    If zoom > 0 Then
                        ' Make it a double!
                        Dim zoomDouble = zoom / 100

                        m_drawing.SetZoom(zoomDouble)
                        UpdatePanels(True)
                    End If
                End If

                e.Handled = True
            End If
        Catch ex As Exception
            MessageBox.Show("ImageViewer error: " & ex.ToString())
        End Try
    End Sub

    Private Function CalculateReversibleRectangle(ptSelectStart As Point, ptSelectEnd As Point) As Rectangle
        Dim rect As New Rectangle()

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

    Private Sub DrawReversibleRectangle(rect As Rectangle)
        pbFull.Refresh()
        ControlPaint.DrawReversibleFrame(rect, Color.LightGray, FrameStyle.Dashed)
    End Sub

    Private Sub pbFull_DragDrop(sender As Object, e As DragEventArgs)
        Try
            ' Get The file(s) you dragged into an array. (We'll just pick the first image anyway)
            Dim FileList = CType(e.Data.GetData(DataFormats.FileDrop, False), String())
            Dim newBmp As Image = Nothing

            For f = 0 To FileList.Length - 1
                ' Make sure the file exists!
                If File.Exists(FileList(f)) Then
                    Dim ext As String = Path.GetExtension(FileList(f)).ToLower()

                    ' Checking the extensions to be Image formats
                    If Equals(ext, ".jpg") OrElse
                        Equals(ext, ".jpeg") OrElse
                        Equals(ext, ".gif") OrElse
                        Equals(ext, ".wmf") OrElse
                        Equals(ext, ".emf") OrElse
                        Equals(ext, ".bmp") OrElse
                        Equals(ext, ".png") OrElse
                        Equals(ext, ".tif") OrElse
                        Equals(ext, ".tiff") Then

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

    Private Sub pbFull_DragEnter(sender As Object, e As DragEventArgs)
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

    Private Sub btnMode_Click(sender As Object, e As EventArgs) Handles ToolStripButtonMode.Click
        If selectMode = False Then
            selectMode = True
            ToolStripButtonMode.Image = Global.Mzkit_win32.MSImagingViewerV2.Resources.btnSelect
        Else
            selectMode = False
            ToolStripButtonMode.Image = Global.Mzkit_win32.MSImagingViewerV2.Resources.btnDrag
        End If
    End Sub

    'Private Sub btnNext_Click(sender As Object, e As EventArgs)
    '    drawing.NextPage()
    '    tbNavigation.Text = (drawing.CurrentPage + 1).ToString()

    '    pbFull.Refresh()
    '    UpdatePanels(True)
    'End Sub

    'Private Sub btnBack_Click(sender As Object, e As EventArgs)
    '    drawing.PreviousPage()
    '    tbNavigation.Text = (drawing.CurrentPage + 1).ToString()

    '    pbFull.Refresh()
    '    UpdatePanels(True)
    'End Sub

    'Private Sub tbNavigation_KeyPress(sender As Object, e As KeyPressEventArgs)
    '    Try
    '        ' If it's not a digit, delete or backspace then make sure the input is being handled with. (Suppressed)
    '        If Not Char.IsDigit(e.KeyChar) AndAlso Asc(e.KeyChar) <> Keys.Delete AndAlso Asc(e.KeyChar) <> Keys.Back Then
    '            ' If enter is pressed apply the entered zoom
    '            If Asc(e.KeyChar) = Keys.Return Then
    '                Dim page = 0

    '                Integer.TryParse(tbNavigation.Text, page)

    '                ' If zoom is higher than zero
    '                If page > 0 AndAlso page <= drawing.Pages Then
    '                    drawing.SetPage(page)
    '                    UpdatePanels(True)

    '                    btnZoomIn.Focus()
    '                Else
    '                    tbNavigation.Text = drawing.CurrentPage.ToString()
    '                End If
    '            End If

    '            e.Handled = True
    '        End If
    '    Catch ex As Exception
    '        MessageBox.Show("ImageViewer error: " & ex.ToString())
    '    End Try
    'End Sub

    Public Event SelectSample(tag As String)
    Public Event SelectPixelRegion(region As Rectangle)
    Public Event SelectPixel(x As Integer, y As Integer, color As Color)
    Public Event SelectPolygon(polygon() As PointF)
    Public Event GetPixelTissueMorphology(x As Integer, y As Integer, ByRef tag As String)

    Private Sub KpImageViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        MSICanvas.ViewerHost = Me
        MSICanvas.Location = New Point(0, ToolStrip1.Height)
        MSICanvas.Size = New Size(Me.Width, Me.Height - ToolStrip1.Height - StatusStrip1.Height)
        MSICanvas.BringToFront()
        MSICanvas.Visible = True

        Call ShowMessage("Ready!")
    End Sub

    Public Sub SetColorMapVisible(visible As Boolean)
        ' Throw New NotImplementedException()
    End Sub

    Public Sub ShowMessage(text As String)
        ToolStripStatusLabel1.Text = text
    End Sub

    Private Sub PixelSelector1_SelectPixelRegion(region As Rectangle) Handles MSICanvas.SelectPixelRegion
        RaiseEvent SelectPixelRegion(region)
    End Sub

    Private Sub PixelSelector1_SelectPixel(x As Integer, y As Integer, pixel As Color) Handles MSICanvas.SelectPixel
        RaiseEvent SelectPixel(x, y, pixel)
    End Sub

    Private Sub MSICanvas_SelectPolygon(polygon() As PointF) Handles MSICanvas.SelectPolygon
        RaiseEvent SelectPolygon(polygon)
    End Sub

    Public Sub SetMsImagingOutput(image As Image, scan_dimension As Size, background As Color,
                                  colorSet As ScalerPalette,
                                  range() As Double,
                                  mapLevels As Integer)

        MSICanvas.SetMsImagingOutput(image, scan_dimension)
        MSICanvas.BackColor = background

        Me.UpdateColorScaler(range, colorSet, mapLevels)
        Me.Image = New Bitmap(image)
    End Sub

    Dim intensityRange As Double()

    Public Sub UpdateColorScaler(range As Double(), colorSet As ScalerPalette, mapLevels As Integer)
        Dim max As Double = If(range.IsNullOrEmpty, 255, range.Max)
        Dim min As Double = If(range.IsNullOrEmpty, 0, range.Min)

        If max = min Then
            min = 0
        End If

        ColorScaler1.ScalerLevels = mapLevels
        ColorScaler1.ScalerPalette = colorSet
        ColorScaler1.SetIntensityMax(max)
        ColorScaler1.UpdateColors(callEvents:=False)

        intensityRange = {min, max}
    End Sub

    Private Sub ToolStripComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox1.SelectedIndexChanged
        Dim tagObj As Object = ToolStripComboBox1.SelectedItem
        Dim tag As String = tagObj.ToString

        RaiseEvent SelectSample(tag)
    End Sub

    Private Sub MSICanvas_GetPixelTissueMorphology(x As Integer, y As Integer, ByRef tag As String) Handles MSICanvas.GetPixelTissueMorphology
        RaiseEvent GetPixelTissueMorphology(x, y, tag)
    End Sub

    Private Sub ColorScaler1_SetRange(range As DoubleRange) Handles ColorScaler1.SetRange
        RaiseEvent SetRange(range)
    End Sub

    Private Sub ColorScaler1_RequestSetCustomRange() Handles ColorScaler1.RequestSetCustomRange
        Dim range As New InputIntensityRange
        Dim customRange As Action(Of InputIntensityRange) =
            Sub(config)
                RaiseEvent SetRange(config.ValueRange)
            End Sub

        If intensityRange.IsNullOrEmpty Then
            intensityRange = {0, 255}
        ElseIf intensityRange.Length = 1 Then
            intensityRange = {0, intensityRange(0)}
        End If

        Call InputDialog.Input(customRange, config:=range.SetRange(intensityRange(0), intensityRange(1)))
    End Sub
End Class


