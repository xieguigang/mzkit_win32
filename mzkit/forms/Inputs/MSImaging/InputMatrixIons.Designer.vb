Imports Mzkit_win32.BasicMDIForm.CommonDialogs

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class InputMatrixIons
    Inherits InputDialog

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
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.AdvancedDataGridView1 = New Zuby.ADGV.AdvancedDataGridView()
        Me.AdvancedDataGridViewSearchToolBar1 = New Zuby.ADGV.AdvancedDataGridViewSearchToolBar()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.txtMzdiff = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtRows = New System.Windows.Forms.TextBox()
        Me.txtColumns = New System.Windows.Forms.TextBox()
        Me.BindingSource1 = New System.Windows.Forms.BindingSource(Me.components)
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.FileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoadMetabolitesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportSingleIonsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        CType(Me.AdvancedDataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage2.SuspendLayout()
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.StatusStrip1.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(12, 34)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(742, 341)
        Me.TabControl1.TabIndex = 1
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.AdvancedDataGridView1)
        Me.TabPage1.Controls.Add(Me.AdvancedDataGridViewSearchToolBar1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(734, 315)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Select Ions"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'AdvancedDataGridView1
        '
        Me.AdvancedDataGridView1.AllowUserToAddRows = False
        Me.AdvancedDataGridView1.AllowUserToDeleteRows = False
        Me.AdvancedDataGridView1.BackgroundColor = System.Drawing.Color.Gainsboro
        Me.AdvancedDataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.AdvancedDataGridView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.AdvancedDataGridView1.FilterAndSortEnabled = True
        Me.AdvancedDataGridView1.FilterStringChangedInvokeBeforeDatasourceUpdate = True
        Me.AdvancedDataGridView1.Location = New System.Drawing.Point(3, 30)
        Me.AdvancedDataGridView1.Name = "AdvancedDataGridView1"
        Me.AdvancedDataGridView1.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.AdvancedDataGridView1.Size = New System.Drawing.Size(728, 282)
        Me.AdvancedDataGridView1.SortStringChangedInvokeBeforeDatasourceUpdate = True
        Me.AdvancedDataGridView1.TabIndex = 0
        '
        'AdvancedDataGridViewSearchToolBar1
        '
        Me.AdvancedDataGridViewSearchToolBar1.AllowMerge = False
        Me.AdvancedDataGridViewSearchToolBar1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.AdvancedDataGridViewSearchToolBar1.Location = New System.Drawing.Point(3, 3)
        Me.AdvancedDataGridViewSearchToolBar1.MaximumSize = New System.Drawing.Size(0, 27)
        Me.AdvancedDataGridViewSearchToolBar1.MinimumSize = New System.Drawing.Size(0, 27)
        Me.AdvancedDataGridViewSearchToolBar1.Name = "AdvancedDataGridViewSearchToolBar1"
        Me.AdvancedDataGridViewSearchToolBar1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        Me.AdvancedDataGridViewSearchToolBar1.Size = New System.Drawing.Size(728, 27)
        Me.AdvancedDataGridViewSearchToolBar1.TabIndex = 1
        Me.AdvancedDataGridViewSearchToolBar1.Text = "AdvancedDataGridViewSearchToolBar1"
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.ComboBox1)
        Me.TabPage2.Controls.Add(Me.Label7)
        Me.TabPage2.Controls.Add(Me.Label6)
        Me.TabPage2.Controls.Add(Me.TextBox2)
        Me.TabPage2.Controls.Add(Me.TextBox3)
        Me.TabPage2.Controls.Add(Me.Label5)
        Me.TabPage2.Controls.Add(Me.txtMzdiff)
        Me.TabPage2.Controls.Add(Me.Label4)
        Me.TabPage2.Controls.Add(Me.TextBox1)
        Me.TabPage2.Controls.Add(Me.Label3)
        Me.TabPage2.Controls.Add(Me.Label2)
        Me.TabPage2.Controls.Add(Me.Label1)
        Me.TabPage2.Controls.Add(Me.txtRows)
        Me.TabPage2.Controls.Add(Me.txtColumns)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(734, 315)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Settings"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'ComboBox1
        '
        Me.ComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(322, 45)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(194, 20)
        Me.ComboBox1.TabIndex = 13
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(319, 21)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(113, 12)
        Me.Label7.TabIndex = 12
        Me.Label7.Text = "Set Color Palatte:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(191, 49)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(11, 12)
        Me.Label6.TabIndex = 11
        Me.Label6.Text = "x"
        '
        'TextBox2
        '
        Me.TextBox2.Location = New System.Drawing.Point(131, 46)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(53, 21)
        Me.TextBox2.TabIndex = 10
        Me.TextBox2.Text = "2800"
        '
        'TextBox3
        '
        Me.TextBox3.Location = New System.Drawing.Point(209, 46)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(53, 21)
        Me.TextBox3.TabIndex = 9
        Me.TextBox3.Text = "2100"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(21, 49)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(77, 12)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "Canvas Size:"
        '
        'txtMzdiff
        '
        Me.txtMzdiff.Location = New System.Drawing.Point(131, 73)
        Me.txtMzdiff.Name = "txtMzdiff"
        Me.txtMzdiff.Size = New System.Drawing.Size(53, 21)
        Me.txtMzdiff.TabIndex = 7
        Me.txtMzdiff.Text = "0.3"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(21, 76)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(65, 12)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Mass Diff:"
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(23, 134)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ReadOnly = True
        Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox1.Size = New System.Drawing.Size(694, 164)
        Me.TextBox1.TabIndex = 5
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(21, 108)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(77, 12)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "Command Log:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(21, 22)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(89, 12)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Matrix Layout:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(173, 21)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(11, 12)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "x"
        '
        'txtRows
        '
        Me.txtRows.Location = New System.Drawing.Point(131, 19)
        Me.txtRows.Name = "txtRows"
        Me.txtRows.Size = New System.Drawing.Size(36, 21)
        Me.txtRows.TabIndex = 1
        Me.txtRows.Text = "3"
        '
        'txtColumns
        '
        Me.txtColumns.Location = New System.Drawing.Point(190, 19)
        Me.txtColumns.Name = "txtColumns"
        Me.txtColumns.Size = New System.Drawing.Size(38, 21)
        Me.txtColumns.TabIndex = 0
        Me.txtColumns.Text = "3"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 420)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(766, 22)
        Me.StatusStrip1.TabIndex = 2
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(119, 17)
        Me.ToolStripStatusLabel1.Text = "ToolStripStatusLabel1"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(643, 385)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 3
        Me.Button1.Text = "Create"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System
        Me.MenuStrip1.Size = New System.Drawing.Size(766, 24)
        Me.MenuStrip1.TabIndex = 5
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem
        '
        Me.FileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LoadMetabolitesToolStripMenuItem, Me.ExportSingleIonsToolStripMenuItem})
        Me.FileToolStripMenuItem.Name = "FileToolStripMenuItem"
        Me.FileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.FileToolStripMenuItem.Text = "File"
        '
        'LoadMetabolitesToolStripMenuItem
        '
        Me.LoadMetabolitesToolStripMenuItem.Name = "LoadMetabolitesToolStripMenuItem"
        Me.LoadMetabolitesToolStripMenuItem.Size = New System.Drawing.Size(168, 22)
        Me.LoadMetabolitesToolStripMenuItem.Text = "Load Metabolites"
        '
        'ExportSingleIonsToolStripMenuItem
        '
        Me.ExportSingleIonsToolStripMenuItem.Name = "ExportSingleIonsToolStripMenuItem"
        Me.ExportSingleIonsToolStripMenuItem.Size = New System.Drawing.Size(168, 22)
        Me.ExportSingleIonsToolStripMenuItem.Text = "Export Single Ions"
        '
        'InputMatrixIons
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(766, 442)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.TabControl1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "InputMatrixIons"
        Me.Text = "MS-Imaging HeatMap Matrix"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        CType(Me.AdvancedDataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents BindingSource1 As BindingSource
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents Button1 As Button
    Friend WithEvents txtRows As TextBox
    Friend WithEvents txtColumns As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents FileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LoadMetabolitesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AdvancedDataGridView1 As Zuby.ADGV.AdvancedDataGridView
    Friend WithEvents AdvancedDataGridViewSearchToolBar1 As Zuby.ADGV.AdvancedDataGridViewSearchToolBar
    Friend WithEvents Label2 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtMzdiff As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents ExportSingleIonsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Label6 As Label
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents TextBox3 As TextBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents ComboBox1 As ComboBox
End Class
