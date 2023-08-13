#Region "Microsoft.VisualBasic::606f2d5195ea118b1e9209f51d7483f4, mzkit\src\mzkit\mzblender\MSImaging\MSImagingBlender.vb"

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

'   Total Lines: 40
'    Code Lines: 29
' Comment Lines: 5
'   Blank Lines: 6
'     File Size: 1.21 KB


' Class MSImagingBlender
' 
'     Properties: HEMap, sample_tag, showAllSample
' 
'     Constructor: (+1 Overloads) Sub New
'     Function: Rendering, TakePixels
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Blender.Scaler
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Task

Public MustInherit Class MSImagingBlender : Inherits Blender

    Protected ReadOnly params As MsImageProperty

    Public Property sample_tag As String
    Public Property HEMap As Image
    Public ReadOnly Property showAllSample As Boolean
        Get
            Return sample_tag.StringEmpty OrElse sample_tag = "*"
        End Get
    End Property

    Public ReadOnly Property filters As RasterPipeline

    <DebuggerStepThrough>
    Sub New(host As MsImageProperty, filters As RasterPipeline)
        Me.params = host
        Me.filters = filters
    End Sub

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
    ''' <returns></returns>
    Protected Function TakePixels(pixels As PixelData()) As PixelData()
        If showAllSample Then
            Return pixels
        Else
            Return pixels _
                .Where(Function(i) i.sampleTag = sample_tag) _
                .ToArray
        End If
    End Function

    Public Overloads Function Rendering(args As PlotProperty, target As Size, sample As String) As Image
        sample_tag = sample
        Return Rendering(args, target)
    End Function
End Class
