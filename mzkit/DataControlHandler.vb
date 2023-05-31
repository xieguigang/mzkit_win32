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
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports Mzkit_win32.BasicMDIForm
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

End Module
