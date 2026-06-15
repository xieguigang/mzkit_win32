#Region "Microsoft.VisualBasic::9028ad3de18e89ab066f734d9e7dfe0e, mzkit\src\mzkit\mzkit\forms\frmMain.Designer.vb"

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

'   Total Lines: 188
'    Code Lines: 131
' Comment Lines: 51
'   Blank Lines: 6
'     File Size: 9.99 KB


' Class frmMain
' 
'     Sub: Dispose, InitializeComponent
' 
' /********************************************************************************/

#End Region

Imports System.Windows.Forms
Imports RibbonLib

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
    ' Inherits System.Windows.Forms.Form
    Inherits Form

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
        components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        StatusStrip = New StatusStrip()
        ToolStripStatusLabel1 = New ToolStripStatusLabel()
        ToolStripDropDownButton1 = New ToolStripDropDownButton()
        MoleculeNetworkingToolStripMenuItem = New ToolStripMenuItem()
        FormulaSearchToolToolStripMenuItem = New ToolStripMenuItem()
        MzCalculatorToolStripMenuItem = New ToolStripMenuItem()
        RawFileViewerToolStripMenuItem = New ToolStripMenuItem()
        ToolStripStatusLabel2 = New ToolStripStatusLabel()
        ToolStripStatusLabel3 = New ToolStripStatusLabel()
        ToolStripProgressBar1 = New ToolStripProgressBar()
        ToolStripStatusLabel4 = New ToolStripStatusLabel()
        ToolTip = New ToolTip(components)
        Ribbon1 = New Ribbon()
        PanelBase = New Panel()
        Timer1 = New Timer(components)
        ToolStripStatusLabel5 = New ToolStripStatusLabel()
        StatusStrip.SuspendLayout()
        SuspendLayout()
        ' 
        ' StatusStrip
        ' 
        resources.ApplyResources(StatusStrip, "StatusStrip")
        StatusStrip.Items.AddRange(New ToolStripItem() {ToolStripStatusLabel1, ToolStripStatusLabel2, ToolStripStatusLabel3, ToolStripProgressBar1, ToolStripStatusLabel4, ToolStripStatusLabel5, ToolStripDropDownButton1})
        StatusStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow
        StatusStrip.Name = "StatusStrip"
        StatusStrip.RenderMode = ToolStripRenderMode.Professional
        ToolTip.SetToolTip(StatusStrip, resources.GetString("StatusStrip.ToolTip"))
        ' 
        ' ToolStripStatusLabel1
        ' 
        resources.ApplyResources(ToolStripStatusLabel1, "ToolStripStatusLabel1")
        ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        ' 
        ' ToolStripDropDownButton1
        ' 
        resources.ApplyResources(ToolStripDropDownButton1, "ToolStripDropDownButton1")
        ToolStripDropDownButton1.DropDownItems.AddRange(New ToolStripItem() {MoleculeNetworkingToolStripMenuItem, FormulaSearchToolToolStripMenuItem, MzCalculatorToolStripMenuItem, RawFileViewerToolStripMenuItem})
        ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
        ' 
        ' MoleculeNetworkingToolStripMenuItem
        ' 
        resources.ApplyResources(MoleculeNetworkingToolStripMenuItem, "MoleculeNetworkingToolStripMenuItem")
        MoleculeNetworkingToolStripMenuItem.Name = "MoleculeNetworkingToolStripMenuItem"
        ' 
        ' FormulaSearchToolToolStripMenuItem
        ' 
        resources.ApplyResources(FormulaSearchToolToolStripMenuItem, "FormulaSearchToolToolStripMenuItem")
        FormulaSearchToolToolStripMenuItem.Name = "FormulaSearchToolToolStripMenuItem"
        ' 
        ' MzCalculatorToolStripMenuItem
        ' 
        resources.ApplyResources(MzCalculatorToolStripMenuItem, "MzCalculatorToolStripMenuItem")
        MzCalculatorToolStripMenuItem.Name = "MzCalculatorToolStripMenuItem"
        ' 
        ' RawFileViewerToolStripMenuItem
        ' 
        resources.ApplyResources(RawFileViewerToolStripMenuItem, "RawFileViewerToolStripMenuItem")
        RawFileViewerToolStripMenuItem.Name = "RawFileViewerToolStripMenuItem"
        ' 
        ' ToolStripStatusLabel2
        ' 
        resources.ApplyResources(ToolStripStatusLabel2, "ToolStripStatusLabel2")
        ToolStripStatusLabel2.Name = "ToolStripStatusLabel2"
        ' 
        ' ToolStripStatusLabel3
        ' 
        resources.ApplyResources(ToolStripStatusLabel3, "ToolStripStatusLabel3")
        ToolStripStatusLabel3.Name = "ToolStripStatusLabel3"
        ' 
        ' ToolStripProgressBar1
        ' 
        resources.ApplyResources(ToolStripProgressBar1, "ToolStripProgressBar1")
        ToolStripProgressBar1.Name = "ToolStripProgressBar1"
        ToolStripProgressBar1.Value = 100
        ' 
        ' ToolStripStatusLabel4
        ' 
        resources.ApplyResources(ToolStripStatusLabel4, "ToolStripStatusLabel4")
        ToolStripStatusLabel4.IsLink = True
        ToolStripStatusLabel4.Name = "ToolStripStatusLabel4"
        ' 
        ' Ribbon1
        ' 
        resources.ApplyResources(Ribbon1, "Ribbon1")
        Ribbon1.Name = "Ribbon1"
        Ribbon1.ResourceIdentifier = Nothing
        Ribbon1.ResourceName = "BioNovoGene.mzkit_win32.RibbonMarkup.ribbon"
        Ribbon1.ShortcutTableResourceName = Nothing
        ToolTip.SetToolTip(Ribbon1, resources.GetString("Ribbon1.ToolTip"))
        ' 
        ' PanelBase
        ' 
        resources.ApplyResources(PanelBase, "PanelBase")
        PanelBase.Name = "PanelBase"
        ToolTip.SetToolTip(PanelBase, resources.GetString("PanelBase.ToolTip"))
        ' 
        ' Timer1
        ' 
        Timer1.Enabled = True
        Timer1.Interval = 1000
        ' 
        ' ToolStripStatusLabel5
        ' 
        resources.ApplyResources(ToolStripStatusLabel5, "ToolStripStatusLabel5")
        ToolStripStatusLabel5.Name = "ToolStripStatusLabel5"
        ToolStripStatusLabel5.Spring = True
        ' 
        ' frmMain
        ' 
        resources.ApplyResources(Me, "$this")
        AutoScaleMode = AutoScaleMode.Inherit
        BackColor = Drawing.SystemColors.Control
        Controls.Add(PanelBase)
        Controls.Add(StatusStrip)
        Controls.Add(Ribbon1)
        Name = "frmMain"
        ToolTip.SetToolTip(Me, resources.GetString("$this.ToolTip"))
        StatusStrip.ResumeLayout(False)
        StatusStrip.PerformLayout()
        ResumeLayout(False)
        PerformLayout()

    End Sub
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents StatusStrip As System.Windows.Forms.StatusStrip
    Friend WithEvents Ribbon1 As Ribbon
    Friend WithEvents ToolStripDropDownButton1 As ToolStripDropDownButton
    Friend WithEvents FormulaSearchToolToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MzCalculatorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RawFileViewerToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents MoleculeNetworkingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripStatusLabel2 As ToolStripStatusLabel
    Friend WithEvents PanelBase As Panel
    Friend WithEvents ToolStripStatusLabel3 As ToolStripStatusLabel
    Friend WithEvents Timer1 As Timer
    Friend WithEvents ToolStripProgressBar1 As ToolStripProgressBar
    Friend WithEvents ToolStripStatusLabel4 As ToolStripStatusLabel
    Friend WithEvents ToolStripStatusLabel5 As ToolStripStatusLabel
End Class
