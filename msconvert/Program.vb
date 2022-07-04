Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection

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
    <Usage("/mzPack --raw <filepath.mzXML> --cache <result.mzPack>")>
    Public Function convertAnyRaw(args As CommandLine) As Integer
        Dim raw As String = args("--raw")
        Dim cache As String = args("--cache")

        Call ConvertToMzPack.CreateMzpack(raw, cache)

        Return 0
    End Function

    <ExportAPI("/rowbinds")>
    <Description("Combine row scans to mzPack")>
    <Argument("--files", False, CLITypes.File, PipelineTypes.std_in, Description:="a temp file path that its content contains selected raw data file path for each row scans.")>
    <Argument("--save", False, CLITypes.File, PipelineTypes.std_in, Description:="a file path for export mzPack data file.")>
    <Usage("/rowbinds --files <list.txt> --save <MSI.mzPack>")>
    Public Function MSIRowCombine(args As CommandLine) As Integer
        Dim files As String() = args("--files").ReadAllLines
        Dim save As String = args("--save")

        Call MSImagingRowBinds.MSI_rowbind(files, save)

        Return 0
    End Function

    <ExportAPI("/imports-SCiLSLab")>
    <Usage("/imports-SCiLSLab --files <spot_files.txt> --save <MSI.mzPack>")>
    Public Function ImportsSCiLSLab(args As CommandLine) As Integer
        Dim files As String() = args("--files").ReadAllLines
        Dim save As String = args("--save")
        Dim msdata As String = files(Scan0)
        Dim index As String = files(1)

        Using msdatafile As Stream = msdata.Open(FileMode.Open, doClear:=False, [readOnly]:=True),
            indexfile As Stream = index.Open(FileMode.Open, doClear:=False, [readOnly]:=True),
            buffer As Stream = save.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)

            Call MSIRawPack _
                .LoadMSIFromSCiLSLab(
                    spots:=indexfile,
                    msdata:=msdatafile,
                    println:=AddressOf RunSlavePipeline.SendMessage
                ) _
                .Write(file:=buffer, version:=2)
        End Using

        Return 0
    End Function
End Module
