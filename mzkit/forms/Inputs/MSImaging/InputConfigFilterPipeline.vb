﻿Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports BioNovoGene.mzkit_win32.Configuration
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.ChartPlots.BarPlot.Histogram
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Axis
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Math2D

Public Class InputConfigFilterPipeline

    Dim into As Double()

    Public Function GetFilter() As RasterPipeline
        Dim filter As New RasterPipeline

        For i As Integer = 0 To CheckedListBox1.Items.Count - 1
            If CheckedListBox1.GetItemChecked(i) Then
                Call filter.Add(CheckedListBox1.Items(i))
            End If
        Next

        Return filter
    End Function

    Public Function GetFilterConfigs() As Filters
        Dim scripts As New List(Of String)
        Dim flags As New List(Of Boolean)

        For i As Integer = 0 To CheckedListBox1.Items.Count - 1
            Dim filter As Scaler = CheckedListBox1.Items(i)

            scripts.Add(filter.ToScript)
            flags.Add(CheckedListBox1.GetItemChecked(i))
        Next

        Return New Filters With {
            .filters = scripts.ToArray,
            .flags = flags.ToArray
        }
    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DialogResult = DialogResult.OK
    End Sub

    Public Sub ConfigIntensity(into As Double())
        Me.into = into

        If Not into.IsNullOrEmpty Then
            Call Me.PlotHist()
        End If
    End Sub

    Private Sub PlotHist()
        Dim into = Me.into.ToArray
        Dim filter = GetFilter()

        into = filter.DoIntensityScale(into)

        Dim axis As DoubleRange = into.CreateAxisTicks
        Dim canvas As Size = PictureBox1.Size.Scale(3)

        PictureBox1.BackgroundImage = into.HistogramPlot(
                  [step]:=(axis.Max - axis.Min) / 100,
                  size:=$"{canvas.Width},{canvas.Height}",
                  padding:="padding: 50px 20px 100px 100px;",
                  showGrid:=False) _
            .AsGDIImage
    End Sub

    Public Sub ConfigPipeline(filters As Scaler(), Optional flags As Boolean() = Nothing)
        Dim fill_scalers As New List(Of Type)(Scaler.GetFilters)

        CheckedListBox1.Items.Clear()

        For i As Integer = 0 To filters.Length - 1
            CheckedListBox1.Items.Add(filters(i), flags.ElementAtOrDefault(i, [default]:=True))
            fill_scalers.Remove(filters(i).GetType)
        Next

        For Each type As Type In fill_scalers
            CheckedListBox1.Items.Add(Activator.CreateInstance(type), False)
        Next

        Call SetPipelineText()
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        Dim i As Integer = CheckedListBox1.SelectedIndex

        If i > -1 Then
            CheckedListBox1.SetItemChecked(i, Not CheckedListBox1.GetItemChecked(i))
        End If

        Call SetPipelineText()
    End Sub

    Private Sub CheckedListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CheckedListBox1.SelectedIndexChanged
        PropertyGrid1.SelectedObject = CheckedListBox1.SelectedItem
        PropertyGrid1.Refresh()
    End Sub

    Private Sub PropertyGrid1_PropertyValueChanged(s As Object, e As PropertyValueChangedEventArgs) Handles PropertyGrid1.PropertyValueChanged
        If Not into.IsNullOrEmpty Then
            Call PlotHist()
        End If

        Call SetPipelineText()
    End Sub

    Private Sub MoveUpToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoveUpToolStripMenuItem.Click
        Dim i As Integer = CheckedListBox1.SelectedIndex

        If i <= 0 Then
            Return
        End If

        Call Swap(i, j:=i - 1)
        Call SetPipelineText()
    End Sub

    Private Sub SetPipelineText()
        txtPipelineText.Text = GetFilter.Select(Function(i) i.ToScript).JoinBy(vbCrLf)
    End Sub

    Private Sub Swap(i As Integer, j As Integer)
        Dim jo = CheckedListBox1.Items(j)
        Dim jf = CheckedListBox1.GetItemChecked(j)
        Dim io = CheckedListBox1.Items(i)
        Dim [if] = CheckedListBox1.GetItemChecked(i)

        CheckedListBox1.Items(j) = io
        CheckedListBox1.Items(i) = jo
        CheckedListBox1.SetItemChecked(i, jf)
        CheckedListBox1.SetItemChecked(j, [if])

        CheckedListBox1.SelectedIndex = j
    End Sub

    Private Sub MoveDownToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoveDownToolStripMenuItem.Click
        Dim i As Integer = CheckedListBox1.SelectedIndex

        If i < 0 Then
            Return
        End If

        Call Swap(i, j:=i + 1)
        Call SetPipelineText()
    End Sub

    Private Sub RefreshToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem.Click
        Call PlotHist()
    End Sub

    Private Sub CheckedListBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles CheckedListBox1.MouseMove
        showTooltip("Check/uncheck the item for enable/disable corresponding filter, move up/down item to construct different pipeline")
    End Sub

    Private Sub PropertyGrid1_Click(sender As Object, e As EventArgs) Handles PropertyGrid1.Click

    End Sub

    Private Sub PropertyGrid1_MouseMove(sender As Object, e As MouseEventArgs) Handles PropertyGrid1.MouseMove
        showTooltip("Modify the filter parameter at here")
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub showTooltip(txt As String)
        TextBox1.Text = txt
    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        showTooltip("The pixel intensity histogram")
    End Sub

    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
        If CheckedListBox1.SelectedItem Is Nothing Then
            Return
        End If

        Call Clipboard.SetText(CheckedListBox1.SelectedItem.ToString)
    End Sub
End Class