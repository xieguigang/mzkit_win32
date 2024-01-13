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
        Me.AdvancedDataGridView1 = New Zuby.ADGV.AdvancedDataGridView()
        Me.AdvancedDataGridViewSearchToolBar1 = New Zuby.ADGV.AdvancedDataGridViewSearchToolBar()
        Me.BindingSource1 = New System.Windows.Forms.BindingSource(Me.components)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.WebView21, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        CType(Me.AdvancedDataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.SplitContainer1.Panel2.Controls.Add(Me.AdvancedDataGridView1)
        Me.SplitContainer1.Panel2.Controls.Add(Me.AdvancedDataGridViewSearchToolBar1)
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
        'AdvancedDataGridView1
        '
        Me.AdvancedDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.AdvancedDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AdvancedDataGridView1.FilterAndSortEnabled = True
        Me.AdvancedDataGridView1.FilterStringChangedInvokeBeforeDatasourceUpdate = True
        Me.AdvancedDataGridView1.Location = New System.Drawing.Point(0, 27)
        Me.AdvancedDataGridView1.Name = "AdvancedDataGridView1"
        Me.AdvancedDataGridView1.ReadOnly = True
        Me.AdvancedDataGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.AdvancedDataGridView1.RowTemplate.Height = 23
        Me.AdvancedDataGridView1.Size = New System.Drawing.Size(684, 610)
        Me.AdvancedDataGridView1.SortStringChangedInvokeBeforeDatasourceUpdate = True
        Me.AdvancedDataGridView1.TabIndex = 3
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
        CType(Me.AdvancedDataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
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
End Class
