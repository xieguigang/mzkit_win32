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
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep.MSEngine.Mummichog
Imports Microsoft.VisualBasic.CommandLine.InteropService.Pipeline
Imports Microsoft.VisualBasic.CommandLine.Reflection
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
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Serialization.JSON
Imports MZWorkPack
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports list = SMRUCC.Rsharp.Runtime.Internal.Object.list
Imports stdNum = System.Math

<Package("BackgroundTask")>
<RTypeExport("ms_search.args", GetType(MassSearchArguments))>
Module BackgroundTask

    Sub New()
        FrameworkInternal.ConfigMemory(MemoryLoads.Heavy)
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
                                         Optional env As Environment = Nothing) As Object

        Dim mzerr = Math.getTolerance(mzdiff, env, [default]:="da:0.005")
        Dim dataKeys As String() = Nothing
        Dim dataset As DataSet()

        If mzerr Like GetType(Message) Then
            Return mzerr.TryCast(Of Message)
        Else
            Call RunSlavePipeline.SendMessage("Initialize raw data file...")
        End If

        Dim render As Drawer
        Dim ppm20 As Tolerance = mzerr.TryCast(Of Tolerance)
        Dim file As New StreamWriter(save)
        Dim loadraw = MSImagingReader.UnifyReadAsMzPack(raw)

        Call RunSlavePipeline.SendMessage("load raw data into ms-imaging render")

        If loadraw Like GetType(mzPack) Then
            render = New Drawer(loadraw.TryCast(Of mzPack))
        Else
            render = New Drawer(loadraw.TryCast(Of ReadRawPack))
        End If

        Call RunSlavePipeline.SendMessage("start to export MSI feature peaks table!")

        If regions.IsNullOrEmpty Then
            dataset = render.exportMSIRawPeakTable(ppm20, into_cutoff, dataKeys, TrIQ)
        Else
            dataset = regions.exportRegionDataset(render, ppm20, into_cutoff, dataKeys, TrIQ)
        End If

        Call RunSlavePipeline.SendProgress(100, $"Save peaktable!")
        ' the data keys is the column names
        Call file.WriteLine({"MID"}.JoinIterates(dataKeys).JoinBy(","))

        For Each line As DataSet In dataset
            Call New String() {"""" & line.ID & """"} _
                .JoinIterates(line(dataKeys).Select(Function(d) d.ToString)) _
                .JoinBy(",") _
                .DoCall(AddressOf file.WriteLine)
        Next

        Call file.Flush()

        Return Nothing
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
    Private Function exportMSIRawPeakTable(render As Drawer, da As Tolerance, into_cutoff As Double, ByRef dataKeys As String(), triq As Double) As DataSet()
        Dim allMs As New List(Of ms2)
        Dim allPixels = render.LoadPixels.ToArray

        Call RunSlavePipeline.SendMessage($"get {allPixels.Length} pixel spots for export feature peaks!")

        For Each pixel As PixelScan In allPixels
            Call allMs.AddRange(pixel.GetMs)
        Next

        Call RunSlavePipeline.SendMessage("pick of the unique ion features...")

        Dim allMz As Double() = allMs.uniqueMz(da, into_cutoff, triq)
        Dim mzKeys As String() = allMz.Select(Function(mzi) mzi.ToString("F3")).ToArray

        RunSlavePipeline.SendProgress(100, $"Run peak alignment for {allMz.Length} m/z features!")

        Dim dataSet As New List(Of DataSet)
        Dim d As Integer = allPixels.Length / 100
        Dim p As i32 = Scan0
        Dim t0 As Date = Now

        For Each pixel As PixelScan In allPixels
            Dim vec As New Dictionary(Of String, Double)
            Dim ms1 = pixel.GetMs

            Call allMz _
                .AsParallel _
                .Select(Function(mzKey, i)
                            Dim into As Double = ms1 _
                                .Where(Function(mzi) da(mzi.mz, mzKey)) _
                                .Sum(Function(a) a.intensity)
                            Dim t = (mzKey, i, into)

                            Return t
                        End Function) _
                .ToArray _
                .ForEach(Sub(t, i)
                             Call vec.Add(mzKeys(t.i), t.into)
                         End Sub)

            Call dataSet.Add(New DataSet With {
                .ID = $"{pixel.X},{pixel.Y}",
                .Properties = vec
            })

            If (++p) Mod d = 0 Then
                Call RunSlavePipeline.SendProgress(p / d * 100, $"[{(100 * p / d).ToString("F1")}%] {p / CInt((Now - t0).TotalSeconds)} pixel/s; {pixel.scanId}")
            End If
        Next

        dataKeys = mzKeys

        Return dataSet.ToArray
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
            .Select(Function(i) i.mz) _
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
                                         triq As Double) As DataSet()

        Dim data As New Dictionary(Of String, ms2())
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

        dataKeys = data.Keys.ToArray

        Dim allMz As Double() = data.Values.IteratesALL.uniqueMz(ppm20, into_cutoff, triq)

        RunSlavePipeline.SendProgress(100, $"Run peak alignment for {allMz.Length} m/z features!")

        Dim generator = data
        Dim dataSet As DataSet() = allMz _
            .AsParallel _
            .Select(Function(mz)
                        Return New DataSet With {
                            .ID = $"MZ_{mz.ToString("F3")}",
                            .Properties = generator _
                                .ToDictionary(Function(a) a.Key,
                                              Function(a)
                                                  Return a.Value.alignMz(mz, ppm20)
                                              End Function)
                        }
                    End Function) _
            .ToArray

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
