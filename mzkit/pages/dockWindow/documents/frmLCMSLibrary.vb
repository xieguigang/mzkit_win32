Imports System.IO
Imports System.Runtime.InteropServices
Imports BioDeep
Imports BioNovoGene.BioDeep.Chemistry
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs

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
        Dim files As String() = SpectrumLibraryModule.ScanLibraries.ToArray
        Return files.GetJson
    End Function

    Public Function OpenLibrary(path As String) As Boolean
        If current IsNot Nothing Then
            Call current.Dispose()
        End If

        current = New RQLib(path.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=False))

        Return True
    End Function

    Public Function GetPage(page As Integer, page_size As Integer) As String
        Dim ls = current.ListMetabolites(page, page_size).ToArray
        Dim json As String = ls.GetJson
        Return json
    End Function

    Public Function Query(name As String) As String
        If current Is Nothing Then
            Return "[]"
        End If

        Dim result = current.QueryMetabolites(name).ToArray
        Return result.GetJson
    End Function

    Public Async Function NewLibrary() As Task(Of Boolean)
        Dim libfile As String = Await CreateLibrary()

        If libfile.StringEmpty OrElse libfile.FileLength < 1024 Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Async Function CreateLibrary() As Task(Of String)
        Dim libfile As String = Nothing
        Dim create As Action =
             Sub()
                 InputDialog.Input(Of InputCreateLCMSLibrary)(
                     Sub([lib])
                         Dim name As String = [lib].LibraryName
                         Dim import As String = [lib].FromImports

                         libfile = SpectrumLibraryModule.LibraryFile(name)

                         If import.FileLength > 1 Then
                             Call importsMsp(import, libfile)
                         Else
                             Call createEmpty(libfile)
                         End If
                     End Sub)
             End Sub

        Await Threading.Tasks.Task.Run(create)

        Return libfile
    End Function

    Private Sub importsMsp(msp_file As String, libfile As String)
        Dim libdb As New RQLib(libfile.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=False))
        Dim msp As IEnumerable(Of SpectraSection) = MspReader.ParseFile(msp_file)
        Dim d As Integer = 17
        Dim i As i32 = 0

        For Each ion As SpectraSection In msp
            Dim anno = ion.GetMetabolite

            Call libdb.AddAnnotation(anno)
            Call libdb.AddSpectrum(ion.GetSpectrumPeaks, key:=anno.name)

            Call System.Windows.Forms.Application.DoEvents()

            If ++i Mod d = 0 Then
                Call Workbench.LogText("Installing... " & ion.ToString)
            End If
        Next

        Call libdb.Dispose()
    End Sub

    Private Sub createEmpty(libfile As String)
        Dim libdata As New RQLib(libfile.Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False))
        Call libdata.Dispose()
    End Sub
End Class