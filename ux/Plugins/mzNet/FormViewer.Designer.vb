Imports Mzkit_win32.BasicMDIForm

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormViewer : Inherits DocumentWindow

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
        Me.components = New System.ComponentModel.Container()
        Me.AdvancedDataGridView1 = New Zuby.ADGV.AdvancedDataGridView()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ViewSpectralToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportSpectrumToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.MassDifferenceAnalysisToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.AutoPlotSpectrumToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AdvancedDataGridViewSearchToolBar1 = New Zuby.ADGV.AdvancedDataGridViewSearchToolBar()
        Me.BindingSource1 = New System.Windows.Forms.BindingSource(Me.components)
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ViewClusterInBrowserToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewBioDeepMetabolitesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        CType(Me.AdvancedDataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'AdvancedDataGridView1
        '
        Me.AdvancedDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.AdvancedDataGridView1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.AdvancedDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AdvancedDataGridView1.FilterAndSortEnabled = True
        Me.AdvancedDataGridView1.FilterStringChangedInvokeBeforeDatasourceUpdate = True
        Me.AdvancedDataGridView1.Location = New System.Drawing.Point(0, 27)
        Me.AdvancedDataGridView1.Name = "AdvancedDataGridView1"
        Me.AdvancedDataGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.AdvancedDataGridView1.RowTemplate.Height = 23
        Me.AdvancedDataGridView1.Size = New System.Drawing.Size(587, 351)
        Me.AdvancedDataGridView1.SortStringChangedInvokeBeforeDatasourceUpdate = True
        Me.AdvancedDataGridView1.TabIndex = 2
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewSpectralToolStripMenuItem, Me.ExportSpectrumToolStripMenuItem, Me.ToolStripMenuItem2, Me.MassDifferenceAnalysisToolStripMenuItem, Me.ToolStripMenuItem1, Me.AutoPlotSpectrumToolStripMenuItem, Me.ToolStripMenuItem3, Me.ViewClusterInBrowserToolStripMenuItem, Me.ViewBioDeepMetabolitesToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(212, 176)
        '
        'ViewSpectralToolStripMenuItem
        '
        Me.ViewSpectralToolStripMenuItem.Name = "ViewSpectralToolStripMenuItem"
        Me.ViewSpectralToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.ViewSpectralToolStripMenuItem.Text = "View Spectral"
        '
        'ExportSpectrumToolStripMenuItem
        '
        Me.ExportSpectrumToolStripMenuItem.Name = "ExportSpectrumToolStripMenuItem"
        Me.ExportSpectrumToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.ExportSpectrumToolStripMenuItem.Text = "Export Spectrum"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(208, 6)
        '
        'MassDifferenceAnalysisToolStripMenuItem
        '
        Me.MassDifferenceAnalysisToolStripMenuItem.Name = "MassDifferenceAnalysisToolStripMenuItem"
        Me.MassDifferenceAnalysisToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.MassDifferenceAnalysisToolStripMenuItem.Text = "Mass Difference Analysis"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(208, 6)
        '
        'AutoPlotSpectrumToolStripMenuItem
        '
        Me.AutoPlotSpectrumToolStripMenuItem.CheckOnClick = True
        Me.AutoPlotSpectrumToolStripMenuItem.Name = "AutoPlotSpectrumToolStripMenuItem"
        Me.AutoPlotSpectrumToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.AutoPlotSpectrumToolStripMenuItem.Text = "Auto Plot Spectrum"
        '
        'AdvancedDataGridViewSearchToolBar1
        '
        Me.AdvancedDataGridViewSearchToolBar1.AllowMerge = False
        Me.AdvancedDataGridViewSearchToolBar1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.AdvancedDataGridViewSearchToolBar1.Location = New System.Drawing.Point(0, 0)
        Me.AdvancedDataGridViewSearchToolBar1.MaximumSize = New System.Drawing.Size(0, 27)
        Me.AdvancedDataGridViewSearchToolBar1.MinimumSize = New System.Drawing.Size(0, 27)
        Me.AdvancedDataGridViewSearchToolBar1.Name = "AdvancedDataGridViewSearchToolBar1"
        Me.AdvancedDataGridViewSearchToolBar1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        Me.AdvancedDataGridViewSearchToolBar1.Size = New System.Drawing.Size(587, 27)
        Me.AdvancedDataGridViewSearchToolBar1.TabIndex = 0
        Me.AdvancedDataGridViewSearchToolBar1.Text = "AdvancedDataGridViewSearchToolBar1"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(208, 6)
        '
        'ViewClusterInBrowserToolStripMenuItem
        '
        Me.ViewClusterInBrowserToolStripMenuItem.Name = "ViewClusterInBrowserToolStripMenuItem"
        Me.ViewClusterInBrowserToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.ViewClusterInBrowserToolStripMenuItem.Text = "View Cluster In Browser"
        '
        'ViewBioDeepMetabolitesToolStripMenuItem
        '
        Me.ViewBioDeepMetabolitesToolStripMenuItem.Name = "ViewBioDeepMetabolitesToolStripMenuItem"
        Me.ViewBioDeepMetabolitesToolStripMenuItem.Size = New System.Drawing.Size(211, 22)
        Me.ViewBioDeepMetabolitesToolStripMenuItem.Text = "View BioDeep Metabolites"
        '
        'FormViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(587, 378)
        Me.Controls.Add(Me.AdvancedDataGridView1)
        Me.Controls.Add(Me.AdvancedDataGridViewSearchToolBar1)
        Me.DoubleBuffered = True
        Me.Name = "FormViewer"
        Me.TabPageContextMenuStrip = Me.DockContextMenuStrip1
        CType(Me.AdvancedDataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents AdvancedDataGridViewSearchToolBar1 As Zuby.ADGV.AdvancedDataGridViewSearchToolBar
    Friend WithEvents BindingSource1 As BindingSource
    Friend WithEvents AdvancedDataGridView1 As Zuby.ADGV.AdvancedDataGridView
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ViewSpectralToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents AutoPlotSpectrumToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExportSpectrumToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents MassDifferenceAnalysisToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As ToolStripSeparator
    Friend WithEvents ViewClusterInBrowserToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ViewBioDeepMetabolitesToolStripMenuItem As ToolStripMenuItem
End Class
