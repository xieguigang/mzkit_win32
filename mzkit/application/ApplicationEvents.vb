﻿#Region "Microsoft.VisualBasic::d8cd6f9b81d685f0993f132369bb1fad, mzkit\src\mzkit\mzkit\application\ApplicationEvents.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 389
'    Code Lines: 294
' Comment Lines: 22
'   Blank Lines: 73
'     File Size: 15.83 KB


'     Class MyApplication
' 
'         Properties: host, LogForm, mzkitRawViewer, REngine
' 
'         Function: CheckPkgFolder, getLanguageString, GetSplashScreen, LoadLibrary, SetDllDirectory
'                   SetProcessDPIAware
' 
'         Sub: doRunScript, doRunScriptWithSpecialCommand, ExecuteRScript, handleResult, InitializeREngine
'              InstallPackageRelease, LogText, MyApplication_Shutdown, MyApplication_Startup, MyApplication_UnhandledException
'              RegisterConsole, RegisterHost, RegisterOutput, RegisterPlot
' 
' 
' /********************************************************************************/

#End Region

Imports System.Globalization
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports BioNovoGene.mzkit_win32.Configuration
Imports BioNovoGene.mzkit_win32.DockSample
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ApplicationServices.Debugging.Logging
Imports Microsoft.VisualBasic.ApplicationServices.Development
Imports Microsoft.VisualBasic.ApplicationServices.Terminal
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Windows.Forms
Imports Mzkit_win32.BasicMDIForm.Container
Imports MZKitWin32.Blender.CommonLibs
Imports RDev
Imports SMRUCC.Rsharp.Interpreter
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components

