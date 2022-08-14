Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Imaging.Math2D

Public Class RegionSampleCard

    Public Event RemoveSampleGroup(card As RegionSampleCard)

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

    Dim regions As Polygon2D()
    Dim updateCallback As Action
    Dim tissue As TissueRegion

    Public Function ExportTissueRegion(dimension As Size) As TissueRegion
        If Not tissue Is Nothing Then
            Return New TissueRegion With {
                .color = SampleColor,
                .label = SampleInfo,
                .points = tissue.points
            }
        Else
            Return regions.Geometry2D(dimension, SampleInfo, SampleColor)
        End If
    End Function

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Using color As New ColorDialog
            If color.ShowDialog = DialogResult.OK Then
                PictureBox1.BackColor = color.Color
                updateCallback()
            End If
        End Using
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
        updateCallback()
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        If MessageBox.Show(text:="Going to remove current sample?",
                           caption:="Group Sample",
                           buttons:=MessageBoxButtons.OKCancel,
                           icon:=MessageBoxIcon.Question) = DialogResult.OK Then

            RaiseEvent RemoveSampleGroup(Me)
        End If
    End Sub
End Class
