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
'  // COPYRIGHT: Copyright (c)  2022
'  // GUID:      9fee3ffe-05c8-4aa5-bbbb-c5ff3a18b1d6
'  // BUILT:     1/1/2000 12:00:00 AM
'  // 
' 
' 
'  < msconvert.Program 
' 
' 
' SYNOPSIS
' msconvert command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /imports-SCiLSLab:     
'  /mzPack:               Build mzPack cach
'  /rowbinds:             Combine row scans to mzPac
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
        MyBase._executableAssembly = App$
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
''' /imports-SCiLSLab --files &lt;spot_files.txt&gt; --save &lt;MSI.mzPack&gt;
''' ```
''' </summary>
'''

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
''' /mzPack --raw &lt;filepath.mzXML&gt; --cache &lt;result.mzPack&gt;
''' ```
''' Build mzPack cache
''' </summary>
'''
''' <param name="raw"> the file path of the mzML/mzXML/raw raw data file to create mzPack cache file.
''' </param>
''' <param name="cache"> the file path of the mzPack cache file.
''' </param>
Public Function convertAnyRaw(raw As String, cache As String) As Integer
Dim cli = GetconvertAnyRawCommandLine(raw:=raw, cache:=cache, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetconvertAnyRawCommandLine(raw As String, cache As String, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/mzPack")
    Call CLI.Append(" ")
    Call CLI.Append("--raw " & """" & raw & """ ")
    Call CLI.Append("--cache " & """" & cache & """ ")
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /rowbinds --files &lt;list.txt&gt; --save &lt;MSI.mzPack&gt;
''' ```
''' Combine row scans to mzPack
''' </summary>
'''
''' <param name="files"> a temp file path that its content contains selected raw data file path for each row scans.
''' </param>
''' <param name="save"> a file path for export mzPack data file.
''' </param>
Public Function MSIRowCombine(files As String, save As String) As Integer
Dim cli = GetMSIRowCombineCommandLine(files:=files, save:=save, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetMSIRowCombineCommandLine(files As String, save As String, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/rowbinds")
    Call CLI.Append(" ")
    Call CLI.Append("--files " & """" & files & """ ")
    Call CLI.Append("--save " & """" & save & """ ")
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function
End Class
End Namespace



