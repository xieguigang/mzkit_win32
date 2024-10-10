Imports ImageUtils
Imports System.Diagnostics
Imports System.Drawing
Imports Image = System.Drawing.Image
Imports System.IO

Namespace nv

    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    Public Class NVIDIAAnsel
        Private forceMode As Boolean = False
        Private supportLevel As SupportLevel
        Private systemCheckComplete As Boolean = False
        Private outputFolderMode As Boolean = False
        Private threadCount As Integer = 1
        Private tasksCompleted As Integer = 0
        Private failedImages As Integer = 0
        Private firstTime As Boolean
        Private ReadOnly imagePaths As List(Of String) = New List(Of String)()

        Const NvDLISR_default As String = "C:/Program Files/NVIDIA Corporation/NVIDIA NvDLISR/nvdlisrwrapper.exe"

        Public Sub New(Optional files As IEnumerable(Of String) = Nothing)
            Program.message("NVIDIA Display Adapter is required ;)" & vbLf & vbLf & vbLf & "TL;DR" & vbLf & "Do not up-res images that will result in a resolution of over 9000x9000." & vbLf & vbLf & "When up-ressing an image that will result in a resolution of over 9000x9000, the image is likely to provide an error during processing due to memory limitations with NVIDIA's API.")

            DetectSystem()

            If files IsNot Nothing Then
                imagePaths.AddRange(files)
            End If
        End Sub

        Public Sub Add(files As IEnumerable(Of String))
            If files IsNot Nothing Then
                imagePaths.AddRange(files)
            End If
        End Sub

        ' Checks whether the system is able to use the app.
        Private Sub DetectSystem()
            Dim graphicsAdapter As GraphicsAdapter = WQL.DetectGraphicsAdapter()

            supportLevel = graphicsAdapter.SupportLevel

            ' Has graphics adapter but not app (probably driver update needed).
            If graphicsAdapter.SupportLevel <> SupportLevel.None AndAlso Not DetectApp() Then
                Program.message($"I was unable to locate '{NvDLISR_default}'. Without this app, I cannot do what I am supposed to do.

It is very likely that your Display Adapter Driver needs updating.")
                ' Has neither the graphics adapter nor the app (probably not NVIDIA).
            ElseIf graphicsAdapter.SupportLevel = SupportLevel.None AndAlso Not DetectApp() Then
                Program.message("An NVIDIA GeForce GTX or RTX display adapter is required and neither could not be found.")
                ' Has the app but not the graphics adapter (probably used to have NVIDIA but not anymore; could also be the app not recognising the graphics adapter properly).
            ElseIf graphicsAdapter.SupportLevel = SupportLevel.None AndAlso DetectApp() Then
                Program.message("An NVIDIA GeForce GTX or RTX display adapter is required and neither could be found." & vbLf & vbLf & "If you feel like this is a mistake, enable 'Force Mode'.")
            End If

            systemCheckComplete = True
        End Sub

        ' Checks if the app required for up-ressing is present.
        Private Function DetectApp() As Boolean
            If Not File.Exists(NvDLISR_default) Then Return False

            Return True
        End Function

        Public Sub StartImageProcessing()
            If Not systemCheckComplete Then
                Program.message("System check not complete. If this keeps occuring, please restart the app.")
                Return
            End If
            If imagePaths.Count < 1 Then
                Program.message("An image must be selected first.")
                Return
            End If
            If Not File.Exists(imagePaths(0)) Then
                Program.message("Could not access the selected image." & vbLf & vbLf & "Does the image exist?" & vbLf & "Am I being blocked by security?")
            End If

            Dim time = Date.Now.ToString("yyyy MM dd HH mm ss")
            Dim resolutionFactor As Integer = Math.Pow(2, CInt(ScaleSize.x16))
            Dim colourMode = "2"
            Dim limitSize = True

            ' Reset
            tasksCompleted = 0
            failedImages = 0

            Program.message($"Processing... ({tasksCompleted + 1}/{imagePaths.Count})")

            Dim progress = New Progress(Of Integer)(Sub(progressValue)
                                                        Debug.Write(progressValue)
                                                        Program.message($"Processing... ({Math.Min(Threading.Interlocked.Increment(progressValue), progressValue - 1)}/{imagePaths.Count})")
                                                    End Sub)

            ProcessImage(time, resolutionFactor, colourMode, limitSize, progress)

            ' If no images failed, show a success image.
            If failedImages <= 0 Then
                Program.message($"[Process complete] All images have enhanced!")
            Else
                Program.message($"[Process complete] {failedImages} images failed to enhance!")
            End If
        End Sub

        ' Processes the selected images into the NVIDIA Ansel AI Up-Res app in a multithreaded way.
        Private Sub ProcessImage(time As String, defaultResolutionFactor As Integer, colourMode As String, limitSize As Boolean, progress As IProgress(Of Integer))
            Dim tempDirectoryName = String.Empty

            ' If output folder mode is on, it will create a new folder and use it as its directory.
            If outputFolderMode Then
                tempDirectoryName = Path.Combine(Path.GetDirectoryName(imagePaths(0)), $"Enhanced Output {time}")
                Directory.CreateDirectory(tempDirectoryName)
            End If

            ' Divides the task into many parallel tasks (multithreading).
            ' The user selected thread count becomes the max possible threads the following tasks will use at once.
            Call Parallel.ForEach(imagePaths, New ParallelOptions() With {
        .MaxDegreeOfParallelism = threadCount
    }, Sub(sourceImagePath) processImage(sourceImagePath, defaultResolutionFactor, colourMode, limitSize, progress, tempDirectoryName))
        End Sub

        Private Sub processImage(sourceImagePath As String, defaultResolutionFactor As Integer, colourMode As String, limitSize As Boolean, progress As IProgress(Of Integer), tempDirectoryName As String)
            Dim withoutExtension = System.IO.Path.GetFileNameWithoutExtension(sourceImagePath)
            Dim directoryName = System.IO.Path.GetDirectoryName(sourceImagePath)
            Dim extension = System.IO.Path.GetExtension(sourceImagePath)
            Dim currentResolutionFactor = defaultResolutionFactor ' Copy of resolution (in case temp change to res is needed)
            Dim sourceImage = Image.FromFile(sourceImagePath)
            Dim isTransparentImage = HasTransparency(sourceImage)

            ' Checks if image height/width will overgrow (8000x8000) which is well.. BIG - modifies resolution factor
            ' with the ability to ignore it.
            If limitSize Then
                Dim largestUpscaledDimension = Math.Max(sourceImage.Width, sourceImage.Height) * defaultResolutionFactor

                ' Calculate new resolution factor to max out to be 8000px.
                If largestUpscaledDimension >= 8000 Then
                    currentResolutionFactor = CInt(Math.Floor(defaultResolutionFactor / (largestUpscaledDimension / 8000R)))

                    ' Floors the new resolution factor down to the closest power of 2.
                    currentResolutionFactor = CInt(Math.Pow(2, CInt(Math.Log(currentResolutionFactor, 2))))
                    Program.message(currentResolutionFactor.ToString())
                End If
            End If
            sourceImage.Dispose()

            ' Resolution factor is lower than what the upscaler than handle.
            If currentResolutionFactor < 2 Then
                failedImages += 1
                tasksCompleted += 1

                ' Updates the program's progress to the UI.
                If progress IsNot Nothing Then progress.Report(tasksCompleted)

                Return
            End If

            If Not outputFolderMode Then tempDirectoryName = directoryName

            ' Gives the correct colour mode labelling.
            Dim colourModeStr As String
            If Equals(colourMode, 1.ToString()) Then
                colourModeStr = "Greyscale"
            Else
                colourModeStr = "Colour"
            End If

            Dim startUpscale As Func(Of String, Boolean, String, String) = Function(args, isMove, customName)
                                                                               ' Starts the NVIDIA Ansel AI Up-Res app and insert the selected image into it.
                                                                               Using process As Process = New Process()
                                                                                   process.StartInfo.FileName = NvDLISR_default
                                                                                   process.StartInfo.Arguments = args
                                                                                   process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden
                                                                                   process.Start()
                                                                                   process.WaitForExit()
                                                                               End Using

                                                                               Try
                                                                                   ' Dynamiquel: "Not sure this is an ideal place for this but whatever." Credit: OlympicAngel.
                                                                                   Dim defaultPath = Path.Combine(directoryName, withoutExtension)
                                                                                   Dim newPath = $"{defaultPath}_{colourModeStr}_x{currentResolutionFactor * 2}{extension}"

                                                                                   If Not isMove Then
                                                                                       Dim customPath = $"{defaultPath}_{customName}"

                                                                                       If File.Exists(customPath) Then File.Delete(customPath)

                                                                                       File.Move(defaultPath, customPath)

                                                                                       Return customPath
                                                                                   End If

                                                                                   If File.Exists(newPath) Then File.Delete(newPath)

                                                                                   File.Move(defaultPath, newPath)

                                                                                   Return newPath
                                                                               Catch ex As Exception
                                                                                   failedImages += 1

                                                                                   Program.message($"This could either be due to NVIDIA's API memory limitations (try a lower resolution) or something to do with file write permissions.

{ex}")

                                                                                   Return ""
                                                                               End Try
                                                                           End Function

            ' Creates the command needed to put into the NVIDIA app. 'url 2/4 1/2'
            Dim command = $"{sourceImagePath} {currentResolutionFactor} {colourMode}"
            ' Preform the normal upscale as normal (if img has transparency dont yet move it to user's location)
            Dim upscaledImagePath = startUpscale(command, Not isTransparentImage, "normal")

            If isTransparentImage Then
                Dim transUpscaled As Bitmap = Nothing

                Try
                    transUpscaled = UpscaleWithAlpha(upscaledImagePath, sourceImagePath, currentResolutionFactor, startUpscale)
                    Dim newPath = Path.Combine(tempDirectoryName, $"{withoutExtension}_trans_{colourModeStr}_x{currentResolutionFactor * 2}{extension}")
                    transUpscaled.Save(newPath)
                Catch ex As Exception
                    failedImages += 1
                    Program.message($"An error has occured during the transparency merge stage.

{ex}")
                Finally
                    transUpscaled?.Dispose()
                End Try
            End If

            tasksCompleted += 1

            ' Updates the program's progress to the UI.
            If progress IsNot Nothing Then progress.Report(tasksCompleted)
        End Sub
    End Class
End Namespace
