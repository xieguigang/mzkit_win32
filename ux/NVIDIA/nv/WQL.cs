using System.Management;

namespace nv;

class WQL
{

    /// <summary>
    /// Returns the most suitable graphics adapter in the system whiles also displaying to the user what graphics adapter they have.
    /// </summary>
    /// <returns></returns>
    public static GraphicsAdapter DetectGraphicsAdapter()
    {
        GraphicsAdapter graphicsAdapter = GetGraphicsAdapter();

        string graphicsAdapterName = graphicsAdapter.Name;
        Program.message(graphicsAdapterName);

        return graphicsAdapter;
    }

    /// <summary>
    /// Returns the most suitable graphics adapter present in the system.
    /// </summary>
    /// <returns></returns>
    public static GraphicsAdapter GetGraphicsAdapter()
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

    static IEnumerable<string> FullySupportedGraphicsAdapters()
    {
        yield return "rtx";
    }

    static IEnumerable<string> PartiallySupportedGraphicsAdapters()
    {
        yield return "1030";
        yield return "1040";
        yield return "1050";
        yield return "1060";
        yield return "1070";
        yield return "1080";
        yield return "titan";
        yield return "1640";
        yield return "1650";
        yield return "1660";
        yield return "1670";
        yield return "1680";
    }
}