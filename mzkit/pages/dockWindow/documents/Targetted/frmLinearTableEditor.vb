﻿Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Windows.Forms.DataValidation.UIInteractive
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports any = Microsoft.VisualBasic.Scripting
Imports xlsx = Microsoft.VisualBasic.MIME.Office.Excel.XLSX.File

' 20250630 dot not set the datagridview selection mode to
' full row select, should be use the default selection mode
' row header select, or the paste data function will not working 
' well

Public Class frmLinearTableEditor : Implements IFileReference, DocumentPageLoader

    Dim is_list As String()
    Public Property FilePath As String Implements IFileReference.FilePath
    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
    Public Property AutoSaveOnClose As Boolean Implements DocumentPageLoader.AutoSaveOnClose

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Dim editList As New InputIdList With {.IdSet = is_list}
        InputDialog.Input(Sub(editor) Call SetIdList(editor.IdSet), config:=editList)
    End Sub

    Private Shared Sub loadComboList(combo As DataGridViewComboBoxCell, is_list As IEnumerable(Of String))
        Call combo.Items.Clear()
        Call combo.Items.Add("None")

        For Each id As String In is_list.SafeQuery
            Call combo.Items.Add(id)
        Next
    End Sub

    Private Sub loadComboList(combo As DataGridViewComboBoxCell)
        Call loadComboList(combo, is_list)
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
            Dim id_row As String = any.ToString(row.Cells(0).Value)

            Call loadComboList(combo)
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

        Call loadComboList(combo)
    End Sub

    Public Function LoadDocument(file As String) As Boolean Implements DocumentPageLoader.LoadDocument
        Dim list As Standards()

        Try
            If file.ExtensionSuffix("csv") Then
                list = file.LoadCsv(Of Standards)()
            Else
                ' load xlsx
                list = xlsx.Open(file).LoadDataSet(Of Standards)(0)
            End If
        Catch ex As Exception
            Return False
        End Try

        If list.IsNullOrEmpty OrElse list.All(Function(s) s.Name.StringEmpty) Then
            Return False
        End If

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
            Return False
        Else
            FilePath = file
        End If

        Call LoadStandardsToTable(DataGridView1, list, is_list)

        Return True
    End Function

    Public Shared Sub LoadStandardsToTable(DataGridView1 As DataGridView, list As Standards(), is_list As String())
        Dim maxLevels = Standards.GetLevelKeys(list).ToArray

        For Each i As String In maxLevels
            Call DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.Name = i, .HeaderText = .Name})
        Next

        For Each compound As Standards In list
            Dim offset As Integer = DataGridView1.Rows.Add()
            Dim row As DataGridViewRow = DataGridView1.Rows(offset)
            Dim comboBoxColumn As DataGridViewComboBoxCell = row.Cells(1)

            ' row.Cells.Add(New DataGridViewTextBoxCell())
            ' row.Cells.Add(comboBoxColumn)

            row.Cells(0).Value = compound.ID

            Call loadComboList(comboBoxColumn, is_list)

            If compound.IS.StringEmpty Then
                comboBoxColumn.Value = "None"
            Else
                comboBoxColumn.Value = compound.IS
            End If

            offset = 2

            For Each level As Double In compound.PopulateLevels
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

            If row Is Nothing OrElse any.ToString(row.Cells(0).Value).StringEmpty Then
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

            If istd = "None" Then
                istd = Nothing
            End If

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

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        Dim selectedRows = DataGridView1.SelectedRows

        If selectedRows.Count = 0 Then
            Call Workbench.Warning("No row data was selected for removes.")
            Return
        End If

        Dim row = selectedRows(0)

        If MessageBox.Show("Going to delete this compound standard curve data?", "Remove data row", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.OK Then
            Try
                DataGridView1.Rows.Remove(row)
            Catch ex As Exception

            End Try
        End If
    End Sub
End Class