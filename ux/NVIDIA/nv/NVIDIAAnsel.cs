using ImageUtils;
using System.Diagnostics;
using System.Drawing;
using Image = System.Drawing.Image;

namespace nv;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public class NVIDIAAnsel
{
    bool forceMode = false;
    SupportLevel supportLevel;
    bool systemCheckComplete = false;
    bool outputFolderMode = false;
    int threadCount = 1;
    int tasksCompleted = 0;
    int failedImages = 0;
    bool firstTime;
    readonly List<string> imagePaths = new List<string>();

    const string NvDLISR_default = "C:/Program Files/NVIDIA Corporation/NVIDIA NvDLISR/nvdlisrwrapper.exe";

    public NVIDIAAnsel(IEnumerable<string> files = null)
    {
        Program.message("NVIDIA Display Adapter is required ;)\n\n\nTL;DR\nDo not up-res images that will result in a resolution of over 9000x9000.\n\nWhen up-ressing an image that will result in a resolution of over 9000x9000, the image is likely to provide an error during processing due to memory limitations with NVIDIA's API.");

        DetectSystem();

        if (files != null)
        {
            this.imagePaths.AddRange(files);
        }
    }

    public void Add(IEnumerable<string> files)
    {
        if (files != null)
        {
            imagePaths.AddRange(files);
        }
    }

    // Checks whether the system is able to use the app.
    void DetectSystem()
    {
        GraphicsAdapter graphicsAdapter = WQL.DetectGraphicsAdapter();

        supportLevel = graphicsAdapter.SupportLevel;

        // Has graphics adapter but not app (probably driver update needed).
        if (graphicsAdapter.SupportLevel != SupportLevel.None && !DetectApp())
            Program.message($"I was unable to locate '{NvDLISR_default}'. Without this app, I cannot do what I am supposed to do.\n\nIt is very likely that your Display Adapter Driver needs updating.");
        // Has neither the graphics adapter nor the app (probably not NVIDIA).
        else if (graphicsAdapter.SupportLevel == SupportLevel.None && !DetectApp())
            Program.message("An NVIDIA GeForce GTX or RTX display adapter is required and neither could not be found.");
        // Has the app but not the graphics adapter (probably used to have NVIDIA but not anymore; could also be the app not recognising the graphics adapter properly).
        else if (graphicsAdapter.SupportLevel == SupportLevel.None && DetectApp())
            Program.message("An NVIDIA GeForce GTX or RTX display adapter is required and neither could be found.\n\nIf you feel like this is a mistake, enable 'Force Mode'.");

        systemCheckComplete = true;
    }

    // Checks if the app required for up-ressing is present.
    bool DetectApp()
    {
        if (!File.Exists(NvDLISR_default))
            return false;

        return true;
    }

    public void StartImageProcessing()
    {
        if (!systemCheckComplete)
        {
            Program.message("System check not complete. If this keeps occuring, please restart the app.");
            return;
        }
        if (imagePaths.Count < 1)
        {
            Program.message("An image must be selected first.");
            return;
        }
        if (!File.Exists(imagePaths[0]))
        {
            Program.message("Could not access the selected image.\n\nDoes the image exist?\nAm I being blocked by security?");
        }

        string time = DateTime.Now.ToString("yyyy MM dd HH mm ss");
        int resolutionFactor = (int)Math.Pow(2, (int)ScaleSize.x16);
        string colourMode = "2";
        bool limitSize = true;

        // Reset
        tasksCompleted = 0;
        failedImages = 0;

        Program.message($"Processing... ({tasksCompleted + 1}/{imagePaths.Count})");

        var progress = new Progress<int>(progressValue =>
        {
            Debug.Write(progressValue);
            Program.message($"Processing... ({progressValue++}/{imagePaths.Count})");
        });

        ProcessImage(time, resolutionFactor, colourMode, limitSize, progress);

        // If no images failed, show a success image.
        if (failedImages <= 0)
            Program.message($"[Process complete] All images have enhanced!");
        else
            Program.message($"[Process complete] {failedImages} images failed to enhance!");
    }

    // Processes the selected images into the NVIDIA Ansel AI Up-Res app in a multithreaded way.
    void ProcessImage(string time, int defaultResolutionFactor, string colourMode, bool limitSize, IProgress<int> progress)
    {
        string tempDirectoryName = string.Empty;

        // If output folder mode is on, it will create a new folder and use it as its directory.
        if (outputFolderMode)
        {
            tempDirectoryName = Path.Combine(Path.GetDirectoryName(imagePaths[0]), $"Enhanced Output {time}");
            Directory.CreateDirectory(tempDirectoryName);
        }

        // Divides the task into many parallel tasks (multithreading).
        Parallel.ForEach(imagePaths, new ParallelOptions()
        {
            // The user selected thread count becomes the max possible threads the following tasks will use at once.
            MaxDegreeOfParallelism = threadCount
        }, sourceImagePath => processImage(sourceImagePath, defaultResolutionFactor, colourMode, limitSize, progress, tempDirectoryName));
    }

