Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports any = Microsoft.VisualBasic.Scripting

Public Class frmPeakFinding

    Dim matrix As ChromatogramTick()
    Dim rawName As String
    Dim peakList As New Dictionary(Of String, ROI)
    Dim plotMatrixList As NamedCollection(Of ChromatogramTick)()
    Dim bspline As Boolean

    Public Sub LoadMatrix(title As String, data As IEnumerable(Of ChromatogramTick))
        Me.matrix = data.OrderBy(Function(t) t.Time).ToArray
        Me.rawName = title

        Call InitPanel()
    End Sub

    Private Sub plotMatrix(spline As Boolean, ParamArray result As NamedCollection(Of ChromatogramTick)())
        Dim size As Size = PictureBox1.Size
        Dim plot As Image = result _
            .TICplot(
                intensityMax:=0,
                isXIC:=True,
                colorsSchema:=Globals.GetColors,
                fillCurve:=True,
                gridFill:="white",
                spline:=If(spline, 3, 0),
                size:=$"{size.Width * 1.5},{size.Height * 1.5}"
            ).AsGDIImage

        bspline = spline
        plotMatrixList = result
        PictureBox1.BackgroundImage = plot
    End Sub

    Public Sub InitPanel()
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
                roi.time.Min.ToString("F2"),
                roi.time.Max.ToString("F2"),
                roi.rt.ToString("F2"),
                roi.peakWidth.ToString("F2"),
                roi.maxInto.ToString("G3"),
                roi.ticks.Length,
                roi.baseline.ToString("G3"),
                roi.integration.ToString("F3"),
                roi.noise.ToString("G3"),
                roi.snRatio.ToString("F2")
            )
        Next

        Call ShowMatrix(matrix)
        Call plotMatrix(spline:=False, New NamedCollection(Of ChromatogramTick)(rawName, matrix))
    End Sub

    Private Sub ShowMatrix(matrix As ChromatogramTick())
        PeakMatrixViewer.Columns.Clear()
        PeakMatrixViewer.Columns.Add("Time", "Time")
        PeakMatrixViewer.Columns.Add("Intensity", "Intensity")

        For Each row As ChromatogramTick In matrix
            Call PeakMatrixViewer.Rows.Add(row.Time.ToString("F2"), row.Intensity)
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
        If Not plotMatrixList.IsNullOrEmpty Then
            Call plotMatrix(bspline, plotMatrixList)
        End If
    End Sub

    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click
        If PeakListViewer.SelectedRows.Count <= 0 Then
            Call MyApplication.host.showStatusMessage("Please select a row data for view content!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Dim row As DataGridViewRow = PeakListViewer.SelectedRows(0)
            Dim peakId As String = any.ToString(row.Cells.Item(0).Value)
            Dim peakROI As ROI = peakList(peakId)
            Dim targetPeak As New NamedCollection(Of ChromatogramTick) With {
                .name = peakROI.ToString,
                .value = peakROI.ticks
            }
            Dim background As New NamedCollection(Of ChromatogramTick) With {
                .name = "Background",
                .value = peakROI _
                    .GetChromatogramData(matrix, 15) _
                    .ToArray
            }

            Call plotMatrix(spline:=True, background, targetPeak)
            Call ShowMatrix(peakROI.ticks)
        End If
    End Sub

    Private Sub PeakListViewer_RowStateChanged(sender As Object, e As DataGridViewRowStateChangedEventArgs) Handles PeakListViewer.RowStateChanged
        If e.StateChanged = DataGridViewElementStates.Selected Then
            Dim peakId As String = any.ToString(e.Row.Cells.Item(0).Value)

            If peakId.StringEmpty OrElse Not peakList.ContainsKey(peakId) Then
                Return
            End If

            Dim peakROI As ROI = peakList(peakId)
            Dim targetPeak As New NamedCollection(Of ChromatogramTick) With {
                .name = peakROI.ToString,
                .value = peakROI.ticks
            }

            Call plotMatrix(spline:=False, New NamedCollection(Of ChromatogramTick)(rawName, matrix), targetPeak)
        End If
    End Sub

    Private Sub frmPeakFinding_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call ApplyVsTheme(ContextMenuStrip1)
    End Sub
End Class