
Partial Class ToolWindow
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If

        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.contextMenuStrip1 = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.option1ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.option2ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.option3ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.contextMenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'contextMenuStrip1
        '
        Me.contextMenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.option1ToolStripMenuItem, Me.option2ToolStripMenuItem, Me.option3ToolStripMenuItem})
        Me.contextMenuStrip1.Name = "contextMenuStrip1"
        Me.contextMenuStrip1.Size = New System.Drawing.Size(118, 70)
        '
        'option1ToolStripMenuItem
        '
        Me.option1ToolStripMenuItem.Name = "option1ToolStripMenuItem"
        Me.option1ToolStripMenuItem.Size = New System.Drawing.Size(117, 22)
        Me.option1ToolStripMenuItem.Text = "Option&1"
        '
        'option2ToolStripMenuItem
        '
        Me.option2ToolStripMenuItem.Name = "option2ToolStripMenuItem"
        Me.option2ToolStripMenuItem.Size = New System.Drawing.Size(117, 22)
        Me.option2ToolStripMenuItem.Text = "Option&2"
        '
        'option3ToolStripMenuItem
        '
        Me.option3ToolStripMenuItem.Name = "option3ToolStripMenuItem"
        Me.option3ToolStripMenuItem.Size = New System.Drawing.Size(117, 22)
        Me.option3ToolStripMenuItem.Text = "Option&3"
        '
        'ToolWindow
        '
        Me.ClientSize = New System.Drawing.Size(269, 400)
        Me.DockAreas = CType(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) _
            Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) _
            Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) _
            Or WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom), WeifenLuo.WinFormsUI.Docking.DockAreas)
        Me.Name = "ToolWindow"
        Me.TabPageContextMenuStrip = Me.contextMenuStrip1
        Me.TabText = "ToolWindow"
        Me.Text = "ToolWindow"
        Me.contextMenuStrip1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private contextMenuStrip1 As Windows.Forms.ContextMenuStrip
    Private option1ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private option2ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private option3ToolStripMenuItem As Windows.Forms.ToolStripMenuItem
End Class

