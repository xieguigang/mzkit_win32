<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmNmrSpectrumExplorer
    Inherits ToolWindow

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
        Me.components = New System.ComponentModel.Container()
        Me.Win7StyleTreeView1 = New ControlLibrary.Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView(Me.components)
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Win7StyleTreeView1
        '
        Me.Win7StyleTreeView1.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.Win7StyleTreeView1.CheckBoxes = True
        Me.Win7StyleTreeView1.Cursor = System.Windows.Forms.Cursors.Default
        Me.Win7StyleTreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Win7StyleTreeView1.FullRowSelect = True
        Me.Win7StyleTreeView1.HotTracking = True
        Me.Win7StyleTreeView1.Location = New System.Drawing.Point(0, 25)
        Me.Win7StyleTreeView1.Name = "Win7StyleTreeView1"
        Me.Win7StyleTreeView1.ShowLines = False
        Me.Win7StyleTreeView1.Size = New System.Drawing.Size(373, 595)
        Me.Win7StyleTreeView1.TabIndex = 2
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLabel1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(373, 25)
        Me.ToolStrip1.TabIndex = 3
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(82, 22)
        Me.ToolStripLabel1.Text = "Feature Peaks:"
        '
        'frmNmrSpectrumExplorer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(373, 620)
        Me.Controls.Add(Me.Win7StyleTreeView1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Name = "frmNmrSpectrumExplorer"
        Me.Text = "NMR Spectrum List"
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Win7StyleTreeView1 As ControlLibrary.Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
End Class
