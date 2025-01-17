﻿#Region "Microsoft.VisualBasic::939884989d4d3ba06a0eecfc7b42ead0, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmMsImagingViewer.Designer.vb"

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

'   Total Lines: 184
'    Code Lines: 123
' Comment Lines: 56
'   Blank Lines: 5
'     File Size: 9.98 KB


' Class frmMsImagingViewer
' 
'     Sub: Dispose, InitializeComponent
' 
' /********************************************************************************/

#End Region

Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.MSImagingViewerV2

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMsImagingViewer
    Inherits DocumentWindow
    'Inherits Form
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMsImagingViewer))
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.PinToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.SamplesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddSampleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExtractRegionSampleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem6 = New System.Windows.Forms.ToolStripSeparator()
        Me.AddSpatialTileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ImageProcessingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.SaveImageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CopyImageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ExportPlotToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExportMatrixToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PixelSelector1 = New Global.Mzkit_win32.MSImagingViewerV2.KpImageViewer()
        Me.RemoveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.PinToolStripMenuItem, Me.ClearToolStripMenuItem, Me.ToolStripMenuItem1, Me.SamplesToolStripMenuItem, Me.ToolStripMenuItem2, Me.ImageProcessingToolStripMenuItem, Me.ToolStripMenuItem3, Me.SaveImageToolStripMenuItem, Me.CopyImageToolStripMenuItem, Me.ToolStripMenuItem4, Me.ExportPlotToolStripMenuItem, Me.ExportMatrixToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        '
        'PinToolStripMenuItem
        '
        resources.ApplyResources(Me.PinToolStripMenuItem, "PinToolStripMenuItem")
        Me.PinToolStripMenuItem.Name = "PinToolStripMenuItem"
        '
        'ClearToolStripMenuItem
        '
        resources.ApplyResources(Me.ClearToolStripMenuItem, "ClearToolStripMenuItem")
        Me.ClearToolStripMenuItem.Name = "ClearToolStripMenuItem"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        resources.ApplyResources(Me.ToolStripMenuItem1, "ToolStripMenuItem1")
        '
        'SamplesToolStripMenuItem
        '
        Me.SamplesToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AddSampleToolStripMenuItem, Me.ClearToolStripMenuItem1, Me.ToolStripMenuItem5, Me.ExtractRegionSampleToolStripMenuItem, Me.RemoveToolStripMenuItem, Me.ToolStripMenuItem6, Me.AddSpatialTileToolStripMenuItem})
        Me.SamplesToolStripMenuItem.Name = "SamplesToolStripMenuItem"
        resources.ApplyResources(Me.SamplesToolStripMenuItem, "SamplesToolStripMenuItem")
        '
        'AddSampleToolStripMenuItem
        '
        resources.ApplyResources(Me.AddSampleToolStripMenuItem, "AddSampleToolStripMenuItem")
        Me.AddSampleToolStripMenuItem.Name = "AddSampleToolStripMenuItem"
        '
        'ClearToolStripMenuItem1
        '
        resources.ApplyResources(Me.ClearToolStripMenuItem1, "ClearToolStripMenuItem1")
        Me.ClearToolStripMenuItem1.Name = "ClearToolStripMenuItem1"
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        resources.ApplyResources(Me.ToolStripMenuItem5, "ToolStripMenuItem5")
        '
        'ExtractRegionSampleToolStripMenuItem
        '
        Me.ExtractRegionSampleToolStripMenuItem.Name = "ExtractRegionSampleToolStripMenuItem"
        resources.ApplyResources(Me.ExtractRegionSampleToolStripMenuItem, "ExtractRegionSampleToolStripMenuItem")
        '
        'ToolStripMenuItem6
        '
        Me.ToolStripMenuItem6.Name = "ToolStripMenuItem6"
        resources.ApplyResources(Me.ToolStripMenuItem6, "ToolStripMenuItem6")
        '
        'AddSpatialTileToolStripMenuItem
        '
        Me.AddSpatialTileToolStripMenuItem.Name = "AddSpatialTileToolStripMenuItem"
        resources.ApplyResources(Me.AddSpatialTileToolStripMenuItem, "AddSpatialTileToolStripMenuItem")
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        resources.ApplyResources(Me.ToolStripMenuItem2, "ToolStripMenuItem2")
        '
        'ImageProcessingToolStripMenuItem
        '
        resources.ApplyResources(Me.ImageProcessingToolStripMenuItem, "ImageProcessingToolStripMenuItem")
        Me.ImageProcessingToolStripMenuItem.Name = "ImageProcessingToolStripMenuItem"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        resources.ApplyResources(Me.ToolStripMenuItem3, "ToolStripMenuItem3")
        '
        'SaveImageToolStripMenuItem
        '
        resources.ApplyResources(Me.SaveImageToolStripMenuItem, "SaveImageToolStripMenuItem")
        Me.SaveImageToolStripMenuItem.Name = "SaveImageToolStripMenuItem"
        '
        'CopyImageToolStripMenuItem
        '
        resources.ApplyResources(Me.CopyImageToolStripMenuItem, "CopyImageToolStripMenuItem")
        Me.CopyImageToolStripMenuItem.Name = "CopyImageToolStripMenuItem"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        resources.ApplyResources(Me.ToolStripMenuItem4, "ToolStripMenuItem4")
        '
        'ExportPlotToolStripMenuItem
        '
        resources.ApplyResources(Me.ExportPlotToolStripMenuItem, "ExportPlotToolStripMenuItem")
        Me.ExportPlotToolStripMenuItem.Name = "ExportPlotToolStripMenuItem"
        '
        'ExportMatrixToolStripMenuItem
        '
        Me.ExportMatrixToolStripMenuItem.Name = "ExportMatrixToolStripMenuItem"
        resources.ApplyResources(Me.ExportMatrixToolStripMenuItem, "ExportMatrixToolStripMenuItem")
        '
        'PixelSelector1
        '
        Me.PixelSelector1.BackgroundColor = System.Drawing.SystemColors.ControlLight
        Me.PixelSelector1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.PixelSelector1.Cursor = System.Windows.Forms.Cursors.Cross
        resources.ApplyResources(Me.PixelSelector1, "PixelSelector1")
        Me.PixelSelector1.GifAnimation = False
        Me.PixelSelector1.Image = Nothing
        Me.PixelSelector1.Name = "PixelSelector1"
        Me.PixelSelector1.PreviewButton = True
        Me.PixelSelector1.Rotation = 0
        Me.PixelSelector1.SelectPolygonMode = False
        Me.PixelSelector1.ShowPointInform = True
        Me.PixelSelector1.ShowPreview = False
        '
        'RemoveToolStripMenuItem
        '
        Me.RemoveToolStripMenuItem.AutoToolTip = True
        Me.RemoveToolStripMenuItem.Name = "RemoveToolStripMenuItem"
        resources.ApplyResources(Me.RemoveToolStripMenuItem, "RemoveToolStripMenuItem")
        '
        'frmMsImagingViewer
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.PixelSelector1)
        Me.DoubleBuffered = True
        Me.Name = "frmMsImagingViewer"
        Me.TabPageContextMenuStrip = Me.DockContextMenuStrip1
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents SaveImageToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExportMatrixToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents PixelSelector1 As KpImageViewer
    Friend WithEvents PinToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents ExportPlotToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SamplesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddSampleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ImageProcessingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As ToolStripSeparator
    Friend WithEvents CopyImageToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As ToolStripSeparator
    Friend WithEvents ToolStripMenuItem5 As ToolStripSeparator
    Friend WithEvents ExtractRegionSampleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem6 As ToolStripSeparator
    Friend WithEvents AddSpatialTileToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RemoveToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolTip1 As ToolTip
End Class
