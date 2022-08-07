Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Imaging.Math2D

Public Class RegionSampleCard

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
            Return tissue
        End If

        Dim x As New List(Of Integer)
        Dim y As New List(Of Integer)

        For i As Integer = 1 To dimension.Width
            For j As Integer = 1 To dimension.Height
#Disable Warning
                If regions.Any(Function(r) r.inside(i, j)) Then
                    x.Add(i)
                    y.Add(j)
                End If
#Enable Warning
            Next
        Next

        Return New TissueRegion With {
            .color = SampleColor,
            .label = SampleInfo,
            .points = x _
                .Select(Function(xi, i)
                            Return New Point(xi, y(i))
                        End Function) _
                .ToArray
        }
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
End Class
