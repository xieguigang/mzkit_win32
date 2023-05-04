#Region "Microsoft.VisualBasic::82cb33f4d127560d66c1c4263033494b, mzkit\src\mzkit\mzkit\DataControlHandler.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 84
'    Code Lines: 60
' Comment Lines: 7
'   Blank Lines: 17
'     File Size: 2.91 KB


' Module DataControlHandler
' 
'     Sub: PasteTextData, SaveDataGrid, WriteTableToFile
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports Mzkit_win32.BasicMDIForm
Imports any = Microsoft.VisualBasic.Scripting
Imports REnv = SMRUCC.Rsharp.Runtime

Module DataControlHandler

    Public Sub OpenInTableViewer(matrix As DataGridView)
        Dim table As frmTableViewer = VisualStudio.ShowDocument(Of frmTableViewer)

        table.LoadTable(
            Sub(grid)
                For i As Integer = 0 To matrix.Columns.Count - 1
                    Call grid.Columns.Add(matrix.Columns(i).HeaderText, GetType(Object))
                Next

                For j As Integer = 0 To matrix.Rows.Count - 1
                    Dim rowObj = matrix.Rows(j)
                    Dim row As New List(Of Object)

                    For i As Integer = 0 To rowObj.Cells.Count - 1
                        Call row.Add(rowObj.Cells(i).Value)
                    Next

                    If row.All(Function(o) o Is Nothing OrElse IsDBNull(o)) Then
                        Continue For
                    End If

                    Call grid.Rows.Add(row.ToArray)
                Next
            End Sub)
    End Sub

    <Extension>
    Public Function getFieldVector(table As DataTable, fieldRef As String) As Array
        Dim fieldNames As New List(Of String)

        For i As Integer = 0 To table.Columns.Count - 1
            Dim tag As String = table.Columns.Item(i).ColumnName

            fieldNames.Add(tag)
        Next

        Dim ordinal As Integer = fieldNames.IndexOf(fieldRef)
        Dim vec = table.getFieldVector(ordinal)

        Return vec
    End Function

    <Extension>
    Public Function getFieldVector(table As DataTable, fieldRef As Integer) As Array
        Dim array As New List(Of Object)

        For index As Integer = 0 To table.Rows.Count - 2
            Dim row = table.Rows.Item(index)
            array.Add(row.Item(fieldRef))
        Next

        Return REnv.TryCastGenericArray(array.ToArray, MyApplication.REngine.globalEnvir)
    End Function

    ''' <summary>
    ''' found based on the <see cref="DataGridViewColumn.Name"/>
    ''' </summary>
    ''' <param name="AdvancedDataGridView1"></param>
    ''' <param name="fieldRef"></param>
    ''' <returns></returns>
    <Extension>
    Public Function getFieldVector(AdvancedDataGridView1 As DataGridView, fieldRef As String) As Array
        Dim fieldNames As New List(Of String)

        For Each col As DataGridViewColumn In AdvancedDataGridView1.Columns
            Call fieldNames.Add(col.Name)
        Next
        Dim i As Integer = fieldNames.IndexOf(fieldRef)
        Dim vec = AdvancedDataGridView1.getFieldVector(i)

        Return vec
    End Function

    <Extension>
    Public Function getFieldVector(AdvancedDataGridView1 As DataGridView, i As Integer) As Array
        Dim array As New List(Of Object)

        For Each row As DataGridViewRow In AdvancedDataGridView1.Rows
            array.Add(row.Cells(i).Value)
        Next

        Return REnv.TryCastGenericArray(array.ToArray, MyApplication.REngine.globalEnvir)
    End Function

    ''' <summary>
    ''' save data grid as excel table file
    ''' </summary>
    ''' <param name="table"></param>
    ''' <param name="title">
    ''' ``%s`` is the place holder for file name
    ''' </param>
    <Extension>
    Public Sub SaveDataGrid(table As DataGridView, title$)
        Using file As New SaveFileDialog With {
            .Filter = "Excel Table(*.xls)|*.xls|Comma data sheet(*.csv)|*.csv",
            .Title = $"Save Table File({title})"
        }
            If file.ShowDialog = DialogResult.OK Then
                Using writeTsv As StreamWriter = file.FileName.OpenWriter(encoding:=Encodings.GB2312)
                    Call table.WriteTableToFile(writeTsv, sep:=If(file.FileName.ExtensionSuffix("csv"), ","c, ASCII.TAB))
                    Call MessageBox.Show(title.Replace("%s", file.FileName), "Export Table", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            End If
        End Using
    End Sub

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
                    Call row.Add(any.ToString(rowObj.Cells.Item(i)))
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
