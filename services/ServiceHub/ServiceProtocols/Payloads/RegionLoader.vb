Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Math2D

Public Class RegionLoader

    Public Property regions As Polygon2D()
    Public Property width As Integer
    Public Property height As Integer

    ''' <summary>
    ''' this property value should be nothing or 
    ''' size equals to the <see cref="regions"/>.
    ''' </summary>
    ''' <returns></returns>
    Public Property sample_tags As String()

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

    Public Function Reload() As RegionLoader
        Return New RegionLoader With {
            .height = height,
            .width = width,
            .regions = regions _
                .Select(Function(r)
                            Return New Polygon2D(r.xpoints, r.ypoints)
                        End Function) _
                .ToArray,
            .sample_tags = sample_tags
        }
    End Function

    Public Function GetTissueMap(Optional label As String = "tissue_region", Optional color As String = "skyblue") As TissueRegion
        Return regions.RasterGeometry2D(New Size(width, height), label, color.TranslateColor)
    End Function

    Public Function ContainsPixel(x As Integer, y As Integer) As Boolean
        Return regions.Any(Function(r) r.inside(x, y))
    End Function
End Class
