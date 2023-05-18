Imports System.ComponentModel
Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.HeatMap.hqx
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.My.FrameworkInternal
Imports Parallel

Public Module Program

    Public Function Main() As Integer
        Return GetType(Program).RunCLI(App.CommandLine)
    End Function

    <ExportAPI("/start")>
    <Description("Start the data visualization rendering background service for the mass spectral data rendering.")>
    <Usage("/start --port <tcp_port> --master <mzkit_win32 PID>")>
    Public Function StartService(args As CommandLine) As Integer
        Dim port As Integer = args <= "--port"
        Dim master As Integer = args <= "--master"
        Dim localhost As New Service(port)

        If master > 0 Then
            Call BackgroundTaskUtils.BindToMaster(parentId:=master, kill:=localhost)
        End If

        Return localhost.Run
    End Function

    <ExportAPI("/ST-imaging")>
    <Usage("/ST-imaging --raw <stimaging.mzPack> --targets <names.txt> [--output <outputdir>]")>
    Public Function RenderSTImagingTargets(args As CommandLine) As Integer
        Dim raw As String = args("--raw")
        Dim targets As String() = args("--targets").ReadAllLines
        Dim output As String = args("--output") Or $"{raw.ParentPath}/{raw.BaseName}_STImaging/"

        Call FrameworkInternal.ConfigMemory(MemoryLoads.Max)

        Using file As Stream = raw.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Dim load As mzPack = mzPack.ReadAll(file, ignoreThumbnail:=True, skipMsn:=True)
            Dim maps As New Dictionary(Of String, Double)
            Dim MSI As New Drawer(load)
            Dim params As New Task.MsImageProperty With {
                .background = Color.Black,
                .colors = ScalerPalette.turbo,
                .enableFilter = True,
                .Hqx = HqxScales.Hqx_4x,
                .knn = 3,
                .knn_qcut = 0.65,
                .mapLevels = 250,
                .showPhysicalRuler = False,
                .showTotalIonOverlap = False,
                .TrIQ = 0.9
            }
            Dim metadata As Metadata = load.GetMSIMetadata
            Dim canvasSize As New Size(metadata.scan_x * 15, metadata.scan_y * 15)

            For Each layer In load.Annotations
                maps(layer.Value) = Val(layer.Key)
            Next

            For Each id As String In targets
                Dim mz As Double = maps.TryGetValue(id, [default]:=-1)

                If mz <= 0 Then
                    Continue For
                Else
                    Dim pixels = MSI.LoadPixels({mz}, Tolerance.DeltaMass(0.3)).ToArray
                    Dim blender As New SingleIonMSIBlender(pixels, Nothing, params)
                    Dim image As Image = blender.Rendering(New Task.PlotProperty, canvasSize)

                    Call VBDebugger.EchoLine(id)
                    Call image.SaveAs($"{output}/{id}.png")
                End If
            Next

            Return 0
        End Using
    End Function
End Module
