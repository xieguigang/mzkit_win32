Imports System.IO
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.BioDeep.Chemistry.NCBI.PubChem
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Math.Statistics.Hypothesis.ANOVA
Imports Microsoft.VisualBasic.My.JavaScript
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports RibbonLib.Interop
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports TaskStream
Imports any = Microsoft.VisualBasic.Scripting
Imports csv = Microsoft.VisualBasic.Data.csv.IO.File

Public Class frmMetabonomicsAnalysis

    Dim sampleinfo As SampleInfo()
    Dim properties As String()
    Dim peaks As PeakSet

    ''' <summary>
    ''' could be custom in the wizard dialog
    ''' </summary>
    Friend workdir As String = getTemp()

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

    Public Sub LoadSampleData(table As DataTable)
        Dim groups = sampleinfo.GroupBy(Function(s) s.sample_info) _
            .Select(Function(g) (g.Key, list:=g.ToArray)) _
            .ToArray

        Call table.Columns.Add("xcms_id", GetType(String))

        For Each group In groups
            Dim col = table.Columns.Add(group.Key, GetType(Double))
            col.ExtendedProperties.Add("color", group.list.First.color.TranslateColor)
        Next

        For Each peak As xcms2 In peaks.peaks
            Dim row As Object() = New Object(groups.Length) {}
            row(0) = peak.ID

            For i As Integer = 0 To groups.Length - 1
                Dim group = groups(i).list
                Dim data As Double() = peak(group.Select(Function(s) s.ID))
                row(i + 1) = data.Average
            Next

            Call table.Rows.Add(row)
        Next
    End Sub

    Public Sub LoadAnalysisTable(table As DataTable, data As DataSet())
        Dim keys As String() = data(0).Properties.Keys.ToArray

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

    Public Delegate Sub LoadDataCallback(sampleinfo As SampleInfo(), properties As String(), df As DataFrame, workdir As String)

    Public Shared Sub LoadData(table As csv, callback As LoadDataCallback)
        Dim wizard As New InputImportsPeaktableDialog
        Dim df As DataFrame = DataFrame.CreateObject(table)

        Call wizard.LoadSampleId(df.HeadTitles)
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

    Public Sub LoadData(sampleinfo As SampleInfo(), properties As String(), df As DataFrame, workdir As String, title As String)
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
                .mzmax = row.TryPopOut({"mzmax", "mz.max"}),
                .mzmin = row.TryPopOut({"mzmin", "mz.min"}),
                .rt = row.TryPopOut({"rt", "RT", "retention_time"}),
                .rtmax = row.TryPopOut({"rtmax", "rt.max"}),
                .rtmin = row.TryPopOut({"rtmin", "rt.min"}),
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
            .peaks = peaks.ToArray
        }

        Call loadTable(Sub(tbl) Call LoadSampleData(tbl))

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

    Public Sub LoadWorkspace(dir As String)
        Me.workdir = dir
        Me.peaks = SaveXcms.ReadSample($"{dir}/peakset.xcms".Open(FileMode.Open, doClear:=False, [readOnly]:=True))
        Me.sampleinfo = sampleinfofile.LoadCsv(Of SampleInfo)().ToArray

        Using f As Stream = matrixfile.Open(FileMode.OpenOrCreate, doClear:=True)
            Call CastMatrix(Me.peaks, sampleinfo).Save(f)
            Call Workbench.LogText($"set workspace for metabonomics workbench: {workdir}")
        End Using

        Call loadTable(Sub(table) Call LoadSampleData(table))
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

        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName

        AdvancedDataGridView1.DataSource = BindingSource1
        AdvancedDataGridViewSearchToolBar1.SetColumns(AdvancedDataGridView1.Columns)
    End Sub

    Private Sub AdvancedDataGridView1_RowStateChanged(sender As Object, e As DataGridViewRowStateChangedEventArgs) Handles AdvancedDataGridView1.RowStateChanged
        Dim rows = AdvancedDataGridView1.SelectedRows
        Dim selected As DataGridViewRow = (From r In rows).FirstOrDefault

        If selected Is Nothing Then
            Return
        End If

        Dim xcms_id As String = any.ToString(selected.Cells(0).Value)

        If xcms_id.StringEmpty Then
            Return
        End If

        Dim peak = peaks.GetById(xcms_id)

        If peak Is Nothing Then
            Return
        Else
            ' TypeDescriptor.AddAttributes(peak, New Attribute() {New ReadOnlyAttribute(True)})

            PropertyGrid1.SelectedObject = peak
            PropertyGrid1.Refresh()
        End If
    End Sub

    Private Sub frmMetabonomicsAnalysis_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call WebKit.Init(Me.WebView21)

        ribbonItems.MetaboAnalysis.ContextAvailable = ContextAvailability.Available

        AddHandler ribbonItems.ButtonPCA.ExecuteEvent, Sub() Call RunPCA(GetType(PCA))
        AddHandler ribbonItems.ButtonPLSDA.ExecuteEvent, Sub() Call RunPCA(GetType(PLS))
        AddHandler ribbonItems.ButtonOPLSDA.ExecuteEvent, Sub() Call RunPCA(GetType(OPLS))
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

    Private Sub SetSvg(svgfile As String)
        Dim url As DataURI = DataURI.SVGImage(Strings.Trim(svgfile.ReadAllText))
        Dim uri As String = url.ToString
        Dim js As String = $"apps.viewer.svgViewer.setSvgUrl(""{uri}"")"

        Me.url.url = svgfile
        WebView21.ExecuteScriptAsync(js).Wait()
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", url)
        Call WebView21.CoreWebView2.Navigate($"http://127.0.0.1:{Workbench.WebPort}/svgViewer.html")
        Call WebKit.DeveloperOptions(WebView21, enable:=True,)
    End Sub

    Private Sub frmMetabonomicsAnalysis_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        ribbonItems.MetaboAnalysis.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub frmMetabonomicsAnalysis_LostFocus(sender As Object, e As EventArgs) Handles Me.LostFocus

    End Sub

    Private Sub frmMetabonomicsAnalysis_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        ribbonItems.MetaboAnalysis.ContextAvailable = ContextAvailability.NotAvailable
    End Sub

    Private Sub RunPCA(analysis As Type)
        If sampleinfo.IsNullOrEmpty Then
            Call Workbench.Warning("Please load sample peaktable data at first!")
            Return
        End If

        InputDialog.Input(
            Sub(config)
                RscriptProgressTask.RunComponentTask(matrixfile, sampleinfofile, config.ncomp, analysis)

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
        Call ToolStripDropDownButton1.DropDownItems.Add("view peaktable", My.Resources._42082,
             Sub()
                 Call loadTable(Sub(table) Call LoadSampleData(table))
             End Sub)
    End Sub

    Private Sub PCAToolStripMenuItem_Click() Handles PCAToolStripMenuItem.Click
        Dim dir As String = $"{workdir}/pca"
        Dim score As String = $"{dir}/pca_score.csv"
        Dim loading As String = $"{dir}/pca_loading.csv"

        Call loadPeakTableCommon()

        PLSDAToolStripMenuItem.Checked = False
        OPLSDAToolStripMenuItem.Checked = False

        ToolStripDropDownButton1.DropDownItems.Add("pca_score", My.Resources._1200px_Checked_svg,
            Sub()
                Dim score_data As DataSet() = DataSet.LoadDataSet(score).ToArray

                Call loadTable(Sub(table) Call LoadAnalysisTable(table, score_data))
                Call SetSvg($"{dir}/pca_score.svg")
            End Sub).PerformClick()
        ToolStripDropDownButton1.DropDownItems.Add("pca_loading", My.Resources._1200px_Checked_svg,
            Sub()
                Dim score_data As DataSet() = DataSet.LoadDataSet(loading).ToArray

                Call loadTable(Sub(table) Call LoadAnalysisTable(table, score_data))
                Call SetSvg($"{dir}/pca_loading.svg")
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

                Call loadTable(Sub(table) Call LoadAnalysisTable(table, score_data))
                Call SetSvg($"{dir}/plsda_scoreMN.svg")
            End Sub).PerformClick()
        ToolStripDropDownButton1.DropDownItems.Add("plsda_loading", My.Resources._1200px_Checked_svg,
            Sub()
                Dim score_data As DataSet() = DataSet.LoadDataSet(loading).ToArray

                Call loadTable(Sub(table) Call LoadAnalysisTable(table, score_data))
                Call SetSvg($"{dir}/plsda_loadingMN.svg")
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

                Call loadTable(Sub(table) Call LoadAnalysisTable(table, score_data))
                Call SetSvg($"{dir}/oplsda_scoreMN.svg")
            End Sub).PerformClick()
        ToolStripDropDownButton1.DropDownItems.Add("oplsda_loading", My.Resources._1200px_Checked_svg,
            Sub()
                Dim score_data As DataSet() = DataSet.LoadDataSet(loading).ToArray

                Call loadTable(Sub(table) Call LoadAnalysisTable(table, score_data))
                Call SetSvg($"{dir}/oplsda_loadingMN.svg")
            End Sub)
    End Sub
End Class