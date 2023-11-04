Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports TaskStream
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmUMAPTools

    Public ReadOnly Property SourceUrl As String
        Get
            Return $"http://127.0.0.1:{Workbench.WebPort}/umap.html"
        End Get
    End Property

    Public source As UMApAnalysis

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
        Text = "UMAP Tool"
        TabText = Text

        WebKit.Init(WebView21)
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
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
End Class

' 所有需要在JavaScript环境中暴露的对象
' 都需要标记上下面的两个自定义属性
<ClassInterface(ClassInterfaceType.AutoDual)>
<ComVisible(True)>
Public Class UMApAnalysis

    Public Property matrix As String
    ''' <summary>
    ''' GCModeller HTS binary matrix or csv ascii text file?
    ''' </summary>
    ''' <returns></returns>
    Public Property binaryMatrix As Boolean = False
    Public Property matrix_dims As Integer()

    ''' <summary>
    ''' apply the umap analysis result
    ''' </summary>
    ''' <returns></returns>
    Public Property callback As Action(Of String)
    ''' <summary>
    ''' click on the umap scatter point
    ''' </summary>
    ''' <returns></returns>
    Public Property onclick As Action(Of String)

    ''' <summary>
    ''' the csv file path of the umap analysis result
    ''' </summary>
    ''' <returns></returns>
    Public Property umap_result As String

    Public Function GetUMAPFile() As String
        If umap_result.StringEmpty Then
            Return $"{matrix.ParentPath}/{matrix.BaseName}_umap3.csv"
        Else
            Return umap_result
        End If
    End Function

    Public Function GetMatrixDims() As String
        Return matrix_dims.GetJson
    End Function

    Public Function GetScatter() As String
        If umap_result.StringEmpty Then
            umap_result = $"{matrix.ParentPath}/{matrix.BaseName}_umap3.csv"

            If Not umap_result.FileExists Then
                umap_result = Nothing
            End If
        End If
        If umap_result.StringEmpty Then
            Return "[]"
        Else
            Dim try_kmeans As String = get_kmeans()
            Dim points As UMAPPoint()

            If try_kmeans.FileExists Then
                points = UMAPPoint.ParseCsvTable(try_kmeans).ToArray
            Else
                points = UMAPPoint.ParseCsvTable(umap_result).ToArray
            End If

            Dim json As String = points.GetJson
            Return json
        End If
    End Function

    Private Function get_kmeans() As String
        Return umap_result.TrimSuffix & "_kmeans.csv"
    End Function

    Public Async Function Run(knn As Integer, knniter As Integer,
                   localConnectivity As Double,
                   bandwidth As Double,
                   learningRate As Double,
                   spectral_cos As Boolean) As Task(Of Boolean)

        Dim umap3 As Task(Of String) = Task(Of String) _
            .Run(Function() As String
                     Return RscriptProgressTask.CreateUMAPCluster(
                        matrix,
                        knn, knniter, localConnectivity, bandwidth, learningRate, spectral_cos,
                        readBinary:=binaryMatrix,
                        noUI:=True)
                 End Function)

        umap_result = Await umap3

        Return True
    End Function

    Public Async Function RunKmeans(k As Integer) As Task(Of Boolean)
        If umap_result.StringEmpty Then
            Return False
        End If

        Dim savefile As String = get_kmeans()
        Dim flag = Task(Of Boolean).Run(
            Function()
                Return RscriptProgressTask.KMeans(umap_result, k, savefile, noUI:=True)
            End Function)

        Return Await flag
    End Function

    Public Sub Save()
        If Not callback Is Nothing Then
            Call _callback(umap_result)
        End If
    End Sub

    Public Sub Click(tag As String)
        If Not onclick Is Nothing Then
            Call _onclick(tag)
        End If
    End Sub
End Class