﻿Imports System.IO
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ThermoRawFileReader
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.Imaging
Imports Task

Module ConvertToMzPack

    Public Sub ConvertCDF(raw As String,
                          cacheFile As String,
                          saveVer As Integer,
                          mute As Boolean,
                          skipThumbnail As Boolean,
                          Optional sleepTime As Double = 1500)

        Dim mzpack As mzPack
        Dim println As Action(Of String) = Nothing

        If Not mute Then
            println = AddressOf RunSlavePipeline.SendMessage
        End If

        Using gcms As New netCDFReader(raw)
            mzpack = GCMSConvertor.ConvertGCMS(gcms, println)
            mzpack.source = raw.FileName
        End Using

        Using file As Stream = cacheFile.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            If Not println Is Nothing Then
                Call println("Write mzPack cache data...")
            End If

            Call mzpack.Write(file, version:=saveVer)
        End Using

        Call Thread.Sleep(sleepTime)

        If Not println Is Nothing Then
            Call println("Job Done!")
        End If
    End Sub

    Public Function LoadMzPackAuto(raw As String, skipThumbnail As Boolean, println As Action(Of String)) As mzPack
        Dim mzpack As mzPack

        If raw.ExtensionSuffix("raw") Then
            Using msraw As New MSFileReader(raw)
                mzpack = msraw.LoadFromXRaw(println)
            End Using
        ElseIf raw.ExtensionSuffix("msp") Then
            mzpack = Converter.LoadMsp(raw)
        ElseIf raw.ExtensionSuffix("mgf") Then
            mzpack = Converter.LoadMgf(raw)
        ElseIf raw.ExtensionSuffix("mzpack") Then
            mzpack = mzPack.Read(raw)
        Else
            mzpack = Converter.LoadRawFileAuto(raw, "ppm:20", , println)
        End If

        If (Not mzpack.MS.IsNullOrEmpty) AndAlso (Not skipThumbnail) Then
            If Not println Is Nothing Then
                println("Create snapshot...")
            End If

            mzpack.Thumbnail = mzpack.DrawScatter
        End If

        Return mzpack
    End Function

    ''' <summary>
    ''' convert any kind of raw data file as mzPack
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="cacheFile"></param>
    Public Sub CreateMzpack(raw As String,
                            cacheFile As String,
                            saveVer As Integer,
                            mute As Boolean,
                            skipThumbnail As Boolean,
                            Optional sleepTime As Double = 1500)

        Dim mzpack As mzPack
        Dim println As Action(Of String) = Nothing

        If Not mute Then
            println = AddressOf RunSlavePipeline.SendMessage
        End If

        mzpack = LoadMzPackAuto(raw, skipThumbnail, println)

        Using file As Stream = cacheFile.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
            If Not println Is Nothing Then
                Call println("Write mzPack cache data...")
            End If

            Call mzpack.Write(file, version:=saveVer)
        End Using

        Call Thread.Sleep(sleepTime)

        If Not println Is Nothing Then
            Call println("Job Done!")
        End If
    End Sub
End Module
