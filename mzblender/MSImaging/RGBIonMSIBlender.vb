#Region "Microsoft.VisualBasic::b189176e306a8bd11c4aee8fb2775ec4, mzkit\mzblender\MSImaging\RGBIonMSIBlender.vb"

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

'   Total Lines: 83
'    Code Lines: 61 (73.49%)
' Comment Lines: 6 (7.23%)
'    - Xml Docs: 0.00%
' 
'   Blank Lines: 16 (19.28%)
'     File Size: 3.48 KB


' Class RGBIonMSIBlender
' 
'     Properties: dimensions
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: checkChannelMissing, GetTrIQIntensity, Rendering
' 
'     Sub: SetIntensityRange
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Linq
Imports Task
Imports Image = System.Drawing.Image

Public Class RGBIonMSIBlender : Inherits MSImagingBlender

    ReadOnly R As PixelData(), G As PixelData(), B As PixelData()
    ReadOnly originalSize As Size
    ReadOnly TIC As Image

    Public ReadOnly Property dimensions As Size
        Get
            Return New Size(params.scan_x, params.scan_y)
        End Get
    End Property

    Sub New(r As PixelData(), g As PixelData(), b As PixelData(), TIC As Image, filter As RasterPipeline)
        Call MyBase.New(filter)

        Dim joinX = r.JoinIterates(g).JoinIterates(b).Select(Function(i) i.x).Max
        Dim joinY = r.JoinIterates(g).JoinIterates(b).Select(Function(i) i.y).Max

        Me.TIC = TIC
        Me.originalSize = New Size(joinX, joinY)
        Me.R = r
        Me.G = g
        Me.B = b
    End Sub

    Private Shared Function checkChannelMissing(c As SingleIonLayer) As Boolean
        Return c Is Nothing OrElse c.MSILayer.IsNullOrEmpty
    End Function

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim drawer As New PixelRender(heatmapRender:=False)
        Dim dimensionSize As New Size(params.scan_x, params.scan_y)
        Dim r As New SingleIonLayer With {.DimensionSize = dimensions, .MSILayer = TakePixels(Me.R)}
        Dim g As New SingleIonLayer With {.DimensionSize = dimensions, .MSILayer = TakePixels(Me.G)}
        Dim b As New SingleIonLayer With {.DimensionSize = dimensions, .MSILayer = TakePixels(Me.B)}

        If params.enableFilter AndAlso filters IsNot Nothing Then
            ' 20240915
            ' r,g,b maybe empty is one channel not has been selected by the user.
            If Not checkChannelMissing(r) Then r = filters(r)
            If Not checkChannelMissing(g) Then g = filters(g)
            If Not checkChannelMissing(b) Then b = filters(b)
        End If

        'Dim qr As Double = q1.ThresholdValue(r.Select(Function(p) p.intensity).ToArray)
        'Dim qg As Double = q1.ThresholdValue(g.Select(Function(p) p.intensity).ToArray)
        'Dim qb As Double = q1.ThresholdValue(b.Select(Function(p) p.intensity).ToArray)
        'Dim cutoff = (New DoubleRange(0, qr), New DoubleRange(0, qg), New DoubleRange(0, qb))
        Dim image As Image = drawer.ChannelCompositions(
            R:=r.MSILayer, G:=g.MSILayer, B:=b.MSILayer,
            dimension:=dimensionSize,
            background:="transparent"
        ).AsGDIImage _
         .CTypeGdiImage

        image = DrawOutlines(image)
        image = New HeatMap.RasterScaler(image.CTypeFromGdiImage).Scale(hqx:=params.Hqx).CTypeGdiImage

        If params.showPhysicalRuler Then
            Call New Ruler(args.GetTheme).DrawOnImage(image.CTypeFromGdiImage, dimensions, Color.White, params.resolution)
        End If

        Return image
    End Function

    Public Overrides Sub SetIntensityRange(normRange As DoubleRange)

    End Sub

    Public Overrides Function GetTrIQIntensity(q As Double) As Double
        Return 1
    End Function
End Class
