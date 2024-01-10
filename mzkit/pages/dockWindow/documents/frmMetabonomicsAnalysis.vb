Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Class frmMetabonomicsAnalysis

    Dim sampleinfo As SampleInfo()
    Dim properties As String()
    Dim peaks As PeakSet

    Public Sub LoadData(table As DataTable)

    End Sub

    Public Sub LoadData(table As File, title As String)
        Dim wizard As New InputImportsPeaktableDialog
        wizard.LoadSampleId(table.First.ToArray)
        InputDialog.Input(
            Sub(config)
                sampleinfo = config.GetSampleInfo.ToArray
                properties = config.GetMetadata.ToArray
            End Sub, Nothing, wizard)

        ' show data
    End Sub

End Class