Imports System.IO
Imports System.Runtime.InteropServices
Imports BioDeep
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm

Public Class frmLCMSLibrary

    Dim [lib] As New LibraryApp

    Public ReadOnly Property sourceURL As String
        Get
            Return $"http://127.0.0.1:{Workbench.WebPort}/LCMS-rql.html"
        End Get
    End Property

    Private Sub frmLCMSLibrary_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Text = "LCMS Reference Library"
        TabText = Text
        AutoScaleMode = AutoScaleMode.Dpi

        Call WebKit.Init(Me.WebView21)
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", New LibraryApp)
        Call WebView21.CoreWebView2.Navigate(sourceURL)
        Call WebKit.DeveloperOptions(WebView21, enable:=True, TabText)
    End Sub
End Class

' 所有需要在JavaScript环境中暴露的对象
' 都需要标记上下面的两个自定义属性
<ClassInterface(ClassInterfaceType.AutoDual)>
<ComVisible(True)>
Public Class LibraryApp

    Dim current As RQLib

    Public Function ScanLibraries() As String
        Dim files As String() = $"{App.ProductProgramData}/lcms/".ListFiles("*.lcms-pack")
        Return files.GetJson
    End Function

    Public Function OpenLibrary(path As String) As Boolean
        If current IsNot Nothing Then
            Call current.Dispose()
        End If

        current = New RQLib(path.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=False))

        Return True
    End Function

    Public Function Query(name As String) As String
        If current Is Nothing Then
            Return "[]"
        End If

        Dim result = current.
    End Function
End Class