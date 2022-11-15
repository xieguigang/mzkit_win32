<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmCFMIDOutputViewer
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
        Me.components = New System.ComponentModel.Container()
        Dim TreeListViewItemCollectionComparer1 As System.Windows.Forms.TreeListViewItemCollection.TreeListViewItemCollectionComparer = New System.Windows.Forms.TreeListViewItemCollection.TreeListViewItemCollectionComparer()
        Me.ContextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.TreeListView1 = New System.Windows.Forms.TreeListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ContextMenuStrip1
        '
        Me.ContextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripMenuItem1, Me.ViewToolStripMenuItem})
        Me.ContextMenuStrip1.Name = "ContextMenuStrip1"
        Me.ContextMenuStrip1.Size = New System.Drawing.Size(181, 70)
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(180, 22)
        Me.ToolStripMenuItem1.Text = "Search Sample"
        '
        'TreeListView1
        '
        Me.TreeListView1.Activation = System.Windows.Forms.ItemActivation.OneClick
        Me.TreeListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader5, Me.ColumnHeader6})
        TreeListViewItemCollectionComparer1.Column = 0
        TreeListViewItemCollectionComparer1.SortOrder = System.Windows.Forms.SortOrder.Ascending
        Me.TreeListView1.Comparer = TreeListViewItemCollectionComparer1
        Me.TreeListView1.ContextMenuStrip = Me.ContextMenuStrip1
        Me.TreeListView1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TreeListView1.Font = New System.Drawing.Font("Microsoft YaHei", 11.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TreeListView1.GridLines = True
        Me.TreeListView1.HideSelection = False
        Me.TreeListView1.Location = New System.Drawing.Point(0, 0)
        Me.TreeListView1.Name = "TreeListView1"
        Me.TreeListView1.ShowItemToolTips = True
        Me.TreeListView1.Size = New System.Drawing.Size(800, 450)
        Me.TreeListView1.TabIndex = 1
        Me.TreeListView1.UseCompatibleStateImageBehavior = False
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Raw File/Feature"
        Me.ColumnHeader1.Width = 380
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "#Features"
        Me.ColumnHeader2.Width = 136
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Forward"
        Me.ColumnHeader3.Width = 158
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Reverse"
        Me.ColumnHeader4.Width = 175
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "m/z"
        Me.ColumnHeader5.Width = 101
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "rt"
        Me.ColumnHeader6.Width = 94
        '
        'ViewToolStripMenuItem
        '
        Me.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        Me.ViewToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.ViewToolStripMenuItem.Text = "View"
        '
        'frmCFMIDOutputViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.TreeListView1)
        Me.Name = "frmCFMIDOutputViewer"
        Me.Text = "View CFM-ID Prediction Outputs"
        Me.ContextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ContextMenuStrip1 As ContextMenuStrip
    Friend WithEvents ToolStripMenuItem1 As ToolStripMenuItem
    Friend WithEvents ViewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TreeListView1 As TreeListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents ColumnHeader2 As ColumnHeader
    Friend WithEvents ColumnHeader3 As ColumnHeader
    Friend WithEvents ColumnHeader4 As ColumnHeader
    Friend WithEvents ColumnHeader5 As ColumnHeader
    Friend WithEvents ColumnHeader6 As ColumnHeader
End Class