Namespace My

    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.
    Partial Friend Class MyApplication

        Public Shared ReadOnly Property host As frmMain
        Public Shared ReadOnly Property LogForm As OutputWindow
        Public Shared ReadOnly Property REngine As RInterpreter

        Shared WithEvents console As Console
        Shared Rtask As Thread
        Shared cancel As New ManualResetEvent(initialState:=False)

        Public Shared ReadOnly TaskQueue As New ThreadQueue

        ''' <summary>
        ''' Enable high DPI
        ''' </summary>
        Shared ReadOnly HighDPIEnabled As Boolean = True

        ''' <summary>
        ''' Load for high DPI
        ''' </summary>
        ''' <returns></returns>
        <DllImport("user32.dll")>
        Private Shared Function SetProcessDPIAware() As Boolean
        End Function

#Region "mzkit"
        Public Shared ReadOnly Property mzkitRawViewer As PageMzkitTools
            Get
                Return _host.mzkitTool
            End Get
        End Property
#End Region

        Public Shared Sub RegisterPlot(plot As Action(Of PlotProperty),
                                       Optional width% = 2048,
                                       Optional height% = 1600,
                                       Optional padding$ = g.DefaultUltraLargePadding,
                                       Optional bg$ = "white",
                                       Optional title$ = "mzkit data plot",
                                       Optional showLegend As Boolean = True,
                                       Optional showGrid As Boolean = True,
                                       Optional gridFill$ = "white",
                                       Optional xlab$ = "X",
                                       Optional ylab$ = "Y",
                                       Optional colorSet As String = Nothing,
                                       Optional legendTitle As String = Nothing,
                                       Optional fill_curve As Boolean = True)

            Dim margin As Padding = padding
            Dim css As New CSSEnvirnment(width, height)
            Dim layout As PaddingLayout = PaddingLayout.EvaluateFromCSS(css, margin)

            With WindowModules.plotParams.params
                .width = width
                .height = height
                .background = bg.TranslateColor
                .title = title
                .xlabel = xlab
                .ylabel = ylab
                .gridFill = gridFill.TranslateColor
                .legend_title = legendTitle

                .show_legend = showLegend
                .show_grid = showGrid
                .fill_curve = fill_curve

                .padding_top = layout.Top
                .padding_right = layout.Right
                .padding_left = layout.Left
                .padding_bottom = layout.Bottom

                If colorSet.StringEmpty Then
                    colorSet = Globals.GetColors
                End If

                .colorSet = colorSet
                .colors = CategoryPalettes.NA
            End With

            WindowModules.plotParams.draw = plot
            WindowModules.plotParams.draw()(WindowModules.plotParams.params)
        End Sub

        Public Shared Sub RegisterOutput(log As OutputWindow)
            _LogForm = log

            Microsoft.VisualBasic.My.Log4VB.redirectError =
                Sub(header, message, level)
                    log.Invoke(Sub() log.AppendMessage($"[{header} {level.ToString}] {message}"))
                End Sub
            Microsoft.VisualBasic.My.Log4VB.redirectWarning =
                Sub(header, message, level)
                    log.Invoke(Sub() log.AppendMessage($"[{header} {level.ToString}] {message}"))
                End Sub
            Microsoft.VisualBasic.My.Log4VB.redirectInfo =
                Sub(header, message, level)
                    log.Invoke(Sub() log.AppendMessage($"[{header} {level.ToString}] {message}"))
                End Sub
            Microsoft.VisualBasic.My.Log4VB.redirectDebug =
                Sub(header, message, level)
                    log.Invoke(Sub() log.AppendMessage($"[{header} {level.ToString}] {message}"))
                End Sub
        End Sub

        Public Shared Sub RegisterConsole(console As Console)
            _console = console
        End Sub

        Public Shared Function GetSplashScreen() As frmSplashScreen
            For i As Integer = 0 To Application.OpenForms.Count - 1
                If TypeOf Application.OpenForms(i) Is frmSplashScreen Then
                    Return DirectCast(Application.OpenForms(i), frmSplashScreen)
                End If
            Next

            Return Nothing
        End Function

        Public Shared Sub ExecuteRScript(scriptText As String, isFile As Boolean, writeLine As Action(Of String))
            Dim result As Object

            Using buffer As New MemoryStream
                Using writer As New StreamWriter(buffer)
                    MyApplication.REngine.RedirectOutput(writer, OutputEnvironments.Html)

                    If isFile Then
                        result = MyApplication.REngine.Source(scriptText)
                    Else
                        result = MyApplication.REngine.Evaluate(scriptText)
                    End If

                    Call REngine.Print(RInterpreter.lastVariableName)

                    writer.Flush()
                    writeLine(Encoding.UTF8.GetString(buffer.ToArray))
                End Using
            End Using

            Call handleResult(result, writeLine)
        End Sub

        Private Shared Sub handleResult(result As Object, writeLine As Action(Of String))
            If TypeOf result Is Message AndAlso DirectCast(result, Message).level = MSG_TYPES.ERR Then
                Dim err As Message = result

                writeLine(err.ToString & vbCrLf)

                For i As Integer = 0 To err.message.Length - 1
                    writeLine((i + 1) & ". " & err.message(i))
                Next

                writeLine(vbCrLf)

                For Each stack In err.environmentStack
                    writeLine(stack.ToString)
                Next

                writeLine(vbCrLf)
            End If
        End Sub

        Friend Shared ReadOnly R_LIBS_USER As String = $"{App.ProductProgramData}/.settings/R#.configs.xml"

        Public Shared Sub InitializeREngine()
            Dim Rcore = GetType(RInterpreter).Assembly.FromAssembly
            Dim framework = GetType(App).Assembly.FromAssembly

            Call console.WriteLine($"
   , __           | 
  /|/  \  |  |    | Documentation: https://r_lang.dev.SMRUCC.org/
   |___/--+--+--  |
   | \  --+--+--  | Version {Rcore.AssemblyVersion} ({Rcore.BuiltTime.ToString})
   |  \_/ |  |    | sciBASIC.NET Runtime: {framework.AssemblyVersion}         
                  
Welcome to the R# language
")
            Call console.WriteLine("Type 'demo()' for some demos, 'help()' for on-line help, or
'help.start()' for an HTML browser interface to help.
Type 'q()' to quit R.
")

            _REngine = RInterpreter.FromEnvironmentConfiguration(configs:=R_LIBS_USER)

            If REngine.globalEnvir.packages.AsEnumerable.Count = 0 Then
                Dim Rhost As String = $"{App.HOME}/Rstudio/bin/R#.exe"

                ' no packages ...
                ' init R# engine environment
                Call PipelineProcess.CreatePipeline(Rhost, $"--reset --R_LIBS_USER {R_LIBS_USER.CLIPath}").WaitForExit()
                Call PipelineProcess.CreatePipeline(Rhost, $"--setup --R_LIBS_USER {R_LIBS_USER.CLIPath}").WaitForExit()

                ' and then reload
                _REngine = RInterpreter.FromEnvironmentConfiguration(configs:=R_LIBS_USER)

                ' install the required packages
                Call VisualStudio.InstallInternalRPackages()
            End If

            _REngine.strict = False

            _REngine.LoadLibrary("base")
            _REngine.LoadLibrary("utils")
            _REngine.LoadLibrary("grDevices")
            _REngine.LoadLibrary("stats")

            _REngine.Evaluate("imports 'gtk' from 'Rstudio';")

            _REngine.LoadLibrary(GetType(MyApplication))

            AddHandler console.CancelKeyPress,
                Sub()
                    ' ctrl + C just break the current executation
                    ' not exit program running
                    cancel.Set()

                    If Not Rtask Is Nothing Then
                        Rtask.Abort()
                    End If
                End Sub

            Call New Thread(AddressOf New Shell(New PS1("> "), AddressOf doRunScriptWithSpecialCommand, dev:=console) With {.Quite = "!.R#::quit" & Rnd()}.Run).Start()
            Call DescriptionTooltip.SetEngine(REngine)
        End Sub

        Private Shared Sub doRunScriptWithSpecialCommand(script As String)
            Select Case script
                Case "CLS"
                    Call console.Clear()
                Case Else
                    If Not script.StringEmpty Then
                        Rtask = New Thread(Sub() Call doRunScript(script))
                        Rtask.Start()

                        ' block the running task thread at here
                        cancel.Reset()
                        cancel.WaitOne()
                    Else
                        console.WriteLine()
                    End If
            End Select
        End Sub

        Private Shared Sub doRunScript(script As String)
            Call ExecuteRScript(script, isFile:=False, AddressOf console.WriteLine)
            Call cancel.Set()
        End Sub

        ''' <summary>
        ''' writeline to the log output window
        ''' </summary>
        ''' <param name="msg"></param>
        Public Overloads Shared Sub LogText(msg As String)
            LogForm.AppendMessage(msg)
        End Sub

        Public Shared Sub RegisterHost(host As frmMain)
            MyApplication._host = host
        End Sub

        Public Shared Sub CloseMSIEngine()
            If Not MyApplication.host Is Nothing Then
                For Each doc In MyApplication.host.DockPanel.Documents
                    If TypeOf doc Is frmMsImagingViewer Then
                        If Not DirectCast(doc, frmMsImagingViewer).MSIservice Is Nothing Then
                            Call DirectCast(doc, frmMsImagingViewer).MSIservice.CloseMSIEngine()
                        End If
                    End If
                Next
            End If
        End Sub

        Private Sub MyApplication_UnhandledException(sender As Object, e As UnhandledExceptionEventArgs) Handles Me.UnhandledException
            If Not AppEnvironment.IsDevelopmentMode Then
                Call App.LogException(e.Exception)
                Call MessageBox.Show(e.Exception.ToString, "Unknown Error!", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Call CloseMSIEngine()
                Call ServiceHub.Manager.Shutdown()

                Try
                    Call host.SaveSettings()
                    Call WindowModules.fileExplorer.SaveFileCache(
                        Sub()
                            ' do nothing
                        End Sub)
                Catch ex As Exception

                End Try
                End
            End If
        End Sub

        Friend Shared ReadOnly pkgs As String() = {"mzkit.zip", "REnv.zip", "ggplot.zip"}

        Public Shared Function CheckPkgFolder(ParamArray checkFiles As String()) As String
            Dim dirRelease As String = $"{App.HOME}/Rstudio/packages"
            Dim dirDev As String = $"{App.HOME}/../../src/mzkit/setup/"
            Dim dirDev2 As String = $"{App.HOME}/../../src/mzkit/setup/Rstudio"

            If checkFiles.Any(Function(fileName) $"{dirRelease}/{fileName}".FileExists) Then
                Return dirRelease
            ElseIf checkFiles.Any(Function(fileName) $"{dirDev}/{fileName}".FileExists) Then
                Return dirDev
            Else
                Return dirDev2
            End If
        End Function

        ''' <summary>
        ''' apply for handling of the application commandline arguments
        ''' </summary>
        Friend Shared afterLoad As Action

        Public Shared Function getCurrentLanguageString(key As String) As String
            Return getLanguageString(key, Globals.Settings.ui.language)
        End Function

        Public Shared Function getLanguageString(key As String, lang As Languages) As String
            Select Case lang
                Case Languages.Chinese
                    Return My.Resources.ResourceManager.GetString($"{key}_zh")
                Case Languages.English
                    Return My.Resources.ResourceManager.GetString($"{key}_en")
                Case Else
                    Return My.Resources.ResourceManager.GetString($"{key}_en")
            End Select
        End Function

        ''' <summary>
        ''' should be less than 2GB
        ''' </summary>
        ''' <returns></returns>
        Public Shared ReadOnly Property buffer_size As Integer = 64 * ByteSize.MB
        Public Shared ReadOnly Property debugMode As Boolean
            Get
                Dim cli = App.CommandLine

                Return cli.Name.TextEquals("--debug") OrElse cli.ContainsParameter("--debug")
            End Get
        End Property

        Private Sub MyApplication_Startup(sender As Object, e As StartupEventArgs) Handles Me.Startup
            Dim cli = App.CommandLine
            Dim config = Globals.Settings

            If config.ui Is Nothing Then
                config.ui = New UISettings
            End If

            Select Case config.ui.language
                Case Languages.Chinese
                    Thread.CurrentThread.CurrentUICulture = New CultureInfo("zh-CN")
                    CultureInfo.DefaultThreadCurrentUICulture = New CultureInfo("zh-CN")
                Case Languages.English
                    Thread.CurrentThread.CurrentUICulture = New CultureInfo("en-US")
                    CultureInfo.DefaultThreadCurrentUICulture = New CultureInfo("en-US")
                Case Else
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentUICulture
                    CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentUICulture
            End Select

            If Not (cli Is Nothing OrElse cli.IsNullOrEmpty) Then
                If cli.Name.FileExists Then
                    Call BioNovoGene.mzkit_win32.CLI.openRawFile(cli.Name, cli)
                ElseIf cli.Name.TextEquals("--debug") Then
                    ServiceHub.MSIDataService.debugPort = cli.GetDataReader.GetInt32("--port")
                ElseIf cli.Name.TextEquals("--devtools") Then
                    Call BioNovoGene.mzkit_win32.CLI.openDevTools()
                ElseIf cli.Name.TextEquals("--debug-plugin") Then
                    Call BioNovoGene.mzkit_win32.CLI.debugPluginPage(cli.GetString("--plugin"))
                ElseIf cli.Name.TextEquals("--deep_zoom") Then
                    Call BioNovoGene.mzkit_win32.CLI.createDeepzoomImage(img:=cli.GetString("--image"))
                End If

                If cli.ContainsParameter("--buffer_size") Then
                    _buffer_size = Unit.ParseByteSize(cli("--buffer_size") Or "64MB")
                ElseIf cli.Name.TextEquals("--buffer_size") Then
                    _buffer_size = Unit.ParseByteSize(cli.Parameters.ElementAtOrDefault(0, "64MB"))
                End If
            End If

            Call AppEnvironment.SetExternalCDllDirectory($"{App.HOME}/tools/cpp/")

            ' start background services
            Call RedisService.Start()

            If Not Globals.Settings.local_blender Then
                Call RenderService.Start()
            End If
        End Sub

        Private Sub MyApplication_Shutdown(sender As Object, e As EventArgs) Handles Me.Shutdown
            Call CloseMSIEngine()
            Call ServiceHub.Manager.Shutdown()
        End Sub
    End Class
End Namespace
