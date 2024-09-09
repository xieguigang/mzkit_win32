Imports Mzkit_win32.BasicMDIForm

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMetabonomicsAnalysis
    Inherits DocumentWindow

    'Form 重写 Dispose，以清理组件列表。
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMetabonomicsAnalysis))
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.WebView21 = New Microsoft.Web.WebView2.WinForms.WebView2()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.PropertyGrid1 = New System.Windows.Forms.PropertyGrid()
        Me.SplitContainer2 = New System.Windows.Forms.SplitContainer()
        Me.AdvancedDataGridView1 = New Zuby.ADGV.AdvancedDataGridView()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ViewExpressionPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AutoPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.AdvancedDataGridViewSearchToolBar1 = New Zuby.ADGV.AdvancedDataGridViewSearchToolBar()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripDropDownButton2 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.OPLSDAToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PLSDAToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PCAToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.ViolinPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BarPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BoxPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BindingSource1 = New System.Windows.Forms.BindingSource(Me.components)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.WebView21, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer2.Panel1.SuspendLayout()
        Me.SplitContainer2.Panel2.SuspendLayout()
        Me.SplitContainer2.SuspendLayout()
        CType(Me.AdvancedDataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip1.SuspendLayout()
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.WebView21)
        Me.SplitContainer1.Panel1.Controls.Add(Me.GroupBox1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainer2)
        Me.SplitContainer1.Panel2.Controls.Add(Me.AdvancedDataGridViewSearchToolBar1)
        Me.SplitContainer1.Panel2.Controls.Add(Me.StatusStrip1)
        Me.SplitContainer1.Size = New System.Drawing.Size(1031, 637)
        Me.SplitContainer1.SplitterDistance = 343
        Me.SplitContainer1.TabIndex = 1
        '
        'WebView21
        '
        Me.WebView21.AllowExternalDrop = True
        Me.WebView21.CreationProperties = Nothing
        Me.WebView21.DefaultBackgroundColor = System.Drawing.Color.White
        Me.WebView21.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WebView21.Location = New System.Drawing.Point(0, 385)
        Me.WebView21.Name = "WebView21"
        Me.WebView21.Size = New System.Drawing.Size(343, 252)
        Me.WebView21.TabIndex = 1
        Me.WebView21.ZoomFactor = 1.0R
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.PropertyGrid1)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Top
        Me.GroupBox1.Location = New System.Drawing.Point(0, 0)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(343, 385)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Metabolite Metadata"
        '
        'PropertyGrid1
        '
        Me.PropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGrid1.Location = New System.Drawing.Point(3, 17)
        Me.PropertyGrid1.Name = "PropertyGrid1"
        Me.PropertyGrid1.Size = New System.Drawing.Size(337, 365)
        Me.PropertyGrid1.TabIndex = 0
        '
        'SplitContainer2
        '
        Me.SplitContainer2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer2.Location = New System.Drawing.Point(0, 27)
        Me.SplitContainer2.Name = "SplitContainer2"
        Me.SplitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer2.Panel1
        '
        Me.SplitContainer2.Panel1.Controls.Add(Me.AdvancedDataGridView1)
        '
        'SplitContainer2.Panel2
        '
        Me.SplitContainer2.Panel2.Controls.Add(Me.PictureBox1)
        Me.SplitContainer2.Size = New System.Drawing.Size(684, 588)
        Me.SplitContainer2.SplitterDistance = 294
        Me.SplitContainer2.TabIndex = 6
        '
        'AdvancedDataGridView1
        '
        Me.AdvancedDataGridView1.AllowUserToAddRows = False
        Me.AdvancedDataGridView1.AllowUserToDeleteRows = False
        Me.AdvancedDataGridView1.BackgroundColor = System.Drawing.Color.WhiteSmoke
        Me.AdvancedDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.AdvancedDataGridView1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.AdvancedDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AdvancedDataGridView1.FilterAndSortEnabled = True
        Me.AdvancedDataGridView1.FilterStringChangedInvokeBeforeDatasourceUpdate = True
        Me.AdvancedDataGridView1.Location = New System.Drawing.Point(0, 0)
        Me.AdvancedDataGridView1.MultiSelect = False
        Me.AdvancedDataGridView1.Name = "AdvancedDataGridView1"
        Me.AdvancedDataGridView1.ReadOnly = True
        Me.AdvancedDataGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.AdvancedDataGridView1.RowTemplate.Height = 23
        Me.AdvancedDataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.AdvancedDataGridView1.Size = New System.Drawing.Size(684, 294)
        Me.AdvancedDataGridView1.SortStringChangedInvokeBeforeDatasourceUpdate = True
        Me.AdvancedDataGridView1.TabIndex = 3
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewExpressionPlotToolStripMenuItem, Me.AutoPlotToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(183, 48)
        '
        'ViewExpressionPlotToolStripMenuItem
        '
        Me.ViewExpressionPlotToolStripMenuItem.Name = "ViewExpressionPlotToolStripMenuItem"
        Me.ViewExpressionPlotToolStripMenuItem.Size = New System.Drawing.Size(182, 22)
        Me.ViewExpressionPlotToolStripMenuItem.Text = "View Expression Plot"
        '
        'AutoPlotToolStripMenuItem
        '
        Me.AutoPlotToolStripMenuItem.CheckOnClick = True
        Me.AutoPlotToolStripMenuItem.Name = "AutoPlotToolStripMenuItem"
        Me.AutoPlotToolStripMenuItem.Size = New System.Drawing.Size(182, 22)
        Me.AutoPlotToolStripMenuItem.Text = "Auto Plot"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(684, 290)
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
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
        Me.AdvancedDataGridViewSearchToolBar1.Size = New System.Drawing.Size(684, 27)
        Me.AdvancedDataGridViewSearchToolBar1.TabIndex = 4
        Me.AdvancedDataGridViewSearchToolBar1.Text = "AdvancedDataGridViewSearchToolBar1"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripDropDownButton2, Me.ToolStripDropDownButton1, Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 615)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(684, 22)
        Me.StatusStrip1.TabIndex = 5
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripDropDownButton2
        '
        Me.ToolStripDropDownButton2.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OPLSDAToolStripMenuItem, Me.PLSDAToolStripMenuItem, Me.PCAToolStripMenuItem})
        Me.ToolStripDropDownButton2.Image = CType(resources.GetObject("ToolStripDropDownButton2.Image"), System.Drawing.Image)
        Me.ToolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButton2.Name = "ToolStripDropDownButton2"
        Me.ToolStripDropDownButton2.Size = New System.Drawing.Size(113, 20)
        Me.ToolStripDropDownButton2.Text = "Select Analysis"
        '
        'OPLSDAToolStripMenuItem
        '
        Me.OPLSDAToolStripMenuItem.CheckOnClick = True
        Me.OPLSDAToolStripMenuItem.Name = "OPLSDAToolStripMenuItem"
        Me.OPLSDAToolStripMenuItem.Size = New System.Drawing.Size(118, 22)
        Me.OPLSDAToolStripMenuItem.Text = "OPLSDA"
        '
        'PLSDAToolStripMenuItem
        '
        Me.PLSDAToolStripMenuItem.CheckOnClick = True
        Me.PLSDAToolStripMenuItem.Name = "PLSDAToolStripMenuItem"
        Me.PLSDAToolStripMenuItem.Size = New System.Drawing.Size(118, 22)
        Me.PLSDAToolStripMenuItem.Text = "PLSDA"
        '
        'PCAToolStripMenuItem
        '
        Me.PCAToolStripMenuItem.CheckOnClick = True
        Me.PCAToolStripMenuItem.Name = "PCAToolStripMenuItem"
        Me.PCAToolStripMenuItem.Size = New System.Drawing.Size(118, 22)
        Me.PCAToolStripMenuItem.Text = "PCA"
        '
        'ToolStripDropDownButton1
        '
        Me.ToolStripDropDownButton1.Image = CType(resources.GetObject("ToolStripDropDownButton1.Image"), System.Drawing.Image)
        Me.ToolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
        Me.ToolStripDropDownButton1.Size = New System.Drawing.Size(126, 20)
        Me.ToolStripDropDownButton1.Text = "View Result Table"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViolinPlotToolStripMenuItem, Me.BarPlotToolStripMenuItem, Me.BoxPlotToolStripMenuItem})
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(134, 20)
        Me.ToolStripStatusLabel1.Text = "Select Expression Plot"
        '
        'ViolinPlotToolStripMenuItem
        '
        Me.ViolinPlotToolStripMenuItem.CheckOnClick = True
        Me.ViolinPlotToolStripMenuItem.Name = "ViolinPlotToolStripMenuItem"
        Me.ViolinPlotToolStripMenuItem.Size = New System.Drawing.Size(128, 22)
        Me.ViolinPlotToolStripMenuItem.Text = "Violin Plot"
        '
        'BarPlotToolStripMenuItem
        '
        Me.BarPlotToolStripMenuItem.Checked = True
        Me.BarPlotToolStripMenuItem.CheckOnClick = True
        Me.BarPlotToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.BarPlotToolStripMenuItem.Name = "BarPlotToolStripMenuItem"
        Me.BarPlotToolStripMenuItem.Size = New System.Drawing.Size(128, 22)
        Me.BarPlotToolStripMenuItem.Text = "Bar Plot"
        '
        'BoxPlotToolStripMenuItem
        '
        Me.BoxPlotToolStripMenuItem.CheckOnClick = True
        Me.BoxPlotToolStripMenuItem.Name = "BoxPlotToolStripMenuItem"
        Me.BoxPlotToolStripMenuItem.Size = New System.Drawing.Size(128, 22)
        Me.BoxPlotToolStripMenuItem.Text = "Box Plot"
        '
        'frmMetabonomicsAnalysis
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1031, 637)
        Me.Controls.Add(Me.SplitContainer1)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmMetabonomicsAnalysis"
        Me.TabPageContextMenuStrip = Me.DockContextMenuStrip1
        Me.Text = "Metabonomics Workbench"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.Panel2.PerformLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.WebView21, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.SplitContainer2.Panel1.ResumeLayout(False)
        Me.SplitContainer2.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer2.ResumeLayout(False)
        CType(Me.AdvancedDataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents WebView21 As Microsoft.Web.WebView2.WinForms.WebView2
    Friend WithEvents AdvancedDataGridView1 As Zuby.ADGV.AdvancedDataGridView
    Friend WithEvents AdvancedDataGridViewSearchToolBar1 As Zuby.ADGV.AdvancedDataGridViewSearchToolBar
    Friend WithEvents BindingSource1 As BindingSource
    Friend WithEvents PropertyGrid1 As PropertyGrid
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripDropDownButton1 As ToolStripDropDownButton
    Friend WithEvents ToolStripDropDownButton2 As ToolStripDropDownButton
    Friend WithEvents OPLSDAToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PLSDAToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PCAToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SplitContainer2 As SplitContainer
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents ToolStripStatusLabel1 As ToolStripDropDownButton
    Friend WithEvents ViolinPlotToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BarPlotToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BoxPlotToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ViewExpressionPlotToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AutoPlotToolStripMenuItem As ToolStripMenuItem
End Class