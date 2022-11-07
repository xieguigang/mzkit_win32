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
'  /cdf_to_mzpack:        Convert GCMS un-targetted CDF raw data file to mzPack
'  /imports-SCiLSLab:     
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
''' /rowbinds --files &lt;list.txt&gt; --save &lt;MSI.mzPack&gt; [/cutoff &lt;intensity_cutoff, default=0&gt;]
''' ```
''' Combine row scans to mzPack
''' </summary>
'''
''' <param name="files"> a temp file path that its content contains selected raw data file path for each row scans.
''' </param>
''' <param name="save"> a file path for export mzPack data file.
''' </param>
Public Function MSIRowCombine(files As String, save As String, Optional cutoff As String = "0") As Integer
Dim cli = GetMSIRowCombineCommandLine(files:=files, save:=save, cutoff:=cutoff, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetMSIRowCombineCommandLine(files As String, save As String, Optional cutoff As String = "0", Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/rowbinds")
    Call CLI.Append(" ")
    Call CLI.Append("--files " & """" & files & """ ")
    Call CLI.Append("--save " & """" & save & """ ")
    If Not cutoff.StringEmpty Then
            Call CLI.Append("/cutoff " & """" & cutoff & """ ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function
End Class
End Namespace