    private void processImage(string sourceImagePath, int defaultResolutionFactor, string colourMode, bool limitSize, IProgress<int> progress, string tempDirectoryName)
    {
        string withoutExtension = Path.GetFileNameWithoutExtension(sourceImagePath);
        string directoryName = Path.GetDirectoryName(sourceImagePath);
        string extension = Path.GetExtension(sourceImagePath);
        int currentResolutionFactor = defaultResolutionFactor; // Copy of resolution (in case temp change to res is needed)
        Image sourceImage = Image.FromFile(sourceImagePath);
        bool isTransparentImage = TransparencySupport.HasTransparency(sourceImage);

        // Checks if image height/width will overgrow (8000x8000) which is well.. BIG - modifies resolution factor
        // with the ability to ignore it.
        if (limitSize)
        {
            int largestUpscaledDimension = Math.Max(sourceImage.Width, sourceImage.Height) * defaultResolutionFactor;

            // Calculate new resolution factor to max out to be 8000px.
            if (largestUpscaledDimension >= 8000)
            {
                currentResolutionFactor = (int)Math.Floor(defaultResolutionFactor / (largestUpscaledDimension / 8000d));

                // Floors the new resolution factor down to the closest power of 2.
                currentResolutionFactor = (int)Math.Pow(2, (int)Math.Log(currentResolutionFactor, 2));
                Program.message(currentResolutionFactor.ToString());
            }
        }
        sourceImage.Dispose();

        // Resolution factor is lower than what the upscaler than handle.
        if (currentResolutionFactor < 2)
        {
            failedImages++;
            tasksCompleted++;

            // Updates the program's progress to the UI.
            if (progress != null)
                progress.Report(tasksCompleted);

            return;
        }

        if (!outputFolderMode)
            tempDirectoryName = directoryName;

        // Gives the correct colour mode labelling.
        string colourModeStr;
        if (colourMode == 1.ToString())
            colourModeStr = "Greyscale";
        else
            colourModeStr = "Colour";

        Func<string, bool, string, string> startUpscale = delegate (string args, bool isMove, string customName)
        {
            // Starts the NVIDIA Ansel AI Up-Res app and insert the selected image into it.
            using (Process process = new Process())
            {
                process.StartInfo.FileName = NvDLISR_default;
                process.StartInfo.Arguments = args;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.Start();
                process.WaitForExit();
            }

            try
            {
                // Dynamiquel: "Not sure this is an ideal place for this but whatever." Credit: OlympicAngel.
                string defaultPath = Path.Combine(directoryName, withoutExtension);
                string newPath = $"{defaultPath}_{colourModeStr}_x{currentResolutionFactor * 2}{extension}";

                if (!isMove)
                {
                    string customPath = $"{defaultPath}_{customName}";

                    if (File.Exists(customPath))
                        File.Delete(customPath);

                    File.Move(defaultPath, customPath);

                    return customPath;
                }

                if (File.Exists(newPath))
                    File.Delete(newPath);

                File.Move(defaultPath, newPath);

                return newPath;
            }
            catch (Exception ex)
            {
                failedImages++;

                Program.message($"This could either be due to NVIDIA's API memory limitations (try a lower resolution) or something to do with file write permissions.\n\n{ex}");

                return "";
            }
        };

        // Creates the command needed to put into the NVIDIA app. 'url 2/4 1/2'
        string command = $"{sourceImagePath} {currentResolutionFactor} {colourMode}";
        // Preform the normal upscale as normal (if img has transparency dont yet move it to user's location)
        string upscaledImagePath = startUpscale(command, !isTransparentImage, "normal");

        if (isTransparentImage)
        {
            Bitmap transUpscaled = null;

            try
            {
                transUpscaled = TransparencySupport.UpscaleWithAlpha(upscaledImagePath, sourceImagePath, currentResolutionFactor, startUpscale);
                string newPath = Path.Combine(tempDirectoryName, $"{withoutExtension}_trans_{colourModeStr}_x{currentResolutionFactor * 2}{extension}");
                transUpscaled.Save(newPath);
            }
            catch (Exception ex)
            {
                failedImages++;
                Program.message($"An error has occured during the transparency merge stage.\n\n{ex}");
            }
            finally
            {
                transUpscaled?.Dispose();
            }
        }

        tasksCompleted++;

        // Updates the program's progress to the UI.
        if (progress != null)
            progress.Report(tasksCompleted);
    }
}

