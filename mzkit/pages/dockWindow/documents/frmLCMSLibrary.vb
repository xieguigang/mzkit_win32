Imports System.IO
Imports System.Runtime.InteropServices
Imports BioDeep
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.SpectrumTree.PackLib
Imports BioNovoGene.BioDeep.Chemistry
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports Task

Public Class frmLCMSLibrary

    Dim [lib] As New LibraryApp
    Dim root As TreeNode

    Public ReadOnly Property sourceURL As String
        Get
            Return $"http://127.0.0.1:{Workbench.WebPort}/LCMS-rql.html"
        End Get
    End Property

    Private Sub frmLCMSLibrary_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Text = "LCMS Reference Library"
        TabText = Text
        AutoScaleMode = AutoScaleMode.Dpi
        root = Win7StyleTreeView1.Nodes(0)

        Call WebKit.Init(Me.WebView21)
        Call LoadLibs()
        Call ApplyVsTheme(ToolStrip1, ContextMenuStrip1)
    End Sub

    Private Sub LoadLibs()
        root.Nodes.Clear()

        For Each file As String In SpectrumLibraryModule.ScanLibraries
            Dim libFolder = root.Nodes.Add(file.BaseName)

            libFolder.Tag = file
            libFolder.ImageIndex = 1
            libFolder.SelectedImageIndex = 1
        Next
    End Sub

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", [lib])
        Call WebView21.CoreWebView2.Navigate(sourceURL)
        Call WebKit.DeveloperOptions(WebView21, enable:=True, TabText)
    End Sub

    Private Sub OpenToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenToolStripMenuItem.Click
        Dim libNode = Win7StyleTreeView1.SelectedNode
        Dim filepath As String = libNode.Tag

        If filepath.StringEmpty Then
            Return
        End If

        Call WebView21.ExecuteScriptAsync($"apps.viewer.lcmsLibrary.openLibfile('{filepath.Replace("\", "/")}', null);")
    End Sub

    Private Sub NewLibraryToolStripMenuItem_Click() Handles NewLibraryToolStripMenuItem.Click
        If Not [lib].NewLibrary() Then
            Call Workbench.Warning("Create new library canceled or not successed.")
        Else
            Call LoadLibs()
        End If
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        NewLibraryToolStripMenuItem_Click()
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        Dim libNode = Win7StyleTreeView1.SelectedNode
        Dim filepath As String = libNode.Tag

        If filepath.StringEmpty Then
            Return
        End If

        If MessageBox.Show($"Going to delete the reference library: {filepath.BaseName}?", "Delete Library", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) <> DialogResult.OK Then
            Return
        ElseIf [lib].current_file.BaseName.TextEquals(filepath.BaseName) Then
            Call [lib].Close()
        End If

        Call filepath.DeleteFile
        Call LoadLibs()
    End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Call LoadLibs()
    End Sub

    Private Sub ExportLibraryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportLibraryToolStripMenuItem.Click
        Dim libNode = Win7StyleTreeView1.SelectedNode
        Dim filepath As String = libNode.Tag

        If filepath.StringEmpty Then
            Return
        End If

        If (Not [lib].current_file.StringEmpty) AndAlso [lib].current_file.BaseName.TextEquals(filepath.BaseName) Then
            Call [lib].Close()
        End If

        Using folder As New FolderBrowserDialog With {
            .Description = "Select a folder path for export the reference library data.",
            .ShowNewFolderButton = True
        }
            If folder.ShowDialog = DialogResult.OK Then
                Call TaskProgress.RunAction(
                    run:=Sub(proc As ITaskProgress)
                             Call ExportLibPack(filepath, folder.SelectedPath, proc)
                         End Sub,
                    title:="Export library data",
                    info:="Export the reference library data for the biodeep workflow...."
                )

                If MessageBox.Show("The LCMS reference library export job done!" & vbCrLf & "View of the reference library folder result?",
                                   "View Result",
                                   MessageBoxButtons.OKCancel,
                                   MessageBoxIcon.Information) = DialogResult.OK Then

                    Call Process.Start(folder.SelectedPath)
                End If
            End If
        End Using
    End Sub

    Private Sub ExportLibPack(libfile As String, export_dir As String, proc As ITaskProgress)
        Dim libdata As New RQLib(libfile.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=True))
        Dim libpos As New SpectrumPack($"{export_dir}/lib.pos.pack".Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False))
        Dim libneg As New SpectrumPack($"{export_dir}/lib.neg.pack".Open(FileMode.OpenOrCreate, doClear:=True, [readOnly]:=False))
        Dim biodeepid As New Dictionary(Of String, String)

        libfile = libfile.BaseName

        For Each metabolite In libdata.ListMetabolites(1, page_size:=Integer.MaxValue)
            If metabolite Is Nothing Then
                Continue For
            End If

            Dim spectral = libdata.GetSpectrumByKey(metabolite.ID)

            If spectral Is Nothing Then
                Continue For
            Else
                biodeepid(metabolite.ID) = metabolite.ID
            End If

            Dim peak As PeakMs2 = mzPack.CastToPeakMs2(spectral, file:=libfile)
            Dim ionMode As Integer = spectral.polarity

            If ionMode = 1 Then
                Call libpos.Push(metabolite.ID, metabolite.formula, peak)
            Else
                Call libneg.Push(metabolite.ID, metabolite.formula, peak)
            End If
        Next

        Call libpos.Dispose()
        Call libneg.Dispose()

        Call biodeepid _
            .GetJson _
            .SaveTo($"{export_dir}/biodeepdb.json")
    End Sub
