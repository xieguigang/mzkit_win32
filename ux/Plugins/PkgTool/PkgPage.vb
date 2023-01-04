Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm

Public Class PkgPage

    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class pluginCreator

        Public Function SelectFolder() As String
            Using folder As New FolderBrowserDialog
                If folder.ShowDialog = DialogResult.OK Then
                    Return folder.SelectedPath
                Else
                    Return Nothing
                End If
            End Using
        End Function

        Public Function GetFiles(dir As String) As String
            Return dir.ListFiles().ToArray.GetJson
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="folder"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Build is a key word? 
        ''' </remarks>
        Public Function BuildPkg(folder As String) As Boolean
            Dim pkg = PluginPkg.FromAppReleaseDirectory(folder)

            Using pkgFile As New SaveFileDialog With {
                .Filter = "MZKit Plugin Package(*.mzdll)",
                .FileName = pkg.Name.NormalizePathString & ".mzdll"
            }
                If pkgFile.ShowDialog = DialogResult.OK Then
                    Using file As Stream = pkgFile.FileName.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False)
                        Return pkg.BuildPackage(New ZipArchive(file, ZipArchiveMode.Create), folder)
                    End Using
                End If
            End Using

            Return False
        End Function
    End Class

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        AutoScaleMode = AutoScaleMode.Dpi
    End Sub

    Private Sub PkgPage_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TabText = "Create MZKit Plugin Package"

        DoubleBuffered = True
        WebKit.Init(WebView21)
    End Sub

    Public Sub SetObject(obj As Object)
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", obj)
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        Call SetObject(New pluginCreator)
        Call WebView21.CoreWebView2.Navigate($"http://127.0.0.1:{Workbench.WebPort}/pluginPkgTool.html")
        Call WebKit.DeveloperOptions(WebView21, enable:=True)
    End Sub
End Class