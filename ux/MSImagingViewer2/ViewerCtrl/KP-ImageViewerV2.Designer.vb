Partial Class KpImageViewer
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <paramname="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        DisposeControl()

        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Component Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify 
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        panelPreview = New Windows.Forms.Panel()
        lblPreview = New Windows.Forms.Label()
        panelMenu = New Windows.Forms.Panel()
        btnMode = New Windows.Forms.Button()
        btnPreview = New Windows.Forms.Button()
        cbZoom = New Windows.Forms.ComboBox()
        btnFitToScreen = New Windows.Forms.Button()
        btnZoomIn = New Windows.Forms.Button()
        btnZoomOut = New Windows.Forms.Button()
        btnRotate270 = New Windows.Forms.Button()
        btnRotate90 = New Windows.Forms.Button()
        btnOpen = New Windows.Forms.Button()
        panelNavigation = New Windows.Forms.Panel()
        lblNavigation = New Windows.Forms.Label()
        tbNavigation = New Windows.Forms.TextBox()
        btnBack = New Windows.Forms.Button()
        btnNext = New Windows.Forms.Button()
        pbPanel = New Windows.Forms.PictureBox()
        pbFull = New PanelDoubleBuffered()
        panelPreview.SuspendLayout()
        panelMenu.SuspendLayout()
        panelNavigation.SuspendLayout()
        CType(pbPanel, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' panelPreview
        ' 
        panelPreview.Anchor = Windows.Forms.AnchorStyles.Top Or Windows.Forms.AnchorStyles.Right
        panelPreview.BackColor = System.Drawing.Color.LightSteelBlue
        panelPreview.BorderStyle = Windows.Forms.BorderStyle.FixedSingle
        panelPreview.Controls.Add(lblPreview)
        panelPreview.Location = New Drawing.Point(301, 3)
        panelPreview.Name = "panelPreview"
        panelPreview.Size = New Drawing.Size(150, 29)
        panelPreview.TabIndex = 12
        ' 
        ' lblPreview
        ' 
        lblPreview.AutoSize = True
        lblPreview.Font = New Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
        lblPreview.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        lblPreview.Location = New Drawing.Point(3, 4)
        lblPreview.Name = "lblPreview"
        lblPreview.Size = New Drawing.Size(59, 18)
        lblPreview.TabIndex = 0
        lblPreview.Text = "Preview"
        ' 
        ' panelMenu
        ' 
        panelMenu.Anchor = Windows.Forms.AnchorStyles.Top Or Windows.Forms.AnchorStyles.Left Or Windows.Forms.AnchorStyles.Right
        panelMenu.BackColor = System.Drawing.Color.LightSteelBlue
        panelMenu.BorderStyle = Windows.Forms.BorderStyle.FixedSingle
        panelMenu.Controls.Add(btnMode)
        panelMenu.Controls.Add(btnPreview)
        panelMenu.Controls.Add(cbZoom)
        panelMenu.Controls.Add(btnFitToScreen)
        panelMenu.Controls.Add(btnZoomIn)
        panelMenu.Controls.Add(btnZoomOut)
        panelMenu.Controls.Add(btnRotate270)
        panelMenu.Controls.Add(btnRotate90)
        panelMenu.Controls.Add(btnOpen)
        panelMenu.Location = New Drawing.Point(2, 3)
        panelMenu.Name = "panelMenu"
        panelMenu.Size = New Drawing.Size(295, 29)
        panelMenu.TabIndex = 11
        ' 
        ' btnMode
        ' 
        '  btnMode.Image = Global.KaiwaProjects.Properties.Resources.btnSelect
        btnMode.Location = New Drawing.Point(142, 1)
        btnMode.Name = "btnMode"
        btnMode.Size = New Drawing.Size(25, 25)
        btnMode.TabIndex = 16
        btnMode.UseVisualStyleBackColor = True
        AddHandler btnMode.Click, New EventHandler(AddressOf btnMode_Click)
        ' 
        ' btnPreview
        ' 
        ' btnPreview.Image = Global.KaiwaProjects.Properties.Resources.btnPreview
        btnPreview.Location = New Drawing.Point(198, 1)
        btnPreview.Name = "btnPreview"
        btnPreview.Size = New Drawing.Size(25, 25)
        btnPreview.TabIndex = 15
        btnPreview.UseVisualStyleBackColor = True
        btnPreview.Visible = False
        AddHandler btnPreview.Click, New EventHandler(AddressOf btnPreview_Click)
        ' 
        ' cbZoom
        ' 
        cbZoom.Anchor = Windows.Forms.AnchorStyles.Top Or Windows.Forms.AnchorStyles.Right
        cbZoom.FormattingEnabled = True
        cbZoom.Location = New Drawing.Point(228, 3)
        cbZoom.Name = "cbZoom"
        cbZoom.Size = New Drawing.Size(62, 21)
        cbZoom.TabIndex = 14
        AddHandler cbZoom.SelectedIndexChanged, New EventHandler(AddressOf cbZoom_SelectedIndexChanged)
        AddHandler cbZoom.KeyPress, New Windows.Forms.KeyPressEventHandler(AddressOf cbZoom_KeyPress)
        ' 
        ' btnFitToScreen
        ' 
        ' btnFitToScreen.Image = Global.KaiwaProjects.Properties.Resources.btnFitToScreen
        btnFitToScreen.Location = New Drawing.Point(58, 1)
        btnFitToScreen.Name = "btnFitToScreen"
        btnFitToScreen.Size = New Drawing.Size(25, 25)
        btnFitToScreen.TabIndex = 13
        btnFitToScreen.UseVisualStyleBackColor = True
        AddHandler btnFitToScreen.Click, New EventHandler(AddressOf btnFitToScreen_Click)
        ' 
        ' btnZoomIn
        ' 
        ' btnZoomIn.Image = Global.KaiwaProjects.Properties.Resources.btnZoomIn
        btnZoomIn.Location = New Drawing.Point(2, 1)
        btnZoomIn.Name = "btnZoomIn"
        btnZoomIn.Size = New Drawing.Size(25, 25)
        btnZoomIn.TabIndex = 12
        btnZoomIn.UseVisualStyleBackColor = True
        AddHandler btnZoomIn.Click, New EventHandler(AddressOf btnZoomIn_Click)
        ' 
        ' btnZoomOut
        ' 
        ' btnZoomOut.Image = Global.KaiwaProjects.Properties.Resources.btnZoomOut
        btnZoomOut.Location = New Drawing.Point(30, 1)
        btnZoomOut.Name = "btnZoomOut"
        btnZoomOut.Size = New Drawing.Size(25, 25)
        btnZoomOut.TabIndex = 11
        btnZoomOut.UseVisualStyleBackColor = True
        AddHandler btnZoomOut.Click, New EventHandler(AddressOf btnZoomOut_Click)
        ' 
        ' btnRotate270
        ' 
        ' btnRotate270.Image = Global.KaiwaProjects.Properties.Resources.btnRotate270
        btnRotate270.Location = New Drawing.Point(86, 1)
        btnRotate270.Name = "btnRotate270"
        btnRotate270.Size = New Drawing.Size(25, 25)
        btnRotate270.TabIndex = 10
        btnRotate270.UseVisualStyleBackColor = True
        AddHandler btnRotate270.Click, New EventHandler(AddressOf btnRotate270_Click)
        ' 
        ' btnRotate90
        ' 
        ' btnRotate90.Image = Global.KaiwaProjects.Properties.Resources.btnRotate90
        btnRotate90.Location = New Drawing.Point(114, 1)
        btnRotate90.Name = "btnRotate90"
        btnRotate90.Size = New Drawing.Size(25, 25)
        btnRotate90.TabIndex = 9
        btnRotate90.UseVisualStyleBackColor = True
        AddHandler btnRotate90.Click, New EventHandler(AddressOf btnRotate90_Click)
        ' 
        ' btnOpen
        ' 
        ' btnOpen.Image = Global.KaiwaProjects.Properties.Resources.btnOpen
        btnOpen.Location = New Drawing.Point(170, 1)
        btnOpen.Name = "btnOpen"
        btnOpen.Size = New Drawing.Size(25, 25)
        btnOpen.TabIndex = 8
        btnOpen.UseVisualStyleBackColor = True
        btnOpen.Visible = False
        AddHandler btnOpen.Click, New EventHandler(AddressOf btnOpen_Click)
        ' 
        ' panelNavigation
        ' 
        panelNavigation.Anchor = Windows.Forms.AnchorStyles.Top Or Windows.Forms.AnchorStyles.Right
        panelNavigation.BackColor = System.Drawing.Color.LightSteelBlue
        panelNavigation.BorderStyle = Windows.Forms.BorderStyle.FixedSingle
        panelNavigation.Controls.Add(lblNavigation)
        panelNavigation.Controls.Add(tbNavigation)
        panelNavigation.Controls.Add(btnBack)
        panelNavigation.Controls.Add(btnNext)
        panelNavigation.Location = New Drawing.Point(301, 157)
        panelNavigation.Name = "panelNavigation"
        panelNavigation.Size = New Drawing.Size(150, 29)
        panelNavigation.TabIndex = 13
        panelNavigation.Visible = False
        ' 
        ' lblNavigation
        ' 
        lblNavigation.AutoSize = True
        lblNavigation.Font = New Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0)
        lblNavigation.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        lblNavigation.Location = New Drawing.Point(41, 5)
        lblNavigation.Name = "lblNavigation"
        lblNavigation.Size = New Drawing.Size(24, 18)
        lblNavigation.TabIndex = 1
        lblNavigation.Text = "/ 0"
        ' 
        ' tbNavigation
        ' 
        tbNavigation.Location = New Drawing.Point(4, 4)
        tbNavigation.Name = "tbNavigation"
        tbNavigation.Size = New Drawing.Size(33, 20)
        tbNavigation.TabIndex = 19
        tbNavigation.Text = "0"
        tbNavigation.TextAlign = Windows.Forms.HorizontalAlignment.Center
        AddHandler tbNavigation.KeyPress, New Windows.Forms.KeyPressEventHandler(AddressOf tbNavigation_KeyPress)
        ' 
        ' btnBack
        ' 
        ' btnBack.Image = Global.KaiwaProjects.Properties.Resources.btnBack
        btnBack.Location = New Drawing.Point(93, 1)
        btnBack.Name = "btnBack"
        btnBack.Size = New Drawing.Size(25, 25)
        btnBack.TabIndex = 18
        btnBack.UseVisualStyleBackColor = True
        AddHandler btnBack.Click, New EventHandler(AddressOf btnBack_Click)
        ' 
        ' btnNext
        ' 
        ' btnNext.Image = Global.KaiwaProjects.Properties.Resources.btnNext
        btnNext.Location = New Drawing.Point(121, 1)
        btnNext.Name = "btnNext"
        btnNext.Size = New Drawing.Size(25, 25)
        btnNext.TabIndex = 17
        btnNext.UseVisualStyleBackColor = True
        AddHandler btnNext.Click, New EventHandler(AddressOf btnNext_Click)
        ' 
        ' pbPanel
        ' 
        pbPanel.Anchor = Windows.Forms.AnchorStyles.Top Or Windows.Forms.AnchorStyles.Right
        pbPanel.Location = New Drawing.Point(302, 36)
        pbPanel.Name = "pbPanel"
        pbPanel.Size = New Drawing.Size(148, 117)
        pbPanel.SizeMode = Windows.Forms.PictureBoxSizeMode.Zoom
        pbPanel.TabIndex = 10
        pbPanel.TabStop = False
        AddHandler pbPanel.MouseDown, New Windows.Forms.MouseEventHandler(AddressOf pbPanel_MouseDown)
        AddHandler pbPanel.MouseMove, New Windows.Forms.MouseEventHandler(AddressOf pbPanel_MouseMove)
        AddHandler pbPanel.MouseUp, New Windows.Forms.MouseEventHandler(AddressOf pbPanel_MouseUp)
        ' 
        ' pbFull
        ' 
        pbFull.Anchor = Windows.Forms.AnchorStyles.Top Or Windows.Forms.AnchorStyles.Bottom Or Windows.Forms.AnchorStyles.Left Or Windows.Forms.AnchorStyles.Right
        pbFull.BackColor = System.Drawing.SystemColors.ControlLight
        pbFull.BorderStyle = Windows.Forms.BorderStyle.FixedSingle
        pbFull.Location = New Drawing.Point(2, 36)
        pbFull.Name = "pbFull"
        pbFull.Size = New Drawing.Size(295, 271)
        pbFull.TabIndex = 13
        AddHandler pbFull.Click, New EventHandler(AddressOf pbFull_Click)
        AddHandler pbFull.DragDrop, New Windows.Forms.DragEventHandler(AddressOf pbFull_DragDrop)
        AddHandler pbFull.DragEnter, New Windows.Forms.DragEventHandler(AddressOf pbFull_DragEnter)
        AddHandler pbFull.Paint, New Windows.Forms.PaintEventHandler(AddressOf pbFull_Paint)
        AddHandler pbFull.MouseDoubleClick, New Windows.Forms.MouseEventHandler(AddressOf pbFull_MouseDoubleClick)
        AddHandler pbFull.MouseDown, New Windows.Forms.MouseEventHandler(AddressOf pbFull_MouseDown)
        AddHandler pbFull.MouseEnter, New EventHandler(AddressOf pbFull_MouseEnter)
        AddHandler pbFull.MouseLeave, New EventHandler(AddressOf pbFull_MouseLeave)
        AddHandler pbFull.MouseHover, New EventHandler(AddressOf pbFull_MouseHover)
        AddHandler pbFull.MouseMove, New Windows.Forms.MouseEventHandler(AddressOf pbFull_MouseMove)
        AddHandler pbFull.MouseUp, New Windows.Forms.MouseEventHandler(AddressOf pbFull_MouseUp)
        ' 
        ' KpImageViewer
        ' 
        AutoScaleDimensions = New Drawing.SizeF(6.0F, 13.0F)
        AutoScaleMode = Windows.Forms.AutoScaleMode.Font
        Controls.Add(panelNavigation)
        Controls.Add(pbFull)
        Controls.Add(panelPreview)
        Controls.Add(panelMenu)
        Controls.Add(pbPanel)
        MinimumSize = New Drawing.Size(454, 157)
        Name = "KpImageViewer"
        Size = New Drawing.Size(454, 310)
        AddHandler Load, New EventHandler(AddressOf KP_ImageViewerV2_Load)
        AddHandler Click, New EventHandler(AddressOf KpImageViewer_Click)
        AddHandler MouseWheel, New Windows.Forms.MouseEventHandler(AddressOf KP_ImageViewerV2_MouseWheel)
        AddHandler Resize, New EventHandler(AddressOf KP_ImageViewerV2_Resize)
        panelPreview.ResumeLayout(False)
        panelPreview.PerformLayout()
        panelMenu.ResumeLayout(False)
        panelNavigation.ResumeLayout(False)
        panelNavigation.PerformLayout()
        CType(pbPanel, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)

    End Sub

#End Region

    Private panelPreview As Windows.Forms.Panel
    Private lblPreview As Windows.Forms.Label
    Private panelMenu As Windows.Forms.Panel
    Private pbPanel As Windows.Forms.PictureBox
    Private pbFull As PanelDoubleBuffered
    Private btnOpen As Windows.Forms.Button
    Private btnRotate270 As Windows.Forms.Button
    Private btnRotate90 As Windows.Forms.Button
    Private btnZoomIn As Windows.Forms.Button
    Private btnZoomOut As Windows.Forms.Button
    Private btnFitToScreen As Windows.Forms.Button
    Private cbZoom As Windows.Forms.ComboBox
    Private btnPreview As Windows.Forms.Button
    Private btnMode As Windows.Forms.Button
    Private panelNavigation As Windows.Forms.Panel
    Private btnBack As Windows.Forms.Button
    Private btnNext As Windows.Forms.Button
    Private lblNavigation As Windows.Forms.Label
    Private tbNavigation As Windows.Forms.TextBox
End Class
