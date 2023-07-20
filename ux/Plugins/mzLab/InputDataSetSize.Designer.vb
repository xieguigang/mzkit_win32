Imports Mzkit_win32.BasicMDIForm.CommonDialogs

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class InputDataSetSize
    Inherits InputDialog

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.TextBox5 = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TextBox4 = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TextBox6 = New System.Windows.Forms.TextBox()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.TextBox6)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.TextBox5)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.TextBox4)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.TextBox3)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.TextBox2)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.TextBox1)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Location = New System.Drawing.Point(10, 11)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(344, 217)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Config DataSet Size"
        '
        'TextBox5
        '
        Me.TextBox5.Location = New System.Drawing.Point(232, 98)
        Me.TextBox5.Name = "TextBox5"
        Me.TextBox5.Size = New System.Drawing.Size(89, 21)
        Me.TextBox5.TabIndex = 9
        Me.TextBox5.Text = "3500"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(19, 101)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(197, 12)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "Number of spectrum in each file:"
        '
        'TextBox4
        '
        Me.TextBox4.Location = New System.Drawing.Point(263, 176)
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.Size = New System.Drawing.Size(58, 21)
        Me.TextBox4.TabIndex = 7
        Me.TextBox4.Text = "1200"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(241, 180)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(11, 12)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "-"
        '
        'TextBox3
        '
        Me.TextBox3.Location = New System.Drawing.Point(176, 176)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(55, 21)
        Me.TextBox3.TabIndex = 5
        Me.TextBox3.Text = "0"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(95, 180)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(59, 12)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "RT range:"
        '
        'TextBox2
        '
        Me.TextBox2.Location = New System.Drawing.Point(207, 65)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(114, 21)
        Me.TextBox2.TabIndex = 3
        Me.TextBox2.Text = "100"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(53, 68)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(137, 12)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Number of Raw DataSet:"
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(207, 35)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(114, 21)
        Me.TextBox1.TabIndex = 1
        Me.TextBox1.Text = "10000"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(95, 38)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(95, 12)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Number of Ions:"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(256, 234)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "OK"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(166, 234)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 2
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(95, 139)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(65, 12)
        Me.Label6.TabIndex = 10
        Me.Label6.Text = "File Name:"
        '
        'TextBox6
        '
        Me.TextBox6.Location = New System.Drawing.Point(176, 136)
        Me.TextBox6.Name = "TextBox6"
        Me.TextBox6.Size = New System.Drawing.Size(145, 21)
        Me.TextBox6.TabIndex = 11
        Me.TextBox6.Text = "RawDATAfile"
        '
        'InputDataSetSize
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(364, 269)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "InputDataSetSize"
        Me.Text = "Set Validation Data Size"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label4 As Label
    Friend WithEvents TextBox3 As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents TextBox5 As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents TextBox4 As TextBox
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents TextBox6 As TextBox
    Friend WithEvents Label6 As Label
End Class
