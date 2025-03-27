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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ColorScaler))
        Me.picUpperbound = New System.Windows.Forms.PictureBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ResetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.picLowerbound = New System.Windows.Forms.PictureBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SetRangeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        CType(Me.picUpperbound, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.picLowerbound, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'picUpperbound
        '
        Me.picUpperbound.BackColor = System.Drawing.Color.Black
        Me.picUpperbound.ContextMenuStrip = Me.ContextMenuStrip1
        Me.picUpperbound.Cursor = System.Windows.Forms.Cursors.NoMoveVert
        Me.picUpperbound.Location = New System.Drawing.Point(2, 2)
        Me.picUpperbound.Name = "picUpperbound"
        Me.picUpperbound.Size = New System.Drawing.Size(152, 10)
        Me.picUpperbound.TabIndex = 0
        Me.picUpperbound.TabStop = False
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SetRangeToolStripMenuItem, Me.ToolStripMenuItem1, Me.ResetToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(181, 76)
        '
        'ResetToolStripMenuItem
        '
        Me.ResetToolStripMenuItem.Image = CType(resources.GetObject("ResetToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ResetToolStripMenuItem.Name = "ResetToolStripMenuItem"
        Me.ResetToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ResetToolStripMenuItem.Text = "Reset"
        Me.ResetToolStripMenuItem.ToolTipText = "Reset the intensity scale range to the original [min, max] range of the raw data " &
    "input."
        '
        'picLowerbound
        '
        Me.picLowerbound.BackColor = System.Drawing.Color.Black
        Me.picLowerbound.ContextMenuStrip = Me.ContextMenuStrip1
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
        Me.PictureBox1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.PictureBox1.Location = New System.Drawing.Point(3, 30)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(149, 508)
        Me.PictureBox1.TabIndex = 2
        Me.PictureBox1.TabStop = False
        '
        'SetRangeToolStripMenuItem
        '
        Me.SetRangeToolStripMenuItem.AutoToolTip = True
        Me.SetRangeToolStripMenuItem.Image = CType(resources.GetObject("SetRangeToolStripMenuItem.Image"), System.Drawing.Image)
        Me.SetRangeToolStripMenuItem.Name = "SetRangeToolStripMenuItem"
        Me.SetRangeToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.SetRangeToolStripMenuItem.Text = "Set Range"
        Me.SetRangeToolStripMenuItem.ToolTipText = "Set Intensity Scale Range For the Heatmap Rendering"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(177, 6)
        '
        'ColorScaler
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ContextMenuStrip = Me.ContextMenuStrip1
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.picLowerbound)
        Me.Controls.Add(Me.picUpperbound)
        Me.DoubleBuffered = True
        Me.Name = "ColorScaler"
        Me.Size = New System.Drawing.Size(155, 589)
        CType(Me.picUpperbound, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        CType(Me.picLowerbound, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents picUpperbound As PictureBox
    Friend WithEvents picLowerbound As PictureBox
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ResetToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents SetRangeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
End Class
