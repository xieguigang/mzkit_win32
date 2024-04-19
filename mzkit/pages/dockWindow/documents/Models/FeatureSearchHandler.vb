#Region "Microsoft.VisualBasic::3a08a6d23b962e50783a11ef893e465e, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\FeatureSearchHandler.vb"

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
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Ms1.PrecursorType
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.Linq
Imports Mzkit_win32.BasicMDIForm
Imports RibbonLib.Interop
Imports Task
Imports WeifenLuo.WinFormsUI.Docking
Imports std = System.Math

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
                matches:=MatchByExactMass(mass, file, ppm:=mzdiff).ToArray
            )
        Next

        ribbonItems.TabGroupExactMassSearchTools.ContextAvailable = ContextAvailability.Active
    End Sub

    Private Sub runFormulaMatch(formula As String,
                                files As IEnumerable(Of MZWork.Raw),
                                directRaw As Boolean,
                                ppm As Tolerance)

        Dim display As frmFeatureSearch = VisualStudio.ShowDocument(Of frmFeatureSearch)

        display.TabText = $"Search [{formula}]"
        display.formula = formula
        display.directRaw = files.ToArray

        If Not directRaw Then
            display.AddEachFileMatch(
                Sub(raw)
                    Call display.AddFileMatch(
                        file:=raw.source,
                        matches:=MatchByFormula(formula, raw, ppm).ToArray
                    )
                End Sub)
        Else
            Call ProgressSpinner.DoLoading(
                Sub()
                    For Each file As MZWork.Raw In files
                        Dim result = MatchByFormula(formula, file, ppm).ToArray

                        display.Invoke(
                            Sub()
                                display.AddFileMatch(file.source, result)
                            End Sub)
                    Next
                End Sub)
        End If
    End Sub

    Public Iterator Function MatchByExactMass(exact_mass As Double, raw As MZWork.Raw, ppm As Tolerance) As IEnumerable(Of ParentMatch)
        ' C25H40N4O5
        Dim pos = MzCalculator.EvaluateAll(exact_mass, "+", False).ToArray
        Dim neg = MzCalculator.EvaluateAll(exact_mass, "-", False).ToArray
        Dim info As PrecursorInfo()

        For Each scan As ScanMS2 In raw.GetMs2Scans
            If scan.polarity > 0 Then
                info = pos
            Else
                info = neg
            End If

            For Each mode As PrecursorInfo In info
                If ppm(scan.parentMz, Val(mode.mz)) Then
                    Yield New ParentMatch With {
                        .scan_id = scan.scan_id,
                        .mz = scan.mz,
                        .rt = CInt(scan.rt),
                        .BPC = scan.into.Max,
                        .TIC = scan.into.Sum,
                        .M = mode.M,
                        .adducts = mode.adduct,
                        .charge = mode.charge,
                        .precursor_type = mode.precursor_type,
                        .ppm = PPMmethod.PPM(scan.parentMz, Val(mode.mz)).ToString("F0"),
                        .polarity = scan.polarity,
                        .XIC = scan.intensity,
                        .into = scan.into,
                        .parentMz = scan.parentMz,
                        .rawfile = raw.source,
                        .da = std.Round(std.Abs(scan.parentMz - Val(mode.mz)), 3)
                    }
                End If
            Next

            Call System.Windows.Forms.Application.DoEvents()
        Next
    End Function

    Public Function MatchByFormula(formula As String, raw As MZWork.Raw, ppm As Tolerance) As IEnumerable(Of ParentMatch)
        ' formula
        Dim exact_mass As Double = Math.EvaluateFormula(formula)
        Dim q = MatchByExactMass(exact_mass, raw, ppm)

        Call Workbench.StatusMessage($"Search MS ions for [{formula}] exact_mass={exact_mass} with tolerance error {ppm}...")

        Return q
    End Function

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
