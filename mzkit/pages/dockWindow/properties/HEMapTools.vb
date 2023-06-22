Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Serialization.JSON
Imports TaskStream
Imports WeifenLuo.WinFormsUI.Docking

Public Class HEMapTools

    Dim polygons As New List(Of Polygon2D)
    Dim hemap As Bitmap
    Dim heatmap As Cell()
    Dim heatmap_dims As Size

    Public Sub Add(regions As Polygon2D())
        polygons.AddRange(regions)
        TextBox1.Text = $"mark {polygons.Count} polygon regions"
    End Sub

    Public Sub Clear(newMap As Bitmap)
        Call ColorComboBox1.Items.Clear()
        Call polygons.Clear()
        Call hideBox()

        hemap = newMap
    End Sub

    Public Overrides Function GetMainToolStrip() As ToolStrip
        Return ToolStrip1
    End Function

    Private Sub HEMapTools_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TabText = "HEMap Tools"
        ApplyVsTheme(ToolStrip1)
        hideBox()
    End Sub

    ''' <summary>
    ''' add new color channel
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        DockState = DockState.DockRight
        MyApplication.host.showStatusMessage("Select a new color channel from the image viewer!")
        PictureBox1.BackColor = Color.Transparent
        WindowModules.viewer.DockState = DockState.Document
        WindowModules.viewer.clickPixel =
            Sub(x, y, pixel)
                PictureBox1.BackColor = pixel
            End Sub
        closePolygonEditor()
        GroupBox1.Visible = True
    End Sub

    Private Sub ColorComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ColorComboBox1.SelectedIndexChanged
        If ColorComboBox1.SelectedIndex > -1 AndAlso Not heatmap.IsNullOrEmpty Then
            Dim color As Color = ColorComboBox1.SelectedItem
            Dim key As String = color.ToHtmlColor
            Dim layer As PixelData() = heatmap.GetHeatMapLayer(, channel:=key)

            If layer.IsNullOrEmpty Then
                Call MyApplication.host.warning($"No heatmap scanning result of '{key}', please run heatmap scanning and then try again...")
            Else
                Call WindowModules.viewer.BlendingHEMap(layer, heatmap_dims)

                Dim pixels = layer.Where(Function(p) p.Scale > 0).ToArray
                Dim rsd As Double = layer.RSD.Average

                TextBox1.Text =
                    $"cells: {pixels.Length}/{layer.Length}" & vbCrLf &
                    $"RSD: {(rsd * 100).ToString("F2")}"
            End If
        End If

        closePolygonEditor()
    End Sub

    ''' <summary>
    ''' view all channels
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        If heatmap.IsNullOrEmpty Then
            Call MyApplication.host.warning("No heatmap layer data was scanned!")
            Return
        End If

        Dim layer As PixelData() = heatmap _
            .Select(Function(p)
                        Return New PixelData(p.ScaleX, p.ScaleY, p.layers.Values.Select(Function(a) a.Density).Max)
                    End Function) _
            .ToArray

        Call WindowModules.viewer.BlendingHEMap(layer, heatmap_dims)

        Dim pixels = layer.Where(Function(p) p.Scale > 0).ToArray
        Dim rsd As Double = layer.RSD.Average

        TextBox1.Text =
            $"cells: {pixels.Length}/{layer.Length}" & vbCrLf &
            $"RSD: {(rsd * 100).ToString("F2")}"
    End Sub

    Private Sub hideBox()
        WindowModules.viewer.clickPixel = Nothing
        GroupBox1.Visible = False
    End Sub

    ''' <summary>
    ''' cancel
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        Call hideBox()
    End Sub

    ''' <summary>
    ''' ok
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Call hideBox()

        If Not PictureBox1.BackColor.IsTransparent Then
            Call ColorComboBox1.Items.Add(PictureBox1.BackColor)
        End If
    End Sub

    ''' <summary>
    ''' set region
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LinkLabel4_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel4.LinkClicked
        WindowModules.viewer.DrawHeMapRegion = True
        RibbonEvents.ribbonItems.ButtonTogglePolygon.BooleanValue = True
        WindowModules.viewer.TogglePolygonMode()
        WindowModules.viewer.StartNewPolygon()
    End Sub

    Private Sub closePolygonEditor()
        RibbonEvents.ribbonItems.ButtonTogglePolygon.BooleanValue = False
        WindowModules.viewer.DrawHeMapRegion = False
        WindowModules.viewer.StartNewPolygon()
        WindowModules.viewer.TogglePolygonMode()
        ribbonItems.ButtonAddNewPolygon.BooleanValue = False
        ' clear content
        WindowModules.viewer.PixelSelector1.MSICanvas.GetPolygons(popAll:=True).ToArray
    End Sub

    ''' <summary>
    ''' exec
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LinkLabel5_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel5.LinkClicked
        Dim colors = ColorComboBox1.Colors.ToArray

        If colors.IsNullOrEmpty Then
            Call MyApplication.host.warning("No color channels!")
            Return
        Else
            Call closePolygonEditor()
        End If

        Dim grid As Cell() = RscriptProgressTask.ScanBitmap(hemap, colors).LoadJSON(Of Cell())(throwEx:=False)

        If grid.IsNullOrEmpty Then
            MyApplication.host.warning("Heatmap scanning task error!")
        Else
            If polygons.IsNullOrEmpty Then
                heatmap = grid
            Else
                heatmap = grid _
                    .Where(Function(i) polygons.Any(Function(r) r.inside(i.X, i.Y))) _
                    .ToArray
            End If

            heatmap_dims = New Size(
                width:=(Aggregate cell In grid Into Max(cell.ScaleX)),
                height:=(Aggregate cell In grid Into Max(cell.ScaleY))
            )
        End If
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Call WindowModules.viewer.loadHEMap()
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        If heatmap.IsNullOrEmpty Then
            Call MyApplication.host.warning($"no heatmap layer data to export!")
            Return
        End If

        Using file As New SaveFileDialog With {.Filter = "Excel Table(*.csv)|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                Call ExportTable(file.FileName)
            End If
        End Using
    End Sub

    Private Sub ExportTable(file As String)
        Using buffer As New StreamWriter(file.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False))
            Dim line As String = "X,Y,ScaleX,ScaleY,R,G,B,black(pixels),black(density),black(ratio)"
            Dim layers = heatmap(Scan0).layers.Keys.ToArray

            If layers.Count > 0 Then
                line = line &
                    ",all(pixels),all(density),all(ratio)," &
                    layers.Select(Function(t) $"{t}(pixels),{t}(density),{t}(ratio)").JoinBy(",")
            End If

            Call buffer.WriteLine(line)

            For Each cell As Cell In heatmap
                line = {
                    cell.X, cell.Y, cell.ScaleX, cell.ScaleY,
                    cell.R, cell.G, cell.B,
                    cell.Black.Pixels, cell.Black.Density, cell.Black.Ratio
                }.JoinBy(",")

                If layers.Count > 0 Then
                    Dim allPixels = cell.layers.Values.Select(Function(a) a.Pixels).Max
                    Dim allDensity = cell.layers.Values.Select(Function(a) a.Density).Max
                    Dim allRatio = cell.layers.Values.Select(Function(a) a.Ratio).Max

                    line = line & $",{allPixels},{allDensity},{allRatio}"
                    line = line & "," & layers _
                        .Select(Function(t)
                                    Dim o = cell.layers(t)
                                    Return $"{o.Pixels},{o.Density},{o.Ratio}"
                                End Function) _
                        .JoinBy(",")
                End If

                Call buffer.WriteLine(line)
            Next
        End Using
    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        WindowModules.viewer.PixelSelector1.SetMsImagingOutput(
            WindowModules.viewer.PixelSelector1.MSICanvas.HEMap,
            WindowModules.viewer.PixelSelector1.MSICanvas.HEMap.Size,
            Color.Black,
            Drawing2D.Colors.ScalerPalette.Jet,
            {0, 255},
            120
        )
    End Sub
End Class