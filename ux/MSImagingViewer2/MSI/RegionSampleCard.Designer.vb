<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class RegionSampleCard
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RegionSampleCard))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ExtractRegionSampleDataToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewMS1SpectrumToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.MoveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetHTMLColorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.ContextMenuStrip1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        resources.ApplyResources(Me.Label1, "Label1")
        Me.Label1.Name = "Label1"
        '
        'Label2
        '
        resources.ApplyResources(Me.Label2, "Label2")
        Me.Label2.Name = "Label2"
        '
        'TextBox1
        '
        resources.ApplyResources(Me.TextBox1, "TextBox1")
        Me.TextBox1.Name = "TextBox1"
        '
        'TextBox2
        '
        resources.ApplyResources(Me.TextBox2, "TextBox2")
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.ReadOnly = True
        '
        'Label3
        '
        resources.ApplyResources(Me.Label3, "Label3")
        Me.Label3.Name = "Label3"
        '
        'TextBox3
        '
        resources.ApplyResources(Me.TextBox3, "TextBox3")
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.ReadOnly = True
        '
        'LinkLabel1
        '
        resources.ApplyResources(Me.LinkLabel1, "LinkLabel1")
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.TabStop = True
        '
        'ContextMenuStrip1
        '
        resources.ApplyResources(Me.ContextMenuStrip1, "ContextMenuStrip1")
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ExtractRegionSampleDataToolStripMenuItem, Me.ViewMS1SpectrumToolStripMenuItem, Me.ToolStripMenuItem1, Me.MoveToolStripMenuItem, Me.SetHTMLColorToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        '
        'ExtractRegionSampleDataToolStripMenuItem
        '
        resources.ApplyResources(Me.ExtractRegionSampleDataToolStripMenuItem, "ExtractRegionSampleDataToolStripMenuItem")
        Me.ExtractRegionSampleDataToolStripMenuItem.Name = "ExtractRegionSampleDataToolStripMenuItem"
        '
        'ViewMS1SpectrumToolStripMenuItem
        '
        resources.ApplyResources(Me.ViewMS1SpectrumToolStripMenuItem, "ViewMS1SpectrumToolStripMenuItem")
        Me.ViewMS1SpectrumToolStripMenuItem.Name = "ViewMS1SpectrumToolStripMenuItem"
        '
        'ToolStripMenuItem1
        '
        resources.ApplyResources(Me.ToolStripMenuItem1, "ToolStripMenuItem1")
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        '
        'MoveToolStripMenuItem
        '
        resources.ApplyResources(Me.MoveToolStripMenuItem, "MoveToolStripMenuItem")
        Me.MoveToolStripMenuItem.Name = "MoveToolStripMenuItem"
        '
        'SetHTMLColorToolStripMenuItem
        '
        resources.ApplyResources(Me.SetHTMLColorToolStripMenuItem, "SetHTMLColorToolStripMenuItem")
        Me.SetHTMLColorToolStripMenuItem.Name = "SetHTMLColorToolStripMenuItem"
        '
        'PictureBox1
        '
        resources.ApplyResources(Me.PictureBox1, "PictureBox1")
        Me.PictureBox1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.TabStop = False
        '
        'RegionSampleCard
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.LinkLabel1)
        Me.Controls.Add(Me.TextBox3)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.TextBox2)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Label1)
        Me.Name = "RegionSampleCard"
        Me.ContextMenuStrip1.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents Label2 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents TextBox3 As TextBox
    Friend WithEvents LinkLabel1 As LinkLabel
    Public WithEvents ContextMenuStrip1 As ContextMenuStrip
    Public WithEvents ViewMS1SpectrumToolStripMenuItem As ToolStripMenuItem
    Public WithEvents SetHTMLColorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents MoveToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExtractRegionSampleDataToolStripMenuItem As ToolStripMenuItem
End Class
