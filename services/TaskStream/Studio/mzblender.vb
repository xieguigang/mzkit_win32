Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.InteropService
Imports Microsoft.VisualBasic.ApplicationServices

' Microsoft VisualBasic CommandLine Code AutoGenerator
' assembly: ..\bin\mzblender.exe

' 
'  // 
'  // 
'  // 
'  // VERSION:   1.0.0.0
'  // ASSEMBLY:  mzblender, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
'  // COPYRIGHT: 
'  // GUID:      
'  // BUILT:     1/1/2000 12:00:00 AM
'  // 
' 
' 
'  < mzblender.Program >
' 
' 
' SYNOPSIS
' mzblender command [/argument argument-value...] [/@set environment-variable=value...]
' 
' All of the command that available in this program has been list below:
' 
'  /start:     Start the data visualization rendering background service for the mass spectral 
'              data rendering
' 
' 
' ----------------------------------------------------------------------------------------------------
' 
'    1. You can using "mzblender ??<commandName>" for getting more details command help.
'    2. Using command "mzblender /CLI.dev [---echo]" for CLI pipeline development.
'    3. Using command "mzblender /i" for enter interactive console mode.

Namespace CLI


    ''' <summary>
    ''' mzblender.Program
    ''' </summary>
    '''
    Public Class mzblender : Inherits InteropService

        Public Const App$ = "mzblender.exe"

        Sub New(App$)
            Call MyBase.New(app:=App$)
        End Sub

        ''' <summary>
        ''' Create an internal CLI pipeline invoker from a given environment path. 
        ''' </summary>
        ''' <param name="directory">A directory path that contains the target application</param>
        ''' <returns></returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function FromEnvironment(directory As String) As mzblender
            Return New mzblender(App:=directory & "/" & mzblender.App)
        End Function

        ''' <summary>
        ''' ```bash
        ''' /start --port &lt;tcp_port&gt; --master &lt;mzkit_win32 PID&gt;
        ''' ```
        ''' Start the data visualization rendering background service for the mass spectral data rendering.
        ''' </summary>
        '''

        Public Function StartService(port As String, master As String) As Integer
            Dim cli = GetStartServiceCommandLine(port:=port, master:=master, internal_pipelineMode:=True)
            Dim proc As IIORedirectAbstract = RunDotNetApp(cli)
            Return proc.Run()
        End Function
        Public Function GetStartServiceCommandLine(port As String, master As String, Optional internal_pipelineMode As Boolean = True) As String
            Dim CLI As New StringBuilder("/start")
            Call CLI.Append(" ")
            Call CLI.Append("--port " & """" & port & """ ")
            Call CLI.Append("--master " & """" & master & """ ")
            Call CLI.Append($"/@set --internal_pipeline={internal_pipelineMode.ToString.ToUpper()} ")


            Return CLI.ToString()
        End Function
    End Class
End Namespace

