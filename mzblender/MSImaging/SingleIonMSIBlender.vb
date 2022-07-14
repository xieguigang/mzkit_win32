Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.mzkit_win32.My
Imports ControlLibrary
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Serialization.JSON
Imports mzblender
Imports Task
Imports WeifenLuo.WinFormsUI.Docking
Imports File = Microsoft.VisualBasic.Data.csv.IO.File
Imports stdNum = System.Math

Public Class SingleIonMSIBlender : Inherits Blender

    ReadOnly layer As SingleIonLayer
    ReadOnly params As MsImageProperty

    Public ReadOnly Property range As DoubleRange

    Sub New(layer As PixelData(), params As MsImageProperty)
        Me.layer = New SingleIonLayer With {.MSILayer = layer}
        Me.params = params
        Me.range = layer.Select(Function(i) i.intensity).Range
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim dimensionSize As New Size(params.scan_x, params.scan_y)
        Dim size As String = "2,2"
        Dim pixels As PixelData() = layer.MSILayer
        Dim pixelFilter As PixelData()

        pixelFilter = MsImaging.Drawer.ScalePixels(pixels, params.GetTolerance, cut:={0, 1})
        pixelFilter = MsImaging.Drawer.GetPixelsMatrix(pixelFilter)

        Dim range As DoubleRange =
        Dim drawer As New PixelRender(heatmapRender:=False)
        Dim image As Image = Drawer.RenderPixels(
            pixels:=pixelFilter,
            dimension:=dimensionSize,
            dimSize:=size.SizeParser,
            mapLevels:=params.mapLevels,
            colorSet:=params.colors.Description,
            scale:=params.scale
        ).AsGDIImage

        Return image
    End Function
End Class
