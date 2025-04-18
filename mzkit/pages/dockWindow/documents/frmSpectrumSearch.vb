﻿#Region "Microsoft.VisualBasic::62feaec406808c4fef9fbe2e53a492e9, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmSpectrumSearch.vb"

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

'   Total Lines: 12
'    Code Lines: 9
' Comment Lines: 0
'   Blank Lines: 3
'     File Size: 362.00 B


' Class frmSpectrumSearch
' 
'     Sub: frmSpectrumSearch_Load
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.mzData.mzWebCache
Imports BioNovoGene.Analytical.MassSpectrometry.Math.Spectra
Imports Mzkit_win32.BasicMDIForm

Public Class frmSpectrumSearch : Implements SpectrumSearchPage

    Friend ReadOnly page As New PageSpectrumSearch With {.Text = "Spectrum Similarity Search"}

    Dim loadMs2Success As Boolean = False

    Public Sub LoadMs2(ms2 As Object) Implements SpectrumSearchPage.LoadMs2
        loadMs2Success = True

        If ms2 Is Nothing Then
            loadMs2Success = False
            Return
        End If

        If TypeOf ms2 Is PeakMs2 Then
            Call page.loadMs2(DirectCast(ms2, PeakMs2).mzInto, DirectCast(ms2, PeakMs2).lib_guid)
        ElseIf TypeOf ms2 Is LibraryMatrix Then
            Call page.loadMs2(DirectCast(ms2, LibraryMatrix).ms2, DirectCast(ms2, LibraryMatrix).name)
        ElseIf TypeOf ms2 Is ms2() Then
            Call page.loadMs2(DirectCast(ms2, ms2()))
        ElseIf TypeOf ms2 Is ScanMS2 Then
            Call page.loadMs2(DirectCast(ms2, ScanMS2).GetMs, name:=DirectCast(ms2, ScanMS2).scan_id)
        Else
            loadMs2Success = False
            Call Workbench.Warning($"load spectrum matrix data from clr object '{ms2.GetType.FullName}' has not yet implemented!")
        End If
    End Sub

    Public Sub RunSearch(Optional showUi As Boolean = True) Implements SpectrumSearchPage.RunSearch
        If loadMs2Success Then
            Call page.runSearch(, showUI:=showUi)
        Else
            Call Workbench.Warning("the spectrum data apply for run query must be loaded at first!")
        End If
    End Sub

    Private Sub frmSpectrumSearch_Load(sender As Object, e As EventArgs) Handles Me.Load
        Controls.Add(page)

        page.Dock = DockStyle.Fill
        TabText = "Spectrum Similarity Search"
        Text = TabText
    End Sub
End Class