End Class

' 所有需要在JavaScript环境中暴露的对象
' 都需要标记上下面的两个自定义属性
<ClassInterface(ClassInterfaceType.AutoDual)>
<ComVisible(True)>
Public Class LibraryApp

    Friend current As RQLib
    Friend current_file As String

    ''' <summary>
    ''' Close current
    ''' </summary>
    Public Sub Close()
        current.Dispose()
        current = Nothing
    End Sub

    Public Function ScanLibraries() As String
        Dim files As String() = SpectrumLibraryModule.ScanLibraries.ToArray
        Return files.GetJson
    End Function

    Public Function OpenLibrary(path As String) As Boolean
        If current IsNot Nothing Then
            Call current.Dispose()
        End If

        current_file = path
        current = New RQLib(path.Open(FileMode.OpenOrCreate, doClear:=False, [readOnly]:=False))

        Return True
    End Function

    Public Function GetPage(page As Integer, page_size As Integer) As String
        If current Is Nothing Then
            Return "[]"
        End If

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

    Public Function ShowSpectral(data_id As String) As Boolean
        Dim raw As ScanMS2 = current.GetSpectrumByKey(data_id)
        Dim spectral As PeakMs2 = Nothing

        If Not raw Is Nothing Then
            spectral = mzPack.CastToPeakMs2(raw, file:=current_file.BaseName)
        End If

        If Not spectral Is Nothing Then
            Call SpectralViewerModule.ViewSpectral(spectral)
            Call MyApplication.host.ShowProperties(New SpectrumProperty(data_id, current_file.BaseName, 2, raw))
        Else
            Call Workbench.Warning($"missing spectral data associated with guid key: {data_id}!")
        End If

        Return True
    End Function

    Public Async Function AlignSpectral(data_id As String) As Task(Of Boolean)
        Dim raw As ScanMS2 = current.GetSpectrumByKey(data_id)
        Dim spectral As PeakMs2 = Nothing
        Dim searchTask As Action =
            Sub()
                ' align with samples inside the sample file explorer
                Dim doc As frmSpectrumSearch = MyApplication.host.Invoke(Function() SpectrumSearchModule.ShowDocument)

                Call doc.Invoke(
                    Sub()
                        Call doc.LoadMs2(spectral)
                        ' Call doc.RunSearch(showUi:=False)
                    End Sub)
            End Sub

        If Not raw Is Nothing Then
            spectral = mzPack.CastToPeakMs2(raw, file:=current_file.BaseName)
        End If

        If Not spectral Is Nothing Then
            Await Threading.Tasks.Task.Run(searchTask)
        Else
            Call Workbench.Warning($"missing spectral data associated with guid key: {data_id}!")
        End If

        Return True
    End Function

    Public Async Function FindExactMass(mass As Double) As Task(Of Boolean)
        Dim findMSI As frmMsImagingViewer = MyApplication.host.DockPanel.Documents _
            .Where(Function(p) TypeOf p Is frmMsImagingViewer) _
            .FirstOrDefault

        If findMSI Is Nothing Then
            ' search for the lcms file directly

        Else
            ' show dialog for select item 
            ' and then implements the exact mass search

        End If

        Dim qTask =
            Sub()
                Call MyApplication.host.Invoke(
                    Sub()
                        Call WindowModules.fileExplorer.SearchExactMassFeatures(mass, Tolerance.PPM(20))
                    End Sub)
            End Sub

        Await Threading.Tasks.Task.Run(qTask)

        Return True
    End Function

    Public Function NewLibrary() As Boolean
        Dim libfile As String = CreateLibrary()

        If libfile.StringEmpty OrElse libfile.FileLength < 1024 Then
            Return False
        Else
            Return True
        End If
    End Function

    Private Function CreateLibrary() As String
        Dim libfile As String = Nothing

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
            Call libdb.AddSpectrum(ion.GetSpectrumPeaks, key:=ion.ID)
            Call libdb.AddSpectrum(ion.GetSpectrumPeaks, key:=ion.name)

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