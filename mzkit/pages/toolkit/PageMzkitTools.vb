#Region "Microsoft.VisualBasic::a5e9b66e1d576a5f5d204479c731f2b8, mzkit\src\mzkit\mzkit\pages\toolkit\PageMzkitTools.vb"

' Author:
' 
'       xieguigang (gg.xie@bionovogene.com, BioNovoGene Co., LTD.)
' 
' Copyright (c) 2018 gg.xie@bionovogene.com, BioNovoGene Co., LTD.
' 
' 
' MIT License
' 
' 
' Permission is hereby granted, free of charge, to any person obtaining a copy
' of this software and associated documentation files (the "Software"), to deal
' in the Software without restriction, including without limitation the rights
' to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
' copies of the Software, and to permit persons to whom the Software is
' furnished to do so, subject to the following conditions:
' 
' The above copyright notice and this permission notice shall be included in all
' copies or substantial portions of the Software.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
' OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
' SOFTWARE.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 876
'    Code Lines: 665
' Comment Lines: 56
'   Blank Lines: 155
'     File Size: 39.36 KB


' Class PageMzkitTools
' 
'     Function: getSelectedIonSpectrums, getXICMatrix, missingCacheFile, rawTIC, relativeInto
' 
'     Sub: ClearToolStripMenuItem_Click, CustomTabControl1_TabClosing, DataGridView1_CellContentClick, ExportExactMassSearchTable, (+2 Overloads) MolecularNetworkingTool
'          PageMzkitTools_Load, PageMzkitTools_Resize, PictureBox1_DoubleClick, PictureBox1_MouseClick, PlotMatrx
'          PlotSpectrum, Ribbon_Load, SaveImageToolStripMenuItem_Click, SaveMatrixToolStripMenuItem_Click, (+2 Overloads) showAlignment
'          (+3 Overloads) showMatrix, (+2 Overloads) ShowMatrix, ShowMRMTIC, ShowPage, ShowPlotTweaks
'          showScatter, showSpectrum, ShowTabPage, showUVscans, ShowXIC
'          (+3 Overloads) TIC
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.Math.UV
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.mzkit_win32.MSdata
Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.RibbonLib.Controls
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.MatrixViewer
Imports RibbonLib
Imports RibbonLib.Interop
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports Task
Imports WeifenLuo.WinFormsUI.Docking

