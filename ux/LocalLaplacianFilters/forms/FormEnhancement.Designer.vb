
    Partial Class FormEnhancement
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormEnhancement))
        Me.button1 = New System.Windows.Forms.Button()
        Me.pictureBox1 = New System.Windows.Forms.PictureBox()
        Me.label1 = New System.Windows.Forms.Label()
        Me.trackBar1 = New System.Windows.Forms.TrackBar()
        Me.textBox1 = New System.Windows.Forms.TextBox()
        Me.label2 = New System.Windows.Forms.Label()
        Me.trackBar2 = New System.Windows.Forms.TrackBar()
        Me.textBox2 = New System.Windows.Forms.TextBox()
        Me.label3 = New System.Windows.Forms.Label()
        Me.trackBar3 = New System.Windows.Forms.TrackBar()
        Me.textBox3 = New System.Windows.Forms.TextBox()
        Me.label4 = New System.Windows.Forms.Label()
        Me.trackBar4 = New System.Windows.Forms.TrackBar()
        Me.textBox4 = New System.Windows.Forms.TextBox()
        Me.label5 = New System.Windows.Forms.Label()
        Me.trackBar5 = New System.Windows.Forms.TrackBar()
        Me.textBox5 = New System.Windows.Forms.TextBox()
        Me.label6 = New System.Windows.Forms.Label()
        Me.trackBar6 = New System.Windows.Forms.TrackBar()
        Me.textBox6 = New System.Windows.Forms.TextBox()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar3, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar4, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar5, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar6, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'button1
        '
        Me.button1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.button1.Location = New System.Drawing.Point(12, 554)
        Me.button1.Name = "button1"
        Me.button1.Size = New System.Drawing.Size(225, 28)
        Me.button1.TabIndex = 1
        Me.button1.Text = "Apply"
        Me.button1.UseVisualStyleBackColor = True
        '
        'pictureBox1
        '
        Me.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.pictureBox1.Name = "pictureBox1"
        Me.pictureBox1.Size = New System.Drawing.Size(225, 225)
        Me.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.pictureBox1.TabIndex = 2
        Me.pictureBox1.TabStop = False
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label1.Location = New System.Drawing.Point(7, 246)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(90, 15)
        Me.label1.TabIndex = 10
        Me.label1.Text = "Lights/shadows"
        '
        'trackBar1
        '
        Me.trackBar1.AutoSize = False
        Me.trackBar1.Location = New System.Drawing.Point(3, 267)
        Me.trackBar1.Maximum = 100
        Me.trackBar1.Minimum = -100
        Me.trackBar1.Name = "trackBar1"
        Me.trackBar1.Size = New System.Drawing.Size(243, 26)
        Me.trackBar1.TabIndex = 9
        Me.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'textBox1
        '
        Me.textBox1.BackColor = System.Drawing.Color.White
        Me.textBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.textBox1.Location = New System.Drawing.Point(201, 243)
        Me.textBox1.Name = "textBox1"
        Me.textBox1.ReadOnly = True
        Me.textBox1.Size = New System.Drawing.Size(36, 23)
        Me.textBox1.TabIndex = 8
        Me.textBox1.Text = "0"
        Me.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label2.Location = New System.Drawing.Point(7, 296)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(40, 15)
        Me.label2.TabIndex = 13
        Me.label2.Text = "Sigma"
        '
        'trackBar2
        '
        Me.trackBar2.AutoSize = False
        Me.trackBar2.Location = New System.Drawing.Point(3, 317)
        Me.trackBar2.Maximum = 199
        Me.trackBar2.Name = "trackBar2"
        Me.trackBar2.Size = New System.Drawing.Size(243, 26)
        Me.trackBar2.TabIndex = 12
        Me.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trackBar2.Value = 49
        '
        'textBox2
        '
        Me.textBox2.BackColor = System.Drawing.Color.White
        Me.textBox2.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.textBox2.Location = New System.Drawing.Point(201, 293)
        Me.textBox2.Name = "textBox2"
        Me.textBox2.ReadOnly = True
        Me.textBox2.Size = New System.Drawing.Size(36, 23)
        Me.textBox2.TabIndex = 11
        Me.textBox2.Text = "0.05"
        Me.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label3.Location = New System.Drawing.Point(7, 346)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(35, 15)
        Me.label3.TabIndex = 16
        Me.label3.Text = "Steps"
        '
        'trackBar3
        '
        Me.trackBar3.AutoSize = False
        Me.trackBar3.Location = New System.Drawing.Point(3, 367)
        Me.trackBar3.Maximum = 100
        Me.trackBar3.Minimum = 1
        Me.trackBar3.Name = "trackBar3"
        Me.trackBar3.Size = New System.Drawing.Size(243, 26)
        Me.trackBar3.TabIndex = 15
        Me.trackBar3.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trackBar3.Value = 20
        '
        'textBox3
        '
        Me.textBox3.BackColor = System.Drawing.Color.White
        Me.textBox3.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.textBox3.Location = New System.Drawing.Point(201, 343)
        Me.textBox3.Name = "textBox3"
        Me.textBox3.ReadOnly = True
        Me.textBox3.Size = New System.Drawing.Size(36, 23)
        Me.textBox3.TabIndex = 14
        Me.textBox3.Text = "20"
        Me.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label4
        '
        Me.label4.AutoSize = True
        Me.label4.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label4.Location = New System.Drawing.Point(7, 396)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(39, 15)
        Me.label4.TabIndex = 19
        Me.label4.Text = "Levels"
        '
        'trackBar4
        '
        Me.trackBar4.AutoSize = False
        Me.trackBar4.Location = New System.Drawing.Point(3, 417)
        Me.trackBar4.Maximum = 100
        Me.trackBar4.Minimum = 1
        Me.trackBar4.Name = "trackBar4"
        Me.trackBar4.Size = New System.Drawing.Size(243, 26)
        Me.trackBar4.TabIndex = 18
        Me.trackBar4.TickStyle = System.Windows.Forms.TickStyle.None
        Me.trackBar4.Value = 20
        '
        'textBox4
        '
        Me.textBox4.BackColor = System.Drawing.Color.White
        Me.textBox4.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.textBox4.Location = New System.Drawing.Point(201, 393)
        Me.textBox4.Name = "textBox4"
        Me.textBox4.ReadOnly = True
        Me.textBox4.Size = New System.Drawing.Size(36, 23)
        Me.textBox4.TabIndex = 17
        Me.textBox4.Text = "20"
        Me.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label5
        '
        Me.label5.AutoSize = True
        Me.label5.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label5.Location = New System.Drawing.Point(9, 446)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(42, 15)
        Me.label5.TabIndex = 22
        Me.label5.Text = "Details"
        '
        'trackBar5
        '
        Me.trackBar5.AutoSize = False
        Me.trackBar5.Location = New System.Drawing.Point(3, 467)
        Me.trackBar5.Maximum = 100
        Me.trackBar5.Minimum = -100
        Me.trackBar5.Name = "trackBar5"
        Me.trackBar5.Size = New System.Drawing.Size(243, 26)
        Me.trackBar5.TabIndex = 21
        Me.trackBar5.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'textBox5
        '
        Me.textBox5.BackColor = System.Drawing.Color.White
        Me.textBox5.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.textBox5.Location = New System.Drawing.Point(201, 443)
        Me.textBox5.Name = "textBox5"
        Me.textBox5.ReadOnly = True
        Me.textBox5.Size = New System.Drawing.Size(36, 23)
        Me.textBox5.TabIndex = 20
        Me.textBox5.Text = "0"
        Me.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label6
        '
        Me.label6.AutoSize = True
        Me.label6.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label6.Location = New System.Drawing.Point(9, 501)
        Me.label6.Name = "label6"
        Me.label6.Size = New System.Drawing.Size(66, 15)
        Me.label6.TabIndex = 25
        Me.label6.Text = "Smoothing"
        '
        'trackBar6
        '
        Me.trackBar6.AutoSize = False
        Me.trackBar6.Location = New System.Drawing.Point(3, 522)
        Me.trackBar6.Maximum = 50
        Me.trackBar6.Name = "trackBar6"
        Me.trackBar6.Size = New System.Drawing.Size(243, 26)
        Me.trackBar6.TabIndex = 24
        Me.trackBar6.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'textBox6
        '
        Me.textBox6.BackColor = System.Drawing.Color.White
        Me.textBox6.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.textBox6.Location = New System.Drawing.Point(201, 498)
        Me.textBox6.Name = "textBox6"
        Me.textBox6.ReadOnly = True
        Me.textBox6.Size = New System.Drawing.Size(36, 23)
        Me.textBox6.TabIndex = 23
        Me.textBox6.Text = "0"
        Me.textBox6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FormEnhancement
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(249, 594)
        Me.Controls.Add(Me.label6)
        Me.Controls.Add(Me.trackBar6)
        Me.Controls.Add(Me.textBox6)
        Me.Controls.Add(Me.label5)
        Me.Controls.Add(Me.trackBar5)
        Me.Controls.Add(Me.textBox5)
        Me.Controls.Add(Me.label4)
        Me.Controls.Add(Me.trackBar4)
        Me.Controls.Add(Me.textBox4)
        Me.Controls.Add(Me.label3)
        Me.Controls.Add(Me.trackBar3)
        Me.Controls.Add(Me.textBox3)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.trackBar2)
        Me.Controls.Add(Me.textBox2)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.trackBar1)
        Me.Controls.Add(Me.textBox1)
        Me.Controls.Add(Me.pictureBox1)
        Me.Controls.Add(Me.button1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FormEnhancement"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Enhancement/Details"
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar3, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar4, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar5, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar6, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private WithEvents button1 As Windows.Forms.Button
    Private WithEvents pictureBox1 As Windows.Forms.PictureBox
    Private WithEvents label1 As Windows.Forms.Label
    Private WithEvents trackBar1 As Windows.Forms.TrackBar
    Public WithEvents textBox1 As Windows.Forms.TextBox
    Private WithEvents label2 As Windows.Forms.Label
    Private WithEvents trackBar2 As Windows.Forms.TrackBar
    Public WithEvents textBox2 As Windows.Forms.TextBox
    Private WithEvents label3 As Windows.Forms.Label
    Private WithEvents trackBar3 As Windows.Forms.TrackBar
    Public WithEvents textBox3 As Windows.Forms.TextBox
    Private WithEvents label4 As Windows.Forms.Label
    Private WithEvents trackBar4 As Windows.Forms.TrackBar
    Public WithEvents textBox4 As Windows.Forms.TextBox
    Private WithEvents label5 As Windows.Forms.Label
    Private WithEvents trackBar5 As Windows.Forms.TrackBar
    Public WithEvents textBox5 As Windows.Forms.TextBox
    Private WithEvents label6 As Windows.Forms.Label
    Private WithEvents trackBar6 As Windows.Forms.TrackBar
    Public WithEvents textBox6 As Windows.Forms.TextBox
End Class

