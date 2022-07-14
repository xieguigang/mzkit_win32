<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class InputIonRGB
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
        Me.cB = New System.Windows.Forms.ComboBox()
        Me.cG = New System.Windows.Forms.ComboBox()
        Me.cR = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.cB)
        Me.GroupBox1.Controls.Add(Me.cG)
        Me.GroupBox1.Controls.Add(Me.cR)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Controls.Add(Me.Label5)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 12)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(303, 181)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "R,G,B Channels"
        '
        'cB
        '
        Me.cB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cB.FormattingEnabled = True
        Me.cB.Items.AddRange(New Object() {"None"})
        Me.cB.Location = New System.Drawing.Point(132, 129)
        Me.cB.Name = "cB"
        Me.cB.Size = New System.Drawing.Size(132, 20)
        Me.cB.TabIndex = 8
        '
        'cG
        '
        Me.cG.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cG.FormattingEnabled = True
        Me.cG.Items.AddRange(New Object() {"None"})
        Me.cG.Location = New System.Drawing.Point(132, 83)
        Me.cG.Name = "cG"
        Me.cG.Size = New System.Drawing.Size(132, 20)
        Me.cG.TabIndex = 7
        '
        'cR
        '
        Me.cR.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cR.FormattingEnabled = True
        Me.cR.Items.AddRange(New Object() {"None"})
        Me.cR.Location = New System.Drawing.Point(132, 37)
        Me.cR.Name = "cR"
        Me.cR.Size = New System.Drawing.Size(132, 20)
        Me.cR.TabIndex = 6
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.BackColor = System.Drawing.Color.Blue
        Me.Label6.Location = New System.Drawing.Point(47, 132)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(53, 12)
        Me.Label6.TabIndex = 5
        Me.Label6.Text = "        "
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.BackColor = System.Drawing.Color.Green
        Me.Label5.Location = New System.Drawing.Point(47, 86)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(53, 12)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "        "
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.BackColor = System.Drawing.Color.Red
        Me.Label4.Location = New System.Drawing.Point(47, 40)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(53, 12)
        Me.Label4.TabIndex = 3
        Me.Label4.Text = "        "
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label3.Location = New System.Drawing.Point(29, 132)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(12, 12)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "B"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label2.Location = New System.Drawing.Point(29, 86)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(12, 12)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "G"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("宋体", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.Label1.Location = New System.Drawing.Point(29, 40)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(12, 12)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "R"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(221, 207)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 1
        Me.Button1.Text = "OK"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(131, 207)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 2
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'InputIonRGB
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(325, 242)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Name = "InputIonRGB"
        Me.Text = "Set RGB Channels"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents cB As ComboBox
    Friend WithEvents cG As ComboBox
    Friend WithEvents cR As ComboBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
End Class
