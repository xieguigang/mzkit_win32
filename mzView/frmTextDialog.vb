Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices

Public Class frmTextDialog

    Dim htmlViewer As New HtmlViewer
    Dim tmp As String

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        Controls.Add(htmlViewer)
    End Sub

    Public Sub ShowText(text As String)
        Dim content = text.GetTagValue(":")

        tmp = TempFileSystem.GetAppSysTempFile(".html")
        TextBox1.Text = content.Value
        content.Value.SaveTo(tmp, Encoding.UTF8)

        Me.Text = "View Text Of: " & content.Name
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex > -1 Then
            If ComboBox1.SelectedIndex = 0 Then
                ' text
                htmlViewer.Visible = False
                htmlViewer.Dock = DockStyle.None
                TextBox1.Visible = True
                TextBox1.Dock = DockStyle.Fill
            Else
                ' html
                TextBox1.Visible = False
                TextBox1.Dock = DockStyle.None
                htmlViewer.Visible = True
                htmlViewer.Dock = DockStyle.Fill
                htmlViewer.LoadPage(tmp)
            End If
        End If
    End Sub
End Class