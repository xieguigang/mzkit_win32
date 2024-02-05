#Region "Microsoft.VisualBasic::0fa80d0731d1dca00d57282e698e0885, mzkit\src\mzkit\mzkit\pages\dockWindow\documents\frmGCxGCViewer.vb"

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

'   Total Lines: 3
'    Code Lines: 2
' Comment Lines: 0
'   Blank Lines: 1
'     File Size: 40.00 B


' Class frmGCxGCViewer
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly
Imports BioNovoGene.Analytical.MassSpectrometry.Assembly.Comprehensive
Imports Mzkit_win32.BasicMDIForm
Imports RibbonLib.Interop

Public Class frmGCxGCViewer

    Dim rawdata As mzPack

    Private Sub frmGCxGCViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        TabText = "GCxGC 2D Imaging Viewer"
        ribbonItems.MenuGroupGCxGC.ContextAvailable = ContextAvailability.Available

        AddHandler ribbonItems.ButtonOpenGCxGCFile.ExecuteEvent, Sub() Call openRawdata()
    End Sub

    Private Sub openRawdata()
        Using file As New OpenFileDialog With {.Filter = "GCxGC mzPack(*.mzpack)|*.mzpack"}
            If file.ShowDialog = DialogResult.OK Then
                Call ProgressSpinner.DoLoading(
                    Sub()
                        Using s As Stream = file.FileName.Open(FileMode.Open, doClear:=False, [readOnly]:=True)
                            Call Me.Invoke(Sub() rawdata = mzPack.ReadAll(s))
                            Call Me.Invoke(Sub() PeakSelector1.SetScans(rawdata.ExtractTIC.ToArray, getModtime))
                        End Using
                    End Sub)
            End If
        End Using
    End Sub

    Private Function getModtime() As Double
        Return Val(rawdata.metadata.TryGetValue("modtime", [default]:="4.0"))
    End Function

    Private Sub frmGCxGCViewer_Activated() Handles Me.Activated, Me.GotFocus
        HookOpen = AddressOf openRawdata
        ribbonItems.MenuGroupGCxGC.ContextAvailable = ContextAvailability.Active
    End Sub

    'Private Sub frmGCxGCViewer_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate, Me.LostFocus
    '    HookOpen = Nothing
    '    ribbonItems.MenuGroupGCxGC.ContextAvailable = ContextAvailability.NotAvailable
    'End Sub
End Class
