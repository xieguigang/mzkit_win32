Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.MIME.Office.Excel.XLSX
Imports Mzkit_win32.BasicMDIForm.Container
Imports SMRUCC.genomics.GCModeller.Workbench.ReportBuilder.HTML

Public Class frmChemicalSolutionMassTool

    Dim chemicals As Dictionary(Of String, ChemicalInformation)
    Dim calc As SolutionMassCalculator

    ReadOnly dbfile As String = App.ProductProgramData & "/solution_chemicals.csv"

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Call ListBox1.Items.Clear()
        Label8.Text = "<Chemical Name>"
        TextBox2.Text = "0"
    End Sub

    Private Sub frmChemicalSolutionMassTool_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim chemicals As ChemicalInformation()

        TabText = Text
        ComboBox1.SelectedIndex = 0

        Call ApplyVsTheme(ToolStrip1)

        If dbfile.FileExists Then
            chemicals = dbfile.LoadCsv(Of ChemicalInformation)
        Else
            Dim path As String

            If AppEnvironment.IsDevelopmentMode Then
                path = $"{App.HOME}/../../src\mzkit\extdata\chemicals.csv"
            Else
                path = $"{App.HOME}/demo/chemicals.csv"
            End If

            chemicals = path.LoadCsv(Of ChemicalInformation)
        End If

        Me.chemicals = chemicals.MakeUniqueNames.ToDictionary(Function(a) a.chemicals)
        Me.updateChemicalsUI()
    End Sub

    Private Sub updateChemicalsUI()
        Call DataGridView1.Rows.Clear()

        For Each item In chemicals.Values
            Call DataGridView1.Rows.Add(item.chemicals, item.formula,
                                        FormulaScanner.EvaluateExactMass(item.formula).ToString("F4"),
                                        FormulaScanner.EvaluateAverageMolecularMass(item.formula).ToString("F4"))
        Next

        Me.calc = New SolutionMassCalculator(chemicals.ToDictionary(Function(a) a.Key, Function(a) a.Value.formula), useExactMass:=CheckBox1.Checked)

        Call DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
    End Sub

    Private Class ChemicalInformation : Implements INamedValue

        Public Property chemicals As String Implements INamedValue.Key
        Public Property formula As String

        Public Function IsValidFormula() As Boolean
            Return FormulaScanner.ScanFormula(formula) IsNot Nothing
        End Function

    End Class

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Dim currentChemical As ChemicalInformation

    Private Sub DataGridView1_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridView1.SelectionChanged
        If DataGridView1.SelectedRows.Count = 0 Then
            Return
        End If

        Dim rowMeta = DataGridView1.SelectedRows(0)

        TextBox2.Text = 0
        Label8.Text = CStr(rowMeta.Cells(0).Value)
        currentChemical = chemicals(Label8.Text)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim type_str = ComboBox1.Items(ComboBox1.SelectedIndex).ToString
        Dim type As ConcentrationType = SolutionMassCalculator.ParseConcentrationType(type_str)
        Dim update As Boolean = False

        For i As Integer = 0 To ListBox1.Items.Count - 1
            Dim item As SolutionChemical = DirectCast(ListBox1.Items(i), SolutionChemical)
            If item.name = Label8.Text Then
                item.content = Val(TextBox2.Text)
                item.type = type
                update = True
                Exit For
            End If
        Next

        If Not update Then
            ListBox1.Items.Add(New SolutionChemical With {
                .name = Label8.Text,
                .content = Val(TextBox2.Text),
                .mass = 0,
                .type = type
            })
        End If

        Call calList()
    End Sub

    Private Sub calList()
        If Not calc Is Nothing Then
            Dim listSet As New List(Of SolutionChemical)

            For i As Integer = 0 To ListBox1.Items.Count - 1
                Call listSet.Add(ListBox1.Items(i))
            Next

            Call ListBox1.Items.Clear()

            For Each item In calc.CalculateSolutionMasses(listSet, Val(TextBox1.Text))
                Call ListBox1.Items.Add(item)
            Next

            Call ListBox1.Refresh()
        End If
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        Call calList()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        Me.calc = New SolutionMassCalculator(chemicals.ToDictionary(Function(a) a.Key, Function(a) a.Value.formula), useExactMass:=CheckBox1.Checked)
        Call calList()
    End Sub

    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit

    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Dim templatefile As String

        If AppEnvironment.IsDevelopmentMode Then
            templatefile = $"{App.HOME}/../../src\mzkit\extdata\templates\ReagentFormulation\index.html"
        Else
            templatefile = $"{App.HOME}/templates/ReagentFormulation\index.html"
        End If

        Using file As New SaveFileDialog With {.Filter = "Report File(*.pdf)|*.pdf"}
            If file.ShowDialog = DialogResult.OK Then
                Dim html As New TemplateHandler(templatefile)
                Dim temp As String = App.AppSystemTemp & "/" & App.GetNextUniqueName("template_") & "/index.html"
                Dim listSet As New List(Of String)
                Dim mw As Func(Of String, Double) = If(CheckBox1.Checked,
                    New Func(Of String, Double)(AddressOf FormulaScanner.EvaluateExactMass),
                    New Func(Of String, Double)(AddressOf FormulaScanner.EvaluateAverageMolecularMass)
                )

                For i As Integer = 0 To ListBox1.Items.Count - 1
                    Dim chemical As SolutionChemical = DirectCast(ListBox1.Items(i), SolutionChemical)
                    Dim row = $"<tr>
