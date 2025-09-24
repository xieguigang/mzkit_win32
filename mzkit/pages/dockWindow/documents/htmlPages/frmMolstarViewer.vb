Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Threading
Imports BioNovoGene.mzkit_win32.ServiceHub.Manager
Imports Emily.PDB_Canvas
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.Web.WebView2.Core
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.Container
Imports RibbonLib.Interop
Imports SMRUCC.genomics.Data.RCSB.PDB
Imports SMRUCC.genomics.Data.RCSB.PDB.Keywords
Imports TaskStream

Public Class frmMolstarViewer

    Dim localfs As Process
    Dim webPort As Integer = -1
    Dim pdb As PDB
    Dim pdbfile As String
    Dim docking_pdbqt As Boolean = False

    Public ReadOnly Property sourceURL As String
        Get
            Return $"http://127.0.0.1:{webPort}/molstar/index.html"
        End Get
    End Property

    ' 所有需要在JavaScript环境中暴露的对象
    ' 都需要标记上下面的两个自定义属性
    <ClassInterface(ClassInterfaceType.AutoDual)>
    <ComVisible(True)>
    Public Class DataSource

    End Class

    Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        AutoScaleMode = AutoScaleMode.Dpi
    End Sub

    Private Sub startHttp()
        webPort = Net.Tcp.GetFirstAvailablePort(9001)
        localfs = New Process With {
            .StartInfo = New ProcessStartInfo With {
                .FileName = $"{App.HOME}/Rstudio/bin/Rserve.exe",
                .Arguments = $"--listen /wwwroot ""{AppEnvironment.getWebViewFolder}"" /port {webPort} --parent={App.PID}",
                .CreateNoWindow = True,
                .WindowStyle = ProcessWindowStyle.Hidden,
                .UseShellExecute = True
            }
        }

        Call localfs.Start()
        Call App.AddExitCleanHook(Sub() Call localfs.Kill())
        Call Hub.Register(New Service With {
            .CPU = 0,
            .Name = "molstar molecule viewer",
            .Description = "Host the molstar molecular viewer model data(pdb files) read/loading from the local filesystem, and then rendering on the 3d model viewer.",
            .isAlive = True,
            .Memory = 0,
            .PID = localfs.Id,
            .Port = webPort,
            .Protocol = "HTTP 1.0",
            .StartTime = Now.ToString,
            .CommandLine = Service.GetCommandLine(localfs)
        })

        Call WorkStudio.LogCommandLine(localfs)
    End Sub

    Shared ReadOnly openButton As New RibbonEventBinding(ribbonItems.ButtonMolmilOpenFile)
    Shared ReadOnly resetCameraButton As New RibbonEventBinding(ribbonItems.ButtonMolstarResetCamera)
    Shared ReadOnly clearCanvasButton As New RibbonEventBinding(ribbonItems.ButtonMolstarClearCanvas)
    Shared ReadOnly snapshotButton As New RibbonEventBinding(ribbonItems.ButtonMolstarMakeSnapshot)
    Shared ReadOnly show2Dplot As New RibbonEventBinding(ribbonItems.ButtonLigand2DPlot)

    Private Sub frm3DMALDIViewer_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Call localfs.Kill()

        ribbonItems.MenuMolmil.ContextAvailable = ContextAvailability.NotAvailable
    End Sub

    Private Sub frm3DMALDIViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Call startHttp()
        Call WebKit.Init(Me.WebView21)
        Call GetActivated()
    End Sub

    Private Sub OpenPdb()
        Using file As New OpenFileDialog With {.Filter = "Protein Structure Data(*.pdb)|*.pdb|Vina Docking Result(*.pdbqt)|*.pdbqt"}
            If file.ShowDialog = DialogResult.OK Then
                Dim pdb_txt As String = file.FileName.ReadAllText
                ' 发送消息到 JavaScript
                Dim jsonString As String = "null"

                pdbfile = file.FileName
                docking_pdbqt = pdbfile.ExtensionSuffix("pdbqt")

                Call ProgressSpinner.DoLoading(
                    Sub()
                        ' 自动处理特殊字符
                        jsonString = pdb_txt.GetJson(simpleDict:=True)

                        Me.Invoke(Sub()
                                      Me.pdb = PDB.Parse(pdb_txt, verbose:=False)
                                  End Sub)
                    End Sub)

                WebView21.CoreWebView2.PostWebMessageAsString(jsonString)
            End If
        End Using
    End Sub

    Private Sub View2DPlot()
        If pdb Is Nothing Then
            Call Workbench.Warning("no pdb data to view...")
            Return
        End If

        Dim ligands As NamedValue(Of Het.HETRecord)() = pdb.ListLigands.ToArray
        Dim list As String() = ligands _
            .Select(Function(l) $"[{l.Value.SequenceNumber}] {l.Name}: {l.Description}") _
            .ToArray
        Dim keyList = list.Zip(ligands) _
            .ToDictionary(Function(a) a.First,
                          Function(a)
                              Return a.Second
                          End Function)

        If docking_pdbqt Then
            Call draw2DLigands(pdb.GetLigandReference)
        Else
            If ligands.IsNullOrEmpty Then
                MessageBox.Show("Current protein molecule docking data contains no ligand model.",
                                "No data",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning)
                Return
            Else
                Call SelectSheetName.SelectName(list,
                    show:=Sub(name)
                              draw2DLigands(keyList(name))
                          End Sub,
                    title:="View Liagnd Plot",
                    labeltext:="Select a ligand and view 2d docking plot")
            End If
        End If
    End Sub

    Private Sub draw2DLigands(ligand As NamedValue(Of Het.HETRecord))
        Dim theme As New Theme With {
            .padding = "padding: 10% 10% 10% 10%;"
        }
        Dim render As New Ligand2DPlot(pdb, ligand, theme)
        Dim page = VisualStudio.ShowDocument(Of frmPlotViewer)(, ligand.Name)
        Dim key As String = ligand.Value.ResidueType
        Dim number As Integer = ligand.Value.SequenceNumber
        Dim ref As New NamedValue(Of Integer)(key, number)

        Call render.CalculateMaxPlainView()

        page.showImage(render, New Ligand2DPlotArguments(theme, render.ViewPoint))
        page.Filter = "plot image(*.png)|*.png|plot image(*.svg)|*.svg|plot image(*.pdf)|*.pdf"
        page.FileSave =
            Sub(outfile As String, args As frmPlotViewer.Arguments)
                Dim args_json As String = DirectCast(args, Ligand2DPlotArguments).GetJson(simpleDict:=True)

                If RscriptProgressTask.PlotLigand2DPlot(pdbfile, ref, outfile, args_json, args.ppi, $"{args.width},{args.height}") Then
                    Call MessageBox.Show("Export Plot success!", "Job Done", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Sub
    End Sub

    Public Class Ligand2DPlotArguments : Inherits frmPlotViewer.Arguments

        ReadOnly theme As Theme

        <Category("3D Camera")>
        <DisplayName("View X")>
        <Description("3D camera view point for make the 3d pdb model to 2d plot projection")>
        Public Property viewX As Double
        <Category("3D Camera")>
        <DisplayName("View Y")>
        <Description("3D camera view point for make the 3d pdb model to 2d plot projection")>
        Public Property viewY As Double
        <Category("3D Camera")>
        <DisplayName("View Z")>
        <Description("3D camera view point for make the 3d pdb model to 2d plot projection")>
        Public Property viewZ As Double

        <Category("Ligand Model")>
        <DisplayName("Distance Cutoff")>
        <Description("The distance cutoff value of ligand atom to the amino acid residue, in data unit Å, default value is 3.5Å")>
        Public Property DistanceCutoff As Double = 3.5
        <Category("Ligand Model")>
        <DisplayName("KNN")>
        <Description("Select closed top n ligand atom and amino acid residue for display the interaction. defult choose the top one closest.")>
        Public Property TopRank As Integer = 1
        <Category("Ligand Render")>
        <DisplayName("Ligand Atom Size")>
        <Description("The shape size of the ligand atom.")>
        Public Property AtomSize As Single = 95
        <Category("Ligand Render")>
        <DisplayName("Amino Acid Size")>
        <Description("The shape size of the amino acid residue.")>
        Public Property AminoAcidSize As Single = 150

        <Category("Ligand Render")>
        <DisplayName("Show Liagnd Atom Label")>
        <Description("Show or hide the ligand molecule atom label on the graphics plot?")>
        Public Property ShowLigandAtomLabel As Boolean = True
        <Category("Ligand Render")>
        <DisplayName("Amino Acid Min Font Size")>
        <Description("The min size of the font label for show the amino acid residue label")>
        Public Property AminoAcidFontSizeMin As Double = 12
        <Category("Ligand Render")>
        <DisplayName("Amino Acid Max Font Size")>
        <Description("The max size of the font label for show the amino acid residue label")>
        Public Property AminoAcidFontSizeMax As Double = 40

        Sub New(theme As Theme, view As Drawing3D.Point3D)
            Me.theme = theme
            Me.viewX = view.X
            Me.viewY = view.Y
            Me.viewZ = view.Z
        End Sub

        Sub New()
        End Sub

        Public Overrides Sub Update(plot As Plot)
            Dim render As Ligand2DPlot = DirectCast(plot, Ligand2DPlot)

            render.ViewPoint = New Drawing3D.Point3D(viewX, viewY, viewZ)
            render.DistanceCutoff = DistanceCutoff
            render.TopRank = TopRank
            render.AtomSize = AtomSize
            render.AminoAcidSize = AminoAcidSize
            render.ShowAtomLabel = ShowLigandAtomLabel
            render.FontSizeMin = AminoAcidFontSizeMin
            render.FontSizeMax = AminoAcidFontSizeMax
            render.Build3DModel()
        End Sub
    End Class

    Private Sub WebView21_CoreWebView2InitializationCompleted(sender As Object, e As CoreWebView2InitializationCompletedEventArgs) Handles WebView21.CoreWebView2InitializationCompleted
        Do While webPort <= 0
            Call System.Windows.Forms.Application.DoEvents()
            Call Thread.Sleep(10)
        Loop

        Call WebView21.CoreWebView2.AddHostObjectToScript("mzkit", New DataSource)
        Call WebView21.CoreWebView2.Navigate(sourceURL)
        Call WebKit.DeveloperOptions(WebView21, enable:=True, TabText:=TabText)
    End Sub

    Private Sub resetCamera()
        Call WebView21.CoreWebView2.ExecuteScriptAsync("viewer.resetCamera();")
    End Sub

    Private Sub clearCanvas()
        Call WebView21.CoreWebView2.ExecuteScriptAsync("viewer.clear();")
    End Sub

    Private Sub takeSnapshot()
        Call WebView21.CoreWebView2.ExecuteScriptAsync("requestSnapshotImage();")
    End Sub

    Private Sub frmMolstarViewer_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        openButton.evt = Nothing
        resetCameraButton.evt = Nothing
        clearCanvasButton.evt = Nothing
        snapshotButton.evt = Nothing
        show2Dplot.evt = Nothing

        ribbonItems.MenuMolmil.ContextAvailable = ContextAvailability.NotAvailable
    End Sub

    Private Sub GetActivated() Handles Me.Activated
        ribbonItems.MenuMolmil.ContextAvailable = ContextAvailability.Active

        snapshotButton.evt = AddressOf takeSnapshot
        openButton.evt = AddressOf OpenPdb
        resetCameraButton.evt = AddressOf resetCamera
        clearCanvasButton.evt = AddressOf clearCanvas
        show2Dplot.evt = AddressOf View2DPlot
    End Sub

    Private Sub frmMolstarViewer_GotFocus(sender As Object, e As EventArgs) Handles Me.GotFocus
        Call GetActivated()
    End Sub
End Class