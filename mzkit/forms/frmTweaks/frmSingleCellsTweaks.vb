Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.SinglesCellViewer

Public Class frmSingleCellsTweaks

    Public ReadOnly Property args As SingleCellViewerArguments

    Public Sub SetSingleCells(cells As IEnumerable(Of UMAPPoint))
        _args = New SingleCellViewerArguments(cells)
        PropertyGrid1.SelectedObject = args
    End Sub

    Private Sub PropertyGrid1_PropertyValueChanged(s As Object, e As PropertyValueChangedEventArgs) Handles PropertyGrid1.PropertyValueChanged
        WindowModules.singleCellViewer.DoRender()
    End Sub

    Private Sub frmSingleCellsTweaks_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "Single Cells Viewer"

        Call ApplyVsTheme(ToolStrip1, ContextMenuStrip1)
    End Sub

    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click
        Dim node = Win7StyleTreeView1.SelectedNode

        If node Is Nothing Then
            Call Workbench.Warning("no target metabolite ion was selected for view the expression!")
            Return
        End If

        Call ViewIonMzExpression(CDbl(node.Tag))
    End Sub

    Private Sub ViewIonMzExpression(mz As Double)
        Call ProgressSpinner.DoLoading(Sub() Call WindowModules.singleCellViewer.DoRenderExpression(mz), host:=WindowModules.singleCellViewer)
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Dim str = Strings.Trim(ToolStripTextBox1.Text)

        If str.StringEmpty(, True) Then
            Call Workbench.Warning("An ion m/z numeric value should be input at first before we view the ion expression data!")
            Return
        End If

        Call ViewIonMzExpression(mz:=Val(str))
    End Sub
End Class