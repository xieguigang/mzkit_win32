Imports System.IO
Imports OpenSlideCs

Module Program

    Dim openSlide = New OpenSlide()

    Private Sub GetJpg(ByVal level As Integer, ByVal row As Integer, ByVal col As Integer, ByVal filename As String, ByVal outputname As String)
        Dim buffer As ArraySegment(Of Byte) = Nothing

        Using stream = openSlide.GetJpg(filename, $"_files/{level}/{row}_{col}.jpeg")
            If stream.TryGetBuffer(buffer) Then
                Console.WriteLine($"{outputname}.jpeg successfully created!")
                Call File.WriteAllBytes($"{outputname}.jpeg", buffer.ToArray())
            Else
                Console.WriteLine($"{outputname}.jpeg failed...")
            End If
        End Using
    End Sub

    Private Sub GetDZI(ByVal filename As String, ByVal outputname As String)
        Dim width, height As Long
        Dim buffer As ArraySegment(Of Byte) = Nothing

        Using stream = openSlide.GetDZI(filename, width, height)
            If stream.TryGetBuffer(buffer) Then
                Call Console.WriteLine($"{outputname}.dzi successfully created!")
                Call File.WriteAllBytes($"{outputname}.dzi", buffer.ToArray())
            Else
                Call Console.WriteLine($"{outputname}.dzi failed...")
            End If
        End Using

        Dim levels = openSlide.Dimensions(filename)
        Dim maxDimensions = levels(levels.Length - 1)

        For level As Integer = 0 To levels.Length - 1
            Dim levelInfo = levels(level)

            Call Directory.CreateDirectory($"{filename}_files/{level}")

            For row As Integer = 0 To levelInfo.Width - 1
                For col As Integer = 0 To levelInfo.Height - 1
                    Call GetJpg(level, row, col, filename, $"{filename}_files/{level}/{row}_{col}")
                Next
            Next
        Next
    End Sub

    Public Sub Main()
        Dim filename = "CMU-1-Small-Region"
        Dim levels = openSlide.GetMPP($"{filename}.svs")

        ' Loads DZI file
        GetDZI($"{filename}.svs", filename)
        Console.WriteLine("Done")
    End Sub
End Module
