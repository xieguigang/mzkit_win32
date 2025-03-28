﻿#Region "Microsoft.VisualBasic::ca52f92a5090e5fe640486533779d552, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmUntargettedViewer.vb"

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

'   Total Lines: 176
'    Code Lines: 142
' Comment Lines: 3
'   Blank Lines: 31
'     File Size: 6.56 KB


' Class frmUntargettedViewer
' 
'     Sub: BPCToolStripMenuItem_Click, CopyFullPath, FilterMs2ToolStripMenuItem_Click, frmUntargettedViewer_Load, loadRaw
'          MS1ToolStripMenuItem_Click, MS2ToolStripMenuItem_Click, MsSelector1_XICSelector, OpenContainingFolder, RtRangeSelector1_RangeSelect
'          SaveDocument, ShowMatrixToolStripMenuItem_Click, showTIC, updatePlot
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Chromatogram
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports BioNovoGene.Analytical.MassSpectrometry.Visualization
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports WeifenLuo.WinFormsUI.Docking

Public Class frmUntargettedViewer

    ''' <summary>
    ''' 已经加载的Raw文件
    ''' </summary>
    Dim raw As Raw

    Public Sub loadRaw(raw As Raw)
        Me.raw = raw
        Me.raw.LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache))
        Me.TabText = raw.source.FileName

        Call showTIC()

        If Not MsSelector1.ContextMenuStrip1 Is Nothing Then
            Call ApplyVsTheme(MsSelector1.ContextMenuStrip1)
        End If
    End Sub

    Dim matrix As LibraryMatrix
    Dim min As Double
    Dim max As Double

    Private Sub updatePlot()
        Dim containsMs1 As Boolean = MS1ToolStripMenuItem.Checked
        Dim containsMs2 As Boolean = MS2ToolStripMenuItem.Checked
        Dim MS1 = raw.GetMs1Scans _
            .Where(Function(m1) m1.rt >= min AndAlso m1.rt <= max) _
            .Select(Iterator Function(t) As IEnumerable(Of ms2())
                        If containsMs1 Then
                            Yield t.GetMs.ToArray
                        End If

                        If containsMs2 Then
                            For Each scan2 As ScanMS2 In t.products.SafeQuery
                                Yield scan2.GetMs.ToArray
                            Next
                        End If
                    End Function) _
            .IteratesALL _
            .IteratesALL _
            .ToArray _
            .Centroid(Tolerance.DeltaMass(0.1), LowAbundanceTrimming.intoCutff) _
            .ToArray

        If MS1.Length > 0 Then
            Dim msLib As New LibraryMatrix With {
                .centroid = True,
                .ms2 = MS1,
                .name = $"Rt: {CInt(min)} ~ {CInt(max)} sec"
            }
            Dim plot As Image = PeakAssign _
                .DrawSpectrumPeaks(msLib, size:=$"{PictureBox1.Width * 2.5},{PictureBox1.Height * 2.5}", dpi:=200) _
                .AsGDIImage

            matrix = msLib
            PictureBox1.BackgroundImage = plot
        End If
    End Sub

    Private Sub RtRangeSelector1_RangeSelect(min As Double, max As Double) Handles MsSelector1.RangeSelect
        Me.min = min
        Me.max = max
        Me.updatePlot()
    End Sub

    Private Sub showTIC() Handles MsSelector1.ShowTIC
        Dim TIC As ChromatogramTick() = raw _
            .GetMs1Scans _
            .Select(Function(t)
                        Return New ChromatogramTick With {.Time = t.rt, .Intensity = t.TIC}
                    End Function) _
            .ToArray

        Call MsSelector1.SetTIC(TIC)
        Call MsSelector1.RefreshRtRangeSelector()
    End Sub

    Private Sub BPCToolStripMenuItem_Click() Handles MsSelector1.ShowBPC
        Dim BPC As ChromatogramTick() = raw _
            .GetMs1Scans _
            .Select(Function(t)
                        Return New ChromatogramTick With {.Time = t.rt, .Intensity = t.BPC}
                    End Function) _
            .ToArray

        Call MsSelector1.SetTIC(BPC)
    End Sub

    Private Sub frmUntargettedViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.Icon = My.Resources.xobject
        Me.SaveDocumentToolStripMenuItem.Enabled = False

        Call ApplyVsTheme(ContextMenuStrip1)
    End Sub

    Private Sub FilterMs2ToolStripMenuItem_Click(rtmin As Double, rtmax As Double) Handles MsSelector1.FilterMs2
        Call WindowModules.rawFeaturesList.LoadRaw(raw, rtmin, rtmax)
        Call VisualStudio.Dock(WindowModules.rawFeaturesList, DockState.DockLeft)
    End Sub

    Protected Overrides Sub CopyFullPath()
        Call Clipboard.SetText(raw.source)
    End Sub

    Protected Overrides Sub OpenContainingFolder()
        Call Process.Start(raw.source.ParentPath)
    End Sub

    Protected Overrides Sub SaveDocument()
        MyBase.SaveDocument()
    End Sub

    Private Sub ShowMatrixToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowMatrixToolStripMenuItem.Click
        If matrix Is Nothing Then
            Call MyApplication.host.showStatusMessage("You should select data below at first!", My.Resources.StatusAnnotations_Warning_32xLG_color)
        Else
            Call MyApplication.mzkitRawViewer.showMatrix(matrix.ms2, matrix.name)
            Call MyApplication.mzkitRawViewer.PlotSpectrum(scanData:=matrix, focusOn:=True)
        End If
    End Sub

    Private Sub MS1ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MS1ToolStripMenuItem.Click
        If min > 0 OrElse max > 0 Then
            Call Me.updatePlot()
        End If
    End Sub

    Private Sub MS2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles MS2ToolStripMenuItem.Click
        If min > 0 OrElse max > 0 Then
            Call Me.updatePlot()
        End If
    End Sub

    Private Sub MsSelector1_XICSelector(rtmin As Double, rtmax As Double) Handles MsSelector1.XICSelector
        Dim MS1 As IEnumerable(Of ms2) = raw.GetMs1Scans _
            .Where(Function(m1) m1.rt >= rtmin AndAlso m1.rt <= rtmax) _
            .Select(Function(t) t.GetMs) _
            .IteratesALL
        Dim showXic As ShowXICPlot =
            Sub(xic_list)
                Dim scalar = xic_list.First

                Call MsSelector1.SetTIC(scalar.value)
                Call MsSelector1.RefreshRtRangeSelector()
            End Sub

        Call SelectXICIon(raw, MS1, 0.01, apply:=showXic)
    End Sub

    Public Shared Sub SelectXICIon(raw As Raw,
                                   ms1 As IEnumerable(Of ms2),
                                   da As Double,
                                   apply As ShowXICPlot,
                                   Optional cancel As Action = Nothing)

        Dim mask As MaskForm = MaskForm.CreateMask(frm:=MyApplication.host)
        Dim getConfig As New InputXICTarget
        Dim candidateSet = ms1.ToArray _
            .Centroid(Tolerance.DeltaMass(0.01), LowAbundanceTrimming.intoCutff) _
            .ToArray

        Call getConfig.SetIons(candidateSet.Select(Function(i) i.mz).OrderBy(Function(i) i))

        If mask.ShowDialogForm(getConfig) = DialogResult.OK Then
            If getConfig.InputOverlaps Then
                ' multiple overlaps
                If Not raw.isLoaded Then
                    Call raw.LoadMzpack(Sub(title, msg) Call Workbench.SuccessMessage($"[{title}] {msg}"))
                End If

                Dim rawpack As mzPack = raw.GetLoadedMzpack
                Dim list As New List(Of NamedCollection(Of ChromatogramTick))

                Call TaskProgress.RunAction(
                    run:=Sub(task As ITaskProgress)
                             Dim overlapTarget = getConfig.GetInputOverlaps.ToArray

                             Call task.SetProgressMode()

                             For Each serial As NamedValue(Of Double) In getConfig.GetInputOverlaps
                                 Call task.SetProgress(list.Count / overlapTarget.Length * 100, $"Extract xic data for {serial.Name}...")
                                 Call list.Add(New NamedCollection(Of ChromatogramTick)(
                                    name:=serial.Name,
                                    value:=rawpack.GetXIC(serial.Value, Tolerance.DeltaMass(da)),
                                    description:=serial.Value.ToString))
                             Next
                         End Sub,
                    title:="Load Xic overlaps data",
                    info:="Extract xic overlaps data from rawdata file...")

                Call apply(list)
            Else
                ' single xic
                Call ShowSingle(getConfig.XICTarget, raw)
            End If
        ElseIf Not cancel Is Nothing Then
            Call cancel()
        End If
    End Sub

    Private Shared Sub ShowSingle(mz As Double, raw As MZWork.Raw)
        If mz <= 0.0 Then
            Call Workbench.Warning($"No target ion m/z was selected or invalid numeric text format!")
            Return
        Else
            Call Workbench.StatusMessage($"View xic data for target ion m/z={mz} (mass error {MyApplication.host.GetXICDaError} da).")
        End If

        If Not raw.isLoaded Then
            Call raw.LoadMzpack(Sub(title, msg) Call Workbench.SuccessMessage($"[{title}] {msg}"))
        End If

        Call frmRawFeaturesList.ViewSingleTarget(mz, raw.GetLoadedMzpack)
    End Sub
End Class

Public Delegate Sub ShowXICPlot(xic As IEnumerable(Of NamedCollection(Of ChromatogramTick)))
Public Delegate Function PopulateXic(ppm As Double) As IEnumerable(Of NamedCollection(Of ChromatogramTick))