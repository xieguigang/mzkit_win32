﻿#Region "Microsoft.VisualBasic::f6e50c5ca8e25ec192141f4521d1c52e, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmTargetedQuantification.vb"

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

'   Total Lines: 1145
'    Code Lines: 901
' Comment Lines: 30
'   Blank Lines: 214
'     File Size: 48.07 KB


' Class frmTargetedQuantification
' 
'     Function: createGCMSLinears, createLinear, createMRMLinears, GetContentTable, GetGCMSFeatureReader
'               GetGCMSFeatures, GetScans, GetStandardReference, GetTableLevelKeys, isValidLinearRow
'               linearProfileNames, LoadGCMSIonLibrary, unifyGetStandards
' 
'     Sub: applyNewParameters, DataGridView1_CellDoubleClick, DataGridView1_CellEndEdit, DataGridView1_DragDrop, DataGridView1_DragEnter
'          DataGridView1_DragOver, DataGridView1_KeyDown, DeleteIonFeatureToolStripMenuItem_Click, deleteProfiles, doLoadSampleFiles
'          ExportImageToolStripMenuItem_Click, ExportLinearTableToolStripMenuItem_Click, ExportTableToolStripMenuItem_Click, frmTargetedQuantification_Closed, frmTargetedQuantification_FormClosing
'          frmTargetedQuantification_Load, ImportsLinearReferenceToolStripMenuItem_Click, loadGCMSReference, loadLinearRaw, loadLinears
'          loadMRMReference, loadReferenceData, loadSampleFiles, LoadSamplesToolStripMenuItem_Click, reload
'          reloadProfileNames, runLinearFileImports, SaveAsToolStripMenuItem_Click, SaveDocument, saveLinearPack
'          saveLinearsTable, SetGCMSKeys, SetMRMKeys, showIonPeaksTable, showLinear
'          showQuanifyTable, showRawXTable, ToolStripComboBox2_SelectedIndexChanged, unifyLoadLinears, ViewLinearReportToolStripMenuItem_Click
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MarkupData.mzML
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Content
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS
Imports BioNovoGene.Analytical.MassSpectrometry.Math.GCMS.QuantifyAnalysis
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.LinearQuantitative.Linear
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Data
Imports BioNovoGene.Analytical.MassSpectrometry.Math.MRM.Models
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Data.IO.MessagePack
Imports Microsoft.VisualBasic.DataStorage.netCDF
Imports Microsoft.VisualBasic.Drawing
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Windows.Forms.DataValidation.UIInteractive
Imports Mzkit_win32.BasicMDIForm
Imports RibbonLib.Controls.Events
Imports RibbonLib.Interop
Imports Task
Imports TaskStream
Imports WeifenLuo.WinFormsUI.Docking
Imports any = Microsoft.VisualBasic.Scripting
Imports std = System.Math

