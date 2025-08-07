Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology.HEMap
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports SMRUCC.genomics.Analysis.Spatial.Imaging
Imports std = System.Math

Public Class HEStainViewer

    Dim MSIMatrix As PixelData()
    Dim MSIDims As Size

    ''' <summary>
    ''' the raw size of the <see cref="HEBitmap"/>
    ''' </summary>
    Dim HEImageSize As Size
    Dim HEBitmap As Bitmap

    Private Sub HEStainViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.DoubleBuffered = True
    End Sub

    Public Function LoadRawData(MSI As IEnumerable(Of PixelData), msi_dims As Size, HEstain As Image) As SpatialTile
        Me.MSIMatrix = MSI.ToArray
        Me.MSIDims = msi_dims
        Me.HEImageSize = HEstain.Size
        Me.HEBitmap = New Bitmap(HEstain)


    End Function



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
