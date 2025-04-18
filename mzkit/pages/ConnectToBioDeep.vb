﻿#Region "Microsoft.VisualBasic::364ac777844c0498fec34a2021894fb8, mzkit\src\mzkit\mzkit\pages\ConnectToBioDeep.vb"

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

'   Total Lines: 162
'    Code Lines: 138
' Comment Lines: 2
'   Blank Lines: 22
'     File Size: 8.19 KB


' Class ConnectToBioDeep
' 
'     Constructor: (+1 Overloads) Sub New
'     Sub: OpenAdvancedFunction, RunMetaDNA, RunMetaDNAImpl, ShowInferAlignment, showTable
' 
' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports BioDeep
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.MZWork
Imports BioNovoGene.BioDeep.MetaDNA
Imports BioNovoGene.BioDeep.MetaDNA.Infer
Imports BioNovoGene.BioDeep.MSEngine
Imports BioNovoGene.BioDeep.MSEngine.Mummichog
Imports BioNovoGene.mzkit_win32.DockSample
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.My
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.CommonDialogs
Imports Task
Imports WeifenLuo.WinFormsUI.Docking
Imports any = Microsoft.VisualBasic.Scripting

Public Class ConnectToBioDeep

    Private Sub New()
    End Sub

    Public Shared Sub OpenAdvancedFunction(action As Action, Optional loginClick As Boolean = False)
        If loginClick OrElse Not SingletonHolder(Of BioDeepSession).Instance.CheckSession Then
            Call InputDialog.Input(Of frmBioDeepAuth)(
                Sub(login)
                    SingletonHolder(Of BioDeepSession).Instance.ssid = Workbench.BioDeepSession
                End Sub)
        End If

        If SingletonHolder(Of BioDeepSession).Instance.CheckSession Then
            Call action()
        End If
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub RunMummichog(raw As Raw, args As MassSearchArguments)
        Call OpenAdvancedFunction(Sub() RunMummichogImpl(raw, args, Function(t) t.source.FileName, AddressOf MetaDNASearch.RunMummichogDIA))
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub RunMetaDNA(raw As Raw)
        Call OpenAdvancedFunction(Sub() RunMetaDNAImpl(raw))
    End Sub

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Shared Sub RunMummichog(mz As Double(), args As MassSearchArguments)
        Call OpenAdvancedFunction(Sub() RunMummichogImpl(mz, args, Function(t) $"M/z peak set of {t.Length} ions", AddressOf MetaDNASearch.RunMummichogDIA))
    End Sub

    Private Shared Sub RunMummichogImpl(Of T)(raw As T, args As MassSearchArguments, getName As Func(Of T, String), impl As RunMummichogDIA(Of T))
        ' work in background
        Dim taskList As TaskListWindow = WindowModules.taskWin
        Dim task As TaskUI = taskList.Add("Mummichog Annotation", getName(raw))
        Dim log As OutputWindow = WindowModules.output
        Dim println As Action(Of String) =
            Sub(message)
                Call task.ProgressMessage(message)
                Call log.AppendMessage(message)
            End Sub
        Dim table As frmTableViewer = VisualStudio.ShowDocument(Of frmTableViewer)

        table.DockState = DockState.Hidden
        taskList.Show(MyApplication.host.m_dockPanel)

        VisualStudio.Dock(taskList, DockState.DockBottom)

        ' Call Alert.ShowSucess($"Imports raw data files in background,{vbCrLf}you can open [Task List] panel for view task progress.")
        Call MyApplication.TaskQueue.AddToQueue(
            Sub()
                Dim result As ActivityEnrichment() = Nothing

                Call task.Running()
                Call impl(raw, args, println, result)
                Call table.Invoke(Sub()
                                      table.DockState = DockState.Document
                                      table.Show(MyApplication.host.m_dockPanel)
                                      table.TabText = $"[Mummichog] {getName(raw)}"
                                  End Sub)

                Call println("output result table")

                Call table.Invoke(Sub() showTable(table, result))
                Call println("Mummichog search job done!")
                Call task.Finish()

                Call MessageBox.Show(
                    $"Mummichog search done!" & vbCrLf & $"Found {result.TryCount} network enrichment annotation hits.",
                    "Mummichog Search",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                )
            End Sub)
    End Sub

    Private Delegate Sub RunMummichogDIA(Of T)(x As T, args As MassSearchArguments, println As Action(Of String), ByRef result As ActivityEnrichment())

    Private Shared Sub RunMetaDNAImpl(raw As Raw)
        ' work in background
        Dim taskList As TaskListWindow = WindowModules.taskWin
        Dim task As TaskUI = taskList.Add("MetaDNA Search", raw.source.GetFullPath)
        Dim log As OutputWindow = WindowModules.output
        Dim println As Action(Of String) =
            Sub(message)
                Call task.ProgressMessage(message)
                Call log.AppendMessage(message)
            End Sub
        Dim table As frmTableViewer = VisualStudio.ShowDocument(Of frmTableViewer)

        table.DockState = DockState.Hidden
        taskList.Show(MyApplication.host.m_dockPanel)

        VisualStudio.Dock(taskList, DockState.DockBottom)

        ' Call Alert.ShowSucess($"Imports raw data files in background,{vbCrLf}you can open [Task List] panel for view task progress.")
        Call MyApplication.TaskQueue.AddToQueue(
            Sub()
                Dim result As MetaDNAResult() = Nothing
                Dim infer As CandidateInfer() = Nothing

                Call task.Running()
                Call MetaDNASearch.RunDIA(raw, println, result, infer)
                Call table.Invoke(Sub()
                                      table.DockState = DockState.Document
                                      table.Show(MyApplication.host.m_dockPanel)
                                      table.TabText = $"[MetaDNA] {raw.source.FileName}"
                                  End Sub)

                Call println("output result table")

                Call table.Invoke(Sub() showTable(table, result))
                Call table.Invoke(Sub()
                                      Call ShowInferAlignment(table, result, infer)
                                  End Sub)

                Call println("MetaDNA search job done!")

                Call task.Finish()

                Call MessageBox.Show($"MetaDNA search done!" & vbCrLf & $"Found {result.Length} DIA annotation hits.", "MetaDNA Search", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End Sub)
    End Sub

    Private Shared Sub ShowInferAlignment(table As frmTableViewer, result As MetaDNAResult(), infer As CandidateInfer())
        Dim inferIndex As Dictionary(Of String, Candidate) = infer _
            .ExportInferRaw(result).Inference _
            .ToDictionary(Function(a)
                              Return $"{a.ROI}|{a.infer.kegg.unique_id}|{a.precursorType}|{a.infer.reference.id}|{a.infer.rawFile}"
                          End Function)

        table.ViewRow =
            Sub(obj)
                Dim uidRef As String = $"{obj!ROI_id}|{obj!KEGGId}|{obj!precursorType}|{obj!seed}|{obj!fileName}"
                Dim align As Candidate = inferIndex(uidRef)

                If align.infer.level <> InferLevel.Ms1 Then
                    Dim qvsref = align.infer.GetAlignmentMirror

                    Call MyApplication.host.Invoke(
                        Sub()
                            Call MyApplication.host.mzkitTool.showAlignment(qvsref.query, qvsref.ref, align.infer, showScore:=False)
                        End Sub)
                Else
                    Call Workbench.StatusMessage($"MS1 level metaDNA infer did'nt have MS/MS alignment data...")
                End If
            End Sub
    End Sub

    Private Shared Iterator Function ParseMzSet1(row As Dictionary(Of String, Object)) As IEnumerable(Of NamedValue(Of Double))
        If row.ContainsKey("hits") Then
            ' parse the cell content as a set of mz value
            Dim hits As String() = any.ToString(row("hits"), "").StringSplit(";\s?")

            For Each meta As String In hits
                Dim mz = Val(meta.Split.Last)

                Yield New NamedValue(Of Double)(meta, mz)
            Next
        End If
    End Function

    Private Shared Sub showTable(table As frmTableViewer, result As ActivityEnrichment())
        table.ParseMsSet = AddressOf ParseMzSet1
        table.LoadTable(
            Sub(grid)
                Call grid.Columns.Add("name", GetType(String))
                Call grid.Columns.Add("description", GetType(String))
                Call grid.Columns.Add("Q", GetType(Double))
                Call grid.Columns.Add("input", GetType(Double))
                Call grid.Columns.Add("background", GetType(Double))
                Call grid.Columns.Add("activity", GetType(Double))
                Call grid.Columns.Add("p-value", GetType(Double))
                Call grid.Columns.Add("hits", GetType(String))

                For Each line As ActivityEnrichment In result.SafeQuery
                    Call grid.Rows.Add(
                        line.Name,
                        line.Description,
                        line.Q,
                        line.Input,
                        line.Background,
                        line.Activity,
                        line.Fisher.two_tail_pvalue,
                        line.Hits.Select(Function(c) $"{c.unique_id} {c.precursor_type}, m/z {c.mz.ToString("F4")}").JoinBy("; ")
                    )
                Next
            End Sub)
    End Sub

    Private Shared Sub showTable(table As frmTableViewer, result As MetaDNAResult())
        Call table.LoadTable(
            Sub(grid)
                grid.Columns.Add(NameOf(MetaDNAResult.ROI_id), GetType(String))
                grid.Columns.Add(NameOf(MetaDNAResult.query_id), GetType(String))
                grid.Columns.Add(NameOf(MetaDNAResult.mz), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.rt), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.intensity), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.KEGGId), GetType(String))
                grid.Columns.Add(NameOf(MetaDNAResult.exactMass), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.formula), GetType(String))
                grid.Columns.Add(NameOf(MetaDNAResult.name), GetType(String))
                grid.Columns.Add(NameOf(MetaDNAResult.precursorType), GetType(String))
                grid.Columns.Add(NameOf(MetaDNAResult.mzCalc), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.ppm), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.inferLevel), GetType(String))
                grid.Columns.Add(NameOf(MetaDNAResult.forward), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.reverse), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.jaccard), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.parentTrace), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.inferSize), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.score1), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.score2), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.pvalue), GetType(Double))
                grid.Columns.Add(NameOf(MetaDNAResult.seed), GetType(String))
                grid.Columns.Add(NameOf(MetaDNAResult.partnerKEGGId), GetType(String))
                grid.Columns.Add(NameOf(MetaDNAResult.KEGG_reaction), GetType(String))
                grid.Columns.Add(NameOf(MetaDNAResult.reaction), GetType(String))
                grid.Columns.Add(NameOf(MetaDNAResult.fileName), GetType(String))

                For Each line As MetaDNAResult In result.SafeQuery
                    Call grid.Rows.Add(line.ROI_id,
                                       line.query_id,
                                       line.mz,
                                       line.rt,
                                       line.intensity,
                                       line.KEGGId,
                                       line.exactMass,
                                       line.formula,
                                       line.name,
                                       line.precursorType,
                                       line.mzCalc,
                                       line.ppm,
                                       line.inferLevel,
                                       line.forward,
                                       line.reverse,
                                       line.jaccard,
                                       line.parentTrace,
                                       line.inferSize,
                                       line.score1,
                                       line.score2,
                                       line.pvalue,
                                       line.seed,
                                       line.partnerKEGGId,
                                       line.KEGG_reaction,
                                       line.reaction,
                                       line.fileName)

                    Call Application.DoEvents()
                Next
            End Sub)
    End Sub
End Class
