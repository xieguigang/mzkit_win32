#Region "Microsoft.VisualBasic::d8ffd9045e6fbfc35c97031d815d0a0d, mzkit\src\mzkit\mzblender\MSImaging\SummaryMSIBlender.vb"

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

'   Total Lines: 79
'    Code Lines: 64
' Comment Lines: 0
'   Blank Lines: 15
'     File Size: 3.08 KB


' Class SummaryMSIBlender
' 
'     Properties: dimensions
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: (+2 Overloads) Rendering
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Task

Public Class SummaryMSIBlender : Inherits MSImagingBlender

    ReadOnly summaryLayer As PixelScanIntensity()
    ReadOnly intensity As Double()

    Dim plotRange As DoubleRange

    Public ReadOnly Property dimensions As Size
        Get
            Return New Size(params.scan_x, params.scan_y)
        End Get
    End Property

    Sub New(summaryLayer As PixelScanIntensity(), filter As RasterPipeline)
        Call MyBase.New(filter)

        Me.summaryLayer = summaryLayer
        Me.intensity = summaryLayer _
            .Select(Function(p) p.totalIon) _
            .ToArray
    End Sub

    Public Overloads Function Rendering() As Image
        Dim mapLevels As Integer = params.mapLevels
        Dim layerData As PixelScanIntensity() = summaryLayer
        Dim filter = Me.filters

        If Not params.instrument.TextEquals("Bruker") Then
            filter = filter.Then(New KNNScaler(params.knn, params.knn_qcut)).Then(New SoftenScaler())
        Else
            filter = filter.Then(New SoftenScaler())
        End If

        Dim pixelDatas As BioNovoGene.Analytical.MassSpectrometry.MsImaging.PixelData()

        If Not plotRange Is Nothing Then
            pixelDatas = layerData _
                .Select(Function(p)
                            Dim into As Double = p.totalIon

                            If into > plotRange.Max Then
                                into = plotRange.Max
                            ElseIf into < plotRange.Min Then
                                into = plotRange.Min
                            End If

                            Return New BioNovoGene.Analytical.MassSpectrometry.MsImaging.PixelData(p.x, p.y, into)
                        End Function) _
                .ToArray
        Else
            pixelDatas = layerData _
                .Select(Function(p)
                            Return New BioNovoGene.Analytical.MassSpectrometry.MsImaging.PixelData With {.x = p.x, .y = p.y, .intensity = p.totalIon}
                        End Function) _
                .ToArray
        End If

        Dim pixelFilter As New SingleIonLayer With {.DimensionSize = dimensions, .IonMz = -1, .MSILayer = pixelDatas}

        If params.enableFilter Then
            pixelFilter = filter(pixelFilter)
        End If

        layerData = pixelFilter.MSILayer.Select(Function(p) New PixelScanIntensity With {.x = p.x, .y = p.y, .totalIon = p.intensity}).ToArray

        Return Rendering(layerData, dimensions, params.colors.Description, mapLevels)
    End Function

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim image As Image = New RasterScaler(Rendering()).Scale(hqx:=params.Hqx)

        If params.showPhysicalRuler Then
            Call New Ruler(args.GetTheme).DrawOnImage(image, dimensions, Color.White, params.resolution)
        End If

        Return image
    End Function

    ''' <summary>
    ''' rendering of the TIC data layer image
    ''' </summary>
    ''' <param name="layerData"></param>
    ''' <param name="dimensions"></param>
    ''' <param name="color"></param>
    ''' <param name="mapLevels"></param>
    ''' <returns>
    ''' this is a safe function, and this function will returns nothing if
    ''' the given <paramref name="layerData"/> is empty
    ''' </returns>
    Public Overloads Shared Function Rendering(layerData As PixelScanIntensity(),
                                               dimensions As Size,
                                               color As String,
                                               mapLevels As Integer) As Image
        If layerData.IsNullOrEmpty Then
            Return Nothing
        End If

        Return Drawer.RenderSummaryLayer(
            layer:=layerData,
            dimension:=dimensions,
            colorSet:=color,
            mapLevels:=mapLevels
        ).AsGDIImage
    End Function

    Public Overrides Sub SetIntensityRange(normRange As DoubleRange)
        Dim std As New DoubleRange(0, 1)
        Dim range As New DoubleRange(intensity)
        Dim min As Double = std.ScaleMapping(normRange.Min, range)
        Dim max As Double = std.ScaleMapping(normRange.Max, range)

        plotRange = New DoubleRange(min, max)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetTrIQIntensity(q As Double) As Double
        Return TrIQ.FindThreshold(intensity, q)
    End Function
End Class
