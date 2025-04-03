Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive.SingleCells
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.Pixel
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports SMRUCC.genomics.Analysis.SingleCell
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmSingleCellViewer

    Public ReadOnly Property SourceUrl As String
        Get
            Return $"http://127.0.0.1:{Workbench.WebPort}/single_cells.html"
        End Get
    End Property

    Public source As SingleCellViewer
    Public dataIndex As Dictionary(Of String, ScanMS1)

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        ' 在 InitializeComponent() 调用之后添加任何初始化。
        ' 在 InitializeComponent() 调用之后添加任何初始化。
        AutoScaleMode = AutoScaleMode.Dpi
        DockAreas = DockAreas.Document Or DockAreas.Float
        TabText = "Loading WebView2 App..."
    End Sub

    Private Sub frmUMAPTools_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "Single Cells Tool"
        TabText = Text

        WebKit.Init(WebView21)

        AddHandler ribbonItems.ButtonViewSingleCellsEmbedding.ExecuteEvent, Sub() Call ViewEmbeddingTable()
    End Sub

    Private Sub ViewEmbeddingTable()
        Dim embedding As UMAPPoint() = SingleCellScatter1.GetEmbedding.ToArray
        Dim table = VisualStudio.ShowDocument(Of frmTableViewer)(DockState.Document, title:="Single Cells Embedding")

        Call table.LoadTable(
            Sub(tbl)
                Call tbl.Columns.Add("label", GetType(String))
                Call tbl.Columns.Add("class_id", GetType(String))
                Call tbl.Columns.Add("umap1", GetType(Double))
                Call tbl.Columns.Add("umap2", GetType(Double))
                Call tbl.Columns.Add("umap3", GetType(Double))

                For Each scatter As UMAPPoint In embedding
                    Call tbl.Rows.Add(scatter.label, scatter.class, scatter.x, scatter.y, scatter.z)
                Next
            End Sub)
    End Sub

    Public Sub DoRender()
        Call SingleCellScatter1.SetRender(WindowModules.singleCellsParameters.args)
    End Sub

    Public Sub DoRenderExpression(mz As Double)
        Dim tol As Tolerance = Tolerance.DeltaMass(0.01)
        Dim scatter As SingleExpression() = dataIndex.Values _
            .AsParallel _
            .Select(Function(s)
                        Return s.ResolveSingleExpression(mz, tol)
                    End Function) _
            .ToArray

        WindowModules.singleCellsParameters.args.SetHeatmapMode(True)

        SingleCellScatter1.LoadCells(scatter)
        SingleCellScatter1.SetRender(WindowModules.singleCellsParameters.args)
    End Sub

    ''' <summary>
    ''' load umap data into viewer context
    ''' </summary>
    ''' <param name="file"></param>
    Public Sub LoadMzkitRawdata(file As mzPack)
        Dim singleCells As UMAPPoint() = file.ResolveSingleCells.ToArray

        dataIndex = file.MS _
            .GroupBy(Function(c) c.scan_id) _
            .ToDictionary(Function(c) c.Key,
                          Function(c)
                              Return c.First
                          End Function)

        WindowModules.singleCellsParameters.SetSingleCells(singleCells)
        WindowModules.singleCellsParameters.args.SetHeatmapMode(False)

        SingleCellScatter1.LoadCells(singleCells)
        SingleCellScatter1.SetRender(WindowModules.singleCellsParameters.args)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="filepath"></param>
    Public Sub LoadH5adRawdata(filepath As String)
        Dim cache_key As String = frmMsImagingViewer.getCacheKey(filepath)
        Dim cachefile As String = $"{App.AppSystemTemp}/.matrix_cache/{cache_key}.dat"

        If Not cachefile.FileExists Then
            ' export matrix to local cache
            Call TaskProgress.RunAction(
                run:=Sub(p As ITaskProgress)

                     End Sub,
                title:="Create matrix data...",
                info:="Export and save matrix data..."
            )
        End If

        source = New SingleCellViewer With {.matrix = cachefile}

        Call WebView21_CoreWebView2InitializationCompleted()
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted()
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", Me.source)
        Call WebView21.CoreWebView2.Navigate(SourceUrl)
        Call WebKit.DeveloperOptions(WebView21, enable:=True,)
    End Sub

    ''' <summary>
    ''' open external link in default webbrowser
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub WebView21_NavigationStarting(sender As Object, e As CoreWebView2NavigationStartingEventArgs) Handles WebView21.NavigationStarting
        Dim url As New URL(e.Uri)

        If url.hostName <> "127.0.0.1" AndAlso url.hostName <> "localhost" Then
            e.Cancel = True
            Process.Start(e.Uri)
        End If
    End Sub

    Private Sub frmSingleCellViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        e.Cancel = True
        Me.DockState = DockState.Hidden
    End Sub

    Private Sub SingleCellScatter1_SelectCell(cell_id As String, umap As UMAPPoint) Handles SingleCellScatter1.SelectCell
        Dim pixel As ScanMS1 = dataIndex.TryGetValue(cell_id)
        Dim info As PixelProperty = Nothing

        If pixel Is Nothing Then
            Call Workbench.Warning($"UMAP space location [{umap.x}, {umap.y}] not contains any data.")
            Call WindowModules.MSIPixelProperty.SetPixel(New InMemoryPixel(0, 0, {}), info)
            Call SingleCellScatter1.ShowMessage($"UMAP space location [{umap.x}, {umap.y}] not contains any data.")

            Return
        Else
            If WindowModules.MSIPixelProperty.DockState = DockState.Hidden Then
                WindowModules.MSIPixelProperty.DockState = DockState.DockRight
            End If

            Call SpectralViewerModule.ViewSpectral(pixel)
            Call WindowModules.MSIPixelProperty.SetSingleCell(pixel, info)
            Call Workbench.StatusMessage($"Select {pixel.scan_id}, totalIons: {info.TotalIon.ToString("G3")}, basePeak m/z: {info.TopIonMz.ToString("F4")}")
        End If
    End Sub
End Class

' 所有需要在JavaScript环境中暴露的对象
' 都需要标记上下面的两个自定义属性
<ClassInterface(ClassInterfaceType.AutoDual)>
<ComVisible(True)>
Public Class SingleCellViewer

    ''' <summary>
    ''' the source data to view
    ''' </summary>
    Public matrix As String

End Class