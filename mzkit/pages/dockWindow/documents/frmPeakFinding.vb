Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math

Public Class frmPeakFinding

    Dim matrix As ChromatogramTick()
    Dim rawName As String
    Dim peakList As New Dictionary(Of String, ROI)

    Public Sub LoadMatrix(title As String, data As IEnumerable(Of ChromatogramTick))
        Me.matrix = data.OrderBy(Function(t) t.Time).ToArray
        Me.rawName = title

        Call InitPanel()
    End Sub

    Public Sub InitPanel()
        Dim size As Size = PictureBox1.Size
        Dim plot As Image = {
            New NamedCollection(Of ChromatogramTick)(rawName, matrix)
        } _
            .TICplot(
                intensityMax:=0,
                isXIC:=True,
                colorsSchema:=Globals.GetColors,
                fillCurve:=Globals.Settings.viewer.fill,
                gridFill:="white"
            ).AsGDIImage

        PictureBox1.BackgroundImage = plot

        PeakMatrixViewer.Columns.Clear()
        PeakMatrixViewer.Columns.Add("Time", "Time")
        PeakMatrixViewer.Columns.Add("Intensity", "Intensity")

        For Each row As ChromatogramTick In matrix
            Call PeakMatrixViewer.Rows.Add(row.Time, row.Intensity)
        Next

        ' do peak list finding
        Dim peakwidth As DoubleRange = getPeakwidth()
        Dim peakROIs As ROI() = matrix.Shadows.PopulateROI(
            peakwidth:=peakwidth,
            baselineQuantile:=getBaseline,
            snThreshold:=getSnThreshold
        ).ToArray

        PeakListViewer.Columns.Clear()
        PeakListViewer.Columns.Add("ROI", "ROI")
        PeakListViewer.Columns.Add("rtmin", "rtmin")
        PeakListViewer.Columns.Add("rtmax", "rtmax")
        PeakListViewer.Columns.Add("rt", "rt")
        PeakListViewer.Columns.Add("peakWidth", "peakWidth")
        PeakListViewer.Columns.Add("maxInto", "maxInto")
        PeakListViewer.Columns.Add("nticks", "nticks")
        PeakListViewer.Columns.Add("baseline", "baseline")
        PeakListViewer.Columns.Add("integration", "integration")
        PeakListViewer.Columns.Add("noise", "noise")
        PeakListViewer.Columns.Add("snRatio", "snRatio")

        For Each roi As ROI In peakROIs
            peakList.Add(roi.ToString, roi)

            PeakListViewer.Rows.Add(
                roi.ToString,
                roi.time.Min,
                roi.time.Max,
                roi.rt,
                roi.peakWidth,
                roi.maxInto,
                roi.ticks.Length,
                roi.baseline,
                roi.integration,
                roi.noise,
                roi.snRatio
            )
        Next
    End Sub

    Private Function getSnThreshold() As Double
        Return 1
    End Function

    Private Function getBaseline() As Double
        Return 0.65
    End Function

    Private Function getPeakwidth() As DoubleRange
        Return New DoubleRange(3, 15)
    End Function

    Private Sub PictureBox1_Resize(sender As Object, e As EventArgs) Handles PictureBox1.Resize

    End Sub

    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click
        Dim rows = PeakListViewer.SelectedRows
        Dim rowId = rows.Item(Scan0)

    End Sub
End Class