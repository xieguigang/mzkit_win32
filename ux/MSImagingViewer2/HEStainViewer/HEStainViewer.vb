Imports System.Drawing
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap

Public Class HEStainViewer

    Dim MSIMatrix As PixelData()
    Dim MSIDims As Size
    Dim HEImage As Size

    Public Sub LoadRawData(MSI As IEnumerable(Of PixelData), msi_dims As Size, HEstain As Image)
        Me.MSIMatrix = MSI.ToArray
        Me.MSIDims = msi_dims
        Me.HEImage = HEstain.Size
        Me.BackgroundImage = HEstain
        Me.Refresh()
    End Sub

End Class
