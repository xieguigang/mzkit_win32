Imports Mzkit_win32.BasicMDIForm.CommonDialogs

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class InputConfigTissueMap
    Inherits InputDialog

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtPrefix = New System.Windows.Forms.TextBox()
        Me.numOpacity = New System.Windows.Forms.TrackBar()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbColorSet = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtSpotSize = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.ckDashLine = New System.Windows.Forms.CheckBox()
        Me.lineColor = New System.Windows.Forms.PictureBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.numLineWidth = New System.Windows.Forms.NumericUpDown()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.ckShowPoints = New System.Windows.Forms.CheckBox()
        Me.pointColor = New System.Windows.Forms.PictureBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.numPointSize = New System.Windows.Forms.NumericUpDown()
        Me.Label5 = New System.Windows.Forms.Label()
        CType(Me.numOpacity, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        CType(Me.lineColor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numLineWidth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pointColor, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.numPointSize, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(251, 221)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 21)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "OK"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(152, 221)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 21)
        Me.Button2.TabIndex = 1
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(21, 28)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(89, 12)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Region Prefix:"
        '
        'txtPrefix
        '
        Me.txtPrefix.Location = New System.Drawing.Point(116, 25)
        Me.txtPrefix.Name = "txtPrefix"
        Me.txtPrefix.Size = New System.Drawing.Size(194, 21)
        Me.txtPrefix.TabIndex = 3
        '
        'numOpacity
        '
        Me.numOpacity.LargeChange = 25
        Me.numOpacity.Location = New System.Drawing.Point(116, 51)
        Me.numOpacity.Maximum = 100
        Me.numOpacity.Name = "numOpacity"
        Me.numOpacity.Size = New System.Drawing.Size(194, 45)
        Me.numOpacity.SmallChange = 5
        Me.numOpacity.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(46, 59)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(53, 12)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Opacity:"
        '
        'cbColorSet
        '
        Me.cbColorSet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbColorSet.FormattingEnabled = True
        Me.cbColorSet.Items.AddRange(New Object() {"Paper", "Clusters", "Material", "ConsoleColors", "TSF", "Rainbow"})
        Me.cbColorSet.Location = New System.Drawing.Point(116, 138)
        Me.cbColorSet.Name = "cbColorSet"
        Me.cbColorSet.Size = New System.Drawing.Size(194, 20)
        Me.cbColorSet.TabIndex = 6
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(14, 141)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(89, 12)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = "Color Pattern:"
        '
        'txtSpotSize
        '
        Me.txtSpotSize.Location = New System.Drawing.Point(116, 110)
        Me.txtSpotSize.Name = "txtSpotSize"
        Me.txtSpotSize.Size = New System.Drawing.Size(100, 21)
        Me.txtSpotSize.TabIndex = 9
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(30, 113)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(65, 12)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Spot Size:"
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(12, 12)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(334, 197)
        Me.TabControl1.TabIndex = 9
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.txtSpotSize)
        Me.TabPage1.Controls.Add(Me.Label1)
        Me.TabPage1.Controls.Add(Me.Label4)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.txtPrefix)
        Me.TabPage1.Controls.Add(Me.numOpacity)
        Me.TabPage1.Controls.Add(Me.Label3)
        Me.TabPage1.Controls.Add(Me.cbColorSet)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(326, 171)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Tissue Segmentation Map"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.ckDashLine)
        Me.TabPage2.Controls.Add(Me.lineColor)
        Me.TabPage2.Controls.Add(Me.Label8)
        Me.TabPage2.Controls.Add(Me.numLineWidth)
        Me.TabPage2.Controls.Add(Me.Label7)
        Me.TabPage2.Controls.Add(Me.ckShowPoints)
        Me.TabPage2.Controls.Add(Me.pointColor)
        Me.TabPage2.Controls.Add(Me.Label6)
        Me.TabPage2.Controls.Add(Me.numPointSize)
        Me.TabPage2.Controls.Add(Me.Label5)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(326, 171)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Polygon Editor Settings"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'ckDashLine
        '
        Me.ckDashLine.AutoSize = True
        Me.ckDashLine.Checked = True
        Me.ckDashLine.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ckDashLine.Location = New System.Drawing.Point(199, 101)
        Me.ckDashLine.Name = "ckDashLine"
        Me.ckDashLine.Size = New System.Drawing.Size(84, 16)
        Me.ckDashLine.TabIndex = 9
        Me.ckDashLine.Text = "Dash Line?"
        Me.ckDashLine.UseVisualStyleBackColor = True
        '
        'lineColor
        '
        Me.lineColor.Location = New System.Drawing.Point(116, 131)
        Me.lineColor.Name = "lineColor"
        Me.lineColor.Size = New System.Drawing.Size(43, 22)
        Me.lineColor.TabIndex = 8
        Me.lineColor.TabStop = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(35, 131)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(71, 12)
        Me.Label8.TabIndex = 7
        Me.Label8.Text = "Line Color:"
        '
        'numLineWidth
        '
        Me.numLineWidth.Location = New System.Drawing.Point(116, 101)
        Me.numLineWidth.Name = "numLineWidth"
        Me.numLineWidth.Size = New System.Drawing.Size(66, 21)
        Me.numLineWidth.TabIndex = 6
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(35, 102)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(71, 12)
        Me.Label7.TabIndex = 5
        Me.Label7.Text = "Line Width:"
        '
        'ckShowPoints
        '
        Me.ckShowPoints.AutoSize = True
        Me.ckShowPoints.Checked = True
        Me.ckShowPoints.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ckShowPoints.Location = New System.Drawing.Point(200, 22)
        Me.ckShowPoints.Name = "ckShowPoints"
        Me.ckShowPoints.Size = New System.Drawing.Size(96, 16)
        Me.ckShowPoints.TabIndex = 4
        Me.ckShowPoints.Text = "Show Points?"
        Me.ckShowPoints.UseVisualStyleBackColor = True
        '
        'pointColor
        '
        Me.pointColor.Location = New System.Drawing.Point(116, 50)
        Me.pointColor.Name = "pointColor"
        Me.pointColor.Size = New System.Drawing.Size(43, 22)
        Me.pointColor.TabIndex = 3
        Me.pointColor.TabStop = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(30, 55)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(77, 12)
        Me.Label6.TabIndex = 2
        Me.Label6.Text = "Point Color:"
        '
        'numPointSize
        '
        Me.numPointSize.Location = New System.Drawing.Point(116, 20)
        Me.numPointSize.Name = "numPointSize"
        Me.numPointSize.Size = New System.Drawing.Size(66, 21)
        Me.numPointSize.TabIndex = 1
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(36, 22)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(71, 12)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "Point Size:"
        '
        'InputConfigTissueMap
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(356, 255)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.Name = "InputConfigTissueMap"
        Me.Text = "Config Tissue Map Layer"
        CType(Me.numOpacity, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        CType(Me.lineColor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numLineWidth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pointColor, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.numPointSize, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents txtPrefix As TextBox
    Friend WithEvents numOpacity As TrackBar
    Friend WithEvents Label2 As Label
    Friend WithEvents cbColorSet As ComboBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents txtSpotSize As TextBox
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents pointColor As PictureBox
    Friend WithEvents Label6 As Label
    Friend WithEvents numPointSize As NumericUpDown
    Friend WithEvents Label5 As Label
    Friend WithEvents ckShowPoints As CheckBox
    Friend WithEvents ckDashLine As CheckBox
    Friend WithEvents lineColor As PictureBox
    Friend WithEvents Label8 As Label
    Friend WithEvents numLineWidth As NumericUpDown
    Friend WithEvents Label7 As Label
End Class
