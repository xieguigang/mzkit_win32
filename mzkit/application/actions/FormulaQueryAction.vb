Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports stdNum = System.Math

Public Class FormulaQueryAction : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Run formula query on a given set of m/z data list, the theoretically m/z is evaluated from the input formula data by combine different ion precursr and then match theoretical m/z with the input m/z set."
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, tbl As DataTable)
        Dim getFormula As New InputFormula
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

        If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
            Dim formula As Formula = FormulaScanner.ScanFormula(getFormula.TextBox1.Text)
            Dim name As String = getFormula.TextBox2.Text
            Dim ppm As Integer = getFormula.NumericUpDown1.Value
            Dim adducts As MzCalculator() = getFormula.GetAdducts
            Dim mz As Double() = data.AsObjectEnumerator.Select(Function(o) Val(o)).ToArray
            Dim headers As New Dictionary(Of String, Type)
            Dim tblView = VisualStudio.ShowDocument(Of frmTableViewer)(title:=$"Formula Query[{formula}]")
            Dim rows As New List(Of (DataRow, MzCalculator, ppm As Double, mztarget As Double))

            headers.Add("mz_theoretical", GetType(Double))
            headers.Add("precursor_type", GetType(String))
            headers.Add("ppm", GetType(Double))
            headers.Add("name", GetType(String))
            headers.Add("formula", GetType(String))

            For i As Integer = 0 To tbl.Columns.Count - 1
                Dim tag As String = tbl.Columns.Item(i).ColumnName

                If headers.ContainsKey(tag) Then
                    tag = $"{tag}_1"
                End If

                headers.Add(tag, tbl.Columns.Item(i).DataType)
            Next

            For Each type As MzCalculator In adducts
                Dim mzTarget As Double = type.CalcMZ(formula.ExactMass)
                Dim query = mz _
                            .Select(Function(mzi, idx) (PPMmethod.PPM(mzi, mzTarget), idx)) _
                            .Where(Function(t) t.Item1 <= ppm) _
                            .ToArray

                If query.Length > 0 Then
                    For Each idx In query
                        Dim row = tbl.Rows.Item(idx.idx)
                        rows.Add((row, type, idx.Item1, mzTarget))
                    Next
                End If
            Next

            Call tblView.LoadTable(
                        Sub(subView)
                            For Each field In headers
                                Call subView.Columns.Add(field.Key, field.Value)
                            Next

                            For Each row In rows
                                Dim values As New List(Of Object)
                                Dim subData = row.Item1

                                values.Add(stdNum.Round(row.mztarget, 4))
                                values.Add(row.Item2.ToString)
                                values.Add(stdNum.Round(row.ppm))
                                values.Add(name)
                                values.Add(formula.EmpiricalFormula)
                                values.AddRange(subData.ItemArray)

                                Call subView.Rows.Add(values.ToArray)
                            Next
                        End Sub)
        End If
    End Sub
End Class
