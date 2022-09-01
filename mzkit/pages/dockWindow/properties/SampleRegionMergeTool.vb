Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Linq

Public Class SampleRegionMergeTool

    Dim rawRegions As New Dictionary(Of String, TissueRegion)
    Dim newRegions As New Dictionary(Of String, TissueRegion)
    Dim Rplot As Image

    Public Sub LoadRegions(regions As TissueRegion())
        Dim dims As New Size With {
            .Width = regions.Select(Function(a) a.points.Select(Function(i) i.X)).IteratesALL.Max,
            .Height = regions.Select(Function(a) a.points.Select(Function(i) i.Y)).IteratesALL.Max
        }
        Dim Rplot As Image = LayerRender.Draw(regions, dims, alphaLevel:=1, dotSize:=3)

        For Each r As TissueRegion In regions
            regions(r.label) = r
            newRegions(r.label) = r
        Next

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
        Else
            label = $"{region.ToString}"
        End If

        Label1.Text = $"[{px},{py}] {label}"
    End Sub
End Class