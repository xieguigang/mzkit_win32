Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\msconvert.exe

' 
'  // 
'  // 
'  // 
'  // VERSION:   1.0.0.0
'  // ASSEMBLY:  msconvert, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: Copyright (c) gg.xie@bionovogene.com 2022
'  // GUID:      9fee3ffe-05c8-4aa5-bbbb-c5ff3a18b1d6
'  // BUILT:     1/1/2000 12:00:00 AM
'  // 
' 
' 
'  < msconvert.Program >
' 
' 
' SYNOPSIS
' msconvert command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /3d-imaging:            Convert 3D ms-imaging raw data file to mzPack
'  /cdf_to_mzpack:         Convert GCMS un-targetted CDF or GCxGC raw data file to mzPack
'  /check-ion-mode:        Check ion mode of the ms raw data file, the ion mode value will be 
'                          print on the console stdout: 1 means positive, -1 means negative, 0 
'                          means pos+neg mixed scan data
'  /imports-SCiLSLab:      
'  /imzml:                 Convert raw data file to imzML file
'  /join_slides:           Join multiple slides into one slide mzpack raw data file
'  /MRM-msimaging:         Convert MRM mzML file to ms-imaging raw data file
'  /msi_pack:              Pack the imzML file as the mzkit MS-Imaging mzpack rawdata file
'  /mzPack:                Build mzPack cache from vendor file format, example like Thermo raw 
'                          data file convert to bionovogene mzpack
'  /pack.single_cells:     Pack the spatial metabolism rawdata as the single cells dataset
'  /rowbinds:              Combine row scans to mzPack
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "msconvert ??<commandName>" for getting more details command help.
'    2. Using command "msconvert /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "msconvert /i" for enter interactive console mode.
'    4. Using command "msconvert /STACK:xxMB" for adjust the application stack size, example as '/STACK:64MB'.

Namespace CLI


''' <summary>
''' msconvert.Program
''' </summary>
'''
Public Class msconvert : Inherits InteropService

    Public Const App$ = "msconvert.exe"

    Sub New(App$)
        Call MyBase.New(app:=App$)
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As msconvert
          Return New msconvert(App:=directory & "/" & msconvert.App)
     End Function

