#Region "Microsoft.VisualBasic::79762f7a54e6c5d0b045e5f4c3c399a8, mzkit\services\ServiceHub\ServiceProtocols\Payloads\RegionLoader.vb"

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

    '   Total Lines: 74
    '    Code Lines: 51 (68.92%)
    ' Comment Lines: 13 (17.57%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (13.51%)
    '     File Size: 2.32 KB


    ' Class RegionLoader
    ' 
    '     Properties: empty, height, regions, sample_tags, size
    '                 width
    ' 
    '     Function: ContainsPixel, GetTissueMap, Reload
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Math2D

''' <summary>
''' A helper data model for the single region polygons data
''' </summary>
Public Class RegionLoader

    ''' <summary>
    ''' the polygon regions for each samples
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' the multiple polygon object represents a single sample region
    ''' </remarks>
    Public Property regions As Polygon2D()
    Public Property width As Integer
    Public Property height As Integer
    Public Property is_raster As Boolean = False
    ''' <summary>
    ''' this property value should be nothing or 
    ''' size equals to the <see cref="regions"/>.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' if this property character vector is not empty, then it means these
    ''' polygon maybe comes from the different rawdata sample files.
    ''' </remarks>
    Public Property sample_tags As String()

    ''' <summary>
    ''' the bootstrapping parameters for extract the matrix sample from current sample region.
    ''' </summary>
    ''' <returns></returns>
    Public Property bootstrapping As SampleBootstrapping

    ''' <summary>
    ''' get number of the samples
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property size As Integer
        Get
            Return regions.TryCount
        End Get
    End Property

    Public ReadOnly Property empty As Boolean
        Get
            Return width = 0 OrElse
                height = 0 OrElse
                regions.IsNullOrEmpty OrElse
                regions.All(Function(r) r.length = 0)
        End Get
    End Property

    Default Public ReadOnly Property Item(tag As String) As Polygon2D
        Get
            Dim i As Integer = sample_tags.IndexOf(tag)

            If i < 0 Then
                Return Nothing
            Else
                Return _regions(i)
            End If
        End Get
    End Property

    Public Function Reload() As RegionLoader
        Return New RegionLoader With {
            .height = height,
            .width = width,
            .regions = regions _
                .Select(Function(r)
                            Return New Polygon2D(r.xpoints, r.ypoints)
                        End Function) _
                .ToArray,
            .sample_tags = sample_tags,
            .bootstrapping = bootstrapping
        }
    End Function

    Public Function GetTissueMap(Optional label As String = "tissue_region", Optional color As String = "skyblue") As TissueRegion
        Return regions.RasterGeometry2D(New Size(width, height), label, color.TranslateColor)
    End Function

    Public Function GetTissueRaster() As IEnumerable(Of Point)
        Return regions.RasterGeometry2D(New Drawing.Size(width, height))
    End Function

    Public Function ContainsPixel(x As Integer, y As Integer) As Boolean
        Return regions.Any(Function(r) r.inside(x, y))
    End Function
End Class

''' <summary>
''' Parameters for make spatial sample bootstrapping
''' </summary>
Public Class SampleBootstrapping

    ''' <summary>
    ''' n sample point result for make the bootstrapping
    ''' </summary>
    ''' <returns></returns>
    Public Property nsamples As Integer = 64

    ''' <summary>
    ''' percentage value in range [0,1]
    ''' </summary>
    ''' <returns></returns>
    Public Property coverage As Double = 0.1

    ''' <summary>
    ''' a set of the target ions m/z for build the sample dataframe
    ''' </summary>
    ''' <returns></returns>
    Public Property ions As Dictionary(Of String, Double)
    ''' <summary>
    ''' the mass tolerance error window size for extract the ion intensity data
    ''' </summary>
    ''' <returns></returns>
    Public Property massWin As Double = 0.01

    Public Shared Function GetDefault() As SampleBootstrapping
        Return New SampleBootstrapping With {.coverage = 0.1, .nsamples = 64}
    End Function

End Class