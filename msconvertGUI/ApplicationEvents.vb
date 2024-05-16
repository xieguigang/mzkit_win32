#Region "Microsoft.VisualBasic::0f305755a76e7b59afb456ce471d4d48, mzkit\msconvertGUI\ApplicationEvents.vb"

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

    '   Total Lines: 102
    '    Code Lines: 62
    ' Comment Lines: 27
    '   Blank Lines: 13
    '     File Size: 5.32 KB


    '     Class MyApplication
    ' 
    '         Function: CreateMSImagingTask, CreateTask, SubmitTask
    ' 
    '         Sub: task
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Task
Imports BackgroundTask = System.Threading.Tasks.Task

Namespace My
    ' The following events are available for MyApplication:
    ' Startup: Raised when the application starts, before the startup form is created.
    ' Shutdown: Raised after all application forms are closed.  This event is not raised if the application terminates abnormally.
    ' UnhandledException: Raised if the application encounters an unhandled exception.
    ' StartupNextInstance: Raised when launching a single-instance application and the application is already active. 
    ' NetworkAvailabilityChanged: Raised when the network connection is connected or disconnected.

    ' **NEW** ApplyApplicationDefaults: Raised when the application queries default values to be set for the application.

    ' Example:
    ' Private Sub MyApplication_ApplyApplicationDefaults(sender As Object, e As ApplyApplicationDefaultsEventArgs) Handles Me.ApplyApplicationDefaults
    '
    '   ' Setting the application-wide default Font:
    '   e.Font = New Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular)
    '
    '   ' Setting the HighDpiMode for the Application:
    '   e.HighDpiMode = HighDpiMode.PerMonitorV2
    '
    '   ' If a splash dialog is used, this sets the minimum display time:
    '   e.MinimumSplashScreenDisplayTime = 4000
    ' End Sub

    Partial Friend Class MyApplication

        ''' <summary>
        ''' convert LCMS/GCMS raw data files in folder batch
        ''' </summary>
        ''' <param name="sourceRoot"></param>
        ''' <param name="raw"></param>
        ''' <param name="outputdir"></param>
        ''' <param name="display"></param>
        ''' <returns></returns>
        Public Shared Function CreateTask(sourceRoot As String,
                                          raw As String,
                                          outputdir As String,
                                          display As TaskProgress) As ImportsRawData

            Dim relpath As String = PathExtensions.RelativePath(sourceRoot, raw, appendParent:=False)
            Dim outputfile As String = $"{outputdir}/{relpath.ChangeSuffix("mzPack")}"
            Dim progress As Action(Of String) = AddressOf display.ShowMessage
            Dim success As Action = Sub() display.ShowMessage("Done!")
            Dim task As New ImportsRawData(raw, progress, success, cachePath:=outputfile) With {
                .protocol = BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache.FileApplicationClass.LCMS
            }

            Return task
        End Function

        Public Shared Function SubmitTask(source As String, output As String, main As FormMain) As BackgroundTask
            Return New BackgroundTask(
                Sub()
                    Call task(source, output, main)
                End Sub)
        End Function

        Private Shared Sub task(source As String, output As String, main As FormMain)
            Dim files As String() = source.ListFiles("*.raw").ToArray

            Select Case main.CurrentTask
                Case BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache.FileApplicationClass.LCMS
                    For Each file As String In files
                        Dim progress As New TaskProgress
                        progress.Label1.Text = file.FileName
                        progress.Label2.Text = "Pending"
                        Dim task As ImportsRawData = CreateTask(source, file, output, progress)
                        task.arguments = main.arguments
                        main.AddTask(progress)
                        Call BackgroundTask.Run(AddressOf task.RunImports)
                    Next
                Case BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache.FileApplicationClass.MSImaging
                    Dim progress As New TaskProgress
                    progress.Label1.Text = source.GetDirectoryFullPath
                    progress.Label2.Text = "Pending"
                    Dim task As ImportsRawData = CreateMSImagingTask(source, output, main, progress)
                    task.arguments = main.arguments
                    main.AddTask(progress)
                    Call BackgroundTask.Run(AddressOf task.RunImports)
                Case Else
                    Throw New NotImplementedException(main.CurrentTask.Description)
            End Select
        End Sub

        Public Shared Function CreateMSImagingTask(source As String,
                                                   outputfile As String,
                                                   main As FormMain,
                                                   display As TaskProgress) As ImportsRawData

            Dim progress As Action(Of String) = AddressOf display.ShowMessage
            Dim success As Action = Sub() display.ShowMessage("Done!")
            Dim task As New ImportsRawData(source, progress, success, cachePath:=outputfile) With {
                .protocol = BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache.FileApplicationClass.MSImaging,
                .arguments = main.arguments
            }

            Return task
        End Function
    End Class
End Namespace
