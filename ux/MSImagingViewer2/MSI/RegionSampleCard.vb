Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq

Public Class RegionSampleCard

    Public Event RemoveSampleGroup(card As RegionSampleCard)
    Public Event ViewRegionMs1Spectrum(card As RegionSampleCard)
    Public Event SetHtmlColorCode(card As RegionSampleCard)
    Public Event StartMoveRegion(card As RegionSampleCard)

    Public Property SampleColor As Color
        Get
            Return PictureBox1.BackColor
        End Get
        Set(value As Color)
            PictureBox1.BackColor = value
        End Set
    End Property

    ''' <summary>
    ''' the name of the sample group
    ''' </summary>
    ''' <returns></returns>
    Public Property SampleInfo As String
        Get
            Return TextBox1.Text
        End Get
        Set(value As String)
            TextBox1.Text = value
        End Set
    End Property

    Friend regions As Polygon2D()
    Friend updateCallback As Action
    Friend tissue As TissueRegion
    Friend alreadyRaster As Boolean = False

    Public Function PixelPointInside(x As Integer, y As Integer) As Boolean
        If regions.IsNullOrEmpty Then
            If tissue Is Nothing Then
                Return False
            Else
                Return tissue.IsInside(x, y)
            End If
        Else
            For Each r As Polygon2D In regions
                If r.inside(x, y) Then
                    Return True
                End If
            Next

            Return False
        End If
    End Function

    ''' <summary>
    ''' make polygon shape object raster matrix
    ''' </summary>
    ''' <param name="dimension"></param>
    ''' <returns>
    ''' this function always create a new <see cref="TissueRegion"/> object
    ''' </returns>
    Public Function ExportTissueRegion(dimension As Size) As TissueRegion
        If Not tissue Is Nothing Then
            Return New TissueRegion With {
                .color = SampleColor,
                .label = SampleInfo,
                .points = tissue.points
            }
        ElseIf alreadyRaster Then
            Return New TissueRegion With {
                .color = SampleColor,
                .label = SampleInfo,
                .points = regions _
                    .Select(Function(r) r.AsEnumerable.Select(Function(pf) New Point(pf.X, pf.Y))) _
                    .IteratesALL _
                    .GroupBy(Function(pt) $"{pt.X},{pt.Y}") _
                    .Select(Function(pg) pg.First) _
                    .ToArray
            }
        Else
            Return regions.RasterGeometry2D(dimension, SampleInfo, SampleColor)
        End If
    End Function

    Private Sub PictureBox1_MouseClick(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseClick
        If e.Button = MouseButtons.Left Then
            Using color As New ColorDialog
                If color.ShowDialog = DialogResult.OK Then
                    PictureBox1.BackColor = color.Color
                    updateCallback()
                End If
            End Using
        End If
    End Sub

    Public Sub SetPolygons(polygons As TissueRegion, callback As Action)
        Me.tissue = polygons
        Me.updateCallback = callback
    End Sub

    Public Sub SetPolygons(polygons As IEnumerable(Of Polygon2D), callback As Action)
        regions = polygons.ToArray
        TextBox2.Text = regions.Length
        updateCallback = callback
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        ' updateCallback()
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        If MessageBox.Show(text:="Going to remove current sample?",
                           caption:="Group Sample",
                           buttons:=MessageBoxButtons.OKCancel,
                           icon:=MessageBoxIcon.Question) = DialogResult.OK Then

            RaiseEvent RemoveSampleGroup(Me)
        End If
    End Sub

    Private Sub RegionSampleCard_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub RegionSampleCard_Click(sender As Object, e As EventArgs) Handles Me.Click

    End Sub

    Private Sub RegionSampleCard_GotFocus(sender As Object, e As EventArgs) Handles Me.GotFocus
        Me.BorderStyle = BorderStyle.FixedSingle
    End Sub

    Private Sub RegionSampleCard_LostFocus(sender As Object, e As EventArgs) Handles Me.LostFocus
        Me.BorderStyle = BorderStyle.None
    End Sub

    Private Sub ViewMS1SpectrumToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewMS1SpectrumToolStripMenuItem.Click
        RaiseEvent ViewRegionMs1Spectrum(Me)
    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening

    End Sub

    Private Sub SetHTMLColorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetHTMLColorToolStripMenuItem.Click
        RaiseEvent SetHtmlColorCode(Me)
    End Sub

    Private Sub MoveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MoveToolStripMenuItem.Click
        RaiseEvent StartMoveRegion(Me)
    End Sub
End Class
