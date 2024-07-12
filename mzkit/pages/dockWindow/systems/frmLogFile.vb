Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging

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
            Dim logs As LogEntry() = LogReader.Parse(file).ToArray

            DataGridView1.Rows.Clear()

            For Each item As LogEntry In logs
                Dim row As New DataGridViewRow With {.Tag = item}
                Dim data = TryParse(item.message)

                row.Cells.Add(New DataGridViewTextBoxCell With {.Value = item.time})
                row.Cells.Add(New DataGridViewTextBoxCell With {.Value = data.Item1})
                row.Cells.Add(New DataGridViewTextBoxCell With {.Value = data.Item2})

                DataGridView1.Rows.Add(row)
            Next
        End If
    End Sub

    Private Shared Function TryParse(log As String) As (String, String)
        Dim lines As String() = Strings.Trim(log).LineTokens
        Dim cd As String = lines(lines.Length - 2)
        Dim cmd As String = lines(lines.Length - 1)

        cd = cd.GetTagValue(" ").Value

        Return (cd, cmd)
    End Function
End Class