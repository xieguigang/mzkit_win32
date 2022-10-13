Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq

Public Class SampleRegionMergeTool

    Dim rawRegions As New Dictionary(Of String, TissueRegion)
    Dim newRegions As New Dictionary(Of String, TissueRegion)
    Dim Rplot As Image
    Dim dims As Size

    Public Function GetMergedRegions() As TissueRegion()
        Return newRegions.Values.ToArray
    End Function

    Public Sub LoadRegions(regions As TissueRegion(), dims As Size)
        For Each r As TissueRegion In regions
            rawRegions(r.label) = r
            newRegions(r.label) = r
            ColorComboBox1.Items.Add(r)
        Next

        Me.dims = dims
        Me.Rendering(regions)
    End Sub

    Private Sub Rendering(regions As TissueRegion())
        Dim highlights As String() = Nothing

        If ColorComboBox1.SelectedIndex > 0 Then
            highlights = New String() {
                DirectCast(ColorComboBox1.SelectedItem, TissueRegion).label
            }
        End If
        If dims.IsEmpty Then
            With regions _
                .Select(Function(p) p.points) _
                .IteratesALL _
                .ToArray

                dims = New Size(.X.Max, .Y.Max)
            End With
        End If

        Dim Rplot As Image = LayerRender.Draw(
            regions:=regions,
            layerSize:=dims,
            alphaLevel:=1,
            dotSize:=1,
            highlights:=highlights
        )

        Me.Rplot = Rplot
        Me.PictureBox1.BackgroundImage = Rplot
    End Sub

    ''' <summary>
    ''' transform scaler
    ''' </summary>
    ''' <param name="e"></param>
    ''' <param name="xpoint"></param>
    ''' <param name="ypoint"></param>
    Private Sub getPoint(e As Point, orginal_imageSize As Size, ByRef xpoint As Integer, ByRef ypoint As Integer)
        Dim Pic_width = orginal_imageSize.Width / PictureBox1.Width
        Dim Pic_height = orginal_imageSize.Height / PictureBox1.Height

        ' 得到图片上的坐标点
        xpoint = e.X * Pic_width
        ypoint = e.Y * Pic_height
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.DialogResult = DialogResult.Cancel
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub SampleRegionMergeTool_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckedListBox1.Items.Clear()
        CheckedListBox1.CheckOnClick = True

        ColorComboBox1.getColor = Function(o) DirectCast(o, TissueRegion).color
        ColorComboBox1.getLabel = Function(o) o.ToString

        Clear()
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub PictureBox1_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseMove
        Dim px As Integer
        Dim py As Integer

        Call getPoint(New Point(e.X, e.Y), Rplot.Size, px, py)

        Dim region As TissueRegion = newRegions.Values _
            .Where(Function(r) r.IsInside(px, py)) _
            .FirstOrDefault
        Dim label As String

        If region Is Nothing Then
            label = "No tissue region."
            PictureBox3.BackColor = Color.Black
        Else
            label = $"{region.ToString}"
            PictureBox3.BackColor = region.color
        End If

        Label1.Text = $"[{px},{py}] {label}"
    End Sub

    ''' <summary>
    ''' add new region
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        If CheckedListBox1.Items.Count > 0 Then
            If MessageBox.Show("There are unmerge region list, still going to create a new region?", "Create New Region", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) = DialogResult.Cancel Then
                Return
            End If
        End If

        Call Clear()
    End Sub

    ''' <summary>
    ''' clear
    ''' </summary>
    Private Sub Clear() Handles LinkLabel3.LinkClicked
        CheckedListBox1.Items.Clear()
        TextBox1.Clear()
        PictureBox2.BackColor = Color.Black

        ColorComboBox1.Items.Clear()

        For Each region As TissueRegion In newRegions.Values
            Call ColorComboBox1.Items.Add(region)
        Next
    End Sub

    ''' <summary>
    ''' merge
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Dim color As Color = PictureBox2.BackColor
        Dim label As String = TextBox1.Text
        Dim subregions As New List(Of TissueRegion)

        If label.StringEmpty Then
            MessageBox.Show("region label can not be empty!", "Merge Region", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        ElseIf CheckedListBox1.CheckedItems.Count = 0 Then
            MessageBox.Show("No sub-region is selected!", "Merge Region", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        For i As Integer = 0 To CheckedListBox1.CheckedItems.Count - 1
            subregions.Add(CheckedListBox1.CheckedItems.Item(i))
            newRegions.Remove(subregions.Last.label)
        Next

        newRegions(label) = New TissueRegion With {
            .label = label,
            .color = color,
            .points = subregions _
                .Select(Function(a) a.points) _
                .IteratesALL _
                .ToArray
        }

        Call Clear()
        Call Rendering(newRegions.Values.ToArray)
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        Using colorDiag As New ColorDialog
            If colorDiag.ShowDialog = DialogResult.OK Then
                PictureBox2.BackColor = colorDiag.Color
            End If
        End Using
    End Sub

    Dim pointTo As Point

    Private Sub PictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseDown
        pointTo = PictureBox1.PointToClient(MousePosition)
    End Sub

    Private Sub AddSubRegionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddSubRegionToolStripMenuItem.Click
        Dim p As Point = pointTo
        Dim px As Integer
        Dim py As Integer

        Call getPoint(p, Rplot.Size, px, py)

        Dim region As TissueRegion = newRegions.Values _
            .Where(Function(r) r.IsInside(px, py)) _
            .FirstOrDefault

        If region Is Nothing Then
            Label1.Text = $"No tissue subregion at here!"
            Return
        ElseIf CheckedListBox1.Items.IndexOf(region) = -1 Then
            CheckedListBox1.Items.Add(region)
            CheckedListBox1.SetItemChecked(CheckedListBox1.Items.Count - 1, True)
            Label1.Text = $"Add subregion {region}!"
        Else
            Label1.Text = $"Subregion {region} is already been added."
        End If
    End Sub

    Private Sub PictureBox2_MouseMove(sender As Object, e As MouseEventArgs) Handles PictureBox2.MouseMove
        Label1.Text = "Click here to change new region color"
    End Sub

    Private Sub ColorComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ColorComboBox1.SelectedIndexChanged
        If ColorComboBox1.SelectedIndex < 0 Then
            Return
        End If

        Call Rendering(newRegions.Values.ToArray)
    End Sub
End Class