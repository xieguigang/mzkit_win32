Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Darwinism.HPC.Parallel

Public Class MemoryPipe : Inherits Darwinism.HPC.Parallel.MemoryPipe

    Public Sub New(channel As MapObject)
        MyBase.New(channel)
    End Sub

    Public Overloads Sub WriteBuffer(pixels As PixelData())
        Call WriteBuffer(PixelData.GetBuffer(pixels))
    End Sub
End Class