<td>{chemical.name}</td>
<td>{chemicals(chemical.name).formula}</td>
<td>{mw(chemicals(chemical.name).formula).ToString("F4")}</td>
<td>{chemical.content} {chemical.type.Description}</td>
<td>{chemical.mass.ToString("F4")} g</td>
</tr>"
                    Call listSet.Add(row)
                Next

                html!chemicals = listSet.JoinBy("")
                html!vl = TextBox1.Text
                html.Flush(False, temp)

                Call Helper.PDF(file.FileName, temp)
                Call Process.Start(file.FileName)
            End If
        End Using
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        If ListBox1.SelectedIndex < 0 Then
            Return
        End If

        Dim item As SolutionChemical = DirectCast(ListBox1.SelectedItem, SolutionChemical)
        Label8.Text = item.name
        TextBox2.Text = item.content
        ComboBox1.SelectedIndex = ComboBox1.FindString(item.type.Description)
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Using file As New OpenFileDialog With {.Filter = "Chemicals List(*.csv)|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                Me.chemicals = file.FileName.LoadCsv(Of ChemicalInformation).MakeUniqueNames.ToDictionary(Function(a) a.chemicals)
                Me.updateChemicalsUI()
            End If
        End Using
    End Sub

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        Using file As New SaveFileDialog With {.Filter = "Excel Table(*.xlsx)|*.xlsx"}
            If file.ShowDialog = DialogResult.OK Then
                Dim listSet As New List(Of SolutionChemical)
                For i As Integer = 0 To ListBox1.Items.Count - 1
                    Call listSet.Add(ListBox1.Items(i))
                Next
                Call listSet.SaveToExcel(file.FileName, "Solution Chemicals", New String()() {
                    New String() {"Solution Volume:", TextBox1.Text, "mL"},
                    New String() {"Use Exact Mass:", CheckBox1.Checked.ToString, If(CheckBox1.Checked, "Config For Mass Spectrum Analysis", "")},
                    New String() {},
                    New String() {"Chemical Reagent Formulation"}
                })

                If MessageBox.Show("Export the chemical reagent formulation data success, open and view in excel?", "Export Success", MessageBoxButtons.YesNo, MessageBoxIcon.Information) = DialogResult.OK Then
                    Call Process.Start(file.FileName)
                End If
            End If
        End Using
    End Sub
End Class