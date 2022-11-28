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
        Me.pbPanelAirscape = New System.Windows.Forms.PictureBox()
        Me.pbFull = New Mzkit_win32.MSImagingViewerV2.PanelDoubleBuffered()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripComboBox1 = New System.Windows.Forms.ToolStripComboBox()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButtonZoomIn = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButtonZoomOut = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButtonFitToScreen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButtonRotate270 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButtonRotate90 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButtonMode = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButtonPreview = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripComboBoxZoom = New System.Windows.Forms.ToolStripComboBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel2 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.MSICanvas = New Mzkit_win32.MSImagingViewerV2.PixelSelector()
        CType(Me.pbPanelAirscape, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pbFull.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        CType(Me.MSICanvas, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pbPanelAirscape
        '
        Me.pbPanelAirscape.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbPanelAirscape.Location = New System.Drawing.Point(485, 3)
        Me.pbPanelAirscape.Name = "pbPanelAirscape"
        Me.pbPanelAirscape.Size = New System.Drawing.Size(148, 108)
        Me.pbPanelAirscape.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pbPanelAirscape.TabIndex = 10
        Me.pbPanelAirscape.TabStop = False
        '
        'pbFull
        '
        Me.pbFull.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pbFull.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pbFull.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pbFull.Controls.Add(Me.pbPanelAirscape)
        Me.pbFull.Location = New System.Drawing.Point(0, 25)
        Me.pbFull.Name = "pbFull"
        Me.pbFull.Size = New System.Drawing.Size(638, 370)
        Me.pbFull.TabIndex = 13
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.ToolStripComboBox1, Me.ToolStripSeparator1, Me.ToolStripButtonZoomIn, Me.ToolStripButtonZoomOut, Me.ToolStripButtonFitToScreen, Me.ToolStripButtonRotate270, Me.ToolStripButtonRotate90, Me.ToolStripButtonMode, Me.ToolStripButtonPreview, Me.ToolStripComboBoxZoom})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(638, 25)
        Me.ToolStrip1.TabIndex = 14
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(77, 22)
        Me.ToolStripLabel1.Text = "View Sample:"
        '
        'ToolStripComboBox1
        '
        Me.ToolStripComboBox1.Name = "ToolStripComboBox1"
        Me.ToolStripComboBox1.Size = New System.Drawing.Size(121, 25)
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripButtonZoomIn
        '
        Me.ToolStripButtonZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButtonZoomIn.Image = Global.Mzkit_win32.MSImagingViewerV2.Resources.btnZoomIn
        Me.ToolStripButtonZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButtonZoomIn.Name = "ToolStripButtonZoomIn"
        Me.ToolStripButtonZoomIn.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButtonZoomIn.Text = "Zoom In"
        '
        'ToolStripButtonZoomOut
        '
        Me.ToolStripButtonZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButtonZoomOut.Image = Global.Mzkit_win32.MSImagingViewerV2.Resources.btnZoomOut
        Me.ToolStripButtonZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButtonZoomOut.Name = "ToolStripButtonZoomOut"
        Me.ToolStripButtonZoomOut.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButtonZoomOut.Text = "Zoom Out"
        '
        'ToolStripButtonFitToScreen
        '
        Me.ToolStripButtonFitToScreen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButtonFitToScreen.Image = Global.Mzkit_win32.MSImagingViewerV2.Resources.btnFitToScreen
        Me.ToolStripButtonFitToScreen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButtonFitToScreen.Name = "ToolStripButtonFitToScreen"
        Me.ToolStripButtonFitToScreen.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButtonFitToScreen.Text = "Fit To Screen"
        '
        'ToolStripButtonRotate270
        '
        Me.ToolStripButtonRotate270.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButtonRotate270.Image = Global.Mzkit_win32.MSImagingViewerV2.Resources.btnRotate270
        Me.ToolStripButtonRotate270.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButtonRotate270.Name = "ToolStripButtonRotate270"
        Me.ToolStripButtonRotate270.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButtonRotate270.Text = "Rotate 270"
        '
        'ToolStripButtonRotate90
        '
        Me.ToolStripButtonRotate90.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButtonRotate90.Image = Global.Mzkit_win32.MSImagingViewerV2.Resources.btnRotate90
        Me.ToolStripButtonRotate90.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButtonRotate90.Name = "ToolStripButtonRotate90"
        Me.ToolStripButtonRotate90.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButtonRotate90.Text = "Rotate 90"
        '
        'ToolStripButtonMode
        '
        Me.ToolStripButtonMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButtonMode.Image = Global.Mzkit_win32.MSImagingViewerV2.Resources.btnDrag
        Me.ToolStripButtonMode.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButtonMode.Name = "ToolStripButtonMode"
        Me.ToolStripButtonMode.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButtonMode.Text = "Mode"
        '
        'ToolStripButtonPreview
        '
        Me.ToolStripButtonPreview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButtonPreview.Image = Global.Mzkit_win32.MSImagingViewerV2.Resources.btnPreview
        Me.ToolStripButtonPreview.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButtonPreview.Name = "ToolStripButtonPreview"
        Me.ToolStripButtonPreview.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButtonPreview.Text = "Preview"
        '
        'ToolStripComboBoxZoom
        '
        Me.ToolStripComboBoxZoom.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripComboBoxZoom.Name = "ToolStripComboBoxZoom"
        Me.ToolStripComboBoxZoom.Size = New System.Drawing.Size(121, 25)
        '
        'StatusStrip1
        '
        Me.StatusStrip1.BackColor = System.Drawing.Color.WhiteSmoke
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1, Me.ToolStripStatusLabel2})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 395)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(638, 22)
        Me.StatusStrip1.TabIndex = 15
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(119, 17)
        Me.ToolStripStatusLabel1.Text = "ToolStripStatusLabel1"
        '
        'ToolStripStatusLabel2
        '
        Me.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        Me.ToolStripStatusLabel2.Size = New System.Drawing.Size(57, 17)
        Me.ToolStripStatusLabel2.Text = "[n/a, n/a]"
        '
        'MSICanvas
        '
        Me.MSICanvas.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.MSICanvas.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.MSICanvas.HEMap = Nothing
        Me.MSICanvas.Location = New System.Drawing.Point(311, 401)
        Me.MSICanvas.Name = "MSICanvas"
        Me.MSICanvas.SelectPolygonMode = False
        Me.MSICanvas.ShowPointInform = False
        Me.MSICanvas.Size = New System.Drawing.Size(10, 10)
        Me.MSICanvas.TabIndex = 11
        Me.MSICanvas.TabStop = False
        Me.MSICanvas.tissue_layer = Nothing
        Me.MSICanvas.ViewerHost = Nothing
        '
        'KpImageViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.MSICanvas)
        Me.Controls.Add(Me.pbFull)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.MinimumSize = New System.Drawing.Size(454, 145)
        Me.Name = "KpImageViewer"
        Me.Size = New System.Drawing.Size(638, 417)
        CType(Me.pbPanelAirscape, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pbFull.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        CType(Me.MSICanvas, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    ''' <summary>
    ''' 鸟瞰图
    ''' </summary>
    Private WithEvents pbPanelAirscape As Windows.Forms.PictureBox
    Private WithEvents pbFull As PanelDoubleBuffered
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents ToolStripComboBox1 As ToolStripComboBox
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripButtonZoomIn As ToolStripButton
    Friend WithEvents ToolStripButtonZoomOut As ToolStripButton
    Friend WithEvents ToolStripButtonFitToScreen As ToolStripButton
    Friend WithEvents ToolStripButtonRotate270 As ToolStripButton
    Friend WithEvents ToolStripButtonRotate90 As ToolStripButton
    Friend WithEvents ToolStripButtonMode As ToolStripButton
    Friend WithEvents ToolStripButtonPreview As ToolStripButton
    Friend WithEvents ToolStripComboBoxZoom As ToolStripComboBox
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel2 As ToolStripStatusLabel
    Friend WithEvents MSICanvas As PixelSelector
End Class
