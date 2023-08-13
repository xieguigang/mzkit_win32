Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
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

        For i As Integer = 0 To CheckedListBox1.Items.Count - 1
            If TypeOf CheckedListBox1.Items(i) Is TrIQScaler Then
                into = DirectCast(CheckedListBox1.Items(i), TrIQScaler).DoIntensityScale(into)
            End If
        Next

        Dim axis As DoubleRange = into.CreateAxisTicks
        Dim canvas As Size = PictureBox1.Size.Scale(5)

        PictureBox1.BackgroundImage = into.HistogramPlot(
                  [step]:=(axis.Max - axis.Min) / 100,
                  size:=$"{canvas.Width},{canvas.Height}",
                  padding:="padding: 50px 50px 100px 100px;",
                  showGrid:=False) _
            .AsGDIImage
    End Sub

    Public Sub ConfigPipeline(filters As Scaler(), Optional flags As Boolean() = Nothing)
        CheckedListBox1.Items.Clear()

        For i As Integer = 0 To filters.Length - 1
            CheckedListBox1.Items.Add(filters(i), flags.ElementAtOrDefault(i, [default]:=True))
        Next
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click
        Dim i As Integer = CheckedListBox1.SelectedIndex

        If i > -1 Then
            CheckedListBox1.SetItemChecked(i, Not CheckedListBox1.GetItemChecked(i))
        End If
    End Sub

    Private Sub CheckedListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CheckedListBox1.SelectedIndexChanged
        PropertyGrid1.SelectedObject = CheckedListBox1.SelectedItem
        PropertyGrid1.Refresh()
    End Sub

    Private Sub PropertyGrid1_PropertyValueChanged(s As Object, e As PropertyValueChangedEventArgs) Handles PropertyGrid1.PropertyValueChanged
        If Not into.IsNullOrEmpty Then
            Call PlotHist()
        End If
    End Sub
End Class