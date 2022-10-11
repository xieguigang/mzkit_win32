
    Partial Class Form5
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

#Region "Windows Form Designer generated code"

        ''' <summary>
        ''' Required method for Designer support - do not modify
        ''' the contents of this method with the code editor.
        ''' </summary>
        Private Sub InitializeComponent()
            Dim resources As ComponentModel.ComponentResourceManager = New ComponentModel.ComponentResourceManager(GetType(Form5))
            trackBar1 = New Windows.Forms.TrackBar()
            textBox2 = New Windows.Forms.TextBox()
            label1 = New Windows.Forms.Label()
            pictureBox1 = New Windows.Forms.PictureBox()
            button1 = New Windows.Forms.Button()
            CType(trackBar1, ComponentModel.ISupportInitialize).BeginInit()
            CType(pictureBox1, ComponentModel.ISupportInitialize).BeginInit()
            SuspendLayout()
            ' 
            ' trackBar1
            ' 
            trackBar1.AutoSize = False
            trackBar1.Location = New Drawing.Point(3, 267)
            trackBar1.Maximum = 90
            trackBar1.Name = "trackBar1"
            trackBar1.Size = New Drawing.Size(243, 26)
            trackBar1.TabIndex = 28
            trackBar1.TickStyle = Windows.Forms.TickStyle.None
            trackBar1.Value = 45
            AddHandler trackBar1.Scroll, New EventHandler(AddressOf trackBar1_Scroll)
            ' 
            ' textBox2
            ' 
            textBox2.BackColor = Drawing.Color.White
            textBox2.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            textBox2.Location = New Drawing.Point(201, 243)
            textBox2.Name = "textBox2"
            textBox2.ReadOnly = True
            textBox2.Size = New Drawing.Size(36, 23)
            textBox2.TabIndex = 27
            textBox2.Text = "0.55"
            ' 
            ' label1
            ' 
            label1.AutoSize = True
            label1.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            label1.Location = New Drawing.Point(7, 246)
            label1.Name = "label1"
            label1.Size = New Drawing.Size(40, 15)
            label1.TabIndex = 26
            label1.Text = "Sigma"
            ' 
            ' pictureBox1
            ' 
            pictureBox1.BackColor = Drawing.SystemColors.ControlLight
            pictureBox1.Location = New Drawing.Point(12, 12)
            pictureBox1.Name = "pictureBox1"
            pictureBox1.Size = New Drawing.Size(225, 225)
            pictureBox1.SizeMode = Windows.Forms.PictureBoxSizeMode.CenterImage
            pictureBox1.TabIndex = 23
            pictureBox1.TabStop = False
            ' 
            ' button1
            ' 
            button1.BackColor = Drawing.SystemColors.ControlLight
            button1.Font = New Drawing.Font("Segoe UI", 9F, Drawing.FontStyle.Regular, Drawing.GraphicsUnit.Point, 204)
            button1.Location = New Drawing.Point(12, 299)
            button1.Name = "button1"
            button1.Size = New Drawing.Size(225, 28)
            button1.TabIndex = 1
            button1.Text = "Apply"
            button1.UseVisualStyleBackColor = False
            AddHandler button1.Click, New EventHandler(AddressOf button1_Click)
            ' 
            ' Form5
            ' 
            AutoScaleDimensions = New Drawing.SizeF(6F, 13F)
            AutoScaleMode = Windows.Forms.AutoScaleMode.Font
            ClientSize = New Drawing.Size(249, 339)
            Controls.Add(trackBar1)
            Controls.Add(textBox2)
            Controls.Add(label1)
            Controls.Add(pictureBox1)
            Controls.Add(button1)
            FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
            '  Icon = CType(resources.GetObject("$this.Icon"), Drawing.Icon)
            MaximizeBox = False
            MinimizeBox = False
            Name = "Form5"
            StartPosition = Windows.Forms.FormStartPosition.CenterParent
            Text = "Exposure fusion"
            AddHandler Load, New EventHandler(AddressOf Form5_Load)
            CType(trackBar1, ComponentModel.ISupportInitialize).EndInit()
            CType(pictureBox1, ComponentModel.ISupportInitialize).EndInit()
            ResumeLayout(False)
            PerformLayout()

        End Sub

#End Region
        Private trackBar1 As Windows.Forms.TrackBar
        Public textBox2 As Windows.Forms.TextBox
        Private label1 As Windows.Forms.Label
        Private pictureBox1 As Windows.Forms.PictureBox
        Private button1 As Windows.Forms.Button
    End Class

