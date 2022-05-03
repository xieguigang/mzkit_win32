Imports System.ComponentModel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection

''' <summary>
''' 主要是为了兼容第三方厂家的原始数据文件模块的引用而构建的.NET4.8兼容模块
''' </summary>
Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("--mzPack")>
    <Description("Build mzPack cache")>
    <Argument("--raw", False, CLITypes.File, PipelineTypes.std_in, Description:="the file path of the mzML/mzXML/raw raw data file to create mzPack cache file.")>
    <Argument("--cache", False, CLITypes.File, PipelineTypes.std_out, Description:="the file path of the mzPack cache file.")>
    <Usage("--mzPack --raw <filepath.mzXML> --cache <result.mzPack>")>
    Public Function convertAnyRaw(args As CommandLine) As Integer
        Dim raw As String = args("--raw")
        Dim cache As String = args("--cache")

        Call ConvertToMzPack.CreateMzpack(raw, cache)

        Return 0
    End Function

End Module
