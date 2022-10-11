
Partial Class PropertyWindow
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If

        MyBase.Dispose(disposing)
    End Sub

    Sub New()
        Call InitializeComponent()
    End Sub

#Region "Windows Form Designer generated code"
    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PropertyWindow))
        Me.mainMenu1 = New System.Windows.Forms.MenuStrip()
        Me.menuItem1 = New System.Windows.Forms.ToolStripMenuItem()
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
        Me.label2 = New System.Windows.Forms.Label()
        Me.label7 = New System.Windows.Forms.Label()
        Me.label3 = New System.Windows.Forms.Label()
        Me.label4 = New System.Windows.Forms.Label()
        Me.label6 = New System.Windows.Forms.Label()
        Me.label5 = New System.Windows.Forms.Label()
        Me.histogram1 = New LaplacianHDR.Histogram()
        Me.histogram2 = New LaplacianHDR.Histogram()
        Me.histogram3 = New LaplacianHDR.Histogram()
        Me.histogram4 = New LaplacianHDR.Histogram()
        Me.mainMenu1.SuspendLayout()
        CType(Me.trackBar3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar5, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'mainMenu1
        '
        Me.mainMenu1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuItem1})
        Me.mainMenu1.Location = New System.Drawing.Point(0, 0)
        Me.mainMenu1.Name = "mainMenu1"
        Me.mainMenu1.Size = New System.Drawing.Size(200, 24)
        Me.mainMenu1.TabIndex = 0
        '
        'menuItem1
        '
        Me.menuItem1.Name = "menuItem1"
        Me.menuItem1.Size = New System.Drawing.Size(37, 20)
        Me.menuItem1.Text = "File"
        '
        'textBox1
        '
        Me.textBox1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.textBox1.BackColor = System.Drawing.Color.White
        Me.textBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.textBox1.Location = New System.Drawing.Point(241, 462)
        Me.textBox1.Name = "textBox1"
        Me.textBox1.ReadOnly = True
        Me.textBox1.Size = New System.Drawing.Size(36, 23)
        Me.textBox1.TabIndex = 162
        Me.textBox1.Text = "0"
        Me.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label16
        '
        Me.label16.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label16.AutoSize = True
        Me.label16.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label16.Location = New System.Drawing.Point(90, 442)
        Me.label16.Name = "label16"
        Me.label16.Size = New System.Drawing.Size(108, 15)
        Me.label16.TabIndex = 170
        Me.label16.Text = "Image adjustments"
        '
        'button1
        '
        Me.button1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.button1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.button1.Enabled = False
        Me.button1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.button1.Location = New System.Drawing.Point(19, 715)
        Me.button1.Name = "button1"
        Me.button1.Size = New System.Drawing.Size(126, 28)
        Me.button1.TabIndex = 169
        Me.button1.Text = "Apply"
        Me.button1.UseVisualStyleBackColor = False
        '
        'label15
        '
        Me.label15.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label15.AutoSize = True
        Me.label15.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label15.Location = New System.Drawing.Point(15, 17)
        Me.label15.Name = "label15"
        Me.label15.Size = New System.Drawing.Size(63, 15)
        Me.label15.TabIndex = 149
        Me.label15.Text = "Histogram"
        Me.label15.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'button2
        '
        Me.button2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.button2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.button2.Enabled = False
        Me.button2.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.button2.Location = New System.Drawing.Point(150, 715)
        Me.button2.Name = "button2"
        Me.button2.Size = New System.Drawing.Size(126, 28)
        Me.button2.TabIndex = 168
        Me.button2.Text = "Reset"
        Me.button2.UseVisualStyleBackColor = False
        '
        'label12
        '
        Me.label12.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label12.AutoSize = True
        Me.label12.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label12.Location = New System.Drawing.Point(22, 661)
        Me.label12.Name = "label12"
        Me.label12.Size = New System.Drawing.Size(49, 15)
        Me.label12.TabIndex = 161
        Me.label12.Text = "Gamma"
        '
        'label10
        '
        Me.label10.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label10.AutoSize = True
        Me.label10.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label10.Location = New System.Drawing.Point(22, 515)
        Me.label10.Name = "label10"
        Me.label10.Size = New System.Drawing.Size(52, 15)
        Me.label10.TabIndex = 167
        Me.label10.Text = "Contrast"
        '
        'trackBar3
        '
        Me.trackBar3.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.trackBar3.AutoSize = False
        Me.trackBar3.Enabled = False
        Me.trackBar3.Location = New System.Drawing.Point(15, 684)
        Me.trackBar3.Maximum = 100
        Me.trackBar3.Minimum = -100
        Me.trackBar3.Name = "trackBar3"
        Me.trackBar3.Size = New System.Drawing.Size(258, 26)
        Me.trackBar3.TabIndex = 160
        Me.trackBar3.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'trackBar2
        '
        Me.trackBar2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.trackBar2.AutoSize = False
        Me.trackBar2.Enabled = False
        Me.trackBar2.Location = New System.Drawing.Point(19, 535)
        Me.trackBar2.Maximum = 100
        Me.trackBar2.Minimum = -100
        Me.trackBar2.Name = "trackBar2"
        Me.trackBar2.Size = New System.Drawing.Size(258, 26)
        Me.trackBar2.TabIndex = 166
        Me.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'textBox3
        '
        Me.textBox3.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.textBox3.BackColor = System.Drawing.Color.White
        Me.textBox3.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.textBox3.Location = New System.Drawing.Point(241, 658)
        Me.textBox3.Name = "textBox3"
        Me.textBox3.ReadOnly = True
        Me.textBox3.Size = New System.Drawing.Size(36, 23)
        Me.textBox3.TabIndex = 159
        Me.textBox3.Text = "0"
        Me.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'textBox2
        '
        Me.textBox2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.textBox2.BackColor = System.Drawing.Color.White
        Me.textBox2.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.textBox2.Location = New System.Drawing.Point(241, 511)
        Me.textBox2.Name = "textBox2"
        Me.textBox2.ReadOnly = True
        Me.textBox2.Size = New System.Drawing.Size(36, 23)
        Me.textBox2.TabIndex = 165
        Me.textBox2.Text = "0"
        Me.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label13
        '
        Me.label13.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label13.AutoSize = True
        Me.label13.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label13.Location = New System.Drawing.Point(22, 614)
        Me.label13.Name = "label13"
        Me.label13.Size = New System.Drawing.Size(55, 15)
        Me.label13.TabIndex = 157
        Me.label13.Text = "Exposure"
        '
        'label11
        '
        Me.label11.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label11.AutoSize = True
        Me.label11.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label11.Location = New System.Drawing.Point(22, 464)
        Me.label11.Name = "label11"
        Me.label11.Size = New System.Drawing.Size(61, 15)
        Me.label11.TabIndex = 164
        Me.label11.Text = "Saturation"
        '
        'trackBar4
        '
        Me.trackBar4.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.trackBar4.AutoSize = False
        Me.trackBar4.Enabled = False
        Me.trackBar4.Location = New System.Drawing.Point(19, 635)
        Me.trackBar4.Maximum = 50
        Me.trackBar4.Minimum = -50
        Me.trackBar4.Name = "trackBar4"
        Me.trackBar4.Size = New System.Drawing.Size(258, 26)
        Me.trackBar4.TabIndex = 155
        Me.trackBar4.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'trackBar1
        '
        Me.trackBar1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.trackBar1.AutoSize = False
        Me.trackBar1.Enabled = False
        Me.trackBar1.Location = New System.Drawing.Point(19, 485)
        Me.trackBar1.Maximum = 100
        Me.trackBar1.Minimum = -100
        Me.trackBar1.Name = "trackBar1"
        Me.trackBar1.Size = New System.Drawing.Size(258, 26)
        Me.trackBar1.TabIndex = 163
        Me.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'textBox4
        '
        Me.textBox4.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.textBox4.BackColor = System.Drawing.Color.White
        Me.textBox4.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.textBox4.Location = New System.Drawing.Point(241, 612)
        Me.textBox4.Name = "textBox4"
        Me.textBox4.ReadOnly = True
        Me.textBox4.Size = New System.Drawing.Size(36, 23)
        Me.textBox4.TabIndex = 154
        Me.textBox4.Text = "0"
        Me.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label14
        '
        Me.label14.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label14.AutoSize = True
        Me.label14.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label14.Location = New System.Drawing.Point(22, 564)
        Me.label14.Name = "label14"
        Me.label14.Size = New System.Drawing.Size(62, 15)
        Me.label14.TabIndex = 152
        Me.label14.Text = "Brightness"
        '
        'checkBox1
        '
        Me.checkBox1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.checkBox1.AutoSize = True
        Me.checkBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.checkBox1.Location = New System.Drawing.Point(19, 169)
        Me.checkBox1.Name = "checkBox1"
        Me.checkBox1.Size = New System.Drawing.Size(147, 19)
        Me.checkBox1.TabIndex = 141
        Me.checkBox1.Text = "Logarithmic histogram"
        Me.checkBox1.UseVisualStyleBackColor = True
        '
        'trackBar5
        '
        Me.trackBar5.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.trackBar5.AutoSize = False
        Me.trackBar5.Enabled = False
        Me.trackBar5.Location = New System.Drawing.Point(19, 586)
        Me.trackBar5.Maximum = 100
        Me.trackBar5.Minimum = -100
        Me.trackBar5.Name = "trackBar5"
        Me.trackBar5.Size = New System.Drawing.Size(258, 26)
        Me.trackBar5.TabIndex = 171
        Me.trackBar5.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'textBox5
        '
        Me.textBox5.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.textBox5.BackColor = System.Drawing.Color.White
        Me.textBox5.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.textBox5.Location = New System.Drawing.Point(241, 561)
        Me.textBox5.Name = "textBox5"
        Me.textBox5.ReadOnly = True
        Me.textBox5.Size = New System.Drawing.Size(36, 23)
        Me.textBox5.TabIndex = 150
        Me.textBox5.Text = "0"
        Me.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label9
        '
        Me.label9.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label9.AutoSize = True
        Me.label9.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label9.Location = New System.Drawing.Point(140, 104)
        Me.label9.Name = "label9"
        Me.label9.Size = New System.Drawing.Size(62, 45)
        Me.label9.TabIndex = 158
        Me.label9.Text = "Level:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Count:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Percentile:"
        Me.label9.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'label8
        '
        Me.label8.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.label8.AutoSize = True
        Me.label8.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label8.Location = New System.Drawing.Point(219, 104)
        Me.label8.Name = "label8"
        Me.label8.Size = New System.Drawing.Size(62, 45)
        Me.label8.TabIndex = 156
        Me.label8.Text = "Level:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Count:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Percentile:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'comboBox2
        '
        Me.comboBox2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.comboBox2.BackColor = System.Drawing.SystemColors.Control
        Me.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comboBox2.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.comboBox2.FormattingEnabled = True
        Me.comboBox2.Location = New System.Drawing.Point(150, 14)
        Me.comboBox2.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.comboBox2.Name = "comboBox2"
        Me.comboBox2.Size = New System.Drawing.Size(126, 23)
        Me.comboBox2.TabIndex = 139
        '
        'label2
        '
        Me.label2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label2.AutoSize = True
        Me.label2.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label2.Location = New System.Drawing.Point(93, 17)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(51, 15)
        Me.label2.TabIndex = 144
        Me.label2.Text = "Channel"
        Me.label2.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'label7
        '
        Me.label7.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label7.AutoSize = True
        Me.label7.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label7.Location = New System.Drawing.Point(15, 359)
        Me.label7.Name = "label7"
        Me.label7.Size = New System.Drawing.Size(75, 15)
        Me.label7.TabIndex = 147
        Me.label7.Text = "Blue channel"
        Me.label7.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'label3
        '
        Me.label3.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label3.AutoSize = True
        Me.label3.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label3.Location = New System.Drawing.Point(16, 104)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(50, 60)
        Me.label3.TabIndex = 142
        Me.label3.Text = "Mean:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Std Dev:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Median:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Pixels:"
        Me.label3.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'label4
        '
        Me.label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.label4.AutoSize = True
        Me.label4.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label4.Location = New System.Drawing.Point(83, 106)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(50, 60)
        Me.label4.TabIndex = 143
        Me.label4.Text = "Mean:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Std Dev:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Median:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Pixels:"
        '
        'label6
        '
        Me.label6.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label6.AutoSize = True
        Me.label6.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label6.Location = New System.Drawing.Point(15, 276)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(83, 15)
        Me.label6.TabIndex = 146
        Me.label6.Text = "Green channel"
        Me.label6.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'label5
        '
        Me.label5.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.label5.AutoSize = True
        Me.label5.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label5.Location = New System.Drawing.Point(15, 194)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(72, 15)
        Me.label5.TabIndex = 145
        Me.label5.Text = "Red channel"
        Me.label5.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'histogram1
        '
        Me.histogram1.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.histogram1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.histogram1.Location = New System.Drawing.Point(18, 44)
        Me.histogram1.Name = "histogram1"
        Me.histogram1.Size = New System.Drawing.Size(258, 51)
        Me.histogram1.TabIndex = 140
        Me.histogram1.Text = "histogram1"
        Me.histogram1.Values = Nothing
        '
        'histogram2
        '
        Me.histogram2.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.histogram2.BackColor = System.Drawing.SystemColors.ControlLight
        Me.histogram2.Location = New System.Drawing.Point(18, 212)
        Me.histogram2.Name = "histogram2"
        Me.histogram2.Size = New System.Drawing.Size(258, 58)
        Me.histogram2.TabIndex = 148
        Me.histogram2.Text = "histogram2"
        Me.histogram2.Values = Nothing
        '
        'histogram3
        '
        Me.histogram3.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.histogram3.BackColor = System.Drawing.SystemColors.ControlLight
        Me.histogram3.Location = New System.Drawing.Point(18, 294)
        Me.histogram3.Name = "histogram3"
        Me.histogram3.Size = New System.Drawing.Size(258, 57)
        Me.histogram3.TabIndex = 151
        Me.histogram3.Text = "histogram3"
        Me.histogram3.Values = Nothing
        '
        'histogram4
        '
        Me.histogram4.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.histogram4.BackColor = System.Drawing.SystemColors.ControlLight
        Me.histogram4.Location = New System.Drawing.Point(18, 377)
        Me.histogram4.Name = "histogram4"
        Me.histogram4.Size = New System.Drawing.Size(258, 54)
        Me.histogram4.TabIndex = 153
        Me.histogram4.Text = "histogram4"
        Me.histogram4.Values = Nothing
        '
        'PropertyWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.ClientSize = New System.Drawing.Size(300, 752)
        Me.Controls.Add(Me.textBox1)
        Me.Controls.Add(Me.label16)
        Me.Controls.Add(Me.button1)
        Me.Controls.Add(Me.label15)
        Me.Controls.Add(Me.button2)
        Me.Controls.Add(Me.label12)
        Me.Controls.Add(Me.label10)
        Me.Controls.Add(Me.trackBar3)
        Me.Controls.Add(Me.trackBar2)
        Me.Controls.Add(Me.textBox3)
        Me.Controls.Add(Me.textBox2)
        Me.Controls.Add(Me.label13)
        Me.Controls.Add(Me.label11)
        Me.Controls.Add(Me.trackBar4)
        Me.Controls.Add(Me.trackBar1)
        Me.Controls.Add(Me.textBox4)
        Me.Controls.Add(Me.label14)
        Me.Controls.Add(Me.checkBox1)
        Me.Controls.Add(Me.trackBar5)
        Me.Controls.Add(Me.textBox5)
        Me.Controls.Add(Me.label9)
        Me.Controls.Add(Me.label8)
        Me.Controls.Add(Me.comboBox2)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.label7)
        Me.Controls.Add(Me.label3)
        Me.Controls.Add(Me.label4)
        Me.Controls.Add(Me.label6)
        Me.Controls.Add(Me.label5)
        Me.Controls.Add(Me.histogram1)
        Me.Controls.Add(Me.histogram2)
        Me.Controls.Add(Me.histogram3)
        Me.Controls.Add(Me.histogram4)
        Me.HideOnClose = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.mainMenu1
        Me.Name = "PropertyWindow"
        Me.Padding = New System.Windows.Forms.Padding(0, 3, 0, 3)
        Me.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockRight
        Me.TabText = "Properties"
        Me.Text = "Properties"
        Me.mainMenu1.ResumeLayout(False)
        Me.mainMenu1.PerformLayout()
        CType(Me.trackBar3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar5, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
#End Region
    Private mainMenu1 As Windows.Forms.MenuStrip
    Private menuItem1 As Windows.Forms.ToolStripMenuItem
    Public WithEvents textBox1 As TextBox
    Private WithEvents label16 As Label
    Private WithEvents button1 As Button
    Private WithEvents label15 As Label
    Private WithEvents button2 As Button
    Private WithEvents label12 As Label
    Private WithEvents label10 As Label
    Private WithEvents trackBar3 As TrackBar
    Private WithEvents trackBar2 As TrackBar
    Public WithEvents textBox3 As TextBox
    Public WithEvents textBox2 As TextBox
    Private WithEvents label13 As Label
    Private WithEvents label11 As Label
    Private WithEvents trackBar4 As TrackBar
    Private WithEvents trackBar1 As TrackBar
    Public WithEvents textBox4 As TextBox
    Private WithEvents label14 As Label
    Private WithEvents checkBox1 As CheckBox
    Private WithEvents trackBar5 As TrackBar
    Public WithEvents textBox5 As TextBox
    Private WithEvents label9 As Label
    Private WithEvents label8 As Label
    Private WithEvents comboBox2 As ComboBox
    Private WithEvents label2 As Label
    Private WithEvents label7 As Label
    Private WithEvents label3 As Label
    Private WithEvents label4 As Label
    Private WithEvents label6 As Label
    Private WithEvents label5 As Label
    Private WithEvents histogram1 As Histogram
    Private WithEvents histogram2 As Histogram
    Private WithEvents histogram3 As Histogram
    Private WithEvents histogram4 As Histogram
End Class

