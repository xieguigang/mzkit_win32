using ImageUtils;
using System.Diagnostics;
using System.Drawing;
using System.Management;
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

    public NVIDIAAnsel()
    {
        Program.message("NVIDIA Display Adapter is required ;)\n\n\nTL;DR\nDo not up-res images that will result in a resolution of over 9000x9000.\n\nWhen up-ressing an image that will result in a resolution of over 9000x9000, the image is likely to provide an error during processing due to memory limitations with NVIDIA's API.");
        DetectSystem();
    }

    // Checks whether the system is able to use the app.
    void DetectSystem()
    {
        GraphicsAdapter graphicsAdapter = DetectGraphicsAdapter();

        supportLevel = graphicsAdapter.SupportLevel;



        // Has graphics adapter but not app (probably driver update needed).
        if (graphicsAdapter.SupportLevel != SupportLevel.None && !DetectApp())
            Program.message("I was unable to locate 'C:/Program Files/NVIDIA Corporation/NVIDIA NvDLISR/nvdlisrwrapper.exe'. Without this app, I cannot do what I am supposed to do.\n\nIt is very likely that your Display Adapter Driver needs updating.");
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
        if (!File.Exists("C:/Program Files/NVIDIA Corporation/NVIDIA NvDLISR/nvdlisrwrapper.exe"))
            return false;

        return true;
    }

    // Returns the most suitable graphics adapter in the system whiles also displaying to the user what graphics adapter they have.
    GraphicsAdapter DetectGraphicsAdapter()
    {
        GraphicsAdapter graphicsAdapter = GetGraphicsAdapter();

        string graphicsAdapterName = graphicsAdapter.Name;
        Program.message(graphicsAdapterName);

        return graphicsAdapter;
    }

    // Returns the most suitable graphics adapter present in the system.
    GraphicsAdapter GetGraphicsAdapter()
    {
        // Gets every graphics adapter in use by the system.
        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");

        List<GraphicsAdapter> graphicsAdapters = new List<GraphicsAdapter>();

        // Saves the name of every found graphics card in a list.
        foreach (ManagementObject mo in searcher.Get())
            foreach (PropertyData property in mo.Properties)
                if (property.Name == "Description")
                    graphicsAdapters.Add(new GraphicsAdapter(property.Value.ToString()));

        // If no graphics adapters could be found...
        if (graphicsAdapters.Count < 1)
        {
            Program.message("For some unknown reason, your display adapter could not be found.\n\nPerhaps you have some security program blocking me access?\n\nIf you feel like this is a mistake, enable 'Force Mode'.");
            return new GraphicsAdapter("N/A");
        }

        // For every graphics adapter, find its support level by looking up its name.
        foreach (var graphicsAdapter in graphicsAdapters)
        {

            if (FullySupportedGraphicsAdapters().Any(s => graphicsAdapter.Name.ToLower().Contains(s)))
                graphicsAdapter.SupportLevel = SupportLevel.Full;
            else if (PartiallySupportedGraphicsAdapters().Any(s => graphicsAdapter.Name.ToLower().Contains(s)))
                graphicsAdapter.SupportLevel = SupportLevel.Partial;
            else
                graphicsAdapter.SupportLevel = SupportLevel.None;
        }

        // Orders the graphics adapters by support level (first graphics adapter should be most suitable).
        graphicsAdapters.OrderBy(o => o.SupportLevel).Reverse().ToList();

        // Returns the most suitable graphics adapter.
        return graphicsAdapters[0];
    }

    List<string> FullySupportedGraphicsAdapters()
    {
        List<string> str = new List<string>();
        str.Add("rtx");

        return str;
    }

    List<string> PartiallySupportedGraphicsAdapters()
    {
        List<string> str = new List<string>();
        str.Add("1030");
        str.Add("1040");
        str.Add("1050");
        str.Add("1060");
        str.Add("1070");
        str.Add("1080");
        str.Add("titan");
        str.Add("1640");
        str.Add("1650");
        str.Add("1660");
        str.Add("1670");
        str.Add("1680");

        return str;
    }

    public async void StartImageProcessing()
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
        int resolutionFactor = (int)Math.Pow(2, (int)ScaleSize.x4);
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

        await Task.Run(() => ProcessImage(time, resolutionFactor, colourMode, limitSize, progress));

        // If no images failed, show a success image.
        if (failedImages <= 0)
            Program.message($"[Process complete] All images have enhanced!");
        else
            Program.message($"[Process complete] {failedImages} images failed to enhance!");
    }

    // Processes the selected images into the NVIDIA Ansel AI Up-Res app in a multithreaded way.
    async Task ProcessImage(string time, int defaultResolutionFactor, string colourMode, bool limitSize, IProgress<int> progress)
    {
        string tempDirectoryName = string.Empty;

        // If output folder mode is on, it will create a new folder and use it as its directory.
        if (outputFolderMode)
        {
            tempDirectoryName = Path.Combine(Path.GetDirectoryName(imagePaths[0]), $"Enhanced Output {time}");
            Directory.CreateDirectory(tempDirectoryName);
        }

        await Task.Run(() =>
        {
            // Divides the task into many parallel tasks (multithreading).
            Parallel.ForEach(imagePaths, new ParallelOptions()
            {
                // The user selected thread count becomes the max possible threads the following tasks will use at once.
                MaxDegreeOfParallelism = threadCount
            }, sourceImagePath => processImage(sourceImagePath, defaultResolutionFactor, colourMode, limitSize, progress, tempDirectoryName));
        });
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
                process.StartInfo.FileName = "C:/Program Files/NVIDIA Corporation/NVIDIA NvDLISR/nvdlisrwrapper.exe";
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

