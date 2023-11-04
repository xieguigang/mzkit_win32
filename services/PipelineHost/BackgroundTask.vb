#Region "Microsoft.VisualBasic::77d0d884bd140d9c4590f9af8ffc4248, mzkit\src\mzkit\services\PipelineHost\BackgroundTask.vb"

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

'   Total Lines: 353
'    Code Lines: 289
' Comment Lines: 12
'   Blank Lines: 52
'     File Size: 14.08 KB


' Module BackgroundTask
' 
'     Constructor: (+1 Overloads) Sub New
' 
'     Function: alignMz, cfmidPredict, convertMzPack, Deconv, getIonRange
'               readTissues, RunFeatureDetections
' 
'     Sub: CreateMSIIndex, DrawMs1Contour, ExportMSISampleTable, formulaSearch, MetaDNASearch
'          Mummichog, SetBioDeepSession
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports BioDeep
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Reader
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep.MSEngine.Mummichog
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Bootstrapping
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors.Scaler
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.My
Imports Microsoft.VisualBasic.My.FrameworkInternal
Imports Microsoft.VisualBasic.Parallel
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports MZWorkPack
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports list = SMRUCC.Rsharp.Runtime.Internal.Object.list
Imports spatialMath = BioNovoGene.Analytical.MassSpectrometry.SingleCells.Deconvolute.Math
Imports std = System.Math

