#Region "Microsoft.VisualBasic::4c08a47882469ff600576b4664f156b3, mzkit\src\mzkit\mzkit\pages\toolkit\PageMzSearch.Designer.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 474
    '    Code Lines: 357
    ' Comment Lines: 113
    '   Blank Lines: 4
    '     File Size: 22.26 KB


    ' Class PageMzSearch
    ' 
    '     Sub: Dispose, InitializeComponent
    ' 
    ' /********************************************************************************/

#End Region

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PageMzSearch
    Inherits System.Windows.Forms.UserControl

    'UserControl 重写释放以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()>
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PageMzSearch))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.GaussianPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MS1PlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExportToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MSISearchToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DataGridView2 = New System.Windows.Forms.DataGridView()
        Me.Column1 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.AdductsPresets = New System.Windows.Forms.ComboBox()
        Me.AdductsPresetsLabel = New System.Windows.Forms.Label()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.cboxNegAdducts = New System.Windows.Forms.CheckedListBox()
        Me.TabControlMs1SearchAlgorithm = New System.Windows.Forms.TabControl()
        Me.TabSimpleMs1Search = New System.Windows.Forms.TabPage()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.cboxDatabaseList = New System.Windows.Forms.CheckedListBox()
        Me.TabMummichogSearch = New System.Windows.Forms.TabPage()
        Me.NumericUpDown3 = New System.Windows.Forms.NumericUpDown()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.cboxPosAdducts = New System.Windows.Forms.CheckedListBox()
        Me.CheckedListBox1 = New System.Windows.Forms.CheckedListBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.NumericPpmSearch = New System.Windows.Forms.NumericUpDown()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPage3.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.TabControlMs1SearchAlgorithm.SuspendLayout()
        Me.TabSimpleMs1Search.SuspendLayout()
        Me.TabMummichogSearch.SuspendLayout()
        CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox2.SuspendLayout()
        CType(Me.NumericPpmSearch, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'TextBox1
        '
        resources.ApplyResources(Me.TextBox1, "TextBox1")
        Me.TextBox1.Name = "TextBox1"
        '
        'Button1
        '
        resources.ApplyResources(Me.Button1, "Button1")
        Me.Button1.Name = "Button1"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'DataGridView1
        '
        resources.ApplyResources(Me.DataGridView1, "DataGridView1")
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Name = "DataGridView1"
        '
        'ComboBox1
        '
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Items.AddRange(New Object() {resources.GetString("ComboBox1.Items"), resources.GetString("ComboBox1.Items1"), resources.GetString("ComboBox1.Items2"), resources.GetString("ComboBox1.Items3"), resources.GetString("ComboBox1.Items4")})
        resources.ApplyResources(Me.ComboBox1, "ComboBox1")
        Me.ComboBox1.Name = "ComboBox1"
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        resources.ApplyResources(Me.TabControl1, "TabControl1")
        Me.TabControl1.Multiline = True
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.DataGridView1)
        Me.TabPage1.Controls.Add(Me.Label3)
        Me.TabPage1.Controls.Add(Me.Label1)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.TextBox1)
        Me.TabPage1.Controls.Add(Me.ComboBox1)
        Me.TabPage1.Controls.Add(Me.Button1)
        resources.ApplyResources(Me.TabPage1, "TabPage1")
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.PictureBox1)
        Me.TabPage2.Controls.Add(Me.DataGridView2)
        Me.TabPage2.Controls.Add(Me.Label4)
        Me.TabPage2.Controls.Add(Me.Label5)
        Me.TabPage2.Controls.Add(Me.TextBox2)
        Me.TabPage2.Controls.Add(Me.Button2)
        resources.ApplyResources(Me.TabPage2, "TabPage2")
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'PictureBox1
        '
        resources.ApplyResources(Me.PictureBox1, "PictureBox1")
        Me.PictureBox1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.TabStop = False
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GaussianPlotToolStripMenuItem, Me.MS1PlotToolStripMenuItem, Me.ToolStripMenuItem1, Me.ExportToolStripMenuItem, Me.MSISearchToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        '
        'GaussianPlotToolStripMenuItem
        '
        Me.GaussianPlotToolStripMenuItem.Name = "GaussianPlotToolStripMenuItem"
        resources.ApplyResources(Me.GaussianPlotToolStripMenuItem, "GaussianPlotToolStripMenuItem")
        '
        'MS1PlotToolStripMenuItem
        '
        Me.MS1PlotToolStripMenuItem.Checked = True
        Me.MS1PlotToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.MS1PlotToolStripMenuItem.Name = "MS1PlotToolStripMenuItem"
        resources.ApplyResources(Me.MS1PlotToolStripMenuItem, "MS1PlotToolStripMenuItem")
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        resources.ApplyResources(Me.ToolStripMenuItem1, "ToolStripMenuItem1")
        '
        'ExportToolStripMenuItem
        '
        resources.ApplyResources(Me.ExportToolStripMenuItem, "ExportToolStripMenuItem")
        Me.ExportToolStripMenuItem.Name = "ExportToolStripMenuItem"
        '
        'MSISearchToolStripMenuItem
        '
        resources.ApplyResources(Me.MSISearchToolStripMenuItem, "MSISearchToolStripMenuItem")
        Me.MSISearchToolStripMenuItem.Name = "MSISearchToolStripMenuItem"
        '
        'DataGridView2
        '
        resources.ApplyResources(Me.DataGridView2, "DataGridView2")
        Me.DataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView2.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column1, Me.Column2})
        Me.DataGridView2.Name = "DataGridView2"
        Me.DataGridView2.RowTemplate.Height = 23
        '
        'Column1
        '
        resources.ApplyResources(Me.Column1, "Column1")
        Me.Column1.Name = "Column1"
        '
        'Column2
        '
        resources.ApplyResources(Me.Column2, "Column2")
        Me.Column2.Name = "Column2"
        '
        'Label4
        '
        resources.ApplyResources(Me.Label4, "Label4")
        Me.Label4.Name = "Label4"
        '
        'Label5
        '
        resources.ApplyResources(Me.Label5, "Label5")
        Me.Label5.Name = "Label5"
        '
        'TextBox2
        '
        resources.ApplyResources(Me.TextBox2, "TextBox2")
        Me.TextBox2.Name = "TextBox2"
        '
        'Button2
        '
        resources.ApplyResources(Me.Button2, "Button2")
        Me.Button2.Name = "Button2"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.GroupBox1)
        Me.TabPage3.Controls.Add(Me.TextBox3)
        Me.TabPage3.Controls.Add(Me.Label6)
        resources.ApplyResources(Me.TabPage3, "TabPage3")
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        resources.ApplyResources(Me.GroupBox1, "GroupBox1")
        Me.GroupBox1.Controls.Add(Me.AdductsPresets)
        Me.GroupBox1.Controls.Add(Me.AdductsPresetsLabel)
        Me.GroupBox1.Controls.Add(Me.Button3)
        Me.GroupBox1.Controls.Add(Me.GroupBox3)
        Me.GroupBox1.Controls.Add(Me.TabControlMs1SearchAlgorithm)
        Me.GroupBox1.Controls.Add(Me.GroupBox2)
        Me.GroupBox1.Controls.Add(Me.CheckedListBox1)
        Me.GroupBox1.Controls.Add(Me.Label8)
        Me.GroupBox1.Controls.Add(Me.NumericPpmSearch)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.TabStop = False
        '
        'AdductsPresets
        '
        Me.AdductsPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.AdductsPresets.FormattingEnabled = True
        Me.AdductsPresets.Items.AddRange(New Object() {resources.GetString("AdductsPresets.Items"), resources.GetString("AdductsPresets.Items1"), resources.GetString("AdductsPresets.Items2")})
        resources.ApplyResources(Me.AdductsPresets, "AdductsPresets")
        Me.AdductsPresets.Name = "AdductsPresets"
        '
        'AdductsPresetsLabel
        '
        resources.ApplyResources(Me.AdductsPresetsLabel, "AdductsPresetsLabel")
        Me.AdductsPresetsLabel.Name = "AdductsPresetsLabel"
        '
        'Button3
        '
        resources.ApplyResources(Me.Button3, "Button3")
        Me.Button3.Name = "Button3"
        Me.Button3.UseVisualStyleBackColor = True
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.cboxNegAdducts)
        resources.ApplyResources(Me.GroupBox3, "GroupBox3")
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.TabStop = False
        '
        'cboxNegAdducts
        '
        resources.ApplyResources(Me.cboxNegAdducts, "cboxNegAdducts")
        Me.cboxNegAdducts.FormattingEnabled = True
        Me.cboxNegAdducts.Name = "cboxNegAdducts"
        '
        'TabControlMs1SearchAlgorithm
        '
        resources.ApplyResources(Me.TabControlMs1SearchAlgorithm, "TabControlMs1SearchAlgorithm")
        Me.TabControlMs1SearchAlgorithm.Controls.Add(Me.TabSimpleMs1Search)
        Me.TabControlMs1SearchAlgorithm.Controls.Add(Me.TabMummichogSearch)
        Me.TabControlMs1SearchAlgorithm.Name = "TabControlMs1SearchAlgorithm"
        Me.TabControlMs1SearchAlgorithm.SelectedIndex = 0
        '
        'TabSimpleMs1Search
        '
        Me.TabSimpleMs1Search.Controls.Add(Me.Label9)
        Me.TabSimpleMs1Search.Controls.Add(Me.cboxDatabaseList)
        resources.ApplyResources(Me.TabSimpleMs1Search, "TabSimpleMs1Search")
        Me.TabSimpleMs1Search.Name = "TabSimpleMs1Search"
        Me.TabSimpleMs1Search.UseVisualStyleBackColor = True
        '
        'Label9
        '
        resources.ApplyResources(Me.Label9, "Label9")
        Me.Label9.Name = "Label9"
        '
        'cboxDatabaseList
        '
        Me.cboxDatabaseList.FormattingEnabled = True
        resources.ApplyResources(Me.cboxDatabaseList, "cboxDatabaseList")
        Me.cboxDatabaseList.Name = "cboxDatabaseList"
        '
        'TabMummichogSearch
        '
        Me.TabMummichogSearch.Controls.Add(Me.NumericUpDown3)
        Me.TabMummichogSearch.Controls.Add(Me.Label12)
        resources.ApplyResources(Me.TabMummichogSearch, "TabMummichogSearch")
        Me.TabMummichogSearch.Name = "TabMummichogSearch"
        Me.TabMummichogSearch.UseVisualStyleBackColor = True
        '
        'NumericUpDown3
        '
        Me.NumericUpDown3.Increment = New Decimal(New Integer() {100, 0, 0, 0})
        resources.ApplyResources(Me.NumericUpDown3, "NumericUpDown3")
        Me.NumericUpDown3.Maximum = New Decimal(New Integer() {1000000, 0, 0, 0})
        Me.NumericUpDown3.Minimum = New Decimal(New Integer() {100, 0, 0, 0})
        Me.NumericUpDown3.Name = "NumericUpDown3"
        Me.NumericUpDown3.Value = New Decimal(New Integer() {1000, 0, 0, 0})
        '
        'Label12
        '
        resources.ApplyResources(Me.Label12, "Label12")
        Me.Label12.Name = "Label12"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.cboxPosAdducts)
        resources.ApplyResources(Me.GroupBox2, "GroupBox2")
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.TabStop = False
        '
        'cboxPosAdducts
        '
        resources.ApplyResources(Me.cboxPosAdducts, "cboxPosAdducts")
        Me.cboxPosAdducts.FormattingEnabled = True
        Me.cboxPosAdducts.Name = "cboxPosAdducts"
        '
        'CheckedListBox1
        '
        Me.CheckedListBox1.FormattingEnabled = True
        Me.CheckedListBox1.Items.AddRange(New Object() {resources.GetString("CheckedListBox1.Items"), resources.GetString("CheckedListBox1.Items1")})
        resources.ApplyResources(Me.CheckedListBox1, "CheckedListBox1")
        Me.CheckedListBox1.Name = "CheckedListBox1"
        '
        'Label8
        '
        resources.ApplyResources(Me.Label8, "Label8")
        Me.Label8.Name = "Label8"
        '
        'NumericPpmSearch
        '
        Me.NumericPpmSearch.Increment = New Decimal(New Integer() {5, 0, 0, 0})
        resources.ApplyResources(Me.NumericPpmSearch, "NumericPpmSearch")
        Me.NumericPpmSearch.Maximum = New Decimal(New Integer() {500, 0, 0, 0})
        Me.NumericPpmSearch.Name = "NumericPpmSearch"
        Me.NumericPpmSearch.Value = New Decimal(New Integer() {20, 0, 0, 0})
        '
        'Label7
        '
        resources.ApplyResources(Me.Label7, "Label7")
        Me.Label7.Name = "Label7"
        '
        'TextBox3
        '
        resources.ApplyResources(Me.TextBox3, "TextBox3")
        Me.TextBox3.BackColor = System.Drawing.Color.White
        Me.TextBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.TextBox3.Name = "TextBox3"
        '
        'Label6
        '
        resources.ApplyResources(Me.Label6, "Label6")
        Me.Label6.Name = "Label6"
        '
        'PageMzSearch
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.Controls.Add(Me.TabControl1)
        Me.DoubleBuffered = True
        Me.Name = "PageMzSearch"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        CType(Me.DataGridView2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.TabControlMs1SearchAlgorithm.ResumeLayout(False)
        Me.TabSimpleMs1Search.ResumeLayout(False)
        Me.TabSimpleMs1Search.PerformLayout()
        Me.TabMummichogSearch.ResumeLayout(False)
        Me.TabMummichogSearch.PerformLayout()
        CType(Me.NumericUpDown3, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox2.ResumeLayout(False)
        CType(Me.NumericPpmSearch, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label1 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents Button1 As Button
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents Label4 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents Button2 As Button
    Friend WithEvents DataGridView2 As DataGridView
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents GaussianPlotToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MS1PlotToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Column1 As DataGridViewTextBoxColumn
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents ExportToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MSISearchToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TabPage3 As TabPage
    Friend WithEvents Button3 As Button
    Friend WithEvents TextBox3 As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents CheckedListBox1 As CheckedListBox
    Friend WithEvents Label7 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents NumericPpmSearch As NumericUpDown
    Friend WithEvents cboxDatabaseList As CheckedListBox
    Friend WithEvents Label9 As Label
    Friend WithEvents TabControlMs1SearchAlgorithm As TabControl
    Friend WithEvents TabSimpleMs1Search As TabPage
    Friend WithEvents TabMummichogSearch As TabPage
    Friend WithEvents NumericUpDown3 As NumericUpDown
    Friend WithEvents Label12 As Label
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents cboxPosAdducts As CheckedListBox
    Friend WithEvents cboxNegAdducts As CheckedListBox
    Friend WithEvents AdductsPresetsLabel As Label
    Friend WithEvents AdductsPresets As ComboBox
End Class
