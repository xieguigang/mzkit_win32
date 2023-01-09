Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\Rserve.dll

' 
'  // 
'  // 
'  // 
'  // VERSION:   1.0.0.0
'  // ASSEMBLY:  Rserve, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: 
'  // GUID:      
'  // BUILT:     1/1/2000 12:00:00 AM
'  // 
' 
' 
'  < Rserve.Program >
' 
' 
' SYNOPSIS
' Rserve command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  --listen:      Start a local static web server for hosting statics web page files
'  --session:     Run GCModeller workbench R# backend session
'  --start:       Start R# web services, host R# script with http get request
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "Rserve ??<commandName>" for getting more details command help.
'    2. Using command "Rserve /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "Rserve /i" for enter interactive console mode.

Namespace CLI


''' <summary>
''' Rserve.Program
''' </summary>
'''
Public Class Rserve : Inherits InteropService

    Public Const App$ = "Rserve.exe"

    Sub New(App$)
        Call MyBase.New(app:=App$)
    End Sub
        
''' <summary>
''' Create an internal CLI pipeline invoker from a given environment path. 
''' </summary>
''' <param name="directory">A directory path that contains the target application</param>
''' <returns></returns>
     <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Function FromEnvironment(directory As String) As Rserve
          Return New Rserve(App:=directory & "/" & Rserve.App)
     End Function

''' <summary>
''' ```bash
''' --listen /wwwroot &lt;directory_path&gt; [--attach &lt;other_directory_path&gt; --parent &lt;parent_process_id&gt; /port &lt;http_port, default=80&gt;]
''' ```
''' Start a local static web server for hosting statics web page files
''' </summary>
'''

Public Function listen(wwwroot As String, Optional attach As String = "", Optional parent As String = "", Optional port As String = "80") As Integer
Dim cli = GetlistenCommandLine(wwwroot:=wwwroot, attach:=attach, parent:=parent, port:=port, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetlistenCommandLine(wwwroot As String, Optional attach As String = "", Optional parent As String = "", Optional port As String = "80", Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("--listen")
    Call CLI.Append(" ")
    Call CLI.Append("/wwwroot " & """" & wwwroot & """ ")
    If Not attach.StringEmpty Then
            Call CLI.Append("--attach " & """" & attach & """ ")
    End If
    If Not parent.StringEmpty Then
            Call CLI.Append("--parent " & """" & parent & """ ")
    End If
    If Not port.StringEmpty Then
            Call CLI.Append("/port " & """" & port & """ ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' --session [--port &lt;port number, default=8848&gt; --workspace &lt;directory, default=./&gt;]
''' ```
''' Run GCModeller workbench R# backend session.
''' </summary>
'''

Public Function runSession(Optional port As String = "8848", Optional workspace As String = "./") As Integer
Dim cli = GetrunSessionCommandLine(port:=port, workspace:=workspace, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetrunSessionCommandLine(Optional port As String = "8848", Optional workspace As String = "./", Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("--session")
    Call CLI.Append(" ")
    If Not port.StringEmpty Then
            Call CLI.Append("--port " & """" & port & """ ")
    End If
    If Not workspace.StringEmpty Then
            Call CLI.Append("--workspace " & """" & workspace & """ ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function

''' <summary>
''' ```bash
''' --start [--port &lt;port number, default=7452&gt; --tcp &lt;port_number, default=3838&gt; --Rweb &lt;directory, default=./Rweb&gt; --startups &lt;packageNames, default=&quot;&quot;&gt; --show_error --n_threads &lt;max_threads, default=8&gt;]
''' ```
''' Start R# web services, host R# script with http get request.
''' </summary>
'''

Public Function start(Optional port As String = "7452", 
                         Optional tcp As String = "3838", 
                         Optional rweb As String = "./Rweb", 
                         Optional startups As String = "", 
                         Optional n_threads As String = "8", 
                         Optional show_error As Boolean = False) As Integer
Dim cli = GetstartCommandLine(port:=port, 
                         tcp:=tcp, 
                         rweb:=rweb, 
                         startups:=startups, 
                         n_threads:=n_threads, 
                         show_error:=show_error, internal_pipelineMode:=True)
    Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
    Return proc.Run()
End Function
Public Function GetstartCommandLine(Optional port As String = "7452", 
                         Optional tcp As String = "3838", 
                         Optional rweb As String = "./Rweb", 
                         Optional startups As String = "", 
                         Optional n_threads As String = "8", 
                         Optional show_error As Boolean = False, Optional internal_pipelineMode As Boolean = True) As String
    Dim CLI As New StringBuilder("--start")
    Call CLI.Append(" ")
    If Not port.StringEmpty Then
            Call CLI.Append("--port " & """" & port & """ ")
    End If
    If Not tcp.StringEmpty Then
            Call CLI.Append("--tcp " & """" & tcp & """ ")
    End If
    If Not rweb.StringEmpty Then
            Call CLI.Append("--rweb " & """" & rweb & """ ")
    End If
    If Not startups.StringEmpty Then
            Call CLI.Append("--startups " & """" & startups & """ ")
    End If
    If Not n_threads.StringEmpty Then
            Call CLI.Append("--n_threads " & """" & n_threads & """ ")
    End If
    If show_error Then
        Call CLI.Append("--show_error ")
    End If
     Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


Return CLI.ToString()
End Function
End Class
End Namespace

