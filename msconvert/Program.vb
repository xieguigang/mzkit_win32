Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.My.FrameworkInternal

''' <summary>
''' 主要是为了兼容第三方厂家的原始数据文件模块的引用而构建的.NET4.8兼容模块
''' </summary>
<CLI> Module Program

    Sub New()
        Call Thread.Sleep(2000)
    End Sub

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/mzPack")>
    <Description("Build mzPack cache")>
    <Argument("--raw", False, CLITypes.File, PipelineTypes.std_in, Description:="the file path of the mzML/mzXML/raw raw data file to create mzPack cache file.")>
    <Argument("--cache", False, CLITypes.File, PipelineTypes.std_out, Description:="the file path of the mzPack cache file.")>
    <Argument("/ver", True, CLITypes.Boolean, PipelineTypes.undefined,
              AcceptTypes:={GetType(Boolean)},
              Description:="the file format version of the generated mzpack data file")>
    <Usage("/mzPack --raw <filepath.mzXML> [--cache <result.mzPack> /ver 2 /mute /no-thumbnail]")>
    Public Function convertAnyRaw(args As CommandLine) As Integer
        Dim raw As String = args("--raw")
        Dim cache As String = args("--cache") Or raw.ChangeSuffix("mzPack")
        Dim ver As Integer = args("/ver") Or 2
        Dim mute As Boolean = args("/mute")
        Dim noSnapshot As Boolean = args("/no-thumbnail")

        If raw.DirectoryExists Then
            For Each file As String In raw.EnumerateFiles("*.raw")
                Dim cachefile As String = $"{cache}/{file.BaseName}.mzPack"

                Call Console.WriteLine(file.BaseName)
                Call ConvertToMzPack.CreateMzpack(file, cachefile, saveVer:=ver, mute:=mute, skipThumbnail:=noSnapshot, sleepTime:=0)
            Next
        Else
            Call ConvertToMzPack.CreateMzpack(raw, cache, saveVer:=ver, mute:=mute, skipThumbnail:=noSnapshot)
        End If

        Return 0
    End Function

    <ExportAPI("/cdf_to_mzpack")>
    <Description("Convert GCMS un-targetted CDF raw data file to mzPack.")>
    <Usage("/cdf_to_mzpack --raw <filepath.cdf> [--cache <result.mzPack> /ver 2 /mute /no-thumbnail]")>
    Public Function convertGCMSCDF(args As CommandLine) As Integer
        Dim raw As String = args("--raw")
        Dim cache As String = args("--cache") Or raw.ChangeSuffix("mzPack")
        Dim ver As Integer = args("/ver") Or 2
        Dim mute As Boolean = args("/mute")
        Dim noSnapshot As Boolean = args("/no-thumbnail")

        Call FrameworkInternal.ConfigMemory(load:=MemoryLoads.Max)
        Call ConvertToMzPack.ConvertCDF(raw, cache, saveVer:=ver, mute:=mute, skipThumbnail:=noSnapshot)

        Return 0
    End Function

    <ExportAPI("/rowbinds")>
    <Description("Combine row scans to mzPack")>
    <Argument("--files", False, CLITypes.File, PipelineTypes.std_in, Description:="a temp file path that its content contains selected raw data file path for each row scans.")>
    <Argument("--save", False, CLITypes.File, PipelineTypes.std_in, Description:="a file path for export mzPack data file.")>
    <Usage("/rowbinds --files <list.txt> --save <MSI.mzPack> [/cutoff <intensity_cutoff, default=0> /matrix_basePeak <mz, default=0>]")>
    Public Function MSIRowCombine(args As CommandLine) As Integer
        Dim files As String() = args("--files").ReadAllLines
        Dim save As String = args("--save")
        Dim cutoff As Double = args("/cutoff") Or 0.0
        Dim basePeak As Double = args("/matrix_basePeak") Or 0.0

        Using file As FileStream = save.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Dim buffer As mzPack = MSImagingRowBinds.MSI_rowbind(files, cutoff, basePeak)
            Call buffer.Write(file, version:=2)
        End Using

        Return 0
    End Function

    <ExportAPI("/imzml")>
    <Description("Convert raw data file to imzML file.")>
    <Usage("/imzml --file <source.data> --save <file.imzML> [/cutoff <intensity_cutoff, default=0> /matrix_basePeak <mz, default=0>]")>
    Public Function MSIToimzML(args As CommandLine) As Integer
        Dim files As String() = args("--file").ReadAllLines
        Dim save As String = args("--save")
        Dim cutoff As Double = args("/cutoff") Or 0.0
        Dim basePeak As Double = args("/matrix_basePeak") Or 0.0
        Dim mzpack As mzPack = MSImagingRowBinds.MSI_rowbind(files, cutoff, basePeak)
        Dim polygon As New Polygon2D(mzpack.MS.Select(Function(scan) scan.GetMSIPixel))
        Dim dimsize As New Size(
            width:=polygon.xpoints.Max,
            height:=polygon.ypoints.Max
        )

        Using writer As imzML.mzPackWriter = imzML.mzPackWriter _
            .OpenOutput(save) _
            .SetMSImagingParameters(dimsize, 17) _
            .SetSourceLocation(files(Scan0)) _
            .SetSpectrumParameters(1)

            For Each scan As ScanMS1 In mzpack.MS
                Call writer.WriteScan(scan)
            Next
        End Using

        Return 0
    End Function

    <ExportAPI("/imports-SCiLSLab")>
    <Usage("/imports-SCiLSLab --files <spot_files.txt> --save <MSI.mzPack>")>
    <Argument("--files", False, CLITypes.File,
              AcceptTypes:={GetType(String())},
              Description:="a list of the csv data files, the spot index and
              the mass data should be paired in this data file list. each line 
              of this text file is a tuple of the spot index and the ms data, 
              and the tuple data should be used the <TAB> as delimiter.")>
    Public Function ImportsSCiLSLab(args As CommandLine) As Integer
        Dim files As String() = args("--files").ReadAllLines
        Dim mzpack As mzPack
        Dim save As String = args("--save")
        Dim spotTuples = files _
            .Select(Function(line)
                        Dim parts = line.Split(CChar(vbTab))
                        Dim index As String = parts(Scan0)
                        Dim msdata As String = parts(1)

                        Return (index, msdata)
                    End Function) _
            .ToArray

        Using buffer As Stream = save.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            mzpack = MSIRawPack.LoadMSIFromSCiLSLab(
                files:=spotTuples,
                println:=AddressOf RunSlavePipeline.SendMessage
            )
            mzpack.Write(file:=buffer, version:=2)
        End Using

        Return 0
    End Function
End Module
