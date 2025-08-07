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

    Public Property ViewMargin As Integer = 25

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
        ' 获取逻辑尺寸（控件实际大小）
        Dim logicalSize As Size = Me.ClientSize
        ' size = (width * scale,
        '         height* scale)
        Dim physicalSize As Size = logicalSize.Scale(5)
        Dim bg As New Bitmap(physicalSize.Width, physicalSize.Height)

        Using gfx As IGraphics = Graphics2D.Open(bg)
            Call gfx.Clear(Color.Black)

            ' draw hemap

            ' draw msi
            ' msi always keeps in center
            ' 计算逻辑可用区域（考虑边距）
            Dim logicalAvailableWidth As Integer = std.Max(logicalSize.Width - 2 * ViewMargin, 1)
            Dim logicalAvailableHeight As Integer = std.Max(logicalSize.Height - 2 * ViewMargin, 1)

            ' 计算缩放比例（取宽高比例的最小值）
            Dim scaleX As Single = CSng(logicalAvailableWidth) / MSIDims.Width
            Dim scaleY As Single = CSng(logicalAvailableHeight) / MSIDims.Height
            Dim scale As Single = std.Min(scaleX, scaleY)
            scale = std.Max(scale, 0.1F)  ' 设置最小缩放限制

            ' 计算缩放后的逻辑尺寸
            Dim logicalNewWidth As Integer = CInt(MSIDims.Width * scale)
            Dim logicalNewHeight As Integer = CInt(MSIDims.Height * scale)

            ' 计算居中位置（使用浮点计算避免取整误差）
            Dim logicalCenterX As Single = ViewMargin + (logicalAvailableWidth - logicalNewWidth) / 2.0F
            Dim logicalCenterY As Single = ViewMargin + (logicalAvailableHeight - logicalNewHeight) / 2.0F

            ' 转换为物理坐标（放大5倍）
            Dim physicalCenterX As Integer = CInt(logicalCenterX * 5)
            Dim physicalCenterY As Integer = CInt(logicalCenterY * 5)
            Dim physicalNewWidth As Integer = CInt(logicalNewWidth * 5)
            Dim physicalNewHeight As Integer = CInt(logicalNewHeight * 5)

            ' 绘制图像（使用高质量插值保持清晰度）
            gfx.DrawImage(MSIBitmap, physicalCenterX, physicalCenterY, physicalNewWidth, physicalNewHeight)
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
