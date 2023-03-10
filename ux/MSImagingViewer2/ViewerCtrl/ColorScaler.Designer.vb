<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ColorScaler
    Inherits System.Windows.Forms.UserControl

    'UserControl 重写释放以清理组件列表。
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

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.picUpperbound = New System.Windows.Forms.PictureBox()
        Me.picLowerbound = New System.Windows.Forms.PictureBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        CType(Me.picUpperbound, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picLowerbound, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'picUpperbound
        '
        Me.picUpperbound.BackColor = System.Drawing.Color.Black
        Me.picUpperbound.Cursor = System.Windows.Forms.Cursors.NoMoveVert
        Me.picUpperbound.Location = New System.Drawing.Point(2, 2)
        Me.picUpperbound.Name = "picUpperbound"
        Me.picUpperbound.Size = New System.Drawing.Size(152, 10)
        Me.picUpperbound.TabIndex = 0
        Me.picUpperbound.TabStop = False
        '
        'picLowerbound
        '
        Me.picLowerbound.BackColor = System.Drawing.Color.Black
        Me.picLowerbound.Cursor = System.Windows.Forms.Cursors.NoMoveVert
        Me.picLowerbound.Location = New System.Drawing.Point(3, 576)
        Me.picLowerbound.Name = "picLowerbound"
        Me.picLowerbound.Size = New System.Drawing.Size(151, 10)
        Me.picLowerbound.TabIndex = 1
        Me.picLowerbound.TabStop = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.PictureBox1.Location = New System.Drawing.Point(3, 30)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(149, 508)
        Me.PictureBox1.TabIndex = 2
        Me.PictureBox1.TabStop = False
        '
        'ColorScaler
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.picLowerbound)
        Me.Controls.Add(Me.picUpperbound)
        Me.Name = "ColorScaler"
        Me.Size = New System.Drawing.Size(155, 589)
        CType(Me.picUpperbound, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picLowerbound, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents picUpperbound As PictureBox
    Friend WithEvents picLowerbound As PictureBox
    Friend WithEvents PictureBox1 As PictureBox
End Class
