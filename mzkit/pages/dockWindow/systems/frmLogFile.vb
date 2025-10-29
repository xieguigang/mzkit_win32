Imports Galaxy.Workbench
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Mzkit_win32.BasicMDIForm

Public Class frmLogFile

    Protected Overrides Sub OpenContainingFolder()
        Call RibbonEvents.openAppData()
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

        Call ApplyVsTheme(ContextMenuStrip1)
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged() Handles ComboBox1.SelectedIndexChanged
        If ComboBox1.SelectedIndex > -1 Then
            Dim file As String = $"{App.ProductProgramData}/{ComboBox1.SelectedItem}.txt"
            Dim logs As LogEntry() = LogReader.Parse(file).ToArray

            DataGridView1.Rows.Clear()

            For Each item As LogEntry In logs.OrderByDescending(Function(i) i.time)
                Dim row As New DataGridViewRow With {.Tag = item}
                Dim data = WorkStudio.TryParse(item.message)

                row.Cells.Add(New DataGridViewTextBoxCell With {.Value = item.time})
                row.Cells.Add(New DataGridViewTextBoxCell With {.Value = data.Item1})
                row.Cells.Add(New DataGridViewTextBoxCell With {.Value = data.Item2})
                row.Cells.Add(New DataGridViewLinkCell With {.Value = "Run"})

                DataGridView1.Rows.Add(row)
            Next
        End If
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
                Call WorkStudio.launch_cmd(log)
            End If
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Call ComboBox1_SelectedIndexChanged()
    End Sub

    Private Sub CopyCommandLineArgumentsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyCommandLineArgumentsToolStripMenuItem.Click
        If DataGridView1.Rows.Count = 0 Then
            Return
        End If
        If DataGridView1.SelectedRows.Count = 0 Then
            Return
        End If

        Dim row = DataGridView1.SelectedRows(0)
        Dim cli As LogEntry = row.Tag

        If cli.message.StringEmpty(, True) Then
            Call Workbench.Warning("no commandline argument to copy!")
        Else
            Dim run = WorkStudio.TryParse(cli.message)

            Clipboard.Clear()
            Clipboard.SetText(run.cmd)
        End If
    End Sub
End Class