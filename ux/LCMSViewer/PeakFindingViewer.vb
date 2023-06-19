Imports System.IO
Imports System.Text
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports any = Microsoft.VisualBasic.Scripting

Public Class PeakFindingViewer
    ''' <summary>
    ''' raw peaks data
    ''' </summary>
    Dim matrix As ChromatogramTick()
    Dim rawName As String
    Dim peakList As New Dictionary(Of String, ROI)
    Dim plotMatrixList As NamedCollection(Of ChromatogramTick)()
    Dim bspline As Boolean
    Dim args As New PeakFindingParameter

    Public Sub LoadMatrix(title As String, data As IEnumerable(Of ChromatogramTick), Optional args As PeakFindingParameter = Nothing)
        Me.matrix = data.OrderBy(Function(t) t.Time).ToArray
        Me.rawName = title

        If Not args Is Nothing Then
            Me.args = args
        End If

        Call InitPanel()
    End Sub

    Private Sub plotMatrix(spline As Boolean, ParamArray result As NamedCollection(Of ChromatogramTick)())
        Dim size As Size = PictureBox1.Size
        Dim plot As Image = result _
            .TICplot(
                intensityMax:=0,
                isXIC:=True,
                colorsSchema:=Workbench.GetPlotColors,
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
        Dim peakwidth As DoubleRange = args.peakwidth
        Dim peakROIs As ROI() = matrix _
            .Shadows _
            .PopulateROI(
                peakwidth:=peakwidth,
                baselineQuantile:=args.baseline,
                snThreshold:=args.SN
            ) _
            .ToArray

        PeakListViewer.Rows.Clear()
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

    Public Class PeakFindingParameter

        Public SN As Double = 1
        Public baseline As Double = 0.65
        Public peakwidth As Double() = {3, 15}

    End Class

    Private Sub PictureBox1_Resize(sender As Object, e As EventArgs) Handles PictureBox1.Resize
        If Not plotMatrixList.IsNullOrEmpty Then
            Call plotMatrix(bspline, plotMatrixList)
        End If
    End Sub

    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click
        If PeakListViewer.SelectedRows.Count <= 0 Then
            Call Workbench.Warning("Please select a row data for view content!")
            Return
        End If

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

    ''' <summary>
    ''' open table file and imports for peak finding analysis
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Using file As New OpenFileDialog With {.Filter = "Excel Table(*.csv)|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                Dim data = file.FileName.LoadCsv(Of ChromatogramTick)()

                Call LoadMatrix(file.FileName.FileName, data)
            End If
        End Using
    End Sub

    ''' <summary>
    ''' save as excel table file
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Call PeakListViewer.SaveDataGrid(rawName)
    End Sub

    ''' <summary>
    ''' copy as table
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Dim sb As New StringBuilder
        Dim text As New StringWriter(sb)

        Call PeakListViewer.WriteTableToFile(text)
        Call text.Flush()
        Call Clipboard.Clear()
        Call Clipboard.SetText(sb.ToString)
    End Sub

    ''' <summary>
    ''' send to table viewer
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        Call PeakListViewer.OpenInTableViewer
    End Sub

    ''' <summary>
    ''' save peaks
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        Call PeakMatrixViewer.SaveDataGrid(rawName)
    End Sub

    ''' <summary>
    ''' copy peaks
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
        Dim sb As New StringBuilder
        Dim text As New StringWriter(sb)

        Call PeakMatrixViewer.WriteTableToFile(text)
        Call text.Flush()
        Call Clipboard.Clear()
        Call Clipboard.SetText(sb.ToString)
    End Sub

    ''' <summary>
    ''' send peak table to table viewer
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SendToTableViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SendToTableViewerToolStripMenuItem.Click
        Call PeakMatrixViewer.OpenInTableViewer
    End Sub

    ''' <summary>
    ''' config of the new parameters
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton5_Click(sender As Object, e As EventArgs) Handles ToolStripButton5.Click
        Dim cfg As New InputPeakFindParameter

        Call cfg.SetArguments(args)
        Call InputDialog.Input(Sub(config) Call InitPanel(), config:=cfg)
    End Sub
End Class
