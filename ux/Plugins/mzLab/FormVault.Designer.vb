Imports Mzkit_win32.BasicMDIForm

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FormVault : Inherits DocumentWindow

    'Form 重写 Dispose，以清理组件列表。
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

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormVault))
        Me.Win7StyleTreeView1 = New ControlLibrary.Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView(Me.components)
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Win7StyleTreeView1
        '
        Me.Win7StyleTreeView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Win7StyleTreeView1.HotTracking = True
        Me.Win7StyleTreeView1.ImageIndex = 0
        Me.Win7StyleTreeView1.ImageList = Me.ImageList1
        Me.Win7StyleTreeView1.Location = New System.Drawing.Point(0, 0)
        Me.Win7StyleTreeView1.Name = "Win7StyleTreeView1"
        Me.Win7StyleTreeView1.SelectedImageIndex = 0
        Me.Win7StyleTreeView1.ShowLines = False
        Me.Win7StyleTreeView1.Size = New System.Drawing.Size(306, 554)
        Me.Win7StyleTreeView1.TabIndex = 1
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.Win7StyleTreeView1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.PictureBox1)
        Me.SplitContainer1.Size = New System.Drawing.Size(919, 554)
        Me.SplitContainer1.SplitterDistance = 306
        Me.SplitContainer1.TabIndex = 2
        '
        'ImageList1
        '
        Me.ImageList1.ImageStream = CType(resources.GetObject("ImageList1.ImageStream"), System.Windows.Forms.ImageListStreamer)
        Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
        Me.ImageList1.Images.SetKeyName(0, "folder-documents.png")
        Me.ImageList1.Images.SetKeyName(1, "application-x-object.png")
        '
        'PictureBox1
        '
        Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PictureBox1.Location = New System.Drawing.Point(0, 0)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(609, 554)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'FormVault
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(919, 554)
        Me.Controls.Add(Me.SplitContainer1)
        Me.DoubleBuffered = True
        Me.Name = "FormVault"
        Me.TabPageContextMenuStrip = Me.DockContextMenuStrip1
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents Win7StyleTreeView1 As ControlLibrary.Kesoft.Windows.Forms.Win7StyleTreeView.Win7StyleTreeView
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents ImageList1 As ImageList
    Friend WithEvents PictureBox1 As PictureBox
End Class
