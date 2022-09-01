Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Linq

Public Class SampleRegionMergeTool

    Dim rawRegions As New Dictionary(Of String, TissueRegion)
    Dim newRegions As New Dictionary(Of String, TissueRegion)
    Dim Rplot As Image

    Public Sub LoadRegions(regions As TissueRegion())
        For Each r As TissueRegion In regions
            rawRegions(r.label) = r
            newRegions(r.label) = r
        Next

        Call Rendering(regions)
    End Sub

    Private Sub Rendering(regions As TissueRegion())
        Dim dims As New Size With {
            .Width = regions.Select(Function(a) a.points.Select(Function(i) i.X)).IteratesALL.Max,
            .Height = regions.Select(Function(a) a.points.Select(Function(i) i.Y)).IteratesALL.Max
        }
        Dim Rplot As Image = LayerRender.Draw(regions, dims, alphaLevel:=1, dotSize:=1)

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

        CheckedListBox1.Items.Clear()
        TextBox1.Clear()
        PictureBox2.BackColor = Color.Black
    End Sub

    ''' <summary>
    ''' clear
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        CheckedListBox1.Items.Clear()
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

        Call Rendering(newRegions.Values.ToArray)
    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs) Handles PictureBox2.Click
        Using colorDiag As New ColorDialog
            If colorDiag.ShowDialog = DialogResult.OK Then
                PictureBox2.BackColor = colorDiag.Color
            End If
        End Using
    End Sub

    Private Sub AddSubRegionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddSubRegionToolStripMenuItem.Click
        Dim p As Point = PictureBox1.PointToClient(MousePosition)
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
            Label1.Text = $"Add subregion {region}!"
        Else
            Label1.Text = $"Subregion {region} is already been added."
        End If
    End Sub
End Class