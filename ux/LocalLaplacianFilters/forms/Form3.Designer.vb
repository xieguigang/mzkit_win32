
    Partial Class Form3
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
            Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(Form3))
            label2 = New Windows.Forms.Label()
            trackBar2 = New Windows.Forms.TrackBar()
            textBox2 = New Windows.Forms.TextBox()
            label1 = New Windows.Forms.Label()
            trackBar1 = New Windows.Forms.TrackBar()
            textBox1 = New Windows.Forms.TextBox()
            pictureBox1 = New Windows.Forms.PictureBox()
            button1 = New Windows.Forms.Button()
            CType(trackBar2, ComponentModel.ISupportInitialize).BeginInit()
            CType(trackBar1, ComponentModel.ISupportInitialize).BeginInit()
            CType(pictureBox1, ComponentModel.ISupportInitialize).BeginInit()
            SuspendLayout()
            ' 
            ' label2
            ' 
            label2.AutoSize = True
            label2.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            label2.Location = New Drawing.Point(7, 296)
            label2.Name = "label2"
            label2.Size = New Drawing.Size(52, 15)
            label2.TabIndex = 21
            label2.Text = "Strength"
            ' 
            ' trackBar2
            ' 
            trackBar2.AutoSize = False
            trackBar2.Location = New Drawing.Point(3, 317)
            trackBar2.Maximum = 100
            trackBar2.Name = "trackBar2"
            trackBar2.Size = New Drawing.Size(243, 26)
            trackBar2.TabIndex = 20
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
            textBox2.TabIndex = 19
            textBox2.Text = "0"
            textBox2.TextAlign = Windows.Forms.HorizontalAlignment.Right
            ' 
            ' label1
            ' 
            label1.AutoSize = True
            label1.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            label1.Location = New Drawing.Point(7, 246)
            label1.Name = "label1"
            label1.Size = New Drawing.Size(91, 15)
            label1.TabIndex = 18
            label1.Text = "Temperature (K)"
            ' 
            ' trackBar1
            ' 
            trackBar1.AutoSize = False
            trackBar1.Location = New Drawing.Point(3, 267)
            trackBar1.Maximum = 100
            trackBar1.Name = "trackBar1"
            trackBar1.Size = New Drawing.Size(243, 26)
            trackBar1.TabIndex = 17
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
            textBox1.TabIndex = 16
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
            pictureBox1.TabIndex = 15
            pictureBox1.TabStop = False
            ' 
            ' button1
            ' 
            button1.BackColor = Drawing.SystemColors.ControlLight
            button1.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            button1.Location = New Drawing.Point(12, 349)
            button1.Name = "button1"
            button1.Size = New Drawing.Size(225, 28)
            button1.TabIndex = 1
            button1.Text = "Apply"
            button1.UseVisualStyleBackColor = False
            AddHandler button1.Click, New EventHandler(AddressOf button1_Click)
            ' 
            ' Form3
            ' 
            AutoScaleDimensions = New Drawing.SizeF(6F, 13F)
            AutoScaleMode = Windows.Forms.AutoScaleMode.Font
            ClientSize = New Drawing.Size(249, 387)
            Controls.Add(label2)
            Controls.Add(trackBar2)
            Controls.Add(textBox2)
            Controls.Add(label1)
            Controls.Add(trackBar1)
            Controls.Add(textBox1)
            Controls.Add(pictureBox1)
            Controls.Add(button1)
            FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
            ' Icon = CType(resources.GetObject("$this.Icon"), Drawing.Icon)
            MaximizeBox = False
            MinimizeBox = False
            Name = "Form3"
            StartPosition = Windows.Forms.FormStartPosition.CenterParent
            Text = "Temperature"
            AddHandler Load, New EventHandler(AddressOf Form3_Load)
            CType(trackBar2, ComponentModel.ISupportInitialize).EndInit()
            CType(trackBar1, ComponentModel.ISupportInitialize).EndInit()
            CType(pictureBox1, ComponentModel.ISupportInitialize).EndInit()
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
    End Class

