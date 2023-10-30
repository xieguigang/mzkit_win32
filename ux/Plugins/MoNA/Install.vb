Imports System.IO.Compression
Imports Mzkit_win32.BasicMDIForm

Public Class Install

    Public file_url As String
    Public tmp_zip As String
    Public msp_file As String

    Public Function InstallMoNA(a As ITaskProgress) As Boolean
        Call a.SetProgressMode()

        If Not file_url.DownloadFile(save:=tmp_zip) Then
            Call Workbench.Warning("Download MoNA database file error!")
            Return False
        End If

        Call Workbench.LogText("Download database file success!")
        Call Workbench.LogText($"database_file_size: {StringFormats.Lanudry(CDbl(tmp_zip.FileLength))}")

        Call a.SetInfo("Extract the zip archive file...")
        Call msp_file.ParentPath.MakeDir

        Using zip As New ZipArchive(tmp_zip.OpenReadonly, ZipArchiveMode.Read)
            zip.Entries.Item(0).ExtractToFile(msp_file, True)
        End Using

        Call a.SetInfo("Install database to local appdata filesystem...")

        Return InstallLocal(msp_file)
    End Function

    Private Function InstallLocal(msp_file As String) As Boolean

    End Function
End Class
