#Region "Microsoft.VisualBasic::9f68f087e1f236807489982ba745deb2, mzkit\mzblender\MSImaging\SummaryMSIBlender.vb"

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

    '   Total Lines: 162
    '    Code Lines: 114 (70.37%)
    ' Comment Lines: 21 (12.96%)
    '    - Xml Docs: 66.67%
    ' 
    '   Blank Lines: 27 (16.67%)
    '     File Size: 6.27 KB


    ' Class SummaryMSIBlender
    ' 
    '     Properties: dimensions
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetTrIQIntensity, (+3 Overloads) Rendering
    ' 
    '     Sub: SetIntensityRange
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.imzML
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Microsoft.VisualBasic.Math.Distributions
Imports MZKitWin32.Blender.CommonLibs
Imports Image = System.Drawing.Image

''' <summary>
''' rendering for total_ions/basepeak/average
''' </summary>
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

        'If Not filter Is Nothing Then
        '    If Not params.instrument.TextEquals("Bruker") Then
        '        filter = filter.Then(New KNNScaler(params.knn, params.knn_qcut)).Then(New SoftenScaler())
        '    Else
        '        filter = filter.Then(New SoftenScaler())
        '    End If
        'End If

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

        If params.enableFilter AndAlso filter IsNot Nothing Then
            pixelFilter = filter(pixelFilter)
        End If

        layerData = pixelFilter.MSILayer _
            .Select(Function(p)
                        Return New PixelScanIntensity With {
                            .x = p.x,
                            .y = p.y,
                            .totalIon = p.intensity
                        }
                    End Function) _
            .ToArray

        Dim MSI As Image = Rendering(layerData, dimensions, params.colors.Description, mapLevels, params.background.ToHtmlColor)

        If HEMap IsNot Nothing Then
            Using g As Graphics2D = MSI.Size.CreateGDIDevice()
                Call g.DrawImage(HEMap, New Rectangle(New Point, g.Size))
                Call g.DrawImageUnscaled(MSI, New Point)

                Return g.GetImageResource
            End Using
        Else
            Return MSI
        End If
    End Function

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim image As Image = Rendering()

        If params.Hqx > hqx.HqxScales.None Then
            image = New RasterScaler(image:=image.CTypeFromGdiImage).Scale(hqx:=params.Hqx).CTypeGdiImage
        End If

        If params.showPhysicalRuler Then
            Call New Ruler(args.GetTheme).DrawOnImage(image.CTypeFromGdiImage, dimensions, Color.White, params.resolution)
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
                                               mapLevels As Integer,
                                               background As String) As Image
        If layerData.IsNullOrEmpty Then
            Return Nothing
        End If

        Return Drawer.RenderSummaryLayer(
            layer:=layerData,
            dimension:=dimensions,
            colorSet:=color,
            mapLevels:=mapLevels,
            background:=background
        ).AsGDIImage.CTypeGdiImage
    End Function

    Public Overrides Sub SetIntensityRange(normRange As DoubleRange)
        Dim std As New DoubleRange(0, 1)
        Dim range As New DoubleRange(intensity)
        ' mapping [0,1] normRange to the rawdata intensity range
        Dim min As Double = std.ScaleMapping(normRange.Min, range)
        Dim max As Double = std.ScaleMapping(normRange.Max, range)

        plotRange = New DoubleRange(min, max)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Overrides Function GetTrIQIntensity(q As Double) As Double
        Return TrIQ.FindThreshold(intensity, q)
    End Function
End Class
