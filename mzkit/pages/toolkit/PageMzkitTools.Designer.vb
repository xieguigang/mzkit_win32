#Region "Microsoft.VisualBasic::cbc35b22879659829ce28214ce962dcb, mzkit\src\mzkit\mzkit\pages\toolkit\PageMzkitTools.Designer.vb"

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

    '   Total Lines: 144
    '    Code Lines: 110
    ' Comment Lines: 29
    '   Blank Lines: 5
    '     File Size: 7.27 KB


    ' Class PageMzkitTools
    ' 
    '     Sub: Dispose, InitializeComponent
    ' 
    ' /********************************************************************************/

#End Region

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class PageMzkitTools
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(PageMzkitTools))
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.CopyValueToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenInTableViewerToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.SaveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.CustomTabControl1 = New System.Windows.Forms.CustomTabControl()
        Me.TabPage5 = New System.Windows.Forms.TabPage()
        Me.TabPage6 = New System.Windows.Forms.TabPage()
        Me.BindingSource1 = New System.Windows.Forms.BindingSource(Me.components)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.CustomTabControl1.SuspendLayout()
        Me.TabPage5.SuspendLayout()
        Me.TabPage6.SuspendLayout()
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "view_16xLG.png")
        Me.ImageList1.Images.SetKeyName(1, "StatusAnnotations_Warning_32xLG_color.png")
        '
        'PictureBox1
        '
        resources.ApplyResources(Me.PictureBox1, "PictureBox1")
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.TabStop = False
        Me.ToolTip1.SetToolTip(Me.PictureBox1, resources.GetString("PictureBox1.ToolTip"))
        '
        'DataGridView1
        '
        resources.ApplyResources(Me.DataGridView1, "DataGridView1")
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.ToolTip1.SetToolTip(Me.DataGridView1, resources.GetString("DataGridView1.ToolTip"))
        '
        'ContextMenuStrip1
        '
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyValueToolStripMenuItem, Me.OpenInTableViewerToolStripMenuItem, Me.ToolStripMenuItem1, Me.SaveToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ToolTip1.SetToolTip(Me.ContextMenuStrip1, resources.GetString("ContextMenuStrip1.ToolTip"))
        '
        'CopyValueToolStripMenuItem
        '
        resources.ApplyResources(Me.CopyValueToolStripMenuItem, "CopyValueToolStripMenuItem")
        Me.CopyValueToolStripMenuItem.Name = "CopyValueToolStripMenuItem"
        '
        'OpenInTableViewerToolStripMenuItem
        '
        resources.ApplyResources(Me.OpenInTableViewerToolStripMenuItem, "OpenInTableViewerToolStripMenuItem")
        Me.OpenInTableViewerToolStripMenuItem.Name = "OpenInTableViewerToolStripMenuItem"
        '
        'ToolStripMenuItem1
        '
        resources.ApplyResources(Me.ToolStripMenuItem1, "ToolStripMenuItem1")
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        '
        'SaveToolStripMenuItem
        '
        resources.ApplyResources(Me.SaveToolStripMenuItem, "SaveToolStripMenuItem")
        Me.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem"
        '
        'CustomTabControl1
        '
        resources.ApplyResources(Me.CustomTabControl1, "CustomTabControl1")
        Me.CustomTabControl1.Controls.Add(Me.TabPage5)
        Me.CustomTabControl1.Controls.Add(Me.TabPage6)
        '
        '
        '
        Me.CustomTabControl1.DisplayStyleProvider.BorderColor = System.Drawing.SystemColors.ControlDark
        Me.CustomTabControl1.DisplayStyleProvider.BorderColorHot = System.Drawing.SystemColors.ControlDark
        Me.CustomTabControl1.DisplayStyleProvider.BorderColorSelected = System.Drawing.Color.FromArgb(CType(CType(127, Byte), Integer), CType(CType(157, Byte), Integer), CType(CType(185, Byte), Integer))
        Me.CustomTabControl1.DisplayStyleProvider.CloserColor = System.Drawing.Color.DarkGray
        Me.CustomTabControl1.DisplayStyleProvider.FocusTrack = True
        Me.CustomTabControl1.DisplayStyleProvider.HotTrack = True
        Me.CustomTabControl1.DisplayStyleProvider.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.CustomTabControl1.DisplayStyleProvider.Opacity = 1.0!
        Me.CustomTabControl1.DisplayStyleProvider.Overlap = 0
        Me.CustomTabControl1.DisplayStyleProvider.Padding = New System.Drawing.Point(6, 3)
        Me.CustomTabControl1.DisplayStyleProvider.Radius = 2
        Me.CustomTabControl1.DisplayStyleProvider.ShowTabCloser = True
        Me.CustomTabControl1.DisplayStyleProvider.TextColor = System.Drawing.SystemColors.ControlText
        Me.CustomTabControl1.DisplayStyleProvider.TextColorDisabled = System.Drawing.SystemColors.ControlDark
        Me.CustomTabControl1.DisplayStyleProvider.TextColorSelected = System.Drawing.SystemColors.ControlText
        Me.CustomTabControl1.HotTrack = True
        Me.CustomTabControl1.Name = "CustomTabControl1"
        Me.CustomTabControl1.SelectedIndex = 0
        Me.ToolTip1.SetToolTip(Me.CustomTabControl1, resources.GetString("CustomTabControl1.ToolTip"))
        '
        'TabPage5
        '
        resources.ApplyResources(Me.TabPage5, "TabPage5")
        Me.TabPage5.Controls.Add(Me.PictureBox1)
        Me.TabPage5.Name = "TabPage5"
        Me.ToolTip1.SetToolTip(Me.TabPage5, resources.GetString("TabPage5.ToolTip"))
        Me.TabPage5.UseVisualStyleBackColor = True
        '
        'TabPage6
        '
        resources.ApplyResources(Me.TabPage6, "TabPage6")
        Me.TabPage6.Controls.Add(Me.DataGridView1)
        Me.TabPage6.Name = "TabPage6"
        Me.ToolTip1.SetToolTip(Me.TabPage6, resources.GetString("TabPage6.ToolTip"))
        Me.TabPage6.UseVisualStyleBackColor = True
        '
        'PageMzkitTools
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.Controls.Add(Me.CustomTabControl1)
        Me.DoubleBuffered = True
        Me.Name = "PageMzkitTools"
        Me.ToolTip1.SetToolTip(Me, resources.GetString("$this.ToolTip"))
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.CustomTabControl1.ResumeLayout(False)
        Me.TabPage5.ResumeLayout(False)
        Me.TabPage6.ResumeLayout(False)
        CType(Me.BindingSource1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents CustomTabControl1 As CustomTabControl
    Friend WithEvents TabPage5 As TabPage
    Friend WithEvents TabPage6 As TabPage
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents OpenInTableViewerToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents SaveToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BindingSource1 As BindingSource
    Friend WithEvents CopyValueToolStripMenuItem As ToolStripMenuItem
End Class
