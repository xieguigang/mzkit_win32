Imports msconvertGUI.My

Public Class FormMain

    ''' <summary>
    ''' start run task
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If TextBox1.Text.StringEmpty OrElse Not TextBox1.Text.DirectoryExists Then
            MessageBox.Show("Invalid source folder path!", "Source Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return
        ElseIf TextBox2.Text.StringEmpty Then
            MessageBox.Show("No output folder path is specificed!", "Target Error", MessageBoxButtons.OK, MessageBoxIcon.Stop)
            Return
        End If

        Dim source As String = TextBox1.Text
        Dim output As String = TextBox2.Text

        Call MyApplication.SubmitTask(source, output, Me).Start()
    End Sub

    Public Sub AddTask(task As TaskProgress)
        Call Me.Invoke(
            Sub()
                Call FlowLayoutPanel1.Controls.Add(task)
            End Sub)
    End Sub

    ''' <summary>
    ''' open source folder
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using folder As New FolderBrowserDialog
            If folder.ShowDialog = DialogResult.OK Then
                TextBox1.Text = folder.SelectedPath

                If TextBox2.Text.StringEmpty Then
                    TextBox2.Text = TextBox1.Text
                End If
            End If
        End Using
    End Sub

    ''' <summary>
    ''' open save folder
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Using folder As New FolderBrowserDialog
            If folder.ShowDialog = DialogResult.OK Then
                TextBox2.Text = folder.SelectedPath
            End If
        End Using
    End Sub
End Class
