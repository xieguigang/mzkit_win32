Imports Microsoft.VisualBasic.Imaging.Math2D

Public Class RegionSampleCard

    Public ReadOnly Property SampleColor As Color
        Get
            Return PictureBox1.BackColor
        End Get
    End Property

    ''' <summary>
    ''' the name of the sample group
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property SampleInfo As String
        Get
            Return TextBox1.Text
        End Get
    End Property

    Dim regions As Polygon2D()

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click
        Using color As New ColorDialog
            If color.ShowDialog = DialogResult.OK Then
                PictureBox1.BackColor = color.Color
            End If
        End Using
    End Sub

    Public Sub SetPolygons(polygons As IEnumerable(Of Polygon2D))
        regions = polygons.ToArray
    End Sub
End Class
