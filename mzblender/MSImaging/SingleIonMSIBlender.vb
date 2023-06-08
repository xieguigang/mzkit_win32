#Region "Microsoft.VisualBasic::434bd1bc910e4973b6ef264f49501733, mzkit\src\mzkit\mzblender\MSImaging\SingleIonMSIBlender.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 86
'    Code Lines: 60
' Comment Lines: 13
'   Blank Lines: 13
'     File Size: 3.37 KB


' Class SingleIonMSIBlender
' 
'     Properties: dimensionSize, range
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: Rendering
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Task

Public Class SingleIonMSIBlender : Inherits MSImagingBlender

    ReadOnly layer As SingleIonLayer
    ''' <summary>
    ''' the raw intensity values
    ''' </summary>
    ReadOnly intensity As Double()
    ReadOnly TIC As Image

    Dim plotRange As DoubleRange

    Public ReadOnly Property range As DoubleRange
    Public ReadOnly Property dimensionSize As Size
        Get
            Return New Size(params.scan_x, params.scan_y)
        End Get
    End Property

    Sub New(layer As PixelData(), TIC As PixelScanIntensity(), params As MsImageProperty)
        Call Me.New(layer, params, SummaryMSIBlender.Rendering(TIC, New Size(params.scan_x, params.scan_y), "gray", 255))
    End Sub

    Sub New(layer As PixelData(), params As MsImageProperty, Optional TIC As Image = Nothing)
        Call MyBase.New(params)

        Me.layer = New SingleIonLayer With {
            .MSILayer = layer,
            .DimensionSize = New Size(
                width:=params.scan_x,
                height:=params.scan_y
            )
        }
        Me.TIC = TIC
        Me.intensity = layer.Select(Function(i) i.intensity).ToArray
        Me.range = intensity.Range
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim pixels As PixelData() = TakePixels(layer.MSILayer)
        ' denoise_scale() > TrIQ_scale(0.8) > knn_scale() > soften_scale()
        Dim filter As RasterPipeline = New DenoiseScaler() _
            .Then(New TrIQScaler(params.TrIQ)) _
            .Then(New KNNScaler(params.knn, params.knn_qcut)) _
            .Then(New SoftenScaler())

        If Not plotRange Is Nothing Then
            pixels = pixels _
                .Select(Function(p)
                            Dim into As Double = p.intensity

                            If into > plotRange.Max Then
                                into = plotRange.Max
                            ElseIf into < plotRange.Min Then
                                into = plotRange.Min
                            End If

                            Return New PixelData(p.x, p.y, into)
                        End Function) _
                .ToArray
        End If

        Dim pixelFilter As New SingleIonLayer With {
            .DimensionSize = layer.DimensionSize,
            .IonMz = -1,
            .MSILayer = pixels
        }

        If params.enableFilter Then
            ' pixelFilter = MsImaging.Drawer.ScalePixels(pixels, params.GetTolerance, cut:={0, cut})
            pixelFilter = filter(pixelFilter)
        End If

        Dim background As Image = If(params.showTotalIonOverlap, TIC, Nothing)
        Dim drawer As New PixelRender(heatmapRender:=False, overlaps:=background)
        Dim image As Image = drawer.RenderPixels(
            pixels:=MsImaging.Drawer.GetPixelsMatrix(pixelFilter),
            dimension:=dimensionSize,
            mapLevels:=params.mapLevels,
            colorSet:=params.colors.Description,
            scale:=params.scale
        ).AsGDIImage

        image = New HeatMap.RasterScaler(image).Scale(hqx:=params.Hqx)

        If params.showPhysicalRuler Then
            Call New Ruler(args.GetTheme).DrawOnImage(image, dimensionSize, Color.White, params.resolution)
        End If

        Return image
    End Function

    Public Overrides Sub SetIntensityRange(normRange As DoubleRange)
        Dim std As New DoubleRange(0, 1)
        Dim min As Double = std.ScaleMapping(normRange.Min, range)
        Dim max As Double = std.ScaleMapping(normRange.Max, range)

        plotRange = New DoubleRange(min, max)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetTrIQIntensity(q As Double) As Double
        Return TrIQ.FindThreshold(intensity, q)
    End Function
End Class
