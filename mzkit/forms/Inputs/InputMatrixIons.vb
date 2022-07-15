Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports DataFrame = Microsoft.VisualBasic.Data.csv.IO.DataFrame

Public Class InputMatrixIons

    Public ReadOnly Property matrixSize As Size
        Get
            Return New Size(Integer.Parse(txtColumns.Text), Integer.Parse(txtRows.Text))
        End Get
    End Property

    Public Iterator Function GetSelectedIons() As IEnumerable(Of NamedValue(Of Double))
        Dim mz As Double() = DataGridView1.getFieldVector("mz")
        Dim name As Array = DataGridView1.getFieldVector("name")
        Dim precursor As Array = DataGridView1.getFieldVector("precursor_type")
        Dim selects As Boolean() = DataGridView1.getFieldVector("select")
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

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DialogResult = DialogResult.OK
    End Sub

    Private Sub InputMatrixIons_Load(sender As Object, e As EventArgs) Handles Me.Load
        ToolStripStatusLabel1.Text = "Please select 9 ions to visual data..."
    End Sub

    Dim n As Integer = 1

    Private Sub DataGridView1_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellValueChanged
        If e.ColumnIndex = 0 AndAlso e.RowIndex >= 0 Then
            Dim r = DataGridView1.Rows(e.RowIndex)
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

    Dim ion_initialized As Boolean = False

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

        DataGridView1.Columns.Clear()
        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName
        DataGridView1.DataSource = BindingSource1
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

            End If
        End Using
    End Sub
End Class