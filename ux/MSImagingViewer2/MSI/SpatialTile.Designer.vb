<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SpatialTile
    Inherits System.Windows.Forms.UserControl

    'UserControl 重写释放以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SpatialTile))
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.LoadTissueImageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RemoveTissueImageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.EditLabelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetSpotColorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.DeleteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportSpatialMappingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.Label1 = New System.Windows.Forms.Label()
        Me.AnchorResize = New System.Windows.Forms.PictureBox()
        Me.RotateToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.AnchorResize, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LoadTissueImageToolStripMenuItem, Me.RemoveTissueImageToolStripMenuItem, Me.ToolStripMenuItem2, Me.EditLabelToolStripMenuItem, Me.RotateToolStripMenuItem, Me.SetSpotColorToolStripMenuItem, Me.ToolStripMenuItem1, Me.DeleteToolStripMenuItem, Me.ExportSpatialMappingToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        '
        'LoadTissueImageToolStripMenuItem
        '
        resources.ApplyResources(Me.LoadTissueImageToolStripMenuItem, "LoadTissueImageToolStripMenuItem")
        Me.LoadTissueImageToolStripMenuItem.Name = "LoadTissueImageToolStripMenuItem"
        '
        'RemoveTissueImageToolStripMenuItem
        '
        resources.ApplyResources(Me.RemoveTissueImageToolStripMenuItem, "RemoveTissueImageToolStripMenuItem")
        Me.RemoveTissueImageToolStripMenuItem.Name = "RemoveTissueImageToolStripMenuItem"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        resources.ApplyResources(Me.ToolStripMenuItem2, "ToolStripMenuItem2")
        '
        'EditLabelToolStripMenuItem
        '
        Me.EditLabelToolStripMenuItem.Name = "EditLabelToolStripMenuItem"
        resources.ApplyResources(Me.EditLabelToolStripMenuItem, "EditLabelToolStripMenuItem")
        '
        'SetSpotColorToolStripMenuItem
        '
        Me.SetSpotColorToolStripMenuItem.Name = "SetSpotColorToolStripMenuItem"
        resources.ApplyResources(Me.SetSpotColorToolStripMenuItem, "SetSpotColorToolStripMenuItem")
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        resources.ApplyResources(Me.ToolStripMenuItem1, "ToolStripMenuItem1")
        '
        'DeleteToolStripMenuItem
        '
        resources.ApplyResources(Me.DeleteToolStripMenuItem, "DeleteToolStripMenuItem")
        Me.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem"
        '
        'ExportSpatialMappingToolStripMenuItem
        '
        resources.ApplyResources(Me.ExportSpatialMappingToolStripMenuItem, "ExportSpatialMappingToolStripMenuItem")
        Me.ExportSpatialMappingToolStripMenuItem.Name = "ExportSpatialMappingToolStripMenuItem"
        '
        'ToolTip1
        '
        Me.ToolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info
        Me.ToolTip1.ToolTipTitle = "Spatial Mapping:"
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.ForeColor = System.Drawing.Color.DodgerBlue
        Me.Label1.Name = "Label1"
        '
        'AnchorResize
        '
        resources.ApplyResources(Me.AnchorResize, "AnchorResize")
        Me.AnchorResize.BackColor = System.Drawing.Color.Red
        Me.AnchorResize.Cursor = System.Windows.Forms.Cursors.SizeNWSE
        Me.AnchorResize.Name = "AnchorResize"
        Me.AnchorResize.TabStop = False
        '
        'RotateToolStripMenuItem
        '
        Me.RotateToolStripMenuItem.Name = "RotateToolStripMenuItem"
        resources.ApplyResources(Me.RotateToolStripMenuItem, "RotateToolStripMenuItem")
        '
        'SpatialTile
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ContextMenuStrip = Me.ContextMenuStrip1
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.AnchorResize)
        Me.Cursor = System.Windows.Forms.Cursors.Hand
        Me.DoubleBuffered = True
        Me.Name = "SpatialTile"
        Me.ContextMenuStrip1.ResumeLayout(False)
        CType(Me.AnchorResize, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents LoadTissueImageToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EditLabelToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents ExportSpatialMappingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents Label1 As Label
    Friend WithEvents AnchorResize As PictureBox
    Friend WithEvents RemoveTissueImageToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents DeleteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SetSpotColorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RotateToolStripMenuItem As ToolStripMenuItem
End Class
