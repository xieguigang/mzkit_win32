Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports NRRD

Public Class frmMRIViewer : Implements IFileReference

    Public Property FilePath As String Implements IFileReference.FilePath

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {}
        End Get
    End Property

    Dim raster As RasterPointCloud
    Dim cache As New Dictionary(Of String, Image)

    Public Sub LoadRaster(nrrd As NRRD.FileReader)
        Dim raster As RasterObject = nrrd.LoadRaster

        If TypeOf raster Is RasterImage Then
            Me.raster = RasterObject.CastPointCloud(raster)
        Else
            Me.raster = raster
        End If

        Dim dims = raster.dimensionSize

        TrackBar1.Minimum = 0
        TrackBar1.Maximum = dims(2) - 1
        TrackBar1.Value = 0

        Call MoveFrame()
    End Sub

    Private Sub MoveFrame()
        Dim i As Integer = TrackBar1.Value + 1
        Dim key As String = i.ToString

        If Not cache.ContainsKey(key) Then
            cache.Add(key, CreateFrame(i))
        End If

        PictureBox1.BackgroundImage = cache(key)
    End Sub

    Private Function CreateFrame(i As Integer) As Image
        Dim layer As RasterImage = raster.GetRasterImage(i)
        Dim render As New PixelRender(ScalerPalette.turbo.Description, 100, defaultColor:=PictureBox1.BackColor)
        Dim img As Image = render.RenderRasterImage(layer.GetRasterPixels, layer.RawSize)

        Return img
    End Function

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        Call MoveFrame()
    End Sub

    Private Sub TrackBar1_ValueChanged(sender As Object, e As EventArgs) Handles TrackBar1.ValueChanged
        Call MoveFrame()
    End Sub
End Class