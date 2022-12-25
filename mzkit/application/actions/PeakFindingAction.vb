Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports WeifenLuo.WinFormsUI.Docking

Public Class PeakFindingAction : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Do peak finding, the selected column data should be the signal intensity value."
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, table As DataTable)
        Dim getFormula As New InputPeakTime
        Dim mask As New MaskForm(MyApplication.host.Location, MyApplication.host.Size)

        For i As Integer = 0 To table.Columns.Count - 1
            Dim tag As String = table.Columns.Item(i).ColumnName

            getFormula.ComboBox1.Items.Add(tag)
        Next

        If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
            Dim TimeName As String = getFormula.TimeField
            Dim intensityValue As Double() = data.AsObjectEnumerator().Select(Function(oi) CType(oi, Double)).ToArray
            Dim timeVal As Double()

            If TimeName.StringEmpty Then
                timeVal = intensityValue.Sequence.Select(Function(t) Val(t)).ToArray
            Else
                timeVal = table.getFieldVector(TimeName)
            End If

            Dim matrix As ChromatogramTick() = timeVal _
                .Select(Function(t, i)
                            Return New ChromatogramTick() With {
                                .Time = t,
                                .Intensity = intensityValue(i)
                            }
                        End Function) _
                .ToArray

            Dim app = VisualStudio.ShowDocument(Of frmPeakFinding)(DockState.Document, $"Peak Finding [{fieldName}]")
            app.LoadMatrix(fieldName, matrix)
        End If
    End Sub
End Class
