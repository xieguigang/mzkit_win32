
    Partial Class FormAdjust
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormAdjust))
        Me.label2 = New System.Windows.Forms.Label()
        Me.trackBar2 = New System.Windows.Forms.TrackBar()
        Me.textBox2 = New System.Windows.Forms.TextBox()
        Me.label1 = New System.Windows.Forms.Label()
        Me.trackBar1 = New System.Windows.Forms.TrackBar()
        Me.textBox1 = New System.Windows.Forms.TextBox()
        Me.pictureBox1 = New System.Windows.Forms.PictureBox()
        Me.button1 = New System.Windows.Forms.Button()
        Me.label3 = New System.Windows.Forms.Label()
        Me.trackBar3 = New System.Windows.Forms.TrackBar()
        Me.textBox3 = New System.Windows.Forms.TextBox()
        CType(Me.trackBar2, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.trackBar3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'label2
        '
        Me.label2.AutoSize = True
        Me.label2.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label2.Location = New System.Drawing.Point(7, 296)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(61, 15)
        Me.label2.TabIndex = 23
        Me.label2.Text = "Saturation"
        '
        'trackBar2
        '
        Me.trackBar2.AutoSize = False
        Me.trackBar2.Location = New System.Drawing.Point(3, 317)
        Me.trackBar2.Maximum = 100
        Me.trackBar2.Minimum = -100
        Me.trackBar2.Name = "trackBar2"
        Me.trackBar2.Size = New System.Drawing.Size(243, 26)
        Me.trackBar2.TabIndex = 22
        Me.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'textBox2
        '
        Me.textBox2.BackColor = System.Drawing.Color.White
        Me.textBox2.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.textBox2.Location = New System.Drawing.Point(201, 293)
        Me.textBox2.Name = "textBox2"
        Me.textBox2.ReadOnly = True
        Me.textBox2.Size = New System.Drawing.Size(36, 23)
        Me.textBox2.TabIndex = 21
        Me.textBox2.Text = "0"
        Me.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label1.Location = New System.Drawing.Point(7, 246)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(29, 15)
        Me.label1.TabIndex = 20
        Me.label1.Text = "Hue"
        '
        'trackBar1
        '
        Me.trackBar1.AutoSize = False
        Me.trackBar1.Location = New System.Drawing.Point(3, 267)
        Me.trackBar1.Maximum = 180
        Me.trackBar1.Minimum = -180
        Me.trackBar1.Name = "trackBar1"
        Me.trackBar1.Size = New System.Drawing.Size(243, 26)
        Me.trackBar1.TabIndex = 19
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
        Me.textBox1.TabIndex = 18
        Me.textBox1.Text = "0"
        Me.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'pictureBox1
        '
        Me.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.pictureBox1.Name = "pictureBox1"
        Me.pictureBox1.Size = New System.Drawing.Size(225, 225)
        Me.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
        Me.pictureBox1.TabIndex = 17
        Me.pictureBox1.TabStop = False
        '
        'button1
        '
        Me.button1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.button1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.button1.Location = New System.Drawing.Point(12, 394)
        Me.button1.Name = "button1"
        Me.button1.Size = New System.Drawing.Size(225, 28)
        Me.button1.TabIndex = 1
        Me.button1.Text = "Apply"
        Me.button1.UseVisualStyleBackColor = False
        '
        'label3
        '
        Me.label3.AutoSize = True
        Me.label3.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.label3.Location = New System.Drawing.Point(7, 341)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(57, 15)
        Me.label3.TabIndex = 26
        Me.label3.Text = "Lightness"
        '
        'trackBar3
        '
        Me.trackBar3.AutoSize = False
        Me.trackBar3.Location = New System.Drawing.Point(3, 362)
        Me.trackBar3.Maximum = 100
        Me.trackBar3.Minimum = -100
        Me.trackBar3.Name = "trackBar3"
        Me.trackBar3.Size = New System.Drawing.Size(243, 26)
        Me.trackBar3.TabIndex = 25
        Me.trackBar3.TickStyle = System.Windows.Forms.TickStyle.None
        '
        'textBox3
        '
        Me.textBox3.BackColor = System.Drawing.Color.White
        Me.textBox3.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.textBox3.Location = New System.Drawing.Point(201, 338)
        Me.textBox3.Name = "textBox3"
        Me.textBox3.ReadOnly = True
        Me.textBox3.Size = New System.Drawing.Size(36, 23)
        Me.textBox3.TabIndex = 24
        Me.textBox3.Text = "0"
        Me.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FormAdjust
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(249, 435)
        Me.Controls.Add(Me.label3)
        Me.Controls.Add(Me.trackBar3)
        Me.Controls.Add(Me.textBox3)
        Me.Controls.Add(Me.button1)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.trackBar2)
        Me.Controls.Add(Me.textBox2)
        Me.Controls.Add(Me.label1)
        Me.Controls.Add(Me.trackBar1)
        Me.Controls.Add(Me.textBox1)
        Me.Controls.Add(Me.pictureBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "FormAdjust"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Hue/Saturation/Lightness"
        CType(Me.trackBar2, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.trackBar3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region
    Private label2 As Windows.Forms.Label
        Private trackBar2 As Windows.Forms.TrackBar
        Public textBox2 As Windows.Forms.TextBox
        Private label1 As Windows.Forms.Label
        Private trackBar1 As Windows.Forms.TrackBar
        Public textBox1 As Windows.Forms.TextBox
        Private pictureBox1 As Windows.Forms.PictureBox
        Private button1 As Windows.Forms.Button
        Private label3 As Windows.Forms.Label
        Private trackBar3 As Windows.Forms.TrackBar
        Public textBox3 As Windows.Forms.TextBox
    End Class

