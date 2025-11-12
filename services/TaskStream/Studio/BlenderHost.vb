Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\blender\BlenderHost.dll

' 
'  // 
'  // 
'  // 
'  // VERSION:   1.0.0.0
'  // ASSEMBLY:  BlenderHost, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: 
'  // GUID:      
'  // BUILT:     1/1/2000 12:00:00 AM
'  // 
' 
' 
'  < BlenderHost.Program >
' 
' 
' SYNOPSIS
' BlenderHost command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /ST-imaging:     
'  /start:          Start the data visualization rendering background service for the mass spectral 
'                   data rendering
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "BlenderHost ??<commandName>" for getting more details command help.
'    2. Using command "BlenderHost /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "BlenderHost /i" for enter interactive console mode.
'    4. Using command "BlenderHost /STACK:xxMB" for adjust the application stack size, example as '/STACK:64MB'.

Namespace CLI


''' <summary>
''' BlenderHost.Program
''' </summary>
'''
Public Class BlenderHost : Inherits InteropService

    Public Const App$ = "BlenderHost.exe"

    Sub New(App$)
        Call MyBase.New(app:=App$)
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As BlenderHost
          Return New BlenderHost(App:=directory & "/" & BlenderHost.App)
     End Function

''' <summary>
''' ```bash
''' /ST-imaging --raw &lt;stimaging.mzPack&gt; [--keeps-background --targets &lt;names.txt&gt; --scale &lt;default=30&gt; --output &lt;outputdir&gt;]
''' ```
''' </summary>
'''

Public Function RenderSTImagingTargets(raw As String, 
                                          Optional targets As String = "", 
                                          Optional scale As String = "30", 
                                          Optional output As String = "", 
                                          Optional keeps_background As Boolean = False) As Integer
Dim cli = GetRenderSTImagingTargetsCommandLine(raw:=raw, 
                                          targets:=targets, 
                                          scale:=scale, 
                                          output:=output, 
                                          keeps_background:=keeps_background, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetRenderSTImagingTargetsCommandLine(raw As String, 
                                          Optional targets As String = "", 
                                          Optional scale As String = "30", 
                                          Optional output As String = "", 
                                          Optional keeps_background As Boolean = False, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/ST-imaging")
    Call CLI.Append(" ")
    Call CLI.Append("--raw " & """" & raw & """ ")
    If Not targets.StringEmpty Then
            Call CLI.Append("--targets " & """" & targets & """ ")
    End If
    If Not scale.StringEmpty Then
            Call CLI.Append("--scale " & """" & scale & """ ")
    End If
    If Not output.StringEmpty Then
            Call CLI.Append("--output " & """" & output & """ ")
    End If
    If keeps_background Then
        Call CLI.Append("--keeps-background ")
    End If
     Call CLI.Append($"/@set internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' /start --port &lt;tcp_port&gt; --master &lt;mzkit_win32 PID&gt; [--debug]
''' ```
''' Start the data visualization rendering background service for the mass spectral data rendering.
''' </summary>
'''

Public Function StartService(port As String, master As String, Optional debug As Boolean = False) As Integer
Dim cli = GetStartServiceCommandLine(port:=port, master:=master, debug:=debug, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetStartServiceCommandLine(port As String, master As String, Optional debug As Boolean = False, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("/start")
    Call CLI.Append(" ")
    Call CLI.Append("--port " & """" & port & """ ")
    Call CLI.Append("--master " & """" & master & """ ")
    If debug Then
        Call CLI.Append("--debug ")
    End If
     Call CLI.Append($"/@set internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function
End Class
End Namespace

