Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.Text

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
            Dim file As String = $"{App.ProductProgramData}/{ComboBox1.SelectedItem}.txt"
            Dim logs As LogEntry() = LogReader.Parse(file).ToArray

            DataGridView1.Rows.Clear()

            For Each item As LogEntry In logs.OrderByDescending(Function(i) i.time)
                Dim row As New DataGridViewRow With {.Tag = item}
                Dim data = TryParse(item.message)

                row.Cells.Add(New DataGridViewTextBoxCell With {.Value = item.time})
                row.Cells.Add(New DataGridViewTextBoxCell With {.Value = data.Item1})
                row.Cells.Add(New DataGridViewTextBoxCell With {.Value = data.Item2})
                row.Cells.Add(New DataGridViewLinkCell With {.Value = "Run"})

                DataGridView1.Rows.Add(row)
            Next
        End If
    End Sub

    Private Shared Function TryParse(log As String) As (String, String)
        Dim lines As String() = Strings.Trim(log) _
            .LineTokens _
            .Where(Function(si) Not si.StartsWith("//")) _
            .Where(Function(si) Not Strings.Trim(si).StringEmpty(, True)) _
            .ToArray
        Dim cd As String = lines(lines.Length - 2)
        Dim cmd As String = lines(lines.Length - 1)

        cd = cd.GetTagValue(" ").Value.Trim(""""c)

        Return (cd, cmd)
    End Function

    Private Sub launch_cmd(cmdlog As LogEntry)
        Dim run = TryParse(cmdlog.message)
        Dim batch As New StringBuilder($"{run.Item1.Split(":"c).First}:" & vbCrLf & vbCrLf)
        batch.AppendLine("CD " & run.Item1.CLIPath)
        batch.AppendLine(run.Item2)

        Dim batch_file As String = App.GetTempFile & ".cmd"
        Dim cmd As New Process
        cmd.StartInfo.FileName = "cmd.exe"
        cmd.StartInfo.Arguments = "/k " & batch_file.CLIPath
        cmd.StartInfo.CreateNoWindow = False

        Call batch.ToString.SaveTo(batch_file, Encodings.UTF8WithoutBOM.CodePage)
        Call cmd.Start()
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If DataGridView1.Rows.Count = 0 Then
            Return
        End If
        If e.RowIndex < 0 OrElse e.ColumnIndex < 0 Then
            Return
        End If
        If e.ColumnIndex = 3 Then
            Dim log As LogEntry = DataGridView1.Rows(e.RowIndex)?.Tag

            If Not log.message.StringEmpty Then
                Call launch_cmd(log)
            End If
        End If
    End Sub
End Class