''' <summary>
''' ```bash
''' /3d-imaging --raw &lt;raw_data_file.imzML&gt; [--cache &lt;output.mzPack/output.ply/output.heap&gt;]
''' ```
''' Convert 3D ms-imaging raw data file to mzPack.
''' </summary>
'''

Public Function convert3DMsImaging(raw As String, Optional cache As String = "") As Integer
Dim cli = Getconvert3DMsImagingCommandLine(raw:=raw, cache:=cache, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function Getconvert3DMsImagingCommandLine(raw As String, Optional cache As String = "", Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/3d-imaging")
    Call CLI.Append(" ")
    Call CLI.Append("--raw " & """" & raw & """ ")
    If Not cache.StringEmpty Then
            Call CLI.Append("--cache " & """" & cache & """ ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /cdf_to_mzpack --raw &lt;filepath.cdf&gt; [--cache &lt;result.mzPack&gt; /gcxgc /modtime &lt;default=4&gt; /ver 2 /mute /no-thumbnail]
''' ```
''' Convert GCMS un-targetted CDF or GCxGC raw data file to mzPack.
''' </summary>
'''

Public Function convertGCMSCDF(raw As String, 
                                  Optional cache As String = "", 
                                  Optional modtime As String = "4", 
                                  Optional ver As String = "", 
                                  Optional gcxgc As Boolean = False, 
                                  Optional mute As Boolean = False, 
                                  Optional no_thumbnail As Boolean = False) As Integer
Dim cli = GetconvertGCMSCDFCommandLine(raw:=raw, 
                                  cache:=cache, 
                                  modtime:=modtime, 
                                  ver:=ver, 
                                  gcxgc:=gcxgc, 
                                  mute:=mute, 
                                  no_thumbnail:=no_thumbnail, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetconvertGCMSCDFCommandLine(raw As String, 
                                  Optional cache As String = "", 
                                  Optional modtime As String = "4", 
                                  Optional ver As String = "", 
                                  Optional gcxgc As Boolean = False, 
                                  Optional mute As Boolean = False, 
                                  Optional no_thumbnail As Boolean = False, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/cdf_to_mzpack")
    Call CLI.Append(" ")
    Call CLI.Append("--raw " & """" & raw & """ ")
    If Not cache.StringEmpty Then
            Call CLI.Append("--cache " & """" & cache & """ ")
    End If
    If Not modtime.StringEmpty Then
            Call CLI.Append("/modtime " & """" & modtime & """ ")
    End If
    If Not ver.StringEmpty Then
            Call CLI.Append("/ver " & """" & ver & """ ")
    End If
    If gcxgc Then
        Call CLI.Append("/gcxgc ")
    End If
    If mute Then
        Call CLI.Append("/mute ")
    End If
    If no_thumbnail Then
        Call CLI.Append("/no-thumbnail ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /check-ion-mode --raw &lt;filepath.raw&gt;
''' ```
''' Check ion mode of the ms raw data file, the ion mode value will be print on the console stdout: 1 means positive, -1 means negative, 0 means pos+neg mixed scan data.
''' </summary>
'''

Public Function checkIonMode(raw As String) As Integer
Dim cli = GetcheckIonModeCommandLine(raw:=raw, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetcheckIonModeCommandLine(raw As String, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/check-ion-mode")
    Call CLI.Append(" ")
    Call CLI.Append("--raw " & """" & raw & """ ")
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /imports-SCiLSLab --files &lt;spot_files.txt&gt; --save &lt;MSI.mzPack&gt;
''' ```
''' </summary>
'''
''' <param name="files"> a list of the csv data files, the spot index and
'''               the mass data should be paired in this data file list. each line 
'''               of this text file is a tuple of the spot index and the ms data, 
'''               and the tuple data should be used the &lt;TAB&gt; as delimiter.
''' </param>
Public Function ImportsSCiLSLab(files As String, save As String) As Integer
Dim cli = GetImportsSCiLSLabCommandLine(files:=files, save:=save, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetImportsSCiLSLabCommandLine(files As String, save As String, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/imports-SCiLSLab")
    Call CLI.Append(" ")
    Call CLI.Append("--files " & """" & files & """ ")
    Call CLI.Append("--save " & """" & save & """ ")
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /imzml --file &lt;source.data&gt; --save &lt;file.imzML&gt; [/TIC_norm /ionMode &lt;1/-1, default=1&gt; /cutoff &lt;intensity_cutoff, default=0&gt; /matrix_basePeak &lt;mz, default=0&gt; /resolution &lt;default=17&gt;]
''' ```
''' Convert raw data file to imzML file.
''' </summary>
'''
''' <param name="file"> the source data file inputs, could be a MZKit mzpack rawdata file or a text file contains the vendor raw data file to combine.
''' </param>
''' <param name="save"> the file location path of the imzML and ibd rawdata file to export.
''' </param>
''' <param name="ionMode"> the polarity mode of the ms data. value could be 1 for positive and -1 for negative
''' </param>
Public Function MSIToimzML(file As String, 
                              save As String, 
                              Optional ionmode As String = "1", 
                              Optional cutoff As String = "0", 
                              Optional matrix_basepeak As String = "0", 
                              Optional resolution As String = "17", 
                              Optional tic_norm As Boolean = False) As Integer
Dim cli = GetMSIToimzMLCommandLine(file:=file, 
                              save:=save, 
                              ionmode:=ionmode, 
                              cutoff:=cutoff, 
                              matrix_basepeak:=matrix_basepeak, 
                              resolution:=resolution, 
                              tic_norm:=tic_norm, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetMSIToimzMLCommandLine(file As String, 
                              save As String, 
                              Optional ionmode As String = "1", 
                              Optional cutoff As String = "0", 
                              Optional matrix_basepeak As String = "0", 
                              Optional resolution As String = "17", 
                              Optional tic_norm As Boolean = False, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/imzml")
    Call CLI.Append(" ")
    Call CLI.Append("--file " & """" & file & """ ")
    Call CLI.Append("--save " & """" & save & """ ")
    If Not ionmode.StringEmpty Then
            Call CLI.Append("/ionmode " & """" & ionmode & """ ")
    End If
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
    If Not matrix_basepeak.StringEmpty Then
            Call CLI.Append("/matrix_basepeak " & """" & matrix_basepeak & """ ")
    End If
    If Not resolution.StringEmpty Then
            Call CLI.Append("/resolution " & """" & resolution & """ ")
    End If
    If tic_norm Then
        Call CLI.Append("/tic_norm ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /join_slides --files &lt;filelist.txt&gt; --layout &lt;layout.txt&gt; [--save &lt;union.mzPack&gt; --filename-as-source-tag --normalize]
''' ```
''' Join multiple slides into one slide mzpack raw data file
''' </summary>
'''

Public Function JoinSlides(files As String, 
                              layout As String, 
                              Optional save As String = "", 
                              Optional filename_as_source_tag As Boolean = False, 
                              Optional normalize As Boolean = False) As Integer
Dim cli = GetJoinSlidesCommandLine(files:=files, 
                              layout:=layout, 
                              save:=save, 
                              filename_as_source_tag:=filename_as_source_tag, 
                              normalize:=normalize, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetJoinSlidesCommandLine(files As String, 
                              layout As String, 
                              Optional save As String = "", 
                              Optional filename_as_source_tag As Boolean = False, 
                              Optional normalize As Boolean = False, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/join_slides")
    Call CLI.Append(" ")
    Call CLI.Append("--files " & """" & files & """ ")
    Call CLI.Append("--layout " & """" & layout & """ ")
    If Not save.StringEmpty Then
            Call CLI.Append("--save " & """" & save & """ ")
    End If
    If filename_as_source_tag Then
        Call CLI.Append("--filename-as-source-tag ")
    End If
    If normalize Then
        Call CLI.Append("--normalize ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /MRM-msimaging /raw &lt;data.mzML&gt; /dims &lt;x,y&gt; [/resolution=50 /out &lt;result.mzPack&gt;]
''' ```
''' Convert MRM mzML file to ms-imaging raw data file
''' </summary>
'''

Public Function MRM_MSImaging(raw As String, dims As String, Optional resolution As String = "", Optional out As String = "") As Integer
Dim cli = GetMRM_MSImagingCommandLine(raw:=raw, dims:=dims, resolution:=resolution, out:=out, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetMRM_MSImagingCommandLine(raw As String, dims As String, Optional resolution As String = "", Optional out As String = "", Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/MRM-msimaging")
    Call CLI.Append(" ")
    Call CLI.Append("/raw " & """" & raw & """ ")
    Call CLI.Append("/dims " & """" & dims & """ ")
    If Not resolution.StringEmpty Then
            Call CLI.Append("/resolution " & """" & resolution & """ ")
    End If
    If Not out.StringEmpty Then
            Call CLI.Append("/out " & """" & out & """ ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /msi_pack /target &lt;file.imzML&gt; [ /dims &lt;w,h,default=NULL&gt; /default_ion &lt;1/-1&gt; /fly_stream &lt;auto/true/false, default=auto&gt; /centroid &lt;da/ppm:mzdiff,default=da:0.01&gt; /noiseless &lt;percentage cutoff,default=0.001&gt; /output &lt;result.mzPack&gt;]
''' ```
''' Pack the imzML file as the mzkit MS-Imaging mzpack rawdata file
''' </summary>
'''
''' <param name="dims"> Set the image dimension size for the ms-imaging data pack output, this options apply for the rawdata which is not a imzML file.
''' </param>
''' <param name="fly_stream"> deal with the ultra large size imzML rawdata file in stream mode? auto mode means auto switch to fly stream mode when 
'''               the ibd rawdata file size is greater than 4GB. Some metadata will be lost in fly stream mode.
''' </param>
Public Function MSIPack(target As String, 
                           Optional dims As String = "NULL", 
                           Optional default_ion As String = "", 
                           Optional fly_stream As String = "auto", 
                           Optional centroid As String = "da:0.01", 
                           Optional noiseless As String = "0.001", 
                           Optional output As String = "") As Integer
Dim cli = GetMSIPackCommandLine(target:=target, 
                           dims:=dims, 
                           default_ion:=default_ion, 
                           fly_stream:=fly_stream, 
                           centroid:=centroid, 
                           noiseless:=noiseless, 
                           output:=output, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetMSIPackCommandLine(target As String, 
                           Optional dims As String = "NULL", 
                           Optional default_ion As String = "", 
                           Optional fly_stream As String = "auto", 
                           Optional centroid As String = "da:0.01", 
                           Optional noiseless As String = "0.001", 
                           Optional output As String = "", Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/msi_pack")
    Call CLI.Append(" ")
    Call CLI.Append("/target " & """" & target & """ ")
    If Not dims.StringEmpty Then
            Call CLI.Append("/dims " & """" & dims & """ ")
    End If
    If Not default_ion.StringEmpty Then
            Call CLI.Append("/default_ion " & """" & default_ion & """ ")
    End If
    If Not fly_stream.StringEmpty Then
            Call CLI.Append("/fly_stream " & """" & fly_stream & """ ")
    End If
    If Not centroid.StringEmpty Then
            Call CLI.Append("/centroid " & """" & centroid & """ ")
    End If
    If Not noiseless.StringEmpty Then
            Call CLI.Append("/noiseless " & """" & noiseless & """ ")
    End If
    If Not output.StringEmpty Then
            Call CLI.Append("/output " & """" & output & """ ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /mzPack --raw &lt;filepath.mzXML&gt; [--cache &lt;result.mzPack&gt; /ver 2 /mute /no-thumbnail /tree /prefix &lt;prefix-string&gt; --debug]
''' ```
''' Build mzPack cache from vendor file format, example like Thermo raw data file convert to bionovogene mzpack.
''' </summary>
'''
''' <param name="raw"> the file path of the mzML/mzXML/raw raw data file to create mzPack cache file.
''' </param>
''' <param name="cache"> the file path of the mzPack cache file.
''' </param>
''' <param name="ver"> the file format version of the generated mzpack data file
''' </param>
''' <param name="prefix"> the result mzpack file its filename prefix, default is empty string.
'''               this argument only works when the input rawdata source is a directory.
''' </param>
''' <param name="tree"> this argument only works when the given raw data source is a directory for 
'''               convert data file in batch mode. set this argument means search the rawdata file in all 
'''               sub-directory and put the result mzpack file to output folder also keeps the directory 
'''               tree structure.
''' </param>
Public Function convertAnyRaw(raw As String, 
                                 Optional cache As String = "", 
                                 Optional ver As String = "", 
                                 Optional prefix As String = "", 
                                 Optional mute As Boolean = False, 
                                 Optional no_thumbnail As Boolean = False, 
                                 Optional tree As Boolean = False, 
                                 Optional debug As Boolean = False) As Integer
Dim cli = GetconvertAnyRawCommandLine(raw:=raw, 
                                 cache:=cache, 
                                 ver:=ver, 
                                 prefix:=prefix, 
                                 mute:=mute, 
                                 no_thumbnail:=no_thumbnail, 
                                 tree:=tree, 
                                 debug:=debug, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetconvertAnyRawCommandLine(raw As String, 
                                 Optional cache As String = "", 
                                 Optional ver As String = "", 
                                 Optional prefix As String = "", 
                                 Optional mute As Boolean = False, 
                                 Optional no_thumbnail As Boolean = False, 
                                 Optional tree As Boolean = False, 
                                 Optional debug As Boolean = False, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/mzPack")
    Call CLI.Append(" ")
    Call CLI.Append("--raw " & """" & raw & """ ")
    If Not cache.StringEmpty Then
            Call CLI.Append("--cache " & """" & cache & """ ")
    End If
    If Not ver.StringEmpty Then
            Call CLI.Append("/ver " & """" & ver & """ ")
    End If
    If Not prefix.StringEmpty Then
            Call CLI.Append("/prefix " & """" & prefix & """ ")
    End If
    If mute Then
        Call CLI.Append("/mute ")
    End If
    If no_thumbnail Then
        Call CLI.Append("/no-thumbnail ")
    End If
    If tree Then
        Call CLI.Append("/tree ")
    End If
    If debug Then
        Call CLI.Append("--debug ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /pack.single_cells /rawdata &lt;file.mzPack&gt; /tissue &lt;tissue_cluster.cdf&gt; [/save &lt;output.mzPack&gt;]
''' ```
''' Pack the spatial metabolism rawdata as the single cells dataset.
''' </summary>
'''

Public Function PackSingleCells(rawdata As String, tissue As String, Optional save As String = "") As Integer
Dim cli = GetPackSingleCellsCommandLine(rawdata:=rawdata, tissue:=tissue, save:=save, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetPackSingleCellsCommandLine(rawdata As String, tissue As String, Optional save As String = "", Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/pack.single_cells")
    Call CLI.Append(" ")
    Call CLI.Append("/rawdata " & """" & rawdata & """ ")
    Call CLI.Append("/tissue " & """" & tissue & """ ")
    If Not save.StringEmpty Then
            Call CLI.Append("/save " & """" & save & """ ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /rowbinds --files &lt;list.txt/directory_path&gt; --save &lt;MSI.mzPack&gt; [/TIC_norm /scan &lt;default=raw&gt; /cutoff &lt;intensity_cutoff, default=0&gt; /matrix_basePeak &lt;mz, default=0&gt; /resolution &lt;default=17&gt;]
''' ```
''' Combine row scans to mzPack
''' </summary>
'''
''' <param name="files"> a temp file path that its content contains selected raw data file path for each row scans.
''' </param>
''' <param name="save"> a file path for export mzPack data file.
''' </param>
''' <param name="scan"> This parameter only works for the directory input file. 
'''               used as the file extension suffix for scan in the target directory. 
'''               value for this argument could be: wiff, raw, mzML, mzXML, mzPack.
''' </param>
''' <param name="matrix_basePeak"> zero or negative value means no removes of the matrix base ion, and the value of this parameter can also be &apos;auto&apos;, means auto check the matrix base ion.
''' </param>
Public Function MSIRowCombine(files As String, 
                                 save As String, 
                                 Optional scan As String = "raw", 
                                 Optional cutoff As String = "0", 
                                 Optional matrix_basepeak As String = "0", 
                                 Optional resolution As String = "17", 
                                 Optional tic_norm As Boolean = False) As Integer
Dim cli = GetMSIRowCombineCommandLine(files:=files, 
                                 save:=save, 
                                 scan:=scan, 
                                 cutoff:=cutoff, 
                                 matrix_basepeak:=matrix_basepeak, 
                                 resolution:=resolution, 
                                 tic_norm:=tic_norm, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetMSIRowCombineCommandLine(files As String, 
                                 save As String, 
                                 Optional scan As String = "raw", 
                                 Optional cutoff As String = "0", 
                                 Optional matrix_basepeak As String = "0", 
                                 Optional resolution As String = "17", 
                                 Optional tic_norm As Boolean = False, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/rowbinds")
    Call CLI.Append(" ")
    Call CLI.Append("--files " & """" & files & """ ")
    Call CLI.Append("--save " & """" & save & """ ")
    If Not scan.StringEmpty Then
            Call CLI.Append("/scan " & """" & scan & """ ")
    End If
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
    If Not matrix_basepeak.StringEmpty Then
            Call CLI.Append("/matrix_basepeak " & """" & matrix_basepeak & """ ")
    End If
    If Not resolution.StringEmpty Then
            Call CLI.Append("/resolution " & """" & resolution & """ ")
    End If
    If tic_norm Then
        Call CLI.Append("/tic_norm ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function
End Class
End Namespace

