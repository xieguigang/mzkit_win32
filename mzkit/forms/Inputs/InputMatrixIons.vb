Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports DataFrame = Microsoft.VisualBasic.Data.csv.IO.DataFrame
Imports stdNum = System.Math
Imports xlsx = Microsoft.VisualBasic.MIME.Office.Excel.File

Public Class InputMatrixIons

    Public ReadOnly Property MSILayout As Size
        Get
            Return New Size(
                width:=Integer.Parse(txtColumns.Text),
                height:=Integer.Parse(txtRows.Text)
            )
        End Get
    End Property

    Public ReadOnly Property CanvasSize As Size
        Get
            Return New Size(
                width:=Integer.Parse(TextBox2.Text),
                height:=Integer.Parse(TextBox3.Text)
            )
        End Get
    End Property

    Dim search As GridSearchHandler

    ''' <summary>
    ''' [name => mz, description = precursor_type]
    ''' </summary>
    ''' <returns></returns>
    Public Iterator Function GetSelectedIons() As IEnumerable(Of NamedValue(Of Double))
        Dim mz As Double() = AdvancedDataGridView1.getFieldVector("mz")
        Dim name As Array = AdvancedDataGridView1.getFieldVector("name")
        Dim precursor As Array = AdvancedDataGridView1.getFieldVector("precursor_type")
        Dim selects As Boolean() = AdvancedDataGridView1.getFieldVector("select")
        Dim n As Integer = MSILayout.Width * MSILayout.Height
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
        Dim nsize As Size = MSILayout
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
            Dim size = Me.MSILayout
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

    Private Function ReadTableAuto(fileName As String) As DataFrame
        If fileName.ExtensionSuffix("csv") Then
            Return DataFrame.Load(fileName)
        Else
#Disable Warning
            Dim xlsx As xlsx = xlsx.Open(fileName)
            Dim view As DataFrame = Nothing
            Dim titles As Index(Of String)
#Enable Warning

            For Each name As String In xlsx.SheetNames
                Dim sheet = xlsx.GetTable(name)

                view = DataFrame.CreateObject(file:=sheet)
                titles = view.HeadTitles.Indexing

                If {"name", "formula"}.All(Function(str) str Like titles) Then
                    Return view
                End If
            Next

            Return view
        End If
    End Function

    Public Sub LoadMetabolites() Handles LoadMetabolitesToolStripMenuItem.Click
        Using file As New OpenFileDialog With {.Filter = "Excel table(*.xlsx;*.csv)|*.xlsx;*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                ' the fields is required:
                '   name: the metabolite name
                '   formula: the molecule formula for evaluate the exact mass
                Dim data As DataFrame = ReadTableAuto(file.FileName)
                Dim name_i As Integer = data.GetOrdinal("name")
                Dim formula_i As Integer = data.GetOrdinal("formula")

                If name_i = -1 Then
                    MessageBox.Show("missing the metabolite 'name'!", "Load metabolites", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                ElseIf formula_i = -1 Then
                    MessageBox.Show("missing the metabolite 'formula'!", "Load metabolites", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return
                End If

                Dim mzrange_i As Integer = data.GetOrdinal("mz", "m/z")

                If mzrange_i > -1 Then
                    Call setupMzRange(data, name_i, formula_i, mzrange_i)
                Else
                    Call setupAllIons(data, name_i, formula_i)
                End If
            End If
        End Using
    End Sub

    Private Sub setupMzRange(data As DataFrame, name_i As Integer, formula_i As Integer, mzrange_i As Integer)
        Dim name As String() = data.Column(name_i).ToArray
        Dim formula As String() = data.Column(formula_i).ToArray
        Dim mzList As Double() = data.Column(mzrange_i).Select(Function(str) str.ParseDouble).ToArray
        Dim exact_mass As Double() = formula _
            .Select(Function(f)
                        Return FormulaScanner.ScanFormula(f).ExactMass
                    End Function) _
            .ToArray

        Dim precursor_type As New List(Of String)
        Dim pixels As New List(Of Integer)
        Dim density As New List(Of Double)

        For i As Integer = 0 To name.Length - 1
            Dim mzi As Double = mzList(i)
            Dim mass As Double = exact_mass(i)
            Dim d As Double = 999
            Dim target As String = "n/a"

            For Each type As MzCalculator In Provider.Positives
                Dim mz_ref As Double = type.CalcMZ(mass)

                If stdNum.Abs(mz_ref - mzi) < d Then
                    target = type.ToString
                    d = stdNum.Abs(mz_ref - mzi)
                End If
            Next

            Call precursor_type.Add(target)
            Call pixels.Add(0)
            Call density.Add(0)
        Next

        Call Setup(mzList, name.ToArray, precursor_type.ToArray, pixels.ToArray, density.ToArray)
    End Sub

    Private Sub setupAllIons(data As DataFrame, name_i As Integer, formula_i As Integer)
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
    End Sub

    Private Sub ExportSingleIonsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportSingleIonsToolStripMenuItem.Click
        Using folder As New FolderBrowserDialog With {.ShowNewFolderButton = True}
            If folder.ShowDialog = DialogResult.OK Then
                Dim mzdiff As String = $"da:{txtMzdiff.Text}"

                For Each mz As NamedValue(Of Double) In GetSelectedIons()
                    Call RscriptProgressTask.ExportSingleIonPlot(
                        mz:=mz.Value,
                        tolerance:=mzdiff,
                        saveAs:=$"{folder.SelectedPath}/${mz.Value.ToString("F4")}.png",
                        title:=$"{mz.Name} {mz.Description}",
                        background:="black",
                        colorSet:="viridis:turbo"
                    )
                Next
            End If
        End Using
    End Sub
End Class