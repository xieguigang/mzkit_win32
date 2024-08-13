Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology.HEMap
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports RibbonLib.Interop

Public Class frmHEStainAnalysisTool

    Public Sub LoadRawData(MSI As PixelScanIntensity(), dims As Size, HEstain As Image)
        Call frmHEStainAnalysisTool_Activated()
        Call HeStainViewer1.LoadRawData(MSI.Select(Function(m) New PixelData(m, m.totalIon)), dims, HEstain)
        Call ApplyVsTheme(HeStainViewer1.GetMenu)
    End Sub

    Private Sub frmHEStainAnalysisTool_Activated() Handles Me.Activated
        ribbonItems.MenuHeStainTools.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub frmHEStainAnalysisTool_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ribbonItems.MenuHeStainTools.ContextAvailable = ContextAvailability.NotAvailable
    End Sub

    Private Sub frmHEStainAnalysisTool_Load(sender As Object, e As EventArgs) Handles Me.Load
        AddHandler ribbonItems.ButtonHEstainEditLabel.ExecuteEvent, Sub() Call HeStainViewer1.EditLabel()
        AddHandler ribbonItems.ButtonHEstainRotate.ExecuteEvent, Sub() Call HeStainViewer1.Rotate()
        AddHandler ribbonItems.ButtonHEstainSave.ExecuteEvent, Sub() Call HeStainViewer1.SaveExport()
        AddHandler ribbonItems.ButtonHEstainSetSpotColor.ExecuteEvent, Sub() Call HeStainViewer1.SetSpotColor()
        AddHandler ribbonItems.ButtonSpatialTileUI.ExecuteEvent, Sub() Call HeStainViewer1.UpdateSpatialTileUI()
        AddHandler ribbonItems.CheckboxHEstainKeepsAspectRatio.ExecuteEvent,
            Sub()
                HeStainViewer1.KeepAspectRatio = ribbonItems.CheckboxHEstainKeepsAspectRatio.BooleanValue
            End Sub
    End Sub

    ''' <summary>
    ''' debug test
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub HeStainViewer1_DragDrop(sender As Object, e As DragEventArgs) Handles HeStainViewer1.DragDrop
        Dim files() As String = e.Data.GetData(DataFormats.FileDrop)
        Dim firstFile As String = files.ElementAtOrDefault(Scan0)

        If Not firstFile Is Nothing Then
            If firstFile.ExtensionSuffix("cdf") Then

                Call HeStainViewer1.Clear()

                Dim register As SpatialRegister = SpatialRegister.ParseFile(firstFile.OpenReadonly)
                Dim pixels As PixelData() = register.mappings _
                    .Select(Function(p) New PixelData(p.STX, p.STY, p.heatmap)) _
                    .ToArray
                Dim msi_dims As Size = register.MSIdims
                Dim tile = HeStainViewer1.LoadRawData(pixels, msi_dims, register.HEstain)

                tile.Location = register.offset.ToPoint
                tile.Size = register.MSIscale
                tile.Label = register.label
                tile.SpotColor = register.spotColor.TranslateColor
                tile.CanvasOnPaintBackground()
            End If
        End If
    End Sub

    Private Sub HeStainViewer1_DragEnter(sender As Object, e As DragEventArgs) Handles HeStainViewer1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub
End Class