Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioDeep
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.IndexedCache
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep.MSEngine.Mummichog
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.My.FrameworkInternal
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports STImaging
Imports stdNum = System.Math

<Package("BackgroundTask")>
Module BackgroundTask

    Sub New()
        FrameworkInternal.ConfigMemory(MemoryLoads.Heavy)
    End Sub

    <ExportAPI("ST_spaceranger.mzpack")>
    Public Function convertMzPack(spots As SpaceSpot(), matrix As Matrix) As mzPack
        Return spots.ST_spacerangerToMzPack(matrix)
    End Function

    Public Function cfmidPredict() As Object
        Dim cfmid As New CFM_ID.Prediction("")

        Throw New NotImplementedException
    End Function

    <ExportAPI("read.tissue_regions")>
    Public Function readTissues(file As String) As TissueRegion()
        Using buffer As Stream = file.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            Return buffer.ReadTissueMorphology
        End Using
    End Function

    <ExportAPI("phenograph")>
    Public Function RunFeatureDetections(mzpackRaw As String, topN As Integer, dims As Integer, mzdiff As String) As NetworkGraph
        Dim mzpack As mzPack = mzPack.Read(mzpackRaw, ignoreThumbnail:=True)
        Dim mzErr As Tolerance = Tolerance.ParseScript(mzdiff)
        Dim intocutoff As New RelativeIntensityCutoff(0.01)
        Dim pixelsData = mzpack.MS _
            .AsParallel _
            .Select(Function(m)
                        Dim msArray = m.GetMs _
                            .ToArray _
                            .Centroid(mzErr, intocutoff) _
                            .OrderByDescending(Function(d) d.intensity) _
                            .Take(topN) _
                            .ToArray

                        Return (m.GetMSIPixel, msArray)
                    End Function) _
            .ToArray
        Dim allMz = pixelsData _
            .Select(Function(d) d.msArray) _
            .IteratesALL _
            .ToArray _
            .Centroid(mzErr, intocutoff) _
            .OrderByDescending(Function(d) d.intensity) _
            .Take(dims) _
            .Select(Function(d) d.mz) _
            .ToArray
        Dim mzData As DataSet() = pixelsData _
            .AsParallel _
            .Select(Function(d)
                        Dim vec As New Dictionary(Of String, Double)

                        For Each mz As Double In allMz
                            vec(mz.ToString("F4")) = d.msArray _
                                .Where(Function(di) mzErr(di.mz, mz)) _
                                .Select(Function(di)
                                            Return di.intensity
                                        End Function) _
                                .Sum
                        Next

                        Return New DataSet With {
                            .ID = $"{d.GetMSIPixel.X},{d.GetMSIPixel.Y}",
                            .Properties = vec
                        }
                    End Function) _
            .ToArray

        Throw New NotImplementedException
        ' Return mzData.CreatePhenoGraph(k:=120)
    End Function

    <Extension>
    Public Function alignMz(data As ms2(), mz As Double, tolerance As Tolerance) As Double
        Return data _
            .Where(Function(i) tolerance(mz, i.mz)) _
            .OrderByDescending(Function(a) a.intensity) _
            .Select(Function(a) a.intensity) _
            .Average
    End Function

    <ExportAPI("biodeep.session")>
    Public Sub SetBioDeepSession(ssid As String)
        SingletonHolder(Of BioDeepSession).Instance.ssid = ssid
    End Sub

    <ExportAPI("Mummichog")>
    Public Sub Mummichog(raw As String, outputdir As String)
        Dim mzpack As mzPack

        Using file As Stream = raw.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            mzpack = mzPack.ReadAll(file)
        End Using

        Dim println As Action(Of String) = AddressOf RunSlavePipeline.SendMessage
        Dim keggCompounds = KEGGRepo.RequestKEGGcompounds(println)
        Dim range As MzCalculator() = mzpack.getIonRange _
            .Select(Function(adducts) Parser.ParseMzCalculator(adducts)) _
            .ToArray
        Dim pool As IMzQuery = KEGGHandler.CreateIndex(keggCompounds, range, PPMmethod.PPM(20))
        Dim mzInputs As Double() = mzpack.MS _
            .Select(Function(i) i.mz) _
            .IteratesALL _
            .GroupBy(PPMmethod.PPM(20)) _
            .Select(Function(a) Val(a.name)) _
            .ToArray
        Dim init0 = pool.GetCandidateSet(peaks:=mzInputs).ToArray
        Dim models = KEGGRepo.RequestKEGGMaps.CreateBackground(KEGGRepo.RequestKeggReactionNetwork).ToArray
        Dim result = init0.PeakListAnnotation(models, permutation:=100)

        Call result.GetJson.SaveTo($"{outputdir}/Mummichog.json")
    End Sub

    <Extension>
    Private Function getIonRange(mzpack As mzPack) As String()
        Dim ionMode As Integer = mzpack.MS _
            .Select(Function(a) a.products) _
            .IteratesALL _
            .First _
            .polarity

        If ionMode = 1 Then
            Return {"[M]+", "[M+H]+"}
        Else
            Return {"[M]-", "[M-H]-"}
        End If
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="raw">the file path of *.mzpack</param>
    ''' <param name="outputdir"></param>
    <ExportAPI("metaDNA")>
    Public Sub MetaDNASearch(raw As String, outputdir As String)
        Dim metaDNA As New Algorithm(Tolerance.PPM(20), 0.4, Tolerance.DeltaMass(0.3))
        Dim mzpack As mzPack

        Using file As Stream = raw.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
            mzpack = mzPack.ReadAll(file)
        End Using

        Dim range As String() = mzpack.getIonRange
        Dim println As Action(Of String) = AddressOf RunSlavePipeline.SendMessage
        Dim infer As CandidateInfer() = metaDNA _
            .SetSearchRange(range) _
            .SetNetwork(KEGGRepo.RequestKEGGReactions(println)) _
            .SetKeggLibrary(KEGGRepo.RequestKEGGcompounds(println)) _
            .SetSamples(mzpack.GetMs2Peaks, autoROIid:=True) _
            .SetReportHandler(println) _
            .DIASearch() _
            .ToArray

        Dim output As MetaDNAResult() = metaDNA _
            .ExportTable(infer, unique:=True) _
            .ToArray

        Call output.SaveTo($"{outputdir}/metaDNA_annotation.csv")
        Call infer.GetJson.SaveTo($"{outputdir}/infer_network.json")
    End Sub

    ''' <summary>
    ''' convert imzML to mzpack
    ''' </summary>
    ''' <param name="imzML"></param>
    ''' <param name="cacheFile"></param>
    <ExportAPI("cache.MSI")>
    Public Sub CreateMSIIndex(imzML As String, cacheFile As String)
        Dim mzpack As mzPack

        RunSlavePipeline.SendProgress(0, "Create workspace cache file, wait for a while...")

        If imzML.ExtensionSuffix("imzML") Then
            mzpack = Converter.LoadimzML(imzML, AddressOf RunSlavePipeline.SendProgress)
        Else
            mzpack = mzPack.ReadAll(imzML.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
        End If

        Call RunSlavePipeline.SendProgress(100, "build pixels index...")

        Try
            Using temp As Stream = cacheFile.Open(FileMode.OpenOrCreate, doClear:=True)
                '    Call XICPackWriter.IndexRawData(raw:=New ReadRawPack(mzpack), file:=temp)
                Call mzpack.Write(temp, progress:=AddressOf RunSlavePipeline.SendMessage)
            End Using
        Catch ex As Exception
        Finally
            Call RunSlavePipeline.SendProgress(100, "Job done!")
        End Try
    End Sub

    <ExportAPI("formula")>
    Public Sub formulaSearch()

    End Sub

    <ExportAPI("MS1deconv")>
    Public Function Deconv(raw As String, massdiff As Double) As PeakFeature()
        Dim pack As mzPack = mzPack.ReadAll(raw.Open)

        Call RunSlavePipeline.SendMessage("get all scan data!")

        Dim scanPoints As ms1_scan() = pack.GetAllScanMs1().ToArray

        Call RunSlavePipeline.SendMessage("create mass groups...")

        Dim massGroups = scanPoints.GetMzGroups(mzdiff:=DAmethod.DeltaMass(massdiff)).ToArray

        Call RunSlavePipeline.SendMessage("Run peak finding for each XIC data...")

        Dim features = massGroups.DecoMzGroups({5, 20}).ToArray

        Return features
    End Function

    <ExportAPI("Ms1Contour")>
    Public Sub DrawMs1Contour(mzpackFile As String, cache As String)
        Dim ms1 As ms1_scan() = mzPack _
            .Read(mzpackFile, ignoreThumbnail:=True).MS _
            .GetMs1Points() _
            .GroupBy(Tolerance.DeltaMass(1.125)) _
            .AsParallel _
            .Select(Function(mz)
                        Return mz _
                            .GroupBy(Function(t)
                                         Return t.scan_time
                                     End Function,
                                     Function(a, b)
                                         Return stdNum.Abs(a - b) <= 5
                                     End Function) _
                            .Select(Function(p)
                                        Return New ms1_scan With {
                                            .mz = Val(mz.name),
                                            .intensity = p.Select(Function(t) t.intensity).Average,
                                            .scan_time = Val(p.name)
                                        }
                                    End Function)
                    End Function) _
            .IteratesALL _
            .ToArray
        Dim data As MeasureData() = ms1 _
            .Select(Function(p)
                        Return New MeasureData(p.scan_time, p.mz, If(p.intensity <= 1, 0, stdNum.Log(p.intensity)))
                    End Function) _
            .ToArray
        Dim layers = ContourLayer _
            .GetContours(data, interpolateFill:=False) _
            .Select(Function(g) g.GetContour) _
            .ToArray

        Call layers.GetJson.SaveTo(cache)
    End Sub

    <ExportAPI("MSI_peaktable")>
    Public Sub ExportMSISampleTable(raw As String, regions As TissueRegion(), save As Stream)
        Dim data As New Dictionary(Of String, ms2())

        Call RunSlavePipeline.SendMessage("Initialize raw data file...")

        Dim render As New Drawer(mzPack.ReadAll(raw.Open(FileMode.Open, doClear:=False, [readOnly]:=True), ignoreThumbnail:=True))
        Dim ppm20 As Tolerance = Tolerance.DeltaMass(0.005)
        Dim j As i32 = 1
        Dim regionId As String
        Dim pixels As PixelScan()

        For Each region As TissueRegion In regions
            RunSlavePipeline.SendProgress(j / regions.Length * 100, $"scan for region {region.label}... [{j}/{regions.Length}]")

            regionId = region.label
            pixels = region.points _
                .Select(Function(p)
                            Return render.pixelReader.GetPixel(p.X, p.Y)
                        End Function) _
                .Where(Function(p) Not p Is Nothing) _
                .ToArray
            data.Add(regionId, pixels.Select(Function(i) i.GetMs).IteratesALL.ToArray)
        Next

        Dim allMz As Double() = data.Values _
            .IteratesALL _
            .ToArray _
            .Centroid(ppm20, New RelativeIntensityCutoff(0.01)) _
            .Select(Function(i) i.mz) _
            .OrderBy(Function(mz) mz) _
            .ToArray

        RunSlavePipeline.SendProgress(100, $"Run peak alignment for {allMz.Length} m/z features!")

        Dim dataSet As DataSet() = allMz _
            .AsParallel _
            .Select(Function(mz)
                        Return New DataSet With {
                            .ID = $"MZ_{mz.ToString("F3")}",
                            .Properties = data _
                                .ToDictionary(Function(a) a.Key,
                                              Function(a)
                                                  Return a.Value.alignMz(mz, ppm20)
                                              End Function)
                        }
                    End Function) _
            .ToArray
        Dim file As New StreamWriter(save)

        Call RunSlavePipeline.SendProgress(100, $"Save peaktable!")
        Call file.WriteLine({"MID"}.JoinIterates(data.Keys).JoinBy(","))

        For Each line As DataSet In dataSet
            Call file.WriteLine({line.ID}.JoinIterates(line(data.Keys).Select(Function(d) d.ToString)).JoinBy(","))
        Next

        Call file.Flush()
    End Sub
End Module
