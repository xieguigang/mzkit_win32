
using Microsoft.VisualBasic;
using nv;

internal class Program
{
    static void Main(string[] args)
    {
        NVIDIAAnsel ansel = new NVIDIAAnsel();

        ansel.Add(App.CommandLine.Tokens);
        ansel.StartImageProcessing();
    }

    public static void message(string msg)
    {
        Console.WriteLine(msg);
    }
}