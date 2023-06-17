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

Imports System.Threading
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.MoleculeNetworking
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra.Xml
Imports BioNovoGene.Analytical.MassSpectrometry.Math.UV
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.mzkit_win32.My
Imports BioNovoGene.mzkit_win32.RibbonLib.Controls
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Contour
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic.Canvas
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.DataMining.KMeans
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.MarchingSquares
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.SignalProcessing
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports mzblender
Imports Mzkit_win32.BasicMDIForm
Imports RibbonLib
Imports RibbonLib.Interop
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.bGetObject
Imports Task
Imports WeifenLuo.WinFormsUI.Docking
Imports stdNum = System.Math

Public Class PageMzkitTools

    Friend matrix As Array
    Friend matrixName As String

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
        Else
            Dim colorSet As String
            Dim data As ContourLayer() = Nothing
            Dim width As Integer = 2048
            Dim height As Integer = 1600
            Dim padding As String = "padding:100px 400px 100px 100px;"

            If XIC Then
                colorSet = "YlGnBu:c8"
                width = 2400
            ElseIf contour Then
                colorSet = "Jet"

                Call ProgressSpinner.DoLoading(
                    Sub()
                        data = raw.GetContourData
                    End Sub)

                width = 3600
                height = 2700
                padding = "padding:100px 750px 100px 100px;"
            Else
                colorSet = "darkblue,blue,skyblue,green,orange,red,darkred"
            End If

            Call MyApplication.RegisterPlot(
                Sub(args)
                    Call ProgressSpinner.DoLoading(Sub()
                                                       Dim image As Image

                                                       If contour Then
                                                           image = data.Plot(
                                    size:=$"{args.width},{args.height}",
                                    padding:=args.GetPadding.ToString,
                                    colorSet:=args.GetColorSetName,
                                    ppi:=200,
                                    legendTitle:=args.legend_title
                                ).AsGDIImage
                                                       ElseIf XIC Then
                                                           image = raw.Draw3DPeaks(colorSet:=args.GetColorSetName, size:=$"{args.width},{args.height}", args.GetPadding.ToString)
                                                       Else
                                                           image = raw.DrawScatter(colorSet:=args.GetColorSetName)
                                                       End If

                                                       Me.Invoke(Sub() PictureBox1.BackgroundImage = image)
                                                   End Sub)

                End Sub, colorSet:=colorSet, width:=width, height:=height, padding:=padding, legendTitle:="Levels")
        End If

        Me.matrixName = $"{raw.source.FileName}_{If(XIC, "XICPeaks", "rawscatter_2D")}"

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

    Dim currentMatrix As [Variant](Of ms2(), ChromatogramTick())

    Friend Sub showSpectrum(scanId As String, raw As MZWork.Raw)
        If raw.cacheFileExists OrElse raw.isInMemory Then
            Dim prop As SpectrumProperty = Nothing
            Dim showAnnotation As Boolean = RibbonEvents.ribbonItems.CheckBoxShowMs2Fragment.BooleanValue
            Dim scanData As LibraryMatrix = raw.GetSpectrum(scanId, Globals.Settings.viewer.GetMethod, Sub(src, cache) frmFileExplorer.getRawCache(src,, cache), showAnnotation, prop)

            If prop.msLevel = 1 AndAlso ribbonItems.CheckBoxShowKEGGAnnotation.BooleanValue Then
                Call ConnectToBioDeep.OpenAdvancedFunction(
                    Sub()
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
                    End Sub)
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
    ''' plot spectrum matrix
    ''' </summary>
    ''' <param name="scanData"></param>
    ''' <param name="focusOn">
    ''' show the canvas page after data plot is done?
    ''' </param>
    ''' <param name="nmr"></param>
    Public Sub PlotSpectrum(scanData As LibraryMatrix, Optional focusOn As Boolean = True, Optional nmr As Boolean = False)
        Call MyApplication.RegisterPlot(
              Sub(args)
                  scanData.name = args.title

                  If nmr Then
                      PictureBox1.BackgroundImage = New NMRSpectrum(scanData, args.GetTheme) With {
                        .main = args.title
                      } _
                         .Plot(New Size(args.width, args.height), dpi:=150) _
                         .AsGDIImage
                  Else
                      PictureBox1.BackgroundImage = PeakAssign.DrawSpectrumPeaks(
                        scanData,
                        padding:=args.GetPadding.ToString,
                        bg:=args.background.ToHtmlColor,
                        size:=$"{args.width},{args.height}",
                        labelIntensity:=If(args.show_tag, 0.25, 100),
                        gridFill:=args.gridFill.ToHtmlColor,
                        barStroke:=$"stroke: steelblue; stroke-width: {args.line_width}px; stroke-dash: solid;"
                    ) _
                    .AsGDIImage
                  End If
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

    Public Sub PlotMatrix(title1$, title2$, scanData As LibraryMatrix, Optional focusOn As Boolean = True)
        Call MyApplication.RegisterPlot(
            Sub(args)
                PictureBox1.BackgroundImage = scanData _
                    .MirrorPlot(
                        titles:={title1, title2},
                        margin:=args.GetPadding.ToString,
                        drawLegend:=args.show_legend,
                        bg:=args.background.ToHtmlColor,
                        plotTitle:=args.title,
                        size:=$"{args.width},{args.height}"
                    ) _
                    .AsGDIImage
            End Sub,
            width:=1200,
            height:=800,
            padding:="padding: 100px 30px 50px 100px;",
            bg:="white",
            title:="BioDeep™ MS/MS alignment Viewer"
        )

        If focusOn Then
            Call ShowTabPage(TabPage5)
        End If
    End Sub

    Friend Sub ShowMatrix(PDA As PDAPoint(), name As String)
        Me.matrix = PDA
        Me.matrixName = name

        Dim memoryData As New DataSet
        Dim table As DataTable = memoryData.Tables.Add("memoryData")

        Try
            Call DataGridView1.Columns.Clear()
            Call DataGridView1.Rows.Clear()
        Catch ex As Exception

        End Try

        table.Columns.Add("scan_time", GetType(Double))
        table.Columns.Add("total_ion", GetType(Double))
        table.Columns.Add("relative", GetType(Double))

        Dim max As Double = PDA.Select(Function(a) a.total_ion).Max

        For Each tick As PDAPoint In PDA
            table.Rows.Add(tick.scan_time, tick.total_ion, tick.total_ion / max * 100)
        Next

        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName
        DataGridView1.DataSource = BindingSource1
    End Sub

    Friend Sub ShowMatrix(UVscan As UVScanPoint(), name As String)
        Me.matrix = UVscan
        Me.matrixName = name

        Dim memoryData As New DataSet
        Dim table As DataTable = memoryData.Tables.Add("memoryData")

        Try
            Call DataGridView1.Columns.Clear()
            Call DataGridView1.Rows.Clear()
        Catch ex As Exception

        End Try

        table.Columns.Add("wavelength(nm)", GetType(Double))
        table.Columns.Add("intensity", GetType(Double))
        table.Columns.Add("relative", GetType(Double))

        Dim max As Double = UVscan.Select(Function(a) a.intensity).Max

        For Each tick As UVScanPoint In UVscan
            table.Rows.Add(tick.wavelength, tick.intensity, tick.intensity / max * 100)
        Next

        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName
        DataGridView1.DataSource = BindingSource1
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
        Dim alignName As String = If(showScore, $"Cos: [{result.forward.ToString("F3")}, {result.reverse.ToString("F3")}]", $"{query.name}_vs_{ref.name}")

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

    Public Sub TIC(TICList As NamedCollection(Of ChromatogramTick)(), Optional d3 As Boolean = False, Optional xlab$ = "Time (s)", Optional ylab$ = "Intensity")
        If TICList.IsNullOrEmpty Then
            MyApplication.host.showStatusMessage("no chromatogram data!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        ElseIf TICList.All(Function(file) file.All(Function(t) t.Intensity = 0.0)) Then
            MyApplication.host.showStatusMessage("not able to create a TIC/BPC plot due to the reason of all of the tick intensity data is ZERO, please check your raw data file!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return
        End If

        Dim blender As Blender

        If d3 Then
            blender = New XIC3DBlender(TICList)
        Else
            blender = New ChromatogramBlender(TICList)
        End If

        Call showMatrix(TICList(Scan0).value, TICList(Scan0).name)

        MyApplication.RegisterPlot(
            Sub(args)
                PictureBox1.BackgroundImage = blender.Rendering(args, PictureBox1.Size)
            End Sub, width:=1600, height:=1200, showGrid:=True, padding:="padding:100px 100px 150px 200px;", showLegend:=Not d3, xlab:=xlab, ylab:=ylab)

        MyApplication.host.ShowPage(Me)
    End Sub

    Public Sub ShowMRMTIC(name As String, ticks As ChromatogramTick())
        Call TIC({New NamedCollection(Of ChromatogramTick)(name, ticks)})
    End Sub

    Public Sub TIC(isBPC As Boolean)
        Dim rawList As MZWork.Raw() = WindowModules.fileExplorer.GetSelectedRaws.ToArray

        If rawList.Length = 0 Then
            MyApplication.host.showStatusMessage("No file data selected for TIC plot...")
        Else
            Call TIC(rawList, isBPC)
        End If
    End Sub

    Public Sub SaveImageToolStripMenuItem_Click()
        If Not PictureBox1.BackgroundImage Is Nothing Then
            Dim preFileName As String = matrixName.NormalizePathString(alphabetOnly:=False)

            Using file As New SaveFileDialog With {.Filter = "image(*.png)|*.png", .FileName = preFileName & ".png"}
                If file.ShowDialog = DialogResult.OK Then
                    Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
                    Call Process.Start(file.FileName)
                End If
            End Using
        Else
            MyApplication.host.showStatusMessage("No plot image for save, please select one spectrum to start!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        End If
    End Sub

    Public Sub ExportExactMassSearchTable()
        Call DataGridView1.SaveDataGrid($"Exact mass search result table export to [%s] successfully!")
    End Sub

    Public Sub SaveMatrixToolStripMenuItem_Click()
        If matrix Is Nothing Then
            MyApplication.host.showStatusMessage("No matrix data for save, please select one spectrum to start!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Using file As New SaveFileDialog() With {.Filter = "Excel Table(*.xls)|*.xls", .FileName = matrixName.NormalizePathString(False)}
                If file.ShowDialog = DialogResult.OK Then

                    Select Case matrix.GetType
                        Case GetType(ms2()) : Call DirectCast(matrix, ms2()).SaveTo(file.FileName)
                        Case GetType(ChromatogramTick()) : Call DirectCast(matrix, ChromatogramTick()).SaveTo(file.FileName)
                        Case GetType(SSM2MatrixFragment()) : Call DirectCast(matrix, SSM2MatrixFragment()).SaveTo(file.FileName)
                        Case GetType(PDAPoint()) : Call DirectCast(matrix, PDAPoint()).SaveTo(file.FileName)
                        Case GetType(UVScanPoint()) : Call DirectCast(matrix, UVScanPoint()).SaveTo(file.FileName)

                        Case Else
                            Throw New NotImplementedException
                    End Select

                End If
            End Using
        End If
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
            MyApplication.host.showStatusMessage("No spectrum data, please select a file or some spectrum...", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call MolecularNetworkingTool(raw, progress, similarityCutoff)
        End If
    End Sub

    Friend Sub MolecularNetworkingTool(raw As PeakMs2(), progress As ITaskProgress, similarityCutoff As Double)
        Dim protocol As New Protocols(
            ms1_tolerance:=Tolerance.PPM(15),
            ms2_tolerance:=Tolerance.DeltaMass(0.3),
            treeIdentical:=Globals.Settings.network.treeNodeIdentical,
            treeSimilar:=Globals.Settings.network.treeNodeSimilar,
            intoCutoff:=Globals.Settings.viewer.GetMethod
        )
        Dim progressMsg As Action(Of String) = AddressOf progress.SetTitle

        ' filter empty spectrum
        raw = (From r As PeakMs2 In raw Where Not r.mzInto.IsNullOrEmpty).ToArray

        'Dim run As New List(Of PeakMs2)
        'Dim nodes As New Dictionary(Of String, ScanEntry)
        'Dim idList As New Dictionary(Of String, Integer)



        'Using cache As New netCDFReader(raw.cache)
        '    For Each scan In raw.scans.Where(Function(s) s.mz > 0)
        '        Dim uid As String = $"M{CInt(scan.mz)}T{CInt(scan.rt)}"

        '        If idList.ContainsKey(uid) Then
        '            idList(uid) += 1
        '            uid = uid & "_" & (idList(uid) - 1)
        '        Else
        '            idList.Add(uid, 1)
        '        End If

        '        run += New PeakMs2 With {
        '            .rt = scan.rt,
        '            .mz = scan.mz,
        '            .lib_guid = uid,
        '            .mzInto = cache.getDataVariable(scan.id).numerics.AsMs2.ToArray.Centroid(Tolerance.DeltaMass(0.3)).ToArray
        '        }

        '        progress.Invoke(Sub() progress.ShowProgressTitle (scan.id)
        '        nodes.Add(run.Last.lib_guid, scan)
        '    Next
        'End Using

        progress.SetTitle("run molecular networking....")

        ' Call tree.doCluster(run)
        Dim links = protocol.RunProtocol(raw, progressMsg).ProduceNodes.Networking.ToArray
        Dim net As IO.DataSet() = ProtocolPipeline _
            .Networking(Of IO.DataSet)(links, Function(a, b) stdNum.Min(a, b)) _
            .ToArray

        progress.SetTitle("run family clustering....")

        If net.Length < 3 Then
            Call Workbench.Warning("the ions data is not enough for create network!")
        Else
            Dim kn As Integer

            If net.Length > 9 Then
                kn = 9
            Else
                kn = CInt(net.Length / 2)
            End If

            Dim clusters = net.ToKMeansModels.Kmeans(expected:=kn, debug:=False)
            Dim rawLinks = links.ToDictionary(Function(a) a.reference, Function(a) a)

            progress.SetInfo("initialize result output...")

            MyApplication.host.Invoke(
                Sub()
                    Call MyApplication.host.mzkitMNtools.loadNetwork(clusters, protocol, rawLinks, similarityCutoff)
                    Call MyApplication.host.ShowPage(MyApplication.host.mzkitMNtools)
                End Sub)
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

    ''' <summary>
    ''' load matrix data into the data grid
    ''' </summary>
    ''' <param name="matrix"></param>
    ''' <param name="name"></param>
    Sub showMatrix(matrix As ms2(), name As String, Optional nmr As Boolean = False)
        Dim memoryData As New DataSet
        Dim table As DataTable = memoryData.Tables.Add("memoryData")

        Me.matrix = matrix
        Me.matrixName = name

        Try
            Call DataGridView1.Columns.Clear()
            Call DataGridView1.Rows.Clear()
        Catch ex As Exception

        End Try

        If nmr Then
            table.Columns.Add("ppm", GetType(Double))
            table.Columns.Add("intensity", GetType(Double))

            For Each tick As ms2 In matrix
                table.Rows.Add(tick.mz, tick.intensity)
                System.Windows.Forms.Application.DoEvents()
            Next
        Else
            Dim max As Double

            If matrix.Length = 0 Then
                max = 0
                Call Workbench.Warning($"'{name}' didn't contains any data...")
            Else
                max = matrix.Select(Function(a) a.intensity).Max
            End If

            table.Columns.Add("m/z", GetType(Double))
            table.Columns.Add("intensity", GetType(Double))
            table.Columns.Add("relative", GetType(Double))
            table.Columns.Add("annotation", GetType(String))

            For Each tick As ms2 In matrix
                table.Rows.Add(tick.mz, tick.intensity, CInt(tick.intensity / max * 100), tick.Annotation)
                System.Windows.Forms.Application.DoEvents()
            Next
        End If

        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName
        DataGridView1.DataSource = BindingSource1
    End Sub

    Sub showMatrix(matrix As SSM2MatrixFragment(), name As String)
        Me.matrix = matrix
        Me.matrixName = name

        Dim memoryData As New DataSet
        Dim table As DataTable = memoryData.Tables.Add("memoryData")

        Try
            Call DataGridView1.Columns.Clear()
            Call DataGridView1.Rows.Clear()
        Catch ex As Exception

        End Try

        table.Columns.Add("m/z", GetType(Double))
        table.Columns.Add("intensity(query)", GetType(Double))
        table.Columns.Add("intensity(target)", GetType(Double))
        table.Columns.Add("tolerance", GetType(Double))

        For Each tick In matrix
            table.Rows.Add(tick.mz, tick.query, tick.ref, tick.da)
        Next

        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName
        DataGridView1.DataSource = BindingSource1
    End Sub

    Public Sub showMatrix(matrix As ChromatogramTick(), name As String)
        Me.matrix = matrix
        Me.matrixName = name

        Dim memoryData As New DataSet
        Dim table As DataTable = memoryData.Tables.Add("memoryData")

        Try
            Call DataGridView1.Columns.Clear()
            Call DataGridView1.Rows.Clear()
        Catch ex As Exception

        End Try

        table.Columns.Add("time", GetType(Double))
        table.Columns.Add("intensity", GetType(Double))

        For Each tick In matrix
            table.Rows.Add(tick.Time, tick.Intensity)
        Next

        BindingSource1.DataSource = memoryData
        BindingSource1.DataMember = table.TableName
        DataGridView1.DataSource = BindingSource1
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

    Private Function relativeInto() As Boolean
        Return MyApplication.host.ribbonItems.CheckBoxXICRelative.BooleanValue
    End Function

    ''' <summary>
    ''' get XIC Chromatogram collection
    ''' </summary>
    ''' <param name="raw"></param>
    ''' <param name="scanId"></param>
    ''' <param name="ppm"></param>
    ''' <param name="relativeInto"></param>
    ''' <returns></returns>
    Friend Function getXICMatrix(raw As MZWork.Raw, scanId As String, ppm As Double, relativeInto As Boolean) As NamedCollection(Of ChromatogramTick)
        Dim ms2 As ScanMS2 = raw.FindMs2Scan(scanId)
        Dim name As String = raw.source.FileName
        Dim ms1 As ScanMS1 = raw.GetMs1Scans _
            .Where(Function(scan1)
                       Return scan1.products _
                           .SafeQuery _
                           .Any(Function(a) a.scan_id = scanId)
                   End Function) _
            .FirstOrDefault

        If ms2 Is Nothing OrElse ms2.parentMz = 0.0 Then
            MyApplication.host.showStatusMessage("XIC plot is not avaliable for MS1 parent scan!", My.Resources.StatusAnnotations_Warning_32xLG_color)
            Return Nothing
        Else
            MyApplication.host.showStatusMessage(name, Nothing)
        End If

        Dim parentMz As Double = ms1.mz _
            .Select(Function(mzi) (PPMmethod.PPM(mzi, ms2.parentMz), mzi)) _
            .OrderBy(Function(a) a.Item1) _
            .First _
            .mzi
        Dim mzdiff As Tolerance = PPMmethod.PPM(ppm)
        Dim XIC As ChromatogramTick() = raw _
            .GetMs1Scans _
            .Select(Function(a)
                        Return New ChromatogramTick With {
                            .Time = a.rt,
                            .Intensity = a.GetIntensity(ms2.parentMz, mzdiff)
                        }
                    End Function) _
            .ToArray

        name = $"XIC [m/z={parentMz.ToString("F4")}]"

        If Not relativeInto Then
            XIC = {
                  New ChromatogramTick With {.Time = raw.rtmin},
                  New ChromatogramTick With {.Time = raw.rtmax}
              }.JoinIterates(XIC) _
               .OrderBy(Function(c) c.Time) _
               .ToArray
        End If

        Dim plotTIC As New NamedCollection(Of ChromatogramTick) With {
            .name = name,
            .value = XIC,
            .description = ms2.parentMz & " " & raw.source.FileName
        }

        Return plotTIC
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
        Call DataControlHandler.OpenInTableViewer(matrix:=DataGridView1)
    End Sub
End Class
