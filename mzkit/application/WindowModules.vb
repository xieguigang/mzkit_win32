﻿#Region "Microsoft.VisualBasic::378238dabfb5f9c93c835b6b955842c1, mzkit\src\mzkit\mzkit\application\WindowModules.vb"

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

'   Total Lines: 123
'    Code Lines: 97
' Comment Lines: 0
'   Blank Lines: 26
'     File Size: 4.66 KB


' Class WindowModules
' 
'     Properties: ribbon
' 
'     Constructor: (+1 Overloads) Sub New
'     Sub: initializeVSPanel, OpenFile
' 
' /********************************************************************************/

#End Region

Imports BioNovoGene.mzkit_win32.DockSample
Imports BioNovoGene.mzkit_win32.My
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Data.Framework.IO
Imports Microsoft.VisualBasic.Language
Imports Mzkit_win32.BasicMDIForm
Imports Mzkit_win32.BasicMDIForm.RibbonLib.Controls
Imports WeifenLuo.WinFormsUI.Docking

Friend MustInherit Class WindowModules

    Friend Shared ReadOnly viewer As New frmMsImagingViewer
    Friend Shared ReadOnly singleCellViewer As New frmSingleCellViewer

    Friend Shared ReadOnly fileExplorer As New frmFileExplorer
    Friend Shared ReadOnly rawFeaturesList As New frmRawFeaturesList
    Friend Shared ReadOnly UVScansList As New frmUVScans
    Friend Shared ReadOnly spectrumTreeExplorer As New frmTreeExplorer

    Friend Shared ReadOnly output As New OutputWindow
    Friend Shared WithEvents panelMain As New frmDockDocument
    Friend Shared startPage As New frmStartPage
    Friend Shared ReadOnly settingsPage As New frmSettings
    Friend Shared ReadOnly RtermPage As New frmRsharp
    Friend Shared ReadOnly propertyWin As New PropertyWindow
    Friend Shared ReadOnly taskWin As New TaskListWindow
    Friend Shared ReadOnly plotParams As New frmTweaks

    Friend Shared ReadOnly msImageParameters As New frmMsImagingTweaks
    Friend Shared ReadOnly singleCellsParameters As New frmSingleCellsTweaks

    Friend Shared ReadOnly msDemo As New frmDemo
    Friend Shared ReadOnly MRMIons As New frmSRMIonsExplorer
    Friend Shared ReadOnly GCMSPeaks As New frmGCMSPeaks
    Friend Shared ReadOnly parametersTool As New AdjustParameters
    Friend Shared ReadOnly MSIPixelProperty As New MSIPixelPropertyWindow
    Friend Shared ReadOnly nmrSpectrums As New frmNmrSpectrumExplorer

    Public Shared ReadOnly Property ribbon As RibbonItems
        Get
            Return RibbonEvents.ribbonItems
        End Get
    End Property

    Private Sub New()
    End Sub

    Public Shared Sub initializeVSPanel()
        Dim dockPanel As DockPanel = MyApplication.host.m_dockPanel

        output.Show(dockPanel)
        MyApplication.RegisterOutput(output)
        fileExplorer.Show(dockPanel)

        UVScansList.Show(dockPanel)
        UVScansList.DockState = DockState.Hidden

        spectrumTreeExplorer.Show(dockPanel)
        spectrumTreeExplorer.DockState = DockState.Hidden

        plotParams.Show(dockPanel)
        plotParams.DockState = DockState.Hidden

        rawFeaturesList.Show(dockPanel)
        propertyWin.Show(dockPanel)

        startPage.Show(dockPanel)
        startPage.DockState = DockState.Document

        panelMain.Show(dockPanel)
        panelMain.DockState = DockState.Document

        settingsPage.Show(dockPanel)
        settingsPage.DockState = DockState.Hidden

        GCMSPeaks.Show(dockPanel)
        GCMSPeaks.DockState = DockState.Hidden

        nmrSpectrums.Show(dockPanel)
        nmrSpectrums.DockState = DockState.Hidden

        MRMIons.Show(dockPanel)
        MRMIons.DockState = DockState.Hidden

        RtermPage.Show(dockPanel)
        RtermPage.DockState = DockState.Hidden

        taskWin.Show(dockPanel)
        taskWin.DockState = DockState.DockBottomAutoHide

        msImageParameters.Show(dockPanel)
        msImageParameters.DockState = DockState.Hidden

        parametersTool.Show(dockPanel)
        parametersTool.DockState = DockState.Hidden

        msDemo.Show(dockPanel)
        msDemo.DockState = DockState.Hidden

        MSIPixelProperty.Show(dockPanel)
        MSIPixelProperty.DockState = DockState.Hidden

        If Globals.Settings.ui.rememberLayouts Then
            fileExplorer.DockState = Globals.Settings.ui.fileExplorerDock
            rawFeaturesList.DockState = Globals.Settings.ui.featureListDock
            output.DockState = Globals.Settings.ui.OutputDock
            propertyWin.DockState = Globals.Settings.ui.propertyWindowDock
        Else
            fileExplorer.DockState = DockState.DockLeftAutoHide
            rawFeaturesList.DockState = DockState.DockLeftAutoHide
            output.DockState = DockState.DockBottomAutoHide
            propertyWin.DockState = DockState.DockRightAutoHide
        End If
    End Sub

    Public Shared Sub OpenFile()
        Dim doc As IDockContent = MyApplication.host.DockPanel.ActiveDocument

        If TypeOf doc Is DocumentWindow AndAlso DirectCast(doc, DocumentWindow).HookOpen IsNot Nothing Then
            Call DirectCast(doc, DocumentWindow).HookOpen()
        Else
            Call openFileDefault()
        End If
    End Sub

    ''' <summary>
    ''' default handler for open file button on main ribbon menu
    ''' </summary>
    Private Shared Sub openFileDefault()
        Dim filters As String() = {
            "All Raw Data Files(*.mzXML;*.mzML;*.mzPack;*.imzML;*.cdf;*.netcdf;*.raw;*.wiff)|*.mzXML;*.mzML;*.mzPack;*.imzML;*.cdf;*.netcdf;*.raw;*.wiff",
            "Untargetted Raw Data(*.mzXML;*.mzML;*.mzPack)|*.mzXML;*.mzML;*.mzPack",
            "Image mzML(*.imzML)|*.imzML",
            "GC-MS Targeted(*.cdf)|*.cdf;*.netcdf",
            "GC-MS / LC-MS/MS Targeted(*.mzML)|*.mzML",
            "NMR data(*.nmrML)|*.nmrML",
            "msiPL Dataset(*.h5)|*.h5",
            "Thermo Raw File(*.raw)|*.raw",
            "Ab Sciex Wiff(*.wiff)|*.wiff",
            "R# Script(*.R)|*.R",
            "Excel Table(*.csv;*.xlsx)|*.csv;*.xlsx",
            "Open Source Spectrum(*.msp;*.mgf)|*.msp;*.mgf",
            "Raman spectroscopy(*.txt)|*.txt",
            "Virtual Pathology Slide(*.tif;*.dzi;*.ndpi)|*.tif;*.dzi;*.ndpi",
            "10x Genomics h5ad file(*.h5ad)|*.h5ad"
        }

        Using file As New OpenFileDialog With {
            .Filter = filters.JoinBy("|")
        }
            If file.ShowDialog = DialogResult.OK Then
                Call MyApplication.host.OpenFile(file.FileName, showDocument:=True)
            End If
        End Using
    End Sub

    Public Shared Sub ShowTable(Of T As {INamedValue, DynamicPropertyBase(Of String)})(table As IEnumerable(Of T), title As String)
        Call ShowTable(DataFrameResolver.CreateObject(table.ToCsvDoc), title)
    End Sub

    Public Shared Sub ShowTable(dataframe As DataFrameResolver, title As String)
        Call VisualStudio _
            .ShowDocument(Of frmTableViewer)(title:=title) _
            .LoadTable(Sub(grid)
                           Call loadInternal(grid, dataframe)
                       End Sub)
    End Sub

    Private Shared Sub loadInternal(grid As DataTable, dataframe As DataFrameResolver)
        Dim numericFields As Index(Of String) = {"mz", "rt", "rtmin", "rtmax", "mzmin", "mzmax"}
        Dim schema As New List(Of Type)
        Dim i As i32 = Scan0

        For Each name As String In dataframe.HeadTitles
            'If name Like numericFields Then
            '    grid.Columns.Add(name, GetType(Double))
            'Else

            ' End If
            Dim v As String() = dataframe.Column(++i).ToArray
            Dim type As Type = v.SampleForType

            Call schema.Add(type)
            grid.Columns.Add(name, type)
        Next

        For Each item As RowObject In dataframe.Rows
            Dim values = item _
              .Select(Function(str, idx)
                          Select Case schema(idx)
                              Case GetType(Double) : Return Val(str)
                              Case GetType(Integer) : Return str.ParseInteger
                              Case GetType(Boolean) : Return str.ParseBoolean
                              Case GetType(Date) : Return str.ParseDate
                              Case Else
                                  Return CObj(str)
                          End Select
                      End Function) _
              .ToArray
            Dim row = grid.Rows.Add(values)
        Next
    End Sub
End Class
