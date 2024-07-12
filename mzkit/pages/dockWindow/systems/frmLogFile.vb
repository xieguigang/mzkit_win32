Public Class frmLogFile

    Protected Overrides Sub OpenContainingFolder()
        Call Process.Start("explorer.exe", App.ProductProgramData)
    End Sub

    Protected Overrides Sub CopyFullPath()
        Call Clipboard.Clear()
        Call Clipboard.SetText(App.ProductProgramData)
    End Sub

    Private Sub frmLogFile_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim logfiles As String() = App.ProductProgramData.ListFiles("*.txt").ToArray

        For Each file As String In logfiles
            Call ComboBox1.Items.Add(file.BaseName)
        Next
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex > -1 Then
            Dim file As String = $"{App.ProductProgramData}/{ComboBox1.SelectedValue}.txt"
            Dim logs
        End If
    End Sub
End Class