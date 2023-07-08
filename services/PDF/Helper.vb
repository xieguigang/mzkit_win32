Imports Microsoft.VisualBasic.ApplicationServices
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.Container
Imports WkHtmlToPdf.Arguments

Module Helper

    Public Sub PDF(filepath As String, sourceURL As String)
        Static bin As String = AppEnvironment.getWkhtmltopdf

        If bin.FileExists Then
            Dim env As New PdfConvertEnvironment With {
                .Debug = False,
                .TempFolderPath = TempFileSystem.GetAppSysTempFile,
                .Timeout = 60000,
                .WkHtmlToPdfPath = bin
            }
            Dim content As New PdfDocument With {.Url = {sourceURL}}
            Dim pdfFile As New PdfOutput With {.OutputFilePath = filepath}

            Call WkHtmlToPdf.PdfConvert.ConvertHtmlToPdf(content, pdfFile, env)
        Else
            Call Workbench.Warning("'wkhtmltopdf' tool is missing for generate PDF file...")
        End If
    End Sub
End Module
