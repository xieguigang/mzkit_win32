Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Task

Public Class SingleIonMSIBlender : Inherits Blender

    ReadOnly layer As SingleIonLayer
    ReadOnly params As MsImageProperty

    Sub New(layer As SingleIonLayer, params As MsImageProperty)
        Me.layer = layer
        Me.params = params
    End Sub

    Public Overrides Function Rendering(args As PlotProperty, target As Size) As Image
        Throw New NotImplementedException()
    End Function
End Class
