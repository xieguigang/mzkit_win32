#Region "Microsoft.VisualBasic::498c00bc19998fc198aeb4f028d1e973, mzkit\services\BlenderHost\Program.vb"

    ' Author:
    ' 
    '       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
    ' 
    ' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
    ' 
    ' 
    ' MIT License
    ' 
    ' 
    ' Permission is hereby granted, free of charge, to any person obtaining a copy
    ' of this software and associated documentation files (the "Software"), to deal
    ' in the Software without restriction, including without limitation the rights
    ' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    ' copies of the Software, and to permit persons to whom the Software is
    ' furnished to do so, subject to the following conditions:
    ' 
    ' The above copyright notice and this permission notice shall be included in all
    ' copies or substantial portions of the Software.
    ' 
    ' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    ' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    ' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    ' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    ' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    ' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    ' SOFTWARE.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 109
    '    Code Lines: 91 (83.49%)
    ' Comment Lines: 1 (0.92%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 17 (15.60%)
    '     File Size: 4.58 KB


    ' Module Program
    ' 
    '     Function: Main, RenderSTImagingTargets, StartService
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports System.Drawing
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Darwinism.HPC.Parallel
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap.hqx
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.My.FrameworkInternal
Imports mzblender
Imports Image = System.Drawing.Image

Public Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/start")>
    <Description("Start the data visualization rendering background service for the mass spectral data rendering.")>
    <Usage("/start --port <tcp_port> --master <mzkit_win32 PID> [--debug]")>
    Public Function StartService(args As CommandLine) As Integer
        Dim port As Integer = args <= "--port"
        Dim master As Integer = Val(CStr(args <= "--master"))
        Dim is_debug As Boolean = args("--debug")
        Dim localhost As New Service(port, masterChannel:=If(is_debug, "debug-blender", master))

        If (Not is_debug) AndAlso master > 0 Then
            Call BackgroundTaskUtils.BindToMaster(parentId:=master, kill:=localhost)
        End If

        Return localhost.Run
    End Function

    <ExportAPI("/ST-imaging")>
    <Usage("/ST-imaging --raw <stimaging.mzPack> [--keeps-background --targets <names.txt> --scale <default=30> --output <outputdir>]")>
    Public Function RenderSTImagingTargets(args As CommandLine) As Integer
        Dim raw As String = args("--raw")
        Dim targets As String() = args("--targets").ReadAllLines
        Dim output As String = args("--output") Or $"{raw.ParentPath}/{raw.BaseName}/"
        Dim scale As Double = args("--scale") Or 15.0
        Dim keepsBackground As Boolean = args("--keeps-background")

        Call FrameworkInternal.ConfigMemory(MemoryLoads.Max)

        Using file As Stream = raw.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim load As mzPack = mzPack.ReadAll(file, ignoreThumbnail:=True, skipMsn:=True)
            Dim maps As New Dictionary(Of String, Double)

            If Not keepsBackground Then
                ' filter out the background spots
                load.MS = load.MS _
                    .Where(Function(s) s.meta(mzStreamWriter.SampleMetaName) = "sample") _
                    .ToArray
            End If

            Dim MSI As New Drawer(load)
            Dim metadata As Metadata = load.GetMSIMetadata
            Dim params As New Task.MsImageProperty(metadata.scan_x, metadata.scan_y) With {
                .background = Color.Black,
                .colors = ScalerPalette.turbo,
                .enableFilter = True,
                .Hqx = HqxScales.Hqx_4x,
                .knn = 3,
                .knn_qcut = 0.65,
                .mapLevels = 250,
                .showPhysicalRuler = False,
                .showTotalIonOverlap = False,
                .TrIQ = 1
            }
            Dim canvasSize As New Size(metadata.scan_x * scale, metadata.scan_y * scale)

            For Each layer In load.Annotations
                maps(layer.Value) = Val(layer.Key)
            Next

            If targets.IsNullOrEmpty Then
                targets = maps.Keys.ToArray
            End If

            For Each id As String In targets
                Dim mz As Double = maps.TryGetValue(id, [default]:=-1)

                If mz <= 0 Then
                    Call $"Missing '{id}'!".Warning
                    Continue For
                Else
                    Try
                        Dim pixels = MSI.LoadPixels({mz}, Tolerance.DeltaMass(0.3)).ToArray
                        Dim blender As New SingleIonMSIBlender(pixels, filters:=Nothing, params)
                        Dim image As Image = blender.Rendering(New Task.PlotProperty, canvasSize)

                        Call VBDebugger.EchoLine(id)
                        Call image.SaveAs($"{output}/{id}.png")
                    Catch ex As Exception

                    End Try
                End If
            Next

            Return 0
        End Using
    End Function
End Module
