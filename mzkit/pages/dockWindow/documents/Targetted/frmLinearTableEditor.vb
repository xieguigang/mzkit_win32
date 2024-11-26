Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Windows.Forms.DataValidation.UIInteractive
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports any = Microsoft.VisualBasic.Scripting

Public Class frmLinearTableEditor : Implements IFileReference, DocumentPageLoader

    Dim is_list As String()
    Public Property FilePath As String Implements IFileReference.FilePath
    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
    Public Property AutoSaveOnClose As Boolean Implements DocumentPageLoader.AutoSaveOnClose

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Dim editList As New InputIdList With {.IdSet = is_list}
        InputDialog.Input(Sub(editor) Call SetIdList(editor.IdSet), config:=editList)
    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = System.Windows.Forms.Keys.V AndAlso e.Control AndAlso Clipboard.ContainsText Then
            Call DataGridView1.PasteTextData()
        End If
    End Sub

    Private Sub SetIdList(ids As IEnumerable(Of String))
        is_list = ids.SafeQuery.Where(Function(s) Strings.Len(s) > 0).ToArray

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            Dim row = DataGridView1.Rows(i)

            If row Is Nothing Then
                Continue For
            End If

            Dim combo As DataGridViewComboBoxCell = row.Cells(1)

            Call combo.Items.Clear()

            For Each id As String In is_list
                Call combo.Items.Add(id)
            Next
        Next

        Call DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        If DataGridView1.Columns.Count > 2 Then
            If MessageBox.Show("The last column for the reference point will be removed from this linear table?",
                               "Delete data",
                               MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then

                DataGridView1.Columns.RemoveAt(DataGridView1.Columns.Count - 1)
            End If
        Else
            Call Workbench.Warning("no more reference point column could be removes.")
        End If
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Dim n = DataGridView1.Columns.Count
        Dim level = n - 2 + 1

        DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.Name = $"L{level}", .HeaderText = .Name})
    End Sub

    Private Sub DataGridView1_RowsAdded(sender As Object, e As DataGridViewRowsAddedEventArgs) Handles DataGridView1.RowsAdded
        Dim row = DataGridView1.Rows(e.RowIndex)
        Dim combo As DataGridViewComboBoxCell = row.Cells(1)

        Call combo.Items.Clear()

        For Each id As String In is_list.SafeQuery
            Call combo.Items.Add(id)
        Next
    End Sub

    Public Sub LoadDocument(file As String) Implements DocumentPageLoader.LoadDocument
        Dim list = file.LoadCsv(Of Standards)()

        is_list = list _
            .Select(Function(a) a.IS) _
            .Where(Function(sid) Strings.Len(sid) > 0) _
            .Distinct _
            .ToArray

        DataGridView1.Rows.Clear()

        If DataGridView1.Columns.Count > 2 Then
            For i As Integer = DataGridView1.Columns.Count - 1 To 1 Step -1
                DataGridView1.Columns.RemoveAt(i)
            Next
        End If

        If list.Count = 0 Then
            Return
        End If

        Dim maxLevels = list.Select(Function(a) a.C.Count).Max

        For Each compound As Standards In list
            Dim offset As Integer = DataGridView1.Rows.Add()
            Dim row As DataGridViewRow = DataGridView1.Rows(offset)
            Dim comboBoxColumn = New DataGridViewComboBoxCell()

            row.Cells.Add(New DataGridViewTextBoxCell())
            row.Cells.Add(comboBoxColumn)

            row.Cells(0).Value = compound.ID

            For Each id As String In is_list
                Call comboBoxColumn.Items.Add(id)
            Next

            comboBoxColumn.Value = compound.IS

            offset = 2

            For Each level As Double In compound.PopulateLevels
                row.Cells.Add(New DataGridViewTextBoxCell)
                row.Cells(offset).Value = level

                offset += 1
            Next
        Next

        Call DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
    End Sub

    Protected Overrides Sub SaveDocument()
        If FilePath.StringEmpty Then
            Using file As New SaveFileDialog With {.Filter = "Linear Table(*.csv)|*.csv"}
                If file.ShowDialog = DialogResult.OK Then
                    FilePath = file.FileName
                    SaveFile()
                End If
            End Using
        Else
            Call SaveFile()
        End If
    End Sub

    Private Sub SaveFile()
        Dim standards As New List(Of Standards)
        Dim ncols = DataGridView1.Columns.Count

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            Dim row As DataGridViewRow = DataGridView1.Rows(i)

            If row Is Nothing OrElse row.Cells(0).Value Is Nothing Then
                Continue For
            End If

            Dim id = row.Cells(0).Value
            Dim istd = row.Cells(1).Value
            Dim lv As Integer = 1
            Dim c As New Dictionary(Of String, Double)
            Dim val As Double

            For j As Integer = 2 To ncols - 1
                val = CDbl(row.Cells(j).Value)
                c.Add("L" & lv, val)
                lv += 1
            Next

            Call standards.Add(New Standards With {
                .ID = any.ToString(id),
                .Name = .ID,
                .[IS] = any.ToString(istd),
                .ISTD = .IS,
                .C = c,
                .Factor = 1
            })
        Next

        Call standards.SaveTo(FilePath)
    End Sub

    Private Sub frmLinearTableEditor_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If AutoSaveOnClose Then
            Call SaveDocument()
        End If
    End Sub
End Class