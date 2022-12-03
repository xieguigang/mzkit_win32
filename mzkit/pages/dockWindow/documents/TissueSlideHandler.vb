Imports Microsoft.VisualBasic.FileIO
Imports Task

Public Module TissueSlideHandler

    Public Sub OpenNdpiFile(ndpi As String)
        ' do convert and then load the raw tiff image
        Dim ndpitools As String = $"{AppEnvironment.GetNdpiTools}/ndpi2tiff.exe"
        Dim tiff As String

        Using workdir As New TemporaryEnvironment(ndpi.ParentPath)
            Dim invoke As New Process With {
                .StartInfo = New ProcessStartInfo With {
                    .FileName = ndpitools,
                    .Arguments = $"""./{ndpi.FileName}"",0",
                    .CreateNoWindow = True
                }
            }

            tiff = $"{ndpi.ParentPath}/{ndpi.FileName},0.tif"
        End Using

        'PixelSelector1.OpenImageFile(tiff)
        'PixelSelector1.PreviewButton = True
        'PixelSelector1.ShowPreview = True
        Call VisualStudio.ShowDocument(Of frmOpenseadragonViewer)(, title:="Hamamatsu slide: " & tiff.FileName).LoadSlide(tiff)
    End Sub

    Public Sub OpenTifFile(tif As String)
        Call VisualStudio.ShowDocument(Of frmOpenseadragonViewer)(, title:="Deep Zoom Image: " & tif.FileName).LoadSlide(tif)
    End Sub

End Module