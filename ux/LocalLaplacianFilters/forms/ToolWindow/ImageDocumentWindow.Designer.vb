
Partial Class ImageDocumentWindow
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ImageDocumentWindow))
        Me.contextMenuTabPage = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.menuItem3 = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItem4 = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuItem5 = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.pictureBox1 = New System.Windows.Forms.PictureBox()
        Me.contextMenuTabPage.SuspendLayout()
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'contextMenuTabPage
        '
        Me.contextMenuTabPage.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuItem3, Me.menuItem4, Me.menuItem5})
        Me.contextMenuTabPage.Name = "contextMenuTabPage"
        Me.contextMenuTabPage.Size = New System.Drawing.Size(121, 70)
        '
        'menuItem3
        '
        Me.menuItem3.Name = "menuItem3"
        Me.menuItem3.Size = New System.Drawing.Size(120, 22)
        Me.menuItem3.Text = "Option &1"
        '
        'menuItem4
        '
        Me.menuItem4.Name = "menuItem4"
        Me.menuItem4.Size = New System.Drawing.Size(120, 22)
        Me.menuItem4.Text = "Option &2"
        '
        'menuItem5
        '
        Me.menuItem5.Name = "menuItem5"
        Me.menuItem5.Size = New System.Drawing.Size(120, 22)
        Me.menuItem5.Text = "Option &3"
        '
        'pictureBox1
        '
        Me.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLight
        Me.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pictureBox1.Location = New System.Drawing.Point(0, 4)
        Me.pictureBox1.Name = "pictureBox1"
        Me.pictureBox1.Size = New System.Drawing.Size(865, 684)
        Me.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.pictureBox1.TabIndex = 2
        Me.pictureBox1.TabStop = False
        '
        'ImageDocumentWindow
        '
        Me.ClientSize = New System.Drawing.Size(865, 688)
        Me.Controls.Add(Me.pictureBox1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "ImageDocumentWindow"
        Me.Padding = New System.Windows.Forms.Padding(0, 4, 0, 0)
        Me.TabPageContextMenuStrip = Me.contextMenuTabPage
        Me.contextMenuTabPage.ResumeLayout(False)
        CType(Me.pictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
#End Region
    Private contextMenuTabPage As Windows.Forms.ContextMenuStrip
    Private menuItem3 As Windows.Forms.ToolStripMenuItem
    Private menuItem4 As Windows.Forms.ToolStripMenuItem
    Private menuItem5 As Windows.Forms.ToolStripMenuItem
    Private toolTip As Windows.Forms.ToolTip
    Public WithEvents pictureBox1 As PictureBox
End Class

