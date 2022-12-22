Imports System.ComponentModel

Public Class InputMSISlideLayout

    Public Property layoutData As String
        Get
            Return TextBox1.Text
        End Get
        Set(value As String)
            TextBox1.Text = value
        End Set
    End Property

    <DefaultValue(True)>
    Public Property useFileNameAsSourceTag As Boolean
        Get
            Return CheckBox1.Checked
        End Get
        Set(value As Boolean)
            CheckBox1.Checked = value
        End Set
    End Property

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub
End Class