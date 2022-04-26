﻿#Region "Microsoft.VisualBasic::9e26f5bfb935750138a24c1429de3d8b, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmGCMS_CDFExplorer.Designer.vb"

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

    '   Total Lines: 81
    '    Code Lines: 58
    ' Comment Lines: 17
    '   Blank Lines: 6
    '     File Size: 3.33 KB


    ' Class frmGCMS_CDFExplorer
    ' 
    '     Sub: Dispose, InitializeComponent
    ' 
    ' /********************************************************************************/

#End Region

Imports ControlLibrary

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmGCMS_CDFExplorer
    Inherits DocumentWindow

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.RtRangeSelector1 = New RtRangeSelector()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'RtRangeSelector1
        '
        Me.RtRangeSelector1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.RtRangeSelector1.SelectedColor = System.Drawing.Color.Green
        Me.RtRangeSelector1.Location = New System.Drawing.Point(3, 3)
        Me.RtRangeSelector1.Name = "RtRangeSelector1"
        Me.RtRangeSelector1.Size = New System.Drawing.Size(976, 82)
        Me.RtRangeSelector1.TabIndex = 0
        '
        'PictureBox1
        '
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(982, 489)
        Me.PictureBox1.TabIndex = 1
        Me.PictureBox1.TabStop = False
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.RtRangeSelector1)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.Panel1.Location = New System.Drawing.Point(0, 489)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(982, 88)
        Me.Panel1.TabIndex = 2
        '
        'frmGCMS_CDFExplorer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(982, 577)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Panel1)
        Me.Name = "frmGCMS_CDFExplorer"
        Me.Text = "Form1"
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents RtRangeSelector1 As RtRangeSelector
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Panel1 As Panel
End Class
