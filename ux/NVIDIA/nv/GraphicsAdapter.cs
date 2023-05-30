class GraphicsAdapter
{
    public string Name { get; set; }
    public string DriverVersion { get; set; }
    public SupportLevel SupportLevel { get; set; }

    public GraphicsAdapter(string name, string ver = "n/a")
    {
        Name = name;
        DriverVersion = ver;
    }
}

enum SupportLevel
{
    None,
    Partial,
    Full
}