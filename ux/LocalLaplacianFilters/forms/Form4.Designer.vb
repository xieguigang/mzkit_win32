Namespace LaplacianHDR
    Partial Class Form4
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
            Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(Form4))
            label2 = New Windows.Forms.Label()
            trackBar2 = New Windows.Forms.TrackBar()
            textBox2 = New Windows.Forms.TextBox()
            label1 = New Windows.Forms.Label()
            trackBar1 = New Windows.Forms.TrackBar()
            textBox1 = New Windows.Forms.TextBox()
            pictureBox1 = New Windows.Forms.PictureBox()
            button1 = New Windows.Forms.Button()
            label3 = New Windows.Forms.Label()
            trackBar3 = New Windows.Forms.TrackBar()
            textBox3 = New Windows.Forms.TextBox()
            CType(trackBar2, ComponentModel.ISupportInitialize).BeginInit()
            CType(trackBar1, ComponentModel.ISupportInitialize).BeginInit()
            CType(pictureBox1, ComponentModel.ISupportInitialize).BeginInit()
            CType(trackBar3, ComponentModel.ISupportInitialize).BeginInit()
            SuspendLayout()
            ' 
            ' label2
            ' 
            label2.AutoSize = True
            label2.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            label2.Location = New Drawing.Point(7, 296)
            label2.Name = "label2"
            label2.Size = New Drawing.Size(61, 15)
            label2.TabIndex = 23
            label2.Text = "Saturation"
            ' 
            ' trackBar2
            ' 
            trackBar2.AutoSize = False
            trackBar2.Location = New Drawing.Point(3, 317)
            trackBar2.Maximum = 100
            trackBar2.Minimum = -100
            trackBar2.Name = "trackBar2"
            trackBar2.Size = New Drawing.Size(243, 26)
            trackBar2.TabIndex = 22
            trackBar2.TickStyle = Windows.Forms.TickStyle.None
            AddHandler trackBar2.Scroll, New EventHandler(AddressOf trackBar2_Scroll)
            ' 
            ' textBox2
            ' 
            textBox2.BackColor = Drawing.Color.White
            textBox2.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            textBox2.Location = New Drawing.Point(201, 293)
            textBox2.Name = "textBox2"
            textBox2.ReadOnly = True
            textBox2.Size = New Drawing.Size(36, 23)
            textBox2.TabIndex = 21
            textBox2.Text = "0"
            textBox2.TextAlign = Windows.Forms.HorizontalAlignment.Right
            ' 
            ' label1
            ' 
            label1.AutoSize = True
            label1.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            label1.Location = New Drawing.Point(7, 246)
            label1.Name = "label1"
            label1.Size = New Drawing.Size(29, 15)
            label1.TabIndex = 20
            label1.Text = "Hue"
            ' 
            ' trackBar1
            ' 
            trackBar1.AutoSize = False
            trackBar1.Location = New Drawing.Point(3, 267)
            trackBar1.Maximum = 180
            trackBar1.Minimum = -180
            trackBar1.Name = "trackBar1"
            trackBar1.Size = New Drawing.Size(243, 26)
            trackBar1.TabIndex = 19
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
            textBox1.TabIndex = 18
            textBox1.Text = "0"
            textBox1.TextAlign = Windows.Forms.HorizontalAlignment.Right
            ' 
            ' pictureBox1
            ' 
            pictureBox1.BackColor = Drawing.SystemColors.ControlLight
            pictureBox1.Location = New Drawing.Point(12, 12)
            pictureBox1.Name = "pictureBox1"
            pictureBox1.Size = New Drawing.Size(225, 225)
            pictureBox1.SizeMode = Windows.Forms.PictureBoxSizeMode.CenterImage
            pictureBox1.TabIndex = 17
            pictureBox1.TabStop = False
            ' 
            ' button1
            ' 
            button1.BackColor = Drawing.SystemColors.ControlLight
            button1.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            button1.Location = New Drawing.Point(12, 394)
            button1.Name = "button1"
            button1.Size = New Drawing.Size(225, 28)
            button1.TabIndex = 1
            button1.Text = "Apply"
            button1.UseVisualStyleBackColor = False
            AddHandler button1.Click, New EventHandler(AddressOf button1_Click)
            ' 
            ' label3
            ' 
            label3.AutoSize = True
            label3.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            label3.Location = New Drawing.Point(7, 341)
            label3.Name = "label3"
            label3.Size = New Drawing.Size(57, 15)
            label3.TabIndex = 26
            label3.Text = "Lightness"
            ' 
            ' trackBar3
            ' 
            trackBar3.AutoSize = False
            trackBar3.Location = New Drawing.Point(3, 362)
            trackBar3.Maximum = 100
            trackBar3.Minimum = -100
            trackBar3.Name = "trackBar3"
            trackBar3.Size = New Drawing.Size(243, 26)
            trackBar3.TabIndex = 25
            trackBar3.TickStyle = Windows.Forms.TickStyle.None
            AddHandler trackBar3.Scroll, New EventHandler(AddressOf trackBar3_Scroll)
            ' 
            ' textBox3
            ' 
            textBox3.BackColor = Drawing.Color.White
            textBox3.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            textBox3.Location = New Drawing.Point(201, 338)
            textBox3.Name = "textBox3"
            textBox3.ReadOnly = True
            textBox3.Size = New Drawing.Size(36, 23)
            textBox3.TabIndex = 24
            textBox3.Text = "0"
            textBox3.TextAlign = Windows.Forms.HorizontalAlignment.Right
            ' 
            ' Form4
            ' 
            AutoScaleDimensions = New Drawing.SizeF(6F, 13F)
            AutoScaleMode = Windows.Forms.AutoScaleMode.Font
            ClientSize = New Drawing.Size(249, 435)
            Controls.Add(label3)
            Controls.Add(trackBar3)
            Controls.Add(textBox3)
            Controls.Add(button1)
            Controls.Add(label2)
            Controls.Add(trackBar2)
            Controls.Add(textBox2)
            Controls.Add(label1)
            Controls.Add(trackBar1)
            Controls.Add(textBox1)
            Controls.Add(pictureBox1)
            FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
            '  Icon = CType(resources.GetObject("$this.Icon"), Drawing.Icon)
            MaximizeBox = False
            MinimizeBox = False
            Name = "Form4"
            StartPosition = Windows.Forms.FormStartPosition.CenterParent
            Text = "Hue/Saturation/Lightness"
            AddHandler Load, New EventHandler(AddressOf Form4_Load)
            CType(trackBar2, ComponentModel.ISupportInitialize).EndInit()
            CType(trackBar1, ComponentModel.ISupportInitialize).EndInit()
            CType(pictureBox1, ComponentModel.ISupportInitialize).EndInit()
            CType(trackBar3, ComponentModel.ISupportInitialize).EndInit()
            ResumeLayout(False)
            PerformLayout()

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
End Namespace
