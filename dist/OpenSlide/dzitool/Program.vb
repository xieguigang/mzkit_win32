Imports System.IO
Imports dzitool.Container
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Language
Imports OpenSlideCs

Module Program

    Dim openSlide As OpenSlide
    Dim pack As StreamPack

    Private Sub GetJpg(level As Integer, row As Integer, col As Integer, filename As String, outputname As String)
        Dim buffer As ArraySegment(Of Byte) = Nothing

        Using stream = openSlide.GetJpg(filename, $"_files/{level}/{row}_{col}.jpeg")
            If stream.TryGetBuffer(buffer) Then
                Call buffer.FlushStream($"{outputname}.jpeg")
                ' Call RunSlavePipeline.SendMessage($"{outputname}.jpeg successfully created!")
            Else
                ' Call RunSlavePipeline.SendMessage($"{outputname}.jpeg failed...")
            End If
        End Using
    End Sub

    Private Sub GetDZI(filepath As String, outputname As String)
        Dim width, height As Long
        Dim buffer As ArraySegment(Of Byte) = Nothing

        Using stream = openSlide.GetDZI(filepath, width, height)
            If stream.TryGetBuffer(buffer) Then
                Call RunSlavePipeline.SendMessage($"{outputname} successfully created!")
                Call buffer.FlushStream(outputname)
            Else
                Call RunSlavePipeline.SendMessage($"{outputname} failed...")
            End If
        End Using

        Dim levels = openSlide.Dimensions(filepath)
        Dim maxDimensions = levels(levels.Length - 1)
        Dim export_dir As String = $"{outputname.ParentPath}/{outputname.BaseName}"

        For level As Integer = 0 To levels.Length - 1
            Dim levelInfo = levels(level)
            Dim total As Integer = levelInfo.Width * levelInfo.Height
            Dim d As Integer = total / 25
            Dim proc As i32 = Scan0

            Call Directory.CreateDirectory($"{export_dir}_files/{level}")
            Call RunSlavePipeline.SendProgress(0, $"Process tile image at level: {level}...")

            For row As Integer = 0 To levelInfo.Width - 1
                For col As Integer = 0 To levelInfo.Height - 1
                    Try
                        Call GetJpg(level, row, col, filepath, $"{export_dir}_files/{level}/{row}_{col}")
                    Catch ex As Exception

                    End Try

                    If d <> 0 AndAlso ((++proc) Mod d = 0) Then
                        Call RunSlavePipeline.SendProgress(proc / total * 100, $"{export_dir}_files/{level}/{row}_{col}")
                    End If
                Next
            Next
        Next
    End Sub

    <ExportAPI("/parse")>
    <Usage("/parse --file <slide.svs/ndpi/tiff> [--export <image.dzi> --verbose]")>
    Public Function GetDziImage(args As CommandLine) As Integer
        Dim inputfile As String = args("--file")
        Dim export_file As String = args("--export") Or inputfile.ChangeSuffix("dzi")
        Dim verbose As Boolean = args("--verbose")

        If Not export_file.ExtensionSuffix("dzi") Then
            Call RunSlavePipeline.SendMessage("The export file name should be a deep zoom image(*.dzi)!")
            Return 500
        End If

        Call AppEnvironment.SetDllDirectory(AppEnvironment.getOpenSlideLibDLL)

        openSlide = New OpenSlide

        If verbose Then
            OpenSlide.OnTrace = AddressOf RunSlavePipeline.SendMessage
        End If

        Try
            openSlide.EnsureOpen(inputfile)
        Catch ex As Exception
            If inputfile.ExtensionSuffix("tif", "tiff") Then
                ' needs conversion via vips
                Dim input_tiled As String = $"{inputfile.ParentPath}/{inputfile.BaseName}-tiled.tiff"
                Dim cli As String = $"tiffsave {inputfile.CLIPath} {input_tiled.CLIPath} --tile --pyramid"
                Dim vips As String = $"{AppEnvironment.getVIPS}/vips.exe"

                Call RunSlavePipeline.SendMessage("The input tiff image needs to be convert to tiled image...")

                Call PipelineProcess.ExecSub(vips, cli)
                Call input_tiled.Swap(inputfile)
                Call openSlide.EnsureOpen(inputfile)

                Call RunSlavePipeline.SendMessage($"Use the new tiled image file {inputfile}!")
            Else
                Return 500
            End If
        End Try

        ' export DZI file
        Call GetDZI(inputfile, export_file)
        Call RunSlavePipeline.SendMessage("Done!")

        Return 0
    End Function

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function
End Module
