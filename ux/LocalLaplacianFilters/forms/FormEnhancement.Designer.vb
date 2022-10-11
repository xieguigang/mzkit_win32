
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
            Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(FormEnhancement))
            button1 = New Windows.Forms.Button()
            pictureBox1 = New Windows.Forms.PictureBox()
            label1 = New Windows.Forms.Label()
            trackBar1 = New Windows.Forms.TrackBar()
            textBox1 = New Windows.Forms.TextBox()
            label2 = New Windows.Forms.Label()
            trackBar2 = New Windows.Forms.TrackBar()
            textBox2 = New Windows.Forms.TextBox()
            label3 = New Windows.Forms.Label()
            trackBar3 = New Windows.Forms.TrackBar()
            textBox3 = New Windows.Forms.TextBox()
            label4 = New Windows.Forms.Label()
            trackBar4 = New Windows.Forms.TrackBar()
            textBox4 = New Windows.Forms.TextBox()
            label5 = New Windows.Forms.Label()
            trackBar5 = New Windows.Forms.TrackBar()
            textBox5 = New Windows.Forms.TextBox()
            label6 = New Windows.Forms.Label()
            trackBar6 = New Windows.Forms.TrackBar()
            textBox6 = New Windows.Forms.TextBox()
            CType(pictureBox1, ComponentModel.ISupportInitialize).BeginInit()
            CType(trackBar1, ComponentModel.ISupportInitialize).BeginInit()
            CType(trackBar2, ComponentModel.ISupportInitialize).BeginInit()
            CType(trackBar3, ComponentModel.ISupportInitialize).BeginInit()
            CType(trackBar4, ComponentModel.ISupportInitialize).BeginInit()
            CType(trackBar5, ComponentModel.ISupportInitialize).BeginInit()
            CType(trackBar6, ComponentModel.ISupportInitialize).BeginInit()
            SuspendLayout()
            ' 
            ' button1
            ' 
            button1.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            button1.Location = New Drawing.Point(12, 554)
            button1.Name = "button1"
            button1.Size = New Drawing.Size(225, 28)
            button1.TabIndex = 1
            button1.Text = "Apply"
            button1.UseVisualStyleBackColor = True
            AddHandler button1.Click, New EventHandler(AddressOf button1_Click)
            ' 
            ' pictureBox1
            ' 
            pictureBox1.BackColor = Drawing.SystemColors.ControlLight
            pictureBox1.Location = New Drawing.Point(12, 12)
            pictureBox1.Name = "pictureBox1"
            pictureBox1.Size = New Drawing.Size(225, 225)
            pictureBox1.SizeMode = Windows.Forms.PictureBoxSizeMode.CenterImage
            pictureBox1.TabIndex = 2
            pictureBox1.TabStop = False
            ' 
            ' label1
            ' 
            label1.AutoSize = True
            label1.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            label1.Location = New Drawing.Point(7, 246)
            label1.Name = "label1"
            label1.Size = New Drawing.Size(90, 15)
            label1.TabIndex = 10
            label1.Text = "Lights/shadows"
            ' 
            ' trackBar1
            ' 
            trackBar1.AutoSize = False
            trackBar1.Location = New Drawing.Point(3, 267)
            trackBar1.Maximum = 100
            trackBar1.Minimum = -100
            trackBar1.Name = "trackBar1"
            trackBar1.Size = New Drawing.Size(243, 26)
            trackBar1.TabIndex = 9
            trackBar1.TickStyle = Windows.Forms.TickStyle.None
            AddHandler trackBar1.Scroll, New EventHandler(AddressOf trackBar1_Scroll)
            ' 
            ' textBox1
            ' 
            textBox1.BackColor = Drawing.Color.White
            textBox1.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            textBox1.Location = New Drawing.Point(201, 243)
            textBox1.Name = "textBox1"
            textBox1.ReadOnly = True
            textBox1.Size = New Drawing.Size(36, 23)
            textBox1.TabIndex = 8
            textBox1.Text = "0"
            textBox1.TextAlign = Windows.Forms.HorizontalAlignment.Right
            ' 
            ' label2
            ' 
            label2.AutoSize = True
            label2.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            label2.Location = New Drawing.Point(7, 296)
            label2.Name = "label2"
            label2.Size = New Drawing.Size(40, 15)
            label2.TabIndex = 13
            label2.Text = "Sigma"
            ' 
            ' trackBar2
            ' 
            trackBar2.AutoSize = False
            trackBar2.Location = New Drawing.Point(3, 317)
            trackBar2.Maximum = 199
            trackBar2.Name = "trackBar2"
            trackBar2.Size = New Drawing.Size(243, 26)
            trackBar2.TabIndex = 12
            trackBar2.TickStyle = Windows.Forms.TickStyle.None
            trackBar2.Value = 49
            AddHandler trackBar2.Scroll, New EventHandler(AddressOf trackBar2_Scroll)
            ' 
            ' textBox2
            ' 
            textBox2.BackColor = Drawing.Color.White
            textBox2.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 0)
            textBox2.Location = New Drawing.Point(201, 293)
            textBox2.Name = "textBox2"
            textBox2.ReadOnly = True
            textBox2.Size = New Drawing.Size(36, 23)
            textBox2.TabIndex = 11
            textBox2.Text = "0.05"
            textBox2.TextAlign = Windows.Forms.HorizontalAlignment.Right
            ' 
            ' label3
            ' 
            label3.AutoSize = True
            label3.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            label3.Location = New Drawing.Point(7, 346)
            label3.Name = "label3"
            label3.Size = New Drawing.Size(35, 15)
            label3.TabIndex = 16
            label3.Text = "Steps"
            ' 
            ' trackBar3
            ' 
            trackBar3.AutoSize = False
            trackBar3.Location = New Drawing.Point(3, 367)
            trackBar3.Maximum = 100
            trackBar3.Minimum = 1
            trackBar3.Name = "trackBar3"
            trackBar3.Size = New Drawing.Size(243, 26)
            trackBar3.TabIndex = 15
            trackBar3.TickStyle = Windows.Forms.TickStyle.None
            trackBar3.Value = 20
            AddHandler trackBar3.Scroll, New EventHandler(AddressOf trackBar3_Scroll)
            ' 
            ' textBox3
            ' 
            textBox3.BackColor = Drawing.Color.White
            textBox3.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            textBox3.Location = New Drawing.Point(201, 343)
            textBox3.Name = "textBox3"
            textBox3.ReadOnly = True
            textBox3.Size = New Drawing.Size(36, 23)
            textBox3.TabIndex = 14
            textBox3.Text = "20"
            textBox3.TextAlign = Windows.Forms.HorizontalAlignment.Right
            ' 
            ' label4
            ' 
            label4.AutoSize = True
            label4.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            label4.Location = New Drawing.Point(7, 396)
            label4.Name = "label4"
            label4.Size = New Drawing.Size(39, 15)
            label4.TabIndex = 19
            label4.Text = "Levels"
            ' 
            ' trackBar4
            ' 
            trackBar4.AutoSize = False
            trackBar4.Location = New Drawing.Point(3, 417)
            trackBar4.Maximum = 100
            trackBar4.Minimum = 1
            trackBar4.Name = "trackBar4"
            trackBar4.Size = New Drawing.Size(243, 26)
            trackBar4.TabIndex = 18
            trackBar4.TickStyle = Windows.Forms.TickStyle.None
            trackBar4.Value = 20
            AddHandler trackBar4.Scroll, New EventHandler(AddressOf trackBar4_Scroll)
            ' 
            ' textBox4
            ' 
            textBox4.BackColor = Drawing.Color.White
            textBox4.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            textBox4.Location = New Drawing.Point(201, 393)
            textBox4.Name = "textBox4"
            textBox4.ReadOnly = True
            textBox4.Size = New Drawing.Size(36, 23)
            textBox4.TabIndex = 17
            textBox4.Text = "20"
            textBox4.TextAlign = Windows.Forms.HorizontalAlignment.Right
            ' 
            ' label5
            ' 
            label5.AutoSize = True
            label5.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            label5.Location = New Drawing.Point(9, 446)
            label5.Name = "label5"
            label5.Size = New Drawing.Size(42, 15)
            label5.TabIndex = 22
            label5.Text = "Details"
            ' 
            ' trackBar5
            ' 
            trackBar5.AutoSize = False
            trackBar5.Location = New Drawing.Point(3, 467)
            trackBar5.Maximum = 100
            trackBar5.Minimum = -100
            trackBar5.Name = "trackBar5"
            trackBar5.Size = New Drawing.Size(243, 26)
            trackBar5.TabIndex = 21
            trackBar5.TickStyle = Windows.Forms.TickStyle.None
            AddHandler trackBar5.Scroll, New EventHandler(AddressOf trackBar5_Scroll)
            ' 
            ' textBox5
            ' 
            textBox5.BackColor = Drawing.Color.White
            textBox5.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            textBox5.Location = New Drawing.Point(201, 443)
            textBox5.Name = "textBox5"
            textBox5.ReadOnly = True
            textBox5.Size = New Drawing.Size(36, 23)
            textBox5.TabIndex = 20
            textBox5.Text = "0"
            textBox5.TextAlign = Windows.Forms.HorizontalAlignment.Right
            ' 
            ' label6
            ' 
            label6.AutoSize = True
            label6.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            label6.Location = New Drawing.Point(9, 501)
            label6.Name = "label6"
            label6.Size = New Drawing.Size(66, 15)
            label6.TabIndex = 25
            label6.Text = "Smoothing"
            ' 
            ' trackBar6
            ' 
            trackBar6.AutoSize = False
            trackBar6.Location = New Drawing.Point(3, 522)
            trackBar6.Maximum = 50
            trackBar6.Name = "trackBar6"
            trackBar6.Size = New Drawing.Size(243, 26)
            trackBar6.TabIndex = 24
            trackBar6.TickStyle = Windows.Forms.TickStyle.None
            AddHandler trackBar6.Scroll, New EventHandler(AddressOf trackBar6_Scroll)
            ' 
            ' textBox6
            ' 
            textBox6.BackColor = Drawing.Color.White
            textBox6.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            textBox6.Location = New Drawing.Point(201, 498)
            textBox6.Name = "textBox6"
            textBox6.ReadOnly = True
            textBox6.Size = New Drawing.Size(36, 23)
            textBox6.TabIndex = 23
            textBox6.Text = "0"
            textBox6.TextAlign = Windows.Forms.HorizontalAlignment.Right
            ' 
            ' Form2
            ' 
            AutoScaleDimensions = New Drawing.SizeF(6F, 13F)
            AutoScaleMode = Windows.Forms.AutoScaleMode.Font
            ClientSize = New Drawing.Size(249, 594)
            Controls.Add(label6)
            Controls.Add(trackBar6)
            Controls.Add(textBox6)
            Controls.Add(label5)
            Controls.Add(trackBar5)
            Controls.Add(textBox5)
            Controls.Add(label4)
            Controls.Add(trackBar4)
            Controls.Add(textBox4)
            Controls.Add(label3)
            Controls.Add(trackBar3)
            Controls.Add(textBox3)
            Controls.Add(label2)
            Controls.Add(trackBar2)
            Controls.Add(textBox2)
            Controls.Add(label1)
            Controls.Add(trackBar1)
            Controls.Add(textBox1)
            Controls.Add(pictureBox1)
            Controls.Add(button1)
            FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
            '  Icon = CType(resources.GetObject("$this.Icon"), Drawing.Icon)
            MaximizeBox = False
            MinimizeBox = False
            Name = "Form2"
            StartPosition = Windows.Forms.FormStartPosition.CenterParent
            Text = "Enhancement/Details"
            AddHandler Load, New EventHandler(AddressOf Form2_Load)
            CType(pictureBox1, ComponentModel.ISupportInitialize).EndInit()
            CType(trackBar1, ComponentModel.ISupportInitialize).EndInit()
            CType(trackBar2, ComponentModel.ISupportInitialize).EndInit()
            CType(trackBar3, ComponentModel.ISupportInitialize).EndInit()
            CType(trackBar4, ComponentModel.ISupportInitialize).EndInit()
            CType(trackBar5, ComponentModel.ISupportInitialize).EndInit()
            CType(trackBar6, ComponentModel.ISupportInitialize).EndInit()
            ResumeLayout(False)
            PerformLayout()

        End Sub

#End Region

        Private button1 As Windows.Forms.Button
        Private pictureBox1 As Windows.Forms.PictureBox
        Private label1 As Windows.Forms.Label
        Private trackBar1 As Windows.Forms.TrackBar
        Public textBox1 As Windows.Forms.TextBox
        Private label2 As Windows.Forms.Label
        Private trackBar2 As Windows.Forms.TrackBar
        Public textBox2 As Windows.Forms.TextBox
        Private label3 As Windows.Forms.Label
        Private trackBar3 As Windows.Forms.TrackBar
        Public textBox3 As Windows.Forms.TextBox
        Private label4 As Windows.Forms.Label
        Private trackBar4 As Windows.Forms.TrackBar
        Public textBox4 As Windows.Forms.TextBox
        Private label5 As Windows.Forms.Label
        Private trackBar5 As Windows.Forms.TrackBar
        Public textBox5 As Windows.Forms.TextBox
        Private label6 As Windows.Forms.Label
        Private trackBar6 As Windows.Forms.TrackBar
        Public textBox6 As Windows.Forms.TextBox
    End Class