Public Class frmTargetedQuantification : Implements QuantificationLinearPage

    Dim args As PeakFindingParameters

    Private Sub frmTargetedQuantification_Load(sender As Object, e As EventArgs) Handles Me.Load
        WindowModules.ribbon.TargetedContex.ContextAvailable = ContextAvailability.Active

        AddHandler WindowModules.ribbon.ImportsLinear.ExecuteEvent, AddressOf loadLinearRaw
        AddHandler WindowModules.ribbon.SaveLinears.ExecuteEvent, AddressOf saveLinearsTable

        TabText = "Targeted Quantification"
        args = If(Globals.Settings.peak_finding, New PeakFindingParameters)
        CopyFullPathToolStripMenuItem.Enabled = False
        OpenContainingFolderToolStripMenuItem.Enabled = False

        Call reloadProfileNames()
        Call ApplyVsTheme(ToolStrip1, ToolStrip2, ContextMenuStrip1, ContextMenuStrip2, ContextMenuStrip3)

        Call VisualStudio.Dock(WindowModules.parametersTool, DockState.DockRight)
        Call WindowModules.parametersTool.SetParameterObject(args, AddressOf applyNewParameters)
    End Sub

    ''' <summary>
    ''' 调整参数后重新计算标准曲线
    ''' </summary>
    ''' <param name="args"></param>
    Private Sub applyNewParameters(args As PeakFindingParameters)
        If rowIndex >= 0 Then
            ' 这个可能是因为之前的一批标准曲线计算留下来的
            If DataGridView1.Rows.Count <= rowIndex Then
                Return
            End If

            showLinear(args)
        End If
    End Sub

    Private Sub reloadProfileNames()
        cbProfileNameSelector.Items.Clear()

        For Each key As String In LinearProfileNames()
            cbProfileNameSelector.Items.Add(key)
        Next
    End Sub

    Private Sub frmTargetedQuantification_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        RemoveHandler WindowModules.ribbon.ImportsLinear.ExecuteEvent, AddressOf loadLinearRaw
        RemoveHandler WindowModules.ribbon.SaveLinears.ExecuteEvent, AddressOf saveLinearsTable
    End Sub

    Sub loadLinearRaw(sender As Object, e As ExecuteEventArgs)
        Call ImportsLinearReferenceToolStripMenuItem_Click(Nothing, Nothing)
    End Sub

    Dim linearPack As LinearPack

    ''' <summary>
    ''' the raw data file list of the reference linear points
    ''' </summary>
    Dim linearFiles As NamedValue(Of String)()
    Dim mzpackRaw As mzPack
    Dim allFeatures As String()

    ''' <summary>
    ''' <see cref="QuantificationLinearPage.RunLinearFileImports"/>
    ''' 
    ''' value of this symbol could be MRM/GCMS_SIM
    ''' </summary>
    Dim targetType As TargetTypes
    Dim cals As NamedValue(Of String)()

    Sub saveLinearsTable(sender As Object, e As ExecuteEventArgs)
        If linearPack Is Nothing OrElse linearPack.linears.IsNullOrEmpty Then
            Call MyApplication.host.showStatusMessage("No linears for save!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call SaveDocument()
        End If
    End Sub

    Private Sub ImportsLinearReferenceToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ImportsLinearReferenceToolStripMenuItem.Click
        Using importsFile As New OpenFileDialog With {
            .Filter = "LC-MSMS / GC-MS Targeted(*.mzML)|*.mzML|GC-MS Targeted(*.cdf)|*.cdf",
            .Multiselect = True,
            .Title = "Select linears"
        }

            If importsFile.ShowDialog = DialogResult.OK Then
                Call runLinearFileImports(importsFile.FileNames, Nothing)
            End If
        End Using
    End Sub

    Public Sub RunLinearmzPackImports(cals() As String, mzpack As Object) Implements QuantificationLinearPage.RunLinearmzPackImports
        Dim files As NamedValue(Of String)() = ContentTable.StripMaxCommonNames(cals)
        Dim fakeLevels As Dictionary(Of String, Double)
        Dim directMapName As Boolean = False

        If files.All(Function(name) name.Value.IsContentPattern) Then
            ' parse quantification reference content value from
            ' file names directly
            files = files _
                .Select(Function(file)
                            Return New NamedValue(Of String) With {
                                .Name = file.Value,
                                .Value = file.Value,
                                .Description = file.Description
                            }
                        End Function) _
                .ToArray
            fakeLevels = files _
                .ToDictionary(Function(file) file.Value,
                              Function(file)
                                  Return file.Value _
                                      .ParseContent _
                                      .ScaleTo(ContentUnits.ppb) _
                                      .Value
                              End Function)
            directMapName = True
        Else
            fakeLevels = files _
                .ToDictionary(Function(file) file.Name,
                              Function()
                                  Return 0.0
                              End Function)
        End If

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.HeaderText = "Features"})
        DataGridView1.Columns.Add(New DataGridViewComboBoxColumn With {.HeaderText = "IS"})

        Dim pack As mzPack = mzpack
        Dim type As TargetTypes = If(
            pack.Application = FileApplicationClass.LCMSMS,
            TargetTypes.MRM,
            TargetTypes.GCMS_SIM
        )

        For Each file As NamedValue(Of String) In files
            Call DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = file.Name})
        Next

        Me.linearFiles = files
        Me.linearPack = New LinearPack With {
            .reference = New Dictionary(Of String, SampleContentLevels) From {
                {"n/a", New SampleContentLevels(fakeLevels, directMapName)}
            }
        }

        targetType = type
        mzpackRaw = pack

        Call WindowModules.MRMIons.LoadMRM(pack)

        If type <> TargetTypes.MRM Then
            Workbench.Warning("GCMS sim quantification is not implemented yet...")
        Else
            Call loadMRMReference(files, pack, directMapName)
        End If
    End Sub

    Dim linearFileDatas As DataFile()

    Private Sub importsDataFileLinears(fileNames As DataFile(), type As TargetTypes)
        Dim fileIndex = DirectCast(fileNames, DataFile()).ToDictionary(Function(a) a.filename)
        Dim files As NamedValue(Of String)() = ContentTable.StripMaxCommonNames(fileIndex.Keys.ToArray)
        Dim fakeLevels As Dictionary(Of String, Double) = Nothing
        Dim directMapName As Boolean = ConstructFileLevels(files, fakeLevels)

        ' fix of the file name mis-matched between the file model and the ion model
        For Each file As DataFile In fileNames
            file.ionPeaks = file.ionPeaks _
                .Select(Function(a) New IonPeakTableRow(a) With {.raw = file.filename}) _
                .ToArray
        Next

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.HeaderText = "Features"})
        DataGridView1.Columns.Add(New DataGridViewComboBoxColumn With {.HeaderText = "IS"})

        For Each level As KeyValuePair(Of String, Double) In fakeLevels
            Call DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = level.Key})
        Next

        Call DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)

        Dim Istd As String() = DirectCast(fileNames, DataFile()) _
            .Select(Function(file) file.ionPeaks.Select(Function(a) a.IS)) _
            .IteratesALL _
            .Distinct _
            .Where(Function(id) Strings.Len(id) > 0) _
            .ToArray

        Me.allFeatures = DirectCast(fileNames, DataFile()) _
            .Select(Function(a) a.ionPeaks.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray
        Me.linearFiles = files
        Me.linearPack = New LinearPack With {
            .reference = New Dictionary(Of String, SampleContentLevels) From {
                {"n/a", New SampleContentLevels(fakeLevels, directMapName, resolve_duplication:=Not directMapName)}
            },
            .time = Now,
            .title = "standards linears data",
            .targetted = type,
            .[IS] = Istd.Select(Function(id) New [IS](id)).ToArray,
            .peakSamples = DirectCast(fileNames, DataFile()) _
                .Select(Iterator Function(file) As IEnumerable(Of TargetPeakPoint)
                            For Each peak As IonPeakTableRow In file.ionPeaks
                                For Each pt As TargetPeakPoint In IonPeakTableRow.CastPoints(peak, file.filename)
                                    Yield pt
                                Next
                            Next
                        End Function) _
                .IteratesALL _
                .ToArray
        }

        targetType = type
        mzpackRaw = Nothing
        linearFileDatas = fileNames

        Dim contentLevels = linearPack.reference("n/a")

        For Each ion As String In allFeatures
            Dim refId As String = ion
            Dim i As Integer = DataGridView1.Rows.Add(refId)
            Dim comboxBox As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

            comboxBox.Items.Add("")

            For Each IS_candidate As String In Istd
                comboxBox.Items.Add(IS_candidate)
            Next

            If directMapName Then
                Dim row As DataGridViewRow = DataGridView1.Rows(i)

                For index As Integer = 2 To DataGridView1.Columns.Count - 1
                    row.Cells(index).Value = contentLevels.Content(DataGridView1.Columns(index).HeaderText)
                Next
            End If
        Next
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="fileNames"></param>
    ''' <param name="type">MRM/GCMS_SIM</param>
    Private Sub runLinearFileImports(fileNames As Array, type As TargetTypes?) Implements QuantificationLinearPage.RunLinearFileImports
        If TypeOf fileNames Is DataFile() Then
            Call importsDataFileLinears(fileNames, type)
        Else
            Call importsRawLinearFiles(fileNames, type)
        End If
    End Sub

    Private Function ConstructFileLevels(<Out> ByRef files As NamedValue(Of String)(), <Out> ByRef fakeLevels As Dictionary(Of String, Double)) As Boolean
        If files.All(Function(name) name.Value.BaseName.IsContentPattern) Then
            ' 10ppm 100ppb 100ppm, etc
            '
            ' parse quantification reference content value from
            ' file names directly
            files = files _
                .Select(Function(file)
                            Return New NamedValue(Of String) With {
                                .Name = file.Value.BaseName,
                                .Value = file.Value,
                                .Description = file.Description
                            }
                        End Function) _
                .ToArray
            fakeLevels = files _
                .ToDictionary(Function(file) file.Value.BaseName,
                              Function(file)
                                  ' content value could be parsed base on the file name directly
                                  Return file.Value _
                                      .BaseName _
                                      .ParseContent _
                                      .ScaleTo(ContentUnits.ppb) _
                                      .Value
                              End Function)
            Return True
        Else
            ' needs get levels data from external file
            If cals.IsNullOrEmpty Then
                fakeLevels = files _
                    .ToDictionary(Function(file) file.Name,
                                  Function()
                                      Return 0.0
                                  End Function)
            Else
                fakeLevels = cals _
                    .ToDictionary(Function(file) file.Name,
                                  Function()
                                      Return 0.0
                                  End Function)
            End If
        End If

        Return False
    End Function

    ''' <summary>
    ''' imports mzML raw data files
    ''' </summary>
    ''' <param name="fileNames"></param>
    ''' <param name="type"></param>
    Private Sub importsRawLinearFiles(fileNames As String(), type As TargetTypes?)
        Dim files As NamedValue(Of String)() = ContentTable.StripMaxCommonNames(fileNames)
        Dim fakeLevels As Dictionary(Of String, Double) = Nothing
        Dim directMapName As Boolean = ConstructFileLevels(files, fakeLevels)

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.HeaderText = "Features"})
        DataGridView1.Columns.Add(New DataGridViewComboBoxColumn With {.HeaderText = "IS"})

        ' imports levels
        For Each file As NamedValue(Of String) In files
            Call DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = file.Name})

            If type Is Nothing Then
                If file.Value.ExtensionSuffix("CDF") OrElse RawScanParser.IsSIMData(file.Value) Then
                    type = TargetTypes.GCMS_SIM
                    Call MyApplication.host.ShowGCMSSIM(file.Value, isBackground:=False, showExplorer:=False)
                Else
                    type = TargetTypes.MRM
                    Call MyApplication.host.ShowMRMIons(file.Value)
                End If
            ElseIf type.Value = TargetTypes.MRM Then
                Call MyApplication.host.ShowMRMIons(file.Value)
            Else
                Call MyApplication.host.ShowGCMSSIM(file.Value, isBackground:=False, showExplorer:=False)
            End If
        Next

        Me.linearFiles = files
        Me.linearPack = New LinearPack With {
            .reference = New Dictionary(Of String, SampleContentLevels) From {
                {"n/a", New SampleContentLevels(fakeLevels, directMapName)}
            }
        }

        targetType = type
        mzpackRaw = Nothing

        If type.Value <> TargetTypes.MRM Then
            Call loadGCMSReference(files, directMapName)
        Else
            Call loadMRMReference(files, directMapName)
        End If
    End Sub

    Private Function LoadGCMSIonLibrary() As QuantifyIon()
        Dim filePath = Globals.Settings.QuantifyIonLibfile

        If filePath.FileLength > 0 Then
            Try
                Using file As Stream = filePath.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                    Return MsgPackSerializer.Deserialize(Of QuantifyIon())(file)
                End Using
            Catch ex As Exception
                Call App.LogException(ex)
                Call MyApplication.host.showStatusMessage("Error while load GCMS reference: " & ex.Message, My.Resources.StatusAnnotations_Warning_32xLG_color)

                Return {}
            End Try
        Else
            Return {}
        End If
    End Function

    Private Sub loadGCMSReference(files As NamedValue(Of String)(), directMapName As Boolean)
        Dim ions As QuantifyIon() = LoadGCMSIonLibrary()
        Dim extract As SIMIonExtract = GetGCMSFeatureReader(ions)
        Dim allFeatures = files _
            .Select(Function(file) GetGCMSFeatures(file, extract)) _
            .IteratesALL _
            .GroupBy(Function(p) p.rt, Function(x, y) std.Abs(x - y) <= 15) _
            .ToArray
        Dim contentLevels = linearPack.reference("n/a")

        Me.allFeatures = allFeatures.Select(Function(p) $"{p.value.First.time.Min}/{p.value.First.time.Max}").ToArray

        For Each group As NamedCollection(Of ROI) In allFeatures
            Dim ion As QuantifyIon = extract.FindIon(group.First)
            Dim i As Integer = DataGridView1.Rows.Add(ion.name)
            Dim comboxBox As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

            comboxBox.Items.Add("")

            For Each IS_candidate In allFeatures
                comboxBox.Items.Add(extract.FindIon(IS_candidate.First).name)
            Next

            If directMapName Then
                Dim row As DataGridViewRow = DataGridView1.Rows(i)

                For index As Integer = 2 To DataGridView1.Columns.Count - 1
                    row.Cells(index).Value = contentLevels.Content(DataGridView1.Columns(index).HeaderText)
                Next
            End If
        Next
    End Sub

    Private Function GetGCMSFeatures(file As String, extract As SIMIonExtract) As IEnumerable(Of ROI)
        Dim gcms As GCMS.Raw

        If file.ExtensionSuffix("cdf") Then
            gcms = netCDFReader.Open(file).ReadData(showSummary:=False)
        Else
            gcms = mzMLReader.LoadFile(file)
        End If

        Return extract.GetAllFeatures(gcms)
    End Function

    ''' <summary>
    ''' fill the data table with MRM ion pair and IS ions information
    ''' </summary>
    ''' <param name="files"></param>
    ''' <param name="data"></param>
    ''' <param name="directMapName"></param>
    Private Sub loadMRMReference(files As NamedValue(Of String)(), data As mzPack, directMapName As Boolean)
        Dim ionsLib As IonLibrary = Globals.LoadIonLibrary
        Dim allFeatures As IonPair() = data.MS.Select(Function(s) s.meta.Keys) _
            .IteratesALL.Distinct.Where(Function(t) t.StartsWith("MRM:")) _
            .Select(Function(si)
                        si = si.Replace("MRM:", "").Trim
                        Dim t = si.Split("/"c).Select(AddressOf Strings.Trim).Select(Function(ti) ti.ParseDouble).ToArray
                        Return New IonPair With {.precursor = t(0), .product = t(1)}
                    End Function) _
            .ToArray
        Dim contentLevels As SampleContentLevels = linearPack.reference("n/a")

        Me.allFeatures = allFeatures.Select(AddressOf ionsLib.GetDisplay).ToArray

        For Each ion As IonPair In allFeatures
            Dim refId As String = ionsLib.GetDisplay(ion)
            Dim i As Integer = DataGridView1.Rows.Add(refId)
            Dim comboxBox As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

            comboxBox.Items.Add("")

            For Each IS_candidate As IonPair In allFeatures
                comboxBox.Items.Add(ionsLib.GetDisplay(IS_candidate))
            Next

            If directMapName Then
                Dim row As DataGridViewRow = DataGridView1.Rows(i)

                For index As Integer = 2 To DataGridView1.Columns.Count - 1
                    row.Cells(index).Value = contentLevels.Content(DataGridView1.Columns(index).HeaderText)
                Next
            End If
        Next
    End Sub

    Private Sub loadMRMReference(files As NamedValue(Of String)(), directMapName As Boolean)
        Dim ionsLib As IonLibrary = Globals.LoadIonLibrary
        Dim allFeatures As IonPair() = files _
            .Select(Function(file) file.Value) _
            .GetAllFeatures
        Dim contentLevels As SampleContentLevels = linearPack.reference("n/a")

        Me.allFeatures = allFeatures.Select(AddressOf ionsLib.GetDisplay).ToArray

        For Each ion As IonPair In allFeatures
            Dim refId As String = ionsLib.GetDisplay(ion)
            Dim i As Integer = DataGridView1.Rows.Add(refId)
            Dim comboxBox As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

            comboxBox.Items.Add("")

            For Each IS_candidate As IonPair In allFeatures
                comboxBox.Items.Add(ionsLib.GetDisplay(IS_candidate))
            Next

            If directMapName Then
                Dim row As DataGridViewRow = DataGridView1.Rows(i)

                For index As Integer = 2 To DataGridView1.Columns.Count - 1
                    row.Cells(index).Value = contentLevels.Content(DataGridView1.Columns(index).HeaderText)
                Next
            End If
        Next
    End Sub

    Private Function isValidLinearRow(r As DataGridViewRow) As Boolean
        Dim allKeys = linearPack.GetLevelKeys

        For i As Integer = 2 To allKeys.Length - 1 + 2
            If (Not any.ToString(r.Cells(i).Value).IsNumeric) OrElse (any.ToString(r.Cells(i).Value) = "0") Then
                Return False
            End If
        Next

        Return True
    End Function

    Private Sub DeleteIonFeatureToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteIonFeatureToolStripMenuItem.Click

    End Sub

    Private Iterator Function GetTableLevelKeys() As IEnumerable(Of String)
        For i As Integer = 2 To DataGridView1.Columns.Count - 1
            Yield DataGridView1.Columns(i).HeaderText
        Next
    End Function

    Private Iterator Function unifyGetStandards() As IEnumerable(Of Standards)
        Dim levelKeys As String() = GetTableLevelKeys.ToArray
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim GCMSIons As Dictionary(Of String, QuantifyIon) = LoadGCMSIonLibrary.ToDictionary(Function(i) i.name)
        Dim ref As New Value(Of Standards)

        For Each row As DataGridViewRow In DataGridView1.Rows
            If Not ref = GetStandardReference(row, GCMSIons, ionLib, levelKeys) Is Nothing Then
                Yield CType(ref, Standards)
            End If
        Next
    End Function

    Private Function GetStandardReference(row As DataGridViewRow, GCMSIons As Dictionary(Of String, QuantifyIon), ionLib As IonLibrary, levelKeys As String()) As Standards
        Dim rid As String = any.ToString(row.Cells(0).Value)
        Dim IS_id As String = any.ToString(row.Cells(1).Value)
        Dim levels As New Dictionary(Of String, Double)

        If rid.StringEmpty AndAlso IS_id.StringEmpty Then
            Return Nothing
        End If

        'If targetType = TargetTypes.GCMS_SIM Then
        '    Dim ion As QuantifyIon = GCMSIons.GetIon(rid)

        '    If Not ion Is Nothing Then
        '        rid = $"{ion.rt.Min}/{ion.rt.Max}"
        '    End If

        '    ion = GCMSIons.GetIon(IS_id)

        '    If Not ion Is Nothing Then
        '        IS_id = $"{ion.rt.Min}/{ion.rt.Max}"
        '    End If

        'Else
        '    Dim ion As IonPair = ionLib.GetIonByKey(rid)

        '    If Not ion Is Nothing Then
        '        rid = $"{ion.precursor}/{ion.product}"
        '    ElseIf rid.IsPattern("Ion \[.+?\]") Then
        '        rid = rid.GetStackValue("[", "]")
        '    End If

        '    ion = ionLib.GetIonByKey(IS_id)

        '    If Not ion Is Nothing Then
        '        IS_id = $"{ion.precursor}/{ion.product}"
        '    ElseIf IS_id.IsPattern("Ion \[.+?\]") Then
        '        IS_id = IS_id.GetStackValue("[", "]")
        '    End If
        'End If

        For i As Integer = 2 To DataGridView1.Columns.Count - 1
            levels(levelKeys(i - 2)) = any.ToString(row.Cells(i).Value).ParseDouble
        Next

        If levels.Values.All(Function(x) x = 0.0) Then
            Return Nothing
        Else
            Return New Standards() With {
                .ID = rid,
                .Name = rid,
                .[IS] = IS_id,
                .ISTD = IS_id,
                .Factor = 1,
                .C = levels
            }
        End If
    End Function

    Protected Overrides Sub SaveDocument() Handles SaveToolStripMenuItem.Click, ToolStripButton1.Click
        ' Dim ref As New List(Of Standards)(getStandards)
        Dim profileName As String = cbProfileNameSelector.Text

        If profileName.StringEmpty Then
            Call MessageBox.Show("A linear reference profile name must be provided for save file!",
                                 "Targeted Quantification Linear",
                                 MessageBoxButtons.OK,
                                 MessageBoxIcon.Error)
            Return
        End If

        Dim file As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/{profileName}.csv"

        Call TaskProgress.RunAction(
            Sub()
                Call Me.Invoke(Sub() Call SaveRefereneStandard(profileName, file))
                Call Me.Invoke(Sub() Call reloadProfileNames())
            End Sub, "Save Linear Reference Models", "...")

        Call Workbench.StatusMessage($"linear model profile '{profileName}' saved!")
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsToolStripMenuItem.Click
        Using savefile As New SaveFileDialog With {
            .Title = "Select location for save linear pack data.",
            .Filter = "Mzkit Linear Models(*.linearPack)|*.linearPack"
        }
            If savefile.ShowDialog = DialogResult.OK Then
                Call TaskProgress.RunAction(
                    Sub()
                        Call Me.Invoke(Sub() saveLinearPack(savefile.FileName.BaseName, savefile.FileName))
                    End Sub, "Save Linear Reference Models", "...")
            End If
        End Using
    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DataGridView1.KeyDown
        If e.KeyCode = System.Windows.Forms.Keys.V AndAlso e.Control AndAlso Clipboard.ContainsText Then
            Call DataGridView1.PasteTextData()
        End If
    End Sub

    Private Sub unifyLoadLinears()
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim quantifyIons As SIMIonExtract = GetGCMSFeatureReader(LoadGCMSIonLibrary)

        If linearPack.targetted = TargettedData.SIM Then
            targetType = TargetTypes.GCMS_SIM
        Else
            targetType = TargetTypes.MRM
        End If

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.HeaderText = "Features"})
        DataGridView1.Columns.Add(New DataGridViewComboBoxColumn With {.HeaderText = "IS"})

        Dim levelKeys As String() = linearPack.GetLevelKeys

        For Each level As String In levelKeys
            DataGridView1.Columns.Add(New DataGridViewTextBoxColumn With {.HeaderText = level})
        Next

        Dim islist As String() = linearPack.IS _
            .Select(Function(i)
                        Dim ionpairtext = i.ID.Split("/"c).Select(AddressOf Val).ToArray
                        Dim name As String

                        If ionpairtext.IsNullOrEmpty OrElse ionpairtext.All(Function(a) a = 0.00) Then
                            Return i.ID
                        End If

                        If targetType = TargetTypes.GCMS_SIM Then
                            name = quantifyIons.FindIon(ionpairtext.Min, ionpairtext.Max).name
                        Else
                            name = New IonPair With {
                                .precursor = ionpairtext(0),
                                .product = ionpairtext(1)
                            } _
                            .DoCall(AddressOf ionLib.GetDisplay)
                        End If

                        Return name
                    End Function) _
            .ToArray

        allFeatures = islist

        For Each linear As KeyValuePair(Of String, SampleContentLevels) In linearPack.reference
            Call loadReferenceData(quantifyIons, islist, levelKeys, ionLib, linear)
        Next
    End Sub

    Private Sub loadReferenceData(quantifyIons As SIMIonExtract,
                                  islist As String(),
                                  levelKeys As String(),
                                  ionLib As IonLibrary,
                                  linear As KeyValuePair(Of String, SampleContentLevels))

        Dim ionpairtext = linear.Key.Split("/"c).Select(AddressOf Val).ToArray
        Dim ionID As String
        Dim [is] As [IS] = linearPack.GetLinear(linear.Key)?.IS

        If [is] Is Nothing Then
            [is] = New [IS]
        End If

        If targetType = TargetTypes.GCMS_SIM Then
            ionID = quantifyIons.FindIon(ionpairtext.Min, ionpairtext.Max).name
        Else
            ionID = New IonPair With {
                .precursor = ionpairtext(0),
                .product = ionpairtext(1)
            } _
            .DoCall(AddressOf ionLib.GetDisplay)
        End If

        If Not [is].ID.StringEmpty Then
            ionpairtext = [is].ID.Split("/"c).Select(AddressOf Val).ToArray

            If targetType = TargetTypes.GCMS_SIM Then
                [is].name = quantifyIons.FindIon(ionpairtext.Min, ionpairtext.Max).name
            Else
                [is].name = New IonPair With {
                    .precursor = ionpairtext(0),
                    .product = ionpairtext(1)
                } _
                .DoCall(AddressOf ionLib.GetDisplay)
            End If
        End If

        Dim i As Integer = DataGridView1.Rows.Add(ionID)
        Dim IScandidate As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

        IScandidate.Items.Add("")

        For Each id As String In islist
            IScandidate.Items.Add(id)
        Next

        IScandidate.Value = [is].name

        For j As Integer = 0 To levelKeys.Length - 1
            DataGridView1.Rows(i).Cells(j + 2).Value = CStr(linear.Value(levelKeys(j)))
        Next
    End Sub

    Dim linearEdit As Boolean = False

    Private Sub loadLinears(sender As Object, e As EventArgs) Handles cbProfileNameSelector.SelectedIndexChanged
        If linearEdit AndAlso MessageBox.Show(
            "Current linear profiles has been edited, do you want continute to load new linear profiles data?",
            "Linear Profile Unsaved",
            MessageBoxButtons.OKCancel,
            MessageBoxIcon.Question) = DialogResult.Cancel Then

            Return
        End If

        If cbProfileNameSelector.SelectedIndex = -1 Then
            Return
        End If

        Dim profileName As String = any.ToString(cbProfileNameSelector.Items(cbProfileNameSelector.SelectedIndex))

        Call RunLinearRegression(profileName)
    End Sub

    Private Sub reload(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Call reloadProfileNames()
        Call loadLinears(Nothing, Nothing)
    End Sub

    Private Function GetContentTable(ionId As String, isId As String, row As DataGridViewRow) As ContentTable
        Dim contentLevel As New Dictionary(Of String, Double)

        For Each id As SeqValue(Of String) In GetTableLevelKeys().SeqIterator
            contentLevel(id.value) = any _
                .ToString(row.Cells(id + 2).Value) _
                .ParseDouble
        Next

        Dim directMap As Boolean

        If Me.linearFiles Is Nothing Then
            directMap = Me.linearPack _
                .GetLevelKeys _
                .All(Function(name)
                         Return name.BaseName.IsContentPattern
                     End Function)
        Else
            directMap = Me.linearFiles _
                .All(Function(name)
                         Return name.Value.BaseName.IsContentPattern
                     End Function)
        End If

        If isId.TextEquals("None") Then
            isId = Nothing
        End If

        Dim contentSampleLevel As New SampleContentLevels(contentLevel, directMap)
        Dim ref As New Standards With {
            .C = New Dictionary(Of String, Double),
            .ID = ionId,
            .[IS] = isId,
            .ISTD = isId,
            .Name = ionId
        }
        Dim levels As New Dictionary(Of String, SampleContentLevels) From {{ionId, contentSampleLevel}}
        Dim refs As New Dictionary(Of String, Standards) From {{ionId, ref}}
        Dim ISlist As Dictionary(Of String, [IS])

        If isId Is Nothing Then
            ISlist = New Dictionary(Of String, [IS])
        Else
            ISlist = New Dictionary(Of String, [IS]) From {
                {isId, New [IS] With {.ID = isId, .name = isId, .CIS = 5}}
            }
        End If

        Return New ContentTable(levels, refs, ISlist)
    End Function

    Private Function GetContentTable(row As DataGridViewRow) As ContentTable
        Dim ionId As String = any.ToString(row.Cells(0).Value)
        Dim isId As String = any.ToString(row.Cells(1).Value)

        Return GetContentTable(ionId, isId, row)
    End Function

    Dim standardCurve As StandardCurve
    Dim rowIndex As Integer = -1

    ''' <summary>
    ''' 鼠标点击参考线性表格重新计算线性方程
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub DataGridView1_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellDoubleClick
        If e.ColumnIndex <> 0 OrElse e.RowIndex < 0 Then
            Return
        Else
            rowIndex = e.RowIndex
            showLinear(args)
        End If
    End Sub

    Private Sub showLinear(args As PeakFindingParameters)
        ' 计算出线性方程
        standardCurve = createLinear(DataGridView1.Rows(rowIndex), args)

        If standardCurve Is Nothing Then
            Return
        End If

        ' 进行线性方程的可视化
        PictureBox1.BackgroundImage = standardCurve _
            .StandardCurves(
                size:="1920,1200",
                name:=$"Linear of {standardCurve.name}",
                margin:="padding: 100px 100px 200px 200px;",
                gridFill:="white"
            ) _
            .AsGDIImage

        Call DataGridView2.Rows.Clear()

        ' 显示出线性方程的线性拟合建模表格
        For Each point As ReferencePoint In standardCurve.points
            Call DataGridView2.Rows.Add(point.ID, point.Name, point.AIS, point.Ati, point.cIS, point.Cti, point.Px, point.yfit, point.error, point.variant, point.valid, point.level)
        Next
    End Sub

    Private Sub SaveRefereneStandard(title As String, file As String)
        Call unifyGetStandards.SaveTo(file)
    End Sub

    ''' <summary>
    ''' unify save linear pack data
    ''' </summary>
    ''' <param name="title"></param>
    ''' <param name="file"></param>
    Private Sub saveLinearPack(title As String, file As String)
        Dim ref As Standards() = unifyGetStandards.ToArray
        Dim linears As New List(Of StandardCurve)
        Dim points As TargetPeakPoint() = Nothing
        Dim refPoints As New List(Of TargetPeakPoint)
        Dim refLevels As New Dictionary(Of String, SampleContentLevels)
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim GCMSIons As Dictionary(Of String, QuantifyIon) = LoadGCMSIonLibrary.ToDictionary(Function(i) i.name)
        Dim directMap As Boolean = ref(Scan0).C.Keys.All(Function(name) name.IsContentPattern)

        For Each i As Standards In ref
            refLevels(i.ID) = New SampleContentLevels(i.C, directMap:=directMap)
        Next

        For Each row As DataGridViewRow In DataGridView1.Rows
            If isValidLinearRow(row) Then
                Dim line = createLinear(row, args, points)

                If Not line Is Nothing Then
                    linears.Add(line)
                    refPoints.AddRange(points)
                End If
            End If
        Next

        refPoints = refPoints _
            .GroupBy(Function(p) $"{p.SampleName}\{p.Name}") _
            .Select(Function(pg) pg.First) _
            .AsList

        If targetType = TargetTypes.GCMS_SIM Then
            Call SetGCMSKeys(refPoints, linears, GCMSIons)
        Else
            Call SetMRMKeys(refPoints, linears, ionLib)
        End If

        Dim linearPack As New LinearPack With {
            .linears = linears.ToArray,
            .peakSamples = refPoints.ToArray,
            .time = Now,
            .title = title,
            .reference = refLevels,
            .[IS] = allFeatures _
                .Select(Function(name)
                            If targetType = TargetTypes.GCMS_SIM Then
                                ' do nothing
                            Else
                                Dim nameIon As IonPair = ionLib.GetIonByKey(name)
                                name = $"{nameIon.precursor}/{nameIon.product}"
                            End If

                            Return New [IS] With {
                                .ID = name,
                                .name = name,
                                .CIS = 5
                            }
                        End Function) _
                .ToArray,
            .targetted = If(targetType = TargetTypes.GCMS_SIM, TargettedData.SIM, TargettedData.MRM)
        }

        Call linearPack.Write(file)
    End Sub

    Private Sub SetGCMSKeys(refPoints As List(Of TargetPeakPoint), linears As List(Of StandardCurve), GCMSIons As Dictionary(Of String, QuantifyIon))
        Dim ion As QuantifyIon

        For Each point As TargetPeakPoint In refPoints
            ion = GCMSIons.GetIon(point.Name)

            If Not ion Is Nothing Then
                point.Name = $"{ion.rt.Min}/{ion.rt.Max}"
            End If
        Next

        For Each line As StandardCurve In linears
            ion = GCMSIons.GetIon(line.name)

            If Not ion Is Nothing Then
                line.name = $"{ion.rt.Min}/{ion.rt.Max}"
            End If

            If Not line.IS Is Nothing AndAlso Not line.IS.ID.StringEmpty Then
                ion = GCMSIons.GetIon(line.IS.ID)

                If Not ion Is Nothing Then
                    line.IS.ID = $"{ion.rt.Min}/{ion.rt.Max}"
                End If

                line.IS.name = line.IS.ID
            End If
        Next
    End Sub

    Private Sub SetMRMKeys(refPoints As List(Of TargetPeakPoint), linears As List(Of StandardCurve), ionLib As IonLibrary)
        Dim ion As IonPair

        For Each point As TargetPeakPoint In refPoints
            ion = ionLib.GetIonByKey(point.Name)
            point.Name = $"{ion.precursor}/{ion.product}"
        Next

        For Each line As StandardCurve In linears
            ion = ionLib.GetIonByKey(line.name)
            line.name = $"{ion.precursor}/{ion.product}"

            If Not line.IS Is Nothing AndAlso Not line.IS.ID.StringEmpty Then
                ion = ionLib.GetIonByKey(line.IS.ID)
                line.IS.ID = $"{ion.precursor}/{ion.product}"
                line.IS.name = line.IS.ID
            End If
        Next
    End Sub

    Private Function GetGCMSFeatureReader(ionLib As IEnumerable(Of QuantifyIon)) As SIMIonExtract
        Return New SIMIonExtract(ionLib, New Double() {5, 15}, Tolerance.DeltaMass(0.3), 20, 0.65)
    End Function


    ''' <summary>
    ''' unify create linear reference
    ''' </summary>
    ''' <param name="refRow"></param>
    ''' <param name="refPoints"></param>
    ''' <returns></returns>
    Private Function createLinear(refRow As DataGridViewRow, args As PeakFindingParameters, Optional ByRef refPoints As TargetPeakPoint() = Nothing) As StandardCurve
        Dim id As String = any.ToString(refRow.Cells(0).Value)
        Dim isid As String = any.ToString(refRow.Cells(1).Value)

        Return createLinear(id, isid, refRow, args, refPoints)
    End Function

    ''' <summary>
    ''' unify create linear reference
    ''' </summary>
    ''' <param name="refRow"></param>
    ''' <param name="refPoints"></param>
    ''' <returns></returns>
    Private Function createLinear(id As String, isid As String, refRow As DataGridViewRow, args As PeakFindingParameters, Optional ByRef refPoints As TargetPeakPoint() = Nothing) As StandardCurve
        Dim chr As New List(Of TargetPeakPoint)

        If targetType = TargetTypes.GCMS_SIM Then
            chr.AddRange(createGCMSLinears(id, isid))
        Else
            chr.AddRange(createMRMLinears(id, isid))
        End If

        Dim algorithm As New InternalStandardMethod(GetContentTable(id, isid, refRow), PeakAreaMethods.NetPeakSum)
        Dim nameMapsReverse = cals.ToDictionary(Function(aa) aa.Value, Function(aa) aa.Name)

        refPoints = chr.ToArray

        For Each p As TargetPeakPoint In chr
            If nameMapsReverse.ContainsKey(p.SampleName) Then
                p.SampleName = nameMapsReverse(p.SampleName)
            End If
        Next

        If Not cals.IsNullOrEmpty Then
            Dim calIndex = cals.Keys.Indexing

            chr = chr _
                .Where(Function(t) t.SampleName Like calIndex) _
                .AsList
        End If

        If chr = 0 OrElse chr.All(Function(p) p.Name <> id) Then
            Call Workbench.Warning($"No sample data was found of ion '{id}'!")
            Return Nothing
        Else
            ' Return algorithm.ToLinears(chr).FirstOrDefault
            Dim keys As Index(Of String) = {id, isid}.Where(Function(s) Not s.StringEmpty(, True)).Indexing
            Dim samples As TargetPeakPoint() = sampleData _
                .SafeQuery _
                .Where(Function(f) f.filename Like sampleNames) _
                .Select(Function(f)
                            Return f.ionPeaks _
                                .Where(Function(i) i.ID Like keys) _
                                .Select(Function(i) IonPeakTableRow.CastPoints(i, f.filename)) _
                                .IteratesALL
                        End Function) _
                .IteratesALL _
                .ToArray
            Dim targets = chr.Where(Function(i) i.Name = id).ToArray
            Dim istd = chr.Where(Function(i) i.Name <> id).ToArray

            Return algorithm.ToLinear(targets, istd, id, samples)
        End If
    End Function

    Private Function createGCMSLinears(id As String, isid As String) As IEnumerable(Of TargetPeakPoint)
        Dim ionLib = LoadGCMSIonLibrary.ToDictionary(Function(a) a.name)
        Dim quantifyIon = ionLib.GetIon(id)
        Dim quantifyIS = ionLib.GetIon(isid)
        Dim SIMIonExtract = GetGCMSFeatureReader(ionLib.Values)
        Dim chr As New List(Of TargetPeakPoint)

        If linearFiles.IsNullOrEmpty Then
            Call linearPack.peakSamples _
                .Select(Function(p)
                            Dim t = p.Name.Split("/"c).Select(AddressOf Val).ToArray

                            If std.Abs(t(0) - quantifyIon.rt.Min) <= 10 AndAlso std.Abs(t(1) - quantifyIon.rt.Max) <= 10 Then
                                Return New TargetPeakPoint With {
                                    .Name = quantifyIon.name,
                                    .ChromatogramSummary = p.ChromatogramSummary,
                                    .Peak = p.Peak,
                                    .SampleName = p.SampleName
                                }
                            ElseIf std.Abs(t(0) - quantifyIS.rt.Min) <= 10 AndAlso std.Abs(t(1) - quantifyIS.rt.Max) <= 10 Then
                                Return New TargetPeakPoint With {
                                    .Name = quantifyIS.name,
                                    .ChromatogramSummary = p.ChromatogramSummary,
                                    .Peak = p.Peak,
                                    .SampleName = p.SampleName
                                }
                            Else
                                Return Nothing
                            End If
                        End Function) _
                .Where(Function(p) Not p Is Nothing) _
                .DoCall(AddressOf chr.AddRange)
        Else
            Call SIMIonExtract.LoadSamples(linearFiles, quantifyIon, keyByName:=True).DoCall(AddressOf chr.AddRange)

            If Not isid.StringEmpty Then
                Call SIMIonExtract.LoadSamples(linearFiles, quantifyIS, keyByName:=True).DoCall(AddressOf chr.AddRange)
            End If
        End If

        Return chr
    End Function

    ''' <summary>
    ''' select the raw peak area data points jsut for linear information
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="isid"></param>
    ''' <returns></returns>
    Private Function createMRMLinears(id As String, isid As String) As IEnumerable(Of TargetPeakPoint)
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim quantifyIon = ionLib.GetIonByKey(id)
        Dim quantifyIS = ionLib.GetIonByKey(isid)
        Dim dadot3 As Tolerance = args.GetTolerance
        Dim chr As New List(Of TargetPeakPoint)

        If linearFiles.IsNullOrEmpty Then
            ' load from model files
            Call linearPack.peakSamples _
                .Select(Function(p)
                            Dim t = p.Name.Split("/"c).Select(AddressOf Val).ToArray

                            If dadot3(t(0), quantifyIon.precursor) AndAlso dadot3(t(1), quantifyIon.product) Then
                                Return New TargetPeakPoint With {
                                    .Name = quantifyIon.name,
                                    .ChromatogramSummary = p.ChromatogramSummary,
                                    .Peak = p.Peak,
                                    .SampleName = p.SampleName
                                }
                            ElseIf dadot3(t(0), quantifyIS.precursor) AndAlso dadot3(t(1), quantifyIS.product) Then
                                Return New TargetPeakPoint With {
                                    .Name = quantifyIS.name,
                                    .ChromatogramSummary = p.ChromatogramSummary,
                                    .Peak = p.Peak,
                                    .SampleName = p.SampleName
                                }
                            Else
                                Return Nothing
                            End If
                        End Function) _
                .Where(Function(p) Not p Is Nothing) _
                .DoCall(AddressOf chr.AddRange)
        ElseIf mzpackRaw IsNot Nothing Then
            Dim arguments As MRMArguments = args.GetMRMArguments
            Dim cals As Index(Of String) = linearFiles.Select(Function(f) f.Value).Indexing
            Dim raw As Dictionary(Of String, ScanMS1()) = mzpackRaw.MS _
                .GroupBy(Function(si) si.meta(mzStreamWriter.SampleMetaName)) _
                .Where(Function(g) g.Key Like cals) _
                .ToDictionary(Function(a) a.Key,
                              Function(a)
                                  Return a.ToArray
                              End Function)

            arguments.sn_threshold = -1

            Call MRMIonExtract.LoadSamples(raw, quantifyIon, arguments).DoCall(AddressOf chr.AddRange)

            If Not isid.StringEmpty Then
                Call MRMIonExtract.LoadSamples(raw, quantifyIS, arguments).DoCall(AddressOf chr.AddRange)
            End If
        ElseIf Not linearFileDatas.IsNullOrEmpty AndAlso Not linearPack.peakSamples.IsNullOrEmpty Then
            ' target and IS points
            Return linearPack.peakSamples _
                .AsParallel _
                .Where(Function(i)
                           Return i.Name = id OrElse i.Name = isid
                       End Function) _
                .AsList
        Else
            Dim arguments As MRMArguments = args.GetMRMArguments

            ' load from raw data files
            Call MRMIonExtract _
                .LoadSamples(linearFiles, quantifyIon, arguments) _
                .DoCall(AddressOf chr.AddRange)

            If Not isid.StringEmpty Then
                Call MRMIonExtract _
                    .LoadSamples(linearFiles, quantifyIS, arguments) _
                    .DoCall(AddressOf chr.AddRange)
            End If
        End If

        Return chr
    End Function

    Private Sub ExportImageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportImageToolStripMenuItem.Click
        Using file As New SaveFileDialog With {
            .Title = "Export Standard Curve Image",
            .Filter = "Plot Image(*.png)|*.png"
        }
            If file.ShowDialog = DialogResult.OK Then
                Call PictureBox1.BackgroundImage.SaveAs(file.FileName)
            End If
        End Using
    End Sub

    Private Sub ExportLinearTableToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExportLinearTableToolStripMenuItem.Click
        Using file As New SaveFileDialog With {
            .Title = "Export Reference Points",
            .Filter = "Reference Point Table(*.csv)|*.csv"
        }
            If file.ShowDialog = DialogResult.OK Then
                Call standardCurve.points.SaveTo(file.FileName)
            End If
        End Using
    End Sub

    ''' <summary>
    ''' the content data result of the imports sample data files
    ''' </summary>
    Dim scans As New List(Of QuantifyScan)
    Dim report As New List(Of DataReport)

    Private Sub LoadSamplesToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadSamplesToolStripMenuItem.Click
        Using importsFile As New OpenFileDialog With {
            .Filter = "LC-MSMS / GC-MS Targeted(*.mzML)|*.mzML|GC-MS Targeted(*.cdf)|*.cdf",
            .Multiselect = True,
            .Title = "Select linears"
        }
            If importsFile.ShowDialog = DialogResult.OK Then
                Call TaskProgress.LoadData(
                    Function(echo As Action(Of String))
                        Call doLoadSampleFiles(importsFile.FileNames, echo)
                        Return True
                    End Function, "Import sample data files...")
            End If
        End Using
    End Sub

    Private Sub doLoadSampleFiles(FileNames As Array, echo As Action(Of String)) Implements QuantificationLinearPage.LoadSampleFiles
        If TypeOf FileNames Is DataFile() Then
            ' and then do quantify if the linear is exists
            If Not linearPack Is Nothing Then
                Call loadSampleFiles(DirectCast(FileNames, DataFile()), echo)
            Else
                Call Workbench.Warning("no linear model for run quantification...")
            End If
        Else
            Call doLoadSampleFiles(DirectCast(FileNames, String()), echo)
        End If

        ToolStripComboBox2.SelectedIndex = 1

        Call showQuanifyTable()
    End Sub

    Private Sub doLoadSampleFiles(FileNames As String(), echo As Action(Of String))
        Dim files As NamedValue(Of String)() = FileNames _
            .Select(Function(file)
                        Return New NamedValue(Of String) With {
                            .Name = file.BaseName,
                            .Value = file
                        }
                    End Function) _
            .ToArray

        ' add files to viewer
        For Each file As NamedValue(Of String) In files
            Call Workbench.StatusMessage($"open raw data file '{file.Value.FileName}'...")
            Call MyApplication.host.OpenFile(file.Value, showDocument:=linearPack Is Nothing)
            Call System.Windows.Forms.Application.DoEvents()
        Next

        ' and then do quantify if the linear is exists
        If Not linearPack Is Nothing Then
            Call loadSampleFiles(files, echo)
        Else
            Call Workbench.Warning("no linear model for run quantification, just open raw files viewer...")
        End If
    End Sub

    Public Sub LoadSampleMzpack(samples() As String, mzpack As Object, echo As Action(Of String)) Implements QuantificationLinearPage.LoadSampleMzpack
        Dim points As New List(Of TargetPeakPoint)
        Dim linears As New List(Of StandardCurve)
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim GCMSIons = LoadGCMSIonLibrary.ToDictionary(Function(a) a.name)
        Dim extract = GetGCMSFeatureReader(GCMSIons.Values)
        Dim massError As MRMArguments = args.GetMRMArguments
        Dim sampleIndex = samples.Indexing
        Dim sampleData = DirectCast(mzpack, mzPack).MS _
            .Where(Function(si) si.meta(mzStreamWriter.SampleMetaName) Like sampleIndex) _
            .GroupBy(Function(si) si.meta(mzStreamWriter.SampleMetaName)) _
            .ToDictionary(Function(a) a.Key, Function(a) a.ToArray)

        massError.sn_threshold = -1
        Call scans.Clear()

        If Not mzpack Is mzpackRaw Then
            Call WindowModules.MRMIons.LoadMRM(DirectCast(mzpack, mzPack))
        End If

        For Each refRow As DataGridViewRow In DataGridView1.Rows
            If isValidLinearRow(refRow) Then
                Dim id As String = any.ToString(refRow.Cells(0).Value)
                Dim isid As String = any.ToString(refRow.Cells(1).Value)

                Call linears.Add(createLinear(refRow, args))

                If targetType = TargetTypes.GCMS_SIM Then

                Else
                    Dim ion As IonPair = ionLib.GetIonByKey(id)
                    Dim ISion As IonPair = ionLib.GetIonByKey(isid)


                    points.AddRange(MRMIonExtract.LoadSamples(sampleData, ion, massError))
                    echo($"Measure linear for {ion.ToString}")

                    If Not ISion Is Nothing Then
                        points.AddRange(MRMIonExtract.LoadSamples(sampleData, ISion, massError))
                    End If
                End If
            End If
        Next

        With linears.Where(Function(l) Not l Is Nothing).ToArray
            For Each file As IGrouping(Of String, TargetPeakPoint) In points.GroupBy(Function(p) p.SampleName)
                Dim uniqueIons = file.GroupBy(Function(p) p.Name).Select(Function(p) p.First).ToArray
                Dim quantify As QuantifyScan = .SampleQuantify(uniqueIons, PeakAreaMethods.SumAll, fileName:=file.Key)

                Call echo($"Processing quantify for sample: {file.Key}")

                If Not quantify Is Nothing Then
                    scans.Add(quantify)
                End If
            Next
        End With
    End Sub

    Dim sampleData As DataFile()

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="files"></param>
    ''' <param name="echo"></param>
    ''' <param name="istd">
    ''' make istd id reference overrides!
    ''' </param>
    Private Sub loadSampleFiles(files As DataFile(), echo As Action(Of String), Optional istd As String = Nothing)
        Dim linears As New List(Of StandardCurve)
        Dim standardCurve As StandardCurve

        For rowIndex As Integer = 0 To DataGridView1.Rows.Count - 1
            Dim refRow As DataGridViewRow = DataGridView1.Rows(rowIndex)

            If istd Is Nothing Then
                standardCurve = createLinear(refRow, args)
            Else
                standardCurve = createLinear(any.ToString(refRow.Cells(0).Value), istd, refRow, args)
            End If

            If Not standardCurve Is Nothing Then
                Call linears.Add(standardCurve)
            End If
        Next

        Call scans.Clear()
        Call report.Clear()

        linearPack.linears = linears.ToArray
        sampleData = files

        For Each file As DataFile In files
            Dim contents As Dictionary(Of String, Double) = file.CreateQuantifyData(linearPack.linears)
            Dim fill As IonPeakTableRow() = file.ionPeaks _
                .Select(Function(i)
                            Return New IonPeakTableRow(i) With {
                                .content = contents.TryGetValue(i.ID)
                            }
                        End Function) _
                .ToArray
            Dim quantify As New QuantifyScan With {
                .filename = file.filename,
                .ionPeaks = fill,
                .rawX = New DataSet With {
                    .ID = file.filename,
                    .Properties = file.GetPeakData
                },
                .quantify = New DataSet With {
                    .ID = file.filename,
                    .Properties = contents
                }
            }

            Call echo($"Processing quantify for sample: {file.filename}")

            If Not quantify Is Nothing Then
                Call scans.Add(quantify)
            End If
        Next

        For Each line As StandardCurve In linearPack.linears.SafeQuery
            Dim sampledata As New Dictionary(Of String, Double)

            For Each file As QuantifyScan In scans
                sampledata(file.filename) = file.quantify(line.name)
            Next

            Dim var = line.points _
                .Where(Function(p) p.valid AndAlso Not p.variant.IsNaNImaginary) _
                .ToArray

            Call report.Add(New DataReport With {
                .ID = line.name,
                .name = .ID,
                .linear = line.linear.Polynomial.ToString,
                .R2 = line.linear.R2,
                .samples = sampledata,
                .ISTD = If(istd, line.IS?.ID),
                .invalids = line.points _
                    .Where(Function(p) Not p.valid) _
                    .Select(Function(p) p.level) _
                    .ToArray,
                .[variant] = If(var.Length = 0, 0, var.Average(Function(p) p.variant)),
                .weight = If(line.isWeighted, line.weight, "n/a"),
                .range = line.range.MinMax
            })
        Next
    End Sub

    Private Sub loadSampleFiles(files As NamedValue(Of String)(), echo As Action(Of String))
        Dim points As New List(Of TargetPeakPoint)
        Dim linears As New List(Of StandardCurve)
        Dim ionLib As IonLibrary = Globals.LoadIonLibrary
        Dim GCMSIons = LoadGCMSIonLibrary.ToDictionary(Function(a) a.name)
        Dim extract = GetGCMSFeatureReader(GCMSIons.Values)
        Dim massError As MRMArguments = args.GetMRMArguments

        Call scans.Clear()

        For Each refRow As DataGridViewRow In DataGridView1.Rows
            If isValidLinearRow(refRow) Then
                Dim id As String = any.ToString(refRow.Cells(0).Value)
                Dim isid As String = any.ToString(refRow.Cells(1).Value)

                Call linears.Add(createLinear(refRow, args))

                If targetType = TargetTypes.GCMS_SIM Then
                    Dim ion As QuantifyIon = GCMSIons.GetIon(id)
                    Dim ISion As QuantifyIon = GCMSIons.GetIon(isid)

                    points.AddRange(extract.LoadSamples(files, ion, keyByName:=True))
                    points.AddRange(extract.LoadSamples(files, ion, keyByName:=True))
                    echo($"Measure linear for {ion.ToString}")
                Else
                    Dim ion As IonPair = ionLib.GetIonByKey(id)
                    Dim ISion As IonPair = ionLib.GetIonByKey(isid)

                    points.AddRange(MRMIonExtract.LoadSamples(files, ion, massError))
                    echo($"Measure linear for {ion.ToString}")

                    If Not ISion Is Nothing Then
                        points.AddRange(MRMIonExtract.LoadSamples(files, ISion, massError))
                    End If
                End If
            End If
        Next

        With linears.Where(Function(l) Not l Is Nothing).ToArray
            For Each file As IGrouping(Of String, TargetPeakPoint) In points.GroupBy(Function(p) p.SampleName)
                Dim uniqueIons = file.GroupBy(Function(p) p.Name).Select(Function(p) p.First).ToArray
                Dim quantify As QuantifyScan = .SampleQuantify(uniqueIons, PeakAreaMethods.SumAll, fileName:=file.Key)

                Call echo($"Processing quantify for sample: {file.Key}")

                If Not quantify Is Nothing Then
                    scans.Add(quantify)
                End If
            Next
        End With
    End Sub

    Private Sub showQuanifyTable()
        sampleTableName = "Quantify Table"

        DataGridView3.Rows.Clear()
        DataGridView3.Columns.Clear()

        Dim quantify As DataSet() = PullQuantifyResult().Select(Function(q) DirectCast(q.Value, DataSet)).ToArray
        Dim metaboliteNames = quantify.PropertyNames

        DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Sample Name"})

        For Each col As String In metaboliteNames
            DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = col})
        Next

        For Each sample As DataSet In quantify
            Dim vec As Object() = New Object() {sample.ID} _
                .JoinIterates(metaboliteNames.Select(Function(name) CObj(sample(name)))) _
                .ToArray

            DataGridView3.Rows.Add(vec)
        Next
    End Sub

    Private Sub showReportTable()
        Dim sampleNames = report.Select(Function(a) a.samples.Keys).IteratesALL.Distinct.ToArray

        sampleTableName = "Report Table"
        DataGridView3.Rows.Clear()
        DataGridView3.Columns.Clear()

        DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Ion ID"})
        DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "name"})
        DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "linear"})
        DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "R2"})

        For Each col As String In sampleNames
            DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = col})
        Next

        For Each iondata As DataReport In report
            Dim vec As Object() = New Object() {iondata.ID, iondata.name, iondata.linear, iondata.R2} _
                .JoinIterates(sampleNames.Select(Function(name) CObj(iondata(name)))) _
                .ToArray

            DataGridView3.Rows.Add(vec)
        Next
    End Sub

    Private Sub showRawXTable()
        sampleTableName = "Raw Peaks Area"

        DataGridView3.Rows.Clear()
        DataGridView3.Columns.Clear()

        Dim quantify = scans.Select(Function(q) q.rawX).ToArray
        Dim metaboliteNames = quantify.PropertyNames

        DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Sample Name"})

        For Each col As String In metaboliteNames
            DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = col})
        Next

        For Each sample In quantify
            Dim vec As Object() = New Object() {sample.ID} _
                .JoinIterates(metaboliteNames.Select(Function(name) CObj(sample(name)))) _
                .ToArray

            DataGridView3.Rows.Add(vec)
        Next
    End Sub

    Private Sub showIonPeaksTable()
        sampleTableName = "Ion PeakTable"

        DataGridView3.Rows.Clear()
        DataGridView3.Columns.Clear()

        Dim quantify As EntityObject() = scans.Select(Function(q) q.ionPeaks).IteratesALL.DataFrame.ToArray
        Dim metaboliteNames = quantify.PropertyNames

        DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = "Sample Name"})

        For Each col As String In metaboliteNames
            DataGridView3.Columns.Add(New DataGridViewTextBoxColumn() With {.HeaderText = col})
        Next

        For Each sample In quantify
            Dim vec As Object() = New Object() {sample.ID} _
                .JoinIterates(metaboliteNames.Select(Function(name) CObj(sample(name)))) _
                .ToArray

            DataGridView3.Rows.Add(vec)
        Next
    End Sub

    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        Me.linearEdit = True
        Workbench.StatusMessage("For save the linear reference content, click [save] button to save current or create new profile!")
    End Sub

    Private Sub deleteProfiles(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Dim profileName As String = cbProfileNameSelector.Text

        If MessageBox.Show($"Going to delete current linear profile '{cbProfileNameSelector.Text}'?", "Delete current profiles", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
            Return
        Else
            Call (Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/{profileName}.linearPack").DeleteFile
        End If

        linearEdit = False
        linearFiles = Nothing
        linearPack = Nothing

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        Call reloadProfileNames()
    End Sub

    Dim sampleTableName As String

    Private Sub ToolStripComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ToolStripComboBox2.SelectedIndexChanged
        Select Case ToolStripComboBox2.SelectedIndex
            Case 0 : Call showIonPeaksTable()
            Case 1 : Call showQuanifyTable()
            Case 2 : Call showRawXTable()
            Case 3 : Call showReportTable()
        End Select
    End Sub

    Private Sub ExportTableToolStripMenuItem_Click() Handles ExportTableToolStripMenuItem.Click, ToolStripButton4.Click
        Call DataGridView3.SaveDataGrid("Export sample result table [%s] success!")
    End Sub

    Private Sub DataGridView1_DragDrop(sender As Object, e As DragEventArgs) Handles DataGridView1.DragDrop
        Dim path As String = CType(e.Data.GetData(DataFormats.FileDrop), String())(Scan0)

        If path.ExtensionSuffix("csv") Then
            Call LoadStandardsLinear(file:=path)
            Return
        End If

        If Not path.ExtensionSuffix("linearpack") Then
            MessageBox.Show($"[{path}] is not a mzkit linear model file...", "Not a linearPack file", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        Else
            cbProfileNameSelector.Text = path.BaseName
            linearPack = LinearPack.OpenFile(path)

            Call unifyLoadLinears()
        End If
    End Sub

    Private Sub DataGridView1_DragEnter(sender As Object, e As DragEventArgs) Handles DataGridView1.DragEnter
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            e.Effect = DragDropEffects.Copy  ' 允许复制操作
        Else
            e.Effect = DragDropEffects.None  ' 非文件类型则拒绝
        End If
    End Sub

    Private Sub DataGridView1_DragOver(sender As Object, e As DragEventArgs) Handles DataGridView1.DragOver
        e.Effect = DragDropEffects.Copy
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function GetScans() As IEnumerable(Of QuantifyScan)
        Return scans.AsEnumerable
    End Function

    Public Sub ViewLinearModelReport(onHost As Boolean, ignoreError As Boolean) Implements QuantificationLinearPage.ViewLinearModelReport
        Dim tempfile As String = TempFileSystem.GetAppSysTempFile(".html", sessionID:=App.PID.ToHexString, "linear_report")
        Dim packtemp As String = TempFileSystem.GetAppSysTempFile(".cdf", sessionID:=App.PID.ToHexString, "linear_pack")

        Call linearPack.Write(packtemp)
        Call RscriptProgressTask.ExportLinearReport(packtemp, tempfile, onHost)

        If tempfile.FileLength <= 10 Then
            If Not ignoreError Then
                Call MessageBox.Show("Run Rscript workflow error...", "View Linear Report", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Else
            Call VisualStudio.ShowDocument(Of frmHtmlViewer)().LoadHtml(tempfile)
        End If
    End Sub

    Private Sub ViewLinearReportToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ViewLinearReportToolStripMenuItem.Click
        If linearPack Is Nothing OrElse linearPack.linears.IsNullOrEmpty Then
            Call Workbench.Warning("no linear model was loaded!")
        Else
            Call ViewLinearModelReport(onHost:=False, ignoreError:=False)
        End If
    End Sub

    Private Sub frmTargetedQuantification_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        WindowModules.ribbon.TargetedContex.ContextAvailable = ContextAvailability.NotAvailable
    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub

    Public Sub SetLinear(key As String, is_key As String, reference As Dictionary(Of String, Double)) Implements QuantificationLinearPage.SetLinear

    End Sub

    Public Sub RunLinearRegression(profileName As String) Implements QuantificationLinearPage.RunLinearRegression
        Dim file As String

        If profileName.FileExists Then
            file = profileName
        Else
            file = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & $"/mzkit/linears/{profileName}.csv"
        End If

        If file.ExtensionSuffix("csv") Then
            Call LoadStandardsLinear(file)
        Else
            Try
                linearPack = LinearPack.OpenFile(file)
            Catch ex As Exception
                ' usually be the invalid cdf file format
                Call Workbench.Warning("Load linear pack cdf file error: " & ex.Message)
                Call App.LogException(ex)

                Return
            End Try

            Call unifyLoadLinears()
        End If

        Try
            rowIndex = 0
            showLinear(args)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub LoadStandardsLinear(file As String)
        Dim standardLis As Standards() = file.LoadCsv(Of Standards)
        Dim is_list = standardLis _
            .Select(Function(r) r.IS) _
            .Where(Function(id) Strings.Len(id) > 0) _
            .Distinct _
            .ToArray

        DataGridView1.Rows.Clear()
        DataGridView1.Columns.Clear()

        DataGridView1.Columns.Add(New DataGridViewLinkColumn With {.HeaderText = "Features"})
        DataGridView1.Columns.Add(New DataGridViewComboBoxColumn With {.HeaderText = "IS"})

        Call frmLinearTableEditor.LoadStandardsToTable(DataGridView1, standardLis, is_list)

        Dim linears As New List(Of StandardCurve)
        Dim ionGroups = linearFileDatas _
            .Select(Function(a) a.ionPeaks) _
            .IteratesALL _
            .GroupBy(Function(a) a.ID) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return a.ToArray
                          End Function)

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            Dim refRow = DataGridView1.Rows(i)
            Dim algorithm As New InternalStandardMethod(GetContentTable(refRow), PeakAreaMethods.NetPeakSum)
            Dim key As String = any.ToString(refRow.Cells(0).Value)
            Dim ionPoints As IonPeakTableRow() = ionGroups.TryGetValue(key)

            If key = "" OrElse ionPoints.IsNullOrEmpty Then
                Continue For
            End If

            Call linears.Add(algorithm.ToFeatureLinear(ionPoints, key))
        Next

        linearPack.linears = linears.ToArray
    End Sub

    Public Iterator Function PullQuantifyResult() As IEnumerable(Of NamedValue(Of DynamicPropertyBase(Of Double))) Implements QuantificationLinearPage.PullQuantifyResult
        For Each scan As QuantifyScan In scans
            Dim quantify = scan.quantify

            Yield New NamedValue(Of DynamicPropertyBase(Of Double))(quantify.ID, quantify)
        Next
    End Function

    Private Sub DataGridView1_MouseHover(sender As Object, e As EventArgs) Handles DataGridView1.MouseHover
        Workbench.StatusMessage("Double click on the compound [features] name to view corresponding standard curve plot and data points.", My.Resources.preferences_system_notifications)
    End Sub

    Public Sub SetCals(filenames() As NamedValue(Of String)) Implements QuantificationLinearPage.SetCals
        cals = filenames
    End Sub

    Private Sub cbProfileNameSelector_Click(sender As Object, e As EventArgs) Handles cbProfileNameSelector.Click

    End Sub

    Private Sub ToolStripButton5_Click(sender As Object, e As EventArgs) Handles ToolStripButton5.Click
        If Not sampleData.IsNullOrEmpty Then
            Call loadSampleFiles(sampleData, AddressOf Workbench.StatusMessage)
            Call MessageBox.Show($"Make quantification of {sampleData.Length} samples data files success!", "Run Quantification Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub OpenInTableViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OpenInTableViewerToolStripMenuItem.Click
        Dim tbl = VisualStudio.ShowDocument(Of frmTableViewer)(title:=sampleTableName)
        Dim names As New List(Of String)

        For Each col As DataGridViewColumn In DataGridView3.Columns
            Call names.Add(col.HeaderText)
        Next

        Call tbl.LoadTable(
            Sub(grid)
                Dim row_vals As Object() = New Object(names.Count - 1) {}

                For Each name As String In names
                    Call grid.Columns.Add(name, GetType(String))
                Next

                For Each row As DataGridViewRow In DataGridView3.Rows
                    For i As Integer = 0 To names.Count - 1
                        row_vals(i) = row.Cells(i).Value
                    Next

                    Call grid.Rows.Add(row_vals)
                Next
            End Sub)
    End Sub

    Private Sub ToolStripComboBox2_Click(sender As Object, e As EventArgs) Handles ToolStripComboBox2.Click

    End Sub

    ''' <summary>
    ''' set istd id list
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton6_Click(sender As Object, e As EventArgs) Handles ToolStripButton6.Click
        If linearPack Is Nothing Then
            Call Workbench.Warning("Please load the targetted linear regression data before paste the istd id list to the editor!")
        Else
            Dim editor As New InputIdList

            If Not linearPack.IS.IsNullOrEmpty Then
                editor.IdSet = linearPack.IS.Select(Function(i) i.ID).Distinct.ToArray
            End If

            Call editor.Input(Sub(config) Call setIS(editor.IdSet))
        End If
    End Sub

    Private Sub setIS(idset As String())
        linearPack.IS = idset _
            .Select(Function(i) New [IS](i)) _
            .ToArray

        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            Dim IScandidate As DataGridViewComboBoxCell = DataGridView1.Rows(i).Cells(1)

            IScandidate.Items.Clear()
            IScandidate.Items.Add("")
            IScandidate.Value = IScandidate.Items(0)

            For Each id As String In idset
                IScandidate.Items.Add(id)
            Next
        Next

        Call DataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit)
    End Sub

    Private Sub ToolStripButton7_Click(sender As Object, e As EventArgs) Handles ToolStripButton7.Click
        If linearPack.IS.IsNullOrEmpty Then
            Call ToolStripButton6_Click(sender, e)
        End If
        If linearPack.IS.IsNullOrEmpty Then
            Call Workbench.Warning("NO istd data for the linear evaluation!")
            Return
        End If

        Dim reportTable As New List(Of DataReport)
        Dim sampleData = Me.sampleData
        Dim rawNames = Me.sampleNames

        If MessageBox.Show($"Select samples for make content range reference?{vbCrLf}{vbCrLf}Select [NO] means use all sample files for make content range reference.",
                           "Config Options",
                           MessageBoxButtons.YesNo,
                           MessageBoxIcon.Information) = DialogResult.Yes Then

            Call New InputReferencePointNames().SetNames(sampleData.Select(Function(f) f.filename)).Input(
                Sub(config)
                    Dim selNames = DirectCast(config, InputReferencePointNames).GetReferencePointNames(Nothing).ToArray
                    Dim selIndex = selNames.Indexing

                    ' sampleData = sampleData.Where(Function(s) s.filename Like selIndex).ToArray
                    sampleNames = selIndex
                End Sub)
        End If

        Call TaskProgress.RunAction(Sub(echo As ITaskProgress)
                                        Dim n As Integer = linearPack.IS.Length
                                        Dim p As Integer = 0

                                        Call echo.SetProgressMode()

                                        ' loop throught all internal standards
                                        For Each istd As [IS] In linearPack.IS
                                            For Each line As StandardCurve In linearPack.linears
                                                line.IS = New [IS](istd.ID)
                                            Next

                                            Call echo.SetProgress(p / n * 100, $"Processing of the istd({istd.ID}) data option...")

                                            ' set istd id overrides for current option
                                            Call loadSampleFiles(sampleData, AddressOf Workbench.StatusMessage, istd:=istd.ID)
                                            Call reportTable.AddRange(report)

                                            p += 1
                                        Next
                                    End Sub,
             title:="Try All ISTD Options For Linears",
             info:="Processing of the data combination...",
             cancel:=AddressOf App.DoNothing
        )

        Me.sampleNames = rawNames

        Dim tbl = VisualStudio.ShowDocument(Of frmTableViewer)(title:="Linear ISTD Evaluations")
        Dim names As String() = reportTable _
            .Select(Function(a) a.samples.Keys) _
            .IteratesALL _
            .Distinct _
            .ToArray

        '{
        '    NameOf(DataReport.ID),
        '    NameOf(DataReport.name),
        '    NameOf(DataReport.ISTD),
        '    NameOf(DataReport.linear),
        '    NameOf(DataReport.R2)
        '}

        Call tbl.LoadTable(
            Sub(grid)
                Dim fixed As Integer = 11
                Dim row_vals As Object() = New Object((fixed + names.Length) - 1) {}

                Call grid.Columns.Add(NameOf(DataReport.ID), GetType(String))
                Call grid.Columns.Add(NameOf(DataReport.name), GetType(String))
                Call grid.Columns.Add(NameOf(DataReport.ISTD), GetType(String))
                Call grid.Columns.Add(NameOf(DataReport.linear), GetType(String))
                Call grid.Columns.Add(NameOf(DataReport.weight), GetType(String))
                Call grid.Columns.Add(NameOf(DataReport.R2), GetType(Double))
                Call grid.Columns.Add(NameOf(DataReport.R), GetType(Double))
                Call grid.Columns.Add(NameOf(DataReport.variant), GetType(Double))
                Call grid.Columns.Add("delete points", GetType(Integer))
                Call grid.Columns.Add(NameOf(DataReport.invalids), GetType(String))
                Call grid.Columns.Add(NameOf(DataReport.range), GetType(String))

                For Each name As String In names
                    Call grid.Columns.Add(name, GetType(Double))
                Next

                For Each opt As DataReport In reportTable
                    row_vals(0) = opt.ID
                    row_vals(1) = opt.name
                    row_vals(2) = opt.ISTD
                    row_vals(3) = opt.linear
                    row_vals(4) = opt.weight
                    row_vals(5) = std.Round(opt.R2, 4)
                    row_vals(6) = std.Round(opt.R, 4)
                    row_vals(7) = std.Round(opt.variant, 2)
                    row_vals(8) = opt.invalids.TryCount
                    row_vals(9) = opt.invalids.JoinBy(", ")
                    row_vals(10) = $"{opt.range.Min.ToString("F2")} ~ {opt.range.Max.ToString("F2")}"

                    Dim offset = fixed

                    For Each name As String In names
                        row_vals(offset) = std.Round(opt.samples(name), 2)
                        offset += 1
                    Next

                    Call grid.Rows.Add(row_vals)
                Next
            End Sub)
    End Sub

    Private Sub SendToTableViewerToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SendToTableViewerToolStripMenuItem.Click
        Dim tbl_data As ReferencePoint() = standardCurve.points
        Dim tbl = VisualStudio.ShowDocument(Of frmTableViewer)(title:=$"[{standardCurve.name}]Linear Reference Points")

        Call tbl.LoadTable(
            Sub(grid)
                Call grid.Columns.Add(NameOf(ReferencePoint.ID), GetType(String))
                Call grid.Columns.Add(NameOf(ReferencePoint.Name), GetType(String))
                Call grid.Columns.Add(NameOf(ReferencePoint.AIS), GetType(Double))
                Call grid.Columns.Add(NameOf(ReferencePoint.Ati), GetType(Double))
                Call grid.Columns.Add(NameOf(ReferencePoint.cIS), GetType(Double))
                Call grid.Columns.Add(NameOf(ReferencePoint.Cti), GetType(Double))
                Call grid.Columns.Add(NameOf(ReferencePoint.Px), GetType(Double))
                Call grid.Columns.Add(NameOf(ReferencePoint.yfit), GetType(Double))
                Call grid.Columns.Add(NameOf(ReferencePoint.error), GetType(Double))
                Call grid.Columns.Add(NameOf(ReferencePoint.variant), GetType(Double))
                Call grid.Columns.Add(NameOf(ReferencePoint.valid), GetType(String))
                Call grid.Columns.Add(NameOf(ReferencePoint.level), GetType(String))

                For Each pt As ReferencePoint In tbl_data
                    Call grid.Rows.Add(pt.ID, pt.Name, pt.AIS, pt.Ati, pt.cIS, pt.Cti, pt.Px, pt.yfit, pt.error, pt.variant, pt.valid.ToString, pt.level)
                Next
            End Sub)
    End Sub

    Dim sampleNames As Index(Of String)

    Public Sub SetSampleNames(names As IEnumerable(Of String)) Implements QuantificationLinearPage.SetSampleNames
        sampleNames = names.Indexing
    End Sub
End Class
