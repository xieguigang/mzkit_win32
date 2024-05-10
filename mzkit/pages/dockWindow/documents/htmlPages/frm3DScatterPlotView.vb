Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.MsImaging.TissueMorphology
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm

Public Class frm3DScatterPlotView

    Public ReadOnly Property sourceURL As String
        Get
            Return $"http://127.0.0.1:{Workbench.WebPort}/3d-scatter.html"
        End Get
    End Property

    Dim source As New DataSource

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="onclick">
    ''' callback of the <see cref="UMAPPoint.label"/> on click
    ''' </param>
    Public Sub LoadScatter(data As IEnumerable(Of UMAPPoint), onclick As Action(Of String))
        source.points = data _
            .Select(Function(a)
                        Return New UMAPPoint With {
                            .[class] = a.class,
                            .label = a.label,
                            .Pixel = Nothing,
                            .x = a.x,
                            .y = a.y,
                            .z = a.z
                        }
                    End Function) _
            .ToArray
        source.onclick = onclick
    End Sub

    Private Sub frm3DScatterPlotView_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "View Scatter Plot"
        WebKit.Init(WebView21)
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        ' WebView21.CoreWebView2.OpenDevToolsWindow()
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", Me.source)
        Call WebView21.CoreWebView2.Navigate(sourceURL)
        Call WebKit.DeveloperOptions(WebView21, enable:=True)
    End Sub

    ' 所有需要在JavaScript环境中暴露的对象
    ' 都需要标记上下面的两个自定义属性
    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class DataSource

        Friend points As UMAPPoint()
        Friend onclick As Action(Of String)

        Public Function GetScatter() As String
            If points.IsNullOrEmpty Then
                Return "[]"
            Else
                Return JsonContract.GetJson(points)
            End If
        End Function

        Public Sub Click(tag As String)
            If Not onclick Is Nothing Then
                Call onclick(tag)
            End If
        End Sub
    End Class
End Class