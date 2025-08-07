Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology.HEMap
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Mzkit_win32.BasicMDIForm
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

    Dim HE_ratio_w As Single = 1
    Dim HE_ratio_h As Single = 1
    Dim HE_rotate As Single = 0
    Dim HE_move_x As Single = 0
    Dim HE_move_y As Single = 0

    Public Function GetMenu() As ToolStrip()
        Return {}
    End Function

    Private Sub HEStainViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.DoubleBuffered = True
    End Sub

    Public Function LoadRawData(Of PixelData As Pixel)(MSI As IEnumerable(Of PixelData), msi_dims As Size, HEstain As Image) As SpatialTile
        Me.MSIMatrix = MSI.Select(Function(p) New MsImaging.PixelData(p.X, p.Y, p.Scale)).ToArray
        Me.MSIDims = msi_dims
        Me.HEImageSize = HEstain.Size
        Me.HEBitmap = New Bitmap(HEstain)

        Call ResetRegister()
        Call RenderMSI()
        Call RefreshUI()
    End Function

    Private Sub ResetRegister()
        HE_move_x = 0
        HE_move_y = 0
        HE_ratio_h = 1
        HE_ratio_w = 1
        HE_rotate = 0
    End Sub

    Private Sub RenderMSI()
        Dim heatmap As New Blender.PixelRender(False)
        Dim args As New HeatMapParameters(ScalerPalette.turbo) With {.alpha = 80}
        Dim msi As Image = heatmap.RenderPixels(MSIMatrix, MSIDims, New HeatMapBrushes(args)).AsGDIImage

        MSIBitmap = msi
    End Sub

    Private Sub RefreshUI()
        Dim size As Size = Me.Size.Scale(5)
        Dim bg As New Bitmap(size.Width, size.Height)

        Using gfx As IGraphics = Graphics2D.Open(bg)
            ' draw hemap

            ' draw msi
            ' msi always keeps in center
            ' 计算缩放比例（限制最小值为 0.1）
            Dim scaleX As Single = std.Max(CSng(size.Width) / MSIDims.Width, 0.1F)
            Dim scaleY As Single = std.Max(CSng(size.Height) / MSIDims.Height, 0.1F)
            Dim scale As Single = std.Min(scaleX, scaleY) ' 取较小值保证内容完整显示

            ' 应用缩放后的尺寸
            Dim newWidth As Integer = CInt(MSIDims.Width * scale)
            Dim newHeight As Integer = CInt(MSIDims.Height * scale)

            ' 计算居中位置
            Dim centerX As Integer = (size.Width - newWidth) \ 2
            Dim centerY As Integer = (size.Height - newHeight) \ 2

            Call gfx.DrawImage(MSIBitmap, centerX, centerY, newWidth, newHeight)
        End Using

        BackgroundImage = bg
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
