﻿Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MoleculeNetworking.PoolData
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.Distributions
Imports Microsoft.VisualBasic.MIME
Imports Microsoft.VisualBasic.MIME.Office.Excel.XLS
Imports Microsoft.VisualBasic.My.JavaScript
Imports Microsoft.VisualBasic.Net.Http
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports Mzkit_win32.LCMSViewer

Public Class FormScatterViewer

    Dim WithEvents scatterViewer As PeakScatterViewer
    Dim WithEvents exportReport As ToolStripMenuItem
    Dim WithEvents filterScatter As ToolStripMenuItem

    Dim model As HttpTreeFs
    Dim peaksData As New Dictionary(Of String, MetaIon)
    Dim filterOut As Integer
    Dim filterSingle As Boolean = False

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        scatterViewer = New PeakScatterViewer With {
            .Dock = DockStyle.Fill
        }

        Call Controls.Add(scatterViewer)
    End Sub

    Private Sub FormScatterViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        exportReport = New ToolStripMenuItem With {
            .Text = "Export Ion Report",
            .AutoToolTip = True,
            .AutoSize = True
        }
        filterScatter = New ToolStripMenuItem With {
            .Text = "Filter Scatter Points",
            .AutoSize = True,
            .AutoToolTip = True
        }

        Call ApplyVsTheme(scatterViewer.GetMenu)
        Call scatterViewer.GetMenu.Items.AddRange({exportReport, filterScatter})
    End Sub

    Public Sub LoadClusters(model As HttpTreeFs, topN As Integer)
        Dim url As String = $"{model.HttpServices}/get/cluster/?model_id={model.model_id}&limit_n={topN}"

        Me.model = model

        Call TaskProgress.RunAction(
            run:=Sub(p As ITaskProgress)
                     Dim data = Restful.ParseJSON(url.GET)

                     If data.code <> 0 Then
                         Return
                     End If

                     Call Me.Invoke(Sub() Call LoadClusters(p, data.info, filterOut:=2))
                     Call p.SetInfo("Done!")
                 End Sub,
            info:=$"Load top {topN} clusters..."
        )
    End Sub

    Private Sub LoadClusters(p As ITaskProgress, clusters As Object(), filterOut As Integer)
        Dim precursorList As New List(Of Meta)
        Dim ii As i32 = 0

        Me.filterOut = filterOut

        Call p.SetProgressMode()
        Call peaksData.Clear()

        For Each obj As Object In clusters.SafeQuery
            Dim js As JavaScriptObject = DirectCast(obj, JavaScriptObject)
            Dim id As String = js!id

            If CStr(js!consensus) = "*" Then
                Continue For
            End If

            Dim url As String = $"{model.HttpServices}/load_cluster/?id={id}"
            Dim annotext As String = js!annotations
            Dim note As String = $"Processing cluster [{id}] {annotext} <{CStr(js!n_spectrum)} spectrum>"

            Call p.SetProgress(++ii / clusters.Length * 100, note)
            Call p.SetInfo(note)

            Dim json = Restful.ParseJSON(url.GET)

            If json.code <> 0 Then
                Continue For
            End If

            Dim consensus As (mz As Double(), into As Double()) = HttpTreeFs.DecodeConsensus(js!consensus)
            Dim spectra As New LibraryMatrix With {
                .centroid = True,
                .name = id,
                .ms2 = consensus.mz _
                    .Select(Function(mzi, j) New ms2(mzi, consensus.into(j))) _
                    .ToArray
            }

            Dim data As Object() = json.info!metabolites
            Dim metaIons = data.SafeQuery _
                .AsParallel _
                .Select(Function(o) HttpRESTMetadataPool.ParseMetadata(DirectCast(o, JavaScriptObject))) _
                .ToArray
            Dim precursors = metaIons.GroupBy(Function(m) m.mz, offsets:=0.3) _
                .AsParallel _
                .Select(Function(mzset)
                            'Dim rtlist = mzset.OrderBy(Function(a) a.rt).ToArray
                            'Dim bins = rtlist.GroupBy(Function(m) m.rt, offsets:=30).ToArray

                            'Return bins
                            Return mzset.GroupBy(Function(m) m.rt, offsets:=60)
                        End Function) _
                .IteratesALL

            For Each ion As NamedCollection(Of Metadata) In precursors.Where(Function(o) o.Length > filterOut)
                Dim mz As Double = ion.Select(Function(i) i.mz).Average
                Dim rt As Double() = ion.Select(Function(i) i.rt).TabulateBin
                Dim ion1 As New MetaIon With {
                    .id = $"[{id}] {annotext} {mz.ToString("F4")}@{(rt.Average / 60).ToString("F2")}min <{ion.Length} sample files>",
                    .mz = mz,
                    .rtmin = rt.Min,
                    .rtmax = rt.Max,
                    .scan_time = rt.Average,
                    .intensity = ion.Length,
                    .metaList = ion.value,
                    .cluster = js,
                    .consensus = spectra
                }

                Call precursorList.Add(ion1)
                Call peaksData.Add(ion1.id, ion1)
            Next
        Next

        Call scatterViewer.LoadPeaks(precursorList)
    End Sub

    Private Sub filterScatter_Click(sender As Object, e As EventArgs) Handles filterScatter.Click
        InputDialog.Input(Sub(cfg)
                              filterOut = cfg.FilterNumber
                              filterSingle = cfg.RemoveSingle
                              scatterViewer.LoadPeaks(peaksData.Values.Where(Function(i)
                                                                                 If Not i.metaList.Length > filterOut Then
                                                                                     Return False
                                                                                 End If

                                                                                 If filterSingle Then
                                                                                     Return i.consensus.Length > 1
                                                                                 Else
                                                                                     Return True
                                                                                 End If
                                                                             End Function))
                          End Sub, config:=New InputSetScatterFilterNumber With {.FilterNumber = filterOut, .RemoveSingle = filterSingle})
    End Sub

    Private Sub scatterViewer_MoveOverPeak(peakId As String, mz As Double, rt As Double) Handles scatterViewer.MoveOverPeak

    End Sub

    Private Sub scatterViewer_ClickOnPeak(peakId As String, mz As Double, rt As Double) Handles scatterViewer.ClickOnPeak
        Dim ion As MetaIon = peaksData.TryGetValue(peakId)

        If ion Is Nothing Then
            Return
        End If

        Dim spectrum As PeakMs2() = TaskProgress.LoadData(Of PeakMs2())(
            Function(echo As ITaskProgress)
                Dim println As Action(Of String) = AddressOf echo.SetInfo

                Call echo.SetProgressMode()

                Return ion.metaList _
                    .Select(Function(m, i)
                                Call echo.SetProgress(100 * i / ion.metaList.Length)
                                Call println($"load spectrum: {peakId} | {m.guid}")

                                Dim ms2 = model.ReadSpectrum(m.guid)
                                ms2.mz = m.mz
                                ms2.rt = m.rt
                                ms2.file = m.source_file
                                ms2.scan = m.name
                                ms2.lib_guid = $"[MS/MS][{i + 1}] {peakId} | {m.guid}"

                                Return ms2
                            End Function) _
                    .ToArray
            End Function, title:="Fetch Spectrum From Cloud")

        Call SpectralViewerModule.showCluster(spectrum, ion.id)
    End Sub

    Private Sub exportReport_Click(sender As Object, e As EventArgs) Handles exportReport.Click
        Using file As New SaveFileDialog With {.Filter = "PDF report file(*.pdf)|*.pdf|Excel table(*.xls)|*.xls"}
            If file.ShowDialog = DialogResult.OK Then
                Dim path As String = file.FileName

                If path.ExtensionSuffix("xls") Then
                    Dim xlsxfile As String = TaskProgress.LoadData(
                        streamLoad:=Function(p As ITaskProgress) As String
                                        Return Me.Invoke(Function() RunTableReportExports(p, path))
                                    End Function,
                        title:="Generate Report Exports",
                        info:="Export Report excel table file data")

                    If xlsxfile.FileExists Then
                        Call Process.Start(xlsxfile)
                    Else
                        Call MessageBox.Show("Create excel table file error!", "Export Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                Else
                    Dim pdffile As String = TaskProgress.LoadData(
                        streamLoad:=Function(p As ITaskProgress) As String
                                        Return Me.Invoke(Function() RunReportExports(p, path))
                                    End Function,
                        title:="Generate Report Exports",
                        info:="Export Report pdf file data")

                    If pdffile.FileExists Then
                        Call Process.Start(pdffile)
                    Else
                        Call MessageBox.Show("Create PDF file error!", "Export Report Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End If
                End If
            End If
        End Using
    End Sub

    Private Function RunTableReportExports(p As ITaskProgress, xlsfile As String) As String
        Dim metaIonsDesc As MetaIon() = getReportIonSource()
        Dim table As New List(Of RowObject)
        Dim i As i32 = 0

        Call p.SetProgressMode()
        Call p.SetInfo("Build excel table document file...")

        Call table.Add(New RowObject({"ID", "name", "mz", "rt", "rt(minute)", "rtmin", "rtmax", "spectra", "spectra_text", "nsamples", "sample files"}))

        For Each ion In metaIonsDesc
            Dim img = PeakAssign.DrawSpectrumPeaks(ion.consensus, size:="1920,900").AsGDIImage
            Dim uri As New DataURI(img)

            Call table.Add(New RowObject({
                CStr(ion.cluster!id), ion.id,
                ion.mz, ion.metaList.Select(Function(j) j.rt).Average, (ion.metaList.Select(Function(j) j.rt).Average / 60).ToString("F1"),
                ion.rtmin, ion.rtmax, $"<img src=""{uri}"" width=""450"">",
                ion.consensus.ms2.Select(Function(m) $"{m.mz}_{m.intensity}").JoinBy(" "),
                ion.metaList.Length,
                ion.metaList.Select(Function(a) a.source_file).Distinct.JoinBy(", ")
            }))

            Call p.SetProgress(100 * (++i / metaIonsDesc.Length))
            Call p.SetInfo($"Build table rows: {ion.id} [{i}/{metaIonsDesc.Length}]")

            Call System.Windows.Forms.Application.DoEvents()
        Next

        Call New csv.IO.File(table) _
            .ToExcel("MetaIons", width:=New Dictionary(Of String, String) From {{"spectra", "450"}}) _
            .SaveTo(xlsfile)

        Return xlsfile
    End Function

    Private Function getReportIonSource() As MetaIon()
        Return scatterViewer.GetSelectedIons _
            .Select(Function(m) DirectCast(m, MetaIon)) _
            .Where(Function(o) o.metaList.Length > filterOut) _
            .OrderByDescending(Function(i) i.metaList.Length) _
            .ToArray
    End Function

    Private Function RunReportExports(p As ITaskProgress, pdffile As String) As String
        Dim metaIonsDesc As MetaIon() = getReportIonSource()
        Dim htmltemp As String = TempFileSystem.GetAppSysTempFile(".html", sessionID:=App.PID.ToHexString, prefix:="metabo_clusters")

        Using file As New StreamWriter(htmltemp.Open(FileMode.OpenOrCreate, doClear:=True))
            Dim i As i32 = 0

            Call p.SetProgressMode()
            Call p.SetInfo("Build html document file...")

            Call file.WriteLine("<style></style>")

            For Each ion In metaIonsDesc
                Dim img = PeakAssign.DrawSpectrumPeaks(ion.consensus, size:="1920,900").AsGDIImage
                Dim uri As New DataURI(img)

                Call file.WriteLine($"<h2>{ion.id}</h2>")
                Call file.WriteLine($"<p><img src=""{uri}"" style=""width: 85%;""></p>")
                Call file.WriteLine($"<p>precursor m/z: {ion.mz.ToString("F4")}</p>")
                Call file.WriteLine($"<p>RT range: {ion.rtmin.ToString("F0")} ~ {ion.rtmax.ToString("F0")}s | {(ion.rtmin / 60).ToString("F2")} ~ {(ion.rtmax / 60).ToString("F2")}min</p>")
                Call file.WriteLine($"<p>Find {ion.metaList.Length} spectrum</p>")
                Call file.WriteLine($"<p>Find in samples: <i>{ion.metaList.Select(Function(io) io.source_file).Distinct.JoinBy(", ")}</i></p>")
                Call file.WriteLine(Html.Document.Pagebreak)

                Call p.SetProgress(100 * (++i / metaIonsDesc.Length))
                Call p.SetInfo($"Build html document file... [{i}/{metaIonsDesc.Length}]")
                Call System.Windows.Forms.Application.DoEvents()
            Next

            Call file.Flush()
        End Using

        Call p.SetInfo("Create PDF report file...")
        Call Helper.PDF(pdffile, htmltemp)

        Return pdffile
    End Function

End Class

Public Class MetaIon : Inherits Meta

    Public Property metaList As Metadata()
    Public Property rtmin As Double
    Public Property rtmax As Double
    Public Property cluster As JavaScriptObject
    Public Property consensus As LibraryMatrix

End Class