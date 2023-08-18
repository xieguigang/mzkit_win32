Imports System.IO
Imports dzitool.Container
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports OpenSlideCs

Module Program

    Dim openSlide As OpenSlide

    Private Sub GetJpg(ByVal level As Integer, ByVal row As Integer, ByVal col As Integer, ByVal filename As String, ByVal outputname As String)
        Dim buffer As ArraySegment(Of Byte) = Nothing

        Using stream = openSlide.GetJpg(filename, $"_files/{level}/{row}_{col}.jpeg")
            If stream.TryGetBuffer(buffer) Then
                Call buffer.FlushStream($"{outputname}.jpeg")
                Call RunSlavePipeline.SendMessage($"{outputname}.jpeg successfully created!")
            Else
                Call RunSlavePipeline.SendMessage($"{outputname}.jpeg failed...")
            End If
        End Using
    End Sub

    Private Sub GetDZI(filepath As String, ByVal outputname As String)
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

            Call Directory.CreateDirectory($"{export_dir}_files/{level}")

            For row As Integer = 0 To levelInfo.Width - 1
                For col As Integer = 0 To levelInfo.Height - 1
                    Call GetJpg(level, row, col, filepath, $"{export_dir}_files/{level}/{row}_{col}")
                Next
            Next
        Next
    End Sub

    <ExportAPI("/parse")>
    <Usage("/parse --file <slide.svs/ndpi> [--export <image.dzi>]")>
    Public Function GetDziImage(args As CommandLine) As Integer
        Dim inputfile As String = args("--file")
        Dim export_file As String = args("--export") Or inputfile.ChangeSuffix("dzi")

        If Not export_file.ExtensionSuffix("dzi") Then
            Call RunSlavePipeline.SendMessage("The export file name should be a deep zoom image(*.dzi)!")
            Return 500
        End If

        Call AppEnvironment.SetDllDirectory(AppEnvironment.getOpenSlideLibDLL)

        openSlide = New OpenSlide
        OpenSlide.OnTrace = AddressOf RunSlavePipeline.SendMessage
        openSlide.EnsureOpen(inputfile)

        ' export DZI file
        Call GetDZI(inputfile, export_file)
        Call RunSlavePipeline.SendMessage("Done!")

        Return 0
    End Function

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function
End Module
