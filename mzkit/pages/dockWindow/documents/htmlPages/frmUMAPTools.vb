﻿Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
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
    ''' the csv file path of the umap analysis result
    ''' </summary>
    ''' <returns></returns>
    Public Property umap_result As String

    Public Function GetMatrixDims() As String
        Return matrix_dims.GetJson
    End Function

    Public Sub Apply()
        If Not callback Is Nothing Then
            Call _callback(umap_result)
        End If
    End Sub

End Class