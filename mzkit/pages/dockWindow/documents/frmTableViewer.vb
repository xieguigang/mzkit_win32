#Region "Microsoft.VisualBasic::09e529c89b620cda995388ea15f96330, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmTableViewer.vb"

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

    '   Total Lines: 274
    '    Code Lines: 198
    ' Comment Lines: 8
    '   Blank Lines: 68
    '     File Size: 10.44 KB


    ' Class frmTableViewer
    ' 
    '     Properties: FilePath, MimeType, ViewRow
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: getCurrentTable, (+2 Overloads) getFieldVector, GetSchema, (+2 Overloads) Save
    ' 
    '     Sub: ActionsToolStripMenuItem_Click, AdvancedDataGridView1_FilterStringChanged, AdvancedDataGridViewSearchToolBar1_Search, columnVectorStat, exportTableCDF
    '          frmTableViewer_Activated, frmTableViewer_FormClosed, frmTableViewer_FormClosing, frmTableViewer_Load, LoadTable
    '          resetFilter, SaveDocument, SendToREnvironmentToolStripMenuItem_Click, ViewToolStripMenuItem_Click, VisualizeToolStripMenuItem_Click
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Text
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports Zuby.ADGV

Public Class frmTableViewer : Implements ISaveHandle, IFileReference, IDataTraceback

    Public Property FilePath As String Implements IFileReference.FilePath
    Public Property ViewRow As Action(Of Dictionary(Of String, Object))

    Public Property SourceName As String Implements IDataTraceback.SourceName
    ''' <summary>
    ''' for raw data traceback
    ''' </summary>
    ''' <returns></returns>
    Public Property InstanceGuid As String Implements IDataTraceback.InstanceGuid

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {.Details = "Excel Data Table", .FileExt = ".xls", .MIMEType = "application/xls", .Name = "Excel Data Table"}
            }
        End Get
    End Property

    Public Property AppSource As Type Implements IDataTraceback.AppSource

    Dim memoryData As New DataSet
    Dim search As GridSearchHandler

    Public Sub LoadTable(apply As Action(Of DataTable))
        Dim table As DataTable = memoryData.Tables.Add("memoryData")

        Call Me.AdvancedDataGridView1.Columns.Clear()
        Call Me.AdvancedDataGridView1.Rows.Clear()
        Call apply(table)
        Call AdvancedDataGridView1.SetDoubleBuffered()

        For Each column As DataGridViewColumn In AdvancedDataGridView1.Columns
            'Select Case table.Columns.Item(column.HeaderText).DataType
            '    Case GetType(String)
            '        AdvancedDataGridView1.SetSortEnabled(column, True)
            '    Case GetType(Double)
            '    Case GetType(Integer)
            '    Case Else
            '        ' do nothing 
            'End Select

            AdvancedDataGridView1.ShowMenuStrip(column)
        Next

        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName

        AdvancedDataGridView1.DataSource = BindingSource1
        AdvancedDataGridViewSearchToolBar1.SetColumns(AdvancedDataGridView1.Columns)
    End Sub

    Protected Overrides Sub SaveDocument()
        Call AdvancedDataGridView1.SaveDataGrid("Save Table View")
    End Sub

    Private Sub frmTableViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        CopyFullPathToolStripMenuItem.Enabled = False
        OpenContainingFolderToolStripMenuItem.Enabled = False

        search = New GridSearchHandler(AdvancedDataGridView1)
        TabText = "Table View"

        AddHandler AdvancedDataGridViewSearchToolBar1.Search, AddressOf search.AdvancedDataGridViewSearchToolBar1_Search

        ApplyVsTheme(ContextMenuStrip1)
    End Sub

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Using writeTsv As StreamWriter = path.OpenWriter(encoding:=Encodings.UTF8WithoutBOM)
            Call AdvancedDataGridView1.WriteTableToFile(writeTsv)
        End Using

        Return True
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function

    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click
        If AdvancedDataGridView1.SelectedRows.Count <= 0 Then
            Call MyApplication.host.showStatusMessage("Please select a row data for view content!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        ElseIf Not ViewRow Is Nothing Then
            Dim obj As New Dictionary(Of String, Object)
            Dim row As DataGridViewRow = AdvancedDataGridView1.SelectedRows(0)

            For i As Integer = 0 To AdvancedDataGridView1.Columns.Count - 1
                obj(AdvancedDataGridView1.Columns(i).HeaderText) = row.Cells(i).Value
            Next

            Call _ViewRow(obj)
        End If
    End Sub

    Private Sub SendToREnvironmentToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SendToREnvironmentToolStripMenuItem.Click
        Dim form As New InputRSymbol
        Dim fieldNames As New List(Of String)

        For Each col As DataGridViewColumn In AdvancedDataGridView1.Columns
            Call fieldNames.Add(col.Name)
        Next

        Call form.LoadFields(fieldNames)

        Call InputDialog.Input(Of InputRSymbol)(
            Sub(config)
                Dim name As String = config.ComboBox1.Text.Trim
                Dim fields As String() = config.GetNames.ToArray
                Dim table As New dataframe With {
                    .columns = New Dictionary(Of String, Array)
                }

                For Each fieldRef As String In fields
                    Dim i As Integer = fieldNames.IndexOf(fieldRef)
                    Dim array As Array = AdvancedDataGridView1.getFieldVector(i)

                    Call table.add(fieldRef, array)
                Next

                Call MyApplication.REngine.Add(name, table)
                Call VisualStudio.ShowRTerm()
            End Sub, config:=form)
    End Sub

    Public Function GetSchema() As Dictionary(Of String, Type)
        Dim schema As New Dictionary(Of String, Type)

        For Each col As DataGridViewColumn In AdvancedDataGridView1.Columns
            Call schema.Add(col.Name, GetType(Double))
        Next

        Return schema
    End Function

    Private Sub VisualizeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles VisualizeToolStripMenuItem.Click
        Dim load As New InputDataVisual

        Call load.SetAxis(GetSchema)
        Call InputDialog.Input(
            Sub(creator)
                Dim binding As BindingSource = AdvancedDataGridView1.DataSource
                Dim tableSet As DataSet = binding.DataSource
                Dim table As DataTable = tableSet.Tables.Item(Scan0)

                Call creator.DoPlot(
                    x:=AdvancedDataGridView1.getFieldVector(creator.GetX),
                    getVector:=AddressOf AdvancedDataGridView1.getFieldVector,
                    table:=table
                )
            End Sub, config:=load)
    End Sub

    Public Shared ReadOnly TableGuid As New Dictionary(Of DataTable, String)

    Private Sub ActionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ActionsToolStripMenuItem.Click
        Dim takeActions As New InputAction

        Call takeActions.SetFields(GetSchema.Keys)
        Call InputDialog.Input(Sub(input)
                                   Dim name As String = input.getTargetName
                                   Dim action As String = input.getActionName
                                   Dim data As Array = AdvancedDataGridView1.getFieldVector(name)
                                   Dim source As BindingSource = AdvancedDataGridView1.DataSource
                                   Dim dataset As DataSet = source.DataSource
                                   Dim table As DataTable = dataset.Tables.Item(Scan0)

                                   table.Namespace = SourceName

                                   SyncLock TableGuid
                                       TableGuid(table) = InstanceGuid
                                   End SyncLock

                                   Actions.RunAction(action, name, data, table)
                               End Sub, config:=takeActions)
    End Sub

    Private Sub AdvancedDataGridView1_FilterStringChanged(sender As Object, e As AdvancedDataGridView.FilterEventArgs) Handles AdvancedDataGridView1.FilterStringChanged

    End Sub

    Private Sub frmTableViewer_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed

    End Sub

    Private Sub frmTableViewer_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

    End Sub

    Private Sub frmTableViewer_Activated(sender As Object, e As EventArgs) Handles Me.Activated

    End Sub

    Shared Sub New()
        AddHandler ribbonItems.ButtonResetTableFilter.ExecuteEvent,
            Sub()
                Dim table = getCurrentTable()

                If Not table Is Nothing Then
                    Call table.resetFilter()
                End If
            End Sub

        AddHandler ribbonItems.ButtonColumnStats.ExecuteEvent,
            Sub()
                Dim table = getCurrentTable()

                If Not table Is Nothing Then
                    Call table.columnVectorStat()
                End If
            End Sub

        AddHandler ribbonItems.ButtonSaveTableCDF.ExecuteEvent,
            Sub()
                Dim table = getCurrentTable()

                If Not table Is Nothing Then
                    Call table.exportTableCDF()
                End If
            End Sub
    End Sub

    Private Sub exportTableCDF()

    End Sub

    Private Sub columnVectorStat()

    End Sub

    Private Sub resetFilter()
        Call AdvancedDataGridView1.CleanFilterAndSort()
    End Sub

    Private Shared Function getCurrentTable() As frmTableViewer
        If TypeOf MyApplication.host.dockPanel.ActiveDocument Is frmTableViewer Then
            Return MyApplication.host.dockPanel.ActiveDocument
        Else
            Return Nothing
        End If
    End Function

    Private Sub ExportTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportTableToolStripMenuItem.Click
        Call Me.SaveDocument()
    End Sub

    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
        Dim buffer As New StringBuilder

        Using writer As New StringWriter(buffer)
            Call AdvancedDataGridView1.WriteTableToFile(writer, saveHeader:=False)
        End Using

        Call Clipboard.Clear()
        Call Clipboard.SetText(buffer.ToString)
    End Sub

    Private Sub frmTableViewer_Closed(sender As Object, e As EventArgs) Handles Me.Closed

    End Sub
End Class
