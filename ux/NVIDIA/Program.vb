
Imports Microsoft.VisualBasic
Imports nv

Friend Class Program
    Private Shared Sub Main(args As String())
        Dim ansel As NVIDIAAnsel = New NVIDIAAnsel()

        ansel.Add(App.CommandLine.Tokens)
        ansel.StartImageProcessing()
    End Sub

    Public Shared Sub message(msg As String)
        Console.WriteLine(msg)
    End Sub
End Class
