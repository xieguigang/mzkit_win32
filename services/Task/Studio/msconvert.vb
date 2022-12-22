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
'  /3d-imaging:           Convert 3D ms-imaging raw data file to mzPack
'  /cdf_to_mzpack:        Convert GCMS un-targetted CDF raw data file to mzPack
'  /imports-SCiLSLab:     
'  /imzml:                Convert raw data file to imzML file
'  /join_slides:          Join multiple slides into one slide mzpack raw data file
'  /mzPack:               Build mzPack cache
'  /rowbinds:             Combine row scans to mzPack
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "msconvert ??<commandName>" for getting more details command help.
'    2. Using command "msconvert /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "msconvert /i" for enter interactive console mode.

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
''' /cdf_to_mzpack --raw &lt;filepath.cdf&gt; [--cache &lt;result.mzPack&gt; /ver 2 /mute /no-thumbnail]
''' ```
''' Convert GCMS un-targetted CDF raw data file to mzPack.
''' </summary>
'''

Public Function convertGCMSCDF(raw As String, 
                                  Optional cache As String = "", 
                                  Optional ver As String = "", 
                                  Optional mute As Boolean = False, 
                                  Optional no_thumbnail As Boolean = False) As Integer
Dim cli = GetconvertGCMSCDFCommandLine(raw:=raw, 
                                  cache:=cache, 
                                  ver:=ver, 
                                  mute:=mute, 
                                  no_thumbnail:=no_thumbnail, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetconvertGCMSCDFCommandLine(raw As String, 
                                  Optional cache As String = "", 
                                  Optional ver As String = "", 
                                  Optional mute As Boolean = False, 
                                  Optional no_thumbnail As Boolean = False, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/cdf_to_mzpack")
    Call CLI.Append(" ")
    Call CLI.Append("--raw " & """" & raw & """ ")
    If Not cache.StringEmpty Then
            Call CLI.Append("--cache " & """" & cache & """ ")
    End If
    If Not ver.StringEmpty Then
            Call CLI.Append("/ver " & """" & ver & """ ")
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
''' /imzml --file &lt;source.data&gt; --save &lt;file.imzML&gt; [/cutoff &lt;intensity_cutoff, default=0&gt; /matrix_basePeak &lt;mz, default=0&gt; /resolution &lt;default=17&gt;]
''' ```
''' Convert raw data file to imzML file.
''' </summary>
'''

Public Function MSIToimzML(file As String, 
                              save As String, 
                              Optional cutoff As String = "0", 
                              Optional matrix_basepeak As String = "0", 
                              Optional resolution As String = "17") As Integer
Dim cli = GetMSIToimzMLCommandLine(file:=file, 
                              save:=save, 
                              cutoff:=cutoff, 
                              matrix_basepeak:=matrix_basepeak, 
                              resolution:=resolution, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetMSIToimzMLCommandLine(file As String, 
                              save As String, 
                              Optional cutoff As String = "0", 
                              Optional matrix_basepeak As String = "0", 
                              Optional resolution As String = "17", Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/imzml")
    Call CLI.Append(" ")
    Call CLI.Append("--file " & """" & file & """ ")
    Call CLI.Append("--save " & """" & save & """ ")
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
    If Not matrix_basepeak.StringEmpty Then
            Call CLI.Append("/matrix_basepeak " & """" & matrix_basepeak & """ ")
    End If
    If Not resolution.StringEmpty Then
            Call CLI.Append("/resolution " & """" & resolution & """ ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /join_slides --files &lt;filelist.txt&gt; --layout &lt;layout.txt&gt; [--save &lt;union.mzPack&gt; --filename-as-source-tag]
''' ```
''' Join multiple slides into one slide mzpack raw data file
''' </summary>
'''

Public Function JoinSlides(files As String, layout As String, Optional save As String = "", Optional filename_as_source_tag As Boolean = False) As Integer
Dim cli = GetJoinSlidesCommandLine(files:=files, layout:=layout, save:=save, filename_as_source_tag:=filename_as_source_tag, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetJoinSlidesCommandLine(files As String, layout As String, Optional save As String = "", Optional filename_as_source_tag As Boolean = False, Optional internal_pipelineMode As Boolean = True) As String
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
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /mzPack --raw &lt;filepath.mzXML&gt; [--cache &lt;result.mzPack&gt; /ver 2 /mute /no-thumbnail]
''' ```
''' Build mzPack cache
''' </summary>
'''
''' <param name="raw"> the file path of the mzML/mzXML/raw raw data file to create mzPack cache file.
''' </param>
''' <param name="cache"> the file path of the mzPack cache file.
''' </param>
''' <param name="ver"> the file format version of the generated mzpack data file
''' </param>
Public Function convertAnyRaw(raw As String, 
                                 Optional cache As String = "", 
                                 Optional ver As String = "", 
                                 Optional mute As Boolean = False, 
                                 Optional no_thumbnail As Boolean = False) As Integer
Dim cli = GetconvertAnyRawCommandLine(raw:=raw, 
                                 cache:=cache, 
                                 ver:=ver, 
                                 mute:=mute, 
                                 no_thumbnail:=no_thumbnail, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetconvertAnyRawCommandLine(raw As String, 
                                 Optional cache As String = "", 
                                 Optional ver As String = "", 
                                 Optional mute As Boolean = False, 
                                 Optional no_thumbnail As Boolean = False, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/mzPack")
    Call CLI.Append(" ")
    Call CLI.Append("--raw " & """" & raw & """ ")
    If Not cache.StringEmpty Then
            Call CLI.Append("--cache " & """" & cache & """ ")
    End If
    If Not ver.StringEmpty Then
            Call CLI.Append("/ver " & """" & ver & """ ")
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
''' /rowbinds --files &lt;list.txt/directory_path&gt; --save &lt;MSI.mzPack&gt; [/scan &lt;default=raw&gt; /cutoff &lt;intensity_cutoff, default=0&gt; /matrix_basePeak &lt;mz, default=0&gt; /resolution &lt;default=17&gt;]
''' ```
''' Combine row scans to mzPack
''' </summary>
'''
''' <param name="files"> a temp file path that its content contains selected raw data file path for each row scans.
''' </param>
''' <param name="save"> a file path for export mzPack data file.
''' </param>
''' <param name="scan"> This parameter only works for the directory input file. used as the file extension suffix for scan in the target directory.
''' </param>
''' <param name="matrix_basePeak"> zero or negative value means no removes of the matrix base ion, and the value of this parameter can also be &apos;auto&apos;, means auto check the matrix base ion.
''' </param>
Public Function MSIRowCombine(files As String, 
                                 save As String, 
                                 Optional scan As String = "raw", 
                                 Optional cutoff As String = "0", 
                                 Optional matrix_basepeak As String = "0", 
                                 Optional resolution As String = "17") As Integer
Dim cli = GetMSIRowCombineCommandLine(files:=files, 
                                 save:=save, 
                                 scan:=scan, 
                                 cutoff:=cutoff, 
                                 matrix_basepeak:=matrix_basepeak, 
                                 resolution:=resolution, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetMSIRowCombineCommandLine(files As String, 
                                 save As String, 
                                 Optional scan As String = "raw", 
                                 Optional cutoff As String = "0", 
                                 Optional matrix_basepeak As String = "0", 
                                 Optional resolution As String = "17", Optional internal_pipelineMode As Boolean = True) As String
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
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function
End Class
End Namespace

