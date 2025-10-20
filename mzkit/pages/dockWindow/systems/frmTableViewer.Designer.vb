﻿#Region "Microsoft.VisualBasic::2bf889b9fdb2fdbe4073b82cee9ff964, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmTableViewer.Designer.vb"

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

'   Total Lines: 128
'    Code Lines: 92
' Comment Lines: 32
'   Blank Lines: 4
'     File Size: 7.02 KB


' Class frmTableViewer
' 
'     Sub: Dispose, InitializeComponent
' 
' /********************************************************************************/

#End Region

Imports Galaxy.Data.TableSheet
Imports Galaxy.Workbench.DockDocument

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmTableViewer
    Inherits DocumentWindow
    ' Inherits Form

    'Form overrides dispose to clean up the component list.
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmTableViewer))
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.VisualizeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ActionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SendToREnvironmentToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.TransposeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportTableToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AdvancedDataGridView1 = New AdvancedDataGridView()
        Me.AdvancedDataGridViewSearchToolBar1 = New AdvancedDataGridViewSearchToolBar()

        Me.SendToToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MSImagingIonListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.AdvancedDataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()

        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ViewToolStripMenuItem, Me.ToolStripMenuItem1, Me.VisualizeToolStripMenuItem, Me.ActionsToolStripMenuItem, Me.SendToToolStripMenuItem, Me.ToolStripMenuItem2, Me.TransposeToolStripMenuItem, Me.CopyToolStripMenuItem, Me.ExportTableToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(204, 214)
        '
        'ViewToolStripMenuItem
        '
        Me.ViewToolStripMenuItem.Image = CType(resources.GetObject("ViewToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        Me.ViewToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
        Me.ViewToolStripMenuItem.Text = "View"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(200, 6)
        '
        'VisualizeToolStripMenuItem
        '
        Me.VisualizeToolStripMenuItem.Image = CType(resources.GetObject("VisualizeToolStripMenuItem.Image"), System.Drawing.Image)
        Me.VisualizeToolStripMenuItem.Name = "VisualizeToolStripMenuItem"
        Me.VisualizeToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
        Me.VisualizeToolStripMenuItem.Text = "Visualize"
        '
        'ActionsToolStripMenuItem
        '
        Me.ActionsToolStripMenuItem.Image = CType(resources.GetObject("ActionsToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ActionsToolStripMenuItem.Name = "ActionsToolStripMenuItem"
        Me.ActionsToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
        Me.ActionsToolStripMenuItem.Text = "Actions"
        '
        'SendToREnvironmentToolStripMenuItem
        '
        Me.SendToREnvironmentToolStripMenuItem.Name = "SendToREnvironmentToolStripMenuItem"
        Me.SendToREnvironmentToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
        Me.SendToREnvironmentToolStripMenuItem.Text = "R# Environment"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(200, 6)
        '
        'TransposeToolStripMenuItem
        '
        Me.TransposeToolStripMenuItem.Name = "TransposeToolStripMenuItem"
        Me.TransposeToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
        Me.TransposeToolStripMenuItem.Text = "Transpose"
        '
        'CopyToolStripMenuItem
        '
        Me.CopyToolStripMenuItem.Image = CType(resources.GetObject("CopyToolStripMenuItem.Image"), System.Drawing.Image)
        Me.CopyToolStripMenuItem.Name = "CopyToolStripMenuItem"
        Me.CopyToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
        Me.CopyToolStripMenuItem.Text = "Copy"
        '
        'ExportTableToolStripMenuItem
        '
        Me.ExportTableToolStripMenuItem.Image = CType(resources.GetObject("ExportTableToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ExportTableToolStripMenuItem.Name = "ExportTableToolStripMenuItem"
        Me.ExportTableToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
        Me.ExportTableToolStripMenuItem.Text = "Export Table"
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
        Me.AdvancedDataGridView1.Size = New System.Drawing.Size(834, 468)
        Me.AdvancedDataGridView1.SortStringChangedInvokeBeforeDatasourceUpdate = True
        Me.AdvancedDataGridView1.TabIndex = 1
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
        Me.AdvancedDataGridViewSearchToolBar1.Size = New System.Drawing.Size(834, 27)
        Me.AdvancedDataGridViewSearchToolBar1.TabIndex = 2
        Me.AdvancedDataGridViewSearchToolBar1.Text = "AdvancedDataGridViewSearchToolBar1"
        '
        'SendToToolStripMenuItem
        '
        Me.SendToToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MSImagingIonListToolStripMenuItem, Me.SendToREnvironmentToolStripMenuItem})
        Me.SendToToolStripMenuItem.Name = "SendToToolStripMenuItem"
        Me.SendToToolStripMenuItem.Size = New System.Drawing.Size(203, 22)
        Me.SendToToolStripMenuItem.Text = "Send To"
        '
        'MSImagingIonListToolStripMenuItem
        '
        Me.MSImagingIonListToolStripMenuItem.Name = "MSImagingIonListToolStripMenuItem"
        Me.MSImagingIonListToolStripMenuItem.Size = New System.Drawing.Size(181, 22)
        Me.MSImagingIonListToolStripMenuItem.Text = "MS-Imaging Ion List"
        '
        'frmTableViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(834, 495)
        Me.Controls.Add(Me.AdvancedDataGridView1)
        Me.Controls.Add(Me.AdvancedDataGridViewSearchToolBar1)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "frmTableViewer"
        Me.TabPageContextMenuStrip = Me.DockContextMenuStrip1
        Me.ContextMenuStrip1.ResumeLayout(False)
        CType(Me.AdvancedDataGridView1, System.ComponentModel.ISupportInitialize).EndInit()

        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ViewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents VisualizeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SendToREnvironmentToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ActionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AdvancedDataGridView1 As AdvancedDataGridView
    Friend WithEvents AdvancedDataGridViewSearchToolBar1 As AdvancedDataGridViewSearchToolBar

    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents ExportTableToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CopyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TransposeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SendToToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MSImagingIonListToolStripMenuItem As ToolStripMenuItem
End Class
