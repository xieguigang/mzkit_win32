﻿#Region "Microsoft.VisualBasic::4db438d4ecb88155296bf77deb2f928d, mzkit\src\mzkit\mzkit\forms\frmTweaks\frmMsImagingTweaks.Designer.vb"

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

'   Total Lines: 242
'    Code Lines: 167
' Comment Lines: 69
'   Blank Lines: 6
'     File Size: 12.53 KB


' Class frmMsImagingTweaks
' 
'     Sub: Dispose, InitializeComponent
' 
' /********************************************************************************/

#End Region

Imports ControlLibrary
Imports ControlLibrary.Kesoft.Windows.Forms.Win7StyleTreeView
Imports Mzkit_win32.BasicMDIForm

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMsImagingTweaks
    Inherits ToolWindow
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMsImagingTweaks))
        Me.PropertyGrid1 = New System.Windows.Forms.PropertyGrid()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BoxPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BarPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViolinPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.IntensityHistogramToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.AppendMSImagingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RenderLayerCompositionModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RenderingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.LoadBasePeakIonsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ClearSelectionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.Win7StyleTreeView1 = New ControlLibrary.Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView(Me.components)
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripSpringTextBox1 = New ToolStripSpringTextBox()
        Me.ViewLayerButton = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton2 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButton3 = New System.Windows.Forms.ToolStripButton()
        Me.ExportEachSelectedLayersToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SelectAllToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'PropertyGrid1
        '
        Me.PropertyGrid1.AllowDrop = True
        Me.PropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PropertyGrid1.LineColor = System.Drawing.SystemColors.ScrollBar
        Me.PropertyGrid1.Location = New System.Drawing.Point(0, 0)
        Me.PropertyGrid1.Name = "PropertyGrid1"
        Me.PropertyGrid1.Size = New System.Drawing.Size(377, 414)
        Me.PropertyGrid1.TabIndex = 0
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowPlotToolStripMenuItem, Me.RenderLayerCompositionModeToolStripMenuItem, Me.RenderingToolStripMenuItem, Me.ExportEachSelectedLayersToolStripMenuItem, Me.ToolStripMenuItem1, Me.LoadBasePeakIonsToolStripMenuItem, Me.ToolStripMenuItem2, Me.SelectAllToolStripMenuItem, Me.ClearSelectionToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(262, 192)
        '
        'ShowPlotToolStripMenuItem
        '
        Me.ShowPlotToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BoxPlotToolStripMenuItem, Me.BarPlotToolStripMenuItem, Me.ViolinPlotToolStripMenuItem, Me.ToolStripMenuItem3, Me.IntensityHistogramToolStripMenuItem, Me.ToolStripMenuItem4, Me.AppendMSImagingToolStripMenuItem})
        Me.ShowPlotToolStripMenuItem.Image = CType(resources.GetObject("ShowPlotToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ShowPlotToolStripMenuItem.Name = "ShowPlotToolStripMenuItem"
        Me.ShowPlotToolStripMenuItem.Size = New System.Drawing.Size(261, 22)
        Me.ShowPlotToolStripMenuItem.Text = "Show Plot"
        '
        'BoxPlotToolStripMenuItem
        '
        Me.BoxPlotToolStripMenuItem.Name = "BoxPlotToolStripMenuItem"
        Me.BoxPlotToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.BoxPlotToolStripMenuItem.Text = "Box Plot"
        '
        'BarPlotToolStripMenuItem
        '
        Me.BarPlotToolStripMenuItem.Name = "BarPlotToolStripMenuItem"
        Me.BarPlotToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.BarPlotToolStripMenuItem.Text = "Bar Plot"
        '
        'ViolinPlotToolStripMenuItem
        '
        Me.ViolinPlotToolStripMenuItem.Name = "ViolinPlotToolStripMenuItem"
        Me.ViolinPlotToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.ViolinPlotToolStripMenuItem.Text = "Violin Plot"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(182, 6)
        '
        'IntensityHistogramToolStripMenuItem
        '
        Me.IntensityHistogramToolStripMenuItem.Image = CType(resources.GetObject("IntensityHistogramToolStripMenuItem.Image"), System.Drawing.Image)
        Me.IntensityHistogramToolStripMenuItem.Name = "IntensityHistogramToolStripMenuItem"
        Me.IntensityHistogramToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.IntensityHistogramToolStripMenuItem.Text = "Intensity Histogram"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(182, 6)
        '
        'AppendMSImagingToolStripMenuItem
        '
        Me.AppendMSImagingToolStripMenuItem.Checked = True
        Me.AppendMSImagingToolStripMenuItem.CheckOnClick = True
        Me.AppendMSImagingToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.AppendMSImagingToolStripMenuItem.Image = CType(resources.GetObject("AppendMSImagingToolStripMenuItem.Image"), System.Drawing.Image)
        Me.AppendMSImagingToolStripMenuItem.Name = "AppendMSImagingToolStripMenuItem"
        Me.AppendMSImagingToolStripMenuItem.Size = New System.Drawing.Size(185, 22)
        Me.AppendMSImagingToolStripMenuItem.Text = "Append MS-Imaging"
        '
        'RenderLayerCompositionModeToolStripMenuItem
        '
        Me.RenderLayerCompositionModeToolStripMenuItem.Name = "RenderLayerCompositionModeToolStripMenuItem"
        Me.RenderLayerCompositionModeToolStripMenuItem.Size = New System.Drawing.Size(261, 22)
        Me.RenderLayerCompositionModeToolStripMenuItem.Text = "Render (Layers Composition Mode)"
        '
        'RenderingToolStripMenuItem
        '
        Me.RenderingToolStripMenuItem.Image = CType(resources.GetObject("RenderingToolStripMenuItem.Image"), System.Drawing.Image)
        Me.RenderingToolStripMenuItem.Name = "RenderingToolStripMenuItem"
        Me.RenderingToolStripMenuItem.Size = New System.Drawing.Size(261, 22)
        Me.RenderingToolStripMenuItem.Text = "Render"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(258, 6)
        '
        'LoadBasePeakIonsToolStripMenuItem
        '
        Me.LoadBasePeakIonsToolStripMenuItem.Name = "LoadBasePeakIonsToolStripMenuItem"
        Me.LoadBasePeakIonsToolStripMenuItem.Size = New System.Drawing.Size(261, 22)
        Me.LoadBasePeakIonsToolStripMenuItem.Text = "Load BasePeak Ions"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(258, 6)
        '
        'ClearSelectionToolStripMenuItem
        '
        Me.ClearSelectionToolStripMenuItem.Image = CType(resources.GetObject("ClearSelectionToolStripMenuItem.Image"), System.Drawing.Image)
        Me.ClearSelectionToolStripMenuItem.Name = "ClearSelectionToolStripMenuItem"
        Me.ClearSelectionToolStripMenuItem.Size = New System.Drawing.Size(261, 22)
        Me.ClearSelectionToolStripMenuItem.Text = "Clear Selection"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 25)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.Win7StyleTreeView1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.PropertyGrid1)
        Me.SplitContainer1.Size = New System.Drawing.Size(377, 529)
        Me.SplitContainer1.SplitterDistance = 111
        Me.SplitContainer1.TabIndex = 2
        '
        'Win7StyleTreeView1
        '
        Me.Win7StyleTreeView1.CheckBoxes = True
        Me.Win7StyleTreeView1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.Win7StyleTreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Win7StyleTreeView1.FullRowSelect = True
        Me.Win7StyleTreeView1.HotTracking = True
        Me.Win7StyleTreeView1.Location = New System.Drawing.Point(0, 0)
        Me.Win7StyleTreeView1.Name = "Win7StyleTreeView1"
        Me.Win7StyleTreeView1.ShowLines = False
        Me.Win7StyleTreeView1.ShowNodeToolTips = True
        Me.Win7StyleTreeView1.Size = New System.Drawing.Size(377, 111)
        Me.Win7StyleTreeView1.TabIndex = 0
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "folder-pictures.png")
        Me.ImageList1.Images.SetKeyName(1, "pix.png")
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.ToolStripSpringTextBox1, Me.ViewLayerButton, Me.ToolStripButton2, Me.ToolStripButton1, Me.ToolStripSeparator1, Me.ToolStripButton3})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(377, 25)
        Me.ToolStrip1.TabIndex = 3
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(66, 22)
        Me.ToolStripLabel1.Text = "MSI Target:"
        '
        'ToolStripSpringTextBox1
        '
        Me.ToolStripSpringTextBox1.Name = "ToolStripSpringTextBox1"
        Me.ToolStripSpringTextBox1.Size = New System.Drawing.Size(170, 25)
        '
        'ViewLayerButton
        '
        Me.ViewLayerButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ViewLayerButton.Image = CType(resources.GetObject("ViewLayerButton.Image"), System.Drawing.Image)
        Me.ViewLayerButton.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ViewLayerButton.Name = "ViewLayerButton"
        Me.ViewLayerButton.Size = New System.Drawing.Size(23, 22)
        Me.ViewLayerButton.Text = "View MS-Imaging Layer"
        '
        'ToolStripButton2
        '
        Me.ToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton2.Image = CType(resources.GetObject("ToolStripButton2.Image"), System.Drawing.Image)
        Me.ToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton2.Name = "ToolStripButton2"
        Me.ToolStripButton2.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton2.Text = "Add Ion Layer"
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton1.Text = "Imports A List of Ions"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripButton3
        '
        Me.ToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton3.Image = CType(resources.GetObject("ToolStripButton3.Image"), System.Drawing.Image)
        Me.ToolStripButton3.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton3.Name = "ToolStripButton3"
        Me.ToolStripButton3.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton3.Text = "Clear Layers"
        '
        'ExportEachSelectedLayersToolStripMenuItem
        '
        Me.ExportEachSelectedLayersToolStripMenuItem.Name = "ExportEachSelectedLayersToolStripMenuItem"
        Me.ExportEachSelectedLayersToolStripMenuItem.Size = New System.Drawing.Size(261, 22)
        Me.ExportEachSelectedLayersToolStripMenuItem.Text = "Export Each Selected Layers"
        '
        'SelectAllToolStripMenuItem
        '
        Me.SelectAllToolStripMenuItem.Name = "SelectAllToolStripMenuItem"
        Me.SelectAllToolStripMenuItem.Size = New System.Drawing.Size(261, 22)
        Me.SelectAllToolStripMenuItem.Text = "Select All"
        '
        'frmMsImagingTweaks
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(377, 554)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.DoubleBuffered = True
        Me.Name = "frmMsImagingTweaks"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents PropertyGrid1 As PropertyGrid
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents RenderingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents ClearSelectionToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents ToolStripSpringTextBox1 As ToolStripSpringTextBox
    Friend WithEvents ViewLayerButton As ToolStripButton
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents ToolStripButton2 As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripButton3 As ToolStripButton
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents Win7StyleTreeView1 As Win7StyleTreeView
    Friend WithEvents RenderLayerCompositionModeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LoadBasePeakIonsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents ShowPlotToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BoxPlotToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BarPlotToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ViolinPlotToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As ToolStripSeparator
    Friend WithEvents AppendMSImagingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents IntensityHistogramToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As ToolStripSeparator
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents ExportEachSelectedLayersToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SelectAllToolStripMenuItem As ToolStripMenuItem
End Class
