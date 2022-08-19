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

    Public Overloads Function Rendering(args As PlotProperty, target As Size, sample As String) As Image
        sample_tag = sample
        Return Rendering(args, target)
    End Function
End Class
