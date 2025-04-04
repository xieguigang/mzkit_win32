﻿Imports System.ComponentModel
Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports BioNovoGene.BioDeep.Chemistry.MetaLib.Models
Imports BioNovoGene.BioDeep.Chemistry.NCBI.PubChem
Imports BioNovoGene.BioDeep.Chemoinformatics
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula.MS
Imports BioNovoGene.BioDeep.MSFinder
Imports BioNovoGene.mzkit_win32.ServiceHub
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.ANOVA
Imports Microsoft.VisualBasic.My.JavaScript
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports RibbonLib.Interop
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports SMRUCC.Rsharp.Runtime.Internal.[Object]
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports TaskStream
Imports any = Microsoft.VisualBasic.Scripting
Imports csv = Microsoft.VisualBasic.Data.Framework.IO.File
Imports Matrix = SMRUCC.genomics.Analysis.HTS.DataFrame.Matrix
Imports std = System.Math

Public Class frmMetabonomicsAnalysis

    Dim sampleinfo As SampleInfo()
    Dim properties As String()
    Dim peaks As PeakSet

    ''' <summary>
    ''' could be custom in the wizard dialog
    ''' </summary>
    Friend workdir As String = getTemp()

    Friend imageWeb As Process
    Friend webPort As Integer

    Private Sub startfs()
        webPort = Net.Tcp.GetFirstAvailablePort(6001)
        imageWeb = New Process With {
            .StartInfo = New ProcessStartInfo With {
                .FileName = $"{App.HOME}/Rstudio/bin/Rserve.exe",
                .Arguments = $"--listen /wwwroot ""{workdir}"" /port {webPort} --parent={App.PID}",
                .CreateNoWindow = True,
                .WindowStyle = ProcessWindowStyle.Hidden,
                .UseShellExecute = True
            }
        }

        Call imageWeb.Start()
        Call App.AddExitCleanHook(Sub() Call imageWeb.Kill())
        Call ServiceHub.Manager.Hub.Register(New Manager.Service With {
            .Name = "Metabonomics Workbench",
            .Description = "host image files and analysis result files for viewer",
            .isAlive = True,
            .PID = imageWeb.Id,
            .Port = webPort,
            .CPU = 0,
            .Memory = 0,
            .Protocol = "HTTP 1.0",
            .StartTime = Now.ToString,
            .CommandLine = Manager.Service.GetCommandLine(imageWeb)
        })

        Call WorkStudio.LogCommandLine(imageWeb)
    End Sub

    ''' <summary>
    ''' get temp workspace directory path
    ''' </summary>
    ''' <returns></returns>
    Private Shared Function getTemp() As String
        Return App.CurrentProcessTemp & "/" & App.CurrentUnixTimeMillis & "/"
    End Function

    Public Shared Function CastMatrix(peaktable As PeakSet, sampleinfo As SampleInfo()) As Matrix
        Dim mols As New List(Of DataFrameRow)

        For Each mol As xcms2 In peaktable.peaks
            Call mols.Add(New DataFrameRow With {
                .geneID = mol.ID,
                .experiments = mol(sampleinfo)
            })
        Next

        Return New Matrix With {
            .expression = mols.ToArray,
            .sampleID = sampleinfo _
                .Select(Function(s) s.ID) _
                .ToArray
        }
    End Function

    Dim xcms_id As String()
    Dim annotation As New Dictionary(Of String, AnnotatedIon)
    Dim meta As New Dictionary(Of String, MetaLib)
    Dim mapping As New Dictionary(Of String, String)

    Public Sub LoadSampleData(table As DataTable)
        Dim groups = sampleinfo.GroupBy(Function(s) s.sample_info) _
            .Select(Function(g) (g.Key, list:=g.ToArray)) _
            .ToArray

        Call table.Columns.Add("id", GetType(String))

        For Each group In groups
            Dim col = table.Columns.Add(group.Key, GetType(Double))
            col.ExtendedProperties.Add("color", group.list.First.color.TranslateColor)
        Next

        Dim offset As Integer = 0
        Dim filter As Boolean = ionFilter.Checked

        xcms_id = New String(peaks.ROIs - 1) {}

        For Each peak As xcms2 In peaks.peaks
            Dim row As Object() = New Object(groups.Length) {}
            Dim display As AnnotatedIon = annotation.TryGetValue(peak.ID)
            Dim checked As Boolean = True

            If display Is Nothing Then
                row(0) = peak.ID

                If filter Then
                    checked = False
                End If
            Else
                row(0) = $"{display.metadata.CommonName}_{display.AdductIon.ToString}@{(peak.rt / 60).ToString("F1")}min"
            End If

            mapping(row(0)) = peak.ID
            xcms_id(offset) = peak.ID
            offset += 1

            For i As Integer = 0 To groups.Length - 1
                Dim group = groups(i).list
                Dim data As Double() = peak(group.Select(Function(s) s.ID))

                row(i + 1) = data.Average
            Next

            If checked Then
                Call table.Rows.Add(row)
            End If
        Next
    End Sub

    Public Sub LoadAnalysisTable(table As DataTable, data As DataSet())
        Dim keys As String()

        If data.IsNullOrEmpty Then
            Call MessageBox.Show("No analysis data table contents, please try to run data analysis first.", "Missing data", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        Else
            keys = data(0).Properties.Keys.ToArray
        End If

        Call table.Columns.Add("id", GetType(String))

        For Each key As String In keys
            Call table.Columns.Add(key, GetType(Double))
        Next

        For Each row As DataSet In data
            Dim r As Object() = New Object(row.Properties.Count) {}
            r(0) = row.ID

            For j = 0 To keys.Length - 1
                r(j + 1) = row(keys(j))
            Next

            Call table.Rows.Add(r)
        Next
    End Sub

    Public Sub LoadAnalysisTable(table As DataTable, data As EntityObject())
        Dim keys As String()

        If data.IsNullOrEmpty Then
            Call MessageBox.Show("No analysis data table contents, please try to run data analysis first.", "Missing data", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        Else
            keys = data(0).Properties.Keys.ToArray
        End If

        Call table.Columns.Add("id", GetType(String))

        For Each key As String In keys
            Call table.Columns.Add(key, GetType(String))
        Next

        For Each row As EntityObject In data
            Dim r As Object() = New Object(row.Properties.Count) {}
            r(0) = row.ID

            For j = 0 To keys.Length - 1
                r(j + 1) = row(keys(j))
            Next

            Call table.Rows.Add(r)
        Next
    End Sub

    Public Delegate Sub LoadDataCallback(sampleinfo As SampleInfo(), properties As String(), df As DataFrameResolver, workdir As String)

    Public Shared Sub LoadData(df As DataFrameResolver, sourcefile As String, callback As LoadDataCallback)
        Dim wizard As New InputImportsPeaktableDialog
        Dim checkTitles As Index(Of String) = df.HeadTitles.Indexing
        Dim checks As String()() = {
            New String() {"id", "ID", "xcms_id"},
            New String() {"mz", "m/z", "MZ"},
            New String() {"rt", "RT", "retention_time"}
        }

        If Not checks.All(Function(l) l.Any(Function(si) si Like checkTitles)) Then
            Call Workbench.Warning("In-correct table format: missing one of the data fields for the ion data(xcms_id, mz, rt?).")
            Return
        End If

        Call wizard.LoadSampleId(df.HeadTitles)

        If Not sourcefile Is Nothing Then
            ' for in-memory generated table data
            ' source file is nothing
            Call wizard.SetDefaultWorkdir(sourcefile.ParentPath)
        End If

        Call InputDialog.Input(
            Sub(config)
                Dim sampleinfo = config.GetSampleInfo.ToArray
                Dim properties = config.GetMetadata.ToArray
                Dim workdir As String

                If Not Strings.Trim(config.GetWorkspace).StringEmpty Then
                    workdir = Strings.Trim(config.GetWorkspace)
                Else
                    workdir = getTemp()
                End If

                Call callback(sampleinfo, properties, df, workdir)
            End Sub, Nothing, wizard)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub LoadData(table As csv, sourcefile As String, callback As LoadDataCallback)
        Call LoadData(df:=DataFrameResolver.CreateObject(table), sourcefile, callback)
    End Sub

    ''' <summary>
    ''' imports from csv table
    ''' </summary>
    ''' <param name="sampleinfo"></param>
    ''' <param name="properties"></param>
    ''' <param name="df"></param>
    ''' <param name="workdir"></param>
    ''' <param name="title"></param>
    Public Sub LoadData(sampleinfo As SampleInfo(), properties As String(), df As DataFrameResolver, workdir As String, title As String)
        ' show data
        Dim peaks As New List(Of xcms2)
        Dim peak As xcms2

        Me.workdir = workdir
        Me.sampleinfo = sampleinfo
        Me.properties = properties

        For Each row In df.EnumerateData
            peak = New xcms2 With {
                .ID = row.TryPopOut({"id", "ID", "xcms_id"}),
                .mz = row.TryPopOut({"mz", "m/z", "MZ"}),
                .mzmax = row.TryPopOut({"mzmax", "mz.max"}, [default]:= .mz),
                .mzmin = row.TryPopOut({"mzmin", "mz.min"}, [default]:= .mz),
                .rt = row.TryPopOut({"rt", "RT", "retention_time"}),
                .rtmax = row.TryPopOut({"rtmax", "rt.max"}, [default]:= .rt),
                .rtmin = row.TryPopOut({"rtmin", "rt.min"}, [default]:= .rt),
                .Properties = New Dictionary(Of String, Double)
            }

            For Each id As SampleInfo In sampleinfo
                peak(id.ID) = Val(row.TryPopOut(id.ID))
            Next

            Dim meta As New JavaScriptObject

            For Each item In row
                meta(item.Key) = item.Value
            Next

            Call peaks.Add(peak)
        Next

        Me.peaks = New PeakSet With {
            .peaks = peaks.uniqueNames.ToArray
        }

        Call loadTable(Sub(tbl) Call LoadSampleData(tbl))
        Call startfs()

        Using f As Stream = $"{workdir}/peakset.xcms".Open(FileMode.OpenOrCreate, doClear:=True)
            Call SaveXcms.DumpSample(Me.peaks, f)
            Call f.Flush()
        End Using

        Using f As Stream = matrixfile.Open(FileMode.OpenOrCreate, doClear:=True)
            Call sampleinfo.SaveTo(sampleinfofile)
            Call CastMatrix(Me.peaks, sampleinfo).Save(f)
            Call Workbench.LogText($"set workspace for metabonomics workbench: {workdir}")
        End Using
    End Sub

    Public Sub LoadWorkspace(dir As String, Optional buzy As Boolean = True)
        If buzy Then
            Call ProgressSpinner.DoLoading(Sub() Call LoadWorkspaceTask(dir), host:=Me)
        Else
            Call LoadWorkspaceTask(dir)
        End If
    End Sub

    Private Sub LoadWorkspaceTask(dir As String)
        Dim rawdata As String = $"{dir}/peakset.xcms"
        Dim normdata As String = $"{dir}/norm.xcms"
        Dim sourcefile As String = If(normdata.FileLength > 0, normdata, rawdata)

        Me.workdir = dir
        Me.peaks = SaveXcms.ReadSample(sourcefile.Open(FileMode.Open, doClear:=False, [readOnly]:=True))
        Me.sampleinfo = sampleinfofile.LoadCsv(Of SampleInfo)().ToArray

        ' export expression matrix for
        ' run data visualization and data nslsysis
        Using f As Stream = matrixfile.Open(FileMode.OpenOrCreate, doClear:=True)
            Call CastMatrix(Me.peaks, sampleinfo).Save(f)
            Call Workbench.LogText($"set workspace for metabonomics workbench: {workdir}")
        End Using

        Call loadTable(Sub(table) Call LoadSampleData(table))
        Call startfs()
    End Sub

    Public Sub exportMatrixExcelFile()
        Dim rawdata As String = $"{workdir}/peakset.xcms"
        Dim normdata As String = $"{workdir}/norm.xcms"
        Dim sourcefile As String = If(normdata.FileLength > 0, normdata, rawdata)

        If peaks Is Nothing OrElse sampleinfo.IsNullOrEmpty Then
            Workbench.Warning("please load analysis peaktable data at first!")
            Return
        End If

        Using file As New SaveFileDialog With {.Filter = "Excel Table(*.csv)|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                If CastMatrix(Me.peaks, sampleinfo).SaveMatrix(file.FileName, "xcms_id") Then
                    MessageBox.Show("Matrix export success!" & vbCrLf & file.FileName, "Data export", MessageBoxButtons.OK, MessageBoxIcon.Exclamation)
                Else
                    Workbench.Warning("matrix export error.")
                End If
            End If
        End Using
    End Sub

    ''' <summary>
    ''' the matrix binary file
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property matrixfile As String
        Get
            Return $"{workdir}/mat.dat"
        End Get
    End Property

    ''' <summary>
    ''' the csv file path to the sampleinfo data
    ''' </summary>
    ''' <returns></returns>
    Private ReadOnly Property sampleinfofile As String
        Get
            Return $"{workdir}/sampleinfo.csv"
        End Get
    End Property

    Dim memoryData As System.Data.DataSet

    Private Sub loadTable(load As Action(Of DataTable))
        memoryData = New System.Data.DataSet

        Dim table As DataTable = memoryData.Tables.Add("memoryData")

        Try
            Call Me.AdvancedDataGridView1.Rows.Clear()
        Catch ex As Exception
        End Try
        Try
            Call Me.AdvancedDataGridView1.Columns.Clear()
        Catch ex As Exception
        End Try

        Call load(table)
        Call AdvancedDataGridView1.SetDoubleBuffered()

        For Each column As DataGridViewColumn In AdvancedDataGridView1.Columns
            '    'Select Case table.Columns.Item(column.HeaderText).DataType
            '    '    Case GetType(String)
            '    '        AdvancedDataGridView1.SetSortEnabled(column, True)
            '    '    Case GetType(Double)
            '    '    Case GetType(Integer)
            '    '    Case Else
            '    '        ' do nothing 
            '    'End Select

            Try
                '    AdvancedDataGridView1.ShowMenuStrip(column)
                column.DefaultCellStyle.BackColor = table.Columns(column.HeaderText).ExtendedProperties("color")
            Catch ex As Exception

            End Try
        Next

        Call setBinding(table)
    End Sub

    Private Sub setBinding(table As DataTable)
        On Error Resume Next

        BindingSource1.Clear()
        BindingSource1.RemoveSort()

        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName

        AdvancedDataGridView1.DataSource = BindingSource1
        AdvancedDataGridViewSearchToolBar1.SetColumns(AdvancedDataGridView1.Columns)
    End Sub

    Private Function getExpression(xcms_id As String) As Dictionary(Of String, (color As String, Double()))
        Dim peak = peaks.GetById(xcms_id)
        Dim groups = sampleinfo.GroupBy(Function(s) s.sample_info).ToArray
        Dim exp = groups _
            .ToDictionary(Function(s) s.Key,
                          Function(s)
                              Dim color = s.First.color
                              Dim v = s.Select(Function(sample) peak(sample.ID)).ToArray

                              Return (color, v)
                          End Function)

        Return exp
    End Function

    Private Function plotExpression(name As String, exp As Dictionary(Of String, (color As String, Double())), Optional imageOut As String = Nothing) As Image
        Dim expVisual As New Dictionary(Of String, (color As String, Double()))(exp)

        If groupsVisual IsNot Nothing AndAlso groupsVisual.Count > 0 Then
            For Each key As String In expVisual.Keys.ToArray
                If Not key Like groupsVisual Then
                    Call expVisual.Remove(key)
                End If
            Next
        End If

        Dim factor As Double = 3
        Dim json As String = ggplotVisual.encodeJSON(expVisual)
        Dim plotType As String

        If BarPlotToolStripMenuItem.Checked Then
            plotType = "bar"
        ElseIf BoxPlotToolStripMenuItem.Checked Then
            plotType = "box"
        Else
            plotType = "violin"
        End If

        If imageOut.StringEmpty(, True) Then
            Dim plot As Image = ggplotVisual.ggplot(json,
                title:=name,
                type:=plotType,
                size:={
                    PictureBox1.Width * factor,
                    PictureBox1.Height * factor
            })

            Return plot
        Else
            Call ggplotVisual.ggplot(json,
                title:=name,
                type:=plotType,
                size:={
                    PictureBox1.Width * factor,
                    PictureBox1.Height * factor
            }, savefile:=imageOut)

            Return Nothing
        End If
    End Function

    Private Sub AdvancedDataGridView1_RowStateChanged(sender As Object, e As DataGridViewRowStateChangedEventArgs) Handles AdvancedDataGridView1.RowStateChanged
        Dim rows = AdvancedDataGridView1.SelectedRows
        Dim selected As DataGridViewRow = (From r In rows).FirstOrDefault

        If selected Is Nothing Then
            Return
        End If

        Dim xcms_id As String = any.ToString(selected.Cells(0).Value)
        Dim raw_id As String = xcms_id

        If xcms_id.StringEmpty Then
            Return
        Else
            If mapping.ContainsKey(xcms_id) Then
                xcms_id = mapping(xcms_id)
            End If
        End If

        Dim peak As xcms2 = peaks.GetById(xcms_id)

        If peak Is Nothing Then
            Return
        Else
            ' TypeDescriptor.AddAttributes(peak, New Attribute() {New ReadOnlyAttribute(True)})
            expression = getExpression(xcms_id)

            If peak.mzmin = peak.mzmax Then
                expression_name = $"[{raw_id}] {peak.mz.ToString("F4")}@{(peak.rt / 60).ToString("F2")}min"
            Else
                expression_name = $"[{raw_id}] {peak.mzmin.ToString("F4")}~{peak.mzmax.ToString("F4")}@{(peak.rt / 60).ToString("F2")}min"
            End If

            If AutoPlotToolStripMenuItem.Checked Then
                PictureBox1.BackgroundImage = plotExpression(raw_id, expression)
            End If

            PropertyGrid1.SelectedObject = peak
            PropertyGrid1.Refresh()
        End If
    End Sub

    Dim expression_name As String
    ''' <summary>
    ''' expression of current selected ion, key is the group name inside the sampleinfo
    ''' </summary>
    Dim expression As Dictionary(Of String, (color As String, Double()))

    Shared ReadOnly runPCA_evt As New RibbonEventBinding(ribbonItems.ButtonPCA)
    Shared ReadOnly runPLS_evt As New RibbonEventBinding(ribbonItems.ButtonPLSDA)
    Shared ReadOnly runOPLS_evt As New RibbonEventBinding(ribbonItems.ButtonOPLSDA)

    Shared ReadOnly viewLC_evt As New RibbonEventBinding(ribbonItems.ViewLCMSScatter)
    Shared ReadOnly openFolder_evt As New RibbonEventBinding(ribbonItems.ButtonOpenLCMSWorkspaceFolder)
    Shared ReadOnly viewSampleinfo_evt As New RibbonEventBinding(ribbonItems.ButtonViewSampleInfo)
    Shared ReadOnly viewPeaktable_evt As New RibbonEventBinding(ribbonItems.ButtonViewPeakTable)
    Shared ReadOnly view3D_evt As New RibbonEventBinding(ribbonItems.ButtonViewAnalysis3DScatter)
    Shared ReadOnly view3DPage_evt As New RibbonEventBinding(ribbonItems.ButtonViewScatter3dInSinglePage)

    Shared ReadOnly runNorm_evt As New RibbonEventBinding(ribbonItems.ButtonPreProcessing)

    Shared ReadOnly openMetabolitesFile As New RibbonEventBinding(ribbonItems.ButtonImportsLCAnnotationFromFile)
    Shared ReadOnly openMetabolitesTable As New RibbonEventBinding(ribbonItems.ButtonImportsLCAnnotationFromTable)
    Shared ReadOnly export_matrix_evt As New RibbonEventBinding(ribbonItems.ButtonExportMatrix2)

    Shared ReadOnly massFilter As New RibbonEventBinding(ribbonItems.ButtonLCMSMetabolite)
    Shared ReadOnly ionFilter As New ToggleEventBinding(ribbonItems.ButtonLCMSFilterIons)
    Shared ReadOnly setGroups As New RibbonEventBinding(ribbonItems.ButtonLCMSViewGroups)
    Shared ReadOnly volcanoPlot As New RibbonEventBinding(ribbonItems.ButtonViewVolcano)

    Private Sub frmMetabonomicsAnalysis_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call WebKit.Init(Me.WebView21)
        Call ApplyVsTheme(ContextMenuStrip1)
        Call EventActivated()
    End Sub

    Private Sub viewSampleinfo()
        Call loadTable(
            Sub(table)
                Call table.Columns.Add("ID", GetType(String))
                Call table.Columns.Add("injectionOrder", GetType(String))
                Call table.Columns.Add("batch", GetType(String))
                Call table.Columns.Add("sample_name", GetType(String))
                Call table.Columns.Add("sample_info", GetType(String))
                Call table.Columns.Add("color", GetType(String))
                Call table.Columns.Add("shape", GetType(String))

                For Each sample As SampleInfo In sampleinfo
                    Call table.Rows.Add(sample.ID, sample.injectionOrder, sample.batch, sample.sample_name, sample.sample_info, sample.color, sample.shape)
                Next
            End Sub)
    End Sub

    Private Sub openFolder()
        Call Process.Start("explorer.exe", String.Format("/n, /e, {0}", workdir))
    End Sub

    Private Sub showScatter()
        Dim ls As New InputLCMSScatter

        Call ls.SetSamples(sampleinfo)
        Call InputDialog.Input(Of InputLCMSScatter)(
            Sub(config)
                Dim samples As String()
                Dim label As String

                If config.PlotSampleGroup Then
                    samples = config.GetCurrentSamples.ToArray
                    label = "sample group: " & config.PlotSource
                Else
                    samples = {config.PlotSource}
                    label = "sample: " & config.PlotSource
                End If

                Call VisualStudio _
                    .ShowDocument(Of frmLCMSScatterViewer)(title:=label) _
                    .LoadRaw(peaks.peaks, samples)
            End Sub, config:=ls)
    End Sub

    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class ImageUrl

        Public url As String

        Public Function Download() As String
            Return url
        End Function

    End Class

    Dim url As New ImageUrl
    Dim source As New frm3DScatterPlotView.DataSource

    Private Sub SetSvg(svgfile As String, scatter As UMAPPoint())
        Dim fileName As String = svgfile.FileName
        Dim dirname As String = svgfile.ParentDirName
        Dim url As String = $"http://127.0.0.1:{webPort}/{dirname}/{fileName}"
        Dim js As String = $"apps.viewer.svgViewer.setSvgUrl(""{url}"");"

        Me.source.points = scatter
        Me.url.url = url
        Me.WebView21.ExecuteScriptAsync(js)

        Call viewScatterSVG()
    End Sub

    Private Sub view3DScatterInSinglePage()
        If Not source.points Is Nothing Then
            Call VisualStudio.ShowDocument(Of frm3DScatterPlotView)() _
                .LoadScatter(
                    data:=source.points,
                    onclick:=Sub(id)
                                 ' do nothing
                             End Sub)
        End If
    End Sub

    Private Sub view3DScatter()
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", source)
        Call WebView21.CoreWebView2.Navigate($"http://127.0.0.1:{Workbench.WebPort}/3d-scatter.html")
        Call WebKit.DeveloperOptions(WebView21, enable:=True,)
    End Sub

    Private Sub viewScatterSVG() Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", url)
        Call WebView21.CoreWebView2.Navigate($"http://127.0.0.1:{Workbench.WebPort}/svgViewer.html")
        Call WebKit.DeveloperOptions(WebView21, enable:=True,)
    End Sub

    Private Sub EventActivated() Handles Me.Activated
        ribbonItems.MetaboAnalysis.ContextAvailable = ContextAvailability.Available

        runPCA_evt.evt = Sub() Call RunPCA(GetType(PCA))
        runPLS_evt.evt = Sub() Call RunPCA(GetType(PLS))
        runOPLS_evt.evt = Sub() Call RunPCA(GetType(OPLS))

        runNorm_evt.evt = Sub() Call RunNormaliza()

        openMetabolitesFile.evt = Sub() Call importsMetaboliteFile()
        openMetabolitesTable.evt = Sub() Call importsMetaboliteTable()

        viewLC_evt.evt = Sub() Call showScatter()
        openFolder_evt.evt = Sub() Call openFolder()
        viewSampleinfo_evt.evt = Sub() Call viewSampleinfo()
        viewPeaktable_evt.evt = AddressOf loadPeaktable
        view3D_evt.evt = Sub() Call view3DScatter()
        view3DPage_evt.evt = Sub() Call view3DScatterInSinglePage()

        export_matrix_evt.evt = Sub() Call exportMatrixExcelFile()
        massFilter.evt = Sub() Call MassSearch()
        ionFilter.evt = Sub() Call loadPeaktable()
        setGroups.evt = Sub() Call SetGroupsVisual()
        volcanoPlot.evt = Sub() Call doVolcanoPlot()
    End Sub

    Private Sub doVolcanoPlot()
        Dim config As New InputVolcanoSettings

        Call config.SetGroups(sampleinfo.Select(Function(i) i.sample_info).Distinct)
        Call InputDialog.Input(
            Sub(settings)
                Dim dir As String = workdir & $"/{settings.Trial} vs {settings.ControlGroup}"

                If RscriptProgressTask.RunVolcano(matrixfile, sampleinfofile,
                                                  settings.Trial, settings.ControlGroup, settings.log2fc, settings.pvalue,
                                                  dir) Then

                    Dim score_data As EntityObject() = EntityObject.LoadDataSet($"{dir}/ttest_diffsig.csv").ToArray

                    Call loadTable(Sub(table) Call LoadAnalysisTable(table, score_data))
                    Call SetSvg($"{dir}/volcano.png", {})
                End If
            End Sub, config:=config)
    End Sub

    Dim groupsVisual As Index(Of String)

    Private Sub SetGroupsVisual()
        Dim cfg As New InputSetGroupVisual

        Call cfg.SetGroupLabels(sampleinfo.SafeQuery.Select(Function(si) si.sample_info).Distinct)
        Call InputDialog.Input(
            Sub(groups)
                groupsVisual = groups.GetGroupNames.Indexing
            End Sub, config:=cfg)
    End Sub

    Private Sub importsMetaboliteTable()
        Dim tables = Workbench.AppHost.DockPanel.Documents.Where(Function(doc) TypeOf doc Is frmTableViewer).ToArray
        Dim names = tables.Select(Function(tab) DirectCast(tab, frmTableViewer).TabText).Indexing

        Call SelectSheetName.SelectName(names.Objects,
             Sub(name)
                 Dim i As Integer = names.IndexOf(name)

                 If i > -1 Then
                     Dim tab As frmTableViewer = tables(i)
                     Dim df As DataFrameResolver = tab.AdvancedDataGridView1.GetDataFrame

                     Call importsMetaboliteCommon(df)
                 End If
             End Sub)
    End Sub

    Private Sub MassSearch()
        Dim selector As New InputPubChemProxy

        Call selector.SetIonMassFilter()
        Call InputDialog.Input(Of InputPubChemProxy)(
            Sub(cfg)
                Dim metadata = cfg.GetAnnotation
                Dim mzdiff = cfg.GetTolerance
                Dim ionMode As IonModes = cfg.IonMode
                Dim adducts As MzCalculator()

                If ionMode = IonModes.Positive Then
                    adducts = Provider.Positives
                Else
                    adducts = Provider.Negatives
                End If

                Call annotation.Clear()

                For Each type As MzCalculator In adducts
                    Dim mzi As Double = type.CalcMZ(FormulaScanner.EvaluateExactMass(metadata.formula))
                    Dim peakMatches = peaks _
                        .FilterMz(mzi, 0.005) _
                        .ToArray

                    For Each peak As xcms2 In peakMatches
                        annotation(peak.ID) = New AnnotatedIon With {
                            .AdductIon = New AdductIon(type),
                            .metadata = New MetaboliteAnnotation With {
                                .CommonName = metadata.name,
                                .Formula = metadata.formula,
                                .ExactMass = FormulaScanner.EvaluateExactMass(.Formula),
                                .Id = $"{ .CommonName}_{type.ToString}@{(peak.rt / 60).ToString("F1")}min"
                            }
                        }
                    Next
                Next

                Call loadPeaktable()
            End Sub, config:=selector)
    End Sub

    Private Sub importsMetaboliteCommon(df As DataFrameResolver)
        Dim xcms_id As Integer = df.GetOrdinal("xcms_id", "ID")
        Dim name As Integer = df.GetOrdinal("name")
        Dim formula As Integer = df.GetOrdinal("formula")
        Dim adducts As Integer = df.GetOrdinal("precursor_type", "precursor", "adducts")
        Dim xcms_idset As String() = df.GetColumnValues("xcms_id").SafeQuery.ToArray

        If xcms_idset.IsNullOrEmpty OrElse peaks.peaks.Select(Function(p) p.ID).Intersect(xcms_idset).Count = 0 Then
            ' matches by mz and rt
            Dim mz As Integer = df.GetOrdinal("mz", "m/z")
            Dim rt As Integer = df.GetOrdinal("rt", "RT")

            Do While df.Read
                ' find xcms id with min tolerance error
                Dim mzi As Double = df.GetDouble(mz)
                Dim peak = peaks _
                    .FilterMz(mzi, 0.01) _
                    .OrderBy(Function(p)
                                 If rt > -1 Then
                                     Return std.Abs(df.GetDouble(rt) - p.rt)
                                 Else
                                     Return std.Abs(mzi - p.mz)
                                 End If
                             End Function) _
                    .FirstOrDefault

                If Not peak Is Nothing Then
                    annotation(peak.ID) = New AnnotatedIon With {
                        .AdductIon = New AdductIon(Provider.ParseAdductModel(df.GetString(adducts))),
                        .metadata = New MetaboliteAnnotation With {
                            .CommonName = df.GetString(name),
                            .Formula = df.GetString(formula),
                            .ExactMass = FormulaScanner.EvaluateExactMass(.Formula),
                            .Id = peak.ID
                        }
                    }
                End If
            Loop
        Else
            ' matches by unique ion id
            Do While df.Read
                annotation(df.GetString(xcms_id)) = New AnnotatedIon With {
                    .AdductIon = New AdductIon(Provider.ParseAdductModel(df.GetString(adducts))),
                    .metadata = New MetaboliteAnnotation With {
                        .CommonName = df.GetString(name),
                        .Formula = df.GetString(formula),
                        .ExactMass = FormulaScanner.EvaluateExactMass(.Formula),
                        .Id = df.GetString(xcms_id)
                    }
                }
            Loop
        End If

        Call loadPeaktable()
    End Sub

    Private Sub importsMetaboliteFile()
        Using file As New OpenFileDialog With {.Filter = "Excel Table(*.csv)|*.csv"}
            If file.ShowDialog = DialogResult.OK Then
                Call importsMetaboliteCommon(df:=DataFrameResolver.Load(file.FileName))
            End If
        End Using
    End Sub

    Private Sub RunNormaliza()
        InputDialog.Input(Of InputSampleProcessing)(
            Sub(cfg)
                If RscriptProgressTask.RunPreprocessing($"{workdir}/peakset.xcms", sampleinfofile, cfg.MissingPercentage, cfg.NormScale,
                                                        $"{workdir}/norm.xcms") Then
                    ' reload
                    Call LoadWorkspace(workdir)
                Else
                    Call Workbench.Warning("run data pre-processing error.")
                End If
            End Sub)
    End Sub

    Private Sub EventDeactivate() Handles Me.Deactivate
        ribbonItems.MetaboAnalysis.ContextAvailable = ContextAvailability.NotAvailable

        runPCA_evt.evt = Nothing
        runPLS_evt.evt = Nothing
        runOPLS_evt.evt = Nothing

        runNorm_evt.evt = Nothing

        viewLC_evt.evt = Nothing
        openFolder_evt.evt = Nothing
        viewSampleinfo_evt.evt = Nothing
        view3D_evt.evt = Nothing
        view3DPage_evt.evt = Nothing

        viewPeaktable_evt.evt = Nothing
        export_matrix_evt.evt = Nothing
        massFilter.evt = Nothing
        ionFilter.evt = Nothing
        setGroups.evt = Nothing
        volcanoPlot.evt = Nothing

        openMetabolitesFile.evt = Nothing
        openMetabolitesTable.evt = Nothing
    End Sub

    Private Sub RunPCA(analysis As Type)
        If sampleinfo.IsNullOrEmpty Then
            Call Workbench.Warning("Please load sample peaktable data at first!")
            Return
        End If

        InputDialog.Input(
            Sub(config)
                RscriptProgressTask.RunComponentTask(matrixfile, sampleinfofile, config.ncomp, config.showSampleLable, analysis)

                Select Case analysis
                    Case GetType(PLS) : Call PLSDAToolStripMenuItem_Click()
                    Case GetType(OPLS) : Call OPLSDAToolStripMenuItem_Click()
                    Case Else
                        Call PCAToolStripMenuItem_Click()
                End Select
            End Sub, config:=New InputPCADialog().SetMaxComponent(sampleinfo.Length))
    End Sub

    Private Sub loadPeakTableCommon()
        Call ToolStripDropDownButton1.DropDownItems.Clear()
        Call ToolStripDropDownButton1.DropDownItems.Add("view peaktable", My.Resources._42082, Sub() Call loadPeaktable())
    End Sub

    Private Sub loadPeaktable()
        Call loadTable(Sub(table) Call LoadSampleData(table))
    End Sub

    Private Sub PCAToolStripMenuItem_Click() Handles PCAToolStripMenuItem.Click
        Dim dir As String = $"{workdir}/pca"
        Dim score As String = $"{dir}/pca_score.csv"
        Dim loading As String = $"{dir}/pca_loading.csv"
        Dim sampleIndex = sampleinfo.ToDictionary(Function(a) a.ID)

        Call loadPeakTableCommon()

        PLSDAToolStripMenuItem.Checked = False
        OPLSDAToolStripMenuItem.Checked = False

        ToolStripDropDownButton1.DropDownItems.Add("pca_score", My.Resources._1200px_Checked_svg,
            Sub()
                Dim score_data As DataSet() = DataSet.LoadDataSet(score).ToArray
                Dim scatter As UMAPPoint() = score_data _
                    .Select(Function(d)
                                Return New UMAPPoint(d.ID, d!PC1, d!PC2, d!PC3) With {
                                    .[class] = sampleIndex(d.ID).sample_info
                                }
                            End Function) _
                    .ToArray

                Call loadTable(Sub(table) Call LoadAnalysisTable(table, score_data))
                Call SetSvg($"{dir}/pca_score.png", scatter)
            End Sub).PerformClick()
        ToolStripDropDownButton1.DropDownItems.Add("pca_loading", My.Resources._1200px_Checked_svg,
            Sub()
                Dim score_data As DataSet() = DataSet.LoadDataSet(loading).ToArray
                Dim scatter = score_data _
                    .Select(Function(d)
                                Return New UMAPPoint(d.ID, d!PC1, d!PC2, d!PC3) With {.[class] = "metabolite"}
                            End Function) _
                    .ToArray

                Call loadTable(Sub(table) Call LoadAnalysisTable(table, score_data))
                Call SetSvg($"{dir}/pca_loading.png", scatter)
            End Sub)
    End Sub

    Private Sub PLSDAToolStripMenuItem_Click() Handles PLSDAToolStripMenuItem.Click
        Dim dir As String = $"{workdir}/plsda"
        Dim score As String = $"{dir}/plsda_scoreMN.csv"
        Dim loading As String = $"{dir}/plsda_loadingMN.csv"

        Call loadPeakTableCommon()

        PCAToolStripMenuItem.Checked = False
        OPLSDAToolStripMenuItem.Checked = False

        ToolStripDropDownButton1.DropDownItems.Add("plsda_score", My.Resources._1200px_Checked_svg,
            Sub()
                Dim score_data As DataSet() = DataSet.LoadDataSet(score).ToArray
                Dim scatter = score_data.Select(Function(d) New UMAPPoint(d.ID, d!T1, d!T2, d!T3) With {.[class] = d.ID}).ToArray

                Call loadTable(Sub(table) Call LoadAnalysisTable(table, score_data))
                Call SetSvg($"{dir}/plsda_scoreMN.png", scatter)
            End Sub).PerformClick()
        ToolStripDropDownButton1.DropDownItems.Add("plsda_loading", My.Resources._1200px_Checked_svg,
            Sub()
                Dim score_data As DataSet() = DataSet.LoadDataSet(loading).ToArray
                Dim scatter = score_data.Select(Function(d) New UMAPPoint(d.ID, d!P1, d!P2, d!P3) With {.[class] = "metabolite"}).ToArray

                Call loadTable(Sub(table) Call LoadAnalysisTable(table, score_data))
                Call SetSvg($"{dir}/plsda_loadingMN.png", scatter)
            End Sub)
    End Sub

    Private Sub OPLSDAToolStripMenuItem_Click() Handles OPLSDAToolStripMenuItem.Click
        Dim dir As String = $"{workdir}/opls"
        Dim score As String = $"{dir}/oplsda_scoreMN.csv"
        Dim loading As String = $"{dir}/oplsda_loadingMN.csv"

        Call loadPeakTableCommon()

        PCAToolStripMenuItem.Checked = False
        PLSDAToolStripMenuItem.Checked = False

        ToolStripDropDownButton1.DropDownItems.Add("oplsda_score", My.Resources._1200px_Checked_svg,
            Sub()
                Dim score_data As DataSet() = DataSet.LoadDataSet(score).ToArray
                Dim scatter = score_data.Select(Function(d) New UMAPPoint(d.ID, d!T1, d!T2, d!T3) With {.[class] = d.ID}).ToArray

                Call loadTable(Sub(table) Call LoadAnalysisTable(table, score_data))
                Call SetSvg($"{dir}/oplsda_scoreMN.png", scatter)
            End Sub).PerformClick()
        ToolStripDropDownButton1.DropDownItems.Add("oplsda_loading", My.Resources._1200px_Checked_svg,
            Sub()
                Dim score_data As DataSet() = DataSet.LoadDataSet(loading).ToArray
                Dim scatter = score_data.Select(Function(d) New UMAPPoint(d.ID, d!P1, d!P2, d!P3) With {.[class] = "metabolite"}).ToArray

                Call loadTable(Sub(table) Call LoadAnalysisTable(table, score_data))
                Call SetSvg($"{dir}/oplsda_loadingMN.png", scatter)
            End Sub)
    End Sub

    Private Sub frmMetabonomicsAnalysis_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Call EventDeactivate()

        If Not imageWeb Is Nothing Then
            Try
                Call imageWeb.Kill()
            Catch ex As Exception

            End Try
        End If
    End Sub

    Private Sub BoxPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BoxPlotToolStripMenuItem.Click
        BarPlotToolStripMenuItem.Checked = False
        ViolinPlotToolStripMenuItem.Checked = False

        If Not expression Is Nothing Then
            PictureBox1.BackgroundImage = plotExpression(expression_name, expression)
        End If
    End Sub

    Private Sub BarPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles BarPlotToolStripMenuItem.Click
        BoxPlotToolStripMenuItem.Checked = False
        ViolinPlotToolStripMenuItem.Checked = False

        If Not expression Is Nothing Then
            PictureBox1.BackgroundImage = plotExpression(expression_name, expression)
        End If
    End Sub

    Private Sub ViolinPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViolinPlotToolStripMenuItem.Click
        BoxPlotToolStripMenuItem.Checked = False
        BarPlotToolStripMenuItem.Checked = False

        If Not expression Is Nothing Then
            PictureBox1.BackgroundImage = plotExpression(expression_name, expression)
        End If
    End Sub

    Private Sub ViewExpressionPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewExpressionPlotToolStripMenuItem.Click
        If Not expression Is Nothing Then
            PictureBox1.BackgroundImage = plotExpression(expression_name, expression)
        End If
    End Sub

    Private Sub AutoPlotToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AutoPlotToolStripMenuItem.Click
        If AutoPlotToolStripMenuItem.Checked Then
            If Not expression Is Nothing Then
                PictureBox1.BackgroundImage = plotExpression(expression_name, expression)
            Else

            End If
        End If
    End Sub

    Private Sub OpenInTableEditorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenInTableEditorToolStripMenuItem.Click
        Call VisualStudio.ShowDocument(Of frmTableViewer)(, "View analysis result table").LoadTable(AddressOf loadResultTable)
    End Sub

    Private Sub loadResultTable(tbl As DataTable)
        Dim peaks = Me.peaks.peaks.ToDictionary(Function(a) a.ID)
        Dim ncols = AdvancedDataGridView1.Columns.Count
        Dim rowdata As Object() = New Object(ncols - 1) {}
        Dim addMz As Boolean = CLRVector.asCharacter(AdvancedDataGridView1.getFieldVector(0)) _
            .SafeQuery _
            .All(Function(id)
                     Return peaks.ContainsKey(id)
                 End Function)
        Dim addMzOffsetOne As Boolean = addMz

        If addMz Then
            rowdata = New Object(rowdata.Length) {}
        End If

        For Each col As DataGridViewColumn In AdvancedDataGridView1.Columns
            Call tbl.Columns.Add(col.HeaderText, col.ValueType)

            If addMz Then
                addMz = False
                tbl.Columns.Add("m/z", GetType(Double))
            End If
        Next

        For Each row As DataGridViewRow In AdvancedDataGridView1.Rows
            rowdata = New Object(rowdata.Length - 1) {}

            If addMzOffsetOne Then
                rowdata(1) = peaks(CStr(row.Cells(0).Value)).mz
            End If

            For i As Integer = 0 To ncols - 1
                If i > 0 AndAlso addMzOffsetOne Then
                    rowdata(i + 1) = row.Cells(i).Value
                Else
                    rowdata(i) = row.Cells(i).Value
                End If
            Next

            Call tbl.Rows.Add(rowdata)
        Next
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub PictureBox1_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox1.DoubleClick
        If PictureBox1.BackgroundImage IsNot Nothing Then
            Dim tempfile = App.CurrentProcessTemp & $"/{App.NextTempName}.png"
            Dim process As New Process()

            PictureBox1.BackgroundImage.SaveAs(tempfile)

            ' 设置进程启动信息
            process.StartInfo.FileName = tempfile
            process.StartInfo.UseShellExecute = True
            process.StartInfo.ErrorDialog = True

            ' 启动进程
            Try
                Call process.Start()
            Catch ex As Exception
                Call App.LogException(ex)
                Call Workbench.Warning(ex.ToString)
            End Try
        End If
    End Sub

    Private Sub CopyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToolStripMenuItem.Click
        Clipboard.Clear()
        Clipboard.SetImage(PictureBox1.BackgroundImage)
    End Sub

    Private Sub SaveImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveImageToolStripMenuItem.Click
        If expression Is Nothing Then
            Workbench.Warning("no expression data to make plot!")
            Return
        End If

        Using File As New SaveFileDialog With {
            .Filter = "Image file(*.png)|*.png|Svg image file(*.svg)|*.svg|Pdf image file(*.pdf)|*.pdf"
        }
            If File.ShowDialog = DialogResult.OK Then
                Call plotExpression(expression_name, expression, File.FileName)

                Dim process As New Process()

                ' 设置进程启动信息
                process.StartInfo.FileName = File.FileName
                process.StartInfo.UseShellExecute = True
                process.StartInfo.ErrorDialog = True

                ' 启动进程
                Try
                    Call process.Start()
                Catch ex As Exception
                    Call App.LogException(ex)
                    Call Workbench.Warning(ex.ToString)
                End Try
            End If
        End Using
    End Sub
End Class