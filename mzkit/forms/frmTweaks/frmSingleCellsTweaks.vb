﻿Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
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
End Class