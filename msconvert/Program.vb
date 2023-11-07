Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging.MALDI_3D
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader.DataObjects
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.CommandLine.InteropService.SharedORM
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Language.[Default]
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.My.FrameworkInternal
Imports Microsoft.VisualBasic.Scripting.Runtime

''' <summary>
''' 主要是为了兼容第三方厂家的原始数据文件模块的引用而构建的.NET4.8兼容模块
''' </summary>
<CLI> Module Program

    Sub New()
        Call Thread.Sleep(1000)
        Call FrameworkInternal.ConfigMemory(load:=MemoryLoads.Max)
    End Sub

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/check-ion-mode")>
    <Description("Check ion mode of the ms raw data file, the ion mode value will be print on the console stdout: 1 means positive, -1 means negative, 0 means pos+neg mixed scan data.")>
    <Usage("/check-ion-mode --raw <filepath.raw>")>
    Public Function checkIonMode(args As CommandLine) As Integer
        Dim raw As String = args("--raw")

        Using msraw As New MSFileReader(raw)
            Dim ions As New List(Of IonModes)
            Dim options As ThermoReaderOptions = msraw.InitReader()
            Dim rt As Double = Nothing
            Dim scanInfo As SingleScanInfo

            For i As Integer = options.MinScan To options.MaxScan
                scanInfo = msraw.GetScanInfo(scanNumber:=i)
                ions.Add(scanInfo.IonMode)
            Next

            Dim groups = ions.GroupBy(Function(i) i).ToArray

            If groups.Length = 1 Then
                If groups(Scan0).Key = IonModes.Positive Then
                    Call Console.WriteLine(1)
                ElseIf groups(Scan0).Key = IonModes.Negative Then
                    Call Console.WriteLine(-1)
                Else
                    Call Console.WriteLine(0)
                End If
            Else
                Call Console.WriteLine(0)
            End If
        End Using

        Return True
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

    <ExportAPI("/3d-imaging")>
    <Description("Convert 3D ms-imaging raw data file to mzPack.")>
    <Usage("/3d-imaging --raw <raw_data_file.imzML> [--cache <output.mzPack/output.ply/output.heap>]")>
    Public Function convert3DMsImaging(args As CommandLine) As Integer
        Dim raw As String = args <= "--raw"
        Dim output As String = args("--cache") Or $"{raw.TrimSuffix}.mzPack"

        Select Case output.ExtensionSuffix.ToLower
            Case "mzpack" : Return Imports3DMSI.FileConvert(raw, output).CLICode
            Case "ply" : Return MALDIPointCloud.FileConvert(raw, output).CLICode
            Case "heap"
                Dim cachefile As String = output.ChangeSuffix("heatmap_cache")

                If raw.ExtensionSuffix("imzml") Then
                    Call MALDIPointCloud.SaveCache(raw, cachefile)
                Else
                    cachefile = raw
                End If

                Call MALDIPointCloud.ExportHeatMapModel(cachefile, output)

                Return 0
            Case Else
                Call Console.WriteLine($"Target file format '{output.ExtensionSuffix}' is not yet supported!")
                Return 500
        End Select
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

        Call ConvertToMzPack.ConvertCDF(raw, cache, saveVer:=ver, mute:=mute, skipThumbnail:=noSnapshot)

        Return 0
    End Function

    <ExportAPI("/join_slides")>
    <Description("Join multiple slides into one slide mzpack raw data file")>
    <Usage("/join_slides --files <filelist.txt> --layout <layout.txt> [--save <union.mzPack> --filename-as-source-tag]")>
    Public Function JoinSlides(args As CommandLine) As Integer
        Dim files As String = args <= "--files"
        Dim layout As String = args <= "--layout"
        Dim save As String = args("--save") Or files.ChangeSuffix(".mzPack")
        Dim tagfileName As Boolean = args("--filename-as-source-tag")
        Dim union As mzPack = MergeSlides.JoinDataSet(files.IterateAllLines, layout.ReadAllText, tagfileName)

        Using buf As Stream = save.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Call union.Write(buf, progress:=AddressOf RunSlavePipeline.SendMessage)
        End Using

        Return True
    End Function

    Private Function getRowFiles(inputfile As String, scanExt As String) As IEnumerable(Of String)
        If inputfile.FileExists Then
            Return inputfile.ReadAllLines _
                .Select(Function(path) Strings.Trim(path).Trim(""""c)) _
                .Where(Function(str)
                           Return Not str.StringEmpty
                       End Function)
        Else
            Return inputfile.EnumerateFiles($"*.{scanExt}")
        End If
    End Function

    ''' <summary>
    ''' Convert MRM mzML file to ms-imaging raw data file
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/MRM-msimaging")>
    <Description("Convert MRM mzML file to ms-imaging raw data file")>
    <Usage("/MRM-msimaging /raw <data.mzML> /dims <x,y> [/resolution=50 /out <result.mzPack>]")>
    Public Function MRM_MSImaging(args As CommandLine) As Integer
        Dim raw As String = args("/raw")
        Dim dims As Size = args("/dims").DefaultValue.SizeParser
        Dim resolution As Integer = args("/resolution") Or 50
        Dim output As String = args("/out") Or $"{raw.ParentPath}/{raw.BaseName}_{dims.Width}x{dims.Height}@{resolution}um.mzPack"

    End Function

    <ExportAPI("/rowbinds")>
    <Description("Combine row scans to mzPack")>
    <Argument("--files", False, CLITypes.File, PipelineTypes.std_in, Description:="a temp file path that its content contains selected raw data file path for each row scans.")>
    <Argument("--save", False, CLITypes.File, PipelineTypes.std_in, Description:="a file path for export mzPack data file.")>
    <Usage("/rowbinds --files <list.txt/directory_path> --save <MSI.mzPack> [/TIC_norm /scan <default=raw> /cutoff <intensity_cutoff, default=0> /matrix_basePeak <mz, default=0> /resolution <default=17>]")>
    <Argument("/scan", True, CLITypes.String,
              Description:="This parameter only works for the directory input file. 
              used as the file extension suffix for scan in the target directory. 
              value for this argument could be: wiff, raw, mzML, mzXML, mzPack.")>
    <Argument("/matrix_basePeak", True, CLITypes.Double, Description:="zero or negative value means no removes of the matrix base ion, and the value of this parameter can also be 'auto', means auto check the matrix base ion.")>
    Public Function MSIRowCombine(args As CommandLine) As Integer
        Dim inputfile As String = args <= "--files"
        Dim scanExt As String = args <= "/scan"
        Dim files As String() = getRowFiles(inputfile, scanExt).ToArray
        Dim save As String = args("--save") Or (inputfile.ParentPath & "/" & inputfile.BaseName & ".mzPack")
        Dim cutoff As Double = args("/cutoff") Or 0.0
        Dim matrixBase As String = args <= "/matrix_basePeak"
        Dim basePeak As Double = 0.0
        Dim res As Double = args("/resolution") Or 17.0
        Dim norm As Boolean = args("/TIC_norm")

        Call Console.WriteLine(save)

        If matrixBase.IsNumeric Then
            basePeak = Val(matrixBase)
        ElseIf matrixBase.TextEquals("auto") Then
            ' measure ion by auto method
            Dim maxFile As String = files _
                .OrderByDescending(Function(path) path.FileLength) _
                .First
            Dim check = CheckMatrixBaseIon(maxFile)

            basePeak = check.ion
        End If

        Using file As FileStream = save.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            Dim buffer As mzPack = MSImagingRowBinds.MSI_rowbind(files, cutoff, basePeak, norm)

            buffer.metadata!resolution = res
            buffer.Write(file, version:=2)
        End Using

        Return 0
    End Function

    <ExportAPI("/imzml")>
    <Description("Convert raw data file to imzML file.")>
    <Usage("/imzml --file <source.data> --save <file.imzML> [/TIC_norm /cutoff <intensity_cutoff, default=0> /matrix_basePeak <mz, default=0> /resolution <default=17>]")>
    Public Function MSIToimzML(args As CommandLine) As Integer
        Dim file_handle As String = args <= "--file"
        Dim files As String() = args("--file").ReadAllLines
        Dim save As String = args("--save")
        Dim cutoff As Double = args("/cutoff") Or 0.0
        Dim basePeak As Double = args("/matrix_basePeak") Or 0.0
        Dim norm As Boolean = args("/TIC_norm")
        Dim mzpack As mzPack

        If file_handle.ExtensionSuffix("mzPack") Then
            mzpack = mzPack.ReadAll(file_handle.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
        Else
            mzpack = MSImagingRowBinds.MSI_rowbind(files, cutoff, basePeak, norm)
        End If

        Dim polygon As New Polygon2D(mzpack.MS.Select(Function(scan) scan.GetMSIPixel))
        Dim dimsize As New Size(
            width:=polygon.xpoints.Max,
            height:=polygon.ypoints.Max
        )
        Dim res As Double = args("/resolution") Or 17.0

        Using writer As imzML.mzPackWriter = imzML.mzPackWriter _
            .OpenOutput(save) _
            .SetMSImagingParameters(dimsize, res) _
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
