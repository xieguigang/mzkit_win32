Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

Public Class InputSelectMassWindow

    Public Shared Sub GetMassWindows(mz As IEnumerable(Of Double), apply As Action(Of MassWindow), cancel As Action)
        Dim histogram = MzBins.GetScatter(mz, 0.005)
        Dim windows = MzBins.GetMzBins(histogram.x, histogram.y).ToArray
        Dim selector As New InputSelectMassWindow
        Dim theme As New Theme With {.drawLegend = False, .padding = "padding: 100px 100px 150px 200px;"}
        Dim plot As New PlotMassWindow(histogram.x, histogram.y, windows, theme)
        Dim size As Size = (selector.PictureBox1.Size)

        size = New Size(size.Width * 5, size.Height * 5)
        selector.PictureBox1.BackgroundImage = plot _
            .Plot(size, dpi:=200, Drivers.GDI) _
            .AsGDIImage

        For Each item As MassWindow In windows
            Call selector.ListBox1.Items.Add(item)
        Next

        Call InputDialog.Input(
            setConfig:=Sub(val)
                           Dim i As Integer = selector.ListBox1.SelectedIndex
                           Dim mass As MassWindow = windows(i)

                           Call apply(mass)
                       End Sub,
            config:=selector,
            cancel:=cancel)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub InputSelectMassWindow_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If ListBox1.Items.Count > 0 Then
            ListBox1.SelectedIndex = 0
        End If
    End Sub
End Class