Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq

Public Class frmPeakFinding

    Dim matrix As ChromatogramTick()
    Dim rawName As String

    Public Sub LoadMatrix(title As String, data As IEnumerable(Of ChromatogramTick))
        Me.matrix = data.ToArray
        Me.rawName = title

        Call InitPanel()
    End Sub

    Public Sub InitPanel()
        Dim size As Size = PictureBox1.Size
        Dim plot As Image = {
            New NamedCollection(Of ChromatogramTick)(rawName, matrix)
        } _
            .TICplot(
                intensityMax:=0,
                isXIC:=True,
                colorsSchema:=Globals.GetColors,
                fillCurve:=Globals.Settings.viewer.fill,
                gridFill:="white"
            ).AsGDIImage

        PictureBox1.BackgroundImage = plot
    End Sub

End Class