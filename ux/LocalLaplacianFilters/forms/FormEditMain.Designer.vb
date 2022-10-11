
Partial Class FormEditMain
    ''' <summary>
    ''' Требуется переменная конструктора.
    ''' </summary>
    Private components As ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Освободить все используемые ресурсы.
    ''' </summary>
    ''' <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Код, автоматически созданный конструктором форм Windows"

    ''' <summary>
    ''' Обязательный метод для поддержки конструктора - не изменяйте
    ''' содержимое данного метода при помощи редактора кода.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormEditMain))
        Me.menuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.openToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.reloadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.saveToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.closeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.editToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.flipVerticalToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.flipHorizontalToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.undoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.redoToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.filterToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.localLaplacianToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.exposureToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.temperatureToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.programToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.aboutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.dockPanel = New WeifenLuo.WinFormsUI.Docking.DockPanel()
        Me.comboBox1 = New System.Windows.Forms.ComboBox()
        Me.label1 = New System.Windows.Forms.Label()
        Me.VisualStudioToolStripExtender1 = New WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender(Me.components)
        Me.VS2015BlueTheme1 = New WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme()
        Me.menuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'menuStrip1
        '
        Me.menuStrip1.BackColor = System.Drawing.SystemColors.Control
        Me.menuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem, Me.editToolStripMenuItem, Me.filterToolStripMenuItem, Me.programToolStripMenuItem})
        Me.menuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.menuStrip1.Name = "menuStrip1"
        Me.menuStrip1.Size = New System.Drawing.Size(1192, 24)
        Me.menuStrip1.TabIndex = 0
        Me.menuStrip1.Text = "menuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.openToolStripMenuItem, Me.reloadToolStripMenuItem, Me.saveToolStripMenuItem, Me.closeToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
        Me.fileToolStripMenuItem.Size = New System.Drawing.Size(37, 20)
        Me.fileToolStripMenuItem.Text = "File"
        '
        'openToolStripMenuItem
        '
        Me.openToolStripMenuItem.Name = "openToolStripMenuItem"
        Me.openToolStripMenuItem.Size = New System.Drawing.Size(110, 22)
        Me.openToolStripMenuItem.Text = "Open"
        '
        'reloadToolStripMenuItem
        '
        Me.reloadToolStripMenuItem.Enabled = False
        Me.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem"
        Me.reloadToolStripMenuItem.Size = New System.Drawing.Size(110, 22)
        Me.reloadToolStripMenuItem.Text = "Reload"
        '
        'saveToolStripMenuItem
        '
        Me.saveToolStripMenuItem.Enabled = False
        Me.saveToolStripMenuItem.Name = "saveToolStripMenuItem"
        Me.saveToolStripMenuItem.Size = New System.Drawing.Size(110, 22)
        Me.saveToolStripMenuItem.Text = "Save"
        '
        'closeToolStripMenuItem
        '
        Me.closeToolStripMenuItem.Enabled = False
        Me.closeToolStripMenuItem.Name = "closeToolStripMenuItem"
        Me.closeToolStripMenuItem.Size = New System.Drawing.Size(110, 22)
        Me.closeToolStripMenuItem.Text = "Close"
        '
        'editToolStripMenuItem
        '
        Me.editToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.flipVerticalToolStripMenuItem, Me.flipHorizontalToolStripMenuItem, Me.undoToolStripMenuItem, Me.redoToolStripMenuItem})
        Me.editToolStripMenuItem.Name = "editToolStripMenuItem"
        Me.editToolStripMenuItem.Size = New System.Drawing.Size(39, 20)
        Me.editToolStripMenuItem.Text = "Edit"
        '
        'flipVerticalToolStripMenuItem
        '
        Me.flipVerticalToolStripMenuItem.Enabled = False
        Me.flipVerticalToolStripMenuItem.Name = "flipVerticalToolStripMenuItem"
        Me.flipVerticalToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.flipVerticalToolStripMenuItem.Text = "Flip vertical"
        '
        'flipHorizontalToolStripMenuItem
        '
        Me.flipHorizontalToolStripMenuItem.Enabled = False
        Me.flipHorizontalToolStripMenuItem.Name = "flipHorizontalToolStripMenuItem"
        Me.flipHorizontalToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.flipHorizontalToolStripMenuItem.Text = "Flip horizontal"
        '
        'undoToolStripMenuItem
        '
        Me.undoToolStripMenuItem.Enabled = False
        Me.undoToolStripMenuItem.Name = "undoToolStripMenuItem"
        Me.undoToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.undoToolStripMenuItem.Text = "Undo"
        '
        'redoToolStripMenuItem
        '
        Me.redoToolStripMenuItem.Enabled = False
        Me.redoToolStripMenuItem.Name = "redoToolStripMenuItem"
        Me.redoToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.redoToolStripMenuItem.Text = "Redo"
        '
        'filterToolStripMenuItem
        '
        Me.filterToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.localLaplacianToolStripMenuItem, Me.exposureToolStripMenuItem, Me.temperatureToolStripMenuItem})
        Me.filterToolStripMenuItem.Name = "filterToolStripMenuItem"
        Me.filterToolStripMenuItem.Size = New System.Drawing.Size(45, 20)
        Me.filterToolStripMenuItem.Text = "Filter"
        '
        'localLaplacianToolStripMenuItem
        '
        Me.localLaplacianToolStripMenuItem.Enabled = False
        Me.localLaplacianToolStripMenuItem.Name = "localLaplacianToolStripMenuItem"
        Me.localLaplacianToolStripMenuItem.Size = New System.Drawing.Size(210, 22)
        Me.localLaplacianToolStripMenuItem.Text = "Enhancement/Details"
        '
        'exposureToolStripMenuItem
        '
        Me.exposureToolStripMenuItem.Enabled = False
        Me.exposureToolStripMenuItem.Name = "exposureToolStripMenuItem"
        Me.exposureToolStripMenuItem.Size = New System.Drawing.Size(210, 22)
        Me.exposureToolStripMenuItem.Text = "Hue/Saturation/Lightness"
        '
        'temperatureToolStripMenuItem
        '
        Me.temperatureToolStripMenuItem.Enabled = False
        Me.temperatureToolStripMenuItem.Name = "temperatureToolStripMenuItem"
        Me.temperatureToolStripMenuItem.Size = New System.Drawing.Size(210, 22)
        Me.temperatureToolStripMenuItem.Text = "Temperature"
        '
        'programToolStripMenuItem
        '
        Me.programToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.aboutToolStripMenuItem})
        Me.programToolStripMenuItem.Name = "programToolStripMenuItem"
        Me.programToolStripMenuItem.Size = New System.Drawing.Size(65, 20)
        Me.programToolStripMenuItem.Text = "Program"
        '
        'aboutToolStripMenuItem
        '
        Me.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem"
        Me.aboutToolStripMenuItem.Size = New System.Drawing.Size(107, 22)
        Me.aboutToolStripMenuItem.Text = "About"
        '
        'dockPanel
        '
        Me.dockPanel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dockPanel.DockBackColor = System.Drawing.Color.FromArgb(CType(CType(41, Byte), Integer), CType(CType(57, Byte), Integer), CType(CType(85, Byte), Integer))
        Me.dockPanel.DockBottomPortion = 150.0R
        Me.dockPanel.DockLeftPortion = 200.0R
        Me.dockPanel.DockRightPortion = 200.0R
        Me.dockPanel.DockTopPortion = 150.0R
        Me.dockPanel.Font = New System.Drawing.Font("Tahoma", 11.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, CType(0, Byte))
        Me.dockPanel.Location = New System.Drawing.Point(0, 24)
        Me.dockPanel.Name = "dockPanel"
        Me.dockPanel.RightToLeftLayout = True
        Me.dockPanel.ShowAutoHideContentOnHover = False
        Me.dockPanel.Size = New System.Drawing.Size(1192, 216)
        Me.dockPanel.TabIndex = 0
        '
        'comboBox1
        '
        Me.comboBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.comboBox1.BackColor = System.Drawing.SystemColors.Control
        Me.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.comboBox1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.comboBox1.FormattingEnabled = True
        Me.comboBox1.Location = New System.Drawing.Point(770, 2)
        Me.comboBox1.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.comboBox1.Name = "comboBox1"
        Me.comboBox1.Size = New System.Drawing.Size(126, 23)
        Me.comboBox1.TabIndex = 2
        '
        'label1
        '
        Me.label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.label1.AutoSize = True
        Me.label1.BackColor = System.Drawing.SystemColors.Control
        Me.label1.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.label1.Location = New System.Drawing.Point(698, 5)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(72, 15)
        Me.label1.TabIndex = 21
        Me.label1.Text = "Colorspace  "
        '
        'VisualStudioToolStripExtender1
        '
        Me.VisualStudioToolStripExtender1.DefaultRenderer = Nothing
        '
        'FormEditMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Control
        Me.ClientSize = New System.Drawing.Size(1192, 240)
        Me.Controls.Add(Me.dockPanel)
        Me.Controls.Add(Me.menuStrip1)
        Me.Controls.Add(Me.comboBox1)
        Me.Controls.Add(Me.label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.menuStrip1
        Me.Name = "FormEditMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Image Editor"
        Me.menuStrip1.ResumeLayout(False)
        Me.menuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

    Private WithEvents menuStrip1 As Windows.Forms.MenuStrip
    Private WithEvents fileToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents openToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents reloadToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents saveToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents filterToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents localLaplacianToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents temperatureToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents programToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents aboutToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents exposureToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents comboBox1 As Windows.Forms.ComboBox
    Private WithEvents label1 As Windows.Forms.Label
    Private WithEvents closeToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents editToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents undoToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents redoToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents flipVerticalToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Private WithEvents flipHorizontalToolStripMenuItem As Windows.Forms.ToolStripMenuItem
    Friend WithEvents VisualStudioToolStripExtender1 As WeifenLuo.WinFormsUI.Docking.VisualStudioToolStripExtender
    Friend WithEvents VS2015BlueTheme1 As WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme

    Private WithEvents dockPanel As WeifenLuo.WinFormsUI.Docking.DockPanel

End Class

