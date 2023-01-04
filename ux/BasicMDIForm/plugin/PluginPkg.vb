Imports System.IO
Imports System.IO.Compression

Public Class PluginPkg

    Public Property guid As String
    Public Property Name As String
    Public Property Link As String
    Public Property Description As String
    Public Property assets As String()

    Public Function BuildPackage(pkg As ZipArchive, rootDir As String) As Boolean
        Dim assets As New List(Of String)

        For Each path As String In assets
            Dim relative As String = PathExtensions.RelativePath(rootDir, path)

            Call assets.Add(relative)

            Using file As Stream = pkg.CreateEntry($"/bin/{relative}").Open
                Call New MemoryStream(path.ReadBinary).CopyTo(file)
                Call file.Flush()
            End Using
        Next

        Me.assets = assets.ToArray

        Dim metadata As String = Me.GetXml

        Using buf As Stream = pkg.CreateEntry("/index.xml").Open
            Using text As New StreamWriter(buf)
                Call text.WriteLine(metadata)
            End Using
        End Using

        Return True
    End Function

    Public Shared Function FromAppReleaseDirectory(dir As String) As PluginPkg

    End Function

End Class
