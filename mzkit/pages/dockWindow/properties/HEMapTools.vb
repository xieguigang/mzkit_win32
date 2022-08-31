Imports WeifenLuo.WinFormsUI.Docking
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology

Public Class HEMapTools

    Dim polygons As New List(Of Polygon2D)
    Dim hemap As Bitmap
    Dim heatmap As Cell()

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

    Private Sub HEMapTools_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TabText = "HEMap Tools"
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
        GroupBox1.Visible = True
    End Sub

    Private Sub ColorComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ColorComboBox1.SelectedIndexChanged
        If ColorComboBox1.SelectedIndex > -1 Then
            Dim color As Color = ColorComboBox1.SelectedItem

        End If
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
        WindowModules.viewer.TogglePolygonMode()
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
        End If

        Dim grid As Cell() = RscriptProgressTask.ScanBitmap(hemap, colors)

        If grid.IsNullOrEmpty Then
            MyApplication.host.warning("Heatmap scanning task error!")
        Else
            heatmap = grid
        End If
    End Sub
End Class