#Region "Microsoft.VisualBasic::f8e3c8b040622f6abdcb339bc6911ed9, mzkit\mzblender\MSImaging\MSImagingBlender.vb"

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

    '   Total Lines: 98
    '    Code Lines: 65 (66.33%)
    ' Comment Lines: 15 (15.31%)
    '    - Xml Docs: 93.33%
    ' 
    '   Blank Lines: 18 (18.37%)
    '     File Size: 3.42 KB


    ' Class MSImagingBlender
    ' 
    '     Properties: filters, HEMap, region_outlines, sample_outline, sample_tag
    '                 showAllSample
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: DrawOutlines, Rendering, TakePixels
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing.Drawing2D
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Task

Public MustInherit Class MSImagingBlender : Inherits Blender

    Public Property sample_tag As String
    Public Property HEMap As Image

    Public ReadOnly Property showAllSample As Boolean
        Get
            Return sample_tag.StringEmpty OrElse sample_tag = "*"
        End Get
    End Property

    Public Property filters As RasterPipeline
    Public Property sample_outline As GeneralPath
    Public Property region_outlines As (GeneralPath, Color)()

    Protected params As MsImageProperty

    <DebuggerStepThrough>
    Sub New(filters As RasterPipeline)
        Me.filters = filters
    End Sub

    Protected Function DrawOutlines(image As Image) As Image
        Dim line_width As Single = 1
        Dim showOutline As Boolean = False

        ' draw outline before upscale
        If showOutline AndAlso Not sample_outline Is Nothing Then
            Using g As Graphics2D = New Graphics2D(image)
                Dim pen As Pen = New Pen(Color.White, line_width) With {.DashStyle = DashStyle.Solid}

                For Each path As PointF() In sample_outline.GetPolygons
                    Call g.DrawPolygon(pen, path)
                Next

                image = g.ImageResource
            End Using
        End If

        If showOutline AndAlso Not region_outlines.IsNullOrEmpty Then
            Using g As New Graphics2D(image)
                For Each region In region_outlines
                    Dim pen As New Pen(region.Item2, line_width) With {.DashStyle = DashStyle.Solid}

                    For Each path As PointF() In region.Item1.GetPolygons
                        Call g.DrawPolygon(pen, path)
                    Next
                Next

                image = g.ImageResource
            End Using
        End If

        Return image
    End Function

    ''' <summary>
    ''' set the intensity range for plot imaging visual
    ''' </summary>
    ''' <param name="normRange">
    ''' A numeric range which is normalized at [0,1] level.
    ''' </param>
    Public MustOverride Sub SetIntensityRange(normRange As DoubleRange)
    Public MustOverride Function GetTrIQIntensity(q As Double) As Double

    ''' <summary>
    ''' take sample pixels via <see cref="sample_tag"/>
    ''' </summary>
    ''' <param name="pixels"></param>
    ''' <returns>
    ''' this function will ensure that the data is not nothing 
    ''' but maybe empty collection generated.
    ''' </returns>
    Protected Function TakePixels(pixels As PixelData()) As PixelData()
        If showAllSample Then
            Return pixels
        Else
            Return pixels _
                .Where(Function(i) i.sampleTag = sample_tag) _
                .ToArray
        End If
    End Function

    Public Overloads Function Rendering(args As PlotProperty, target As Size, params As MsImageProperty, sample As String) As Image
        Me.sample_tag = sample
        Me.params = params

        Return Rendering(args, target)
    End Function
End Class
