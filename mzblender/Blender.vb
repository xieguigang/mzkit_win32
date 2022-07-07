Imports Task

Public MustInherit Class Blender

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="args">
    ''' the image plot arguments
    ''' </param>
    ''' <param name="target">
    ''' the size of the target control to show the rednering image result.
    ''' </param>
    ''' <returns></returns>
    Public MustOverride Function Rendering(args As PlotProperty, target As Size) As Image

End Class
