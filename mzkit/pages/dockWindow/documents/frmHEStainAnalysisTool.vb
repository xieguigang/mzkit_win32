Imports System.ComponentModel
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
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
End Class