#Region "Microsoft.VisualBasic::f296c25cd48d10ae7f67d0c8a5ff055b, G:/mzkit/src/mzkit/mzblender//MSImaging/HeatMapBlender.vb"

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

    '   Total Lines: 38
    '    Code Lines: 27
    ' Comment Lines: 0
    '   Blank Lines: 11
    '     File Size: 1.33 KB


    ' Class HeatMapBlender
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: GetTrIQIntensity, Rendering
    ' 
    '     Sub: SetIntensityRange
    ' 
    ' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap
Imports Task

Public Class HeatMapBlender : Inherits MSImagingBlender

    Dim layer As PixelData()
    Dim dimension As Size

    Public Sub New(layer As PixelData(), dimension As Size, filter As RasterPipeline)
        MyBase.New(filter)

        Me.layer = layer
        Me.dimension = dimension
    End Sub

    Public Overrides Sub SetIntensityRange(normRange As DoubleRange)

    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Dim blender As New HeatMap.PixelRender(params.colors.Description, params.mapLevels, defaultColor:=params.background)
        Dim img As Image = blender.RenderRasterImage(layer, dimension, fillRect:=True)

        If params.showPhysicalRuler Then
            Call New Ruler(args.GetTheme).DrawOnImage(img, dimension, Color.White, params.resolution)
        End If

        Return img
    End Function

    Public Overrides Function GetTrIQIntensity(q As Double) As Double

    End Function
End Class
