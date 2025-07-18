#Region "Microsoft.VisualBasic::da118f049c5edafb8cffe71f8c214ecc, mzkit\src\mzkit\mzkit\pages\dockWindow\explorer\frmSRMIonsExplorer.Designer.vb"

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

'   Total Lines: 262
'    Code Lines: 177
' Comment Lines: 80
'   Blank Lines: 5
'     File Size: 14.40 KB


' Class frmSRMIonsExplorer
' 
'     Sub: Dispose, InitializeComponent
' 
' /********************************************************************************/

#End Region

Imports ControlLibrary
Imports Mzkit_win32.BasicMDIForm

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmSRMIonsExplorer
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmSRMIonsExplorer))
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowTICOverlapToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TICToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BPCToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowTICOverlap3DToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.SelectAllIonsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearIonSelectionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ClearFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Win7StyleTreeView1 = New ControlLibrary.Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView(Me.components)
        Me.ContextMenuStrip3 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ImportsFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.SelectAllFilesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearFileSelectionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ClearFilesToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton2 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButton3 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton4 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripButton5 = New System.Windows.Forms.ToolStripButton()
        Me.ContextMenuStrip2 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowTICOverlapToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowTICOverlap3DToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripSeparator()
        Me.PeakFindingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetIndividualPeakFindingArgumentsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SwitchIonsDataViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.ContextMenuStrip3.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.ContextMenuStrip2.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowTICOverlapToolStripMenuItem, Me.ShowTICOverlap3DToolStripMenuItem, Me.ToolStripMenuItem1, Me.SwitchIonsDataViewToolStripMenuItem, Me.SelectAllIonsToolStripMenuItem, Me.ClearIonSelectionsToolStripMenuItem, Me.ToolStripMenuItem2, Me.ClearFilesToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        '
        'ShowTICOverlapToolStripMenuItem
        '
        Me.ShowTICOverlapToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TICToolStripMenuItem, Me.BPCToolStripMenuItem})
        resources.ApplyResources(Me.ShowTICOverlapToolStripMenuItem, "ShowTICOverlapToolStripMenuItem")
        Me.ShowTICOverlapToolStripMenuItem.Name = "ShowTICOverlapToolStripMenuItem"
        '
        'TICToolStripMenuItem
        '
        resources.ApplyResources(Me.TICToolStripMenuItem, "TICToolStripMenuItem")
        Me.TICToolStripMenuItem.Name = "TICToolStripMenuItem"
        '
        'BPCToolStripMenuItem
        '
        resources.ApplyResources(Me.BPCToolStripMenuItem, "BPCToolStripMenuItem")
        Me.BPCToolStripMenuItem.Name = "BPCToolStripMenuItem"
        '
        'ShowTICOverlap3DToolStripMenuItem
        '
        Me.ShowTICOverlap3DToolStripMenuItem.Name = "ShowTICOverlap3DToolStripMenuItem"
        resources.ApplyResources(Me.ShowTICOverlap3DToolStripMenuItem, "ShowTICOverlap3DToolStripMenuItem")
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        resources.ApplyResources(Me.ToolStripMenuItem1, "ToolStripMenuItem1")
        '
        'SelectAllIonsToolStripMenuItem
        '
        Me.SelectAllIonsToolStripMenuItem.Name = "SelectAllIonsToolStripMenuItem"
        resources.ApplyResources(Me.SelectAllIonsToolStripMenuItem, "SelectAllIonsToolStripMenuItem")
        '
        'ClearIonSelectionsToolStripMenuItem
        '
        Me.ClearIonSelectionsToolStripMenuItem.Name = "ClearIonSelectionsToolStripMenuItem"
        resources.ApplyResources(Me.ClearIonSelectionsToolStripMenuItem, "ClearIonSelectionsToolStripMenuItem")
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        resources.ApplyResources(Me.ToolStripMenuItem2, "ToolStripMenuItem2")
        '
        'ClearFilesToolStripMenuItem
        '
        resources.ApplyResources(Me.ClearFilesToolStripMenuItem, "ClearFilesToolStripMenuItem")
        Me.ClearFilesToolStripMenuItem.Name = "ClearFilesToolStripMenuItem"
        '
        'Win7StyleTreeView1
        '
        Me.Win7StyleTreeView1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Win7StyleTreeView1.CheckBoxes = True
        Me.Win7StyleTreeView1.ContextMenuStrip = Me.ContextMenuStrip3
        resources.ApplyResources(Me.Win7StyleTreeView1, "Win7StyleTreeView1")
        Me.Win7StyleTreeView1.FullRowSelect = True
        Me.Win7StyleTreeView1.HotTracking = True
        Me.Win7StyleTreeView1.ImageList = Me.ImageList1
        Me.Win7StyleTreeView1.ItemHeight = 16
        Me.Win7StyleTreeView1.Name = "Win7StyleTreeView1"
        Me.Win7StyleTreeView1.ShowLines = False
        '
        'ContextMenuStrip3
        '
        Me.ContextMenuStrip3.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ImportsFilesToolStripMenuItem, Me.ToolStripMenuItem3, Me.SelectAllFilesToolStripMenuItem, Me.ClearFileSelectionsToolStripMenuItem, Me.ToolStripMenuItem4, Me.ClearFilesToolStripMenuItem1})
        Me.ContextMenuStrip3.Name = "ContextMenuStrip3"
        resources.ApplyResources(Me.ContextMenuStrip3, "ContextMenuStrip3")
        '
        'ImportsFilesToolStripMenuItem
        '
        resources.ApplyResources(Me.ImportsFilesToolStripMenuItem, "ImportsFilesToolStripMenuItem")
        Me.ImportsFilesToolStripMenuItem.Name = "ImportsFilesToolStripMenuItem"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        resources.ApplyResources(Me.ToolStripMenuItem3, "ToolStripMenuItem3")
        '
        'SelectAllFilesToolStripMenuItem
        '
        Me.SelectAllFilesToolStripMenuItem.Name = "SelectAllFilesToolStripMenuItem"
        resources.ApplyResources(Me.SelectAllFilesToolStripMenuItem, "SelectAllFilesToolStripMenuItem")
        '
        'ClearFileSelectionsToolStripMenuItem
        '
        Me.ClearFileSelectionsToolStripMenuItem.Name = "ClearFileSelectionsToolStripMenuItem"
        resources.ApplyResources(Me.ClearFileSelectionsToolStripMenuItem, "ClearFileSelectionsToolStripMenuItem")
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        resources.ApplyResources(Me.ToolStripMenuItem4, "ToolStripMenuItem4")
        '
        'ClearFilesToolStripMenuItem1
        '
        resources.ApplyResources(Me.ClearFilesToolStripMenuItem1, "ClearFilesToolStripMenuItem1")
        Me.ClearFilesToolStripMenuItem1.Name = "ClearFilesToolStripMenuItem1"
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "folder-pictures.png")
        Me.ImageList1.Images.SetKeyName(1, "office_chart_area_256px_540041_easyicon.net.png")
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1, Me.ToolStripButton1, Me.ToolStripButton2, Me.ToolStripSeparator1, Me.ToolStripButton3, Me.ToolStripButton4, Me.ToolStripButton5})
        resources.ApplyResources(Me.ToolStrip1, "ToolStrip1")
        Me.ToolStrip1.Name = "ToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.IsLink = True
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        resources.ApplyResources(Me.ToolStripLabel1, "ToolStripLabel1")
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripButton1, "ToolStripButton1")
        Me.ToolStripButton1.Name = "ToolStripButton1"
        '
        'ToolStripButton2
        '
        Me.ToolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripButton2, "ToolStripButton2")
        Me.ToolStripButton2.Name = "ToolStripButton2"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        resources.ApplyResources(Me.ToolStripSeparator1, "ToolStripSeparator1")
        '
        'ToolStripButton3
        '
        Me.ToolStripButton3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripButton3, "ToolStripButton3")
        Me.ToolStripButton3.Name = "ToolStripButton3"
        '
        'ToolStripButton4
        '
        Me.ToolStripButton4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripButton4, "ToolStripButton4")
        Me.ToolStripButton4.Name = "ToolStripButton4"
        '
        'ToolStripButton5
        '
        Me.ToolStripButton5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        resources.ApplyResources(Me.ToolStripButton5, "ToolStripButton5")
        Me.ToolStripButton5.Name = "ToolStripButton5"
        '
        'ContextMenuStrip2
        '
        Me.ContextMenuStrip2.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowTICOverlapToolStripMenuItem1, Me.ShowTICOverlap3DToolStripMenuItem1, Me.ToolStripMenuItem5, Me.PeakFindingToolStripMenuItem, Me.SetIndividualPeakFindingArgumentsToolStripMenuItem})
        Me.ContextMenuStrip2.Name = "ContextMenuStrip2"
        resources.ApplyResources(Me.ContextMenuStrip2, "ContextMenuStrip2")
        '
        'ShowTICOverlapToolStripMenuItem1
        '
        resources.ApplyResources(Me.ShowTICOverlapToolStripMenuItem1, "ShowTICOverlapToolStripMenuItem1")
        Me.ShowTICOverlapToolStripMenuItem1.Name = "ShowTICOverlapToolStripMenuItem1"
        '
        'ShowTICOverlap3DToolStripMenuItem1
        '
        Me.ShowTICOverlap3DToolStripMenuItem1.Name = "ShowTICOverlap3DToolStripMenuItem1"
        resources.ApplyResources(Me.ShowTICOverlap3DToolStripMenuItem1, "ShowTICOverlap3DToolStripMenuItem1")
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        resources.ApplyResources(Me.ToolStripMenuItem5, "ToolStripMenuItem5")
        '
        'PeakFindingToolStripMenuItem
        '
        resources.ApplyResources(Me.PeakFindingToolStripMenuItem, "PeakFindingToolStripMenuItem")
        Me.PeakFindingToolStripMenuItem.Name = "PeakFindingToolStripMenuItem"
        '
        'SetIndividualPeakFindingArgumentsToolStripMenuItem
        '
        Me.SetIndividualPeakFindingArgumentsToolStripMenuItem.CheckOnClick = True
        resources.ApplyResources(Me.SetIndividualPeakFindingArgumentsToolStripMenuItem, "SetIndividualPeakFindingArgumentsToolStripMenuItem")
        Me.SetIndividualPeakFindingArgumentsToolStripMenuItem.Name = "SetIndividualPeakFindingArgumentsToolStripMenuItem"
        '
        'SwitchIonsDataViewToolStripMenuItem
        '
        Me.SwitchIonsDataViewToolStripMenuItem.CheckOnClick = True
        resources.ApplyResources(Me.SwitchIonsDataViewToolStripMenuItem, "SwitchIonsDataViewToolStripMenuItem")
        Me.SwitchIonsDataViewToolStripMenuItem.Name = "SwitchIonsDataViewToolStripMenuItem"
        '
        'frmSRMIonsExplorer
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.Win7StyleTreeView1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.DoubleBuffered = True
        Me.Name = "frmSRMIonsExplorer"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ContextMenuStrip3.ResumeLayout(False)
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ContextMenuStrip2.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ShowTICOverlapToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents Win7StyleTreeView1 As Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents ClearFilesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowTICOverlap3DToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ContextMenuStrip2 As ContextMenuStrip
    Friend WithEvents ShowTICOverlapToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ShowTICOverlap3DToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ClearIonSelectionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As ToolStripSeparator
    Friend WithEvents SelectAllIonsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ContextMenuStrip3 As ContextMenuStrip
    Friend WithEvents ImportsFilesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearFilesToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents TICToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BPCToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As ToolStripSeparator
    Friend WithEvents SelectAllFilesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearFileSelectionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As ToolStripSeparator
    Friend WithEvents ToolStripMenuItem5 As ToolStripSeparator
    Friend WithEvents PeakFindingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents ToolStripButton2 As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripButton3 As ToolStripButton
    Friend WithEvents ToolStripButton4 As ToolStripButton
    Friend WithEvents ToolStripButton5 As ToolStripButton
    Friend WithEvents SetIndividualPeakFindingArgumentsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SwitchIonsDataViewToolStripMenuItem As ToolStripMenuItem
End Class
