Imports System
Imports System.Collections.Generic
Imports System.Windows.Forms
Imports Galaxy.Workbench.CommonDialogs
Imports Galaxy.ExcelPad
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports BioNovoGene.mzkit_win32.My
Imports Mzkit_win32.BasicMDIForm
Imports Galaxy.Data

Public Class ExcelTableViewer

    ReadOnly excel As FormExcelPad

    Friend WithEvents MSImagingIonListToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SendToREnvironmentToolStripMenuItem As ToolStripMenuItem

    Sub New(excel As FormExcelPad)
        Me.excel = excel
        Me.MSImagingIonListToolStripMenuItem = New ToolStripMenuItem
        Me.SendToREnvironmentToolStripMenuItem = New ToolStripMenuItem

        AddHandler ribbonItems.ButtonColumnStats.ExecuteEvent,
    Sub()
        Call DoTableSampleStats()
    End Sub
    End Sub

    Shared Sub New()
        AddHandler ribbonItems.ButtonResetTableFilter.ExecuteEvent,
            Sub()
                Dim table = formexcelpad.getCurrentTable()

                If Not table Is Nothing Then
                    Call table.resetFilter()
                End If
            End Sub

        AddHandler ribbonItems.ButtonColumnStats.ExecuteEvent,
            Sub()
                Dim table = formexcelpad.getCurrentTable()

                If Not table Is Nothing Then
                    Call table.columnVectorStat()
                End If
            End Sub

        AddHandler ribbonItems.ButtonSaveTableCDF.ExecuteEvent,
            Sub()
                Dim table = formexcelpad.getCurrentTable()

                If Not table Is Nothing Then
                    Call table.exportTableCDF()
                End If
            End Sub
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

    ''' <summary>
    ''' send to ms-imaging ion list
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Public Sub MSImagingIonListToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MSImagingIonListToolStripMenuItem.Click
        Dim msi As frmMsImagingViewer = Workbench.AppHost.GetDocuments.Where(Function(f) TypeOf f Is frmMsImagingViewer).FirstOrDefault

        If msi Is Nothing Then
            Call Workbench.Warning("You must open a ms-imaging data viewer at first!")
            Return
        End If

        Dim labels As String() = Nothing
        Dim mz As Double() = Nothing

        If ParseMsSet Is Nothing Then
            ' use a column as mz source
            ' additional column as label name of the corresponding mz
            Dim form As New SetTableMzSource
            Dim fieldNames As New List(Of String)

            For Each col As DataGridViewColumn In AdvancedDataGridView1.Columns
                Call fieldNames.Add(col.Name)
            Next

            Call form.SetColumns(fieldNames)
            Call InputDialog.Input(
                Sub(frm)
                    mz = frm.GetMz(AdvancedDataGridView1)
                    labels = frm.GetNames(AdvancedDataGridView1)
                End Sub, config:=form)
        Else
            Dim row = excel.GetSelectedRow

            If row.IsNullOrEmpty Then
                Call Workbench.Warning("A row data must be selected!")
                Return
            End If

            Dim mzSet = ParseMsSet(row).ToArray

            labels = mzSet.Select(Function(a) a.Name).ToArray
            mz = mzSet.Select(Function(a) a.Value).ToArray
        End If

        If labels.IsNullOrEmpty Then
            labels = New String(mz.Length - 1) {}

            For i As Integer = 0 To mz.Length - 1
                labels(i) = "MSI" & mz(i).ToString("F4")
            Next

            labels = labels.UniqueNames
        End If

        Call WindowModules.msImageParameters.ImportsIons(labels, mz)
        Call VisualStudio.Dock(WindowModules.msImageParameters, DockState.DockLeft)
        Call VisualStudio.ShowSingleDocument(Of frmMsImagingViewer)()
    End Sub
End Class
