Imports System.IO
Imports System.IO.Compression
Imports BioDeep
Imports BioNovoGene.BioDeep.Chemistry
Imports Microsoft.VisualBasic.Language
Imports Mzkit_win32.BasicMDIForm

Public Class Install

    Public file_url As String
    Public tmp_zip As String
    Public msp_file As String
    Public targetLib As Export

    Public Function InstallMoNA(a As ITaskProgress) As Boolean
        Call a.SetProgressMode()

        If Not file_url.DownloadFile(save:=tmp_zip) Then
            Call Workbench.Warning("Download MoNA database file error!")
            Return False
        End If

        Call Workbench.LogText("Download database file success!")
        Call Workbench.LogText($"database_file_size: {StringFormats.Lanudry(CDbl(tmp_zip.FileLength))}")

        Return InstallZip(a)
    End Function

    Public Function InstallZip(a As ITaskProgress) As Boolean
        Call a.SetInfo("Extract the zip archive file...")
        Call msp_file.ParentPath.MakeDir

        Try
            Using zip As New ZipArchive(tmp_zip.OpenReadonly, ZipArchiveMode.Read)
                zip.Entries.Item(0).ExtractToFile(msp_file, True)
            End Using
        Catch ex As Exception
            Return False
        End Try

        Call a.SetInfo("Install database to local appdata filesystem...")

        Return InstallLocal(msp_file, a)
    End Function

    Private Function InstallLocal(msp_file As String, a As ITaskProgress) As Boolean
        Dim path As String = SpectrumLibraryModule.LibraryFile($"{targetLib.label.NormalizePathString(alphabetOnly:=False)}.lcms-pack")
        Dim libdb As New RQLib(path.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=False))
        Dim msp As IEnumerable(Of SpectraSection) = MspReader.ParseFile(msp_file)
        Dim d As Integer = 17
        Dim i As i32 = 0

        For Each ion As SpectraSection In msp
            Dim anno = ion.GetMetabolite

            Call libdb.AddAnnotation(anno)
            Call libdb.AddSpectrum(ion.GetSpectrumPeaks, key:=anno.name)

            Call System.Windows.Forms.Application.DoEvents()

            If ++i Mod d = 0 Then
                Call a.SetInfo("Installing... " & ion.ToString)
            End If
        Next

        Call libdb.Dispose()

        Return True
    End Function
End Class
