#Region "Microsoft.VisualBasic::0062a7e92cc1bc6271909ef2b93288ad, G:/mzkit/src/mzkit/dist/OpenSlide/dzitool//Program.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 148
    '    Code Lines: 115
    ' Comment Lines: 5
    '   Blank Lines: 28
    '     File Size: 5.81 KB


    ' Module Program
    ' 
    '     Function: GetDZI, GetDziImage, Main
    ' 
    '     Sub: convertTiledTiff, GetJpg
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports dzitool.Container
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.DataStorage.HDSPack.FileSystem
Imports Microsoft.VisualBasic.Language
Imports OpenSlideCs
Imports dir = Microsoft.VisualBasic.FileIO.Directory

Module Program

    Dim openSlide As OpenSlide
    Dim pack As IFileSystemEnvironment

    Private Sub GetJpg(level As Integer, row As Integer, col As Integer, filename As String, outputname As String)
        Using stream = openSlide.GetJpg(filename, $"_files/{level}/{row}_{col}.jpeg")
            Dim file As Stream = pack.OpenFile($"{outputname}.jpeg", FileMode.OpenOrCreate, FileAccess.Write)
            Call stream.Seek(Scan0, SeekOrigin.Begin)
            Call stream.CopyTo(file)
            Call file.Flush()
            Call file.Close()
        End Using
    End Sub

    Private Function GetDZI(filepath As String, outputname As String) As Integer
        Dim width, height As Long

        Using stream As MemoryStream = openSlide.GetDZI(filepath, width, height)
            Dim dziIndex As String = outputname.ChangeSuffix("dzi").FileName
            Dim file As Stream = pack.OpenFile(dziIndex, FileMode.OpenOrCreate, FileAccess.Write)

            Call RunSlavePipeline.SendMessage($"{outputname} successfully created!")
            Call stream.Seek(Scan0, SeekOrigin.Begin)
            Call stream.CopyTo(file)
            Call file.Flush()
            Call file.Close()

            Call pack.WriteText(dziIndex, "/index.txt")
        End Using

        Dim levels = openSlide.Dimensions(filepath)
        Dim maxDimensions = levels(levels.Length - 1)
        Dim export_dir As String = $"/{outputname.BaseName}"

        For level As Integer = 0 To levels.Length - 1
            Dim levelInfo = levels(level)
            Dim total As Integer = levelInfo.Width * levelInfo.Height
            Dim d As Integer = total / 25
            Dim proc As i32 = Scan0

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

        Call pack.Flush()
        Call pack.Close()
        Call RunSlavePipeline.SendMessage("Done!")

        Return 0
    End Function

    <ExportAPI("/parse")>
    <Usage("/parse --file <slide.svs/ndpi/tiff> [--export <image.dzi> --verbose]")>
    Public Function GetDziImage(args As CommandLine) As Integer
        Dim inputfile As String = args("--file")
        Dim export_file As String = args("--export") Or inputfile.ChangeSuffix("dzi")
        Dim verbose As Boolean = args("--verbose")

        If Not export_file.ExtensionSuffix("dzi") Then
            If export_file.ExtensionSuffix("hds") Then
                pack = StreamPack.CreateNewStream(export_file, meta_size:=8 * 1024 * 1024)
            Else
                Call RunSlavePipeline.SendMessage("The export file name should be a deep zoom image(*.dzi)!")
                Return 500
            End If
        Else
            pack = dir.FromLocalFileSystem(export_file.ParentPath)
        End If

        Call AppEnvironment.SetExternalCDllDirectory(AppEnvironment.getOpenSlideLibDLL)

        openSlide = New OpenSlide

        If verbose Then
            OpenSlide.OnTrace = AddressOf RunSlavePipeline.SendMessage
        End If

        Try
            openSlide.EnsureOpen(inputfile)
        Catch ex As Exception
            If inputfile.ExtensionSuffix("tif", "tiff") Then
                Call convertTiledTiff(inputfile)
            Else
                Return 500
            End If
        End Try

        ' export DZI file
        Return GetDZI(inputfile, export_file)
    End Function

    ''' <summary>
    ''' needs conversion via vips
    ''' </summary>
    Private Sub convertTiledTiff(ByRef inputfile As String)
        ' needs conversion via vips
        Dim input_tiled As String = $"{inputfile.ParentPath}/{inputfile.BaseName}-tiled.tiff"
        Dim cli As String = $"tiffsave {inputfile.CLIPath} {input_tiled.CLIPath} --tile --pyramid"
        Dim vips As String = $"{AppEnvironment.getVIPS}/vips.exe".GetFullPath

        Call VBDebugger.EchoLine("calling external vips tool:")
        Call VBDebugger.EchoLine(vips)
        Call RunSlavePipeline.SendMessage("The input tiff image needs to be convert to tiled image...")

        If input_tiled.FileLength < 64 * 1024& * 1024& Then
            Try
                Call PipelineProcess.ExecSub(vips, cli)
            Catch ex As Exception
                Call App.LogException(ex)
                Call RunSlavePipeline.SendMessage($"error while convert tiff file: {ex.Message}!")
            End Try
        End If

        Call input_tiled.Swap(inputfile)
        Call openSlide.EnsureOpen(inputfile)

        Call RunSlavePipeline.SendMessage($"Use the new tiled image file {inputfile}!")
    End Sub

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function
End Module
