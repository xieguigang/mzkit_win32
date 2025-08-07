Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology.HEMap
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports SMRUCC.genomics.Analysis.Spatial.Imaging
Imports std = System.Math

Public Class HEStainViewer

    Dim MSIMatrix As MsImaging.PixelData()
    Dim MSIDims As Size

    ''' <summary>
    ''' the raw size of the <see cref="HEBitmap"/>
    ''' </summary>
    Dim HEImageSize As Size
    Dim HEBitmap As Bitmap
    Dim MSIBitmap As Bitmap

    Private Sub HEStainViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.DoubleBuffered = True
    End Sub

    Public Function LoadRawData(Of PixelData As Pixel)(MSI As IEnumerable(Of PixelData), msi_dims As Size, HEstain As Image) As SpatialTile
        Me.MSIMatrix = MSI.Select(Function(p) New MsImaging.PixelData(p.X, p.Y, p.Scale)).ToArray
        Me.MSIDims = msi_dims
        Me.HEImageSize = HEstain.Size
        Me.HEBitmap = New Bitmap(HEstain)

        Call RenderMSI()
        Call RefreshUI()
    End Function

    Private Sub RenderMSI()
        Dim heatmap As New Blender.PixelRender(False)
        Dim args As New HeatMapParameters(ScalerPalette.turbo) With {.alpha = 80}
        Dim msi As Image = heatmap.RenderPixels(MSIMatrix, MSIDims, New HeatMapBrushes(args)).AsGDIImage

        MSIBitmap = msi
    End Sub

    Private Sub RefreshUI()

    End Sub

    Public Sub SaveExport()
        Using file As New SaveFileDialog With {.Filter = "HEstain spatial register matrix(*.cdf)|*.cdf"}
            If file.ShowDialog = DialogResult.OK Then
                Call TaskProgress.RunAction(
                    Sub(p As ITaskProgress)
                        Call p.SetProgressMode()
                        Call p.SetProgress(0)
                        Call Me.Invoke(Sub() Call ExportMatrixFile(file.FileName, p))
                    End Sub, title:="Export HE stain register matrix", info:="Processing pixel spot mapping...")
            End If
        End Using
    End Sub

    Private Sub ExportMatrixFile(filepath As String, p As ITaskProgress)
        Dim register As New SpatialRegister
        Dim file As Stream = filepath.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)

        Call p.SetInfo("Export register matrix to file...")
        Call register.Save(file)
    End Sub
End Class
