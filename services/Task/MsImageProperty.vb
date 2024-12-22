Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports Microsoft.VisualBasic.Linq
Imports MZKitWin32.Blender.CommonLibs

Public Module MsImagePropertyExtensions

    <Extension>
    Public Function LoadFromRender(render As Drawer) As MsImageProperty
        Dim args As New MsImageProperty(
            scan_x:=render.dimension.Width,
            scan_y:=render.dimension.Height) With {
            .background = Color.Black,
            .resolution = render.pixelReader.resolution
        }

        If TypeOf render.pixelReader Is ReadIbd Then
            Dim UUID = DirectCast(render.pixelReader, ReadIbd).UUID
            Dim fileSize = DirectCast(render.pixelReader, ReadIbd).ibd.size _
                .DoCall(AddressOf StringFormats.Lanudry)

            Call args.SetimzML(fileSize, UUID)
        End If

        Return args
    End Function
End Module
