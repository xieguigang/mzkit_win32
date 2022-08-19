Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Task

Public MustInherit Class MSImagingBlender : Inherits Blender

    Protected ReadOnly params As MsImageProperty

    Public Property sample_tag As String

    Public ReadOnly Property showAllSample As Boolean
        Get
            Return sample_tag.StringEmpty OrElse sample_tag = "*"
        End Get
    End Property

    Sub New(host As MsImageProperty)
        params = host
    End Sub

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
