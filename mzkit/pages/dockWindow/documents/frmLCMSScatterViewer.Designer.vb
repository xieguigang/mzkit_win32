Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.LCMSViewer

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmLCMSScatterViewer
    Inherits DocumentWindow

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'frmLCMSScatterViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.DoubleBuffered = True
        Me.Name = "frmLCMSScatterViewer"
        Me.TabPageContextMenuStrip = Me.DockContextMenuStrip1
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ScatterViewer As PeakScatterViewer

End Class
