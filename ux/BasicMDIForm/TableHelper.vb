Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports any = Microsoft.VisualBasic.Scripting

Public Module TableHelper

    ''' <summary>
    ''' write table in tsv file format
    ''' </summary>
    ''' <param name="table2"></param>
    ''' <param name="writeTsv">
    ''' target file or the <see cref="StringBuilder"/> for copy to clipboard
    ''' </param>
    <Extension>
    Public Sub WriteTableToFile(table2 As DataGridView, writeTsv As TextWriter,
                                Optional saveHeader As Boolean = True,
                                Optional sep As Char = ASCII.TAB)

        Dim row As New List(Of String)
        Dim src As BindingSource = table2.DataSource

        If saveHeader Then
            For i As Integer = 0 To table2.Columns.Count - 1
                Call row.Add(table2.Columns(i).HeaderText)
            Next

            Call writeTsv.WriteLine(row.PopAll.JoinBy(sep))
        End If

        If src Is Nothing Then
            For j As Integer = 0 To table2.Rows.Count - 1
                Dim rowObj As DataGridViewRow = table2.Rows(j)

                For i As Integer = 0 To rowObj.Cells.Count - 1
                    Call row.Add(any.ToString(rowObj.Cells.Item(i).Value))
                Next

                Call writeTsv.WriteLine(row.PopAll.JoinBy(sep))
            Next
        Else
            Dim ds As DataSet = src.DataSource
            Dim table As DataTable = ds.Tables.Item(src.DataMember)

            For j As Integer = 0 To table.Rows.Count - 1
                Dim rowObj As DataRow = table.Rows(j)

                Try
                    Call row.AddRange(rowObj.ItemArray.Select(AddressOf any.ToString))
                    Call writeTsv.WriteLine(row.PopAll.JoinBy(sep))
                Catch ex As Exception
                    Call Workbench.Warning(ex.ToString)
                End Try
            Next
        End If

        Call writeTsv.Flush()
    End Sub

    ''' <summary>
    ''' paste the text data to the table control
    ''' </summary>
    ''' <param name="table"></param>
    <Extension>
    Public Sub PasteTextData(table As DataGridView)
        Dim text As String = Strings.Trim(Clipboard.GetText).Trim(ASCII.CR, ASCII.LF, ASCII.TAB)

        If table.SelectedCells.Count = 0 Then
            Return
        End If

        Dim i As Integer = table.SelectedCells.Item(Scan0).RowIndex
        Dim j As Integer = table.SelectedCells.Item(Scan0).ColumnIndex

        If text.Contains(vbCr) OrElse text.Contains(vbLf) Then
            Dim colCells As String() = text.LineTokens

            If i + colCells.Length >= table.Rows.Count Then
                Dim n As Integer = table.Rows.Count

                For rid As Integer = 0 To (colCells.Length + i) - n
                    table.Rows.Add()
                Next
            End If

            For ii As Integer = 0 To colCells.Length - 1
                table.Rows(ii + i).Cells(j).Value = colCells(ii)
            Next
        Else
            Dim rowCells As String() = text.Split(ASCII.TAB)

            For ci As Integer = 0 To rowCells.Length - 1
                table.Rows(i).Cells(j + ci).Value = rowCells(ci)
            Next
        End If
    End Sub
End Module
