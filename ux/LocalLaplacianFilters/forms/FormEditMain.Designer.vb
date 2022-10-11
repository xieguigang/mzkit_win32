
Partial Class FormEditMain
    ''' <summary>
    ''' Требуется переменная конструктора.
    ''' </summary>
    Private components As ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Освободить все используемые ресурсы.
    ''' </summary>
    ''' <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Код, автоматически созданный конструктором форм Windows"

    ''' <summary>
    ''' Обязательный метод для поддержки конструктора - не изменяйте
    ''' содержимое данного метода при помощи редактора кода.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormEditMain))
        Me.menuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.openToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.reloadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.saveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.closeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.editToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.flipVerticalToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.flipHorizontalToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.undoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.redoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.filterToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.localLaplacianToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.exposureToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.temperatureToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.programToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.aboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.pictureBox1 = New System.Windows.Forms.PictureBox()
        Me.comboBox1 = New System.Windows.Forms.ComboBox()
        Me.label1 = New System.Windows.Forms.Label()
        Me.label4 = New System.Windows.Forms.Label()
        Me.label3 = New System.Windows.Forms.Label()
        Me.label2 = New System.Windows.Forms.Label()
        Me.label5 = New System.Windows.Forms.Label()
        Me.label6 = New System.Windows.Forms.Label()
        Me.label7 = New System.Windows.Forms.Label()
        Me.panel1 = New System.Windows.Forms.Panel()
        Me.textBox1 = New System.Windows.Forms.TextBox()
        Me.label16 = New System.Windows.Forms.Label()
        Me.button1 = New System.Windows.Forms.Button()
        Me.label15 = New System.Windows.Forms.Label()
        Me.button2 = New System.Windows.Forms.Button()
        Me.label12 = New System.Windows.Forms.Label()
        Me.label10 = New System.Windows.Forms.Label()
        Me.trackBar3 = New System.Windows.Forms.TrackBar()
        Me.trackBar2 = New System.Windows.Forms.TrackBar()
        Me.textBox3 = New System.Windows.Forms.TextBox()
        Me.textBox2 = New System.Windows.Forms.TextBox()
        Me.label13 = New System.Windows.Forms.Label()
        Me.label11 = New System.Windows.Forms.Label()
        Me.trackBar4 = New System.Windows.Forms.TrackBar()
        Me.trackBar1 = New System.Windows.Forms.TrackBar()
        Me.textBox4 = New System.Windows.Forms.TextBox()
        Me.label14 = New System.Windows.Forms.Label()
        Me.checkBox1 = New System.Windows.Forms.CheckBox()
        Me.trackBar5 = New System.Windows.Forms.TrackBar()
        Me.textBox5 = New System.Windows.Forms.TextBox()
        Me.label9 = New System.Windows.Forms.Label()
        Me.label8 = New System.Windows.Forms.Label()
        Me.comboBox2 = New System.Windows.Forms.ComboBox()
        Me.histogram1 = New LaplacianHDR.Histogram()
        Me.histogram2 = New LaplacianHDR.Histogram()
        Me.histogram3 = New LaplacianHDR.Histogram()
        Me.histogram4 = New LaplacianHDR.Histogram()
        Me.VisualStudioToolStripExtender1 = New WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender(Me.components)
        Me.VS2015BlueTheme1 = New WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme()
        Me.menuStrip1.SuspendLayout()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.panel1.SuspendLayout()
        CType(Me.trackBar3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'menuStrip1
        '
        Me.menuStrip1.BackColor = System.Drawing.SystemColors.Control
        Me.menuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem, Me.editToolStripMenuItem, Me.filterToolStripMenuItem, Me.programToolStripMenuItem})
        Me.menuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.menuStrip1.Name = "menuStrip1"
        Me.menuStrip1.Size = New System.Drawing.Size(1355, 24)
        Me.menuStrip1.TabIndex = 0
        Me.menuStrip1.Text = "menuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.openToolStripMenuItem, Me.reloadToolStripMenuItem, Me.saveToolStripMenuItem, Me.closeToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
        Me.fileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.fileToolStripMenuItem.Text = "File"
        '
        'openToolStripMenuItem
        '
        Me.openToolStripMenuItem.Name = "openToolStripMenuItem"
        Me.openToolStripMenuItem.Size = New System.Drawing.Size(110, 22)
        Me.openToolStripMenuItem.Text = "Open"
        '
        'reloadToolStripMenuItem
        '
        Me.reloadToolStripMenuItem.Enabled = False
        Me.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem"
        Me.reloadToolStripMenuItem.Size = New System.Drawing.Size(110, 22)
        Me.reloadToolStripMenuItem.Text = "Reload"
        '
        'saveToolStripMenuItem
        '
        Me.saveToolStripMenuItem.Enabled = False
        Me.saveToolStripMenuItem.Name = "saveToolStripMenuItem"
        Me.saveToolStripMenuItem.Size = New System.Drawing.Size(110, 22)
        Me.saveToolStripMenuItem.Text = "Save"
        '
        'closeToolStripMenuItem
        '
        Me.closeToolStripMenuItem.Enabled = False
        Me.closeToolStripMenuItem.Name = "closeToolStripMenuItem"
        Me.closeToolStripMenuItem.Size = New System.Drawing.Size(110, 22)
        Me.closeToolStripMenuItem.Text = "Close"
        '
        'editToolStripMenuItem
        '
        Me.editToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.flipVerticalToolStripMenuItem, Me.flipHorizontalToolStripMenuItem, Me.undoToolStripMenuItem, Me.redoToolStripMenuItem})
        Me.editToolStripMenuItem.Name = "editToolStripMenuItem"
        Me.editToolStripMenuItem.Size = New System.Drawing.Size(39, 20)
        Me.editToolStripMenuItem.Text = "Edit"
        '
        'flipVerticalToolStripMenuItem
        '
        Me.flipVerticalToolStripMenuItem.Enabled = False
        Me.flipVerticalToolStripMenuItem.Name = "flipVerticalToolStripMenuItem"
        Me.flipVerticalToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.flipVerticalToolStripMenuItem.Text = "Flip vertical"
        '
        'flipHorizontalToolStripMenuItem
        '
        Me.flipHorizontalToolStripMenuItem.Enabled = False
        Me.flipHorizontalToolStripMenuItem.Name = "flipHorizontalToolStripMenuItem"
        Me.flipHorizontalToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.flipHorizontalToolStripMenuItem.Text = "Flip horizontal"
        '
        'undoToolStripMenuItem
        '
        Me.undoToolStripMenuItem.Enabled = False
        Me.undoToolStripMenuItem.Name = "undoToolStripMenuItem"
        Me.undoToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.undoToolStripMenuItem.Text = "Undo"
        '
        'redoToolStripMenuItem
        '
        Me.redoToolStripMenuItem.Enabled = False
        Me.redoToolStripMenuItem.Name = "redoToolStripMenuItem"
        Me.redoToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.redoToolStripMenuItem.Text = "Redo"
        '
        'filterToolStripMenuItem
        '
        Me.filterToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.localLaplacianToolStripMenuItem, Me.exposureToolStripMenuItem, Me.temperatureToolStripMenuItem})
        Me.filterToolStripMenuItem.Name = "filterToolStripMenuItem"
        Me.filterToolStripMenuItem.Size = New System.Drawing.Size(45, 20)
        Me.filterToolStripMenuItem.Text = "Filter"
        '
        'localLaplacianToolStripMenuItem
        '
        Me.localLaplacianToolStripMenuItem.Enabled = False
        Me.localLaplacianToolStripMenuItem.Name = "localLaplacianToolStripMenuItem"
        Me.localLaplacianToolStripMenuItem.Size = New System.Drawing.Size(210, 22)
        Me.localLaplacianToolStripMenuItem.Text = "Enhancement/Details"
        '
        'exposureToolStripMenuItem
        '
        Me.exposureToolStripMenuItem.Enabled = False
        Me.exposureToolStripMenuItem.Name = "exposureToolStripMenuItem"
        Me.exposureToolStripMenuItem.Size = New System.Drawing.Size(210, 22)
        Me.exposureToolStripMenuItem.Text = "Hue/Saturation/Lightness"
        '
        'temperatureToolStripMenuItem
        '
        Me.temperatureToolStripMenuItem.Enabled = False
        Me.temperatureToolStripMenuItem.Name = "temperatureToolStripMenuItem"
        Me.temperatureToolStripMenuItem.Size = New System.Drawing.Size(210, 22)
        Me.temperatureToolStripMenuItem.Text = "Temperature"
        '
        'programToolStripMenuItem
        '
        Me.programToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.aboutToolStripMenuItem, Me.exitToolStripMenuItem})
        Me.programToolStripMenuItem.Name = "programToolStripMenuItem"
        Me.programToolStripMenuItem.Size = New System.Drawing.Size(65, 20)
        Me.programToolStripMenuItem.Text = "Program"
        '
        'aboutToolStripMenuItem
        '
        Me.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem"
        Me.aboutToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.aboutToolStripMenuItem.Text = "About"
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
        Me.exitToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.exitToolStripMenuItem.Text = "Exit"
        '
        'pictureBox1
        '
        Me.pictureBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pictureBox1.Location = New System.Drawing.Point(11, 29)
        Me.pictureBox1.Name = "pictureBox1"
        Me.pictureBox1.Size = New System.Drawing.Size(988, 393)
        Me.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pictureBox1.TabIndex = 1
        Me.pictureBox1.TabStop = False
        '
        'comboBox1
        '
        Me.comboBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.comboBox1.BackColor = System.Drawing.SystemColors.Control
        Me.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comboBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.comboBox1.FormattingEnabled = True
        Me.comboBox1.Location = New System.Drawing.Point(933, 2)
        Me.comboBox1.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.comboBox1.Name = "comboBox1"
        Me.comboBox1.Size = New System.Drawing.Size(126, 23)
        Me.comboBox1.TabIndex = 2
        '
        'label1
        '
        Me.label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.label1.AutoSize = True
        Me.label1.BackColor = System.Drawing.SystemColors.Control
        Me.label1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label1.Location = New System.Drawing.Point(861, 5)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(72, 15)
        Me.label1.TabIndex = 21
        Me.label1.Text = "Colorspace  "
        '
        'label4
        '
        Me.label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.label4.AutoSize = True
        Me.label4.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label4.Location = New System.Drawing.Point(77, 113)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(50, 60)
        Me.label4.TabIndex = 28
        Me.label4.Text = "Mean:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Std Dev:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Median:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Pixels:"
        '
        'label3
        '
        Me.label3.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label3.AutoSize = True
        Me.label3.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label3.Location = New System.Drawing.Point(21, 113)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(50, 60)
        Me.label3.TabIndex = 27
        Me.label3.Text = "Mean:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Std Dev:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Median:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Pixels:"
        Me.label3.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'label2
        '
        Me.label2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label2.AutoSize = True
        Me.label2.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label2.Location = New System.Drawing.Point(92, 5)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(51, 15)
        Me.label2.TabIndex = 29
        Me.label2.Text = "Channel"
        Me.label2.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'label5
        '
        Me.label5.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label5.AutoSize = True
        Me.label5.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label5.Location = New System.Drawing.Point(13, 203)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(72, 15)
        Me.label5.TabIndex = 31
        Me.label5.Text = "Red channel"
        Me.label5.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'label6
        '
        Me.label6.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label6.AutoSize = True
        Me.label6.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label6.Location = New System.Drawing.Point(14, 300)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(83, 15)
        Me.label6.TabIndex = 33
        Me.label6.Text = "Green channel"
        Me.label6.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'label7
        '
        Me.label7.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label7.AutoSize = True
        Me.label7.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label7.Location = New System.Drawing.Point(13, 401)
        Me.label7.Name = "label7"
        Me.label7.Size = New System.Drawing.Size(75, 15)
        Me.label7.TabIndex = 35
        Me.label7.Text = "Blue channel"
        Me.label7.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'panel1
        '
        Me.panel1.AutoScroll = True
        Me.panel1.AutoScrollMinSize = New System.Drawing.Size(200, 300)
        Me.panel1.Controls.Add(Me.textBox1)
        Me.panel1.Controls.Add(Me.label16)
        Me.panel1.Controls.Add(Me.button1)
        Me.panel1.Controls.Add(Me.label15)
        Me.panel1.Controls.Add(Me.button2)
        Me.panel1.Controls.Add(Me.label12)
        Me.panel1.Controls.Add(Me.label10)
        Me.panel1.Controls.Add(Me.trackBar3)
        Me.panel1.Controls.Add(Me.trackBar2)
        Me.panel1.Controls.Add(Me.textBox3)
        Me.panel1.Controls.Add(Me.textBox2)
        Me.panel1.Controls.Add(Me.label13)
        Me.panel1.Controls.Add(Me.label11)
        Me.panel1.Controls.Add(Me.trackBar4)
        Me.panel1.Controls.Add(Me.trackBar1)
        Me.panel1.Controls.Add(Me.textBox4)
        Me.panel1.Controls.Add(Me.label14)
        Me.panel1.Controls.Add(Me.checkBox1)
        Me.panel1.Controls.Add(Me.trackBar5)
        Me.panel1.Controls.Add(Me.textBox5)
        Me.panel1.Controls.Add(Me.label9)
        Me.panel1.Controls.Add(Me.label8)
        Me.panel1.Controls.Add(Me.comboBox2)
        Me.panel1.Controls.Add(Me.label2)
        Me.panel1.Controls.Add(Me.label7)
        Me.panel1.Controls.Add(Me.label3)
        Me.panel1.Controls.Add(Me.label4)
        Me.panel1.Controls.Add(Me.label6)
        Me.panel1.Controls.Add(Me.label5)
        Me.panel1.Controls.Add(Me.histogram1)
        Me.panel1.Controls.Add(Me.histogram2)
        Me.panel1.Controls.Add(Me.histogram3)
        Me.panel1.Controls.Add(Me.histogram4)
        Me.panel1.Location = New System.Drawing.Point(1064, 29)
        Me.panel1.Name = "panel1"
        Me.panel1.Size = New System.Drawing.Size(291, 824)
        Me.panel1.TabIndex = 36
        '
        'textBox1
        '
        Me.textBox1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.textBox1.BackColor = System.Drawing.Color.White
        Me.textBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.textBox1.Location = New System.Drawing.Point(243, 522)
        Me.textBox1.Name = "textBox1"
        Me.textBox1.ReadOnly = True
        Me.textBox1.Size = New System.Drawing.Size(36, 23)
        Me.textBox1.TabIndex = 45
        Me.textBox1.Text = "0"
        Me.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label16
        '
        Me.label16.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label16.AutoSize = True
        Me.label16.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label16.Location = New System.Drawing.Point(92, 502)
        Me.label16.Name = "label16"
        Me.label16.Size = New System.Drawing.Size(108, 15)
        Me.label16.TabIndex = 54
        Me.label16.Text = "Image adjustments"
        '
        'button1
        '
        Me.button1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.button1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.button1.Enabled = False
        Me.button1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.button1.Location = New System.Drawing.Point(21, 775)
        Me.button1.Name = "button1"
        Me.button1.Size = New System.Drawing.Size(126, 28)
        Me.button1.TabIndex = 53
        Me.button1.Text = "Apply"
        Me.button1.UseVisualStyleBackColor = False
        '
        'label15
        '
        Me.label15.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label15.AutoSize = True
        Me.label15.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label15.Location = New System.Drawing.Point(14, 5)
        Me.label15.Name = "label15"
        Me.label15.Size = New System.Drawing.Size(63, 15)
        Me.label15.TabIndex = 37
        Me.label15.Text = "Histogram"
        Me.label15.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'button2
        '
        Me.button2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.button2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.button2.Enabled = False
        Me.button2.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.button2.Location = New System.Drawing.Point(152, 775)
        Me.button2.Name = "button2"
        Me.button2.Size = New System.Drawing.Size(126, 28)
        Me.button2.TabIndex = 52
        Me.button2.Text = "Reset"
        Me.button2.UseVisualStyleBackColor = False
        '
        'label12
        '
        Me.label12.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label12.AutoSize = True
        Me.label12.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label12.Location = New System.Drawing.Point(24, 721)
        Me.label12.Name = "label12"
        Me.label12.Size = New System.Drawing.Size(49, 15)
        Me.label12.TabIndex = 45
        Me.label12.Text = "Gamma"
        '
        'label10
        '
        Me.label10.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label10.AutoSize = True
        Me.label10.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label10.Location = New System.Drawing.Point(24, 575)
        Me.label10.Name = "label10"
        Me.label10.Size = New System.Drawing.Size(52, 15)
        Me.label10.TabIndex = 50
        Me.label10.Text = "Contrast"
        '
        'trackBar3
        '
        Me.trackBar3.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.trackBar3.AutoSize = False
        Me.trackBar3.Enabled = False
        Me.trackBar3.Location = New System.Drawing.Point(17, 744)
        Me.trackBar3.Maximum = 100
        Me.trackBar3.Minimum = -100
        Me.trackBar3.Name = "trackBar3"
        Me.trackBar3.Size = New System.Drawing.Size(258, 26)
        Me.trackBar3.TabIndex = 44
        Me.trackBar3.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'trackBar2
        '
        Me.trackBar2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.trackBar2.AutoSize = False
        Me.trackBar2.Enabled = False
        Me.trackBar2.Location = New System.Drawing.Point(21, 595)
        Me.trackBar2.Maximum = 100
        Me.trackBar2.Minimum = -100
        Me.trackBar2.Name = "trackBar2"
        Me.trackBar2.Size = New System.Drawing.Size(258, 26)
        Me.trackBar2.TabIndex = 49
        Me.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'textBox3
        '
        Me.textBox3.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.textBox3.BackColor = System.Drawing.Color.White
        Me.textBox3.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.textBox3.Location = New System.Drawing.Point(243, 718)
        Me.textBox3.Name = "textBox3"
        Me.textBox3.ReadOnly = True
        Me.textBox3.Size = New System.Drawing.Size(36, 23)
        Me.textBox3.TabIndex = 43
        Me.textBox3.Text = "0"
        Me.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'textBox2
        '
        Me.textBox2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.textBox2.BackColor = System.Drawing.Color.White
        Me.textBox2.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.textBox2.Location = New System.Drawing.Point(243, 571)
        Me.textBox2.Name = "textBox2"
        Me.textBox2.ReadOnly = True
        Me.textBox2.Size = New System.Drawing.Size(36, 23)
        Me.textBox2.TabIndex = 48
        Me.textBox2.Text = "0"
        Me.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label13
        '
        Me.label13.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label13.AutoSize = True
        Me.label13.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label13.Location = New System.Drawing.Point(24, 674)
        Me.label13.Name = "label13"
        Me.label13.Size = New System.Drawing.Size(55, 15)
        Me.label13.TabIndex = 42
        Me.label13.Text = "Exposure"
        '
        'label11
        '
        Me.label11.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label11.AutoSize = True
        Me.label11.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label11.Location = New System.Drawing.Point(24, 524)
        Me.label11.Name = "label11"
        Me.label11.Size = New System.Drawing.Size(61, 15)
        Me.label11.TabIndex = 47
        Me.label11.Text = "Saturation"
        '
        'trackBar4
        '
        Me.trackBar4.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.trackBar4.AutoSize = False
        Me.trackBar4.Enabled = False
        Me.trackBar4.Location = New System.Drawing.Point(21, 695)
        Me.trackBar4.Maximum = 50
        Me.trackBar4.Minimum = -50
        Me.trackBar4.Name = "trackBar4"
        Me.trackBar4.Size = New System.Drawing.Size(258, 26)
        Me.trackBar4.TabIndex = 41
        Me.trackBar4.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'trackBar1
        '
        Me.trackBar1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.trackBar1.AutoSize = False
        Me.trackBar1.Enabled = False
        Me.trackBar1.Location = New System.Drawing.Point(21, 545)
        Me.trackBar1.Maximum = 100
        Me.trackBar1.Minimum = -100
        Me.trackBar1.Name = "trackBar1"
        Me.trackBar1.Size = New System.Drawing.Size(258, 26)
        Me.trackBar1.TabIndex = 46
        Me.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'textBox4
        '
        Me.textBox4.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.textBox4.BackColor = System.Drawing.Color.White
        Me.textBox4.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.textBox4.Location = New System.Drawing.Point(243, 672)
        Me.textBox4.Name = "textBox4"
        Me.textBox4.ReadOnly = True
        Me.textBox4.Size = New System.Drawing.Size(36, 23)
        Me.textBox4.TabIndex = 40
        Me.textBox4.Text = "0"
        Me.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label14
        '
        Me.label14.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label14.AutoSize = True
        Me.label14.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label14.Location = New System.Drawing.Point(24, 624)
        Me.label14.Name = "label14"
        Me.label14.Size = New System.Drawing.Size(62, 15)
        Me.label14.TabIndex = 39
        Me.label14.Text = "Brightness"
        '
        'checkBox1
        '
        Me.checkBox1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.checkBox1.AutoSize = True
        Me.checkBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.checkBox1.Location = New System.Drawing.Point(17, 178)
        Me.checkBox1.Name = "checkBox1"
        Me.checkBox1.Size = New System.Drawing.Size(147, 19)
        Me.checkBox1.TabIndex = 4
        Me.checkBox1.Text = "Logarithmic histogram"
        Me.checkBox1.UseVisualStyleBackColor = True
        '
        'trackBar5
        '
        Me.trackBar5.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.trackBar5.AutoSize = False
        Me.trackBar5.Enabled = False
        Me.trackBar5.Location = New System.Drawing.Point(21, 646)
        Me.trackBar5.Maximum = 100
        Me.trackBar5.Minimum = -100
        Me.trackBar5.Name = "trackBar5"
        Me.trackBar5.Size = New System.Drawing.Size(258, 26)
        Me.trackBar5.TabIndex = 138
        Me.trackBar5.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'textBox5
        '
        Me.textBox5.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.textBox5.BackColor = System.Drawing.Color.White
        Me.textBox5.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.textBox5.Location = New System.Drawing.Point(243, 621)
        Me.textBox5.Name = "textBox5"
        Me.textBox5.ReadOnly = True
        Me.textBox5.Size = New System.Drawing.Size(36, 23)
        Me.textBox5.TabIndex = 37
        Me.textBox5.Text = "0"
        Me.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label9
        '
        Me.label9.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label9.AutoSize = True
        Me.label9.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label9.Location = New System.Drawing.Point(153, 113)
        Me.label9.Name = "label9"
        Me.label9.Size = New System.Drawing.Size(62, 45)
        Me.label9.TabIndex = 43
        Me.label9.Text = "Level:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Count:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Percentile:"
        Me.label9.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'label8
        '
        Me.label8.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.label8.AutoSize = True
        Me.label8.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label8.Location = New System.Drawing.Point(224, 113)
        Me.label8.Name = "label8"
        Me.label8.Size = New System.Drawing.Size(62, 45)
        Me.label8.TabIndex = 41
        Me.label8.Text = "Level:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Count:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Percentile:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'comboBox2
        '
        Me.comboBox2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.comboBox2.BackColor = System.Drawing.SystemColors.Control
        Me.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comboBox2.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.comboBox2.FormattingEnabled = True
        Me.comboBox2.Location = New System.Drawing.Point(149, 2)
        Me.comboBox2.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.comboBox2.Name = "comboBox2"
        Me.comboBox2.Size = New System.Drawing.Size(126, 23)
        Me.comboBox2.TabIndex = 2
        '
        'histogram1
        '
        Me.histogram1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.histogram1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.histogram1.Location = New System.Drawing.Point(17, 27)
        Me.histogram1.Name = "histogram1"
        Me.histogram1.Size = New System.Drawing.Size(258, 83)
        Me.histogram1.TabIndex = 3
        Me.histogram1.Text = "histogram1"
        Me.histogram1.Values = Nothing
        '
        'histogram2
        '
        Me.histogram2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.histogram2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.histogram2.Location = New System.Drawing.Point(16, 221)
        Me.histogram2.Name = "histogram2"
        Me.histogram2.Size = New System.Drawing.Size(258, 71)
        Me.histogram2.TabIndex = 37
        Me.histogram2.Text = "histogram2"
        Me.histogram2.Values = Nothing
        '
        'histogram3
        '
        Me.histogram3.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.histogram3.BackColor = System.Drawing.SystemColors.ControlLight
        Me.histogram3.Location = New System.Drawing.Point(17, 318)
        Me.histogram3.Name = "histogram3"
        Me.histogram3.Size = New System.Drawing.Size(258, 75)
        Me.histogram3.TabIndex = 38
        Me.histogram3.Text = "histogram3"
        Me.histogram3.Values = Nothing
        '
        'histogram4
        '
        Me.histogram4.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.histogram4.BackColor = System.Drawing.SystemColors.ControlLight
        Me.histogram4.Location = New System.Drawing.Point(16, 419)
        Me.histogram4.Name = "histogram4"
        Me.histogram4.Size = New System.Drawing.Size(258, 72)
        Me.histogram4.TabIndex = 39
        Me.histogram4.Text = "histogram4"
        Me.histogram4.Values = Nothing
        '
        'VisualStudioToolStripExtender1
        '
        Me.VisualStudioToolStripExtender1.DefaultRenderer = Nothing
        '
        'FormEditMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(1355, 960)
        Me.Controls.Add(Me.panel1)
        Me.Controls.Add(Me.comboBox1)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.pictureBox1)
        Me.Controls.Add(Me.menuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.menuStrip1
        Me.Name = "FormEditMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Image Editor"
        Me.menuStrip1.ResumeLayout(False)
        Me.menuStrip1.PerformLayout()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.panel1.ResumeLayout(False)
        Me.panel1.PerformLayout()
        CType(Me.trackBar3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private WithEvents menuStrip1 As Windows.Forms.MenuStrip
    Private WithEvents fileToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents openToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents reloadToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents saveToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents filterToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents localLaplacianToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents temperatureToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents programToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents aboutToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents exitToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents pictureBox1 As Windows.Forms.PictureBox
    Private WithEvents exposureToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents comboBox1 As Windows.Forms.ComboBox
    Private WithEvents label1 As Windows.Forms.Label
    Private WithEvents closeToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents label4 As Windows.Forms.Label
    Private WithEvents label3 As Windows.Forms.Label
    Private WithEvents label2 As Windows.Forms.Label
    Private WithEvents label5 As Windows.Forms.Label
    Private WithEvents label6 As Windows.Forms.Label
    Private WithEvents label7 As Windows.Forms.Label
    Private WithEvents panel1 As Windows.Forms.Panel
    Private WithEvents histogram1 As Histogram
    Private WithEvents histogram2 As Histogram
    Private WithEvents histogram3 As Histogram
    Private WithEvents histogram4 As Histogram
    Private WithEvents comboBox2 As Windows.Forms.ComboBox
    Private WithEvents label8 As Windows.Forms.Label
    Private WithEvents checkBox1 As Windows.Forms.CheckBox
    Private WithEvents label9 As Windows.Forms.Label
    Private WithEvents label10 As Windows.Forms.Label
    Private WithEvents trackBar2 As Windows.Forms.TrackBar
    Public WithEvents textBox2 As Windows.Forms.TextBox
    Private WithEvents label11 As Windows.Forms.Label
    Private WithEvents trackBar1 As Windows.Forms.TrackBar
    Public WithEvents textBox1 As Windows.Forms.TextBox
    Private WithEvents label12 As Windows.Forms.Label
    Private WithEvents trackBar3 As Windows.Forms.TrackBar
    Public WithEvents textBox3 As Windows.Forms.TextBox
    Private WithEvents label13 As Windows.Forms.Label
    Private WithEvents trackBar4 As Windows.Forms.TrackBar
    Public WithEvents textBox4 As Windows.Forms.TextBox
    Private WithEvents label14 As Windows.Forms.Label
    Private WithEvents trackBar5 As Windows.Forms.TrackBar
    Public WithEvents textBox5 As Windows.Forms.TextBox
    Private WithEvents button2 As Windows.Forms.Button
    Private WithEvents label15 As Windows.Forms.Label
    Private WithEvents button1 As Windows.Forms.Button
    Private WithEvents label16 As Windows.Forms.Label
    Private WithEvents editToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents undoToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents redoToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents flipVerticalToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents flipHorizontalToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents VisualStudioToolStripExtender1 As WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender
    Friend WithEvents VS2015BlueTheme1 As WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme
End Class

