#Region "Microsoft.VisualBasic::c37a95dbbd1d71a59ccc9f66e0157503, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmFeatureSearch.vb"

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

'   Total Lines: 424
'    Code Lines: 315
' Comment Lines: 28
'   Blank Lines: 81
'     File Size: 17.52 KB


' Class frmFeatureSearch
' 
'     Properties: FilePath, MimeType
' 
'     Function: (+2 Overloads) Save
' 
'     Sub: (+2 Overloads) AddFileMatch, ApplyFeatureFilterToolStripMenuItem_Click, frmFeatureSearch_Load, RunMs2ClusteringToolStripMenuItem_Click, ViewToolStripMenuItem_Click
'          ViewXICToolStripMenuItem_Click
' 
' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Windows.Forms.ListViewItem
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.ASCII.MGF
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.BioDeep.MSFinder
Imports BioNovoGene.mzkit_win32.MSdata
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports Mzkit_win32.LCMSViewer
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports any = Microsoft.VisualBasic.Scripting
Imports std = System.Math

''' <summary>
''' view feature search result
''' </summary>
Public Class frmFeatureSearch : Implements ISaveHandle, IFileReference

    Dim appendHeader As Boolean = False

    ''' <summary>
    ''' raw source list for ms1 search
    ''' </summary>
    Dim list1 As New List(Of (File As String, matches As ParentMatch()))
    ''' <summary>
    ''' raw source list for ms2 search
    ''' </summary>
    Dim list2 As New List(Of (file As String, targetMz As Double, matches As ScanMS2()))
    Dim rangeMin As Double = 999999999
    Dim rangeMax As Double = -99999999999999
    Dim adducts As New Dictionary(Of String, List(Of XICFeatureViewer))

    Friend formula As String = Nothing

    Public Sub AddEachFileMatch(addMatch As Action(Of Raw))
        For Each file As Raw In directRaw
            Call addMatch(file)
        Next
    End Sub

    Public Sub AddFileMatch(file As String, matches As ParentMatch(), Optional all_adducts As Dictionary(Of String, Double) = Nothing)
        list1.Add((file, matches))

        If Not appendHeader Then
            Dim matchHeaders = {
                New ColumnHeader() With {.Text = "Precursor Type"},
                New ColumnHeader() With {.Text = "Adducts"},
                New ColumnHeader() With {.Text = "M"}
            }

            Me.TreeListView1.Columns.AddRange(matchHeaders)
            Me.appendHeader = True
        End If

        Dim row As New TreeListViewItem With {.Text = file.FileName, .ImageIndex = 0, .ToolTipText = file}
        Dim i As i32 = 1

        For Each member As ParentMatch In matches
            Dim ion As New TreeListViewItem(member.scan.Identity) With {.ImageIndex = 1, .ToolTipText = member.scan.intensity, .Tag = member}

            ion.SubItems.Add(New ListViewSubItem With {.Text = $"#{++i}"})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.scan.mz.ToString("F4")})
            ion.SubItems.Add(New ListViewSubItem With {.Text = CInt(member.scan.rt)})
            ion.SubItems.Add(New ListViewSubItem With {.Text = (member.scan.rt / 60).ToString("F1")})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.da})
            ion.SubItems.Add(New ListViewSubItem With {.Text = CInt(member.ppm)})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.scan.Polarity})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.scan.Charge})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.BPC.ToString("G3")})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.TIC.ToString("G3")})

            ion.SubItems.Add(New ListViewSubItem With {.Text = member.precursor_type})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.adducts})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.M})

            If rangeMin > member.scan.rt Then
                rangeMin = member.scan.rt
            End If
            If rangeMax < member.scan.rt Then
                rangeMax = member.scan.rt
            End If

            Call row.Items.Add(ion)
            Call System.Windows.Forms.Application.DoEvents()
        Next

        row.SubItems.Add(New ListViewSubItem With {.Text = matches.Length})

        TreeListView1.Items.Add(row)

        If Not directRaw.IsNullOrEmpty Then
            Dim raw As Raw = directRaw.Where(Function(r) r.source = file).FirstOrDefault

            If Not raw Is Nothing Then
                Dim rt_range As New DoubleRange(raw.GetMs1Scans.Select(Function(s1) s1.rt))
                Dim da As Tolerance = Tolerance.DeltaMass(0.05)
                Dim current_matches = matches _
                    .GroupBy(Function(m) m.precursor_type) _
                    .ToDictionary(Function(a) a.Key,
                                  Function(a)
                                      Return a.ToArray
                                  End Function)

                If all_adducts.IsNullOrEmpty Then
                    all_adducts = current_matches _
                        .ToDictionary(Function(a) a.Key,
                                      Function(a)
                                          Return Aggregate ion As ParentMatch
                                                 In a.Value
                                                 Into Average(ion.scan.mz)
                                      End Function)
                End If

                For Each ion_group As KeyValuePair(Of String, Double) In all_adducts
                    Dim mz As Double = ion_group.Value   '
                    Dim xic = GetXIC(mz, raw, da)
                    Dim viewer As New XICFeatureViewer
                    Dim source As String() = {file.FileName, ion_group.Key, $"m/z: {mz.ToString("F4")}"}
                    Dim spectrum As PeakMs2() = Nothing

                    If current_matches.ContainsKey(ion_group.Key) Then
                        spectrum = current_matches(ion_group.Key) _
                            .Select(Function(ion) ion.ToMs2) _
                            .ToArray
                    End If

                    viewer.SetFeatures(source, xic.value, spectrum, rt_range)
                    viewer.Width = FlowLayoutPanel1.Width * 0.95

                    Call FlowLayoutPanel1.Controls.Add(viewer)
                    Call System.Windows.Forms.Application.DoEvents()

                    If multipleMode Then
                        If Not adducts.ContainsKey(ion_group.Key) Then
                            Call adducts.Add(ion_group.Key, New List(Of XICFeatureViewer))
                        End If

                        Call adducts(ion_group.Key).Add(viewer)
                    End If

                    AddHandler viewer.ViewSpectrum,
                        Sub(spec)
                            Call MyApplication.host.mzkitTool.PlotSpectrum(New LibraryMatrix(spec.lib_guid, spec.mzInto))
                            Call MyApplication.host.mzkitTool.ShowPage()
                        End Sub
                Next
            End If
        End If
    End Sub

    Public Sub LoadAdducts()
        Call ToolStripComboBox1.Items.Clear()

        For Each name As String In adducts.Keys
            Call ToolStripComboBox1.Items.Add(name)
        Next
    End Sub

    Public Sub AddFileMatch(file As String, targetMz As Double, matches As ScanMS2())
        Dim row As New TreeListViewItem With {.Text = file.FileName, .ImageIndex = 0, .ToolTipText = file}
        Dim i As i32 = 1

        list2.Add((file, targetMz, matches))

        For Each member As ScanMS2 In matches
            Dim ion As New TreeListViewItem(member.scan_id) With {.ImageIndex = 1, .ToolTipText = member.scan_id, .Tag = member}

            ion.SubItems.Add(New ListViewSubItem With {.Text = $"#{++i}"})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.parentMz.ToString("F4")})
            ion.SubItems.Add(New ListViewSubItem With {.Text = CInt(member.rt)})
            ion.SubItems.Add(New ListViewSubItem With {.Text = (member.rt / 60).ToString("F1")})
            ion.SubItems.Add(New ListViewSubItem With {.Text = std.Round(std.Abs(member.parentMz - targetMz), 3)})
            ion.SubItems.Add(New ListViewSubItem With {.Text = CInt(PPMmethod.PPM(member.parentMz, targetMz))})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.polarity})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.charge})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.into.Max.ToString("G3")})
            ion.SubItems.Add(New ListViewSubItem With {.Text = member.into.Sum.ToString("G3")})

            If rangeMin > member.rt Then
                rangeMin = member.rt
            End If
            If rangeMax < member.rt Then
                rangeMax = member.rt
            End If

            row.Items.Add(ion)
        Next

        row.SubItems.Add(New ListViewSubItem With {.Text = matches.Length})

        TreeListView1.Items.Add(row)
    End Sub

    Friend directRaw As Raw()

    Public Property FilePath As String Implements IFileReference.FilePath

    Public ReadOnly Property MimeType As ContentType() Implements IFileReference.MimeType
        Get
            Return {
                New ContentType With {.Details = "Microsoft Excel Table", .FileExt = ".csv", .MIMEType = "application/csv", .Name = "Microsoft Excel Table"}
            }
        End Get
    End Property

    ''' <summary>
    ''' view spectrum data
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ViewToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewToolStripMenuItem.Click
        Dim cluster As TreeListViewItem
        Dim host = MyApplication.host

        If TreeListView1.SelectedItems.Count = 0 Then
            Return
        Else
            cluster = TreeListView1.SelectedItems(0)
        End If

        ' 当没有feature搜索结果的时候， children count也是零
        ' 但是raw文件的parent是空的
        ' 所以还需要加上parent是否为空的判断来避免无结果产生的冲突
        If cluster.ChildrenCount > 0 OrElse cluster.Parent Is Nothing Then
            ' 选择的是一个文件节点
            Dim filePath As String = cluster.ToolTipText
            Dim raw As Raw

            If Not directRaw.IsNullOrEmpty Then
                raw = directRaw.First
            Else
                raw = Globals.workspace.FindRawFile(filePath)
            End If

            If Not raw Is Nothing Then
                Call MyApplication.mzkitRawViewer.showScatter(raw, XIC:=False, directSnapshot:=True, contour:=False)
            End If
        Else
            ' 选择的是一个scan数据节点
            Dim parentFile = cluster.Parent.ToolTipText
            Dim scan_id As String = cluster.Text

            ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active

            ' scan节点
            Dim raw As Raw

            If directRaw.IsNullOrEmpty Then
                raw = Globals.workspace.FindRawFile(parentFile)
            Else
                raw = directRaw.First
            End If

            Call MyApplication.host.mzkitTool.showSpectrum(scan_id, raw, formula:=formula)
            Call MyApplication.host.mzkitTool.ShowPage()
        End If
    End Sub

    Private Sub frmFeatureSearch_Load(sender As Object, e As EventArgs) Handles Me.Load
        Text = "Feature Search Result"
        TabText = Text
        Icon = My.Resources.Search

        OpenContainingFolderToolStripMenuItem.Enabled = False
        CopyFullPathToolStripMenuItem.Enabled = False
        SaveDocumentToolStripMenuItem.Enabled = False

        Call ApplyVsTheme(ContextMenuStrip1)

        Static proxy As EventHandler(Of ExecuteEventArgs)

        ' 20220218 makes bugs fixed of the event handler
        proxy = Sub()
                    ppm = 30
                    rtmin = rangeMin
                    rtmax = rangeMax
                    types.Clear()

                    Call ApplyFeatureFilterToolStripMenuItem_Click(Nothing, Nothing)

                    MessageBox.Show("All feature filter condition has been clear!", "Reset Feature Filter", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End Sub

        AddHandler ribbonItems.ButtonResetFeatureFilter.ExecuteEvent, proxy
        AddHandler ribbonItems.ButtonExportFeatureIons.ExecuteEvent, Sub() Call exportSpectrum()
    End Sub

    Private Sub exportSpectrum()
        Using file As New SaveFileDialog With {.Filter = "MGF File(*.mgf)|*.mgf"}
            If file.ShowDialog <> DialogResult.OK Then
                Return
            End If

            Dim list = TreeListView1
            Dim parents As New List(Of ParentMatch)

            For i As Integer = 0 To list.Items.Count - 1
                Dim raw = list.Items(i)
                Dim parentFile As String = raw.ToolTipText
                Dim rawdata As Raw

                If directRaw.IsNullOrEmpty Then
                    rawdata = Globals.workspace.FindRawFile(parentFile)
                Else
                    rawdata = directRaw.First
                End If

                For j As Integer = 0 To raw.Items.Count - 1
                    Dim scan = raw.Items(j)
                    Dim scan_id As String = raw.Text

                    Call parents.Add(scan.Tag)
                Next
            Next

            Dim msdata = parents.Select(Function(p) p.ToMs2).ToArray

            Call msdata.SaveAsMgfIons(file.FileName)
            Call MessageBox.Show("The matched feature spectrum has been save to spectrum file:" & vbCrLf & file.FileName,
                                 "Export Success",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Information)
        End Using
    End Sub

    ''' <summary>
    ''' view xic or view xic overlaps
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ViewXICToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewXICToolStripMenuItem.Click
        Dim cluster As TreeListViewItem
        Dim host = MyApplication.host
        Dim ppm As New PPMmethod(30)

        If TreeListView1.SelectedItems.Count = 0 Then
            Return
        Else
            cluster = TreeListView1.SelectedItems(0)
        End If

        ribbonItems.TabGroupTableTools.ContextAvailable = ContextAvailability.Active

        ' 当没有feature搜索结果的时候， children count也是零
        ' 但是raw文件的parent是空的
        ' 所以还需要加上parent是否为空的判断来避免无结果产生的冲突
        If cluster.ChildrenCount > 0 OrElse cluster.Parent Is Nothing Then
            ' Call Workbench.Warning("Select a ms2 feature for view XIC plot!")
            ' view of the xic overlaps of current rawdata file
            Dim parentFile = cluster.ToolTipText
            Dim xic_ions = cluster.Items
            ' scan节点
            Dim raw As Raw

            If directRaw.IsNullOrEmpty Then
                raw = Globals.workspace.FindRawFile(parentFile)
            Else
                raw = directRaw.First
            End If

            Dim mzset As New List(Of NamedValue(Of Double))

            For i As Integer = 0 To xic_ions.Count - 1
                cluster = xic_ions.Item(i)

                Dim scan_id As String = cluster.Text
                Dim scan = raw.FindMs2Scan(scan_id)

                If scan Is Nothing Then
                    Call Workbench.Warning($"no scan data was found for scan id: {scan_id}!")
                    Continue For
                End If

                Dim mz As Double = scan.parentMz
                Dim adducts As String

                If cluster.SubItems.Count <= 11 Then
                    Call Workbench.Warning($"invalid scan adducts source: {scan_id}")
                    Continue For
                Else
                    adducts = cluster.SubItems.Item(11).Text
                End If

                Call mzset.Add(New NamedValue(Of Double)(adducts, mz))
            Next

            Dim GetXICCollection = Iterator Function() As IEnumerable(Of NamedCollection(Of ChromatogramTick))
                                       For Each xic_ion In mzset.GroupBy(Function(i) i.Name)
                                           Dim mz As Double = xic_ion.Average(Function(i) i.Value)
                                           Dim name As String = $"{parentFile.FileName} - {mz.ToString("F4")} {xic_ion.Key}"
                                           Dim data = GetXIC(mz, raw, ppm)

                                           Yield New NamedCollection(Of ChromatogramTick)(name, data.value)
                                       Next
                                   End Function

            ' Call MyApplication.host.mzkitTool.showSpectrum(scan_id, raw)
            Call MyApplication.mzkitRawViewer.ShowXIC(ppm.DeltaTolerance, Nothing, GetXICCollection, 0)
            Call MyApplication.host.mzkitTool.ShowPage()
        Else
            ' 选择的是一个scan数据节点
            Dim parentFile = cluster.Parent.ToolTipText
            Dim scan_id As String = cluster.Text
            ' scan节点
            Dim raw As Raw

            If directRaw.IsNullOrEmpty Then
                raw = Globals.workspace.FindRawFile(parentFile)
            Else
                raw = directRaw.First
            End If

            Dim scan = raw.FindMs2Scan(scan_id)

            If scan Is Nothing Then
                Call Workbench.Warning($"no scan data was found for scan id: {scan_id}!")
            Else
                Dim mz As Double = scan.parentMz
                Dim GetXICCollection = Iterator Function() As IEnumerable(Of NamedCollection(Of ChromatogramTick))
                                           Yield GetXIC(mz, raw, ppm)
                                       End Function

                ' Call MyApplication.host.mzkitTool.showSpectrum(scan_id, raw)
                Call MyApplication.mzkitRawViewer.ShowXIC(ppm.DeltaTolerance, Nothing, GetXICCollection, 0)
                Call MyApplication.host.mzkitTool.ShowPage()
            End If
        End If
    End Sub

    Public Shared Function GetXIC(mz As Double, raw As Raw, ppm As Tolerance) As NamedCollection(Of ChromatogramTick)
        Dim ticks As ChromatogramTick() = raw _
            .LoadMzpack(Sub(s1, s2) Workbench.LogText(s1 & " " & s2)) _
            .GetLoadedMzpack _
            .GetXIC(mz, ppm)

        Return New NamedCollection(Of ChromatogramTick) With {
            .name = $"{mz.ToString("F4")} @ {raw.source.FileName}",
            .value = ticks
        }
    End Function

    Public Function Save(s As IO.Stream, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Throw New NotImplementedException
    End Function

    Public Function Save(path As String, encoding As Encoding) As Boolean Implements ISaveHandle.Save
        Dim file As New File
        Dim row As New List(Of String)

        For i As Integer = 0 To TreeListView1.Columns.Count - 1
            row.Add(TreeListView1.Columns(i).Text)
        Next

        file.Add(New RowObject(row))

        For Each item As TreeListViewItem In TreeListView1.Items
            Dim tag As String = item.Text

            For Each feature As ListViewItem In item.Items
                row.Clear()
                row.Add(tag)

                Dim i As i32 = 0

                For Each cell As ListViewSubItem In feature.SubItems
                    If ++i <> 1 Then
                        ' skip of add no-sense #num
                        Call row.Add(cell.Text)
                    End If
                Next

                Call file.Add(New RowObject(row))
            Next
        Next

        Return file.Save(path, encoding)
    End Function

    Public Function Save(path As String, Optional encoding As Encodings = Encodings.UTF8) As Boolean Implements ISaveHandle.Save
        Return Save(path, encoding.CodePage)
    End Function

    Dim rtmin As Double = Double.NaN
    Dim rtmax As Double = Double.NaN
    Dim ppm As Double = 30
    Dim da As Double = 0.1
    Dim types As New Dictionary(Of String, Boolean)

    Private Sub ApplyFeatureFilterToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ApplyFeatureFilterToolStripMenuItem.Click
        Dim getFilters As New InputFeatureFilter
        Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)

        If rtmin.IsNaNImaginary Then
            rtmin = rangeMin
        End If
        If rtmax.IsNaNImaginary Then
            rtmax = rangeMax
        End If

        If Not list1.IsNullOrEmpty Then
            If types.IsNullOrEmpty Then
                types = list1 _
                    .Select(Function(f) f.matches) _
                    .IteratesALL _
                    .Select(Function(a) a.precursor_type) _
                    .Distinct _
                    .ToDictionary(Function(type) type,
                                  Function(any)
                                      Return True
                                  End Function)
            End If

            getFilters.AddTypes(types)
        End If

        getFilters.NumericUpDown1.Value = ppm
        getFilters.NumericUpDown2.Value = da
        getFilters.txtRtMax.Text = rtmax
        getFilters.txtRtMin.Text = rtmin

        If mask.ShowDialogForm(getFilters) = DialogResult.OK Then
            rtmin = Val(getFilters.txtRtMin.Text)
            rtmax = Val(getFilters.txtRtMax.Text)
            ppm = getFilters.NumericUpDown1.Value
            da = getFilters.NumericUpDown2.Value

            If rtmin = rtmax OrElse (rtmin = rtmax AndAlso rtmin = 0.0) OrElse rtmin > rtmax Then
                Call Workbench.Warning("invalid filter value...")
                Return
            Else
                Call TreeListView1.Items.Clear()
            End If

            If Not list1.IsNullOrEmpty Then
                Dim source = list1.ToArray
                Dim requiredTypes As Index(Of String) = getFilters.GetTypes
                Dim method = getFilters.GetMethod
                Dim filter = list1 _
                    .Select(Function(i)
                                Dim hits = i.matches _
                                    .Where(Function(p)
                                               Dim filterMass As Boolean

                                               If method = ToleranceMethods.da Then
                                                   filterMass = p.da <= da
                                               Else
                                                   filterMass = p.ppm <= ppm
                                               End If

                                               Return p.scan.rt >= rtmin AndAlso
                                                      p.scan.rt <= rtmax AndAlso
                                                      filterMass AndAlso
                                                      p.precursor_type Like requiredTypes
                                           End Function)

                                Return (i.File, hits.ToArray)
                            End Function) _
                    .ToArray

                For Each type As String In types.Keys.ToArray
                    types(type) = type Like requiredTypes
                Next

                For Each row In filter
                    Call Me.AddFileMatch(row.File, row.ToArray)
                Next

                list1.Clear()
                list1.AddRange(source)
            ElseIf Not list2.IsNullOrEmpty Then
                Dim source = list2.ToArray
                Dim filter = list2 _
                    .Select(Function(i)
                                Return (i.file, i.targetMz, i.matches.Where(Function(p) p.rt >= rtmin AndAlso p.rt <= rtmax).ToArray)
                            End Function) _
                    .ToArray

                For Each row In filter
                    Call Me.AddFileMatch(row.file, row.targetMz, row.ToArray)
                Next

                list2.Clear()
                list2.Add(source)
            End If
        End If
    End Sub

    ''' <summary>
    ''' 进行分子网络的建立来完成二级聚类
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RunMs2ClusteringToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RunMs2ClusteringToolStripMenuItem.Click
        If list1.IsNullOrEmpty Then
            ' is ms2 search
            ' scanms2
            Throw New NotImplementedException
        Else
            ' is ms1 search
            ' parent match
            Dim parents As New List(Of ParentMatch)

            For Each fileRow As TreeListViewItem In TreeListView1.Items
                For Each feature As TreeListViewItem In fileRow.Items
                    Call parents.Add(feature.Tag)
                Next
            Next

            Dim peaksData As PeakMs2() = parents.Select(Function(p) p.ToMs2).ToArray

            Call TaskProgress.RunAction(
                run:=Sub(p)
                         Call peaksData.MolecularNetworkingTool(p, 0.8)
                     End Sub,
                title:="Build Molecular Networking...",
                info:="Run ms2 clustering!"
            )
            Call MyApplication.host.mzkitMNtools.RefreshNetwork()
        End If
    End Sub

    ''' <summary>
    ''' do ms2 query of the mona database
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SearchMoNAToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SearchMoNAToolStripMenuItem.Click
        Call InputDialog.Input(Of InputMoNA)(
            Sub(q)
                If q.MoNA_id.StringEmpty Then
                    Return
                End If

                Dim name As String = q.MoNA_id

            End Sub)
    End Sub

    Private Sub FlowLayoutPanel1_SizeChanged(sender As Object, e As EventArgs) Handles FlowLayoutPanel1.SizeChanged
        For Each viewer As Control In FlowLayoutPanel1.Controls
            viewer.Width = FlowLayoutPanel1.Width * 0.95
        Next
    End Sub

    Dim _multipleMode As Boolean

    Public Property multipleMode As Boolean
        Get
            Return _multipleMode
        End Get
        Set(flag As Boolean)
            ToolStrip1.Enabled = flag
            _multipleMode = flag
        End Set
    End Property

    Private Sub ToolStripComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox1.SelectedIndexChanged
        Dim precursor_type As String = any.ToString(ToolStripComboBox1.SelectedItem)

        If adducts.ContainsKey(precursor_type) Then
            Call FlowLayoutPanel1.Controls.Clear()

            For Each viewer In adducts(precursor_type)
                viewer.Width = FlowLayoutPanel1.Width * 0.95
                FlowLayoutPanel1.Controls.Add(viewer)
            Next
        End If
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Using file As New SaveFileDialog With {.Filter = "Image File(*.png)|*.png"}
            If file.ShowDialog = DialogResult.OK Then
                Dim xicSet As New List(Of NamedCollection(Of ChromatogramTick))

                For Each viewer_ctl As Control In FlowLayoutPanel1.Controls
                    Dim viewer As XICFeatureViewer = viewer_ctl
                    Dim xic_data As NamedCollection(Of ChromatogramTick) = viewer.GetXICData

                    Call xicSet.Add(xic_data)
                Next

                Call ChromatogramPlot _
                    .TICplot(xicSet, size:="2400,1600", colorsSchema:="paper", gridFill:="white", fillCurve:=False, showLabels:=False) _
                    .AsGDIImage _
                    .SaveAs(file.FileName)

                Call Process.Start(file.FileName)
            End If
        End Using
    End Sub
End Class