<Package("BackgroundTask")>
<RTypeExport("ms_search.args", GetType(MassSearchArguments))>
Module BackgroundTask

    Sub New()
        FrameworkInternal.ConfigMemory(MemoryLoads.Heavy)
        VectorTask.n_threads = std.Max(8, App.CPUCoreNumbers)
    End Sub

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

    Private Function getPolarity(mzpack As mzPack) As Integer
        Return mzpack.MS _
            .Select(Function(a) a.products) _
            .IteratesALL _
            .First _
            .polarity
    End Function

    <ExportAPI("Mummichog")>
    Public Function Mummichog(<RRawVectorArgument> raw As Object, args As MassSearchArguments, outputdir As String, Optional env As Environment = Nothing) As Object
        Dim mzInputs As Double()
        Dim printf = env.WriteLineHandler

        If TypeOf raw Is String Then
            Dim mzpack As mzPack

            Using file As Stream = CStr(raw).Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                mzpack = mzPack.ReadAll(file)
            End Using

            mzInputs = mzpack.MS _
                .Select(Function(i) i.mz) _
                .IteratesALL _
                .GroupBy(PPMmethod.PPM(20)) _
                .Select(Function(a) Val(a.name)) _
                .ToArray
        Else
            mzInputs = CLRVector.asNumeric(raw)
        End If

        Call printf("get ions m/z set:")
        Call printf(mzInputs)

        Dim println As Action(Of String) = AddressOf RunSlavePipeline.SendMessage
        Dim keggCompounds = KEGGRepo.RequestKEGGCompounds()
        Dim range As MzCalculator() = If(args.Adducts, getIonRange(args.IonMode)) _
            .Select(Function(adducts) Parser.ParseMzCalculator(adducts)) _
            .ToArray
        Dim pool As IMzQuery = KEGGHandler.CreateIndex(keggCompounds, range, PPMmethod.PPM(args.PPM))
        Dim init0 = pool.GetCandidateSet(peaks:=mzInputs).ToArray
        Dim models = KEGGRepo.RequestKEGGMaps.CreateBackground(KEGGRepo.RequestKeggReactionNetwork).ToArray
        Dim result = init0.PeakListAnnotation(models, permutation:=Integer.Parse(args.Optionals("permutation")))

        Return result _
            .GetJson _
            .SaveTo($"{outputdir}/Mummichog.json")
    End Function

    <Extension>
    Private Function getIonRange(ionMode As Integer) As String()
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

        Dim range As String() = getIonRange(getPolarity(mzpack))
        Dim println As Action(Of String) = AddressOf RunSlavePipeline.SendMessage
        Dim infer As CandidateInfer() = metaDNA _
            .SetSearchRange(range) _
            .SetNetwork(KEGGRepo.RequestKEGGReactions(println)) _
            .SetKeggLibrary(KEGGRepo.RequestKEGGCompounds()) _
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

        Dim features = massGroups.DecoMzGroups(New Double() {5, 20}).ToArray

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
                                         Return std.Abs(a - b) <= 5
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
                        Return New MeasureData(p.scan_time, p.mz, If(p.intensity <= 1, 0, std.Log(p.intensity)))
                    End Function) _
            .ToArray
        Dim layers = ContourLayer _
            .GetContours(data, interpolateFill:=False) _
            .Select(Function(g) g.GetContour) _
            .ToArray

        Call layers.GetJson.SaveTo(cache)
    End Sub

    ''' <summary>
    ''' extract the ms-imaging raw data peak feature table
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="regions"></param>
    ''' <param name="save"></param>
    ''' <param name="mzdiff"></param>
    ''' <param name="into_cutoff"></param>
    ''' <param name="TrIQ"></param>
    <ExportAPI("MSI_peaktable")>
    Public Function ExportMSISampleTable(raw As String, regions As TissueRegion(), save As Stream,
                                         Optional mzdiff As Object = "da:0.005",
                                         Optional into_cutoff As Double = 0.05,
                                         Optional TrIQ As Double = 0.6,
                                         Optional save_bin As Boolean = False,
                                         Optional env As Environment = Nothing) As Object

        Dim mzerr = Math.getTolerance(mzdiff, env, [default]:="da:0.005")
        Dim dataset As List(Of NamedCollection(Of Double))
        Dim titleKeys As String() = Nothing

        If mzerr Like GetType(Message) Then
            Return mzerr.TryCast(Of Message)
        Else
            Call RunSlavePipeline.SendMessage("Initialize raw data file...")
        End If

        dataset = raw.exportMatrixRows(regions, titleKeys, mzerr.TryCast(Of Tolerance), TrIQ, into_cutoff)

        Call RunSlavePipeline.SendProgress(100, $"Save peaktable!")

        If save_bin Then
            ' save as the GCModeller HTS matrix object
            Dim spotSet As DataFrameRow() = dataset.Select(Function(i) New DataFrameRow(i)).ToArray
            Dim hts As New Matrix With {
                .expression = spotSet,
                .sampleID = titleKeys,
                .tag = raw.BaseName
            }

            Call RunSlavePipeline.SendMessage("save binary matrix file!")
            Call hts.Save(save)
        Else
            Dim file As New StreamWriter(save)

            ' the data keys is the column names
            Call file.WriteLine({"MID"}.JoinIterates(titleKeys).JoinBy(","))
            Call RunSlavePipeline.SendMessage("save csv ascii text file!")

            For Each line As NamedCollection(Of Double) In dataset
                Call New String() {"""" & line.name & """"} _
                    .JoinIterates(line.value.Select(Function(d) d.ToString)) _
                    .JoinBy(",") _
                    .DoCall(AddressOf file.WriteLine)
            Next

            Call file.Flush()
        End If

        Return Nothing
    End Function

    <Extension>
    Private Function exportMatrixRows(raw As String, regions As TissueRegion(), ByRef titleKeys As String(),
                                      mzerr As Tolerance,
                                      TrIQ As Double,
                                      into_cutoff As Double) As List(Of NamedCollection(Of Double))
        Dim render As Drawer
        Dim ppm20 As Tolerance = mzerr
        Dim loadraw = MSImagingReader.UnifyReadAsMzPack(raw)
        Dim annos As Dictionary(Of String, String) = Nothing
        Dim index_win As Double
        Dim dataKeys As String() = Nothing
        Dim dataset As List(Of NamedCollection(Of Double))

        Call RunSlavePipeline.SendMessage("load raw data into ms-imaging render")

        If loadraw Like GetType(mzPack) Then
            annos = loadraw.TryCast(Of mzPack).Annotations
            render = New Drawer(loadraw.TryCast(Of mzPack))
        Else
            render = New Drawer(loadraw.TryCast(Of ReadRawPack))
        End If

        Call RunSlavePipeline.SendMessage("start to export MSI feature peaks table!")

        If regions.IsNullOrEmpty Then
            dataset = render.exportMSIRawPeakTable(ppm20, into_cutoff, dataKeys, TrIQ, index_win)
        Else
            dataset = regions.exportRegionDataset(render, ppm20, into_cutoff, dataKeys, TrIQ, index_win)
        End If

        titleKeys = dataKeys.ToArray

        If Not annos.IsNullOrEmpty Then
            Dim nameIndex = New BlockSearchFunction(Of (mz As Double, String))(
                data:=annos.Select(Function(mzi) (Val(mzi.Key), mzi.Value)),
                eval:=Function(i) i.mz,
                tolerance:=index_win,
                fuzzy:=True
            )

            For i As Integer = 0 To titleKeys.Length - 1
                Dim mzkey As Double = Val(titleKeys(i))
                Dim hit = nameIndex _
                   .Search((mzkey, -1)) _
                   .OrderBy(Function(a) std.Abs(a.mz - mzkey)) _
                   .FirstOrDefault

                If hit.mz = 0 AndAlso hit.Item2.StringEmpty Then
                    ' no hits
                    ' do nothing
                Else
                    titleKeys(i) = hit.Item2
                End If
            Next
        End If

        Return dataset
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="render"></param>
    ''' <param name="da"></param>
    ''' <param name="into_cutoff"></param>
    ''' <param name="triq"></param>
    ''' <returns>
    ''' spot pixel id in row, and
    ''' ion m/z peak features in column
    ''' </returns>
    <Extension>
    Private Function exportMSIRawPeakTable(render As Drawer, da As Tolerance, into_cutoff As Double, ByRef dataKeys As String(), triq As Double, ByRef index_win As Double) As List(Of NamedCollection(Of Double))
        Dim allMs As New List(Of ms2)
        Dim allPixels = render.LoadPixels.ToArray

        Call RunSlavePipeline.SendMessage($"get {allPixels.Length} pixel spots for export feature peaks!")

        For Each pixel As PixelScan In allPixels
            Call allMs.AddRange(pixel.GetMs)
        Next

        Call RunSlavePipeline.SendMessage("pick of the unique ion features...")

        Dim allMz As Double() = allMs.uniqueMz(da, into_cutoff, triq)
        Dim mzKeys As String() = allMz.Select(Function(mzi) mzi.ToString("F4")).ToArray

        RunSlavePipeline.SendProgress(0, $"Run peak alignment for {allMz.Length} m/z features!")

        Dim dataset As New List(Of NamedCollection(Of Double))
        Dim size As Double = eval_winsize(allMz)

        dataKeys = mzKeys
        index_win = size
        ' duplicated pixel may be found in the data
        dataset.AddRange(allPixels.GroupBy(Function(s) $"{s.X},{s.Y}").ToArray.ExportSpotVectors(Function(a) a.Key, Function(pixel) pixel.Select(Function(si) si.GetMs).IteratesALL.ToArray, allMz, index_win))

        Return dataset
    End Function

    Private Function eval_winsize(allMz As Double()) As Double
        allMz = allMz.OrderByDescending(Function(a) a).ToArray

        Dim diff As New List(Of Double)

        For i As Integer = 1 To allMz.Length - 1
            diff.Add(allMz(i - 1) - allMz(i))
        Next

        If diff.Average > 0.9 AndAlso allMz.Length > 5000 Then
            ' ST data probably
            Return 300
        Else
            Return std.Max(diff.Average * 10, 1.5)
        End If
    End Function

    <Extension>
    Private Iterator Function ExportSpotVectors(Of T)(rawdata As T(), getKey As Func(Of T, String), getMs As Func(Of T, ms2()), allMz As Double(), index_win As Double) As IEnumerable(Of NamedCollection(Of Double))
        Dim d As Integer = rawdata.Length / 50
        Dim p As i32 = Scan0
        Dim t0 As Date = Now
        Dim index = allMz.CreateMzIndex(win_size:=index_win)
        Dim len As Integer = allMz.Length

        For Each pixel As T In rawdata
            Dim pixelKey As String = getKey(pixel)
            Dim ms1 As ms2() = getMs(pixel)
            Dim scan_mz As Double() = ms1.Select(Function(a) a.mz).ToArray
            Dim scan_into As Double() = ms1.Select(Function(a) a.intensity).ToArray
            Dim vec As Double() = spatialMath.DeconvoluteScan(scan_mz, scan_into, len, index)

            If vec.All(Function(di) di = 0.0) Then
                Continue For
            End If

            Yield New NamedCollection(Of Double) With {
                .name = pixelKey,
                .value = vec
            }

            If (++p) Mod d = 0 Then
                Call RunSlavePipeline.SendProgress(p / rawdata.Length * 100, $"[{(100 * p / rawdata.Length).ToString("F1")}%] {p / CInt((Now - t0).TotalSeconds)} pixel/s; {pixelKey}")
            End If
        Next
    End Function

    <Extension>
    Private Function uniqueMz(allMs As IEnumerable(Of ms2), da As Tolerance, into_cutoff As Double, triq As Double) As Double()
        Dim raw = allMs.ToArray
        Dim threshold As Double = raw.Select(Function(m) m.intensity).FindThreshold(triq)

        Return raw.AsParallel _
            .Select(Function(msi)
                        ' apply of the intensity trim cutoff
                        If msi.intensity > threshold Then
                            msi.intensity = threshold
                        End If

                        Return msi
                    End Function) _
            .ToArray _
            .Centroid(da, New RelativeIntensityCutoff(into_cutoff)) _
            .Select(Function(i) std.Round(i.mz, 4)) _
            .Distinct _
            .OrderBy(Function(mz) mz) _
            .ToArray
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="regions"></param>
    ''' <param name="render"></param>
    ''' <param name="ppm20"></param>
    ''' <param name="into_cutoff"></param>
    ''' <param name="dataKeys">column names</param>
    ''' <returns>
    ''' m/z ion features in row, and
    ''' region labels in column
    ''' </returns>
    <Extension>
    Private Function exportRegionDataset(regions As TissueRegion(), render As Drawer, ppm20 As Tolerance,
                                         into_cutoff As Double,
                                         ByRef dataKeys As String(),
                                         triq As Double, ByRef index_win As Double) As List(Of NamedCollection(Of Double))

        Dim data As New Dictionary(Of String, ms2())
        Dim j As i32 = 1
        Dim regionId As String
        Dim pixels As PixelScan()

        For Each region As TissueRegion In regions
            RunSlavePipeline.SendProgress(j / regions.Length * 100, $"scan for region {region.label}... [{++j}/{regions.Length}]")

            regionId = region.label
            pixels = region.points _
                .Select(Function(p)
                            Return render.pixelReader.GetPixel(p.X, p.Y)
                        End Function) _
                .Where(Function(p) Not p Is Nothing) _
                .ToArray
            data.Add(regionId, pixels.Select(Function(i) i.GetMs).IteratesALL.ToArray)
        Next

        Dim allMz As Double() = data.Values.IteratesALL.uniqueMz(ppm20, into_cutoff, triq)
        Dim size As Double = eval_winsize(allMz)

        dataKeys = allMz.Select(Function(a) a.ToString("F4")).ToArray
        index_win = size

        RunSlavePipeline.SendProgress(100, $"Run peak alignment for {allMz.Length} m/z features!")

        Dim dataSet = data.ToArray.ExportSpotVectors(Function(a) a.Key, Function(a) a.Value, allMz, index_win:=size).AsList

        Return dataSet
    End Function

    <ExportAPI("linear.ions_raw")>
    Public Function linear_ionsRaw(linearPack As LinearPack) As list
        Return New list With {
            .slots = linearPack.peakSamples _
                .GroupBy(Function(sample) sample.Name) _
                .ToDictionary(Function(ion) ion.Key,
                              Function(ionGroup)
                                  Dim innerList As New list With {
                                      .slots = ionGroup _
                                          .ToDictionary(Function(ion) ion.SampleName,
                                                        Function(ion)
                                                            Return CObj(ion.Peak.ticks)
                                                        End Function)
                                  }

                                  Return CObj(innerList)
                              End Function)
        }
    End Function

    <ExportAPI("linear.setErrPoints")>
    Public Function linear_setErrorPoints(linearPack As LinearPack) As Object
        For Each line As LinearQuantitative.StandardCurve In linearPack.linears
            line.linear.ErrorTest = line.points _
                .Select(Function(p)
                            Return CType(New TestPoint With {.X = p.Px, .Y = p.Cti, .Yfit = p.yfit}, IFitError)
                        End Function) _
                .ToArray
        Next

        Return linearPack
    End Function
End Module
