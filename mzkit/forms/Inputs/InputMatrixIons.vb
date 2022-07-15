Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports DataFrame = Microsoft.VisualBasic.Data.csv.IO.DataFrame

Public Class InputMatrixIons

    Public ReadOnly Property matrixSize As Size
        Get
            Return New Size(Integer.Parse(txtColumns.Text), Integer.Parse(txtRows.Text))
        End Get
    End Property

    Dim search As GridSearchHandler

    Public Iterator Function GetSelectedIons() As IEnumerable(Of NamedValue(Of Double))
        Dim mz As Double() = AdvancedDataGridView1.getFieldVector("mz")
        Dim name As Array = AdvancedDataGridView1.getFieldVector("name")
        Dim precursor As Array = AdvancedDataGridView1.getFieldVector("precursor_type")
        Dim selects As Boolean() = AdvancedDataGridView1.getFieldVector("select")
        Dim n As Integer = matrixSize.Width * matrixSize.Height
        Dim j As Integer = 1

        For i As Integer = 0 To selects.Length - 1
            If selects(i) Then
                Yield New NamedValue(Of Double)(name(i), mz(i), precursor(i))

                j += 1

                If j > n Then
                    Exit For
                End If
            End If
        Next
    End Function

    ''' <summary>
    ''' 不进行窗口的隐藏，可以在弹出对话框后，进行不同离子的组合，产生多种组合形式下的绘图结果
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim nsize As Size = matrixSize
        Dim n1 = GetSelectedIons.Count
        Dim n2 = nsize.Width * nsize.Height

        If n1 < n2 Then
            MessageBox.Show($"the number of the selected ions({n1}) should be equals to or greater than the matrix layout size({n2})!",
                            "Matrix Heatmap",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        )
        Else
            Call frmMsImagingViewer.renderMatrixHeatmap(Me)
        End If
    End Sub

    Private Sub InputMatrixIons_Load(sender As Object, e As EventArgs) Handles Me.Load
        ToolStripStatusLabel1.Text = "Please select 9 ions to visual data..."
        search = New GridSearchHandler(AdvancedDataGridView1)
        AddHandler AdvancedDataGridViewSearchToolBar1.Search, AddressOf search.AdvancedDataGridViewSearchToolBar1_Search
    End Sub

    Dim n As Integer = 1

    Private Sub DataGridView1_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs)
        If e.ColumnIndex = 0 AndAlso e.RowIndex >= 0 Then
            Dim r = AdvancedDataGridView1.Rows(e.RowIndex)
            Dim value As Boolean = r.Cells(e.ColumnIndex).Value
            Dim size = Me.matrixSize
            Dim total As Integer = size.Width * size.Height

            If value = True Then
                n += 1
            Else
                n -= 1
            End If

            If n < 0 Then
                n = 0
            End If

            Dim text As String = $"there are {n}/{total} ions has been selected."

            If n > total Then
                text &= $" ions that after #{total} will be ignores."
            End If

            ToolStripStatusLabel1.Text = text
        End If
    End Sub

    Friend ion_initialized As Boolean = False

    Public Sub Setup(mz As Double(), name As String(), precursor_type As String(), pixels As Integer(), density As Double())
        Dim memoryData As New DataSet
        Dim table As DataTable = memoryData.Tables.Add("memoryData")

        table.Columns.Add("select", GetType(Boolean))
        table.Columns.Add("mz", GetType(Double))
        table.Columns.Add("name", GetType(String))
        table.Columns.Add("precursor_type", GetType(String))
        table.Columns.Add("pixels", GetType(Integer))
        table.Columns.Add("density", GetType(Double))

        For i As Integer = 0 To mz.Length - 1
            ion_initialized = True
            table.Rows.Add({False, mz(i), name(i), precursor_type(i), pixels(i), density(i)})
        Next

        Call Me.AdvancedDataGridView1.Columns.Clear()
        Call Me.AdvancedDataGridView1.Rows.Clear()
        Call AdvancedDataGridView1.SetDoubleBuffered()

        For Each column As DataGridViewColumn In AdvancedDataGridView1.Columns
            AdvancedDataGridView1.ShowMenuStrip(column)
        Next

        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName

        AdvancedDataGridView1.DataSource = BindingSource1
        AdvancedDataGridViewSearchToolBar1.SetColumns(AdvancedDataGridView1.Columns)
    End Sub

    Public Sub LoadMetabolites() Handles LoadMetabolitesToolStripMenuItem.Click
        Using file As New OpenFileDialog With {.Filter = "Excel table(*.csv)|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                ' the fields is required:
                '   name: the metabolite name
                '   formula: the molecule formula for evaluate the exact mass
                Dim data As DataFrame = DataFrame.Load(file.FileName)
                Dim name_i As Integer = data.GetOrdinal("name")
                Dim formula_i As Integer = data.GetOrdinal("formula")

                If name_i = -1 Then
                    MessageBox.Show("missing the metabolite 'name'!", "Load metabolites", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                ElseIf formula_i = -1 Then
                    MessageBox.Show("missing the metabolite 'formula'!", "Load metabolites", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                Dim name As String() = data.Column(name_i).ToArray
                Dim formula As String() = data.Column(formula_i).ToArray
                Dim exact_mass As Double() = formula _
                    .Select(Function(f)
                                Return FormulaScanner.ScanFormula(f).ExactMass
                            End Function) _
                    .ToArray

                Dim mzList As New List(Of Double)
                Dim nameList As New List(Of String)
                Dim precursor_type As New List(Of String)
                Dim pixels As New List(Of Integer)
                Dim density As New List(Of Double)
                Dim i As Integer = 0

                For Each mass As Double In exact_mass
                    For Each type As MzCalculator In Provider.Positives
                        Call mzList.Add(type.CalcMZ(mass))
                        Call nameList.Add(name(i))
                        Call precursor_type.Add(type.ToString)
                        Call pixels.Add(0)
                        Call density.Add(0)
                    Next

                    i += 1
                Next

                Call Setup(mzList.ToArray, nameList.ToArray, precursor_type.ToArray, pixels.ToArray, density.ToArray)
            End If
        End Using
    End Sub
End Class