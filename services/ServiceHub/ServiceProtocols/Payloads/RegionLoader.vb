Imports System.Drawing
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Math2D

Public Class RegionLoader

    Public Property regions As Polygon2D()
    Public Property width As Integer
    Public Property height As Integer

    Public Function Reload() As RegionLoader
        Return New RegionLoader With {
            .height = height,
            .width = width,
            .regions = regions _
                .Select(Function(r)
                            Return New Polygon2D(r.xpoints, r.ypoints)
                        End Function) _
                .ToArray
        }
    End Function

    Public Function GetTissueMap(Optional label As String = "tissue_region", Optional color As String = "skyblue") As TissueRegion
        Return regions.Geometry2D(New Size(width, height), label, color.TranslateColor)
    End Function

    Public Function ContainsPixel(x As Integer, y As Integer) As Boolean
        Return regions.Any(Function(r) r.inside(x, y))
    End Function
End Class
