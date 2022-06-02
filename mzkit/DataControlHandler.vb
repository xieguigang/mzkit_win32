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
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports any = Microsoft.VisualBasic.Scripting

Module DataControlHandler

    ''' <summary>
    ''' save data grid as excel table file
    ''' </summary>
    ''' <param name="table"></param>
    ''' <param name="title">
    ''' ``%s`` is the place holder for file name
    ''' </param>
    <Extension>
    Public Sub SaveDataGrid(table As DataGridView, title$)
        Using file As New SaveFileDialog With {.Filter = "Excel Table(*.xls)|*.xls|Comma data sheet(*.csv)|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                Using writeTsv As StreamWriter = file.FileName.OpenWriter(encoding:=Encodings.UTF8)
                    Call table.WriteTableToFile(writeTsv, sep:=If(file.FileName.ExtensionSuffix("csv"), ","c, ASCII.TAB))
                    Call MessageBox.Show(title.Replace("%s", file.FileName), "Export Table", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Using
            End If
        End Using
    End Sub

    ''' <summary>
    ''' write table in tsv file format
    ''' </summary>
    ''' <param name="table"></param>
    ''' <param name="writeTsv"></param>
    <Extension>
    Public Sub WriteTableToFile(table As DataGridView, writeTsv As TextWriter,
                                Optional saveHeader As Boolean = True,
                                Optional sep As Char = ASCII.TAB)

        Dim row As New List(Of String)

        If saveHeader Then
            For i As Integer = 0 To table.Columns.Count - 1
                Call row.Add(table.Columns(i).HeaderText)
            Next

            Call writeTsv.WriteLine(row.PopAll.JoinBy(sep))
        End If

        For j As Integer = 0 To table.Rows.Count - 1
            Dim rowObj = table.Rows(j)

            For i As Integer = 0 To rowObj.Cells.Count - 1
                Call row.Add(any.ToString(rowObj.Cells(i).Value))
            Next

            Call writeTsv.WriteLine(row.PopAll.JoinBy(sep))
        Next

        Call writeTsv.Flush()
    End Sub

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
