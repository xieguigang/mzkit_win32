Imports System.IO
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text

Public Module DataTableViewer

    Private openTable As Func(Of IDataTableViewer)

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub HookTableViewer(open As Func(Of IDataTableViewer))
        openTable = open
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Sub OpenInTableViewer(matrix As DataGridView)
        Call openTable().LoadTable(AddressOf New GridLoader(matrix).LoadTable)
    End Sub

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

Friend Class GridLoader

    ReadOnly matrix As DataGridView

    Sub New(matrix As DataGridView)
        Me.matrix = matrix
    End Sub

    Public Sub LoadTable(grid As DataTable)
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
    End Sub

End Class

Public Interface IDataTableViewer

    Sub LoadTable(apply As Action(Of DataTable))

End Interface