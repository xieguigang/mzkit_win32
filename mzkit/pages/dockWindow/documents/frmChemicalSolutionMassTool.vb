Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.Repository
Imports Microsoft.VisualBasic.Data.Framework
Imports Mzkit_win32.BasicMDIForm.Container
Imports SMRUCC.genomics.GCModeller.Workbench.ReportBuilder.HTML

Public Class frmChemicalSolutionMassTool

    Dim chemicals As Dictionary(Of String, ChemicalInformation)
    Dim calc As SolutionMassCalculator

    ReadOnly dbfile As String = App.ProductProgramData & "/solution_chemicals.csv"

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Call ListBox1.Items.Clear()
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

        ListBox1.Items.Add(New SolutionChemical With {
            .name = Label8.Text,
            .content = Val(TextBox2.Text),
            .mass = 0,
            .type = type
        })

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

                html!vl = TextBox1.Text
                html.Flush(False, temp)

                Call Helper.PDF(file.FileName, temp)
            End If
        End Using
    End Sub
End Class