Public Class PageMzkitTools

    Friend _matrix As DataMatrix
    Friend _ribbonExportDataContextMenuStrip As ExportData

    Public Sub Ribbon_Load(ribbon As Ribbon)
        _ribbonExportDataContextMenuStrip = New ExportData(ribbon, RibbonItems.cmdContextMap)
    End Sub

    Private Function missingCacheFile(raw As MZWork.Raw) As DialogResult
        Dim options As DialogResult = MessageBox.Show($"The specific raw data cache is missing, run imports again?{vbCrLf}{raw.source.GetFullPath}", $"[{raw.source.FileName}] Cache Not Found!", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning)
        Dim fileExplorer = WindowModules.fileExplorer

        If options = DialogResult.OK Then
            Dim newRaw As MZWork.Raw = frmFileExplorer.getRawCache(raw.source)

            raw.cache = newRaw.cache

            MyApplication.host.showStatusMessage("Ready!")
            MyApplication.host.ToolStripStatusLabel2.Text = fileExplorer.treeView1.Nodes(Scan0).GetTotalCacheSize
        End If

        Return options
    End Function

    Public Sub ShowPage()
        MyApplication.host.ShowPage(Me)
    End Sub

    Public Sub showScatter(raw As MZWork.Raw, XIC As Boolean, directSnapshot As Boolean, contour As Boolean)
        If Not raw.cacheFileExists Then
            MessageBox.Show("Sorry, can not view file data, the cache file is missing...", "Cache Missing", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        ElseIf directSnapshot Then
            PictureBox1.BackgroundImage = raw.GetSnapshot
            Return
        End If

        Dim colorSet As String
        Dim data As ContourLayer() = Nothing
        Dim width As Integer = 2048
        Dim height As Integer = 1600
        Dim padding As String = "padding:100px 400px 100px 100px;"
        Dim matrixName = $"{raw.source.FileName}_{If(XIC, "XICPeaks", "rawscatter_2D")}"

        If XIC Then
            colorSet = "YlGnBu:c8"
            width = 2400

            Call ProgressSpinner.DoLoading(
                Sub()
                    Me.Invoke(Sub() _matrix = New ChromatogramOverlapMatrix(matrixName, raw.Get3DPeaks.ToArray, spatial3D:=True))
                End Sub)
        ElseIf contour Then
            colorSet = "Jet"
            width = 3600
            height = 2700
            padding = "padding:100px 750px 100px 100px;"

            Call ProgressSpinner.DoLoading(Sub() Me.Invoke(Sub() _matrix = New CounterMatrix(matrixName, raw)))
        Else
            colorSet = "darkblue,blue,skyblue,green,orange,red,darkred"

            Call ProgressSpinner.DoLoading(Sub() Me.Invoke(Sub() _matrix = New Ms1ScatterMatrix(matrixName, raw)))
        End If

        Call _matrix.LoadMatrix(DataGridView1, BindingSource1)

        Call MyApplication.RegisterPlot(
            plot:=Sub(args)
                      Call ProgressSpinner.DoLoading(
                    Sub()
                        Me.Invoke(Sub() PictureBox1.BackgroundImage = _matrix.Plot(args, PictureBox1.Size).AsGDIImage)
                    End Sub)
                  End Sub,
            colorSet:=colorSet,
            width:=width,
            height:=height,
            padding:=padding,
            legendTitle:="Levels"
        )

        MyApplication.host.ShowPage(Me)
        MyApplication.host.Invoke(Sub() ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.NotAvailable)
    End Sub

    Public Sub ShowPlotImage(Rplot As Image, layout As ImageLayout)
        PictureBox1.BackgroundImage = Rplot
        PictureBox1.BackgroundImageLayout = layout

        MyApplication.host.ShowPage(Me)
    End Sub

    Private Sub PageMzkitTools_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim host = MyApplication.host

        ' Call InitializeFileTree()
        Call Globals.sharedProgressUpdater("Attatch Command Events...")

        'AddHandler TreeView1.AfterSelect, AddressOf TreeView1_AfterSelect
        'AddHandler host.fileExplorer.Button1.Click, Sub(obj, evt) Call SearchByMz(host.fileExplorer.TextBox2.Text)

        'AddHandler host.fileExplorer.ShowTICToolStripMenuItem.Click, AddressOf ShowTICToolStripMenuItem_Click
        'AddHandler host.fileExplorer.ShowXICToolStripMenuItem.Click, AddressOf ShowXICToolStripMenuItem_Click

        'AddHandler host.fileExplorer.ClearToolStripMenuItem.Click, AddressOf ClearToolStripMenuItem_Click
        'AddHandler host.fileExplorer.ExportToolStripMenuItem.Click, AddressOf ExportToolStripMenuItem_Click

        'AddHandler host.fileExplorer.MS1ToolStripMenuItem.Click, AddressOf MS1ToolStripMenuItem_Click
        'AddHandler host.fileExplorer.MS2ToolStripMenuItem.Click, AddressOf MS2ToolStripMenuItem_Click

        'AddHandler host.fileExplorer.MolecularNetworkingToolStripMenuItem.Click, AddressOf MolecularNetworkingToolStripMenuItem_Click

        'AddHandler host.fileExplorer.SearchInFileToolStripMenuItem.Click, AddressOf SearchInFileToolStripMenuItem_Click
        'AddHandler host.fileExplorer.CustomToolStripMenuItem.Click, AddressOf CustomToolStripMenuItem_Click
        'AddHandler host.fileExplorer.DefaultToolStripMenuItem.Click, AddressOf DefaultToolStripMenuItem_Click
        'AddHandler host.fileExplorer.SmallMoleculeToolStripMenuItem.Click, AddressOf SmallMoleculeToolStripMenuItem_Click
        'AddHandler host.fileExplorer.NatureProductToolStripMenuItem.Click, AddressOf NatureProductToolStripMenuItem_Click
        'AddHandler host.fileExplorer.GeneralFlavoneToolStripMenuItem.Click, AddressOf GeneralFlavoneToolStripMenuItem_Click
    End Sub

    Friend Sub showSpectrum(scanId As String, raw As MZWork.Raw, formula As String)
        If raw.cacheFileExists OrElse raw.isInMemory Then
            Dim prop As SpectrumProperty = Nothing
            Dim showAnnotation As Boolean = RibbonEvents.ribbonItems.CheckBoxShowMs2Fragment.BooleanValue
            Dim scanData As LibraryMatrix = raw.GetSpectrum(
                scanId:=scanId,
                cutoff:=Globals.Settings.viewer.GetMethod,
                reload:=Sub(src, cache) frmFileExplorer.getRawCache(src,, cache),
                showAnnotation:=showAnnotation,
                properties:=prop
            )

            If scanData Is Nothing Then
                Call Workbench.Warning($"Sorry, could not fould any spectrum data that related with scan id: {scanId}.")
                Return
            End If

            If prop.msLevel = 1 AndAlso ribbonItems.CheckBoxShowKEGGAnnotation.BooleanValue Then
                Call ConnectToBioDeep.OpenAdvancedFunction(Sub() Call MakeAnnotations(scanData))
            End If
            If prop.msLevel = 2 AndAlso Not formula.StringEmpty Then
                ' try to do peak annotations
                Dim f As Formula = FormulaScanner.ScanFormula(formula)
                Dim exactMass As Double = f.ExactMass
                Dim adducts As MzCalculator

                If Provider.ParseIonMode(prop.polarity) = IonModes.Positive Then
                    adducts = PrecursorType.FindPrecursorType(exactMass, prop.precursorMz, 1, "+", DAmethod.DeltaMass(0.3))
                Else
                    adducts = PrecursorType.FindPrecursorType(exactMass, prop.precursorMz, 1, "-", DAmethod.DeltaMass(0.3))
                End If

                scanData.ms2 = PeakAnnotation.DoPeakAnnotation(scanData, prop.precursorMz, adducts, f)
            End If

            Call showMatrix(scanData.ms2, scanId)

            Dim title1$
            Dim title2$

            If prop.msLevel = 1 Then
                title1 = $"MS1 Scan@{prop.retentionTime}sec"
                title2 = scanData.name
            Else
                title1 = $"M/Z {prop.precursorMz}, RT {prop.rtmin}min"
                title2 = scanData.name
            End If

            Call VisualStudio.ShowProperties(prop)
            Call PlotSpectrum(scanData)
            ' Call MyApplication.host.ShowPropertyWindow()
        Else
            Call missingCacheFile(raw)
        End If
    End Sub

    ''' <summary>
    ''' add metabolite annotation result based on ms1 peaks
    ''' </summary>
    ''' <param name="scanData">the ms1 scan peaksdata</param>
    Private Sub MakeAnnotations(scanData As LibraryMatrix)
        Dim mzdiff1 As Tolerance = Tolerance.DeltaMass(0.001)
        Dim mode As String = scanData.name.Match("[+-]")
        Dim kegg As MSJointConnection = TaskProgress.LoadData(
            streamLoad:=Function(echo As Action(Of String))
                            Return Globals.LoadKEGG(Sub(print)
                                                        MyApplication.LogText(print)
                                                        echo(print)
                                                    End Sub, If(mode = "+", 1, -1), mzdiff1)
                        End Function,
            info:="Load KEGG repository data..."
        )
        Dim anno As MzQuery() = kegg.SetAnnotation(scanData.mz)
        Dim mzdiff As Tolerance = Tolerance.DeltaMass(0.05)
        Dim compound As Compound

        For Each mzi As ms2 In scanData.ms2
            Dim hit As MzQuery = anno.Where(Function(d) mzdiff(d.mz, mzi.mz)).FirstOrDefault

            If Not hit.unique_id.StringEmpty Then
                compound = kegg.GetCompound(hit.unique_id)
                mzi.Annotation = $"{mzi.mz.ToString("F4")} {compound.commonNames.FirstOrDefault([default]:=hit.unique_id)}{hit.precursorType}"
            End If
        Next
    End Sub

    ''' <summary>
    ''' plot spectrum matrix
    ''' </summary>
    ''' <param name="scanData"></param>
    ''' <param name="focusOn">
    ''' show the canvas page after data plot is done?
    ''' </param>
    ''' <param name="nmr"></param>
    Public Sub PlotSpectrum(scanData As LibraryMatrix, Optional focusOn As Boolean = True, Optional nmr As Boolean = False)
        If scanData.ms2.All(Function(i) i.mz = 0.0) Then
            Call Workbench.Warning("Sorry, no valid m/z ion data...")
            Return
        End If
        If nmr Then
            _matrix = New NMRMatrix(scanData.name, scanData.ms2)
        Else
            _matrix = New SpectralMatrix(scanData.name, scanData.ms2, (scanData.parentMz, 0), "n/a")
        End If

        Call MyApplication.RegisterPlot(
              Sub(args)
                  PictureBox1.BackgroundImage = _matrix.SetName(args.title).Plot(args, PictureBox1.Size).AsGDIImage
              End Sub,
          width:=2100,
          height:=1200,
          padding:=If(nmr, "padding: 200px 50px 200px 50px;", "padding: 100px 30px 50px 100px;"),
          bg:="white",
          title:=scanData.name,
          xlab:=If(nmr, "ppm", "M/z ratio"),
          ylab:=If(nmr, "intensity", "Relative Intensity (%)")
      )

        If focusOn Then
            Call ShowTabPage(TabPage5)
        End If
    End Sub

    Friend Sub ShowMatrix(PDA As PDAPoint(), name As String)
        _matrix = New PDAMatrix(name, PDA)
        _matrix.LoadMatrix(DataGridView1, BindingSource1)
    End Sub

    Friend Sub ShowMatrix(UVscan As UVScanPoint(), name As String)
        _matrix = New UVScanMatrix(name, UVscan)
        _matrix.LoadMatrix(DataGridView1, BindingSource1)
    End Sub

    Friend Sub showUVscans(scans As IEnumerable(Of GeneralSignal), title$, xlable$)
        Dim scanCollection As GeneralSignal() = scans.ToArray

        Call MyApplication.RegisterPlot(
            Sub(args)
                PictureBox1.BackgroundImage = UVsignalPlot.Plot(
                    signals:=scanCollection,
                    legendTitle:=Function(scan) If(scanCollection.Length = 1, $"UV scans", scan("title")),
                    size:=$"{args.width},{args.height}",
                    pt_size:=args.point_size,
                    line_width:=args.line_width,
                    title:=args.title,
                    xlabel:=args.xlabel,
                    ylabel:=args.ylabel,
                    showLegend:=args.show_legend,
                    showGrid:=args.show_grid,
                    gridFill:=args.gridFill.ToHtmlColor
                ).AsGDIImage
            End Sub, width:=2560, height:=1440,
                     padding:="padding:125px 50px 150px 200px;",
                     bg:="white",
                     title:=title,
                     xlab:=xlable,
                     ylab:="intensity",
                     gridFill:="white")

        ShowTabPage(TabPage5)
    End Sub

    Public Sub showAlignment(result As AlignmentOutput, Optional showScore As Boolean = False)
        If result Is Nothing Then
            Return
        Else
            With result.GetAlignmentMirror
                Call showAlignment(.query, .ref, result, showScore)
            End With
        End If
    End Sub

    Public Sub showAlignment(query As LibraryMatrix, ref As LibraryMatrix, result As AlignmentOutput, showScore As Boolean)
        Dim prop As New AlignmentProperty(result)
        Dim alignName As String = If(
            showScore,
            $"Cos: [{result.forward.ToString("F3")}, {result.reverse.ToString("F3")}]",
            $"{query.name}_vs_{ref.name}"
        )

        MyApplication.host.ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active

        Call showMatrix(result.alignments, alignName)
        Call MyApplication.RegisterPlot(
            Sub(args)
                PictureBox1.BackgroundImage = MassSpectra.AlignMirrorPlot(
                    query:=query,
                    ref:=ref,
                    size:=$"{args.width},{args.height}",
                    title:=args.title,
                    drawLegend:=args.show_legend,
                    xlab:=args.xlabel,
                    ylab:=args.ylabel
                ).AsGDIImage
            End Sub,
            width:=1200,
            height:=800,
            padding:="padding: 100px 30px 50px 100px;",
            title:=alignName,
            xlab:="M/Z ratio",
            ylab:="Relative Intensity(%)"
        )

        Call VisualStudio.ShowProperties(prop)

        ShowTabPage(TabPage5)
    End Sub

    Private Function rawTIC(raw As MZWork.Raw, isBPC As Boolean) As NamedCollection(Of ChromatogramTick)
        Dim TIC As New NamedCollection(Of ChromatogramTick) With {
            .name = $"{If(isBPC, "BPC", "TIC")} [{raw.source.FileName}]",
            .value = raw.GetMs1Scans _
                .Select(Function(m)
                            Return New ChromatogramTick With {.Time = m.rt, .Intensity = If(isBPC, m.BPC, m.TIC)}
                        End Function) _
                .ToArray
        }

        TIC.value = {
                New ChromatogramTick With {.Time = raw.rtmin},
                New ChromatogramTick With {.Time = raw.rtmax}
            }.JoinIterates(TIC.value) _
             .OrderBy(Function(c) c.Time) _
             .ToArray

        Return TIC
    End Function

    Public Sub TIC(rawList As IEnumerable(Of MZWork.Raw), Optional isBPC As Boolean = False)
        Dim TICList As New List(Of NamedCollection(Of ChromatogramTick))

        For Each raw As MZWork.Raw In rawList
            TICList.Add(rawTIC(raw, isBPC))
        Next

        Call TIC(TICList.ToArray)
    End Sub

    Public Sub TIC(TICList As NamedCollection(Of ChromatogramTick)(),
                   Optional d3 As Boolean = False,
                   Optional xlab$ = "Time (s)",
                   Optional ylab$ = "Intensity")

        If TICList.IsNullOrEmpty Then
            Workbench.Warning("no chromatogram data!")
            Return
        End If
        If TICList.All(Function(file) file.All(Function(t) t.Intensity = 0.0)) Then
            Workbench.Warning("not able to create a TIC/BPC plot due to the reason of all of the tick intensity data is ZERO, please check your raw data file!")
            Return
        End If

        Dim signals As ChromatogramSerial() = TICList _
            .Select(Function(c)
                        Return New ChromatogramSerial(c.name, From ti In c Order By ti.Time)
                    End Function) _
            .ToArray

        _matrix = New ChromatogramOverlapMatrix("TIC/BPC chromatogram overlaps", signals, d3)
        _matrix.LoadMatrix(DataGridView1, BindingSource1)

        MyApplication.RegisterPlot(
            plot:=Sub(args)
                      PictureBox1.BackgroundImage = _matrix.Plot(args, PictureBox1.Size).AsGDIImage
                  End Sub,
            width:=1600,
            height:=1200,
            showGrid:=True,
            padding:="padding:100px 100px 150px 200px;",
            showLegend:=Not d3,
            xlab:=xlab,
            ylab:=ylab)

        MyApplication.host.ShowPage(Me)
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Sub ShowMRMTIC(name As String, ticks As ChromatogramTick())
        Dim xlab As String = "Time (s)"
        Dim ylab As String = "Intensity"

        _matrix = New ChromatogramMatrix(name, ticks)
        _matrix.LoadMatrix(DataGridView1, BindingSource1)

        MyApplication.RegisterPlot(
            plot:=Sub(args)
                      PictureBox1.BackgroundImage = _matrix.Plot(args, PictureBox1.Size).AsGDIImage
                  End Sub,
            width:=1600,
            height:=1200,
            showGrid:=True,
            padding:="padding:100px 100px 150px 200px;",
            showLegend:=True,
            xlab:=xlab,
            ylab:=ylab)

        MyApplication.host.ShowPage(Me)
    End Sub

    Public Sub TIC(isBPC As Boolean)
        Dim rawList As MZWork.Raw() = WindowModules.fileExplorer.GetSelectedRaws.ToArray

        If rawList.Length = 0 Then
            Workbench.Warning("No file data selected for TIC plot...")
        Else
            Call TIC(rawList, isBPC)
        End If
    End Sub

    Public Sub SaveImageToolStripMenuItem_Click()
        If Not PictureBox1.BackgroundImage Is Nothing Then
            Dim preFileName As String = _matrix.name.NormalizePathString(alphabetOnly:=False)

            Using file As New SaveFileDialog With {.Filter = "image(*.png)|*.png", .FileName = preFileName & ".png"}
                If file.ShowDialog = DialogResult.OK Then
                    Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
                    Call Process.Start(file.FileName)
                End If
            End Using
        Else
            Workbench.Warning("No plot image for save, please select one spectrum to start!")
        End If
    End Sub

    Public Sub ExportExactMassSearchTable()
        Call DataGridView1.SaveDataGrid($"Exact mass search result table export to [%s] successfully!")
    End Sub

    Public Sub SaveMatrixToolStripMenuItem_Click()
        If _matrix Is Nothing Then
            Workbench.Warning("No matrix data for save, please select one spectrum to start!")
            Return
        End If

        Using file As New SaveFileDialog() With {
            .Filter = "Excel Table(*.xls)|*.xls",
            .FileName = _matrix.name.NormalizePathString(False)
        }
            If file.ShowDialog = DialogResult.OK Then
                Dim flag As Boolean = _matrix.SaveTo(file.FileName)

                If Not flag Then
                    Call Workbench.Warning($"the save matrix not success or method has not yet been implemented for data type:{_matrix.UnderlyingType.FullName}")
                End If
            End If
        End Using
    End Sub

    ''' <summary>
    ''' load data and then run clustering
    ''' </summary>
    ''' <param name="progress"></param>
    ''' <param name="similarityCutoff"></param>
    Friend Sub MolecularNetworkingTool(progress As ITaskProgress, similarityCutoff As Double, getSpectrum As Func(Of Action(Of String), IEnumerable(Of PeakMs2)))
        Call Thread.Sleep(1000)

        Call progress.SetTitle("Load Scan data")
        Call progress.SetInfo("loading cache ms2 scan data...")

        Dim raw As PeakMs2() = getSpectrum(AddressOf progress.SetTitle).ToArray

        If raw.Length = 0 Then
            Workbench.Warning("No spectrum data, please select a file or some spectrum...")
        Else
            Call raw.MolecularNetworkingTool(progress, similarityCutoff)
        End If
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If e.ColumnIndex = Scan0 AndAlso e.RowIndex >= 0 Then
            Dim scanId As String = DataGridView1.Rows(e.RowIndex).Cells(0).Value?.ToString

            If Not scanId.StringEmpty Then
                ' Call showSpectrum(scanId, TreeView1.CurrentRawFile.raw)
            End If
        End If
    End Sub

    ''' <summary>
    ''' View spectral plot and then switch to the spectrl viewer UI
    ''' </summary>
    ''' <param name="data"></param>
    Public Shared Sub ShowSpectral(data As Object)
        Dim matrix As ms2()
        Dim name As String

        If data Is Nothing Then
            Call Workbench.Warning("the given spectral data is nothing!")
            Return
        ElseIf TypeOf data Is LibraryMatrix Then
            With DirectCast(data, LibraryMatrix)
                matrix = .ms2
                name = .name
            End With
        ElseIf TypeOf data Is PeakMs2 Then
            With DirectCast(data, PeakMs2)
                matrix = .mzInto
                name = .lib_guid
            End With
        Else
            Call Workbench.Warning($"the spectral view for {data.GetType.FullName} is not implements yet...")
            Return
        End If

        Call MyApplication.host.mzkitTool.PlotSpectrum(New LibraryMatrix(matrix) With {.name = name})
        Call MyApplication.host.mzkitTool.showMatrix(matrix, name)
        Call MyApplication.host.ShowMzkitToolkit()
    End Sub

    Sub ShowExpressionMatrix(expr As Dictionary(Of String, Double()), n As Integer, name As String)
        _matrix = New ExpressionMatrix(name, n, expr)
        _matrix.LoadMatrix(DataGridView1, BindingSource1)
    End Sub

    ''' <summary>
    ''' load matrix data into the data grid
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="name"></param>
    Sub showMatrix(matrix As ms2(), name As String, Optional nmr As Boolean = False)
        If nmr Then
            _matrix = New NMRMatrix(name, matrix)
        Else
            _matrix = New SpectralMatrix(name, matrix, Nothing, "n/a")
        End If

        _matrix.LoadMatrix(DataGridView1, BindingSource1)
    End Sub

    Sub showMatrix(matrix As SSM2MatrixFragment(), name As String)
        _matrix = New MSAlignmentMatrix(name, matrix)
        _matrix.LoadMatrix(DataGridView1, BindingSource1)
    End Sub

    Public Sub showMatrix(matrix As ChromatogramTick(), name As String)
        _matrix = New ChromatogramMatrix(name, matrix)
        _matrix.LoadMatrix(DataGridView1, BindingSource1)
    End Sub

    Public Sub ShowXIC(ppm As Double,
                       plotTIC As NamedCollection(Of ChromatogramTick),
                       getXICCollection As Func(Of Double, IEnumerable(Of NamedCollection(Of ChromatogramTick))),
                       maxY As Double)

        Dim plotImage As Image = Nothing
        Dim relative As Boolean = relativeInto()
        Dim XICPlot As New List(Of NamedCollection(Of ChromatogramTick))

        Call ProgressSpinner.DoLoading(
            Sub()
                If Not plotTIC.IsEmpty Then
                    XICPlot.Add(plotTIC)
                End If

                XICPlot.AddRange(getXICCollection(ppm))
            End Sub)

        If XICPlot.Count = 0 Then
            Call Workbench.Warning("No XIC ions data for generate plot!")
        Else
            Call TIC(XICPlot, d3:=False)
        End If
    End Sub

    ''' <summary>
    ''' get ms2 peaklist from the raw data file
    ''' </summary>
    ''' <param name="progress"></param>
    ''' <returns></returns>
    Friend Iterator Function getSelectedIonSpectrums(progress As Action(Of String)) As IEnumerable(Of PeakMs2)
        Dim raw = WindowModules.rawFeaturesList.CurrentOpenedFile

        For Each ionNode As TreeNode In WindowModules.rawFeaturesList.GetSelectedNodes.Where(Function(a) TypeOf a.Tag Is ScanMS2)
            Dim scanId As String = ionNode.Text
            Dim info As ScanMS2 = ionNode.Tag
            Dim guid As String = $"{raw.source.FileName}#{scanId}"

            If Not progress Is Nothing Then
                Call progress(guid)
            End If

            Yield GetMs2Peak(info, raw)
        Next
    End Function

    Public Shared Function GetMs2Peak(info As ScanMS2, raw As Raw) As PeakMs2
        Dim scanId As String = info.scan_id
        Dim guid As String = $"{raw.source.FileName}#{scanId}"

        Return New PeakMs2 With {
            .mz = info.parentMz,
            .scan = info.scan_id,
            .activation = info.activationMethod,
            .collisionEnergy = info.collisionEnergy,
            .file = raw.source.FileName,
            .lib_guid = guid,
            .mzInto = info.GetMs.ToArray,
            .precursor_type = "n/a",
            .rt = info.rt,
            .intensity = info.intensity
        }
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function relativeInto() As Boolean
        Return MyApplication.host.ribbonItems.CheckBoxXICRelative.BooleanValue
    End Function

    'Private Sub AddToolStripMenuItem_Click(sender As Object, e As EventArgs)
    '    Dim XIC = getXICMatrix(TreeView1.CurrentRawFile.raw)

    '    If Not XIC.IsEmpty Then
    '        XICCollection.Add(XIC)
    '    End If
    'End Sub

    Private Sub ClearToolStripMenuItem_Click(sender As Object, e As EventArgs)
        ' XICCollection.Clear()
        ' MyApplication.host.fileExplorer.Clear()
        '  MyApplication.host.fileExplorer.ClearToolStripMenuItem.Text = "Clear"
    End Sub

    Private Sub PictureBox1_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox1.DoubleClick
        If Not PictureBox1.BackgroundImage Is Nothing Then
            Dim temp As String = TempFileSystem.GetAppSysTempFile(".png", App.PID, "imagePlot_")

            Call PictureBox1.BackgroundImage.SaveAs(temp)
            Call Process.Start(temp)
        End If
    End Sub

    Private Sub PictureBox1_MouseClick(sender As Object, e As MouseEventArgs) Handles PictureBox1.MouseClick
        If e.Button = MouseButtons.Right Then
            Dim p As Point = PictureBox1.PointToScreen(e.Location)
            MyApplication.host.Ribbon1.ShowContextPopup(CUInt(RibbonItems.cmdContextMap), p.X, p.Y)
        End If
    End Sub

    Private Sub CustomTabControl1_TabClosing(sender As Object, e As TabControlCancelEventArgs) Handles CustomTabControl1.TabClosing
        e.Cancel = True

        If CustomTabControl1.Controls.Count = 1 Then
            If e.TabPage Is TabPage5 Then

            Else
                CustomTabControl1.Controls.Clear()
                ShowTabPage(TabPage5)
            End If
        Else
            CustomTabControl1.Controls.Remove(e.TabPage)
            e.TabPage.Hide()
        End If
    End Sub

    Public Sub ShowTabPage(tabpage As TabPage)
        If Not CustomTabControl1.Controls.Contains(tabpage) Then
            CustomTabControl1.Controls.Add(tabpage)
        End If

        WindowModules.panelMain.Show(MyApplication.host.m_dockPanel)

        CustomTabControl1.SelectedTab = tabpage
        tabpage.Visible = True
    End Sub

    Public Sub ShowPlotTweaks()
        ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active
        VisualStudio.Dock(WindowModules.plotParams, DockState.DockRight)
    End Sub

    Private Sub PageMzkitTools_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        With Size
            WindowModules.plotParams.params.width = .Width
            WindowModules.plotParams.params.height = .Height
        End With
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        Call DataGridView1.SaveDataGrid("Export Matrix")
    End Sub

    Private Sub OpenInTableViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenInTableViewerToolStripMenuItem.Click
        Call DataGridView1.OpenInTableViewer
    End Sub
End Class
