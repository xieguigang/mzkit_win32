﻿#Region "Microsoft.VisualBasic::3a08a6d23b962e50783a11ef893e465e, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\FeatureSearchHandler.vb"

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

'   Total Lines: 103
'    Code Lines: 85
' Comment Lines: 2
'   Blank Lines: 16
'     File Size: 4.10 KB


' Module FeatureSearchHandler
' 
'     Function: MatchByFormula
' 
'     Sub: runFormulaMatch, SearchByMz, searchInFileByMz
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1
Imports BioNovoGene.BioDeep.Chemoinformatics.Formula
Imports BioNovoGene.BioDeep.MSFinder
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm
Imports RibbonLib.Interop
Imports WeifenLuo.WinFormsUI.Docking

Module FeatureSearchHandler

    ''' <summary>
    ''' Do search via m/z numeric value or formula exact mass
    ''' </summary>
    ''' <param name="text"></param>
    ''' <param name="raw"></param>
    ''' <param name="directRaw"></param>
    Public Sub SearchByMz(text As String, raw As IEnumerable(Of MZWork.Raw), directRaw As Boolean, mzdiff As Tolerance)
        If text.StringEmpty Then
            Return
        ElseIf text.IsNumeric Then
            Call searchInFileByMz(mz:=Val(text), raw:=raw, tolerance:=mzdiff)
        Else
            Call Workbench.StatusMessage($"Do search features for formula: {text}...")
            Call runFormulaMatch(text, raw, directRaw, ppm:=mzdiff)

            ribbonItems.TabGroupExactMassSearchTools.ContextAvailable = ContextAvailability.Active
        End If
    End Sub

    Public Sub SearchByExactMass(mass As Double, files As IEnumerable(Of MZWork.Raw), mzdiff As Tolerance)
        Dim display As frmFeatureSearch = VisualStudio.ShowDocument(Of frmFeatureSearch)(title:=$"Search Exact Mass [{mass}]")

        Call Workbench.StatusMessage($"Do search features for exact mass: {mass}...")

        For Each file As MZWork.Raw In files
            Call display.AddFileMatch(
                file:=file.source,
                matches:=file.GetMs2Scans.MatchByExactMass(mass, file.source, ppm:=mzdiff).ToArray
            )
        Next

        ribbonItems.TabGroupExactMassSearchTools.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub runFormulaMatch(formula As String,
                                files As IEnumerable(Of MZWork.Raw),
                                directRaw As Boolean,
                                ppm As Tolerance)

        Dim display As frmFeatureSearch = VisualStudio.ShowDocument(Of frmFeatureSearch)
        Dim multipleMode As Boolean = False
        ' formula
        Dim exact_mass As Double = FormulaScanner.EvaluateExactMass(formula)

        Call Workbench.StatusMessage($"Search MS ions for [{formula}] exact_mass={exact_mass} with tolerance error {ppm}...")
        Call display.Invoke(
            Sub()
                display.TabText = $"Search [{formula}]"
                display.formula = formula
                display.directRaw = files.ToArray

                multipleMode = display.directRaw.Length > 1
                display.multipleMode = multipleMode
            End Sub)

        Dim all_matches = display.directRaw _
            .GroupBy(Function(a) a.source) _
            .Select(Function(file) New NamedValue(Of ScanMS2())(file.Key, file.First.GetMs2Scans.ToArray)) _
            .MatchByExactMass(exact_mass, ppm) _
            .ToArray
        Dim all_adducts As Dictionary(Of String, Double) = all_matches _
            .Select(Function(a) a.ToArray) _
            .IteratesALL _
            .GroupBy(Function(a) a.precursor_type) _
            .ToDictionary(Function(a) a.Key,
                          Function(a)
                              Return Aggregate xi In a Into Average(xi.scan.mz)
                          End Function)

        If Not directRaw Then
            'display.AddEachFileMatch(
            '    Sub(raw)
            '        Call display.AddFileMatch(
            '            file:=raw.source,
            '            matches:=MatchByFormula(formula, raw, ppm).ToArray
            '        )
            '    End Sub)

            For Each raw In all_matches
                Call System.Windows.Forms.Application.DoEvents()
                Call display.AddFileMatch(
                    file:=raw.name,
                    matches:=raw.ToArray,
                    all_adducts)
            Next
        Else
            Call ProgressSpinner.DoLoading(
                Sub()
                    'For Each file As MZWork.Raw In files
                    '    Dim result = MatchByFormula(formula, file, ppm).ToArray

                    '    System.Windows.Forms.Application.DoEvents()
                    '    display.Invoke(
                    '        Sub()
                    '            display.AddFileMatch(file.source, result)
                    '        End Sub)
                    'Next
                    For Each file In all_matches
                        Dim result = file.ToArray

                        Call System.Windows.Forms.Application.DoEvents()
                        Call display.Invoke(
                            Sub()
                                Call display.AddFileMatch(file.name, result, all_adducts)
                            End Sub)
                    Next
                End Sub)
        End If

        Call display.Invoke(
            Sub()
                If multipleMode Then
                    Call display.LoadAdducts()
                End If
            End Sub)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="mz">the ms1 m/z value</param>
    ''' <param name="raw"></param>
    ''' <param name="tolerance"></param>
    Private Sub searchInFileByMz(mz As Double, raw As IEnumerable(Of MZWork.Raw), tolerance As Tolerance)
        Dim display As frmFeatureSearch = VisualStudio.ShowDocument(Of frmFeatureSearch)(DockState.Document, title:=$"Feature Search Of Precursor {mz.ToString("F4")}")

        display.directRaw = raw.ToArray

        For Each file As MZWork.Raw In display.directRaw
            Dim result As ScanMS2() = file _
                .LoadMzpack(Sub(src, cache) frmFileExplorer.getRawCache(src,, cache)) _
                .GetMs2Scans _
                .Where(Function(a) tolerance(a.parentMz, mz)) _
                .ToArray

            display.AddFileMatch(file.source, mz, result)
        Next
    End Sub
End Module
