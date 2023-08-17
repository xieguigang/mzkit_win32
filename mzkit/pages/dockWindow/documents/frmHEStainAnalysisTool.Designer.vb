Imports Mzkit_win32.BasicMDIForm

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmHEStainAnalysisTool
    Inherits DocumentWindow

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
        Me.HeStainViewer1 = New Global.Mzkit_win32.MSImagingViewerV2.HEStainViewer()
        Me.SuspendLayout()
        '
        'HeStainViewer1
        '
        Me.HeStainViewer1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.HeStainViewer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.HeStainViewer1.Location = New System.Drawing.Point(0, 0)
        Me.HeStainViewer1.Name = "HeStainViewer1"
        Me.HeStainViewer1.Size = New System.Drawing.Size(800, 450)
        Me.HeStainViewer1.TabIndex = 0
        '
        'frmHEStainAnalysisTool
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.HeStainViewer1)
        Me.Name = "frmHEStainAnalysisTool"
        Me.Text = "HEStain Analysis Tool"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents HeStainViewer1 As Global.Mzkit_win32.MSImagingViewerV2.HEStainViewer
End Class
