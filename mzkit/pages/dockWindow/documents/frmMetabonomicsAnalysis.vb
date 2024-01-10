Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.My.JavaScript
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner

Public Class frmMetabonomicsAnalysis

    Dim sampleinfo As SampleInfo()
    Dim properties As String()
    Dim peaks As PeakSet
    Dim metadata As New Dictionary(Of String, JavaScriptObject)

    Public Sub LoadData(table As DataTable)

    End Sub

    Public Sub LoadData(table As File, title As String)
        Dim wizard As New InputImportsPeaktableDialog
        Dim df As DataFrame = DataFrame.CreateObject(table)

        Call wizard.LoadSampleId(df.HeadTitles)
        Call InputDialog.Input(
            Sub(config)
                sampleinfo = config.GetSampleInfo.ToArray
                properties = config.GetMetadata.ToArray
            End Sub, Nothing, wizard)

        ' show data
        Dim peaks As New List(Of xcms2)
        Dim peak As xcms2

        For Each row In df.EnumerateData
            peak = New xcms2 With {
                .ID = row.TryPopOut({"id", "ID", "xcms_id"}),
                .mz = row.TryPopOut({"mz", "m/z", "MZ"}),
                .mzmax = row.TryPopOut({"mzmax", "mz.max"}),
                .mzmin = row.TryPopOut({"mzmin", "mz.min"}),
                .rt = row.TryPopOut({"rt", "RT", "retention_time"}),
                .rtmax = row.TryPopOut({"rtmax", "rt.max"}),
                .rtmin = row.TryPopOut({"rtmin", "rt.min"}),
                .Properties = New Dictionary(Of String, Double)
            }

            For Each id In sampleinfo
                peak(id.ID) = row.TryPopOut(id.ID)
            Next

            Dim meta As New JavaScriptObject

            For Each item In row
                meta(item.Key) = item.Value
            Next

            peaks.Add(peak)
            metadata(peak.ID) = meta
        Next

        Me.peaks = New PeakSet With {
            .peaks = peaks.ToArray
        }
    End Sub

End Class