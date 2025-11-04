Imports System.Windows.Forms

Public Class ExcelTableViewer

    ReadOnly excel As formexcelpad

    Friend WithEvents MSImagingIonListToolStripMenuItem As ToolStripMenuItem

    Sub New(excel As formexcelpad)
        Me.excel = excel
        Me.MSImagingIonListToolStripMenuItem = New ToolStripMenuItem
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
            Dim row = GetSelectedRow

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
