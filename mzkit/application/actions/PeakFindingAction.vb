﻿Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.mzkit_win32.My
Imports Galaxy.Workbench.CommonDialogs
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualStudio.WinForms.Docking
Imports Mzkit_win32.BasicMDIForm

Public Class PeakFindingAction : Inherits ActionBase

    Public Overrides ReadOnly Property Description As String
        Get
            Return "Do peak finding, the selected column data should be the signal intensity value."
        End Get
    End Property

    Public Overrides Sub RunAction(fieldName As String, data As Array, table As DataTable)
        Dim getFormula As New InputPeakTime
        Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)

        For Each tag As String In GetFieldNames(table)
            Call getFormula.ComboBox1.Items.Add(tag)
        Next

        If mask.ShowDialogForm(getFormula) = DialogResult.OK Then
            Dim TimeName As String = getFormula.TimeField
            Dim intensityValue As Double() = data _
                .AsObjectEnumerator() _
                .Select(Function(oi) CType(oi, Double)) _
                .ToArray
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
