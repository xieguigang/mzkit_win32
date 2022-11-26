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
        Me.panelPreview = New System.Windows.Forms.Panel()
        Me.lblPreview = New System.Windows.Forms.Label()
        Me.panelMenu = New System.Windows.Forms.Panel()
        Me.btnMode = New System.Windows.Forms.Button()
        Me.btnPreview = New System.Windows.Forms.Button()
        Me.cbZoom = New System.Windows.Forms.ComboBox()
        Me.btnFitToScreen = New System.Windows.Forms.Button()
        Me.btnZoomIn = New System.Windows.Forms.Button()
        Me.btnZoomOut = New System.Windows.Forms.Button()
        Me.btnRotate270 = New System.Windows.Forms.Button()
        Me.btnRotate90 = New System.Windows.Forms.Button()
        Me.btnOpen = New System.Windows.Forms.Button()
        Me.panelNavigation = New System.Windows.Forms.Panel()
        Me.lblNavigation = New System.Windows.Forms.Label()
        Me.tbNavigation = New System.Windows.Forms.TextBox()
        Me.btnBack = New System.Windows.Forms.Button()
        Me.btnNext = New System.Windows.Forms.Button()
        Me.pbPanel = New System.Windows.Forms.PictureBox()
        Me.pbFull = New Mzkit_win32.MSImagingViewerV2.PanelDoubleBuffered()
        Me.panelPreview.SuspendLayout()
        Me.panelMenu.SuspendLayout()
        Me.panelNavigation.SuspendLayout()
        CType(Me.pbPanel, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'panelPreview
        '
        Me.panelPreview.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.panelPreview.BackColor = System.Drawing.Color.LightSteelBlue
        Me.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.panelPreview.Controls.Add(Me.lblPreview)
        Me.panelPreview.Location = New System.Drawing.Point(301, 3)
        Me.panelPreview.Name = "panelPreview"
        Me.panelPreview.Size = New System.Drawing.Size(150, 29)
        Me.panelPreview.TabIndex = 12
        '
        'lblPreview
        '
        Me.lblPreview.AutoSize = True
        Me.lblPreview.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPreview.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblPreview.Location = New System.Drawing.Point(3, 4)
        Me.lblPreview.Name = "lblPreview"
        Me.lblPreview.Size = New System.Drawing.Size(59, 18)
        Me.lblPreview.TabIndex = 0
        Me.lblPreview.Text = "Preview"
        '
        'panelMenu
        '
        Me.panelMenu.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.panelMenu.BackColor = System.Drawing.Color.LightSteelBlue
        Me.panelMenu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.panelMenu.Controls.Add(Me.btnMode)
        Me.panelMenu.Controls.Add(Me.btnPreview)
        Me.panelMenu.Controls.Add(Me.cbZoom)
        Me.panelMenu.Controls.Add(Me.btnFitToScreen)
        Me.panelMenu.Controls.Add(Me.btnZoomIn)
        Me.panelMenu.Controls.Add(Me.btnZoomOut)
        Me.panelMenu.Controls.Add(Me.btnRotate270)
        Me.panelMenu.Controls.Add(Me.btnRotate90)
        Me.panelMenu.Controls.Add(Me.btnOpen)
        Me.panelMenu.Location = New System.Drawing.Point(2, 3)
        Me.panelMenu.Name = "panelMenu"
        Me.panelMenu.Size = New System.Drawing.Size(295, 29)
        Me.panelMenu.TabIndex = 11
        '
        'btnMode
        '
        Me.btnMode.Location = New System.Drawing.Point(142, 1)
        Me.btnMode.Name = "btnMode"
        Me.btnMode.Size = New System.Drawing.Size(25, 25)
        Me.btnMode.TabIndex = 16
        Me.btnMode.UseVisualStyleBackColor = True
        '
        'btnPreview
        '
        Me.btnPreview.Location = New System.Drawing.Point(198, 1)
        Me.btnPreview.Name = "btnPreview"
        Me.btnPreview.Size = New System.Drawing.Size(25, 25)
        Me.btnPreview.TabIndex = 15
        Me.btnPreview.UseVisualStyleBackColor = True
        Me.btnPreview.Visible = False
        '
        'cbZoom
        '
        Me.cbZoom.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbZoom.FormattingEnabled = True
        Me.cbZoom.Location = New System.Drawing.Point(228, 3)
        Me.cbZoom.Name = "cbZoom"
        Me.cbZoom.Size = New System.Drawing.Size(62, 21)
        Me.cbZoom.TabIndex = 14
        '
        'btnFitToScreen
        '
        Me.btnFitToScreen.Location = New System.Drawing.Point(58, 1)
        Me.btnFitToScreen.Name = "btnFitToScreen"
        Me.btnFitToScreen.Size = New System.Drawing.Size(25, 25)
        Me.btnFitToScreen.TabIndex = 13
        Me.btnFitToScreen.UseVisualStyleBackColor = True
        '
        'btnZoomIn
        '
        Me.btnZoomIn.Location = New System.Drawing.Point(2, 1)
        Me.btnZoomIn.Name = "btnZoomIn"
        Me.btnZoomIn.Size = New System.Drawing.Size(25, 25)
        Me.btnZoomIn.TabIndex = 12
        Me.btnZoomIn.UseVisualStyleBackColor = True
        '
        'btnZoomOut
        '
        Me.btnZoomOut.Location = New System.Drawing.Point(30, 1)
        Me.btnZoomOut.Name = "btnZoomOut"
        Me.btnZoomOut.Size = New System.Drawing.Size(25, 25)
        Me.btnZoomOut.TabIndex = 11
        Me.btnZoomOut.UseVisualStyleBackColor = True
        '
        'btnRotate270
        '
        Me.btnRotate270.Location = New System.Drawing.Point(86, 1)
        Me.btnRotate270.Name = "btnRotate270"
        Me.btnRotate270.Size = New System.Drawing.Size(25, 25)
        Me.btnRotate270.TabIndex = 10
        Me.btnRotate270.UseVisualStyleBackColor = True
        '
        'btnRotate90
        '
        Me.btnRotate90.Location = New System.Drawing.Point(114, 1)
        Me.btnRotate90.Name = "btnRotate90"
        Me.btnRotate90.Size = New System.Drawing.Size(25, 25)
        Me.btnRotate90.TabIndex = 9
        Me.btnRotate90.UseVisualStyleBackColor = True
        '
        'btnOpen
        '
        Me.btnOpen.Location = New System.Drawing.Point(170, 1)
        Me.btnOpen.Name = "btnOpen"
        Me.btnOpen.Size = New System.Drawing.Size(25, 25)
        Me.btnOpen.TabIndex = 8
        Me.btnOpen.UseVisualStyleBackColor = True
        Me.btnOpen.Visible = False
        '
        'panelNavigation
        '
        Me.panelNavigation.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.panelNavigation.BackColor = System.Drawing.Color.LightSteelBlue
        Me.panelNavigation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.panelNavigation.Controls.Add(Me.lblNavigation)
        Me.panelNavigation.Controls.Add(Me.tbNavigation)
        Me.panelNavigation.Controls.Add(Me.btnBack)
        Me.panelNavigation.Controls.Add(Me.btnNext)
        Me.panelNavigation.Location = New System.Drawing.Point(301, 157)
        Me.panelNavigation.Name = "panelNavigation"
        Me.panelNavigation.Size = New System.Drawing.Size(150, 29)
        Me.panelNavigation.TabIndex = 13
        Me.panelNavigation.Visible = False
        '
        'lblNavigation
        '
        Me.lblNavigation.AutoSize = True
        Me.lblNavigation.Font = New System.Drawing.Font("Calibri", 11.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNavigation.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.lblNavigation.Location = New System.Drawing.Point(41, 5)
        Me.lblNavigation.Name = "lblNavigation"
        Me.lblNavigation.Size = New System.Drawing.Size(24, 18)
        Me.lblNavigation.TabIndex = 1
        Me.lblNavigation.Text = "/ 0"
        '
        'tbNavigation
        '
        Me.tbNavigation.Location = New System.Drawing.Point(4, 4)
        Me.tbNavigation.Name = "tbNavigation"
        Me.tbNavigation.Size = New System.Drawing.Size(33, 20)
        Me.tbNavigation.TabIndex = 19
        Me.tbNavigation.Text = "0"
        Me.tbNavigation.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'btnBack
        '
        Me.btnBack.Location = New System.Drawing.Point(93, 1)
        Me.btnBack.Name = "btnBack"
        Me.btnBack.Size = New System.Drawing.Size(25, 25)
        Me.btnBack.TabIndex = 18
        Me.btnBack.UseVisualStyleBackColor = True
        '
        'btnNext
        '
        Me.btnNext.Location = New System.Drawing.Point(121, 1)
        Me.btnNext.Name = "btnNext"
        Me.btnNext.Size = New System.Drawing.Size(25, 25)
        Me.btnNext.TabIndex = 17
        Me.btnNext.UseVisualStyleBackColor = True
        '
        'pbPanel
        '
        Me.pbPanel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbPanel.Location = New System.Drawing.Point(302, 36)
        Me.pbPanel.Name = "pbPanel"
        Me.pbPanel.Size = New System.Drawing.Size(148, 117)
        Me.pbPanel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pbPanel.TabIndex = 10
        Me.pbPanel.TabStop = False
        '
        'pbFull
        '
        Me.pbFull.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbFull.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pbFull.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbFull.Location = New System.Drawing.Point(2, 36)
        Me.pbFull.Name = "pbFull"
        Me.pbFull.Size = New System.Drawing.Size(295, 271)
        Me.pbFull.TabIndex = 13
        '
        'KpImageViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.panelNavigation)
        Me.Controls.Add(Me.pbFull)
        Me.Controls.Add(Me.panelPreview)
        Me.Controls.Add(Me.panelMenu)
        Me.Controls.Add(Me.pbPanel)
        Me.MinimumSize = New System.Drawing.Size(454, 157)
        Me.Name = "KpImageViewer"
        Me.Size = New System.Drawing.Size(454, 310)
        Me.panelPreview.ResumeLayout(False)
        Me.panelPreview.PerformLayout()
        Me.panelMenu.ResumeLayout(False)
        Me.panelNavigation.ResumeLayout(False)
        Me.panelNavigation.PerformLayout()
        CType(Me.pbPanel, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

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
